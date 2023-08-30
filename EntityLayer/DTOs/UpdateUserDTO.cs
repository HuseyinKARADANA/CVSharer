using EntityLayer.Concrete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityLayer.DTOs
{
    public class UpdateUserDTO
    {
        public int UserId { get; set; }
        public byte[]? Photo { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string? Description { get; set; }
        public string? Position { get; set; }
        public string? Phone{ get; set; }
        public string? Address { get; set; }
        public string? Linkedin { get; set; }
        public string? Instagram { get; set; }
        public string? GitHub { get; set; }
        public string? YouTube { get; set; }

        public static implicit operator User(UpdateUserDTO dto)
        {
            return new User
            {
                UserId = dto.UserId,
                Photo = dto.Photo,
                Name = dto.Name,
                Surname = dto.Surname,
                Description = dto.Description,
                Position = dto.Position,
                Phone = dto.Phone,
                Address = dto.Address,
                Linkedin = dto.Linkedin,
                Instagram = dto.Instagram,
                GitHub = dto.GitHub,
                YouTube = dto.YouTube,
            };
        }
    }
}
