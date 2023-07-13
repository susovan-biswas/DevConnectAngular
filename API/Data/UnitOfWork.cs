using API.Interfaces;
using API.Respositories;
using AutoMapper;

namespace API.Data
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly IMapper _mapper;
        private readonly DataContext _context ;
        public UnitOfWork(DataContext context, 
                          IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
            
        }
        public IUserRepository userRepository => new UserRepository(_context, _mapper);

        public IMessagesRepository messagesRepository => new MessageRepository(_context, _mapper);

        public ILikesRepository likesRepository => new LikesRepository(_context);

        public async Task<bool> Complete()
        {
            return await _context.SaveChangesAsync() > 0;
        }

        public bool HasChanges()
        {
            return  _context.ChangeTracker.HasChanges();
        }
    }
}