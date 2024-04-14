using System;
using System.Collections.Generic;

namespace LMSAPI.Entities;

public partial class RequestedBook
{
    public int Id { get; set; }

    public string? Idnumber { get; set; }

    public string? BookTitle { get; set; }

    public string? Author { get; set; }

    public DateTime? RequestDate { get; set; }

    public bool? RequestResponse { get; set; }

    public bool? AcceptedStatus { get; set; }

    public bool? DeclineStatus { get; set; }
}
