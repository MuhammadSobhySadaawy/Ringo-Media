using test123.Data;
using test123.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
namespace test123.Controllers;

public class RemindersController : Controller
{
    private readonly ApplicationDbContext _context;

    public RemindersController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        return View(await _context.Reminders.ToListAsync());
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(Reminder reminder)
    {
        if (ModelState.IsValid)
        {
            await _context.Reminders.AddAsync(reminder);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(reminder);
    }
}
