using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace API.SignalR
{
    [Authorize]
    public class MessageHub : Hub
    {
        private readonly IMessagesRepository _messagesRepository;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly IHubContext<PresenceHub> _presenceHub;
        public MessageHub(IMessagesRepository messagesRepository, IUserRepository userRepository, 
                          IMapper mapper, IHubContext<PresenceHub> presenceHub)
        {
            _presenceHub = presenceHub;
            _mapper = mapper;
            _userRepository = userRepository;
            _messagesRepository = messagesRepository;
            
        }

        public override async Task OnConnectedAsync()
        {
           var httpContext = Context.GetHttpContext();
           var otherUser = httpContext.Request.Query["user"];
           var groupName = GetGroupName(Context.User.GetUserName(), otherUser);
           await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
           var group = await AddToGroup(groupName);
           await Clients.Group(groupName).SendAsync("UpdatedGroup", group);



           var messages = await _messagesRepository.GetMessageThread(Context.User.GetUserName(), otherUser);
           await Clients.Caller.SendAsync("RecieveMessageThread", messages);


        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
           var group = await RemovedFromMessageGroup();
           await Clients.Group(group.Name).SendAsync("UpdatedGroup");
            await base.OnDisconnectedAsync(exception);
        }

        private string GetGroupName(string caller, string other)
        {
            var stringCompare = string.CompareOrdinal(caller,other) < 0;
            return stringCompare ? $"{caller}-{other}" : $"{other}-{caller}";
        }

        public async Task SendMessage(CreateMessageDto createMessageDto)
        {
            var username = Context.User.GetUserName();
            if (username.ToLower() == createMessageDto.Recipient.ToLower()) throw new HubException("Cannot message you own self");
            var sender = await _userRepository.GetUserByUsernameAsync(username);
            var recipient = await _userRepository.GetUserByUsernameAsync(createMessageDto.Recipient.ToLower());


            if (recipient == null) throw new HubException("Recipient Not Found");
            var message = new Messages
            {
                Sender = sender,
                Recipient = recipient,
                SenderUsername = sender.UserName,
                RecipientUsername = recipient.UserName,
                MessageBody = createMessageDto.MessageBody
               
            };

            var groupName = GetGroupName(sender.UserName, recipient.UserName);
            var group = await _messagesRepository.GetMessageGroups(groupName);

            if(group.Connections.Any(x=>x.Username == recipient.UserName))
            {
                message.DateRead = DateTime.UtcNow;
            }
            else{
                var connections = await PresenceTracker.GetConnectionForUser(recipient.UserName);
                if(connections != null)
                {
                    await _presenceHub.Clients.Clients(connections).SendAsync("NewMessageReceived", 
                    new {username = sender.UserName, fullName=sender.FullName});
                }
            }

            _messagesRepository.AddMessage(message);
            
            if(await _messagesRepository.SaveAllAsync())
            {
                
                await Clients.Groups(groupName).SendAsync("NewMessage", _mapper.Map<MessageDto>(message));
            }             

           
        }

        private async Task<Group> AddToGroup(string groupName){
            var group = await _messagesRepository.GetMessageGroups(groupName);
            var connection = new Connections(Context.ConnectionId, Context.User.GetUserName());

            if(group == null)
            {
                group = new Group(groupName);
                _messagesRepository.AddGroup(group);
            }

            group.Connections.Add(connection);
            if (await _messagesRepository.SaveAllAsync()) return group;

            throw new HubException("Failed to add to group");
        }

        private async Task<Group> RemovedFromMessageGroup()
        {
            var group = await _messagesRepository.GetGroupForConnection(Context.ConnectionId);
            var connection = group.Connections.FirstOrDefault(x=>x.ConnectionId == Context.ConnectionId);
            _messagesRepository.RemoveConnection(connection);
            if(await _messagesRepository.SaveAllAsync()) return group;

            throw new HubException("Failed to remove from group");
        }



    }
}