using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Linq;
using test123.Data;
using Microsoft.AspNetCore.Mvc.Rendering;
using test123.Models;
namespace test123.Controllers;

public class DepartmentsController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly IWebHostEnvironment _environment;

    public DepartmentsController(ApplicationDbContext context, IWebHostEnvironment environment)
    {
        _context = context;
        _environment = environment;
    }

    // Get All Departments
    public async Task<IActionResult> Index()
    {
        return View(await _context.Departments.Include(d => d.SubDepartments).ToListAsync());
    }

    // Create Department
    public async Task<IActionResult> Create()
    {
        ViewBag.Departments = new SelectList(await _context.Departments.Where(e=>e.ParentDepartmentId==null).ToListAsync(), "Id", "Name");
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Department department, IFormFile logo, int[] subDepartmentIds)
    {
        if (ModelState.IsValid)
        {
            if (logo != null && logo.Length > 0)
            {
                var uploads = Path.Combine(_environment.WebRootPath, "uploads");
                var filePath = Path.Combine(uploads, logo.FileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await logo.CopyToAsync(stream);
                }
                department.Logo = "/uploads/" + logo.FileName;
            }

            if (subDepartmentIds != null && subDepartmentIds.Length > 0)
            {
                department.SubDepartments = await _context.Departments
                    .Where(d => subDepartmentIds.Contains(d.Id))
                    .ToListAsync();
            }

            await _context.AddAsync(department);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        ViewBag.Departments = new SelectList(_context.Departments, "Id", "Name");
        return View(department);
    }

    // Edit Department
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var department = await _context.Departments.Include(d => d.SubDepartments).FirstOrDefaultAsync(d => d.Id == id);
        if (department == null)
        {
            return NotFound();
        }

        ViewBag.Departments = new SelectList(await _context.Departments.Where(e=>e.ParentDepartmentId== null).ToListAsync(), "Id", "Name");
        return View(department);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Department department, IFormFile logo, int[] subDepartmentIds)
    {
        if (id != department.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                if (logo != null && logo.Length > 0)
                {
                    var uploads = Path.Combine(_environment.WebRootPath, "uploads");
                    var filePath = Path.Combine(uploads, logo.FileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await logo.CopyToAsync(stream);
                    }
                    department.Logo = "/uploads/" + logo.FileName;
                }

                var existingDepartment = await _context.Departments
                    .Include(d => d.SubDepartments)
                    .FirstOrDefaultAsync(d => d.Id == id);

                if (existingDepartment == null)
                {
                    return NotFound();
                }

                existingDepartment.Name = department.Name;
                existingDepartment.Logo = department.Logo;
                existingDepartment.ParentDepartmentId = department.ParentDepartmentId;

                existingDepartment.SubDepartments.Clear();
                if (subDepartmentIds != null && subDepartmentIds.Length > 0)
                {
                    existingDepartment.SubDepartments = await _context.Departments
                        .Where(d => subDepartmentIds.Contains(d.Id))
                        .ToListAsync();
                }

                _context.Update(existingDepartment);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await DepartmentExists(department.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return RedirectToAction(nameof(Index));
        }
        ViewBag.Departments = new SelectList(_context.Departments, "Id", "Name");
        return View(department);
    }

    // Delete Department
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var department = await _context.Departments
            .FirstOrDefaultAsync(m => m.Id == id);
        if (department == null)
        {
            return NotFound();
        }

        return View(department);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var department = await _context.Departments.FindAsync(id);
        _context.Departments.Remove(department);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private async Task<bool> DepartmentExists(int id)
    {
        return await _context.Departments.AnyAsync(e => e.Id == id);
    }
}