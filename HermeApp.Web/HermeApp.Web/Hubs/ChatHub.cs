using HermeApp.Web.AdditionalClasses;
using Microsoft.AspNetCore.SignalR;

namespace HermeApp.Web.Hubs
{
    public class ChatHub : Hub
    {
        private IUserConnectionTracker _IconnectionTracker;

        private HermeApp.Web.AdditionalClasses.IGroupManager _GroupManager;

        public ChatHub(IUserConnectionTracker userConnectionTracker, HermeApp.Web.AdditionalClasses.IGroupManager groupManager): base() 
        { 
            _IconnectionTracker = userConnectionTracker;
            _GroupManager = groupManager;
        }

        public async Task NotifyUserWentOnline(string user)
        {
            await Clients.Others.SendAsync("UserWentOnline", user);
        }

        public async Task NotifyUserWentOffline(string user)
        {
            await Clients.Others.SendAsync("UserWentOffline", user);
        }

        public async Task NotifyWithCurrentOnlineUsers()
        {
            await Clients.Others.SendAsync("GetCurrentOnlineUsers");
        }

        public async Task SendDirectMessage(string userFrom, string userTo, string message)
        {
            await Clients.Users(userTo).SendAsync("ReceiveDirectMessage", Context.UserIdentifier, userTo, message);
        }


        public async Task JoinOrCreateAGroup(string groupName)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);

            if (!(_GroupManager.DoesGroupExist(groupName)
                &&_GroupManager.IsUserInTheGroup(groupName,Context.UserIdentifier)))
            {
                await Clients.Caller.SendAsync("JoinAGroup", groupName);

                if (!_GroupManager.DoesGroupExist(groupName))
                {
                    _GroupManager.CreateAGroup(groupName);
                }
                _GroupManager.JoinAGroup(groupName, Context.UserIdentifier);
            }
            

            //await Clients.Caller.SendAsync("JoinAGroup", groupName);


            //string response = _GroupManager.JoinAGroup(groupName, Context.UserIdentifier);
            //Clients.Caller.SendAsync("GroupJoinResponse", response);
        }

        public async Task SendAGroupMessage(string groupName, string message)
        {
            await Clients.GroupExcept(groupName, Context.ConnectionId).SendAsync("ReceiveAGroupMessage", groupName, message);
        }

        public override Task OnConnectedAsync()
        {
            Clients.Caller.SendAsync("GetCurrentOnlineUsers", _IconnectionTracker.GetUsers());
            _IconnectionTracker.UserJoined(Context.UserIdentifier);

            Clients.Caller.SendAsync("GetYourGroups", _GroupManager.GetGroupsUserBelongsTo(Context.UserIdentifier));

            Clients.Others.SendAsync("UserWentOnline", Context.UserIdentifier);

            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception? exception)
        {
            _IconnectionTracker.UserLeft(Context.UserIdentifier);

            Clients.Others.SendAsync("UserWentOffline", Context.UserIdentifier);
            return base.OnDisconnectedAsync(exception);
        }


        /*public async Task CreateAGroup(string groupName)
        {
            Groups.AddToGroupAsync(Context.ConnectionId, groupName);

            string response = _GroupManager.CreateAGroup(groupName, Context.UserIdentifier);
            //Clients.Caller.SendAsync("GroupCreationResponse", response);
        }*/
    }
}

