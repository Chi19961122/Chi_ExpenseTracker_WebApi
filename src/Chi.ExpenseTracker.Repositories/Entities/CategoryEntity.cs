using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Chi.ExpenseTracker.Repositories.Entities;

/// <summary>
/// 收支類別資料表實體。
/// </summary>
public partial class CategoryEntity
{
    [Key]
    public int CategoryId { get; set; }

    public int UserId { get; set; }

    [StringLength(50)]
    public string Title { get; set; } = null!;

    [StringLength(5)]
    public string? Icon { get; set; }

    [StringLength(10)]
    public string? CategoryType { get; set; }

    [InverseProperty("Category")]
    public virtual ICollection<TransactionEntity> Transactions { get; set; } = new List<TransactionEntity>();

    [ForeignKey("UserId")]
    [InverseProperty("Categories")]
    public virtual UserEntity User { get; set; } = null!;
}
