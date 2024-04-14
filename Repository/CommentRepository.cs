using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Data;
using api.Dtos.Comment;
using api.Interfaces;
using api.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace api.Repository
{
   
    public class CommentRepository : ICommentRepository
    {
        private readonly ApplicationDBContext _dbcontext;
        public CommentRepository(ApplicationDBContext dBContext)
        {
            _dbcontext = dBContext;
        }
        public async Task<List<Comment>> GetAllAsync()
        {
            return await _dbcontext.Comments.Include(c=>c.AppUser).ToListAsync();
        }

        public async Task<Comment?> GetById(int id)
        {
            var comment = await _dbcontext.Comments.Include(c=>c.AppUser).FirstOrDefaultAsync(c=>c.Id==id);
            return comment;
        }

        public async Task<Comment> CreateAsync(Comment comment)
        {

            await _dbcontext.Comments.AddAsync(comment);
            await _dbcontext.SaveChangesAsync();
            return comment;
        }

        public async Task<Comment?> UpdateAsync(int id, CreateCommentDto commentDto)
        {
            var comment = await GetById(id);
            if(comment == null) return null;
            comment.Content = commentDto.Content;
            comment.Title = commentDto.Title;
            await  _dbcontext.SaveChangesAsync();
            return comment;
        }

        public async Task<Comment?> DeleteAsync(int id)
        {
           var comment = await GetById(id);
           if(comment == null) return null;
            _dbcontext.Comments.Remove(comment);
            await _dbcontext.SaveChangesAsync();
            return comment;
        }
    }
}