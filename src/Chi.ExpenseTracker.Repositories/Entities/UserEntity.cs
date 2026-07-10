using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Chi.ExpenseTracker.Repositories.Entities;

/// <summary>
/// 使用者資料表實體。
/// </summary>
public partial class UserEntity
{
    [Key]
    public int UserId { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string UserName { get; set; } = null!;

    [StringLength(255)]
    [Unicode(false)]
    public string Password { get; set; } = null!;

    [StringLength(100)]
    [Unicode(false)]
    public string Email { get; set; } = null!;

    [Column(TypeName = "datetime")]
    public DateTime CreateDate { get; set; }

    [StringLength(20)]
    [Unicode(false)]
    public string? Role { get; set; }

    [InverseProperty("User")]
    public virtual ICollection<CategoryEntity> Categories { get; set; } = new List<CategoryEntity>();

    [InverseProperty("User")]
    public virtual ICollection<TransactionEntity> Transactions { get; set; } = new List<TransactionEntity>();
}
