namespace MyNotes1.Contracts
{
    public record GetNotesResponse(List<NoteDto> notes);

    public record NoteDto(Guid Id, string Title, string Description, DateTime CreatedAt);
}
