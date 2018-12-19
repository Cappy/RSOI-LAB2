using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Auth.Entities;
using Auth.Helpers;
using Microsoft.EntityFrameworkCore;

namespace Auth.Services
{

    public interface IOAuth2TokenService
    {
        void AddToken(string Token, Guid UserID);
        string GetTokenByUID(Guid UserID);
        Guid GetUIDByToken(string token);
    }

    public class OAuth2TokenService : IOAuth2TokenService
    {
        private OAuth2TokensContext _context;

        public OAuth2TokenService(OAuth2TokensContext context)
        {
            _context = context;
        }

        public async void AddToken(string Token, Guid UserID)
        {
            OAuth2Tokens tk = new OAuth2Tokens
            {
                UserId = UserID,
                Token = Token,
                Revoked = 0,
                IssuedAt = DateTime.Now
            };

            OAuth2Tokens tk1 = _context.OAuth2Tokens.SingleOrDefault(m => m.UserId == UserID);

            if (tk1 != null)
            {
                _context.OAuth2Tokens.Remove(tk1);
                _context.OAuth2Tokens.Add(tk);

            }
            else
            {
                _context.OAuth2Tokens.Add(tk);
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
            var user = _context.OAuth2Tokens.SingleOrDefault(m => m.UserId == UserID);

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

        public Guid GetUIDByToken(string token)
        {
            var user = _context.OAuth2Tokens.SingleOrDefault(m => m.Token == token);

            if (user == null)
            {
                return Guid.Empty;
            }

            return user.UserId;
        }


    }
}
