using Gymbokning.AutoMapper;
using Gymbokning.Data;
using Gymbokning.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

var _configuration = builder.Configuration;
builder.Services.AddSingleton(_configuration.GetSection("IdentityUser").Get<PasswordSettings>());
builder.Services.Configure<PasswordSettings>(_configuration.GetSection("IdentityUser").Bind);

//builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
//    .AddCookie(o => o.Events = new CookieAuthenticationEvents()
//    {
//        OnValidatePrincipal = async (c) =>
//        {
//            c.RejectPrincipal;
//        }
//    });
builder.Services.AddDatabaseDeveloperPageExceptionFilter();


builder.Services.AddDefaultIdentity<ApplicationUser>(options =>
{
    //Dimitris tycker RequireConfirmedAccount är "onödigt" för detta projekt, men såg det som en utmaning att följa flödet för att göra confirm och få seeds som är confirmed också
    options.SignIn.RequireConfirmedAccount = true;
    options.Password.RequireDigit = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequiredLength = 3;
})
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddControllersWithViews();

builder.Services.AddAutoMapper(typeof(MapperProfile));
builder.Services.AddScoped<SeedData>();

var app = builder.Build();

using var scope = app.Services.CreateScope();
var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
db.Database.EnsureDeleted();
db.Database.Migrate();

try
{
    //Maybe no need to call the scopes serviceProvider?
    using var nestedScope = scope.ServiceProvider.CreateScope();
    var seedData = nestedScope.ServiceProvider.GetRequiredService<SeedData>();
    await seedData.InitAsync();
}
catch (Exception ex)
{
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
    logger.LogError(String.Join(" ", ex.Message));
}


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=GymClasses}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();
