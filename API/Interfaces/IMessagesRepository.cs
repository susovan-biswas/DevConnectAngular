using API.DTOs;
using API.Entities;
using API.Helpers;

namespace API.Interfaces
{
    public interface IMessagesRepository
    {
        void AddMessage(Messages message);
        void DeleteMessage(Messages message);

        Task<Messages> GetMessage(int messageId);

        Task<PagedList<MessageDto>> GetUserMessages(MessagesParams messagesParams);

        Task<IEnumerable<MessageDto>> GetMessageThread(string currentUserName, string recipientUsername);

        Task<bool> SaveAllAsync();
        void AddGroup(Group group);
        void RemoveConnection(Connections connection);
        Task<Connections> GetConnection(string connectionId);
         Task<Group> GetMessageGroups(string groupName);
        Task<Group> GetGroupForConnection(string connectionId);
    }
}