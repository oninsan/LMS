using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace LMSAPI.Entities;

public partial class LcctolmsContext : DbContext
{
    public LcctolmsContext()
    {
    }

    public LcctolmsContext(DbContextOptions<LcctolmsContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Attendance> Attendances { get; set; }

    public virtual DbSet<Book> Books { get; set; }

    public virtual DbSet<BookReservation> BookReservations { get; set; }

    public virtual DbSet<BorrowedBook> BorrowedBooks { get; set; }

    public virtual DbSet<BorrowedBookPenalty> BorrowedBookPenalties { get; set; }

    public virtual DbSet<BorrowedEquipment> BorrowedEquipments { get; set; }

    public virtual DbSet<Equipment> Equipment { get; set; }

    public virtual DbSet<EquipmentReservation> EquipmentReservations { get; set; }

    public virtual DbSet<RequestedBook> RequestedBooks { get; set; }

    public virtual DbSet<Student> Students { get; set; }

    public virtual DbSet<TransactionHistory> TransactionHistories { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=DESKTOP-PAAQDTO\\SQLEXPRESS;Database=LCCTOLMS;TrustServerCertificate=true;Trusted_Connection=True;Integrated Security=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Attendance>(entity =>
        {
            entity.ToTable("attendance");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.AttendanceDate)
                .HasColumnType("date")
                .HasColumnName("attendance_date");
            entity.Property(e => e.IdNumber)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("id_number");
            entity.Property(e => e.Location)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("location");
            entity.Property(e => e.TimeIn).HasColumnName("time_in");
            entity.Property(e => e.TimeOut).HasColumnName("time_out");
        });

        modelBuilder.Entity<Book>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("pk_Book");

            entity.ToTable("book");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Author)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("author");
            entity.Property(e => e.Category)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("category");
            entity.Property(e => e.DeleteStatus).HasColumnName("delete_status");
            entity.Property(e => e.Publisher)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("publisher");
            entity.Property(e => e.Quantity).HasColumnName("quantity");
            entity.Property(e => e.Remarks)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("remarks");
            entity.Property(e => e.SourceOfFund)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("source_of_fund");
            entity.Property(e => e.Title)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("title");
            entity.Property(e => e.YearPublished)
                .HasColumnType("date")
                .HasColumnName("year_published");
        });

        modelBuilder.Entity<BookReservation>(entity =>
        {
            entity.ToTable("book_reservation");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.AcceptedStatus).HasColumnName("accepted_status");
            entity.Property(e => e.BookId).HasColumnName("book_id");
            entity.Property(e => e.DeclineStatus).HasColumnName("decline_status");
            entity.Property(e => e.IdNumber)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("id_number");
            entity.Property(e => e.ReservationDate)
                .HasColumnType("date")
                .HasColumnName("reservation_date");
            entity.Property(e => e.ReturnDate)
                .HasColumnType("date")
                .HasColumnName("return_date");
        });

        modelBuilder.Entity<BorrowedBook>(entity =>
        {
            entity.ToTable("borrowed_book");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.BookId).HasColumnName("book_id");
            entity.Property(e => e.BorrowedDate)
                .HasColumnType("date")
                .HasColumnName("borrowed_date");
            entity.Property(e => e.DaysDue).HasColumnName("days_due");
            entity.Property(e => e.DeclineStatus).HasColumnName("decline_status");
            entity.Property(e => e.Fines).HasColumnName("fines");
            entity.Property(e => e.IdNumber)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("id_number");
            entity.Property(e => e.RequestStatus).HasColumnName("request_status");
            entity.Property(e => e.ReturnDate)
                .HasColumnType("date")
                .HasColumnName("return_date");
            entity.Property(e => e.ReturnRequest).HasColumnName("return_request");
            entity.Property(e => e.Returned).HasColumnName("returned");
        });

        modelBuilder.Entity<BorrowedBookPenalty>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_BorrowedBookPenalty");

            entity.ToTable("borrowed_book_penalty");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.BorrowedBookId).HasColumnName("borrowed_book_id");
            entity.Property(e => e.BorrowedDate)
                .HasColumnType("date")
                .HasColumnName("borrowed_date");
            entity.Property(e => e.Idnumber).HasColumnName("idnumber");
            entity.Property(e => e.PaidStatus).HasColumnName("paid_status");
            entity.Property(e => e.Penalty).HasColumnName("penalty");
            entity.Property(e => e.ReturnDate)
                .HasColumnType("date")
                .HasColumnName("return_date");
        });

        modelBuilder.Entity<BorrowedEquipment>(entity =>
        {
            entity.ToTable("borrowed_equipment");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.BorrowedDate)
                .HasColumnType("date")
                .HasColumnName("borrowed_date");
            entity.Property(e => e.DaysDue).HasColumnName("days_due");
            entity.Property(e => e.DeclineStatus).HasColumnName("decline_status");
            entity.Property(e => e.EquipmentId).HasColumnName("equipment_id");
            entity.Property(e => e.Fines).HasColumnName("fines");
            entity.Property(e => e.IdNumber)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("id_number");
            entity.Property(e => e.RequestStatus).HasColumnName("request_status");
            entity.Property(e => e.ReturnDate)
                .HasColumnType("date")
                .HasColumnName("return_date");
            entity.Property(e => e.ReturnRequest).HasColumnName("return_request");
            entity.Property(e => e.Returned).HasColumnName("returned");
        });

        modelBuilder.Entity<Equipment>(entity =>
        {
            entity.ToTable("equipment");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.DamageStatus).HasColumnName("damage_status");
            entity.Property(e => e.EquipmentName)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("equipment_name");
            entity.Property(e => e.Quantity).HasColumnName("quantity");
        });

        modelBuilder.Entity<EquipmentReservation>(entity =>
        {
            entity.ToTable("equipment_reservation");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.AcceptedStatus).HasColumnName("accepted_status");
            entity.Property(e => e.DeclineStatus).HasColumnName("decline_status");
            entity.Property(e => e.EquipmentId).HasColumnName("equipment_id");
            entity.Property(e => e.IdNumber)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("id_number");
            entity.Property(e => e.ReservationDate)
                .HasColumnType("date")
                .HasColumnName("reservation_date");
            entity.Property(e => e.ReturnDate)
                .HasColumnType("date")
                .HasColumnName("return_date");
        });

        modelBuilder.Entity<RequestedBook>(entity =>
        {
            entity.ToTable("requested_book");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.AcceptedStatus).HasColumnName("accepted_status");
            entity.Property(e => e.Author)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("author");
            entity.Property(e => e.BookTitle)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("book_title");
            entity.Property(e => e.DeclineStatus).HasColumnName("decline_status");
            entity.Property(e => e.Idnumber)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("idnumber");
            entity.Property(e => e.RequestDate)
                .HasColumnType("date")
                .HasColumnName("request_date");
            entity.Property(e => e.RequestResponse).HasColumnName("request_response");
        });

        modelBuilder.Entity<Student>(entity =>
        {
            entity.ToTable("student");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Category)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("category");
            entity.Property(e => e.FirstName)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("first_name");
            entity.Property(e => e.IdNumber)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("id_number");
            entity.Property(e => e.LastName)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("last_name");
            entity.Property(e => e.MiddleName)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("middle_name");
            entity.Property(e => e.MobileNumber)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("mobile_number");
            entity.Property(e => e.Sex)
                .HasMaxLength(5)
                .IsUnicode(false)
                .HasColumnName("sex");
        });

        modelBuilder.Entity<TransactionHistory>(entity =>
        {
            entity.ToTable("transaction_history");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.BookId).HasColumnName("book_id");
            entity.Property(e => e.BorrowedBookId).HasColumnName("borrowed_book_id");
            entity.Property(e => e.EquipmentId).HasColumnName("equipment_id");
            entity.Property(e => e.IdNumber)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("id_number");
            entity.Property(e => e.RequestId).HasColumnName("request_id");
            entity.Property(e => e.ReservationId).HasColumnName("reservation_id");
            entity.Property(e => e.TransactionDate)
                .HasColumnType("date")
                .HasColumnName("transaction_date");
            entity.Property(e => e.TransactionTime).HasColumnName("transaction_time");
            entity.Property(e => e.TransactionType)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("transaction_type");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__student__3213E83F7780E3B3");

            entity.ToTable("user");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.FirstName)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("first_name");
            entity.Property(e => e.IdNumber)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("id_number");
            entity.Property(e => e.Key)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("key");
            entity.Property(e => e.LastName)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("last_name");
            entity.Property(e => e.MobileNumber)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("mobile_number");
            entity.Property(e => e.Rfid)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("rfid");
            entity.Property(e => e.Role)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("role");
            entity.Property(e => e.Sex)
                .HasMaxLength(2)
                .IsUnicode(false)
                .HasColumnName("sex");
            entity.Property(e => e.YearLevel).HasColumnName("year_level");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
