using System;
using System.Collections.Generic;

namespace LMSAPI.Entities;

public partial class TransactionHistory
{
    public int Id { get; set; }

    public string? IdNumber { get; set; }

    public DateTime? TransactionDate { get; set; }

    public string? TransactionType { get; set; }

    public int? BookId { get; set; }

    public int? EquipmentId { get; set; }

    public int? ReservationId { get; set; }

    public int? RequestId { get; set; }

    public int? BorrowedBookId { get; set; }

    public TimeSpan? TransactionTime { get; set; }
}
