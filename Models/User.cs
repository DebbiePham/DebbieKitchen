#pragma warning disable CS8618
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace DebbieKitchen.Models; 

public class User
{    
    [Key]    
    public int UserId { get; set; }
    
    [Required] 
    [MinLength(3, ErrorMessage = "First Name must be at least 3 characters")]
    [Display(Name = "Username:")]
    public string Name { get; set; }
    
    
    [Required]
    [EmailAddress]
    [UniqueEmail]
    [Display(Name = "Email Address:")]
    public string Email { get; set; }    
    
    [Required]
    [DataType(DataType.Password)]
    [RegularExpression(@"^(?=.*[A-Za-z])(?=.*\d)(?=.*[@$!%*#?&])[A-Za-z\d@$!%*#?&]{8,}$",ErrorMessage="Password must contain at least 1 number, 1 letter, and 1 special character.")]
    [Display(Name = "Password:")]
    public string Password { get; set; } 
    

    [NotMapped]
    [DataType(DataType.Password)]
    [Compare("Password")]
    [Display(Name = "Confirm Password:")]
    public string Confirm { get; set; }   
    
    [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MMMM dd, yyyy}")]
    public DateTime CreatedAt {get;set;} = DateTime.Now;   
    public DateTime UpdatedAt {get;set;} = DateTime.Now;

    // nav properties

    public List<UserLike> LikedRecipes {get; set;} = new();
    public List<SaveRecipe> SavedRecipes {get;set;} = new();
    public List<UserComment> AllComments {get;set;} = new();
}



