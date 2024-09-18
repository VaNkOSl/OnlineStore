namespace OnlineStore.Web.ViewModels.Admin;

using System.ComponentModel.DataAnnotations;
using static OnlineStore.Commons.EntityValidationConstraints.Sellers;
public class RejectSellerFormModel
{
    public string SellerId { get; set; } = string.Empty;

    [Required]
    [StringLength(SellerRejectionReasonMaxLength, MinimumLength = SellerRejectionReasonMinLength)]
    [Display(Name = "Reject Reason")]
    public string RejectionReason { get; set; } = string.Empty;
}
