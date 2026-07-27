using LibraryManagement.Core.DTOs;
using LibraryManagement.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
namespace LibraryManagement.Controllers;

/// <summary>
/// Manages book loan records.
/// </summary>
/// 
[Authorize]
[Route("api/[controller]")]
[ApiController]
public class LoansController : ControllerBase
{
    private readonly ILoanService _loanService;

    public LoansController(ILoanService loanService)
    {
        _loanService = loanService;
    }

    /// <summary>
    /// Returns a paginated and sorted list of loans.
    /// </summary>
    /// <param name="query">Pagination and sorting parameters.</param>
    /// 
    [Authorize(Roles = "User,Admin")]
    [HttpGet]
    [ProducesResponseType(
        typeof(PagedResponseDto<LoanResponseDto>),
        StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResponseDto<LoanResponseDto>>>
        GetAll([FromQuery] ListQueryDto query)
    {
        var loans = await _loanService.GetPagedAsync(query);

        return Ok(loans);
    }

    /// <summary>
    /// Returns a loan by identifier.
    /// </summary>
    /// <param name="id">Loan identifier.</param>
    /// 
    [Authorize(Roles = "User,Admin")]
    [HttpGet("{id:int}")]
    [ProducesResponseType(
        typeof(LoanResponseDto),
        StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<LoanResponseDto>> GetById(int id)
    {
        var loan = await _loanService.GetByIdAsync(id);

        if (loan is null)
        {
            return NotFound();
        }

        return Ok(loan);
    }

    /// <summary>
    /// Creates a new book loan.
    /// </summary>
    /// <param name="dto">Information for the new loan.</param>
    /// 
    [Authorize(Roles = "Admin")]
    [HttpPost]
    [ProducesResponseType(
        typeof(LoanResponseDto),
        StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<LoanResponseDto>>
        Create(LoanCreateDto dto)
    {
        var loan = await _loanService.CreateAsync(dto);

        return CreatedAtAction(
            nameof(GetById),
            new { id = loan.Id },
            loan);
    }

    /// <summary>
    /// Updates an existing book loan.
    /// </summary>
    /// <param name="id">Loan identifier.</param>
    /// <param name="dto">Updated loan information.</param>
    /// 
    [Authorize(Roles = "Admin")]
    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult>
        Update(int id, LoanUpdateDto dto)
    {
        var updated = await _loanService.UpdateAsync(id, dto);

        if (!updated)
        {
            return NotFound();
        }

        return NoContent();
    }

    /// <summary>
    /// Deletes a loan.
    /// </summary>
    /// <param name="id">Loan identifier.</param>
    /// 
    [Authorize(Roles = "Admin")]
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _loanService.DeleteAsync(id);

        if (!deleted)
        {
            return NotFound();
        }

        return NoContent();
    }
}