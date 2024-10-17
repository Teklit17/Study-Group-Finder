using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SG_Finder.Data;using Microsoft.Data.SqlClient;





var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Register the DbContext with EF Core
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    Console.WriteLine($"Using connection string: {connectionString}");
    options.UseSqlServer(connectionString);
});

// Optional: Add ASP.NET Core Identity if you’re using authentication
builder.Services.AddDefaultIdentity<IdentityUser>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

var app = builder.Build();

// Seed the database with initial data using a scoped service
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var dbContext = services.GetRequiredService<ApplicationDbContext>();
    ApplicationDbInitializer.Initialize(dbContext);
}


var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

using (var connection = new SqlConnection(connectionString))
{
    connection.Open();
    using (var command = new SqlCommand("SELECT COUNT(*) FROM Messages", connection))
    {
        var count = (int)command.ExecuteScalar();
        Console.WriteLine($"Number of messages in database: {count}");
    }
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
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");


app.Run();