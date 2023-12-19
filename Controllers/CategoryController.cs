
using Microsoft.AspNetCore.Mvc;
using DebbieKitchen.Models;
using Microsoft.EntityFrameworkCore;

namespace DebbieKitchen.Controllers;

[SessionCheck]
public class CategoryController : Controller
{
    private readonly ILogger<CategoryController> _logger;

    private MyContext _context;

    public CategoryController(ILogger<CategoryController> logger, MyContext context)
    {
        _logger = logger;
        _context = context;
    }


    // CATEGORY SESSION

    [SessionCheck]
    [HttpGet("debbiekitchen/admin/categories")]
    public IActionResult Categories()
    {
        List<Category> categoriesFromDb = _context.Categories
            .Include(r => r.AssociatedRecipes)
            .Include(a => a.Creator)
            .OrderByDescending(c => c.CategoryName)
            .ToList();

        return View(categoriesFromDb);
    }

    [SessionCheck]
    [HttpGet("debbiekitchen/admin/categories/new")]
    public ViewResult NewCategory()
    {

        return View();
    }

    // create new category
    [SessionCheck]
    [HttpPost("debbiekitchen/admin/categories/create")]
    public IActionResult CreateCategory(Category newCategory)
    {
        if (!ModelState.IsValid)
        {
            var message = string.Join(" | ", ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage));
            Console.WriteLine(message);
        }
        if(!ModelState.IsValid)
        {
            return View("NewCategory");
        }
        newCategory.AdminId = (int)HttpContext.Session.GetInt32("AdminId");
        _context.Add(newCategory);
        _context.SaveChanges();
        return RedirectToAction("Categories", "Category");
    }

    // show one category detail
    [SessionCheck]
    [HttpGet("debbiekitchen/admin/categories/{categoryId}")]
    public IActionResult ShowCategory(int categoryId)
    {
        Category singleCategory = _context.Categories
            .Include(a => a.AssociatedRecipes)
            .ThenInclude(r => r.Recipe)
            .FirstOrDefault(r => r.CategoryId == categoryId);
        if(singleCategory == null)
        {
            return RedirectToAction("Categories");
        }
        return View("ShowCategory", singleCategory);
    }

    // Delete a category
    [SessionCheck]
    [HttpPost("debbiekitchen/admin/categories/{categoryId}/delete")]
    public RedirectToActionResult DeleteCategory(int categoryId)
    {
        Category deleteCategory = _context.Categories.SingleOrDefault(p => p.CategoryId == categoryId);
        if(deleteCategory != null) 
        {
            
            if(deleteCategory.AdminId != (int)HttpContext.Session.GetInt32("AdminId"))
            {
                HttpContext.Session.Clear();
                RedirectToAction("Home", "Home");
            }
            _context.Remove(deleteCategory);
            _context.SaveChanges();
        }
        return RedirectToAction("Categories");
    }

    // lead to edit form page
    [SessionCheck]
    [HttpGet("debbiekitchen/admin/categories/{categoryId}/edit")]
    public IActionResult EditCategory(int categoryId)
    {
        Category singleCategory = _context.Categories.FirstOrDefault(p => p.CategoryId == categoryId);
        if(singleCategory == null || singleCategory.AdminId != (int)HttpContext.Session.GetInt32("AdminId"))
        {
            RedirectToAction("Home", "Home");
        }

        return View("EditCategory", singleCategory);
    }

    // process the edit form page
    [SessionCheck]
    [HttpPost("debbiekitchen/admin/categories/{categoryId}/update")]
    public IActionResult UpdateCategory(int categoryId, Category editedCategory)
    {
        Category retCategory = _context.Categories.FirstOrDefault(p => p.CategoryId == categoryId);
        if(!ModelState.IsValid)
        {
            return View("EditCategory", editedCategory);
        }
        retCategory.CategoryName = editedCategory.CategoryName;
        retCategory.Description = editedCategory.Description;
        retCategory.CategoryImg  = editedCategory.CategoryImg ;
        retCategory.UpdatedAt = editedCategory.UpdatedAt;
        
        _context.SaveChanges();
        return RedirectToAction("ShowCategory", new { categoryId });
    }
}
