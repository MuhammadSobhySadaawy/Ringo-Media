using System.ComponentModel;

namespace test123.Models;
public class Reminder
{
    public int Id { get; set; }
    public string Title { get; set; }
    public DateTime DateTime { get; set; }
    [DefaultValue(false)]
    public bool IsSend { get; set; }
}
