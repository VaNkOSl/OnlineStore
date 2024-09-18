namespace OnlineStore.Web.ViewModels.Reviews;

public class ProductReviewViewModel
{
    public string Content { get; set; } = string.Empty;
    public int Rating { get; set; }
    public DateTime ReviewDate { get; set; }
    public string ProductId { get; set; } = string.Empty;
    public string UserFullName { get; set; } = string.Empty;
}
