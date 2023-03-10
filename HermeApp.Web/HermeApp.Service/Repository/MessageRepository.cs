using HermeApp.Service.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HermeApp
{
    public class MessageRepository : BaseRepository<Message>
    {
        public MessageRepository(HermeAppWebContext context) : base(context)
        {
        }
    }
}
