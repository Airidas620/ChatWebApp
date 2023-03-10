using HermeApp.Service.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HermeApp
{
    public class UserRepository:BaseRepository<HermeAppWebUser>
    {
        public UserRepository(HermeAppWebContext context) : base(context)
        {
        }
    }
}
