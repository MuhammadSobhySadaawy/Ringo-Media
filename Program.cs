using test123.Data;
using test123.Jobs;
using test123.Services;
using Microsoft.EntityFrameworkCore;
using Quartz;
using SendGrid.Helpers.Mail;
using test123.Settings;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<ApplicationDbContext>(options =>
          options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddCors(e => e.AddDefaultPolicy(
   builder => {
       builder.AllowAnyMethod().AllowAnyHeader().AllowAnyOrigin();
   }
));
builder.Services.AddTransient<IEmailSender, EmailSender>();

builder.Services.AddQuartz(q =>
{
    q.UseMicrosoftDependencyInjectionJobFactory();

    var jobKey = new JobKey("ReminderJob");
    q.AddJob<ReminderJob>(opts => opts.WithIdentity(jobKey));
    q.AddTrigger(opts => opts
        .ForJob(jobKey)
        .WithIdentity("ReminderJob-trigger")
        .WithCronSchedule("0/30 * * * * ?")); // Every 30 seconds
});
builder.Services.AddOptions<EmailSettings>()
    .Bind(builder.Configuration.GetSection(nameof(MailSettings)));

builder.Services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);
// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseCors(e => e.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
