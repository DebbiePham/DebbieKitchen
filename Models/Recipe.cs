#pragma warning disable CS8618
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DebbieKitchen.Models;

public class Recipe
{
    [Key]
    public int RecipeId {get;set;}

    [Required]
    [MinLength(2, ErrorMessage = "Recipe's Name must be at least 2 characters")]
    [DisplayName("Recipe's Name")]
    public string RecipeName { get; set; }

    [Required]
    [MinLength(10, ErrorMessage = "Ingredients must be at least 10 characters.")]
    [DisplayName("Ingredients")]
    public string Ingredients {get; set;}

    [Required]
    [MinLength(10, ErrorMessage = "Direction must be at least 10 characters.")]
    [DisplayName("Direction")]
    public string Direction {get; set;}

    [Required]
    [MinLength(10, ErrorMessage = "Note must be at least 10 characters.")]
    [DisplayName("Additional Note")]
    public string Note {get; set;}

    [Required]
    [Range(5, 600)]
    [DisplayName("Prep Time")]
    public int PrepTimeInMinutes {get; set;}

    [Required]
    [Range(5, 600)]
    [DisplayName("Cook Time")]
    public int CookTimeInMinutes {get; set;}

    [Required]
    [Range(5, 600)]
    [DisplayName("Additional Time")]
    public int AdditionalTimeInMinutes {get; set;}

    [Required]
    [Range(2, 20)]
    public int Serving {get;set;}

    [Required]
    [DisplayName("Recipe Image")]
    public string RecipeImg {get; set;}

    [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MMMM dd, yyyy}")]
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime UpdatedAt { get; set; } = DateTime.Now;

    
    public int AdminId {get; set;}
    public Admin Creator {get; set;}

    public List<Association> AssociatedCategories {get; set;} = new();
    public List<UserLike> UserLikes { get; set; } = new();
    public List<Category> AllCategories {get; set;} = new();
    public List<UserComment> UserComments {get;set;} = new();
    public List<User> Users {get;set;} = new();
    public List<SaveRecipe> SavedRecipes {get;set;} = new();
}