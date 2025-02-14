using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyNotes1.Contracts;
using MyNotes1.DataAccess;
using MyNotes1.Models;
using System.Linq.Expressions;

namespace MyNotes1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotesController : ControllerBase
    {
        private readonly NotesDbContext _dbContext;

        public NotesController(NotesDbContext dbcontext)
        {
            _dbContext = dbcontext;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateNoteRequest request, CancellationToken ct)
        {
            var note = new Note(request.Title, request.Description);

            await _dbContext.Notes.AddAsync(note, ct);
            await _dbContext.SaveChangesAsync(ct);

            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] GetNotesRequest request, CancellationToken ct)
        {
            var notesQuery = _dbContext.Notes
                        .Where(n => string.IsNullOrWhiteSpace(request.Search) ||
                            n.Title.ToLower().Contains(request.Search.ToLower()));

            Expression<Func<Note, object>> selectorKey = request.SortItem?.ToLower() switch
            {
                "date" => note => note.CreatedAt,
                "title" => note => note.Title,
                _ => note => note.Id
            };

            notesQuery = request.SortOrder == "desc"
                ? notesQuery = notesQuery.OrderByDescending(selectorKey)
                : notesQuery = notesQuery.OrderBy(n => n.CreatedAt);

            var noteDtos = await notesQuery
                .Select(n => new NoteDto(n.Id, n.Title, n.Description, n.CreatedAt))
                .ToListAsync(ct);

            return Ok(new GetNotesResponse(noteDtos));
        }

        
    }
}
