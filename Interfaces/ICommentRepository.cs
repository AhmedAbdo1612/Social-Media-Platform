using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Dtos.Comment;
using api.Models;

namespace api.Interfaces
{
    public interface ICommentRepository
    {
        Task<List<Comment>> GetAllAsync();
        Task<Comment?> GetById( int id);
        Task<Comment> CreateAsync( Comment comment);
        Task<Comment?> UpdateAsync(int id, CreateCommentDto comment);
        Task<Comment?> DeleteAsync(int id);
    }
}