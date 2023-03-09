﻿namespace HermeApp.Web.AdditionalClasses
{
    public class UserConnectionTracker : IUserConnectionTracker
    {
        //remove list at the end
        private HashSet<String> connectedUser;

        public UserConnectionTracker() {
            connectedUser = new HashSet<string>();
        }

        public int GetUserCount()
        {
            return connectedUser.Count;
        }

        public void UserJoined(string user)
        {
            connectedUser.Add(user);
        }

        public void UserLeft(string user)
        {
            if(connectedUser.Count > 0)
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
