using Chi.ExpenseTracker.Api.Models.Parameters;
using FluentValidation;

namespace Chi.ExpenseTracker.Api.Validators;

/// <summary>
/// 新增／更新收支紀錄參數驗證。
/// </summary>
public class SaveTransactionParameterValidator : AbstractValidator<SaveTransactionParameter>
{
    /// <summary>
    /// 建立驗證規則。
    /// </summary>
    public SaveTransactionParameterValidator()
    {
        RuleFor(parameter => parameter.CategoryId)
            .GreaterThan(0).WithMessage("請選擇類別");

        RuleFor(parameter => parameter.Amount)
            .GreaterThan(0).WithMessage("金額必須大於 0");

        RuleFor(parameter => parameter.CreateDate)
            .NotEmpty().WithMessage("請輸入交易日期");

        RuleFor(parameter => parameter.Description)
            .MaximumLength(250).WithMessage("備註不可超過 250 字");
    }
}
