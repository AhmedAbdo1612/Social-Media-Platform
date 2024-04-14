using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace api.Extensions
{
    public static class ClaimsExtension
    {
        public static string? GetUsername(this ClaimsPrincipal user)
        {
            Console.WriteLine($"\n\n{user.Claims}\n\n");
            return user.Claims.SingleOrDefault(x=>x.Type =="*")?.Value;
        }
    }
}