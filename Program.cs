using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using SG_Finder.Data;
using Microsoft.Data.SqlClient;
using SG_Finder.Hubs;
using SG_Finder.Models;
using SG_Finder.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();


builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));


// Register the DbContext with EF Core
/*
// If you want to use SQL Server in the future, uncomment this block
// Register the DbContext with EF Core using SQL Server
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    Console.WriteLine($"Using connection string: {connectionString}");
    options.UseSqlServer(connectionString); // SQL Server commented out for now
});
*/


// Register Identity services with roles and EF stores (Do this only once)
builder.Services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddRazorPages(); // Make sure Razor Pages are added

// Register SignalR and UserIdProvider
builder.Services.AddSignalR(options =>
{
    options.ClientTimeoutInterval = TimeSpan.FromMinutes(1);
    options.KeepAliveInterval = TimeSpan.FromSeconds(10);
});

// Register UserIdProvider as a singleton
builder.Services.AddSingleton<IUserIdProvider, UserIdProvider>();

builder.Services.AddRazorPages(); // Make sure Razor Pages are added
var app = builder.Build();
app.MapHub<StudyHub>("/StudyHub"); // Ensure casing is consistent

// Seed the database with initial data using a scoped service
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var db = services.GetRequiredService<ApplicationDbContext>();
    var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();  // Correct
    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();     // Fix: Use IdentityRole

    await ApplicationDbInitializer.Initialize(db, userManager, roleManager);
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();


app.UseAuthentication();  // Use Identity Authentication middleware (only once)
app.UseAuthorization();   // Use Authorization middleware


// Map Razor Pages and Controllers
app.MapRazorPages();
app.MapControllers();

// Map Controller Routes
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");



app.Run();