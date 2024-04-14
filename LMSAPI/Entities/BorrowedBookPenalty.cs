using System;
using System.Collections.Generic;

namespace LMSAPI.Entities;

public partial class BorrowedBookPenalty
{
    public int Id { get; set; }

    public int? Idnumber { get; set; }

    public int? BorrowedBookId { get; set; }

    public DateTime? BorrowedDate { get; set; }

    public DateTime? ReturnDate { get; set; }

    public double? Penalty { get; set; }

    public bool? PaidStatus { get; set; }
}
