using EntityLayer.Concrete;
using Microsoft.AspNetCore.Http;
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
        public IFormFile? Photo { get; set; }
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
            
            var userImplict= new User
            {
                UserId = dto.UserId,


                
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

            if (dto.Photo != null)
            {
                var extention = Path.GetExtension(dto.Photo.FileName);
                var newImageName = Guid.NewGuid() + extention;
                var location = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/ProfileImg/", newImageName);
                var stream = new FileStream(location, FileMode.Create);
                dto.Photo.CopyTo(stream);
                userImplict.Photo = newImageName;
            }
            return userImplict; 
        }
    }
}
