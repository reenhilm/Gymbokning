using Gymbokning.Data;
using Gymbokning.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;

public class SeedData
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IUserStore<ApplicationUser> _userStore;
    private readonly IUserEmailStore<ApplicationUser> _emailStore;
    private readonly RoleManager<IdentityRole> _roleManager;

    public SeedData(UserManager<ApplicationUser> userManager, IUserStore<ApplicationUser> userStore, RoleManager<IdentityRole> roleManager)
    {
        _roleManager = roleManager;
        _userManager = userManager;
        _userStore = userStore;
        _emailStore = GetEmailStore();
    }
    public void Init()
    {
        if (_userManager.Users.Any()) return;

        var roles= GenerateRoles();
        var users = GenerateSeedUserDTO(2);
        //await db.AddRangeAsync(users);
        //await db.SaveChangesAsync();

        roles.ToList().ForEach(async r => await _roleManager.CreateAsync(r));

        users.ToList().ForEach(async u => {
            var user = CreateUser();
            await _userStore.SetUserNameAsync(user, u.Email, CancellationToken.None);
            await _emailStore.SetEmailAsync(user, u.Email, CancellationToken.None);
            var result = await _userManager.CreateAsync(user, u.Password);
            //var userId = await _userManager.GetUserIdAsync(user);
            /*var code = */
            await _userManager.GenerateEmailConfirmationTokenAsync(user);
        });
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

    private static IEnumerable<SeedUserDTO> GenerateSeedUserDTO()
    {
        var members = new List<SeedUserDTO>()
        {
            new SeedUserDTO() { Email = "christian@kajal.se", Password = "Testar123!" },
            new SeedUserDTO() { Email = "admin@Gymbokning.se", Password = "Testar123!", Roles = new List<string>() { RoleNames.AdminRole } }
        };
        return members;
    }
    private static IEnumerable<IdentityRole> GenerateRoles()
    {
        var roles = new List<IdentityRole>() { new IdentityRole() { Name = RoleNames.AdminRole } };
        return roles;
    }

    private static IEnumerable<SeedUserDTO> GenerateSeedUserDTO(int iMembers) => GenerateSeedUserDTO().Take(iMembers);
}