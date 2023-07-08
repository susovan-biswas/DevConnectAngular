using System.Runtime.Versioning;
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class MessagesController:BaseApiController
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly IMessagesRepository _messagesRepository;
        public MessagesController(IUserRepository userRepository, IMessagesRepository messagesRepository, IMapper mapper)
        {
            _messagesRepository = messagesRepository;
            _mapper = mapper;
            _userRepository = userRepository;
            
        }

        [HttpPost]
        public async Task<ActionResult<MessageDto>> CreateMessage(CreateMessageDto createMessageDto)
        {
            var username = User.GetUserName();
            if(username.ToLower() == createMessageDto.Recipient.ToLower()) return BadRequest("you cannot send Message to yourself");
            var sender = await _userRepository.GetUserByUsernameAsync(username);
            var recipient = await _userRepository.GetUserByUsernameAsync(createMessageDto.Recipient.ToLower());

            if(recipient == null) return NotFound();

            var message = new Messages
            {
                Sender = sender,
                Recipient = recipient,
                SenderUsername = sender.UserName,
                RecipientUsername = recipient.UserName,
                MessageBody = createMessageDto.MessageBody
            };

            _messagesRepository.AddMessage(message);
            
            if(await _messagesRepository.SaveAllAsync()) return Ok(_mapper.Map<MessageDto>(message));

            return BadRequest("Failed to send message");
        }

        [HttpGet]

        public async Task<ActionResult<PagedList<MessageDto>>> GetUserMessages([FromQuery]MessagesParams messagesParams)
        {
            messagesParams.Username = User.GetUserName();
            var message = await _messagesRepository.GetUserMessages(messagesParams);

            Response.AddPaginationHeader(new PaginationHeader(message.CurrentPage, message.PageSize, message.TotalCount, message.TotalPages));

            return message;

        }

        [HttpGet("thread/{username}")]
        public async Task<ActionResult<IEnumerable<MessageDto>>> GetMessageThread(string username)
        {
            var currentUserName = User.GetUserName();
            return Ok(await _messagesRepository.GetMessageThread(currentUserName, username));
        }

        [HttpDelete("{id}")]

        public async Task<ActionResult> DeleteMessage(int id)
        {
            var username= User.GetUserName();
            var message = await _messagesRepository.GetMessage(id);

            if(message.SenderUsername != username && message.RecipientUsername != username) 
            return Unauthorized();

            if(message.SenderUsername == username) message.SenderDeleted = true;

            if(message.RecipientUsername == username) message.RecipientDeleted = true;

            if(message.SenderDeleted && message.RecipientDeleted) 
            {
                _messagesRepository.DeleteMessage(message);
            }

            if(await _messagesRepository.SaveAllAsync()) return Ok();

            return BadRequest("Problem Deleting Message...Please try again");
        }
    }
}
