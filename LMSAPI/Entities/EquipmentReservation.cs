using System;
using System.Collections.Generic;

namespace LMSAPI.Entities;

public partial class EquipmentReservation
{
    public int Id { get; set; }

    public int? EquipmentId { get; set; }

    public string? IdNumber { get; set; }

    public DateTime? ReservationDate { get; set; }

    public DateTime? ReturnDate { get; set; }

    public bool? AcceptedStatus { get; set; }

    public bool? DeclineStatus { get; set; }
}
