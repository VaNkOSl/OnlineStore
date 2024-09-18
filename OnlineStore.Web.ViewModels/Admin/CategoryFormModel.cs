namespace OnlineStore.Web.ViewModels.Admin;

using System.ComponentModel.DataAnnotations;
public class CategoryFormModel
{
    public int Id {  get; set; }
    [Required]
    [Display(Name = "Enter category name")]
    [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "The category name can only contain letters.")]
    public string Name { get; set; } = string.Empty;
}
