using AutoMapper;
using Chi.ExpenseTracker.Api.Models.Parameters;
using Chi.ExpenseTracker.Api.Models.ViewModels;
using Chi.ExpenseTracker.Common.Models.InfoModels;
using Chi.ExpenseTracker.Services.Categories;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Chi.ExpenseTracker.Api.Controllers;

/// <summary>
/// 收支類別維護 API。所有操作皆以目前登入使用者為範圍。
/// </summary>
[Route("api/[controller]")]
[Authorize]
public class CategoriesController : ApiControllerBase
{
    private readonly ICategoryService _categoryService;
    private readonly IMapper _mapper;
    private readonly IValidator<SaveCategoryParameter> _saveValidator;

    /// <summary>
    /// 建立類別 Controller。
    /// </summary>
    public CategoriesController(
        ICategoryService categoryService,
        IMapper mapper,
        IValidator<SaveCategoryParameter> saveValidator)
    {
        _categoryService = categoryService;
        _mapper = mapper;
        _saveValidator = saveValidator;
    }

    /// <summary>
    /// 取得目前使用者的類別清單，可依類別型態篩選。
    /// </summary>
    /// <param name="type">類別型態（Expense／Income），不填則回傳全部。</param>
    /// <param name="cancellationToken">取消權杖。</param>
    /// <returns>類別清單。</returns>
    /// <response code="200">查詢成功。</response>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<CategoryViewModel>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<CategoryViewModel>>> GetCategoriesAsync(
        [FromQuery] string? type,
        CancellationToken cancellationToken)
    {
        var dtos = await _categoryService.GetCategoriesAsync(CurrentUserId, type, cancellationToken);
        return Ok(_mapper.Map<IReadOnlyList<CategoryViewModel>>(dtos));
    }

    /// <summary>
    /// 新增類別。
    /// </summary>
    /// <param name="parameter">新增類別參數。</param>
    /// <param name="cancellationToken">取消權杖。</param>
    /// <returns>建立後的類別。</returns>
    /// <response code="201">建立成功。</response>
    /// <response code="400">參數驗證失敗。</response>
    [HttpPost]
    [ProducesResponseType(typeof(CategoryViewModel), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<CategoryViewModel>> CreateCategoryAsync(
        [FromBody] SaveCategoryParameter parameter,
        CancellationToken cancellationToken)
    {
        var validation = await _saveValidator.ValidateAsync(parameter, cancellationToken);
        if (validation.IsValid is false)
        {
            return ValidationProblemFrom(validation);
        }

        var info = _mapper.Map<SaveCategoryInfo>(parameter);
        info.UserId = CurrentUserId;
        var dto = await _categoryService.CreateCategoryAsync(info, cancellationToken);
        var viewModel = _mapper.Map<CategoryViewModel>(dto);

        return Created($"/api/categories/{viewModel.CategoryId}", viewModel);
    }

    /// <summary>
    /// 更新類別。
    /// </summary>
    /// <param name="categoryId">類別 ID。</param>
    /// <param name="parameter">更新類別參數。</param>
    /// <param name="cancellationToken">取消權杖。</param>
    /// <returns>更新後的類別。</returns>
    /// <response code="200">更新成功。</response>
    /// <response code="400">參數驗證失敗。</response>
    /// <response code="404">類別不存在或不屬於目前使用者。</response>
    [HttpPut("{categoryId:int}")]
    [ProducesResponseType(typeof(CategoryViewModel), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CategoryViewModel>> UpdateCategoryAsync(
        int categoryId,
        [FromBody] SaveCategoryParameter parameter,
        CancellationToken cancellationToken)
    {
        var validation = await _saveValidator.ValidateAsync(parameter, cancellationToken);
        if (validation.IsValid is false)
        {
            return ValidationProblemFrom(validation);
        }

        var info = _mapper.Map<SaveCategoryInfo>(parameter);
        info.UserId = CurrentUserId;
        info.CategoryId = categoryId;
        var dto = await _categoryService.UpdateCategoryAsync(info, cancellationToken);
        if (dto is null)
        {
            return NotFound();
        }

        return Ok(_mapper.Map<CategoryViewModel>(dto));
    }

    /// <summary>
    /// 刪除類別。
    /// </summary>
    /// <param name="categoryId">類別 ID。</param>
    /// <param name="cancellationToken">取消權杖。</param>
    /// <response code="204">刪除成功。</response>
    /// <response code="404">類別不存在或不屬於目前使用者。</response>
    [HttpDelete("{categoryId:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteCategoryAsync(int categoryId, CancellationToken cancellationToken)
    {
        var deleted = await _categoryService.DeleteCategoryAsync(CurrentUserId, categoryId, cancellationToken);
        if (deleted is false)
        {
            return NotFound();
        }

        return NoContent();
    }
}
