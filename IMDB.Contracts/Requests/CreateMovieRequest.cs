﻿using System.ComponentModel.DataAnnotations;

namespace IMDB.Contracts.Requests;

public class CreateMovieRequest
{
    [Required,MaxLength(50)]
    public required string  Title { get; init; }
    public required int YearOfRelease { get; init; }
    public required IEnumerable<string> Genres { get; init; } = Enumerable.Empty<string>();
}