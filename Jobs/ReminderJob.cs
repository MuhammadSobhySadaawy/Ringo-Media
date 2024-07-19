using test123.Data;
using test123.Services;
using Quartz;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
namespace test123.Jobs;

public class ReminderJob : IJob
{
    private readonly ApplicationDbContext _context;
    private readonly IEmailSender _emailSender;

    public ReminderJob(ApplicationDbContext context, IEmailSender emailSender)
    {
        _context = context;
        _emailSender = emailSender;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        var reminders =await _context.Reminders
                                .Where(r => r.DateTime <= DateTime.Now&&r.IsSend==false)
                                .ToListAsync();

        foreach (var reminder in reminders)
        {
           var reult= await _emailSender.SendEmailAsync("mohamed.sobhe2563653@gmail.com", "Reminder", reminder.Title);
             reminder.IsSend = reult;
        }

        await _context.SaveChangesAsync();
    }
}
