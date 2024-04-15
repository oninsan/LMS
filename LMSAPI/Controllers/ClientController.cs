using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using LMSAPI.Entities;
using System.Security.Cryptography;
using Microsoft.Identity.Client;
using System.Resources;
using Microsoft.AspNetCore.Identity;
using System.Text.Json;
using System.Net.Sockets;
using OfficeOpenXml;

namespace LMSAPI.Controllers
{
  [Route("api/[controller]")]
  [ApiController]

  public class ClientController : ControllerBase
  {
    private readonly LcctolmsContext _context;
    private readonly ILogger<ClientController> _logger;

    public ClientController(LcctolmsContext context, ILogger<ClientController> logger)
    {
      _context = context;
      _logger = logger;
    }

    [HttpPost("RegisterStudent")]
    public async Task<IActionResult> RegisterStudent(User user)
    {
      var isExisting = await _context.Users.AnyAsync(u => u.IdNumber == user.IdNumber);
      if (isExisting)
      {
        return new BadRequestObjectResult("Student exist already!");
      }
      user.Role = "user";
      await _context.Users.AddAsync(user);
      await _context.SaveChangesAsync();
      return new JsonResult("Successfully enrolled a student");
    }

    [HttpPost("UpdateStudent")]
    public async Task<IActionResult> UpdateStudent(string idnumber, int studentid, User student)
    {
      var isUpdatingAdmin = await _context.Users.AnyAsync(u => u.IdNumber == idnumber && u.Role == "Admin");
      if (isUpdatingAdmin)
      {
        student.Id = studentid;
        student.Role = "user";
        _context.Users.Update(student);
        await _context.SaveChangesAsync();
        return new JsonResult("A student was updated successfully");
      }
      return new BadRequestObjectResult("User not allowed to do such task!");
    }

    [HttpGet("CheckExistingRfid")]
    public async Task<IActionResult> CheckExistingRfid(string rfid)
    {
      var user = await _context.Users.AnyAsync(u => u.Rfid == rfid);
      if (!user)
      {
        return new JsonResult("No record");
      }
      return new BadRequestObjectResult("RFID already existed!");
    }

    [HttpPost("ValidateUser")]
    public async Task<IActionResult> ValidateUser(string rfid, string attendancedate, string timein)
    {
      var _user = await _context.Users.FirstOrDefaultAsync(u => u.Rfid == rfid);
      if (_user != null)
      {
        DateTime date = DateTime.Parse(attendancedate);
        TimeSpan time = TimeSpan.Parse(timein);
        var attendance = new Attendance
        {
          IdNumber = _user.IdNumber,
          AttendanceDate = date,
          TimeIn = time,
          Location = "On Premise"
        };

        await _context.Attendances.AddAsync(attendance);
        await _context.SaveChangesAsync();

        Dictionary<string, string> user = new()
        {
          {"idnumber", _user.IdNumber},
          {"role", _user.Role}
        };
        return new JsonResult(user);
      }
      return new JsonResult("Account not found!");
    }

    [HttpPost("NormalLogin")]
    public async Task<IActionResult> NormalLogin(string idnumber, string userkey, string attendancedate, string timein)
    {
      var user = await _context.Users.Where(u => u.IdNumber == idnumber && u.Key == userkey).FirstOrDefaultAsync();
      var _current = await _context.Attendances.OrderByDescending(c => c.Id).FirstOrDefaultAsync(c => c.IdNumber == idnumber);

      if (user != null)
      {
        if (_current != null)
        {
          if (_current.TimeOut == null)
          {
            _current.Location = "Premise(online)";
            await _context.SaveChangesAsync();
            return new JsonResult(
              new Dictionary<string, string>{
                { "role",user.Role },
                { "idnumber",user.IdNumber },
                { "login_method", "rfid"}
              }
            );
          }
          else
          {
            DateTime date = DateTime.Parse(attendancedate);
            TimeSpan time = TimeSpan.Parse(timein);
            var attendance = new Attendance
            {
              IdNumber = user.IdNumber,
              AttendanceDate = date,
              TimeIn = time,
              Location = "Online"
            };
            await _context.Attendances.AddAsync(attendance);
            await _context.SaveChangesAsync();
            return new JsonResult(
              new Dictionary<string, string>{
                { "role",user.Role },
                { "idnumber",user.IdNumber },
                { "login_method", "normal"}
              }
            );
          }
        }
      }

      return new BadRequestObjectResult("Wrong idnumber or userkey");
    }

    [HttpPost("Logout")]
    public async Task<IActionResult> Logout(string rfid, string idnumber, string timeout)
    {
      var user = await _context.Users.FirstOrDefaultAsync(u => u.Rfid == rfid || u.IdNumber == idnumber);
      if (user != null)
      {
        var attendance = await _context.Attendances.OrderByDescending(a => a.Id).FirstOrDefaultAsync(a => a.IdNumber == user.IdNumber);
        // check if account already logged out or not
        if (attendance != null)
        {
          if (attendance.TimeOut == null)
          {
            TimeSpan time = TimeSpan.Parse(timeout);
            attendance.TimeOut = time;
            await _context.SaveChangesAsync();
            return new JsonResult("Success");
          }
          // return new BadRequestObjectResult("Already Logged out");
        }
      }
      return new BadRequestObjectResult("User not found");
    }

    [HttpPost("RfidLogout")]
    public async Task<IActionResult> RfidLogout(string rfid, string timeout)
    {
      var user = await _context.Users.FirstOrDefaultAsync(u => u.Rfid == rfid);
      if (user != null)
      {
        var attendance = await _context.Attendances.OrderByDescending(a => a.Id).FirstOrDefaultAsync(a => a.IdNumber == user.IdNumber);
        // check if account already logged out or not
        if (attendance != null)
        {
          if (attendance.TimeOut == null)
          {
            TimeSpan time = TimeSpan.Parse(timeout);
            attendance.TimeOut = time;
            await _context.SaveChangesAsync();
            return new JsonResult("Success");
          }
          // return new BadRequestObjectResult("Already Logged out");
        }
      }
      return new BadRequestObjectResult("User not found");
    }

    // // get all students
    [HttpGet("GetStudents")]
    public async Task<IActionResult> GetStudents(string rfid, string idnumber)
    {
      var user = await _context.Users.FirstOrDefaultAsync(u => (u.Rfid == rfid || u.IdNumber == idnumber) && u.Role == "admin");
      if (user != null)
      {
        return new JsonResult(await _context.Users.Where(s => s.Role != "admin").ToListAsync());
      }
      return new BadRequestObjectResult("Account not permitted to view this list!");
    }

    // // get particular student
    [HttpGet("GetStudent")]
    public async Task<IActionResult> GetStudent(string idnumber, int studentid)
    {
      var requestIsFromAdmin = await _context.Users.AnyAsync(u => u.IdNumber == idnumber && u.Role == "Admin");
      if (requestIsFromAdmin)
      {
        var student = await _context.Users.FindAsync(studentid);
        return new JsonResult(student);
      }
      return new BadRequestObjectResult("User not allowed to see this info!");
    }

    [HttpGet("GetStudentByIdNumber")]
    public async Task<IActionResult> GetStudentByIdNumber(string idnumber, string studentidnumber)
    {
      var requestIsFromAdmin = await _context.Users.AnyAsync(u => u.IdNumber == idnumber && u.Role == "Admin");
      if (requestIsFromAdmin)
      {

        return new JsonResult(await _context.Users.AnyAsync(u => u.IdNumber == studentidnumber));
      }
      return new BadRequestObjectResult("User not allowed to see this info!");
    }

    [HttpGet("GetStudentFromList")]
    public async Task<IActionResult> GetStudentFromList(string idnumber, string studentidnumber)
    {
      var requestIsFromAdmin = await _context.Users.AnyAsync(u => u.IdNumber == idnumber && u.Role == "Admin");
      if (requestIsFromAdmin)
      {
        var student = await _context.Students.Where(s => s.IdNumber == studentidnumber).FirstOrDefaultAsync();

        if (student != null)
        {
          return new JsonResult(student);
        }
        return new BadRequestObjectResult("No student found");
      }
      return new BadRequestObjectResult("User not allowed to see this info!");
    }

    [HttpGet("GetAllAttendance")]
    public async Task<IActionResult> GetAllAttendance(string idnumber)
    {
      var user = await _context.Users.Where(u => u.IdNumber == idnumber).FirstOrDefaultAsync();
      if (user.Role == "admin")
      {
        var attendances = await (
            from a in _context.Attendances
            join u in _context.Users on a.IdNumber equals u.IdNumber
            // where r.DeclineStatus != null
            orderby a.Id descending
            select new { u.IdNumber, u.FirstName, u.LastName, u.YearLevel, a.AttendanceDate, a.TimeIn, a.TimeOut, a.Location }).ToListAsync();
        return new JsonResult(attendances);
      }
      return new BadRequestObjectResult("User not pemitted to see this list!");
    }

    [HttpGet("GetAttendancesByDate")]
    public async Task<IActionResult> GetAttendancesByDate(string idnumber, string fromdate, string todate)
    {
      var user = await _context.Users.Where(u => u.IdNumber == idnumber).FirstOrDefaultAsync();
      if (user.Role == "admin")
      {
        var attendances = await (
            from a in _context.Attendances
            join u in _context.Users on a.IdNumber equals u.IdNumber
            where a.AttendanceDate >= DateTime.Parse(fromdate) && a.AttendanceDate <= DateTime.Parse(todate)
            orderby a.Id descending
            select new { u.IdNumber, u.FirstName, u.LastName, u.YearLevel, a.AttendanceDate, a.TimeIn, a.TimeOut, a.Location }).ToListAsync();
        return new JsonResult(attendances);
      }
      return new BadRequestObjectResult("User not pemitted to see this list!");
    }

    [HttpGet("GetSelfAttendance")]
    public async Task<IActionResult> GetSelfAttendance(string idnumber)
    {
      var user = await _context.Users.Where(u => u.IdNumber == idnumber).FirstOrDefaultAsync();
      if (user != null)
      {
        var attendances = await (
            from a in _context.Attendances.Where(a => a.IdNumber == idnumber)
            join u in _context.Users.Where(u => u.IdNumber == idnumber) on a.IdNumber equals u.IdNumber
            orderby a.Id descending
            select new { u.IdNumber, u.FirstName, u.LastName, u.YearLevel, a.AttendanceDate, a.TimeIn, a.TimeOut, a.Location }).ToListAsync();
        return new JsonResult(attendances);
      }
      return new BadRequestObjectResult("User not pemitted to see this list!");
    }

    [HttpGet("GetSelfAttendancesByDate")]
    public async Task<IActionResult> GetSelfAttendancesByDate(string idnumber, string fromdate, string todate)
    {
      var user = await _context.Users.Where(u => u.IdNumber == idnumber).FirstOrDefaultAsync();
      if (user != null)
      {
        var attendances = await (
          from a in _context.Attendances.Where(a => a.IdNumber == idnumber)
          join u in _context.Users.Where(u => u.IdNumber == idnumber) on a.IdNumber equals u.IdNumber
          where a.AttendanceDate >= DateTime.Parse(fromdate) && a.AttendanceDate <= DateTime.Parse(todate)
          orderby a.Id descending
          select new { u.IdNumber, u.FirstName, u.LastName, u.YearLevel, a.AttendanceDate, a.TimeIn, a.TimeOut, a.Location }).ToListAsync();
        return new JsonResult(attendances);
      }
      return new BadRequestObjectResult("User not pemitted to see this list!");
    }

    [HttpPost("AddBook")]
    public async Task<IActionResult> AddBook(Book book)
    {
      if (book != null)
      {
        await _context.Books.AddAsync(book);
        await _context.SaveChangesAsync();
        return new JsonResult("Book saved succesfully");
      }
      return new BadRequestObjectResult("Book was not saved!");
    }

    [HttpGet("GetBooks")]
    public async Task<IActionResult> GetBooks()
    {
      return new JsonResult(await _context.Books
          .OrderByDescending(b => b.Id)
          .Where(b => b.DeleteStatus == false)
          .ToListAsync());
    }

    [HttpPost("GetBook")]
    public async Task<IActionResult> GetBook(int id)
    {
      var book = await _context.Books.FirstOrDefaultAsync(b => b.Id == id);
      if (book != null)
      {
        return new JsonResult(book);
      }
      return new BadRequestObjectResult("No book found");
    }

    [HttpPost("UpdateBook")]
    public async Task<IActionResult> UpdateBook(int id, Book book)
    {
      book.Id = id;
      if (book != null)
      {
        _context.Books.Update(book);
        await _context.SaveChangesAsync();
        return new JsonResult("A book was updated successfully");
      }
      return new BadRequestObjectResult("No book found!");
    }

    [HttpPost("DeleteBook")]
    public async Task<IActionResult> DeleteBook(int id)
    {
      var book = await _context.Books.FirstOrDefaultAsync(b => b.Id == id);
      if (book != null)
      {
        book.DeleteStatus = true;
        await _context.SaveChangesAsync();
        return new JsonResult("A book was deleted succesfully");
      }
      return new BadRequestObjectResult("No book was found");
    }

    // For filtering books by author
    [HttpGet("GetAuthors")]
    public async Task<IActionResult> GetAuthors(int? bookid)
    {
      var authors = bookid == null ? await _context.Books
        .Where(b => b.DeleteStatus == false)
        .GroupBy(b => b.Author)
        .Select(g => g.First().Author)
        .ToListAsync()
        : await _context.Books
        .Where(b => b.DeleteStatus == false && b.Id == bookid)
        .GroupBy(b => b.Author)
        .Select(g => g.First().Author)
        .ToListAsync();

      return new JsonResult(authors);
    }

    // For filtering books by Date/Year
    [HttpGet("GetDates")]
    public async Task<IActionResult> GetDates()
    {
      var dates = await _context.Books
        .Where(a => a.DeleteStatus == false)
        .GroupBy(a => a.YearPublished)
        .Select(a => a.First().YearPublished)
        .ToListAsync();
      return new JsonResult(dates);
    }

    // For filtering books by Category
    [HttpGet("GetCategories")]
    public async Task<IActionResult> GetCategories()
    {
      var categories = await _context.Books
        .Where(c => c.DeleteStatus == false)
        .GroupBy(c => c.Category)
        .Select(c => c.First().Category)
        .ToListAsync();
      return new JsonResult(categories);
    }

    // For getting borrower's information
    [HttpPost("GetBorrowerInfo")]
    public async Task<IActionResult> GetBorrowerInfo(string? idnumber, string? rfid)
    {
      var borrower = rfid == null
        ? await _context.Users.FirstOrDefaultAsync(b => b.IdNumber == idnumber)
        : await _context.Users.FirstOrDefaultAsync(b => b.Rfid == rfid);

      return new JsonResult(borrower);
    }

    [HttpPost("AddBookReservation")]
    public async Task<IActionResult> AddBookReservation(BookReservation reservation)
    {
      reservation.AcceptedStatus = false;
      reservation.DeclineStatus = false;
      await _context.BookReservations.AddAsync(reservation);
      await _context.SaveChangesAsync();
      return new JsonResult("Book reservation request successful");
    }

    [HttpPost("AddEquipmentReservation")]
    public async Task<IActionResult> AddEquipmentReservation(EquipmentReservation reservation)
    {
      reservation.AcceptedStatus = false;
      reservation.DeclineStatus = false;
      await _context.EquipmentReservations.AddAsync(reservation);
      await _context.SaveChangesAsync();
      return new JsonResult("Equipment reservation request successful");
    }

    // For getting book reservations
    [HttpGet("GetReservations")]
    public async Task<IActionResult> GetReservations(bool adminview)
    {
      var reservations = adminview == true ? await (
        from r in _context.BookReservations
        join b in _context.Books on r.BookId equals b.Id
        where r.DeclineStatus != true && r.AcceptedStatus != true // false means pending
        orderby r.Id descending
        select new { r, b.Title }).ToListAsync()
        : await (
            from r in _context.BookReservations
            join b in _context.Books on r.BookId equals b.Id
            where r.DeclineStatus != null
            orderby r.Id descending
            select new { r, b.Title }).ToListAsync();
      return new JsonResult(reservations);
    }

    // // For getting equipment reservations
    [HttpGet("GetEquipmentReservations")]
    public async Task<IActionResult> GetEquipmentReservations(bool adminview)
    {
      var reservations = adminview == true ? await (
          from r in _context.EquipmentReservations
          join b in _context.Equipment on r.EquipmentId equals b.Id
          where r.DeclineStatus != true && r.AcceptedStatus != true // false means pending
          orderby r.Id descending
          select new { r, b.EquipmentName }).ToListAsync()
          : await (
              from r in _context.EquipmentReservations
              join b in _context.Equipment on r.EquipmentId equals b.Id
              where r.DeclineStatus != null
              orderby r.Id descending
              select new { r, b.EquipmentName }).ToListAsync();
      return new JsonResult(reservations);
    }

    [HttpGet("GetSelfReservation")]
    public async Task<IActionResult> GetSelfReservation(string idnumber)
    {
      var reservations = await (
          from r in _context.BookReservations
          join b in _context.Books on r.BookId equals b.Id
          where r.DeclineStatus != null
          orderby r.Id descending
          select new { r, b.Title, r.IdNumber }).ToListAsync();

      List<object> n_reservations = new();
      foreach (var item in reservations)
      {
        if (item.IdNumber == idnumber)
        {
          n_reservations.Add(item);
        }
      }
      return new JsonResult(n_reservations);
    }

    [HttpGet("GetSelfEquipmentReservation")]
    public async Task<IActionResult> GetSelfEquipmentReservation(string idnumber)
    {
      var reservations = await (
          from r in _context.EquipmentReservations
          join b in _context.Equipment on r.EquipmentId equals b.Id
          where r.DeclineStatus != null
          orderby r.Id descending
          select new { r, b.EquipmentName, r.IdNumber }).ToListAsync();

      List<object> n_reservations = new();
      foreach (var item in reservations)
      {
        if (item.IdNumber == idnumber)
        {
          n_reservations.Add(item);
        }
      }
      return new JsonResult(n_reservations);
    }

    [HttpPost("RespondReservationRequest")]
    public async Task<IActionResult> RespondReservationRequest(string rfid, string idnumber, int reservationid, bool response)
    {
      // var user = await _context.Users.FirstOrDefaultAsync(u => (u.Rfid == rfid || u.IdNumber == idnumber) && u.Role == "admin");
      var user = await _context.Users.FirstOrDefaultAsync(u => u.Role == "admin");
      if (user != null)
      {
        var reservation = await _context.BookReservations.FirstOrDefaultAsync(r => r.Id == reservationid);
        if (reservation != null)
        {
          string _response = "";
          if (response)
          {
            _response = "Accepted";
            reservation.AcceptedStatus = response;
          }
          else
          {
            _response = "Declined";
            reservation.DeclineStatus = true;
          }
          await _context.SaveChangesAsync();
          return new JsonResult($"{_response} a reservation request");
        }
        return new BadRequestObjectResult("Reservation not found!");
      }
      return new BadRequestObjectResult("User not allowed to do such task!");
    }

    [HttpPost("RespondEquipmentReservationRequest")]
    public async Task<IActionResult> RespondEquipmentReservationRequest(string rfid, string idnumber, int reservationid, bool response)
    {
      // var user = await _context.Users.FirstOrDefaultAsync(u => (u.Rfid == rfid || u.IdNumber == idnumber) && u.Role == "admin");
      var user = await _context.Users.FirstOrDefaultAsync(u => u.Role == "admin");
      if (user != null)
      {
        var reservation = await _context.EquipmentReservations.FirstOrDefaultAsync(r => r.Id == reservationid);
        if (reservation != null)
        {
          string _response = "";
          if (response)
          {
            _response = "Accepted";
            reservation.AcceptedStatus = response;
          }
          else
          {
            _response = "Declined";
            reservation.DeclineStatus = true;
          }
          await _context.SaveChangesAsync();
          return new JsonResult($"{_response} a reservation request");
        }
        return new BadRequestObjectResult("Reservation not found!");
      }
      return new BadRequestObjectResult("User not allowed to do such task!");
    }

    [HttpPost("AddEquipment")]
    public async Task<IActionResult> AddEquipment(Equipment equipment)
    {
      if (equipment != null)
      {
        await _context.Equipment.AddAsync(equipment);
        await _context.SaveChangesAsync();
        return new JsonResult("Equipment saved succesfully");
      }
      return new BadRequestObjectResult("Equipment was not saved!");
    }

    [HttpGet("GetEquipments")]
    public async Task<IActionResult> GetEquipments()
    {
      return new JsonResult(
        await _context.Equipment
          .OrderByDescending(e => e.Id)
          .ToListAsync()
      );
    }

    [HttpGet("GetEquipment")]
    public async Task<IActionResult> GetEquipment(int id)
    {
      return new JsonResult(
        await _context.Equipment.FindAsync(id));
    }

    [HttpPost("UpdateEquipment")]
    public async Task<IActionResult> UpdateEquipment(int id, Equipment equipment)
    {
      equipment.Id = id;
      if (equipment != null)
      {
        _context.Equipment.Update(equipment);
        await _context.SaveChangesAsync();
        return new JsonResult("An equipment was updated successfully");
      }
      return new BadRequestObjectResult("No Equipment found");
    }

    [HttpPost("AddBorrowedBook")]
    public async Task<IActionResult> AddBorrowedBook(BorrowedBook borrowedbook)
    {
      if (borrowedbook != null)
      {
        borrowedbook.RequestStatus = false;
        await _context.BorrowedBooks.AddAsync(borrowedbook);
        await _context.SaveChangesAsync();
        return new JsonResult("Borrow book request successful");
      }
      return new BadRequestObjectResult("Borrow book request failed!");
    }

    [HttpPost("AddBorrowedEquipment")]
    public async Task<IActionResult> AddBorrowedEquipment(BorrowedEquipment borrowedequipment)
    {
      if (borrowedequipment != null)
      {
        borrowedequipment.RequestStatus = false;
        await _context.BorrowedEquipments.AddAsync(borrowedequipment);
        await _context.SaveChangesAsync();
        return new JsonResult("Borrow equipment request successful");
      }
      return new BadRequestObjectResult("Borrow equipment request failed!");
    }

    [HttpGet("GetBorrowRequests")]
    public async Task<IActionResult> GetBorrowRequests()
    {
      var borrow_request = await (
          from br in _context.BorrowedBooks
          join b in _context.Books on br.BookId equals b.Id
          where br.RequestStatus == false && br.DeclineStatus == false && br.Returned == false
          orderby br.Id descending
          select new { br, b.Title }
          ).ToListAsync();
      return new JsonResult(borrow_request);
    }

    [HttpGet("GetBorrowEquipmentRequests")]
    public async Task<IActionResult> GetBorrowEquipmentRequests()
    {
      var borrow_request = await (
          from br in _context.BorrowedEquipments
          join b in _context.Equipment on br.EquipmentId equals b.Id
          where br.RequestStatus == false && br.DeclineStatus == false && br.Returned == false
          orderby br.Id descending
          select new { br, b.EquipmentName }
          ).ToListAsync();
      return new JsonResult(borrow_request);
    }

    // admin view for borrow request and borrowed book item
    [HttpGet("GetBorrowedBooks")]
    public async Task<IActionResult> GetBorrowedBooks()
    {
      var borrow_request = await (
          from br in _context.BorrowedBooks
          join b in _context.Books on br.BookId equals b.Id
          where br.DeclineStatus == false && br.Returned == false
          orderby br.Id descending
          select new { br, b.Title }
          ).ToListAsync();
      return new JsonResult(borrow_request);
    }

    [HttpGet("GetBorrowedEquipments")]
    public async Task<IActionResult> GetBorrowedEquipments()
    {
      var borrow_request = await (
          from br in _context.BorrowedEquipments
          join b in _context.Equipment on br.EquipmentId equals b.Id
          where br.DeclineStatus == false && br.Returned == false
          orderby br.Id descending
          select new { br, b.EquipmentName }
          ).ToListAsync();
      return new JsonResult(borrow_request);
    }

    [HttpGet("GetSelfBorrowedBook")]
    public async Task<IActionResult> GetSelfBorrowedBook(string idnumber)
    {
      var borrowed_books = await (
        from br in _context.BorrowedBooks
        join b in _context.Books on br.BookId equals b.Id
        where br.DeclineStatus == false && br.Returned == false
        orderby br.Id descending
        select new { br, b.Title, br.IdNumber }).ToListAsync();

      List<object> n_borrowed = new();
      foreach (var item in borrowed_books)
      {
        if (item.IdNumber == idnumber)
        {
          n_borrowed.Add(item);
        }
      }
      return new JsonResult(n_borrowed);
    }

    [HttpGet("GetSelfBorrowedEquipment")]
    public async Task<IActionResult> GetSelfBorrowedEquipment(string idnumber)
    {
      var borrowed_equipment = await (
        from br in _context.BorrowedEquipments
        join b in _context.Equipment on br.EquipmentId equals b.Id
        where br.DeclineStatus == false && br.Returned == false
        orderby br.Id descending
        select new { br, b.EquipmentName, br.IdNumber }).ToListAsync();

      List<object> n_borrowed = new();
      foreach (var item in borrowed_equipment)
      {
        if (item.IdNumber == idnumber)
        {
          n_borrowed.Add(item);
        }
      }
      return new JsonResult(n_borrowed);
    }

    [HttpPost("RespondBorrowBookRequest")]
    public async Task<IActionResult> RespondBorrowBookRequest(string rfid, string idnumber, int borrowbookid, bool response)
    {
      var user = await _context.Users.FirstOrDefaultAsync(u => (u.Rfid == rfid || u.IdNumber == idnumber) && u.Role == "admin");
      if (user != null)
      {
        var borrow_book_request = await _context.BorrowedBooks.FirstOrDefaultAsync(r => r.Id == borrowbookid);
        var book = await _context.Books.FindAsync(borrow_book_request.BookId);
        if (borrow_book_request != null)
        {
          string _response = "";
          if (response && book.Quantity > 1)
          {
            _response = "Accepted";
            book.Quantity -= 1;
            borrow_book_request.RequestStatus = response;
          }
          else
          {
            _response = "Declined";
            borrow_book_request.DeclineStatus = true;
          }
          await _context.SaveChangesAsync();
          return new JsonResult($"{_response} a borrow request");
        }
        return new BadRequestObjectResult("Book not found or out of stock!");
      }
      return new BadRequestObjectResult("User not allowed to do such task!");
    }

    [HttpPost("RespondBorrowEquipmentRequest")]
    public async Task<IActionResult> RespondBorrowEquipmentRequest(string rfid, string idnumber, int borrowequipmentid, bool response)
    {
      var user = await _context.Users.FirstOrDefaultAsync(u => (u.Rfid == rfid || u.IdNumber == idnumber) && u.Role == "admin");
      if (user != null)
      {
        var borrow_equipment_request = await _context.BorrowedEquipments.FirstOrDefaultAsync(r => r.Id == borrowequipmentid);
        var equipment = await _context.Equipment.FindAsync(borrow_equipment_request.EquipmentId);
        if (borrow_equipment_request != null)
        {
          string _response = "";
          if (response && equipment.Quantity > 1)
          {
            _response = "Accepted";
            equipment.Quantity -= 1;
            borrow_equipment_request.RequestStatus = response;
          }
          else
          {
            _response = "Declined";
            borrow_equipment_request.DeclineStatus = true;
          }
          await _context.SaveChangesAsync();
          return new JsonResult($"{_response} a borrow equipment request");
        }
        return new BadRequestObjectResult("Equipment not found or out of stock!");
      }
      return new BadRequestObjectResult("User not allowed to do such task!");
    }

    //return the book
    [HttpPost("RequestReturnBook")]
    public async Task<IActionResult> RequestReturnBook(string idnumber, int borrowbookid)
    {
      var borrowed_book = await _context.BorrowedBooks.Where(bb => bb.Id == borrowbookid && bb.IdNumber == idnumber).FirstOrDefaultAsync();
      if (borrowed_book != null)
      {
        borrowed_book.ReturnRequest = true;
        await _context.SaveChangesAsync();
        return new JsonResult("Return book request successfull");
      }
      return new BadRequestObjectResult("Borrowed book not found!");
    }

    //return the equipment
    [HttpPost("RequestReturnEquipment")]
    public async Task<IActionResult> RequestReturnEquipment(string idnumber, int borrowedquipmentid)
    {
      var borrowed_book = await _context.BorrowedEquipments.Where(bb => bb.Id == borrowedquipmentid && bb.IdNumber == idnumber).FirstOrDefaultAsync();
      if (borrowed_book != null)
      {
        borrowed_book.ReturnRequest = true;
        await _context.SaveChangesAsync();
        return new JsonResult("Return equipment request successfull");
      }
      return new BadRequestObjectResult("Borrowed equipment not found!");
    }

    // get all return book requests
    [HttpGet("GetReturnBookRequests")]
    public async Task<IActionResult> GetReturnBookRequests(string idnumber)
    {
      var user = _context.Users.FirstOrDefault(u => u.IdNumber == idnumber && u.Role == "admin");
      if (user != null)
      {
        var return_request = await (
          from rr in _context.BorrowedBooks
          join b in _context.Books on rr.BookId equals b.Id
          where rr.DeclineStatus == false && rr.Returned == false && rr.ReturnRequest == true
          orderby rr.Id descending
          select new { rr, b.Title }
          ).ToListAsync();
        return new JsonResult(return_request);
      }
      return new BadRequestObjectResult("User not permitted to see this list!");
    }

    [HttpGet("GetReturnEquipmentRequests")]
    public async Task<IActionResult> GetReturnEquipmentRequests(string idnumber)
    {
      var user = _context.Users.FirstOrDefault(u => u.IdNumber == idnumber && u.Role == "admin");
      if (user != null)
      {
        var return_request = await (
          from rr in _context.BorrowedEquipments
          join b in _context.Equipment on rr.EquipmentId equals b.Id
          where rr.DeclineStatus == false && rr.Returned == false && rr.ReturnRequest == true
          orderby rr.Id descending
          select new { rr, b.EquipmentName }
          ).ToListAsync();
        return new JsonResult(return_request);
      }
      return new BadRequestObjectResult("User not permitted to see this list!");
    }

    [HttpPost("RespondReturnBookRequest")]
    public async Task<IActionResult> RespondReturnBookRequest(string rfid, string idnumber, int borrowbookid, bool response)
    {
      var user = await _context.Users.FirstOrDefaultAsync(u => (u.Rfid == rfid || u.IdNumber == idnumber) && u.Role == "admin");
      if (user != null)
      {
        var borrowed_book = await _context.BorrowedBooks.FirstOrDefaultAsync(r => r.Id == borrowbookid);
        var book = await _context.Books.FindAsync(borrowed_book.BookId);
        if (borrowed_book != null)
        {
          string _response = "";
          if (response)
          {
            _response = "Accepted";
            book.Quantity += 1;
            borrowed_book.Returned = response;
          }
          else
          {
            _response = "Declined";
            borrowed_book.ReturnRequest = false;
          }
          await _context.SaveChangesAsync();
          return new JsonResult($"{_response} a return request");
        }
        return new BadRequestObjectResult("Borrowed book not found!");
      }
      return new BadRequestObjectResult("User not allowed to do such task!");
    }

    [HttpPost("RespondReturnEquipmentRequest")]
    public async Task<IActionResult> RespondReturnEquipmentRequest(string rfid, string idnumber, int borrowedquipmentid, bool response)
    {
      var user = await _context.Users.FirstOrDefaultAsync(u => (u.Rfid == rfid || u.IdNumber == idnumber) && u.Role == "admin");
      if (user != null)
      {
        var borrowed_equipment = await _context.BorrowedEquipments.FirstOrDefaultAsync(r => r.Id == borrowedquipmentid);
        var equipment = await _context.Equipment.FindAsync(borrowed_equipment.EquipmentId);
        if (borrowed_equipment != null)
        {
          string _response = "";
          if (response)
          {
            _response = "Accepted";
            equipment.Quantity += 1;
            borrowed_equipment.Returned = response;
          }
          else
          {
            _response = "Declined";
            borrowed_equipment.ReturnRequest = false;
          }
          await _context.SaveChangesAsync();
          return new JsonResult($"{_response} a return request");
        }
        return new BadRequestObjectResult("Borrowed equipment not found!");
      }
      return new BadRequestObjectResult("User not allowed to do such task!");
    }

    [HttpPost("RequestBook")]
    public async Task<IActionResult> RequestBook(RequestedBook requestedBook)
    {
      if (requestedBook != null)
      {
        await _context.RequestedBooks.AddAsync(requestedBook);
        await _context.SaveChangesAsync();
        return new JsonResult("Requested a book successfully");
      }
      return new BadRequestObjectResult("Something went wrong");
    }

    [HttpGet("GetBookRequests")]
    public async Task<IActionResult> GetBookRequests()
    {
      var requests = await _context.RequestedBooks.OrderByDescending(r => r.Id).ToListAsync();
      return new JsonResult(requests);
    }

    [HttpGet("GetSelfBookRequests")]
    public async Task<IActionResult> GetSelfBookRequests(string idnumber)
    {
      var requests = await _context.RequestedBooks.Where(r => r.Idnumber == idnumber).OrderByDescending(r => r.Id).ToListAsync();
      return new JsonResult(requests);
    }

    [HttpPost("RespondBookRequest")]
    public async Task<IActionResult> RespondBookRequest(string rfid, string idnumber, int requestid, bool response)
    {
      var user = await _context.Users.FirstOrDefaultAsync(u => (u.Rfid == rfid || u.IdNumber == idnumber) && u.Role == "admin");
      if (user != null)
      {
        var request = await _context.RequestedBooks.FirstOrDefaultAsync(r => r.Id == requestid);
        if (request != null)
        {
          string _response = "";
          if (response)
          {
            _response = "Accepted";
            request.AcceptedStatus = response;
          }
          else
          {
            _response = "Declined";
            request.DeclineStatus = true;
          }
          await _context.SaveChangesAsync();
          return new JsonResult($"{_response} a book request");
        }
        return new BadRequestObjectResult("Book request not found!");
      }
      return new BadRequestObjectResult("User not allowed to do such task!");
    }

    [HttpPost("EditUserProfile")]
    public async Task<IActionResult> EditUserProfile(string idnumber, string rfid, User user)
    {
      var _user = await _context.Users.Where(u => u.IdNumber == idnumber || u.Rfid == rfid).FirstOrDefaultAsync();
      if (_user != null)
      {
        _user.FirstName = user.FirstName;
        _user.LastName = user.LastName;
        _user.IdNumber = user.IdNumber;
        _user.YearLevel = user.YearLevel;
        _user.Rfid = user.Rfid ?? _user.Rfid;
        _user.Role = user.Role;
        _user.Key = user.Key;
        await _context.SaveChangesAsync();
        return new JsonResult("Successfully updated the profile");
      }
      return new BadRequestObjectResult("User not found!");
    }

    [HttpPost("ImportBooks")]
    public async Task<IActionResult> ImportBooks([FromForm] string idnumber, [FromForm] List<IFormFile> file)
    {
      bool userIsAdmin = await _context.Users.AnyAsync(u => u.IdNumber == idnumber && u.Role == "Admin");
      if (userIsAdmin)
      {
        using var transaction = _context.Database.BeginTransaction();
        try
        {
          foreach (var formFile in file)
          {
            if (formFile.Length > 0)
            {
              using var stream = new MemoryStream();
              await formFile.CopyToAsync(stream);
              using var package = new ExcelPackage(stream);
              ExcelWorksheet worksheet = package.Workbook.Worksheets[0];
              int rowCount = worksheet.Dimension.Rows;

              for (int row = 2; row <= rowCount; row++)
              {
                string title = worksheet.Cells[row, 1].Value.ToString();
                string author = worksheet.Cells[row, 2].Value.ToString();
                DateTime yearPublished = DateTime.Parse(worksheet.Cells[row, 3].Value.ToString().Trim());
                string category = worksheet.Cells[row, 4].Value.ToString().Trim();
                int quantity = int.Parse(worksheet.Cells[row, 5].Value.ToString().Trim());
                string sourceOfFund = worksheet.Cells[row, 6].Value.ToString();
                string publisher = worksheet.Cells[row, 7].Value.ToString();
                string remarks = worksheet.Cells[row, 8].Value.ToString();

                var existingBook = await _context.Books.FirstOrDefaultAsync(b => b.Title.ToLower() == title.ToLower() && b.Author.ToLower() == author.ToLower());

                if (existingBook != null)
                {
                  existingBook.Quantity += quantity;
                }
                else
                {
                  var newBook = new Book
                  {
                    Title = title,
                    Author = author,
                    YearPublished = yearPublished,
                    Category = category,
                    Quantity = quantity,
                    SourceOfFund = sourceOfFund,
                    Publisher = publisher,
                    Remarks = remarks,
                    DeleteStatus = false,
                  };
                  await _context.Books.AddAsync(newBook);
                }
              }
            }
          }

          await _context.SaveChangesAsync();
          transaction.Commit();
          return new JsonResult("Successful importing books");
        }
        catch (Exception ex)
        {
          _logger.LogError(ex, "Error in importing excel files");
          transaction.Rollback(); // Rollback the transaction on error
          return StatusCode(500, $"Internal server error: {ex.Message}");
        }

      }
      return new BadRequestObjectResult("User not allowed to do such task!");
    }

    [HttpPost("ImportStudentList")]
    public async Task<IActionResult> ImportStudentList([FromForm] string idnumber, [FromForm] List<IFormFile> file)
    {
      bool userIsAdmin = await _context.Users.AnyAsync(u => u.IdNumber == idnumber && u.Role == "Admin");
      if (userIsAdmin)
      {
        using var transaction = _context.Database.BeginTransaction();
        try
        {
          foreach (var formFile in file)
          {
            if (formFile.Length > 0)
            {
              using var stream = new MemoryStream();
              await formFile.CopyToAsync(stream);
              using var package = new ExcelPackage(stream);
              ExcelWorksheet worksheet = package.Workbook.Worksheets[0];
              int rowCount = worksheet.Dimension.Rows;

              for (int row = 2; row <= rowCount; row++)
              {
                string idNumber = worksheet.Cells[row, 1].Value.ToString();
                string firstName = worksheet.Cells[row, 2].Value.ToString();
                string middleName = worksheet.Cells[row, 3].Value.ToString();
                string lastName = worksheet.Cells[row, 4].Value.ToString();
                string category = worksheet.Cells[row, 5].Value.ToString().Trim();
                string sex = worksheet.Cells[row, 6].Value.ToString();
                string mobileNumber = worksheet.Cells[row, 7].Value.ToString().Trim();

                var existingStudent = await _context.Students.FirstOrDefaultAsync(b => b.IdNumber == idNumber);

                if (existingStudent != null)
                {
                  existingStudent.IdNumber = idNumber;
                  existingStudent.MiddleName = middleName;
                  existingStudent.FirstName = firstName;
                  existingStudent.Category = category;
                  existingStudent.LastName = lastName;
                  existingStudent.Sex = sex;
                  existingStudent.MobileNumber = mobileNumber;
                }
                else
                {
                  var newStudent = new Student
                  {
                    IdNumber = idNumber,
                    MiddleName = middleName,
                    FirstName = firstName,
                    LastName = lastName,
                    Category = category,
                    Sex = sex,
                    MobileNumber = mobileNumber,
                  };
                  await _context.Students.AddAsync(newStudent);
                }
              }
            }
          }

          await _context.SaveChangesAsync();
          transaction.Commit();
          return new JsonResult("Successful importing student list");
        }
        catch (Exception ex)
        {
          _logger.LogError(ex, "Error in importing excel files");
          transaction.Rollback(); // Rollback the transaction on error
          return StatusCode(500, $"Internal server error: {ex.Message}");
        }

      }
      return new BadRequestObjectResult("User not allowed to do such task!");
    }

    [HttpPost("AddTransactionHistory")]
    public async Task<IActionResult> AddTransactionHistory(string rfid, TransactionHistory transactionHistory)
    {
      var user = await _context.Users.Where(u => u.IdNumber == transactionHistory.IdNumber || u.Rfid == rfid).FirstOrDefaultAsync();
      if (user != null)
      {
        await _context.TransactionHistories.AddAsync(transactionHistory);
        await _context.SaveChangesAsync();
        return new JsonResult("Success adding transaction.");
      }
      return new BadRequestObjectResult("User not found!");
    }

    [HttpGet("GetTransactions")]
    public async Task<IActionResult> GetTransactions(string idnumber, string rfid)
    {
      var user = await _context.Users.FirstOrDefaultAsync(u => (u.IdNumber == idnumber || u.Rfid == rfid) && u.Role == "admin");

      if (user != null)
      {
        var transactions = await (
            from t in _context.TransactionHistories
            join u in _context.Users on t.IdNumber equals u.IdNumber
            orderby t.Id descending
            select new { t.IdNumber, u.FirstName, u.LastName, t.TransactionType, t.TransactionDate }
        ).ToListAsync();
        return new JsonResult(transactions);
      }
      return new BadRequestObjectResult("Not allowed to see this list!");
    }

    [HttpGet("GetSelfTransactions")]
    public async Task<IActionResult> GetSelfTransactions(string idnumber, string rfid)
    {
      var user = await _context.Users.FirstOrDefaultAsync(u => (u.IdNumber == idnumber || u.Rfid == rfid) && u.Role == "user");

      if (user != null)
      {
        var transactions = await (
            from t in _context.TransactionHistories.Where(t => t.IdNumber == idnumber)
            join u in _context.Users.Where(u => u.IdNumber == idnumber) on t.IdNumber equals u.IdNumber
            orderby t.Id descending
            select new { t.IdNumber, u.FirstName, u.LastName, t.TransactionType, t.TransactionDate }
        ).ToListAsync();
        return new JsonResult(transactions);
      }
      return new BadRequestObjectResult("Not allowed to see this list!");
    }

    [HttpGet("MostBorrowedBook")]
    public async Task<IActionResult> MostBorrowedBook(bool isadmin)
    {
      if (isadmin)
      {
        var mostBorrowedBook = await (
            from rr in _context.BorrowedBooks
            join b in _context.Books on rr.BookId equals b.Id
            where rr.DeclineStatus == false && rr.RequestStatus == true
            group rr by new { rr.BookId, b.Title } into g
            orderby g.Count() descending
            select new { g.Key.BookId, g.Key.Title, Count = g.Count() }
        ).FirstOrDefaultAsync();

        if (mostBorrowedBook != null)
        {
          var borrowingsPerDayOfMostBorrowedBook = await (
              from rr in _context.BorrowedBooks
              where rr.BookId == mostBorrowedBook.BookId && rr.DeclineStatus == false && rr.RequestStatus == true
              group rr by rr.BorrowedDate into g
              select new { Date = g.Key, Count = g.Count() }
          ).ToListAsync();

          return new JsonResult(new { MostBorrowedBook = mostBorrowedBook, BorrowingsPerDay = borrowingsPerDayOfMostBorrowedBook });
        }
        return new JsonResult("No books have been borrowed.");
      }
      return new BadRequestObjectResult("Account not permitted to view the results!");
    }

    [HttpGet("MostBorrowedEquipment")]
    public async Task<IActionResult> MostBorrowedEquipment(bool isadmin)
    {
      if (isadmin)
      {
        var mostBorrowedEquipment = await (
            from rr in _context.BorrowedEquipments
            join b in _context.Equipment on rr.EquipmentId equals b.Id
            where rr.DeclineStatus == false && rr.RequestStatus == true
            group rr by new { rr.EquipmentId, b.EquipmentName } into g
            orderby g.Count() descending
            select new { g.Key.EquipmentId, g.Key.EquipmentName, Count = g.Count() }
        ).FirstOrDefaultAsync();

        if (mostBorrowedEquipment != null)
        {
          var borrowingsPerDayOfMostBorrowedEquipment = await (
              from rr in _context.BorrowedEquipments
              where rr.EquipmentId == mostBorrowedEquipment.EquipmentId && rr.DeclineStatus == false && rr.RequestStatus == true
              group rr by rr.BorrowedDate into g
              select new { Date = g.Key, Count = g.Count() }
          ).ToListAsync();

          return new JsonResult(new { MostBorrowedEquipment = mostBorrowedEquipment, BorrowingsPerDay = borrowingsPerDayOfMostBorrowedEquipment });
        }
        return new JsonResult("No equipments have been borrowed.");
      }
      return new BadRequestObjectResult("Account not permitted to view the results!");
    }

    [HttpGet("LeastBorrowedBook")]
    public async Task<IActionResult> LeastBorrowedBook(bool isadmin)
    {
      if (isadmin)
      {
        var leastBorrowedBook = await (
            from rr in _context.BorrowedBooks
            join b in _context.Books on rr.BookId equals b.Id
            where rr.DeclineStatus == false && rr.RequestStatus == true
            group rr by new { rr.BookId, b.Title } into g
            orderby g.Count() ascending
            select new { g.Key.BookId, g.Key.Title, Count = g.Count() }
        ).FirstOrDefaultAsync();

        if (leastBorrowedBook != null)
        {
          var borrowingsPerDayOfLeastBorrowedBook = await (
              from rr in _context.BorrowedBooks
              where rr.BookId == leastBorrowedBook.BookId && rr.DeclineStatus == false && rr.RequestStatus == true
              group rr by rr.BorrowedDate into g
              select new { Date = g.Key, Count = g.Count() }
          ).ToListAsync();

          return new JsonResult(new { LeastBorrowedBook = leastBorrowedBook, BorrowingsPerDay = borrowingsPerDayOfLeastBorrowedBook });
        }
        return new JsonResult("No books have been borrowed.");
      }
      return new BadRequestObjectResult("Account not permitted to view the results!");
    }

    [HttpGet("LeastBorrowedEquipment")]
    public async Task<IActionResult> LeastBorrowedEquipment(bool isadmin)
    {
      if (isadmin)
      {
        var leastBorrowedEquipment = await (
            from rr in _context.BorrowedEquipments
            join b in _context.Equipment on rr.EquipmentId equals b.Id
            where rr.DeclineStatus == false && rr.RequestStatus == true
            group rr by new { rr.EquipmentId, b.EquipmentName } into g
            orderby g.Count() ascending
            select new { g.Key.EquipmentId, g.Key.EquipmentName, Count = g.Count() }
        ).FirstOrDefaultAsync();

        if (leastBorrowedEquipment != null)
        {
          var borrowingsPerDayOfLeastBorrowedEquipment = await (
              from rr in _context.BorrowedEquipments
              where rr.EquipmentId == leastBorrowedEquipment.EquipmentId && rr.DeclineStatus == false && rr.RequestStatus == true
              group rr by rr.BorrowedDate into g
              select new { Date = g.Key, Count = g.Count() }
          ).ToListAsync();

          return new JsonResult(new { LeastBorrowedEquipment = leastBorrowedEquipment, BorrowingsPerDay = borrowingsPerDayOfLeastBorrowedEquipment });
        }
        return new JsonResult("No equipments have been borrowed.");
      }
      return new BadRequestObjectResult("Account not permitted to view the results!");
    }

    [HttpGet("StudentWithLeastFines")]
    public async Task<IActionResult> StudentWithLeastFines(bool isadmin)
    {
      if (isadmin)
      {
        var studentWithLeastFines = await (
            from rr in _context.BorrowedBooks
            join u in _context.Users on rr.IdNumber equals u.IdNumber
            where rr.DeclineStatus == false && rr.RequestStatus == true && rr.Returned == false
            group rr by new { rr.IdNumber, u.FirstName, u.LastName } into g
            orderby g.Sum(rr => rr.Fines) ascending
            select new { g.Key.IdNumber, g.Key.FirstName, g.Key.LastName, TotalFines = g.Sum(rr => rr.Fines) }
        ).FirstOrDefaultAsync();

        if (studentWithLeastFines != null)
        {
          var finesPerDayOfUser = await (
              from rr in _context.BorrowedBooks
              where rr.IdNumber == studentWithLeastFines.IdNumber && rr.DeclineStatus == false && rr.RequestStatus == true && rr.Returned == false
              group rr by rr.BorrowedDate into g
              select new { Date = g.Key, TotalFines = g.Sum(rr => rr.Fines) }
          ).ToListAsync();

          return new JsonResult(new { studentWithLeastFines = studentWithLeastFines, FinesPerDay = finesPerDayOfUser });
        }
        return new JsonResult("No books have been borrowed.");
      }
      return new BadRequestObjectResult("Account not permitted to view the results!");
    }

    [HttpGet("StudentWithMostFines")]
    public async Task<IActionResult> StudentWithMostFines(bool isadmin)
    {
      if (isadmin)
      {
        var studentWithMostFines = await (
            from rr in _context.BorrowedBooks
            join u in _context.Users on rr.IdNumber equals u.IdNumber
            where rr.DeclineStatus == false && rr.RequestStatus == true && rr.Returned == false
            group rr by new { rr.IdNumber, u.FirstName, u.LastName } into g
            orderby g.Sum(rr => rr.Fines) descending
            select new { g.Key.IdNumber, g.Key.FirstName, g.Key.LastName, TotalFines = g.Sum(rr => rr.Fines) }
        ).FirstOrDefaultAsync();

        if (studentWithMostFines != null)
        {
          var finesPerDayOfUser = await (
              from rr in _context.BorrowedBooks
              where rr.IdNumber == studentWithMostFines.IdNumber && rr.DeclineStatus == false && rr.RequestStatus == true && rr.Returned == false
              group rr by rr.BorrowedDate into g
              select new { Date = g.Key, TotalFines = g.Sum(rr => rr.Fines) }
          ).ToListAsync();

          return new JsonResult(new { studentWithMostFines = studentWithMostFines, FinesPerDay = finesPerDayOfUser });
        }
        return new JsonResult("No books have been borrowed.");
      }
      return new BadRequestObjectResult("Account not permitted to view the results!");
    }

    [HttpGet("PeakHoursAndAttendancePerDay")]
    public async Task<IActionResult> PeakHoursAndAttendancePerDay(bool isAdmin)
    {
      if (isAdmin)
      {
        var peakHours = await (
          from a in _context.Attendances
          group a by a.TimeIn into g
          orderby g.Count() descending
          select new { Hour = g.Key, Count = g.Count() }
        ).Take(2).ToListAsync();

        var attendancePerDay = await (
            from a in _context.Attendances
            group a by a.AttendanceDate into g
            select new { Date = g.Key, Attendance = g.Count() }
        ).ToListAsync();

        if (peakHours != null && attendancePerDay.Count > 0)
        {
          return new JsonResult(new { PeakHour = peakHours, AttendancePerDay = attendancePerDay });
        }
        return new JsonResult("No attendance records found.");
      }
      return new BadRequestObjectResult("Account not permitted to view the results!");
    }

  }
}