using Gymbokning.AutoMapper;
using Gymbokning.Data;
using Gymbokning.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Features;
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
//        OnValidatePrincipal = async (context) =>
//        {
//            var binding = context.HttpContext.Features.Get<ITlsTokenBindingFeature>()?.GetProvidedTokenBindingId();
//            var tlsTokenBinding = binding == null ? null : Convert.ToBase64String(binding);
//            var cookie = context.Options.CookieManager.GetRequestCookie(context.HttpContext, context.Options.CookieName);
//            if (cookie != null)
//            {
//                var ticket = context.Options.TicketDataFormat.Unprotect(cookie, tlsTokenBinding);

//                var expiresUtc = ticket.Properties.ExpiresUtc;
//                var currentUtc = context.Options.SystemClock.UtcNow;
//                if (expiresUtc != null && expiresUtc.Value < currentUtc)
//                {
//                    context.RedirectUri += "&p1=yourparameter";
//                }
//            }
//            context.RejectPrincipal();
//        }
//    });

//app.UseCookieAuthentication(new CookieAuthenticationOptions
//{
//    AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
//    LoginPath = new PathString("/Login"),

//    Provider = new CookieAuthenticationProvider
//    {
//        OnValidateIdentity = SecurityStampValidator.OnValidateIdentity<UserManager, User>(
//                        validateInterval: TimeSpan.FromSeconds(0),
//                        regenerateIdentityCallback: (manager, user) => user.GenerateUserIdentityAsync(manager)
//                }
//});
builder.Services.AddDatabaseDeveloperPageExceptionFilter();


builder.Services.AddDefaultIdentity<ApplicationUser>(options =>
{
    //Dimitris tycker RequireConfirmedAccount är "onödigt" för detta projekt, men såg det som en utmaning att följa flödet för att göra confirm och få seeds som är confirmed också
    options.SignIn.RequireConfirmedAccount = true;
    options.Password.RequireDigit = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireLowercase = false;
    options.Password.RequiredLength = 3;
    options.User.RequireUniqueEmail = true;
})
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.ConfigureApplicationCookie(options =>
{
    //builder.Services.ConfigureOptions()
    //SessionOptions = _configuration<SessionOptions>.GetSection("");
    //int timeout = (int)section.Timeout.TotalMinutes;
    options.ExpireTimeSpan = TimeSpan.FromDays(1);
});

builder.Services.AddControllersWithViews();

builder.Services.AddAutoMapper(typeof(MapperProfile));
builder.Services.AddScoped<ISeedData, SeedData>();
builder.Services.AddAuthorization(options =>
{
    options.FallbackPolicy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();
});

var app = builder.Build();

using var scope = app.Services.CreateScope();
var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

db.Database.EnsureDeleted();
db.Database.Migrate();

try
{
    //Maybe no need to call the scopes serviceProvider?
    using var nestedScope = scope.ServiceProvider.CreateScope();
    var seedData = nestedScope.ServiceProvider.GetRequiredService<ISeedData>();
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
