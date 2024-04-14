using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using api.Dtos.Comment;
using api.Interfaces;
using api.Mappers;
using api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{
    [Route("/api/comment")]
    [ApiController]
    
    public class CommentsController : ControllerBase
    {
        private readonly ICommentRepository _commentRepo;
        private readonly IStockRepository  _stockRepo;
        private readonly UserManager<AppUser> _userManger;
        public CommentsController(ICommentRepository commentRepository,IStockRepository stockRepository,  UserManager<AppUser> userManger)
        {
            _commentRepo = commentRepository;
            _stockRepo = stockRepository;
            _userManger = userManger;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAll()
        {
            var comments = await _commentRepo.GetAllAsync();
            var commnetsDto = comments.Select(co => co.ToCommentDto());
            return Ok(commnetsDto);
        }
        [HttpGet]
        [Route("{id:int}")]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            var comment = await _commentRepo.GetById(id);
            if(comment == null) return NotFound();
            return Ok(comment.ToCommentDto());
        }

        [HttpPost("{stockId:int}")]
        [Authorize]
        public async Task<IActionResult> Create([FromRoute] int stockId,[FromBody] CreateCommentDto commentDto)
        {
            HttpContext.Request.Headers.TryGetValue("Authorization", out var authorizationHeader);
            var tokenstring = authorizationHeader.FirstOrDefault()?.Split(' ').LastOrDefault();

            var tokenHandler = new JwtSecurityTokenHandler();
            JwtSecurityToken token = tokenHandler.ReadJwtToken(tokenstring);
            var userEmail = token.Payload[JwtRegisteredClaimNames.Email]?.ToString();
            var user = await _userManger.FindByEmailAsync(userEmail);
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var stock  = await _stockRepo.GetByIdAsync(stockId);
            if( stock == null) return BadRequest("Stock Does not exisit");
            var commentModel = commentDto.ToCommentFromCreate(stockId);
            commentModel.AppUserId = user.Id;
            await _commentRepo.CreateAsync(commentModel);
            return CreatedAtAction(nameof(GetById), new{id = commentModel.Id},commentModel.ToCommentDto());
        }

        [HttpPut]
        [Route("{id:int}")]
        public async Task<IActionResult> Update([FromRoute] int id ,[FromBody] CreateCommentDto commentModel)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var comment = await _commentRepo.UpdateAsync(id, commentModel);
            if(comment == null) return BadRequest("Comment Not found");
            return CreatedAtAction( nameof(GetById), new {id = comment.Id}, comment.ToCommentDto());
        }

        [HttpDelete]
        [Route("{id:int}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            var comment = await _commentRepo.DeleteAsync(id);
            if(comment ==null) return NotFound();
            return NoContent();
        }
    }
}