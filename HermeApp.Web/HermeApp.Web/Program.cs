using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using HermeApp.Web.Data;
using HermeApp.Web.Areas.Identity.Data;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("HermeAppWebContextConnection") ?? throw new InvalidOperationException("Connection string 'HermeAppWebContextConnection' not found.");

builder.Services.AddDbContext<HermeAppWebContext>(options => options.UseSqlServer(connectionString));

builder.Services.AddDefaultIdentity<HermeAppWebUser>(options => options.SignIn.RequireConfirmedAccount = true).AddEntityFrameworkStores<HermeAppWebContext>();

builder.Services.AddTransient<IUserValidator<HermeAppWebUser>, CustomUsernameEmailPolicy>();

// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
