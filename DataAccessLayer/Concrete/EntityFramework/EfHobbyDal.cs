﻿using DataAccessLayer.Abstract;
using DataAccessLayer.Concrete.Repository;
using DataAccessLayer.Contexts;
using EntityLayer.Concrete;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Concrete.EntityFramework
{
    public class EfHobbyDal : GenericRepository<Hobby>, IHobbyDal
    {
        public EfHobbyDal(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
    }
}
