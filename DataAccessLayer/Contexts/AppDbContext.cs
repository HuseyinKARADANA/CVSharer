using EntityLayer.Concrete;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Contexts
{
    public class AppDbContext: DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }

        DbSet<User> Users { get; set; }
        DbSet<Certificate> Certificates { get; set; }
        DbSet<Experience> Experiences { get; set; }
        DbSet<Hobby> Hobbies { get; set; }
        DbSet<Language> Languages { get; set; }
        DbSet<Link> Links { get; set; }
        DbSet<Skill> Skills { get; set; }

    }
}
