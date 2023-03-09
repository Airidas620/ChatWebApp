namespace HermeApp.Web.AdditionalClasses
{
    public interface IGroupManager
    {
        public string JoinAGroup(string groupName, string user);

        public string CreateAGroup(string groupName);

        public bool DoesGroupExist(string groupName);

        public bool IsUserInTheGroup(string groupName, string user);

        public List<string> GetGroupsUserBelongsTo(string user);

    }
}
