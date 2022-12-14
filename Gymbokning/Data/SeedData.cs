using Gymbokning.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
using System.Text;

namespace Gymbokning.Data
{
    public class SeedData : ISeedData
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUserStore<ApplicationUser> _userStore;
        private readonly IUserEmailStore<ApplicationUser> _emailStore;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IOptions<PasswordSettings> _options;
        private readonly ApplicationDbContext _db;

        public SeedData(UserManager<ApplicationUser> userManager, IUserStore<ApplicationUser> userStore, RoleManager<IdentityRole> roleManager, IOptions<PasswordSettings> options, ApplicationDbContext db)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _userStore = userStore;
            _emailStore = GetEmailStore();
            _options = options;
            _db = db;
        }
        public async Task InitAsync()
        {
            if (_userManager.Users.Any()) return;

            var roles = GenerateRoles();
            var users = GenerateSeedUserDTO(2, _options);
            var gymClasses = GenerateGymClassesWithoutIds();
                //await db.AddRangeAsync(users);
                //await db.SaveChangesAsync();

                //https://stackoverflow.com/questions/18667633/how-can-i-use-async-with-foreach
                /* fungerar ej:
                  roles.ToList().ForEach(async r => await _roleManager.CreateAsync(r)); */

                /* fungerar men fult?
                using (RoleManager<IdentityRole> rm = _roleManager)
                { 
                    var tasks = roles.ToList().Select(r => rm.CreateAsync(r));
                    var results = await Task.WhenAll(tasks);
                }
                */

                /*
                 * även fast alla metoder är asyncrona gick det ej att köra flera samtidigt, felmeddelande:
                 * 		Message	"A second operation was started on this context instance before a previous operation completed. This is usually caused by different threads concurrently using the same instance of DbContext. For more information on how to avoid threading issues with DbContext, see https://go.microsoft.com/fwlink/?linkid=2097913."	string
                 * 		behöver en Transcient DbContext?
                 */

            //await Parallel.ForEachAsync(roles, async (i, CancellationToken) => await _roleManager.CreateAsync(i));
            foreach (var role in roles)
                await _roleManager.CreateAsync(role);


                //ParallelOptions parallelOptions = new() { MaxDegreeOfParallelism = 3 };
                //await Parallel.ForEachAsync(users, parallelOptions, async (seedUser, CToken) =>
                //{
                //    //await _db.GymClass.AddAsync(seedUser, CToken);
                //    var user = CreateUser();
                //    await _userStore.SetUserNameAsync(user, seedUser.Email, CToken);
                //    await _emailStore.SetEmailAsync(user, seedUser.Email, CToken);
                //    user.FirstName = seedUser.FirstName;
                //    user.LastName = seedUser.LastName;
                //    await _userManager.CreateAsync(user, seedUser.Password);

                //    foreach (var role in seedUser.Roles)
                //        await _userManager.AddToRoleAsync(user, role);

                //    await ConfirmUser(user);
                //});

            foreach (var seedUser in users)
            {
                var user = CreateUser();
                await _userStore.SetUserNameAsync(user, seedUser.Email, CancellationToken.None);
                await _emailStore.SetEmailAsync(user, seedUser.Email, CancellationToken.None);
                user.FirstName = seedUser.FirstName;
                user.LastName = seedUser.LastName;
                await _userManager.CreateAsync(user, seedUser.Password);

                foreach (var role in seedUser.Roles)
                    await _userManager.AddToRoleAsync(user, role);

                await ConfirmUser(user);
            }

            gymClasses.ToList().ForEach(gc => _db.GymClass.Add(gc));
            await _db.SaveChangesAsync();
        }

        private async Task ConfirmUser(ApplicationUser user)
        {
            //inRegister
            await _userManager.GenerateEmailConfirmationTokenAsync(user);
            //code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));

            //inRegisterConfirmation
            var code2 = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            code2 = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code2));

            //inConfirmEmail
            var code3 = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code2));
            await _userManager.ConfirmEmailAsync(user, code3);
        }

        private IUserEmailStore<ApplicationUser> GetEmailStore()
        {
            if (!_userManager.SupportsUserEmail)
            {
                throw new NotSupportedException("The default UI requires a user store with email support.");
            }
            return (IUserEmailStore<ApplicationUser>)_userStore;
        }

        private static ApplicationUser CreateUser()
        {
            try
            {
                return Activator.CreateInstance<ApplicationUser>();
            }
            catch
            {
                throw new InvalidOperationException($"Can't create an instance of '{nameof(ApplicationUser)}'. " +
                    $"Ensure that '{nameof(ApplicationUser)}' is not an abstract class and has a parameterless constructor, or alternatively " +
                    $"override the register page in /Areas/Identity/Pages/Account/Register.cshtml");
            }
        }

        private static IEnumerable<SeedUserDTO> GenerateSeedUserDTO(IOptions<PasswordSettings> options)
        {
            var adminPWD = options.Value.AdminPassword;
            var basicPWD = options.Value.BasicPassword;
            var members = new List<SeedUserDTO>()
        {
            new SeedUserDTO() { Email = "christian@kajal.se", Password = basicPWD, FirstName = "Christian", LastName = "Rönnholm" },
            new SeedUserDTO() { Email = "admin@Gymbokning.se", Password = adminPWD, Roles = new List<string>() { RoleNames.AdminRole }, FirstName = "AdminFirstName", LastName = "AdminLastName" }
        };
            return members;
        }
        private static IEnumerable<IdentityRole> GenerateRoles() => new List<IdentityRole>() { new IdentityRole() { Name = RoleNames.AdminRole } };

        private static IEnumerable<GymClass> GenerateGymClassesWithoutIds() => new List<GymClass>() {
            new GymClass() {
                Duration = TimeSpan.FromMinutes(30),
                StartTime = DateTime.Now.AddYears(1),
                Description = "a Future GymClass",
                Name = "Future GymClass"
            },
            new GymClass() {
                Duration = TimeSpan.FromMinutes(30),
                StartTime = DateTime.Now.AddYears(1),
                Description = "a Future GymClass",
                Name = "Another Future GymClass"
            },
            new GymClass() {
                Duration = TimeSpan.FromMinutes(45),
                StartTime = DateTime.Now.AddYears(-1),
                Description = "a Past GymClass",
                Name = "Past GymClass"
            },
            new GymClass() {
                Duration = TimeSpan.FromMinutes(45),
                StartTime = DateTime.Now.AddYears(-1),
                Description = "a Past GymClass",
                Name = "Another Past GymClass"
            }
        };

        private static IEnumerable<SeedUserDTO> GenerateSeedUserDTO(int iMembers, IOptions<PasswordSettings> options) => GenerateSeedUserDTO(options).Take(iMembers);
    }
}