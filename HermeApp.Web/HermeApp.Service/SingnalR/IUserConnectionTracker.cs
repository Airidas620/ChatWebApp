namespace HermeApp.Service.SingnalR
{
    public interface IUserConnectionTracker
    {
        public void UserJoined(string user);

        public void UserLeft(string user);

        public HashSet<string> GetUsers();

        public bool IsOnline(string user);
    }
}
