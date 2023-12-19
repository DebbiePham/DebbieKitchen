#pragma warning disable CS8618
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DebbieKitchen.Models;

public class RecipeViewModel
{

    public Recipe Recipe { get; set; } = new();
    public List<Category> AllCategories {get; set;} = new();
    public List<UserComment> AllComments {get;set;} = new();
}