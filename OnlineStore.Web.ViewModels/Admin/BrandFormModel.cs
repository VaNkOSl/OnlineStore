namespace OnlineStore.Web.ViewModels.Admin;

using System.ComponentModel.DataAnnotations;
public class BrandFormModel
{
    public int Id { get; set; }

    [Required]
    [Display(Name = "Enter brand name")]
    public string Name { get; set; } = string.Empty;

    [Required]
    public string ImageUrl { get; set; } = string.Empty;
}
