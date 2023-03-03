using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WebIdentityTest.Areas.Identity.Data;
using WebIdentityTest.Data;
var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("WebIdentityTestContextConnection") ?? throw new InvalidOperationException("Connection string 'WebIdentityTestContextConnection' not found.");

builder.Services.AddDbContext<WebIdentityTestContext>(options => options.UseSqlServer(connectionString));

builder.Services.AddDefaultIdentity<WebIdentityTestUser>(options => options.SignIn.RequireConfirmedAccount = true).AddEntityFrameworkStores<WebIdentityTestContext>();

// Add services to the container.
builder.Services.AddRazorPages();

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

app.Run();
