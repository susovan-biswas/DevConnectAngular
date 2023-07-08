using API.Data;
using API.DTOs;
using API.Entities;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace API.Respositories
{
    public class MessageRepository : IMessagesRepository
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;
        public MessageRepository(DataContext context, IMapper mapper)
        {
            _mapper = mapper;
            _context = context;
            
        }
        public void AddMessage(Messages message)
        {
            _context.Messages.Add(message);
        }

        public void DeleteMessage(Messages message)
        {
            _context.Messages.Remove(message);
        }

        public async Task<Messages> GetMessage(int messageId)
        {
            return await _context.Messages.FindAsync(messageId);
        }

        public async Task<IEnumerable<MessageDto>> GetMessageThread(string currentUserName, string recipientUsername)
        {
            var messages = await _context.Messages
            .Include(s => s.Sender).ThenInclude(p =>p.Photos)
            .Include(r => r.Recipient).ThenInclude(p =>p.Photos)
            .Where(
                m => m.RecipientUsername == currentUserName && m.RecipientDeleted == false 
                     && m.SenderUsername == recipientUsername
                     ||m.RecipientUsername == recipientUsername && m.SenderDeleted == false 
                     && m.SenderUsername == currentUserName
            )
            .OrderByDescending(m =>m.MessageSentAt).ToListAsync();

            var unreadMessages = messages.Where(m => m.DateRead == null 
            && m.RecipientUsername == currentUserName).ToList();

            if(unreadMessages.Any())
            {
                foreach(var message in unreadMessages)
                {
                    message.DateRead =DateTime.UtcNow;
                }

                await _context.SaveChangesAsync();
            }

            return _mapper.Map<IEnumerable<MessageDto>>(messages);
        }

        public async Task<PagedList<MessageDto>> GetUserMessages(MessagesParams messagesParams)
        {
            var query = _context.Messages.OrderByDescending(x=> x.MessageSentAt).AsQueryable();

            query = messagesParams.Container switch
            {
                "Inbox" => query.Where(u =>u.RecipientUsername == messagesParams.Username && u.RecipientDeleted==false),
                "Outbox" => query.Where(u=>u.SenderUsername == messagesParams.Username && u.SenderDeleted == false),
                _ => query.Where(u=>u.RecipientUsername == messagesParams.Username && u.DateRead == null && u.RecipientDeleted == false)
            };

            var messages = query.ProjectTo<MessageDto>(_mapper.ConfigurationProvider);

            return await PagedList<MessageDto>.CreateAsync(messages, messagesParams.PageNumber, messagesParams.PageSize);
        }

        public async Task<bool> SaveAllAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }
    }
}