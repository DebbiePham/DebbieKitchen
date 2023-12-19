#pragma warning disable CS8618
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace DebbieKitchen.Models;


public class Category
{
    [Key]
    public int CategoryId { get; set; }

    [Required]
    [MinLength(2, ErrorMessage = "Category Name must be at least 2 characters")]
    [DisplayName("Category Name")]
    public string CategoryName { get; set; }

    [Required]
    [MinLength(10, ErrorMessage = "Description must be at least 10 characters.")]
    [DisplayName("Description")]
    public string Description {get; set;}

    
    [Required]
    [DisplayName("Image")]
    public string CategoryImg {get; set;}


    [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MMMM dd, yyyy}")]
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime UpdatedAt { get; set; } = DateTime.Now;


    public int AdminId {get; set;}
    public Admin Creator {get; set;}


    public List<Association> AssociatedRecipes {get; set;} = new();
}