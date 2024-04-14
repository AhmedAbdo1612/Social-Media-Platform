using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Data;
using api.Dtos.Stock;
using api.Helpers;
using api.Interfaces;
using api.Models;
using Microsoft.EntityFrameworkCore;

namespace api.Repository
{
    public class StockRepository : IStockRepository
    {
        private readonly ApplicationDBContext _dbcontext;
        public StockRepository(ApplicationDBContext dbcontext)
        {
            _dbcontext = dbcontext;
        }

        public async Task<Stock> CreateAsync(Stock stockModel)
        {
            await _dbcontext.Stocks.AddAsync(stockModel);
            await _dbcontext.SaveChangesAsync();
            return stockModel;
        }

        public async Task<Stock?> DeleteAsync(int id)
        {
            var stock = await _dbcontext.Stocks.FindAsync(id);
            if(stock == null) return null;
             _dbcontext.Stocks.Remove(stock);
             await _dbcontext.SaveChangesAsync();
             return stock;
        }

        public  async Task<List<Stock>> GetAsllAsync(QueryObject query)
        {
            var stocks =  _dbcontext.Stocks.Include(c=>c.Comments).ThenInclude(a=>a.AppUser).AsQueryable();
            if(!string.IsNullOrWhiteSpace(query.CompanyName))
            {
                stocks = stocks.Where(s=>s.CompanyName.Contains(query.CompanyName));
            }
            if(!string.IsNullOrWhiteSpace(query.Symbol))
            {
                stocks = stocks.Where(s=>s.CompanyName.Contains(query.Symbol));
            }
            if(!string.IsNullOrWhiteSpace(query.SortBy))
            {
                if(query.SortBy.Equals("Symbol", StringComparison.OrdinalIgnoreCase))
                {
                    stocks = query.IsDecsending? stocks.OrderByDescending(s=>s.Symbol):stocks.OrderBy(s=>s.Symbol);
                }
            }
            query.PageNumber = query.PageNumber>=1? query.PageNumber: 1;
            var skipNumber = (query.PageNumber-1) * query.PageSize;
            return await stocks.Skip(skipNumber).Take(query.PageSize).ToListAsync();
        }

        public async Task<Stock?> GetByIdAsync(int id)
        {
            var stock = await _dbcontext.Stocks.Include(c=>c.Comments).FirstOrDefaultAsync(st=>st.Id == id);
            return stock;
        }

        public async Task<Stock?> GetBySymbolAsync(string symbol)
        {
            var stock  = await _dbcontext.Stocks.FirstOrDefaultAsync(s=>s.Symbol==symbol);
            return stock;
            
        }

        public async Task<Stock?> UpdateAsync(int id, UpdateStockRequestDto stockDto)
        {
            var existingStock = await GetByIdAsync(id);
            if(existingStock == null) return null;
            existingStock.Symbol = stockDto.Symbol;
            existingStock.CompanyName = stockDto.CompanyName;
            existingStock.Purchase = stockDto.Purchase;
            existingStock.LastDiv = stockDto.LastDiv;
            existingStock.Industry = stockDto.Industry;
            existingStock.MarketCap = stockDto.MarketCap;
            await _dbcontext.SaveChangesAsync();
            return existingStock;
        }
    }
}