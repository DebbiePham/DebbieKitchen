#pragma warning disable CS8618
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization.Infrastructure;
namespace DebbieKitchen.Models;

public class Association
{
    [Key]
    public int AssociationId {get; set;}
    public int RecipeId {get; set;}
    public int CategoryId {get; set;}
    public int AdminId {get;set;}
    public Recipe Recipe {get; set;}
    public Category Category {get; set;}
    public Admin Creator {get; set;}
}