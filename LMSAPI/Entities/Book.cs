using System;
using System.Collections.Generic;

namespace LMSAPI.Entities;

public partial class Book
{
    public int Id { get; set; }

    public string? Title { get; set; }

    public string? Author { get; set; }

    public DateTime? YearPublished { get; set; }

    public string? Category { get; set; }

    public int? Quantity { get; set; }

    public bool? DeleteStatus { get; set; }

    public string? SourceOfFund { get; set; }

    public string? Publisher { get; set; }

    public string? Remarks { get; set; }
}
