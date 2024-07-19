

namespace test123.Models;
public class Department
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string? Logo { get; set; }
    public int? ParentDepartmentId { get; set; }
    public Department? ParentDepartment { get; set; }
    public List<Department>? SubDepartments { get; set; }
}
