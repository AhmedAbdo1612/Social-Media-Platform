using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using api.Extensions;
using api.Interfaces;
using api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace api.Controllers
{
    [Route("/api/portfolio")]
    [ApiController]
    public class PortfoliosController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IStockRepository _stockRepo;
        private readonly IPortfolioRepository _portfolioRepo;

        public PortfoliosController(IStockRepository stockRepo,
        UserManager<AppUser> userManager, IPortfolioRepository portfolioRepo)
        {
            _stockRepo = stockRepo;
            _userManager = userManager;
            _portfolioRepo = portfolioRepo;
        }
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetUserProtfolio()
        {
            HttpContext.Request.Headers.TryGetValue("Authorization", out var authorizationHeader);
            var tokenstring = authorizationHeader.FirstOrDefault()?.Split(' ').LastOrDefault();

            var tokenHandler = new JwtSecurityTokenHandler();
            JwtSecurityToken token = tokenHandler.ReadJwtToken(tokenstring);
            var userEmail = token.Payload[JwtRegisteredClaimNames.Email]?.ToString();
            var user = await _userManager.FindByEmailAsync(userEmail);
            if (user == null)
                return BadRequest("Unauthorized");
            var userPortfolios = await _portfolioRepo.GetUserPostfolio(user);
            return Ok(userPortfolios);
        }
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddPortfolio(string symbol)
        {
            HttpContext.Request.Headers.TryGetValue("Authorization", out var authorizationHeader);
            var tokenstring = authorizationHeader.FirstOrDefault()?.Split(' ').LastOrDefault();

            var tokenHandler = new JwtSecurityTokenHandler();
            JwtSecurityToken token = tokenHandler.ReadJwtToken(tokenstring);
            var userEmail = token.Payload[JwtRegisteredClaimNames.Email]?.ToString();
            var user = await _userManager.FindByEmailAsync(userEmail);
           
            var stock = await _stockRepo.GetBySymbolAsync(symbol);
            if (stock == null) return BadRequest("Stock not found");
            var userPortfolio = await _portfolioRepo.GetUserPostfolio(user);
            if (userPortfolio.Any(e => e.Symbol.ToLower() == symbol.ToLower()))
                return BadRequest("Cannot add same stock to portfolio");
            var portfolioModel = new Portfolio{
                StockId = stock.Id,
                AppUserId = user.Id
            };
            await _portfolioRepo.CreateAsync(portfolioModel);

            return Created();
        }
        [HttpDelete]
        [Authorize]
        public async Task<IActionResult> DeletePortFolio(string symbol)
        {
             HttpContext.Request.Headers.TryGetValue("Authorization", out var authorizationHeader);
            var tokenstring = authorizationHeader.FirstOrDefault()?.Split(' ').LastOrDefault();

            var tokenHandler = new JwtSecurityTokenHandler();
            JwtSecurityToken token = tokenHandler.ReadJwtToken(tokenstring);
            var userEmail = token.Payload[JwtRegisteredClaimNames.Email]?.ToString();
            var user = await _userManager.FindByEmailAsync(userEmail);
            var userPortfolios = await _portfolioRepo.GetUserPostfolio(user);
            var filteredPortdolios = userPortfolios.Where(x=>x.Symbol.ToLower() == symbol.ToLower());
            if (filteredPortdolios.Count() >=1)
            {
                await _portfolioRepo.DeletePortfolio(user,filteredPortdolios.FirstOrDefault().Id);
            }
            else return BadRequest("Stock not in your portfolio");
            return Ok();

        }
    }
}