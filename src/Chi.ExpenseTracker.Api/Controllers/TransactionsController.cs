using AutoMapper;
using Chi.ExpenseTracker.Api.Models.Parameters;
using Chi.ExpenseTracker.Api.Models.ViewModels;
using Chi.ExpenseTracker.Common.Models.InfoModels;
using Chi.ExpenseTracker.Services.Transactions;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Chi.ExpenseTracker.Api.Controllers;

/// <summary>
/// 收支紀錄維護 API。所有操作皆以目前登入使用者為範圍。
/// </summary>
[Route("api/[controller]")]
[Authorize]
public class TransactionsController : ApiControllerBase
{
    private readonly ITransactionService _transactionService;
    private readonly IMapper _mapper;
    private readonly IValidator<SaveTransactionParameter> _saveValidator;

    /// <summary>
    /// 建立收支紀錄 Controller。
    /// </summary>
    public TransactionsController(
        ITransactionService transactionService,
        IMapper mapper,
        IValidator<SaveTransactionParameter> saveValidator)
    {
        _transactionService = transactionService;
        _mapper = mapper;
        _saveValidator = saveValidator;
    }

    /// <summary>
    /// 取得目前使用者的全部收支紀錄（含類別資訊），依交易日期由新到舊排序。
    /// </summary>
    /// <param name="cancellationToken">取消權杖。</param>
    /// <returns>收支紀錄清單。</returns>
    /// <response code="200">查詢成功。</response>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<TransactionViewModel>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<TransactionViewModel>>> GetTransactionsAsync(CancellationToken cancellationToken)
    {
        var dtos = await _transactionService.GetTransactionsAsync(CurrentUserId, cancellationToken);
        return Ok(_mapper.Map<IReadOnlyList<TransactionViewModel>>(dtos));
    }

    /// <summary>
    /// 取得單筆收支紀錄。
    /// </summary>
    /// <param name="transactionId">收支紀錄 ID。</param>
    /// <param name="cancellationToken">取消權杖。</param>
    /// <returns>收支紀錄。</returns>
    /// <response code="200">查詢成功。</response>
    /// <response code="404">紀錄不存在或不屬於目前使用者。</response>
    [HttpGet("{transactionId:int}")]
    [ProducesResponseType(typeof(TransactionViewModel), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TransactionViewModel>> GetTransactionAsync(int transactionId, CancellationToken cancellationToken)
    {
        var dto = await _transactionService.GetTransactionAsync(CurrentUserId, transactionId, cancellationToken);
        if (dto is null)
        {
            return NotFound();
        }

        return Ok(_mapper.Map<TransactionViewModel>(dto));
    }

    /// <summary>
    /// 新增收支紀錄。
    /// </summary>
    /// <param name="parameter">新增收支紀錄參數。</param>
    /// <param name="cancellationToken">取消權杖。</param>
    /// <returns>建立後的收支紀錄。</returns>
    /// <response code="201">建立成功。</response>
    /// <response code="400">參數驗證失敗。</response>
    /// <response code="404">類別不存在或不屬於目前使用者。</response>
    [HttpPost]
    [ProducesResponseType(typeof(TransactionViewModel), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TransactionViewModel>> CreateTransactionAsync(
        [FromBody] SaveTransactionParameter parameter,
        CancellationToken cancellationToken)
    {
        var validation = await _saveValidator.ValidateAsync(parameter, cancellationToken);
        if (validation.IsValid is false)
        {
            return ValidationProblemFrom(validation);
        }

        var info = _mapper.Map<SaveTransactionInfo>(parameter);
        info.UserId = CurrentUserId;
        var dto = await _transactionService.CreateTransactionAsync(info, cancellationToken);
        if (dto is null)
        {
            return NotFound(new ProblemDetails { Title = "類別不存在或不屬於目前使用者", Status = StatusCodes.Status404NotFound });
        }
        var viewModel = _mapper.Map<TransactionViewModel>(dto);

        return Created($"/api/transactions/{viewModel.TransactionId}", viewModel);
    }

    /// <summary>
    /// 更新收支紀錄。
    /// </summary>
    /// <param name="transactionId">收支紀錄 ID。</param>
    /// <param name="parameter">更新收支紀錄參數。</param>
    /// <param name="cancellationToken">取消權杖。</param>
    /// <returns>更新後的收支紀錄。</returns>
    /// <response code="200">更新成功。</response>
    /// <response code="400">參數驗證失敗。</response>
    /// <response code="404">紀錄或類別不存在、或不屬於目前使用者。</response>
    [HttpPut("{transactionId:int}")]
    [ProducesResponseType(typeof(TransactionViewModel), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TransactionViewModel>> UpdateTransactionAsync(
        int transactionId,
        [FromBody] SaveTransactionParameter parameter,
        CancellationToken cancellationToken)
    {
        var validation = await _saveValidator.ValidateAsync(parameter, cancellationToken);
        if (validation.IsValid is false)
        {
            return ValidationProblemFrom(validation);
        }

        var info = _mapper.Map<SaveTransactionInfo>(parameter);
        info.UserId = CurrentUserId;
        info.TransactionId = transactionId;
        var dto = await _transactionService.UpdateTransactionAsync(info, cancellationToken);
        if (dto is null)
        {
            return NotFound();
        }

        return Ok(_mapper.Map<TransactionViewModel>(dto));
    }

    /// <summary>
    /// 刪除收支紀錄。
    /// </summary>
    /// <param name="transactionId">收支紀錄 ID。</param>
    /// <param name="cancellationToken">取消權杖。</param>
    /// <response code="204">刪除成功。</response>
    /// <response code="404">紀錄不存在或不屬於目前使用者。</response>
    [HttpDelete("{transactionId:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteTransactionAsync(int transactionId, CancellationToken cancellationToken)
    {
        var deleted = await _transactionService.DeleteTransactionAsync(CurrentUserId, transactionId, cancellationToken);
        if (deleted is false)
        {
            return NotFound();
        }

        return NoContent();
    }
}
