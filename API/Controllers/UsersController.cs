using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{

    [Authorize]
    public class UsersController : BaseApiController
    {


     
        private readonly IMapper _mapper;
        private readonly IPhotoService _photoService;
        private readonly IUnitOfWork _unitOfWork;

        public UsersController(IMapper mapper, IUnitOfWork unitOfWork,
        IPhotoService photoService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _photoService = photoService;

        }


        [HttpGet]
        public async Task<ActionResult<PagedList<MemberDto>>> GetUsers([FromQuery]UserParams userParams)
        {
            var currentUser = await _unitOfWork.userRepository.GetUserByUsernameAsync(User.GetUserName());
            userParams.CurrentUsername = currentUser.UserName;
            var users = await _unitOfWork.userRepository.GetMembersAsync(userParams);
            Response.AddPaginationHeader(new PaginationHeader(users.CurrentPage, users.PageSize, 
            users.TotalCount, users.TotalPages));
          
            return Ok(users);

        }


        [HttpGet("{username}")]
        public async Task<ActionResult<MemberDto>> GetUser(string username)
        {
            return await _unitOfWork.userRepository.GetMemberAsync(username);

            
        }

        [HttpPut]
        public async Task<ActionResult> UpdateUser(UpdateMemberDTO updateMemberDTO)
        {
            
            var user = await _unitOfWork.userRepository.GetUserByUsernameAsync(User.GetUserName());

            if(user == null) return NotFound();

            _mapper.Map(updateMemberDTO, user);

            if(await _unitOfWork.Complete()) return NoContent();
            return BadRequest("User update failed");
        }

        [HttpPost("photoUploader")]
        public async Task<ActionResult<PhotoDto>> AddPhoto(IFormFile file)
        {
            var user = await _unitOfWork.userRepository.GetUserByUsernameAsync(User.GetUserName());
             if(user == null) return NotFound();

             var result = await _photoService.AddPhotoAsync(file);
             if(result.Error != null) return BadRequest(result.Error.Message);
             var photo = new Photo{
                Url = result.SecureUrl.AbsoluteUri,
                PublicId = result.PublicId
             };

             if (user.Photos.Count == 0)
             {
                photo.IsProfile = true;
             }

             user.Photos.Add(photo);
             if(await _unitOfWork.Complete())
             {
                return CreatedAtAction(nameof(GetUser),new {username = user.UserName},
                 _mapper.Map<PhotoDto>(photo));
             } 

             return BadRequest("problem Uploading photo");
        }

        [HttpPut("setProfilePic/{photoId}")]
        public async Task<ActionResult> SetProfilePic(int photoId)
        {
            var user = await _unitOfWork.userRepository.GetUserByUsernameAsync(User.GetUserName());
             if(user == null) return NotFound();
             var photo = user.Photos.FirstOrDefault(x=>x.Id == photoId);
             if(photo == null) return NotFound();
             if(photo.IsProfile) return BadRequest("Already profile picture");

             var profilePic = user.Photos.FirstOrDefault(x=>x.IsProfile);
             if(profilePic !=null) profilePic.IsProfile = false;
             photo.IsProfile = true;

             if(await _unitOfWork.Complete()) return NoContent();

             return BadRequest("profile photo update failed....Try again!!!");
        }

        [HttpDelete("deletePhoto/{photoId}")]

        public async Task<ActionResult> DeletePhoto(int photoId)
        {
            var user = await _unitOfWork.userRepository.GetUserByUsernameAsync(User.GetUserName());
             if(user == null) return NotFound();

             var photo = user.Photos.FirstOrDefault(x=>x.Id == photoId);
             if(photo == null) return NotFound();
             if(photo.IsProfile) return BadRequest("Photo set a profile picture. Unable to delete");

             if(photo.PublicId != null)
             {
                var result = await _photoService.DeletePhotoAsync(photo.PublicId);
                if(result.Error != null) return BadRequest(result.Error.Message);
            
             }

             user.Photos.Remove(photo);
            if( await _unitOfWork.Complete()) return Ok();
            return BadRequest("problem occured while deleting photo....Try again!!!");
        }
        
    }
}