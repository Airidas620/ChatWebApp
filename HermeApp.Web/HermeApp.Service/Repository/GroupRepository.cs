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

        public async Task<Group> FindGroupIdByName(string groupName)
        {
            var group = await _context.Groups.FirstOrDefaultAsync(g => g.GroupName == groupName);
            return group;
        }

        public async Task<List<Group>> GetAllGroups(string userId)
        {
            var groupIds = await _context.UserGroups
                        .Where(ug => ug.UserId == userId)
                        .Select(ug => ug.GroupId)
                        .ToListAsync();

            // Select all groups from the Group table using the group IDs
            var groups = await _context.Groups
                .Where(g => groupIds.Contains(g.GroupId))
                .ToListAsync();

            return groups;
        }
    }
}