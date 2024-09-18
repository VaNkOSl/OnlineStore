namespace OnlineStore.Web.ViewModels.Admin;

using System.ComponentModel.DataAnnotations;
public class SizeFormModel
{
    public int Id { get; set; }

    [Required]
    [Display(Name = "Enter size name")]
    public string Name { get; set; } = string.Empty;
}
