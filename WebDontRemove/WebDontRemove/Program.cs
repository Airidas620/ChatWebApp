using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WebDontRemove.Areas.Identity.Data;
using WebDontRemove.Data;
var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("WebDontRemoveContextConnection") ?? throw new InvalidOperationException("Connection string 'WebDontRemoveContextConnection' not found.");

builder.Services.AddDbContext<WebDontRemoveContext>(options => options.UseSqlServer(connectionString));

builder.Services.AddDefaultIdentity<WebDontRemoveUser>(options => options.SignIn.RequireConfirmedAccount = false).AddEntityFrameworkStores<WebDontRemoveContext>();

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
