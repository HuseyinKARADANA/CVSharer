﻿using DataAccessLayer.Abstract;
using DataAccessLayer.Contexts;
using EntityLayer.Concrete;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Concrete.Repository
{
    public class GenericRepository<T> : IGenericDal<T> where T : class, new()
    {
        private readonly DbContextOptions<AppDbContext> options;
        public GenericRepository(DbContextOptions<AppDbContext> options)
        {
            this.options = options;
        }
        public void Delete(T t)
        {

            var context = new AppDbContext(options);
            context.Remove(t);
            context.SaveChanges();
        }

        public T GetElementById(int id)
        {

            using var context = new AppDbContext(options);
            return context.Set<T>().Find(id);
        }

        public List<T> GetListAll()
        {

            using var context = new AppDbContext(options);
            return context.Set<T>().ToList();
        }

        public void Insert(T t)
        {

            using var context = new AppDbContext(options);
            context.Add(t);
            context.SaveChanges();
        }

        public void Update(T t)
        {

            using var context = new AppDbContext(options);
            context.Update(t);
            context.SaveChanges();
        }

        public List<Skill> GetSkillsByUserId(int userId)
        {
            using var context = new AppDbContext(options);
            return context.Set<Skill>().Where(x => x.UserId == userId).ToList();
        }

        public List<Certificate> GetCertificatesByUserId(int userId)
        {
            using var context = new AppDbContext(options);
            return context.Set<Certificate>().Where(x => x.UserId == userId).ToList();
        }

        public List<Education> GetEducationsByUserId(int userId)
        {
            using var context = new AppDbContext(options);
            return context.Set<Education>().Where(x => x.UserId == userId).ToList();
        }

        public List<Experience> GetExperiencesByUserId(int userId)
        {
            using var context = new AppDbContext(options);
            return context.Set<Experience>().Where(x => x.UserId == userId).ToList();
        }
        public List<Hobby> GetHobbiesByUserId(int userId)
        {
            using var context = new AppDbContext(options);
            return context.Set<Hobby>().Where(x => x.UserId == userId).ToList();
        }

        public List<Language> GetLanguagesByUserId(int userId)
        {
            using var context = new AppDbContext(options);
            return context.Set<Language>().Where(x => x.UserId == userId).ToList();
        }

        public List<Link> GetLinksByUserId(int userId)
        {
            using var context = new AppDbContext(options);
            return context.Set<Link>().Where(x => x.UserId == userId).ToList();
        }
        public User GetUserByShareCode(string shareCode)
        {
			using var context = new AppDbContext(options);
            return context.Set<User>().FirstOrDefault(x=>x.ShareCode==shareCode);

		}
    }
}
