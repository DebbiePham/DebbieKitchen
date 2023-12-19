using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using DebbieKitchen.Models;
using Microsoft.EntityFrameworkCore;

namespace DebbieKitchen.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    private MyContext _context;

    public object MessageBox { get; private set; }

    public HomeController(ILogger<HomeController> logger, MyContext context)
    {
        _logger = logger;
        _context = context;
    }

    [HttpGet("")]
    public IActionResult Index()
    {
        return RedirectToAction("Home");
    }

    [HttpGet("debbiekitchen/home")]
    public IActionResult Home()
    {
        return View("Home");
    }

    [HttpGet("debbiekitchen/recipes")]
    public IActionResult AllRecipes(string searchInput)
    {
        IQueryable<Recipe> allRecipes = _context.Recipes
            .Include(r => r.AssociatedCategories)
            .Include(r => r.UserLikes);

        if(!string.IsNullOrEmpty(searchInput))
        {
            allRecipes = allRecipes
                .Where(recipe => recipe.RecipeName.Contains(searchInput));
        }

        List<Recipe> retVal = allRecipes
            .OrderByDescending(r => r.CreatedAt)
            .ToList();

        return View("AllRecipes", retVal);
        
    }

    [HttpPost("debbiekitchen/recipe/search")]
    public IActionResult SearchRecipe(string searchInput)
    {
        return AllRecipes(searchInput);
    }

    // show one recipe detail
    [HttpGet("debbiekitchen/recipes/{recipeId}")]
    public IActionResult ViewRecipe(int recipeId)
    {
        RecipeViewModel viewModel = new ()
        {
            Recipe = _context.Recipes
                .Include(c => c.UserComments)
                .ThenInclude(uc => uc.Commenter)
                .Include(rs => rs.SavedRecipes)
                .Include(rl => rl.UserLikes)
                .Include(a => a.AssociatedCategories)
                .Include(c => c.AllCategories)
                .FirstOrDefault(r => r.RecipeId == recipeId),

            AllCategories = _context.Categories.ToList()
        };
            
        if(viewModel == null)
        {
            return RedirectToAction("AllRecipes");
        }
        return View(viewModel);
    }


    [HttpGet("debbiekitchen/categories")]
    public IActionResult AllCategories()
    {
        List<Category> categoriesFromDb = _context.Categories
            .Include(r => r.AssociatedRecipes)
            .Include(a => a.Creator)
            .OrderByDescending(c => c.CategoryName)
            .ToList();

        return View(categoriesFromDb);
    }

    // show one category detail
    [HttpGet("debbiekitchen/categories/{categoryId}")]
    public IActionResult ViewCategory(int categoryId)
    {
        Category singleCategory = _context.Categories
            .Include(a => a.AssociatedRecipes)
            .ThenInclude(r => r.Recipe)
            .FirstOrDefault(r => r.CategoryId == categoryId);
        if(singleCategory == null)
        {
            return RedirectToAction("AllCategories");
        }
        return View("ViewCategory", singleCategory);
    }

    
    [HttpPost("debbiekitchen/recipes/{recipeId}/likes")]
    public IActionResult ToggleLike(int recipeId)
    {
        int? userId = (int?)HttpContext.Session.GetInt32("UserId");
        if(userId != null){

            UserLike existingLike = _context.UserLikes.FirstOrDefault(upl => upl.RecipeId == recipeId && upl.UserId == userId);
            if(existingLike == null)
            {
                UserLike newLike = new(){UserId=userId.Value, RecipeId=recipeId};
                _context.Add(newLike);
            }
            else 
            {
                _context.Remove(existingLike);
            }
            _context.SaveChanges();

            
            return Redirect(HttpContext.Request.Headers.Referer);
        }
        
        return RedirectToAction("UserLogin", "User");
    }


    [HttpPost("debbiekitchen/recipes/{recipeId}/save")]
    public IActionResult ToggleSave(int recipeId)
    {
        int? userId = (int?)HttpContext.Session.GetInt32("UserId");
        if(userId != null){

            SaveRecipe existingSave = _context.SavedRecipes.FirstOrDefault(us => us.RecipeId == recipeId && us.UserId == userId);
            if(existingSave == null)
            {
                SaveRecipe newSave = new(){UserId=userId.Value, RecipeId=recipeId};
                _context.Add(newSave);
            }
            else 
            {
                _context.Remove(existingSave);
            }
            _context.SaveChanges();

            
            return Redirect(HttpContext.Request.Headers.Referer);
        }
        
        return RedirectToAction("UserLogin", "User");
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
