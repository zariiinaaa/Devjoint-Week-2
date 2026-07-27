using LibraryManagement.Core.DTOs;
using LibraryManagement.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
namespace LibraryManagement.Controllers;

/// <summary>
/// Manages author records.
/// </summary>
/// 
[Authorize]
[Route("api/[controller]")]
[ApiController]
public class AuthorsController : ControllerBase
{
    private readonly IAuthorService _authorService;

    public AuthorsController(IAuthorService authorService)
    {
        _authorService = authorService;
    }

    /// <summary>
    /// Returns a paginated and sorted list of authors.
    /// </summary>
    /// <param name="query">Pagination and sorting parameters.</param>
    /// 
    [Authorize(Roles = "User,Admin")]
    
    [HttpGet]
    [ProducesResponseType(
        typeof(PagedResponseDto<AuthorResponseDto>),
        StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResponseDto<AuthorResponseDto>>>
        GetAll([FromQuery] ListQueryDto query)
    {
        var authors = await _authorService.GetPagedAsync(query);

        return Ok(authors);
    }

    /// <summary>
    /// Returns an author by identifier.
    /// </summary>
    /// <param name="id">Author identifier.</param>
    /// 
    [Authorize(Roles = "User,Admin")]
    [HttpGet("{id:int}")]
  
    [ProducesResponseType(
        typeof(AuthorResponseDto),
        StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<AuthorResponseDto>> GetById(int id)
    {
        var author = await _authorService.GetByIdAsync(id);

        if (author is null)
        {
            return NotFound();
        }

        return Ok(author);
    }

    /// <summary>
    /// Creates a new author.
    /// </summary>
    /// <param name="dto">Information for the new author.</param>
    /// 
    [Authorize(Roles = "Admin")]
    
    [HttpPost]
    [ProducesResponseType(
        typeof(AuthorResponseDto),
        StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<AuthorResponseDto>>
        Create(AuthorCreateDto dto)
    {
        var author = await _authorService.CreateAsync(dto);

        return CreatedAtAction(
            nameof(GetById),
            new { id = author.Id },
            author);
    }

    /// <summary>
    /// Updates an existing author.
    /// </summary>
    /// <param name="id">Author identifier.</param>
    /// <param name="dto">Updated author information.</param>
    /// 

    [Authorize(Roles = "Admin")]
   
    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult>
        Update(int id, AuthorUpdateDto dto)
    {
        var updated = await _authorService.UpdateAsync(id, dto);

        if (!updated)
        {
            return NotFound();
        }

        return NoContent();
    }

    /// <summary>
    /// Deletes an author.
    /// </summary>
    /// <param name="id">Author identifier.</param>
    /// 
    [Authorize(Roles = "Admin")]
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _authorService.DeleteAsync(id);

        if (!deleted)
        {
            return NotFound();
        }

        return NoContent();
    }
}