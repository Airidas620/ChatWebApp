using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Identity;

namespace HermeApp.Web.Areas.Identity.Data
{
    public class CustomUsernameEmailPolicy : UserValidator<HermeAppWebUser>
    {
        public override async Task<IdentityResult> ValidateAsync(UserManager<HermeAppWebUser> manager, HermeAppWebUser user)
        {
            IdentityResult result = await base.ValidateAsync(manager, user);
            List<IdentityError> errors = result.Succeeded ? new List<IdentityError>() : result.Errors.ToList();

            if (user.UserName.Length < 3 || user.UserName.Length > 30)
            {
                errors.Add(new IdentityError
                {
                    Description = "Please enter valid username"
                });
            }
            
            if (!Regex.Match(user.Email, @"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$").Success)
            {
                errors.Add(new IdentityError
                {
                    Description = "Please enter valid e-mail"
                });
            }
            
            return errors.Count == 0 ? IdentityResult.Success : IdentityResult.Failed(errors.ToArray());
        }
    }
}
