using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Eventos.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<EventosContext>(options =>
	options.UseSqlServer(
		builder.Configuration.GetConnectionString("EventosContext")
		?? throw new InvalidOperationException("Connection string 'EventosContext' not found.")));

builder.Services.AddDefaultIdentity<IdentityUser>(options =>
{
	options.SignIn.RequireConfirmedAccount = false;
})
.AddRoles<IdentityRole>()
.AddEntityFrameworkStores<EventosContext>();

builder.Services.AddControllersWithViews();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
	app.UseExceptionHandler("/Home/Error");
	app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
	name: "default",
	pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages(); // necessário para as páginas de login do Identity

// Seed do usuário admin
using (var scope = app.Services.CreateScope())
{
	var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
	var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

	// Cria a role Admin se não existir
	if (!await roleManager.RoleExistsAsync("Admin"))
	{
		await roleManager.CreateAsync(new IdentityRole("Admin"));
	}

	string email = "admin@techconnect.com";
	string senha = "Admin123@";

	if (await userManager.FindByEmailAsync(email) == null)
	{
		var admin = new IdentityUser
		{
			UserName = email,
			Email = email,
			EmailConfirmed = true
		};
		await userManager.CreateAsync(admin, senha);
	}

	// Atribui a role Admin ao usuário
	var adminUser = await userManager.FindByEmailAsync(email);
	if (!await userManager.IsInRoleAsync(adminUser, "Admin"))
	{
		await userManager.AddToRoleAsync(adminUser, "Admin");
	}
}
app.Run();