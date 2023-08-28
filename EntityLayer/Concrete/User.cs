using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityLayer.Concrete
{
    public class User
    {
        public int UserId { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public byte[]? Photo { get; set; }
        public string? Address  { get; set; }
        public string? Position { get; set; }
        public string? Description { get; set; }
        public string? Phone { get; set; }
        public string? Linkedin { get; set; }
        public string? Instagram { get; set; }
        public string? GitHub { get; set; }
        public string? YouTube { get; set; }
        public bool IsActive { get; set; }

        public List<Skill> Skills { get; set; }
        public List<Education> Educations { get; set; }
        public List<Experience> Experiences { get; set; }
        public List<Hobby> Hobbies { get; set; }
        public List<Certificate> Certificates { get; set; }
        public List<Link> Links { get; set; }
        public List<Language> Languages { get;}

    }
}
