using Chi.ExpenseTracker.Repositories.Entities;
using Microsoft.EntityFrameworkCore;

namespace Chi.ExpenseTracker.Repositories.Data;

/// <summary>
/// 記帳系統 EF Core DbContext。
/// </summary>
public partial class ExpenseDbContext : DbContext
{
    public ExpenseDbContext(DbContextOptions<ExpenseDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<CategoryEntity> Categories { get; set; }

    public virtual DbSet<TransactionEntity> Transactions { get; set; }

    public virtual DbSet<UserEntity> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<CategoryEntity>(entity =>
        {
            entity.HasKey(e => e.CategoryId).HasName("PK__Categori__19093A0B0A0969C2");

            entity.Property(e => e.CategoryType).HasDefaultValue("Expense");
            entity.Property(e => e.Icon).HasDefaultValue("");

            entity.HasOne(d => d.User).WithMany(p => p.Categories)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Categorie__Categ__76969D2E");
        });

        modelBuilder.Entity<TransactionEntity>(entity =>
        {
            entity.HasKey(e => e.TransactionId).HasName("PK__Transact__55433A6BCA74B6E2");

            entity.HasOne(d => d.Category).WithMany(p => p.Transactions)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Transacti__Categ__7D439ABD");

            entity.HasOne(d => d.User).WithMany(p => p.Transactions)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Transacti__UserI__7C4F7684");
        });

        modelBuilder.Entity<UserEntity>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__Users__1788CC4C2F1BF495");

            entity.Property(e => e.CreateDate).HasDefaultValueSql("(getdate())");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
