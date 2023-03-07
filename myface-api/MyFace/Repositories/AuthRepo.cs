using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using MyFace.Models.Database;
using MyFace.Models.Request;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Text;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using MyFace.Models.Response;
using MyFace.Repositories;


namespace MyFace.Repositories
{
    public interface IAuthRepo
    {
        bool Authorize(string authHeader);
    }
    public class AuthRepo : IAuthRepo
    {
        private readonly MyFaceDbContext _context;
        public AuthRepo(MyFaceDbContext context)
        {
            _context = context;
        }


        public bool Authorize(string authHeader)
        {
            // Cutting off Basic from header string
            string encodedUsernamePassword = authHeader.Substring("Basic ".Length).Trim();
            // Decode username and password
            Encoding encoding = Encoding.GetEncoding("iso-8859-1");
            string usernamePassword = encoding.GetString(Convert.FromBase64String(encodedUsernamePassword));
            int seperatorIndex = usernamePassword.IndexOf(':');
            // Separate username and password into usable variables
            string pw = usernamePassword.Substring(seperatorIndex + 1);
            string username = usernamePassword.Substring(0, seperatorIndex);

            var user = _context.Users.Where(u => u.Username == username).FirstOrDefault();
            var passwordHash = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: pw,
                salt: user.Salt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 100000,
                numBytesRequested: 256 / 8));
            return user.HashedPassword == passwordHash;
        }
    }

}