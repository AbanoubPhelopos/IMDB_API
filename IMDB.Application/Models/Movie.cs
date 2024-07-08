using System.Text.RegularExpressions;

namespace IMDB.Application.Models;

public partial class Movie
{
    public required Guid Id { get; init; }
    public required string  Title { get; set; }
    public string Slug => GenrateSlug();
    public required int YearOfRelease { get; set; }
    public required List<string> Genres { get; init; } = new();

    private string GenrateSlug()
    {
        var sluggedTitle = slugRegex().Replace(Title , string.Empty)
            .ToLower().Replace(" ", "-");
        return $"{sluggedTitle}-{YearOfRelease}";
    }
    [GeneratedRegex("[^0-9A-Za-z _-]",RegexOptions.NonBacktracking,5)]
    private static partial Regex slugRegex();
}