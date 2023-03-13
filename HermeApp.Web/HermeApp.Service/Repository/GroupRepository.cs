using HermeApp.Service.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HermeApp
{
    public class GroupRepository : BaseRepository<Group>
    {
        private HermeAppWebContext _context;
        public GroupRepository(HermeAppWebContext context) : base(context)
        {
            _context = context;
        }

        public async Task<int> FindGroupIdByName(string groupName)
        {
            var group = await _context.Groups.FirstOrDefaultAsync(g => g.GroupName == groupName);
            return group.GroupId;
        }
    }
}