using HermeApp.Service.Data;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HermeApp
{
    public class UserRepository:BaseRepository<HermeAppWebUser>
    {
        private readonly UserManager<HermeAppWebUser> _userManager;
        public UserRepository(UserManager<HermeAppWebUser> userManager, HermeAppWebContext context) : base(context)
        {
            _userManager = userManager;
        }
        public async Task<string> FindIdByName(string userName)
        {
            var user = await _userManager.FindByNameAsync(userName);
            return user.Id;
        }
    }
}
