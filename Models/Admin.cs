#pragma warning disable CS8618
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace DebbieKitchen.Models;


public class Admin
{
    [Key]
    public int AdminId { get; set; }

    [Required]
    [MinLength(3, ErrorMessage = "Full Name must be at least 3 characters")]
    [DisplayName("Full Name")]
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


    public List<Recipe> AllRecipes { get; set; } = new();
    public List<Category> AllCategories { get; set; } = new();
    public List<UserLike> UserLikes { get; set; } = new();


    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime UpdatedAt { get; set; } = DateTime.Now;

}

public class UniqueEmailAttribute : ValidationAttribute
{
    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
    	// Though we have Required as a validation, sometimes we make it here anyways
    	// In which case we must first verify the value is not null before we proceed
        if(value == null)
        {
    	    // If it was, return the required error
            return new ValidationResult("Email is required!");
        }
    
    	// This will connect us to our database since we are not in our Controller
        MyContext _context = (MyContext)validationContext.GetService(typeof(MyContext));
        // Check to see if there are any records of this email in our database
        if(_context.Admins.Any(e => e.Email == value.ToString()))
        {
    	    // If yes, throw an error
            return new ValidationResult("Email must be unique!");
        } else {
    	    // If no, proceed
            return ValidationResult.Success;
        }
    }
}






