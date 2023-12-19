using System.ComponentModel.DataAnnotations;

namespace DebbieKitchen.Models;


public class UserComment
{
    [Key]
    public int UserCommentId {get; set;}

    [Required]
    [MinLength(2)]
    public string Content {get;set;}

    [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MMMM dd, yyyy}")]
    public DateTime CreatedAt {get;set;} = DateTime.Now;
    public DateTime UpdateAt {get;set;} = DateTime.Now;

    //fk
    public int UserId {get;set;}
    public int RecipeId {get;set;}
    

    // Nav prop
    public User Commenter {get; set;}
    public Recipe CommentedRecipe {get;set;}
}