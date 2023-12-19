using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using DebbieKitchen.Models;
using Microsoft.EntityFrameworkCore;
using DebbieKitchen.Controllers;



namespace SocialMedia.Controllers;

[SessionCheck]
public class CommentController : Controller
{
    private readonly ILogger<CommentController> _logger;

    private MyContext _context;

    public CommentController(ILogger<CommentController> logger, MyContext context)
    {
        _logger = logger;
        _context = context;
    }


    [SessionCheck]
    [HttpPost("debbiekitchen/recipes/{recipeId}/comment/create")]
    public IActionResult AddComment(UserComment newComment)
    {
        if(!ModelState.IsValid)
        {
            Recipe singlePost = _context.Recipes
                .Include(r => r.UserLikes)
                .ThenInclude(url => url.Likedby) // theninclude is joining table that we just joiun above 
                .Include(r => r.Creator)
                .Include(p => p.UserComments)
                .ThenInclude(uc => uc.Commenter)
                .FirstOrDefault(r => r.RecipeId == newComment.RecipeId);
            return View("ViewRecipe", singlePost);
        }
        newComment.UserId = (int)HttpContext.Session.GetInt32("UserId");
        _context.Add(newComment);
        _context.SaveChanges();
        return RedirectToAction("ViewRecipe", "Home", new {recipeId = newComment.RecipeId});
    }


    [SessionCheck]
    [HttpPost("debbiekitchen/recipes/{recipeId}/comment/{commentId}/delete")]
    public IActionResult DeleteComment(int commentId)
    {
        UserComment commentInDb = _context.UserComments.FirstOrDefault(upc => upc.UserCommentId == commentId);
        if(commentInDb != null)
        {
            _context.Remove(commentInDb);
            _context.SaveChanges();
        }
        return RedirectToAction("ViewRecipe", "Home", new {recipeId = commentInDb.RecipeId});
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}