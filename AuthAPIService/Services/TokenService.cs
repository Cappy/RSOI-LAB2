using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Auth.Entities;
using Auth.Helpers;
using Microsoft.EntityFrameworkCore;

namespace Auth.Services
{

    public interface ITokenService
    {
        void AddToken(string Token, Guid UserID);
        string GetTokenByUID(Guid UserID);
    }

    public class TokenService : ITokenService
    {
        private TokensContext _context;

        public TokenService(TokensContext context)
        {
            _context = context;
        }

        public async void AddToken(string Token, Guid UserID)
        {
            Tokens tk = new Tokens
            {
                UserId = UserID,
                Token = Token,
                Revoked = 0,
                IssuedAt = DateTime.Now
            };

            Tokens tk1 = _context.Tokens.SingleOrDefault(m => m.UserId == UserID);

            if (tk1 != null)
            {
                _context.Tokens.Remove(tk1);
                _context.Tokens.Add(tk);

            }
            else
            {
                _context.Tokens.Add(tk);
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                    throw;
            }

        }

        public string GetTokenByUID(Guid UserID)
        {
            var user = _context.Tokens.SingleOrDefault(m => m.UserId == UserID);

            if (user == null)
            {
                return null;
            }

            if (user.IssuedAt.AddMinutes(30) < DateTime.Now)
            {
                return null;
            }

            return user.Token;
        }


    }
}
