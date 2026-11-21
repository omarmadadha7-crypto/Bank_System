using System;
using Data_Access.Entities;

namespace Business_Logic.Interfaces
{
    public interface ITokenService
    {
        (string Token, DateTime ExpiresAt) CreateToken(User user, string roleName);
    }
}
