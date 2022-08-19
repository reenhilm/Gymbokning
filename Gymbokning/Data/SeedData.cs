using Gymbokning.Data;
using Gymbokning.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;

public class SeedData
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IUserStore<ApplicationUser> _userStore;
    private readonly IUserEmailStore<ApplicationUser> _emailStore;

    public SeedData(
        UserManager<ApplicationUser> userManager,
        IUserStore<ApplicationUser> userStore)
    {
        _userManager = userManager;
        _userStore = userStore;
        _emailStore = GetEmailStore();
    }
    public async Task InitAsync()
    {
        if (_userManager.Users.Any()) return;

        var users = GenerateApplicationUsers(1);
        //await db.AddRangeAsync(users);
        //await db.SaveChangesAsync();

        var user = CreateUser();

        foreach (var item in users)
        { 
            await _userStore.SetUserNameAsync(user, item.Email, CancellationToken.None);
            await _emailStore.SetEmailAsync(user, item.Email, CancellationToken.None);
            var result = await _userManager.CreateAsync(user, "Testar123!");
            //var userId = await _userManager.GetUserIdAsync(user);
            /*var code = */await _userManager.GenerateEmailConfirmationTokenAsync(user);

        }
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

    private static IEnumerable<ApplicationUser> GenerateApplicationUsers(int iMembers)
    {
        var members = new List<ApplicationUser>()
        {
            new ApplicationUser()
            {
                Email = "testar@gmail.com"
            }
        };
        return members;
    }
}