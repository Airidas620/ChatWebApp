using HermeApp.Service.SingnalR;
using Microsoft.AspNetCore.SignalR;
using NuGet.Protocol.Plugins;

namespace HermeApp.Web.Hubs
{
    public class ChatHub : Hub
    {
        private IUserConnectionTracker _IconnectionTracker;

        private Service.SingnalR.IGroupManager _GroupManager;

        public ChatHub(IUserConnectionTracker userConnectionTracker, Service.SingnalR.IGroupManager groupManager) : base()
        {
            _IconnectionTracker = userConnectionTracker;
            _GroupManager = groupManager;
        }

        public override Task OnConnectedAsync()
        {
            Clients.Caller.SendAsync("GetUserName", Context.UserIdentifier);

            Clients.Caller.SendAsync("GetCurrentOnlineUsers", _IconnectionTracker.GetUsers());

            if (Context.UserIdentifier != null && !_IconnectionTracker.IsOnline(Context.UserIdentifier))
            {
                _IconnectionTracker.UserJoined(Context.UserIdentifier);

                Clients.Caller.SendAsync("GetYourGroups", _GroupManager.GetGroupsUserBelongsTo(Context.UserIdentifier));

                Clients.Others.SendAsync("UserWentOnline", Context.UserIdentifier);
            }

            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception? exception)
        {
            if (Context.UserIdentifier != null)
                _IconnectionTracker.UserLeft(Context.UserIdentifier);

            Clients.Others.SendAsync("UserWentOffline", Context.UserIdentifier);

            return base.OnDisconnectedAsync(exception);
        }

        public async Task SendDirectMessage(string sender, string receiver, string message)
        {
            await Clients.Users(receiver).SendAsync("ReceiveDirectMessage", sender, message);
        }

        public async Task JoinOrCreateAGroup(string groupName)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);

            if (!(_GroupManager.DoesGroupExist(groupName)
                && _GroupManager.IsUserInTheGroup(groupName, Context.UserIdentifier)))
            {
                await Clients.Caller.SendAsync("JoinAGroup", groupName);

                if (!_GroupManager.DoesGroupExist(groupName))
                {
                    _GroupManager.CreateAGroup(groupName);
                }
                _GroupManager.JoinAGroup(groupName, Context.UserIdentifier);
            }
        }

        public async Task SendAGroupMessage(string groupName, string sender, string message)
        {
            await Clients.GroupExcept(groupName, Context.ConnectionId).SendAsync("ReceiveAGroupMessage", groupName, sender, message);
        }
    }
}

