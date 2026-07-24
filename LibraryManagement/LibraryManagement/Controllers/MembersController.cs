using LibraryManagement.Core.DTOs;
using LibraryManagement.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagement.Controllers;

/// <summary>
/// Manages library member records.
/// </summary>
[Route("api/[controller]")]
[ApiController]
public class MembersController : ControllerBase
{
    private readonly IMemberService _memberService;

    public MembersController(IMemberService memberService)
    {
        _memberService = memberService;
    }

    /// <summary>
    /// Returns a paginated and sorted list of members.
    /// </summary>
    /// <param name="query">Pagination and sorting parameters.</param>
    [HttpGet]
    [ProducesResponseType(
        typeof(PagedResponseDto<MemberResponseDto>),
        StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResponseDto<MemberResponseDto>>>
        GetAll([FromQuery] ListQueryDto query)
    {
        var members = await _memberService.GetPagedAsync(query);

        return Ok(members);
    }

    /// <summary>
    /// Returns a member by identifier.
    /// </summary>
    /// <param name="id">Member identifier.</param>
    [HttpGet("{id:int}")]
    [ProducesResponseType(
        typeof(MemberResponseDto),
        StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<MemberResponseDto>> GetById(int id)
    {
        var member = await _memberService.GetByIdAsync(id);

        if (member is null)
        {
            return NotFound();
        }

        return Ok(member);
    }

    /// <summary>
    /// Creates a new library member.
    /// </summary>
    /// <param name="dto">Information for the new member.</param>
    [HttpPost]
    [ProducesResponseType(
        typeof(MemberResponseDto),
        StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<MemberResponseDto>>
        Create(MemberCreateDto dto)
    {
        var member = await _memberService.CreateAsync(dto);

        return CreatedAtAction(
            nameof(GetById),
            new { id = member.Id },
            member);
    }

    /// <summary>
    /// Updates an existing member.
    /// </summary>
    /// <param name="id">Member identifier.</param>
    /// <param name="dto">Updated member information.</param>
    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult>
        Update(int id, MemberUpdateDto dto)
    {
        var updated = await _memberService.UpdateAsync(id, dto);

        if (!updated)
        {
            return NotFound();
        }

        return NoContent();
    }

    /// <summary>
    /// Deletes a member.
    /// </summary>
    /// <param name="id">Member identifier.</param>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _memberService.DeleteAsync(id);

        if (!deleted)
        {
            return NotFound();
        }

        return NoContent();
    }
}