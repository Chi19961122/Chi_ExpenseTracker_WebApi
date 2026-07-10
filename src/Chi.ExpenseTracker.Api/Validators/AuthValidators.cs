using Chi.ExpenseTracker.Api.Models.Parameters;
using FluentValidation;

namespace Chi.ExpenseTracker.Api.Validators;

/// <summary>
/// 登入參數驗證。
/// </summary>
public class LoginParameterValidator : AbstractValidator<LoginParameter>
{
    /// <summary>
    /// 建立驗證規則。
    /// </summary>
    public LoginParameterValidator()
    {
        RuleFor(parameter => parameter.Email)
            .NotEmpty().WithMessage("請輸入電子郵件")
            .EmailAddress().WithMessage("電子郵件格式不正確");

        RuleFor(parameter => parameter.Password)
            .NotEmpty().WithMessage("請輸入密碼");
    }
}

/// <summary>
/// 註冊參數驗證。
/// </summary>
public class RegisterParameterValidator : AbstractValidator<RegisterParameter>
{
    /// <summary>
    /// 建立驗證規則。
    /// </summary>
    public RegisterParameterValidator()
    {
        RuleFor(parameter => parameter.UserName)
            .NotEmpty().WithMessage("請輸入顯示名稱")
            .MaximumLength(50).WithMessage("顯示名稱不可超過 50 字");

        RuleFor(parameter => parameter.Email)
            .NotEmpty().WithMessage("請輸入電子郵件")
            .EmailAddress().WithMessage("電子郵件格式不正確")
            .MaximumLength(100).WithMessage("電子郵件不可超過 100 字");

        RuleFor(parameter => parameter.Password)
            .NotEmpty().WithMessage("請輸入密碼")
            .MinimumLength(6).WithMessage("密碼至少需 6 個字元")
            .MaximumLength(72).WithMessage("密碼不可超過 72 個字元");
    }
}
