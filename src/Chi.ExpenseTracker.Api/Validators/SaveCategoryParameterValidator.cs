using Chi.ExpenseTracker.Api.Models.Parameters;
using FluentValidation;

namespace Chi.ExpenseTracker.Api.Validators;

/// <summary>
/// 新增／更新收支類別參數驗證。
/// </summary>
public class SaveCategoryParameterValidator : AbstractValidator<SaveCategoryParameter>
{
    private static readonly string[] AllowedCategoryTypes = ["Expense", "Income"];

    /// <summary>
    /// 建立驗證規則。
    /// </summary>
    public SaveCategoryParameterValidator()
    {
        RuleFor(parameter => parameter.Title)
            .NotEmpty().WithMessage("請輸入類別名稱")
            .MaximumLength(50).WithMessage("類別名稱不可超過 50 字");

        RuleFor(parameter => parameter.CategoryType)
            .Must(type => AllowedCategoryTypes.Contains(type))
            .When(parameter => string.IsNullOrEmpty(parameter.CategoryType) is false)
            .WithMessage("類別型態僅接受 Expense 或 Income");

        RuleFor(parameter => parameter.Icon)
            .MaximumLength(5).WithMessage("圖示不可超過 5 字");
    }
}
