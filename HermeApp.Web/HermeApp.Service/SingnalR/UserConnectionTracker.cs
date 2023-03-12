namespace HermeApp.Service.SingnalR
{
    public class UserConnectionTracker : IUserConnectionTracker
    {
        private HashSet<string> connectedUser;

        public UserConnectionTracker()
        {
            connectedUser = new HashSet<string>();
        }

        public int GetUserCount()
        {
            return connectedUser.Count;
        }

        public void UserJoined(string user)
        {
            if (!IsOnline(user))
                connectedUser.Add(user);
        }
        public bool IsOnline(string user)
        {
            return connectedUser.Contains(user);
        }

        public void UserLeft(string user)
        {
            if (connectedUser.Count > 0)
            {
                connectedUser.Remove(user);
            }
        }

        public HashSet<string> GetUsers()
        {
            return connectedUser;
        }


    }
}
