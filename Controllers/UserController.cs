using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using DebbieKitchen.Models;
using System.Security.Principal;

namespace DebbieKitchen.Controllers;

public class UserController : Controller
{
    private readonly ILogger<UserController> _logger;

    private MyContext _context;

    public UserController(ILogger<UserController> logger, MyContext context)
    {
        _logger = logger;
        _context = context;
    }

    [HttpGet("debbiekitchen/users/register")]
    public IActionResult UserRegister()
    {
        return View();
    }

    [HttpGet("debbiekitchen/users/login")]
    public IActionResult UserLogin()
    {
        return View();
    }

    [HttpPost("debbiekitchen/users/create")]   
    public IActionResult CreateUser(User newUser)    
    {    
        if (!ModelState.IsValid)
        {
            var message = string.Join(" | ", ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage));
            Console.WriteLine(message);
        }
        if(ModelState.IsValid)        
        {
            // Initializing a PasswordHasher object, providing our User class as its type            
            PasswordHasher<User> Hasher = new();   
            // Updating our newUser's password to a hashed version         
            newUser.Password = Hasher.HashPassword(newUser, newUser.Password);            
            //Save your user object to the database 
            _context.Add(newUser);
            _context.SaveChanges();
            HttpContext.Session.SetInt32("UserId", newUser.UserId);
            HttpContext.Session.SetString("UserName", newUser.Name);
            return RedirectToAction("Home", "Home"); 
            
        } else {
            return View("UserRegister");
        }   
    }

    [HttpPost("debbiekitchen/users/login/process")]
    public IActionResult LoginUser(LoginUser loginUser)
    {    
        if(ModelState.IsValid)    
        {        
            // If initial ModelState is valid, query for a user with the provided email        
            User userInDb = _context.Users.FirstOrDefault(u => u.Email == loginUser.LogEmail);        
            // If no user exists with the provided email        
            if(userInDb == null)        
            {            
                // Add an error to ModelState and return to View!            
                ModelState.AddModelError("LogEmail", "Invalid Email/Password");            
                return View("UserLogin");        
            }   
            // Otherwise, we have a user, now we need to check their password                 
            // Initialize hasher object        
            PasswordHasher<LoginUser> hasher = new();                    
            // Verify provided password against hash stored in db        
            var result = hasher.VerifyHashedPassword(loginUser, userInDb.Password, loginUser.LogPassword);        
            if(result == 0)        
            {            
                ModelState.AddModelError("LogEmail", "Invalid Email/Password");            
                return View("UserLogin");       
            } else {
                HttpContext.Session.SetInt32("UserId", userInDb.UserId);
                HttpContext.Session.SetString("UserName", userInDb.Name);
                return RedirectToAction("Home", "Home");
            }
        } else {
            return View("UserLogin");
        }
    }


    // Log out function
    
    [HttpGet("debbiekitchen/users/logout")]
    public IActionResult UserLogout()
    {
        HttpContext.Session.Clear();
        return RedirectToAction("Home", "Home");
    }


    [HttpGet("debbiekitchen/users/profile")]
    public IActionResult UserProfile()
    {
        int? currentUserId = (int?)HttpContext.Session.GetInt32("UserId");
        User currentUser = _context.Users
            .Include(u => u.SavedRecipes)
            .ThenInclude(sr => sr.SavedRecipe)
            .Include(u => u.LikedRecipes)
            .FirstOrDefault(u => u.UserId == currentUserId);

        if(currentUser == null)
        {
            HttpContext.Session.Clear();
            RedirectToAction("UserLogin");
        }
        return View("UserProfile", currentUser);
    }


    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}


