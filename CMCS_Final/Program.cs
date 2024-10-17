using CMCS_Final.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
        builder.Services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(connectionString));
        builder.Services.AddDatabaseDeveloperPageExceptionFilter();

        builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = false)
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>();
        builder.Services.AddControllersWithViews();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseMigrationsEndPoint();
        }
        else
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

        using (var scope = app.Services.CreateScope())
        {
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();

            var roles = new[] { "Academic Manager", "Programme Coordinator", "Lecturer" };
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            var adminEmail = "Academic@gmail.com";
            var adminPassword = "Academic!23";

            var adminUser = await userManager.FindByEmailAsync(adminEmail);
            if (adminUser == null)
            {
                adminUser = new IdentityUser { UserName = adminEmail, Email = adminEmail };
                var result = await userManager.CreateAsync(adminUser, adminPassword);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, "Academic Manager");
                }
            }

            var programmeCoordinatorEmail = "Programme@gmail.com";
            var programmeCoordinatorPassword = "Programme!23";

            var programmeCoordinatorUser = await userManager.FindByEmailAsync(programmeCoordinatorEmail);
            if (programmeCoordinatorUser == null)
            {
                programmeCoordinatorUser = new IdentityUser { UserName = programmeCoordinatorEmail, Email = programmeCoordinatorEmail };
                var result = await userManager.CreateAsync(programmeCoordinatorUser, programmeCoordinatorPassword);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(programmeCoordinatorUser, "Programme Coordinator");
                }
            }
        }

        app.Use(async (context, next) =>
        {
            if (context.User.Identity.IsAuthenticated)
            {
                using (var scope = context.RequestServices.CreateScope())
                {
                    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();

                    var user = await userManager.GetUserAsync(context.User);
                    if (user != null)
                    {
                        var isLecturer = await userManager.IsInRoleAsync(user, "Lecturer");
                        if (!isLecturer)
                        {
                            await userManager.AddToRoleAsync(user, "Lecturer");
                        }
                    }
                }
            }
            await next();
        });

        app.Run();
    }
}