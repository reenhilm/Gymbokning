using Gymbokning.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using System.Text;

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
    public async Task InitAsync()
    {
        if (_userManager.Users.Any()) return;

        var roles= GenerateRoles();
        var users = GenerateSeedUserDTO(2);
        //await db.AddRangeAsync(users);
        //await db.SaveChangesAsync();

        foreach (var role in roles)
            await _roleManager.CreateAsync(role);

        foreach(var seedUser in users)
        {
            var user = CreateUser();
            await _userStore.SetUserNameAsync(user, seedUser.Email, CancellationToken.None);
            await _emailStore.SetEmailAsync(user, seedUser.Email, CancellationToken.None);
            await _userManager.CreateAsync(user, seedUser.Password);

            foreach (var role in seedUser.Roles)
                await _userManager.AddToRoleAsync(user, role);

            await ConfirmUser(seedUser, user);
        }
    }

    private async Task ConfirmUser(SeedUserDTO seedUser, ApplicationUser user)
    {
        //inRegister
        var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));

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

    private static IEnumerable<SeedUserDTO> GenerateSeedUserDTO()
    {
        var members = new List<SeedUserDTO>()
        {
            new SeedUserDTO() { Email = "christian@kajal.se", Password = "Testar123!" },
            new SeedUserDTO() { Email = "admin@Gymbokning.se", Password = "Testar123!", Roles = new List<string>() { RoleNames.AdminRole } }
        };
        return members;
    }
    private static IEnumerable<IdentityRole> GenerateRoles() => new List<IdentityRole>() { new IdentityRole() { Name = RoleNames.AdminRole } };

    private static IEnumerable<SeedUserDTO> GenerateSeedUserDTO(int iMembers) => GenerateSeedUserDTO().Take(iMembers);
}