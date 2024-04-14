using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Dtos.Comment;
using api.Models;

namespace api.Mappers
{
    public static class CommentMapper
    {
        public static CommentDtos ToCommentDto(this Comment commentModel)
        {
            return new CommentDtos{
                Id = commentModel.Id,
                Title = commentModel.Title,
                Content = commentModel.Content,
                CreatedOn = commentModel.CreatedOn,
                StockId = commentModel.StockId,
                CreatedBy = commentModel.AppUser.UserName
            };
        }

         public static Comment ToCommentFromCreate(this CreateCommentDto CommentDto, int stockId)
        {
            return new Comment{
                Title = CommentDto.Title,
                Content = CommentDto.Content,
                StockId = stockId
            };
        }
    }
}