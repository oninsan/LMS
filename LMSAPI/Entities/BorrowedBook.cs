using System;
using System.Collections.Generic;

namespace LMSAPI.Entities;

public partial class BorrowedBook
{
    public int Id { get; set; }

    public int? BookId { get; set; }

    public DateTime? BorrowedDate { get; set; }

    public DateTime? ReturnDate { get; set; }

    public bool? RequestStatus { get; set; }

    public int? DaysDue { get; set; }

    public string? IdNumber { get; set; }

    public bool? DeclineStatus { get; set; }

    public bool? ReturnRequest { get; set; }

    public bool? Returned { get; set; }

    public double? Fines { get; set; }
}
