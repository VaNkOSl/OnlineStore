namespace OnlineStore.Web.ViewModels.Admin;

using System.ComponentModel.DataAnnotations;
public class ColorFormModel
{
    public int Id { get; set; }

    [Required]
    [Display(Name = "Enter color name")]
    [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "The color name can only contain letters.")]
    public string ColorName { get; set; } = string.Empty;
}
