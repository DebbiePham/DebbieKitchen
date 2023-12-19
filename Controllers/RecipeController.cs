using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using DebbieKitchen.Models;
using Microsoft.EntityFrameworkCore;

namespace DebbieKitchen.Controllers;

[SessionCheck]
public class RecipeController : Controller
{
    private readonly ILogger<RecipeController> _logger;

    private MyContext _context;

    public RecipeController(ILogger<RecipeController> logger, MyContext context)
    {
        _logger = logger;
        _context = context;
    }



    // RECIPE SESSION
    [SessionCheck]
    [HttpGet("debbiekitchen/admin/recipes")]
    public IActionResult Recipes(string searchInput)
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

        return View("Recipes",retVal);
    }

    [SessionCheck]
    [HttpPost("debbiekitchen/admin/recipe/search")]
    public IActionResult Search(string searchInput)
    {
        return Recipes(searchInput);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    // page show create new recipe form
    [SessionCheck]
    [HttpGet("debbiekitchen/admin/recipes/new")]
    public ViewResult NewRecipe()
    {
        return View("NewRecipe");
    }


    // Add category session
    [HttpPost("debbiekitchen/admin/recipes/new/add/category")]
    public IActionResult AddCategory (int recipeId, int categoryId )
    {
        if(recipeId == 0){
            TempData["alertMessage"] = "<p class='text-danger' >Please choose a category.</p>";
            return RedirectToAction("NewRecipe", new {id = HttpContext.Session.GetInt32("RecipeId") });
        }
        // Category retrivedCategory = _context.Categories.FirstOrDefault(c => c.CategoryId == recipeId);
        // Recipe currentRecipe = _context.Recipes.FirstOrDefault(p => p.RecipeId == HttpContext.Session.GetInt32("RecipeId"));
        Association newAssociation = new();
        newAssociation.CategoryId = categoryId;
        newAssociation.RecipeId = recipeId;
        newAssociation.AdminId = (int)HttpContext.Session.GetInt32("AdminId");

        _context.Associations.Add(newAssociation);
        _context.SaveChanges();
        return RedirectToAction("ShowRecipe", new {recipeId});
    }

    // create new recipe
    [SessionCheck]
    [HttpPost("debbiekitchen/admin/recipes/create")]
    public IActionResult CreateRecipe (Recipe newRecipe)
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
            return View("NewRecipe");
        }
        newRecipe.AdminId = (int)HttpContext.Session.GetInt32("AdminId");
        _context.Add(newRecipe);
        _context.SaveChanges();
        return RedirectToAction("Recipes");
    }

    // show one recipe detail
    [HttpGet("debbiekitchen/admin/recipes/{recipeId}")]
    public IActionResult ShowRecipe(int recipeId)
    {
        RecipeViewModel viewModel = new ()
        {
            Recipe = _context.Recipes
                .Include(ul => ul.UserLikes)
                .Include(uc => uc.UserComments)
                .Include(a => a.AssociatedCategories)
                .Include(c => c.AllCategories)
                .FirstOrDefault(r => r.RecipeId == recipeId),

            AllCategories = _context.Categories.ToList()
        };
            
        if(viewModel == null)
        {
            return RedirectToAction("Recipes");
        }
        return View(viewModel);
    }


    // delete the association category
    [SessionCheck]
    [HttpPost("debbiekitchen/admin/recipes/category/{assId}")]
    public IActionResult DeleteAssociation(int assId)
    {
        Association associationToDelete = _context.Associations.SingleOrDefault(c => c.AssociationId == assId);
        _context.Associations.Remove(associationToDelete);
        _context.SaveChanges();
        return Redirect(HttpContext.Request.Headers.Referer);
    }

    // Delete a recipe
    [SessionCheck]
    [HttpPost("debbiekitchen/admin/recipes/{recipeId}/delete")]
    public RedirectToActionResult DeleteRecipe(int recipeId)
    {
        Recipe deleteRecipe = _context.Recipes.SingleOrDefault(p => p.RecipeId == recipeId);
        if(deleteRecipe != null) 
        {
            // if the current user try to delete other users'post, they will be log out
            if(deleteRecipe.AdminId != (int)HttpContext.Session.GetInt32("AdminId"))
            {
                HttpContext.Session.Clear();
                RedirectToAction("Home", "Home");
            }
            _context.Remove(deleteRecipe);
            _context.SaveChanges();
        }
        return RedirectToAction("Recipes");
    }

    // lead to edit form page
    [SessionCheck]
    [HttpGet("debbiekitchen/admin/recipes/{recipeId}/edit")]
    public IActionResult EditRecipe(int recipeId)
    {
        Recipe singleRecipe = _context.Recipes.FirstOrDefault(p => p.RecipeId == recipeId);
        if(singleRecipe == null || singleRecipe.AdminId != (int)HttpContext.Session.GetInt32("AdminId"))
        {
            RedirectToAction("Home", "Home");
        }

        return View("EditRecipe", singleRecipe);
    }

    // process the edit form page
    [SessionCheck]
    [HttpPost("debbiekitchen/admin/recipes/{recipeId}/update")]
    public IActionResult UpdateRecipe(int recipeId, Recipe editedRecipe)
    {
        Recipe retRecipe = _context.Recipes.FirstOrDefault(p => p.RecipeId == recipeId);
        if(!ModelState.IsValid)
        {
            return View("EditPost", editedRecipe);
        }
        retRecipe.RecipeName = editedRecipe.RecipeName;
        retRecipe.Ingredients = editedRecipe.Ingredients;
        retRecipe.Direction  = editedRecipe.Direction ;
        retRecipe.Note  = editedRecipe.Note ;
        retRecipe.PrepTimeInMinutes  = editedRecipe.PrepTimeInMinutes ;
        retRecipe.CookTimeInMinutes  = editedRecipe.CookTimeInMinutes ;
        retRecipe.AdditionalTimeInMinutes  = editedRecipe.AdditionalTimeInMinutes ;
        retRecipe.Serving  = editedRecipe.Serving ;
        retRecipe.RecipeImg  = editedRecipe.RecipeImg ;
        retRecipe.UpdatedAt = editedRecipe.UpdatedAt;
        
        _context.SaveChanges();
        return RedirectToAction("ShowRecipe", new { recipeId });
    }

    


}


