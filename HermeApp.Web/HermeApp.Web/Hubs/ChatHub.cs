using HermeApp.Web.AdditionalClasses;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;

namespace HermeApp.Web.Hubs
{
    public class ChatHub : Hub
    {
        private IUserConnectionTracker _IconnectionTracker;
        private GroupRepository _groupRepository;
        private MessageRepository _messageRepository;
        private UserGroupRepository _userGroupRepository;
        private UserRepository _userRepository;
        private HermeApp.Web.AdditionalClasses.IGroupManager _GroupManager;

        public ChatHub(IUserConnectionTracker userConnectionTracker, HermeApp.Web.AdditionalClasses.IGroupManager groupManager,
            GroupRepository groupRepository, MessageRepository messageRepository, UserGroupRepository userGroupRepository,
            UserRepository userRepository, UserManager<IdentityUser> userManager) : base()
        {
            _IconnectionTracker = userConnectionTracker;
            _GroupManager = groupManager;
            _groupRepository = groupRepository;
            _userGroupRepository = userGroupRepository;
            _userRepository = userRepository;
            _messageRepository = messageRepository;
        }

        public async Task NotifyUserWentOnline(string user)
        {
            //&& user != Context.UserIdentifier
            if (!_IconnectionTracker.IsOnline(user) )
            {
                _IconnectionTracker.UserJoined(user);
                await Clients.Others.SendAsync("UserWentOnline", user);
            }
        }

        public async Task NotifyUserWentOffline(string user)
        {
            _IconnectionTracker.UserLeft(user);
            await Clients.Others.SendAsync("UserWentOffline", user);
        }

        public async Task SendDirectMessage(string userFrom, string userTo, string message)
        {
            Message _message = new Message();
            _message.SenderId = userFrom;
            _message.ReceiverId = userTo;
            _message.MessageText = message;
            _message.Timestamp = DateTime.Now;
            await _messageRepository.CreateAsync(_message);
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
                    Group _group = new Group();
                    _group.GroupName = groupName;
                    await _groupRepository.CreateAsync(_group);
                    _GroupManager.CreateAGroup(groupName);
                }
                int groupId = await _groupRepository.FindGroupIdByName(groupName);
                UserGroup userGroup = new UserGroup();
                userGroup.GroupId = groupId;
                userGroup.UserId = Context.UserIdentifier;
                await _userGroupRepository.CreateAsync(userGroup);
                _GroupManager.JoinAGroup(groupName, Context.UserIdentifier);
            }
        }

        public async Task SendAGroupMessage(string groupName, string userFrom, string message)
        {
            int? groupId = await _groupRepository.FindGroupIdByName(groupName);
            Message _message = new Message();
            _message.SenderId = userFrom;
            _message.MessageText = message;
            _message.GroupId = groupId;
            _message.Timestamp = DateTime.Now;
            await _messageRepository.CreateAsync(_message);
            await Clients.GroupExcept(groupName, Context.ConnectionId).SendAsync("ReceiveAGroupMessage", groupName, message);
        }

        public override Task OnConnectedAsync()
        {
            Clients.Caller.SendAsync("GetCurrentOnlineUsers", _IconnectionTracker.GetUsers());

            if(Context.UserIdentifier != null)
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
    }
}

