using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using DebbieKitchen.Models;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace DebbieKitchen.Controllers;

public class AdminController : Controller
{
    private readonly ILogger<AdminController> _logger;

    private MyContext _context;

    public AdminController(ILogger<AdminController> logger, MyContext context)
    {
        _logger = logger;
        _context = context;
    }

    // Admin page show the Register and Login forms
    [HttpGet("debbiekitchen/admin/login")]
    public IActionResult AdminLogin()
    {
        return View();
    }

    [HttpPost("admin/create")]   
    public IActionResult CreateAdmin(Admin newAdmin)    
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
            // Initializing a PasswordHasher object, providing our Admin class as its type            
            PasswordHasher<Admin> Hasher = new();   
            // Updating our newAdmin's password to a hashed version         
            newAdmin.Password = Hasher.HashPassword(newAdmin, newAdmin.Password);            
            //Save your admin object to the database 
            _context.Add(newAdmin);
            _context.SaveChanges();
            HttpContext.Session.SetInt32("AdminId", newAdmin.AdminId);
            HttpContext.Session.SetString("AdminName", newAdmin.Name);
            return RedirectToAction("Dashboard"); 
            
        } else {
            return View("AdminLogin");
        }   
    }

    [HttpPost("admin/login")]
    public IActionResult LoginAdmin(LoginAdmin loginAdmin)
    {    
        if(ModelState.IsValid)    
        {        
            // If initial ModelState is valid, query for a admin with the provided email        
            Admin adminInDb = _context.Admins.FirstOrDefault(u => u.Email == loginAdmin.LogEmail);        
            // If no admin exists with the provided email        
            if(adminInDb == null)        
            {            
                // Add an error to ModelState and return to View!            
                ModelState.AddModelError("LogEmail", "Invalid Email/Password");            
                return View("AdminLogin");        
            }   
            // Otherwise, we have a admin, now we need to check their password                 
            // Initialize hasher object        
            PasswordHasher<LoginAdmin> hasher = new();                    
            // Verify provided password against hash stored in db        
            var result = hasher.VerifyHashedPassword(loginAdmin, adminInDb.Password, loginAdmin.LogPassword);        
            if(result == 0)        
            {            
                ModelState.AddModelError("LogEmail", "Invalid Email/Password");            
                return View("AdminLogin");       
            } else {
                HttpContext.Session.SetInt32("AdminId", adminInDb.AdminId);
                HttpContext.Session.SetString("AdminName", adminInDb.Name);
                return RedirectToAction("Dashboard");
            }
        } else {
            return View("AdminLogin");
        }
    }


    // Log out function
    [HttpGet("logout")]
    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        return RedirectToAction("AdminLogin");
    }

    [SessionCheck]
    [HttpGet("debbiekitchen/admin/home")]
    public IActionResult Dashboard()
    {
        int currentAdminId = (int)HttpContext.Session.GetInt32("AdminId");
        Admin currentAdmin = _context.Admins
            .Include(r => r.AllRecipes)
            .Include(l => l.UserLikes)
            .Include(c => c.AllCategories)
            .FirstOrDefault(u => u.AdminId == currentAdminId);
        
        if(currentAdmin == null)
        {
            HttpContext.Session.Clear();
            RedirectToAction("AdminLogin");
        }
        return View("Dashboard", currentAdmin);
    }

}


// Name this anything you want with the word "Attribute" at the end
public class SessionCheckAttribute : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        // Find the session, but remember it may be null so we need int?
        int? adminId = context.HttpContext.Session.GetInt32("AdminId");
        // Check to see if we got back null
        if(adminId == null)
        {
            // Redirect to the Index page if there was nothing in session
            // "Home" here is referring to "HomeController", you can use any controller that is appropriate here
            context.Result = new RedirectToActionResult("Index", "Admin", null);
        }
    }
}