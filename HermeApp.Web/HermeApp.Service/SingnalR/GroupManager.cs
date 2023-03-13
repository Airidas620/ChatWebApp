
namespace HermeApp.Service.SingnalR
{
    public class GroupManager : IGroupManager
    {
        Dictionary<string, HashSet<string>> groups;

        public GroupManager()
        {
            groups = new Dictionary<string, HashSet<string>>();
        }

        public string CreateAGroup(string groupName)
        {
            if (groups.ContainsKey(groupName))
            {
                return "Group already exist";
            }

            groups.Add(groupName, new HashSet<string>());

            return "Group created";
        }

        public bool DoesGroupExist(string groupName)
        {
            return groups.ContainsKey(groupName);
        }

        public List<string> GetGroupsUserBelongsTo(string user)
        {
            List<string> userGroups = new List<string>();

            foreach (var group in groups.Where(x => x.Value.All(y => y.Contains(user))))
            {
                userGroups.Add(group.Key);
            }
            return userGroups;
        }

        public bool IsUserInTheGroup(string groupName, string user)
        {
            return groups[groupName].Contains(user);
        }

        public string JoinAGroup(string groupName, string user)
        {
            if (!groups.ContainsKey(groupName))
            {
                return "Group doesn't exist";
            }

            if (groups[groupName].Contains(user))
            {
                return "You're already in the group";
            }

            groups[groupName].Add(user);

            return "Group joined";
        }
    }
}
