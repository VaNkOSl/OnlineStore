namespace OnlineStore.Web.ViewModels.Admin;

using System.ComponentModel.DataAnnotations;
using static OnlineStore.Commons.EntityValidationConstraints.Products;

public class RejectProductFormModel
{
    public string ProductId { get; set; } = string.Empty;

    [Required]
    [StringLength(ProductRejectionReasonMaxLength,MinimumLength = ProductRejectionReasonMinLength)]
    [Display(Name = "Rejection Reason")]
    public string RejectionReason { get; set; } = string.Empty;
}
