namespace OnlineStore.Data.Models;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static OnlineStore.Commons.EntityValidationConstraints.Reviews;
public class Review
{
    public Review()
    {
        Id = Guid.NewGuid();
    }

    [Key]
    public Guid Id { get; set; }

    [Required]
    [MaxLength(ReviewContentMaxLength)]
    public string Content { get; set; } = string.Empty;

    [Required]
    [Range(ReviewRatingMinValue, ReviewRatingMaxValue)]
    public int Rating { get; set; }

    [Required]
    public Guid UserId { get; set; }

    [ForeignKey(nameof(UserId))]
    public virtual ApplicationUser User { get; set; } = null!;
    public DateTime ReviewDate { get; set; }

    [Required]
    public Guid ProductId { get; set; }

    [ForeignKey(nameof(ProductId))]
    public virtual Product Product { get; set; } = null!;
}
