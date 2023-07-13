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
      
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        
        public MessagesController(IMapper mapper, IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;            
            _mapper = mapper;
            
        }

        [HttpPost]
        public async Task<ActionResult<MessageDto>> CreateMessage(CreateMessageDto createMessageDto)
        {
            var username = User.GetUserName();
            if(username.ToLower() == createMessageDto.Recipient.ToLower()) return BadRequest("you cannot send Message to yourself");
            var sender = await _unitOfWork.userRepository.GetUserByUsernameAsync(username);
            var recipient = await _unitOfWork.userRepository.GetUserByUsernameAsync(createMessageDto.Recipient.ToLower());

            if(recipient == null) return NotFound();

            var message = new Messages
            {
                Sender = sender,
                Recipient = recipient,
                SenderUsername = sender.UserName,
                RecipientUsername = recipient.UserName,
                MessageBody = createMessageDto.MessageBody
            };

            _unitOfWork.messagesRepository.AddMessage(message);
            
            if(await _unitOfWork.Complete()) return Ok(_mapper.Map<MessageDto>(message));

            return BadRequest("Failed to send message");
        }

        [HttpGet]

        public async Task<ActionResult<PagedList<MessageDto>>> GetUserMessages([FromQuery]MessagesParams messagesParams)
        {
            messagesParams.Username = User.GetUserName();
            var message = await _unitOfWork.messagesRepository.GetUserMessages(messagesParams);

            Response.AddPaginationHeader(new PaginationHeader(message.CurrentPage, message.PageSize, message.TotalCount, message.TotalPages));

            return message;

        }

        // [HttpGet("thread/{username}")]
        // public async Task<ActionResult<IEnumerable<MessageDto>>> GetMessageThread(string username)
        // {
        //     var currentUserName = User.GetUserName();
        //     return Ok(await _unitOfWork.messagesRepository.GetMessageThread(currentUserName, username));
        // }

        [HttpDelete("{id}")]

        public async Task<ActionResult> DeleteMessage(int id)
        {
            var username= User.GetUserName();
            var message = await _unitOfWork.messagesRepository.GetMessage(id);

            if(message.SenderUsername != username && message.RecipientUsername != username) 
            return Unauthorized();

            if(message.SenderUsername == username) message.SenderDeleted = true;

            if(message.RecipientUsername == username) message.RecipientDeleted = true;

            if(message.SenderDeleted && message.RecipientDeleted) 
            {
                _unitOfWork.messagesRepository.DeleteMessage(message);
            }

            if(await _unitOfWork.Complete()) return Ok();

            return BadRequest("Problem Deleting Message...Please try again");
        }
    }
}
