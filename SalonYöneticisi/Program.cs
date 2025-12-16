using GymManagementApp.Data;
using GymManagementApp.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using GymManagementApp.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<SporSalonuContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddIdentity<Uye, IdentityRole>(options =>
{
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequiredLength = 3;
    options.SignIn.RequireConfirmedAccount = false;
})
.AddEntityFrameworkStores<SporSalonuContext>()
.AddDefaultTokenProviders();

builder.Services.AddControllersWithViews();

// email servisi hatasini cozmek icin:
builder.Services.AddTransient<IEmailSender, EmailSender>();
// login & register sayfalari icin
builder.Services.AddRazorPages();

var app = builder.Build();


using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = services.GetRequiredService<UserManager<Uye>>();

        string[] roller = { "Admin", "Uye" };
        foreach (var rol in roller)
        {
            if (!await roleManager.RoleExistsAsync(rol))
            {
                await roleManager.CreateAsync(new IdentityRole(rol));
            }
        }

        var adminMail = "b221210371@sakarya.edu.tr";

        var adminVarMi = await userManager.FindByEmailAsync(adminMail);
        if (adminVarMi == null)
        {
            var yeniAdmin = new Uye();
            yeniAdmin.UserName = adminMail;
            yeniAdmin.Email = adminMail;
            yeniAdmin.EmailConfirmed = true;
            yeniAdmin.AdSoyad = "Sistem Yöneticisi";

            await userManager.CreateAsync(yeniAdmin, "sau");
            await userManager.AddToRoleAsync(yeniAdmin, "Admin");
        }
    }
    catch (Exception)
    {
    }
}


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

app.Run();