using EntityLayer.Concrete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityLayer.DTOs
{
    public class LoginDTO
    {
        public string Email { get; set; }
        public string Password { get; set; }
        
        public static implicit operator User(LoginDTO loginDTO)
        {
            return new User
            {
                Email = loginDTO.Email,
                Password = loginDTO.Password
            };
        }
    }
}
