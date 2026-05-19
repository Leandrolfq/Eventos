using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Eventos.Data;

var builder = WebApplication.CreateBuilder(args);

// DbContext
builder.Services.AddDbContext<EventosContext>(options =>
	options.UseSqlServer(
		builder.Configuration.GetConnectionString("EventosContext")
		?? throw new InvalidOperationException("Connection string 'EventosContext' not found.")));

// 🔐 IDENTITY (FALTAVA ISSO)
builder.Services.AddDefaultIdentity<IdentityUser>(options =>
{
	options.SignIn.RequireConfirmedAccount = false;
})
.AddEntityFrameworkStores<EventosContext>();

// MVC
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Pipeline
if (!app.Environment.IsDevelopment())
{
	app.UseExceptionHandler("/Home/Error");
	app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// 🔐 FALTAVA ISSO TAMBÉM
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
	name: "default",
	pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();