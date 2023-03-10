namespace HermeApp.Web.AdditionalClasses
{
    public interface IUserConnectionTracker
    {
        public void UserJoined(string user);

        public void UserLeft(string user);

        public HashSet<String> GetUsers();

        public bool IsOnline(string user);
    }
}
