using LibraryManagement.Core.DTOs;
using LibraryManagement.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagement.Controllers;

/// <summary>
/// Manages book records.
/// </summary>
[Route("api/[controller]")]
[ApiController]
public class BooksController : ControllerBase
{
    private readonly IBookService _bookService;

    public BooksController(IBookService bookService)
    {
        _bookService = bookService;
    }

    /// <summary>
    /// Returns a paginated and sorted list of books.
    /// </summary>
    /// <param name="query">Pagination and sorting parameters.</param>
    [HttpGet]
    [ProducesResponseType(
        typeof(PagedResponseDto<BookResponseDto>),
        StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResponseDto<BookResponseDto>>>
        GetAll([FromQuery] ListQueryDto query)
    {
        var books = await _bookService.GetPagedAsync(query);

        return Ok(books);
    }

    /// <summary>
    /// Returns a book by its identifier.
    /// </summary>
    /// <param name="id">Book identifier.</param>
    [HttpGet("{id:int}")]
    [ProducesResponseType(
        typeof(BookResponseDto),
        StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<BookResponseDto>> GetById(int id)
    {
        var book = await _bookService.GetByIdAsync(id);

        if (book is null)
        {
            return NotFound();
        }

        return Ok(book);
    }

    /// <summary>
    /// Creates a new book.
    /// </summary>
    /// <param name="dto">Information for the new book.</param>
    [HttpPost]
    [ProducesResponseType(
        typeof(BookResponseDto),
        StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<BookResponseDto>>
        Create(BookCreateDto dto)
    {
        var createdBook = await _bookService.CreateAsync(dto);

        return CreatedAtAction(
            nameof(GetById),
            new { id = createdBook.Id },
            createdBook);
    }

    /// <summary>
    /// Updates an existing book.
    /// </summary>
    /// <param name="id">Book identifier.</param>
    /// <param name="dto">Updated book information.</param>
    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult>
        Update(int id, BookUpdateDto dto)
    {
        var updated = await _bookService.UpdateAsync(id, dto);

        if (!updated)
        {
            return NotFound();
        }

        return NoContent();
    }

    /// <summary>
    /// Deletes a book.
    /// </summary>
    /// <param name="id">Book identifier.</param>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _bookService.DeleteAsync(id);

        if (!deleted)
        {
            return NotFound();
        }

        return NoContent();
    }
}