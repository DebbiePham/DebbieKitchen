#pragma warning disable CS8618
using System.ComponentModel.DataAnnotations;
namespace DebbieKitchen.Models; 

public class LoginUser
{
    
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress]
    [Display(Name = "Email Address:")]
    public string LogEmail { get; set; }  

    
    [Required(ErrorMessage = "Password is required")]
    [DataType(DataType.Password)]
    [Display(Name = "Password:")]
    public string LogPassword { get; set; } 
}