using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using LMSAPI.Entities;

public class UpdateDaysDueService : BackgroundService
{
  private readonly IServiceScopeFactory _scopeFactory;
  private readonly ILogger<UpdateDaysDueService> _logger;

  public UpdateDaysDueService(IServiceScopeFactory scopeFactory, ILogger<UpdateDaysDueService> logger)
  {
    _scopeFactory = scopeFactory;
    _logger = logger;
  }
  protected override async Task ExecuteAsync(CancellationToken stoppingToken)
  {
    while (!stoppingToken.IsCancellationRequested)
    {
      try
      {
        using (var scope = _scopeFactory.CreateScope())
        {
          var context = scope.ServiceProvider.GetRequiredService<LcctolmsContext>();
          UpdateDaysDueForBorrowedBooks(context);
          UpdateDaysDueForBorrowedEquipments(context);
        }

        _logger.LogInformation("UpdateDaysDueService ran at: {time}", DateTimeOffset.Now);

        var now = DateTime.Now;
        var nextRunTime = now.Date.AddDays(1).AddHours(1); // Set to run at 1 AM each day
        var delay = nextRunTime - now;

        if (delay > TimeSpan.Zero)
        {
          await Task.Delay(delay, stoppingToken);
        }
      }
      catch (Exception ex)
      {
        // Log the error details
        _logger.LogError(ex, "An error occurred in ExecuteAsync.");
      }
    }
  }

  private void UpdateDaysDueForBorrowedBooks(LcctolmsContext context)
  {
    try
    {
      var borrowedBooks = context.BorrowedBooks
          .Where(b => b.ReturnDate.HasValue && b.RequestStatus == true && b.Returned == false && b.ReturnDate.Value < DateTime.Now)
          .ToList();

      foreach (var book in borrowedBooks)
      {
        book.DaysDue = (DateTime.Now - book.ReturnDate.Value).Days;
        book.Fines = book.DaysDue * 6.0;
      }

      context.SaveChanges();
    }
    catch (Exception ex)
    {
      // Log the error details
      _logger.LogError(ex, "An error occurred while updating days due for borrowed books.");
    }
  }

  private void UpdateDaysDueForBorrowedEquipments(LcctolmsContext context)
  {
    try
    {
      var borrowedEquipments = context.BorrowedEquipments
          .Where(b => b.ReturnDate.HasValue && b.RequestStatus == true && b.Returned == false && b.ReturnDate.Value < DateTime.Now)
          .ToList();

      foreach (var equipment in borrowedEquipments)
      {
        equipment.DaysDue = (DateTime.Now - equipment.ReturnDate.Value).Days;
        equipment.Fines = equipment.DaysDue * 6.0;
      }

      context.SaveChanges();
    }
    catch (Exception ex)
    {
      // Log the error details
      _logger.LogError(ex, "An error occurred while updating days due for borrowed equipments.");
    }
  }

}

