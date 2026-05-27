using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Eventos.Data;

var builder = WebApplication.CreateBuilder(args);

// Pega a string de conexão configurada
var connectionString = builder.Configuration.GetConnectionString("EventosContext")
	?? throw new InvalidOperationException("Connection string 'EventosContext' not found.");

// LÓGICA DE BANCO INTELIGENTE: Local (SQL Server) vs Nuvem (SQLite)
if (builder.Environment.IsDevelopment())
{
	// Usa o seu SQL Server local quando você roda no seu PC
	builder.Services.AddDbContext<EventosContext>(options =>
		options.UseSqlServer(connectionString));
}
else
{
	// Usa SQLite automaticamente quando o projeto vai para o Render
	builder.Services.AddDbContext<EventosContext>(options =>
		options.UseSqlite(connectionString));
}

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

app.MapRazorPages();

// Bloco para garantir a criação do banco SQLite e rodar o Seed do Admin
using (var scope = app.Services.CreateScope())
{
	var services = scope.ServiceProvider;
	var context = services.GetRequiredService<EventosContext>();

	// CRIAÇÃO AUTOMÁTICA DO BANCO: Garante que o arquivo do banco nasça no Render com as tabelas
	if (!app.Environment.IsDevelopment())
	{
		context.Database.EnsureCreated();
	}

	var userManager = services.GetRequiredService<UserManager<IdentityUser>>();
	var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

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
	if (adminUser != null && !await userManager.IsInRoleAsync(adminUser, "Admin"))
	{
		await userManager.AddToRoleAsync(adminUser, "Admin");
	}
}

app.Run();