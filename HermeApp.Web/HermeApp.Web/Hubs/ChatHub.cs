using HermeApp.Service.SingnalR;
using Microsoft.AspNetCore.SignalR;
using NuGet.Protocol.Plugins;
using System.Text.RegularExpressions;

namespace HermeApp.Web.Hubs
{
    public class ChatHub : Hub
    {
        private IUserConnectionTracker _IconnectionTracker;

        private Service.SingnalR.IGroupManager _GroupManager;

        private GroupRepository _groupRepository;

        private MessageRepository _messageRepository;

        private UserGroupRepository _userGroupRepository;

        private UserRepository _userRepository;

        public ChatHub(IUserConnectionTracker userConnectionTracker, Service.SingnalR.IGroupManager groupManager,
            GroupRepository groupRepository, MessageRepository messageRepository, UserGroupRepository userGroupRepository,
            UserRepository userRepository) : base()
        {
            _IconnectionTracker = userConnectionTracker;
            _GroupManager = groupManager;
            _groupRepository = groupRepository;
            _userGroupRepository = userGroupRepository;
            _userRepository = userRepository;
            _messageRepository = messageRepository;
        }

        public override Task OnConnectedAsync()
        {
            Clients.Caller.SendAsync("GetUserName", Context.UserIdentifier);

            Clients.Caller.SendAsync("GetCurrentOnlineUsers", _IconnectionTracker.GetUsers());

            if (Context.UserIdentifier != null && !_IconnectionTracker.IsOnline(Context.UserIdentifier)) {
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
            string senderId = await _userRepository.FindIdByName(sender);
            string receiverId = await _userRepository.FindIdByName(receiver);

            Message _message = new Message();
            _message.SenderId = senderId;
            _message.ReceiverId = receiverId;
            _message.MessageText = message;
            _message.Timestamp = DateTime.Now;
            await _messageRepository.CreateAsync(_message);
            await Clients.Users(receiver).SendAsync("ReceiveDirectMessage", sender, message);
        }

        public async Task JoinOrCreateAGroup(string groupName, string userName)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);

            if (!(_GroupManager.DoesGroupExist(groupName)
                && _GroupManager.IsUserInTheGroup(groupName, Context.UserIdentifier))) {
                await Clients.Caller.SendAsync("JoinAGroup", groupName);

                if (!_GroupManager.DoesGroupExist(groupName)) {
                    Group _group = new Group();
                    _group.GroupName = groupName;
                    await _groupRepository.CreateAsync(_group);
                    _GroupManager.CreateAGroup(groupName);
                }
                string userId = await _userRepository.FindIdByName(userName);

                Group group = await _groupRepository.FindGroupIdByName(groupName);

                UserGroup userGroup = new UserGroup();
                
                userGroup.GroupId = group.GroupId;
                userGroup.UserId = userId;
                userGroup.Group = group;
                await _userGroupRepository.CreateAsync(userGroup);

                _GroupManager.JoinAGroup(groupName, Context.UserIdentifier);
            }
        }

        public async Task SendAGroupMessage(string groupName, string sender, string message)
        {
            string senderId = await _userRepository.FindIdByName(sender);
            Group group = await _groupRepository.FindGroupIdByName(groupName);
            Message _message = new Message();
            _message.SenderId = senderId;
            _message.MessageText = message;
            _message.GroupId = group.GroupId;
            _message.Group = group;
            _message.Timestamp = DateTime.Now;
            await _messageRepository.CreateAsync(_message);
            await Clients.GroupExcept(groupName, Context.ConnectionId).SendAsync("ReceiveAGroupMessage", groupName, sender, message);
        }
    }
}

