using System.ComponentModel.DataAnnotations;

namespace DebbieKitchen.Models;


public class UserLike
{
    [Key]
    public int UserLikeId {get; set;}

    [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MMMM dd, yyyy}")]
    public DateTime CreatedAt {get;set;} = DateTime.Now;
    public DateTime UpdateAt {get;set;} = DateTime.Now;

    //fk
    public int RecipeId {get;set;}
    public int UserId {get; set;}
    // Nav prop
    public Recipe LikedRecipe {get;set;}
    public User Likedby {get; set;}
}