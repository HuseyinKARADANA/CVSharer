﻿using EntityLayer.Concrete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Abstract
{
    public interface IEducationDal: IGenericDal<Education>
    {
        List<Education> GetEducationsByUserId(int userId);

    }
}
