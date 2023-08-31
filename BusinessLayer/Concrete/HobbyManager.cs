using BusinessLayer.Abstract;
using DataAccessLayer.Abstract;
using DataAccessLayer.Concrete.EntityFramework;
using EntityLayer.Concrete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Concrete
{
    public class HobbyManager : IHobbyService
    {
        private readonly IHobbyDal _hobbyDal;

        public HobbyManager(IHobbyDal hobbyDal)
        {
            _hobbyDal = hobbyDal;
        }

        public void Delete(Hobby t)
        {
            _hobbyDal.Delete(t);
        }

        public Hobby GetElementById(int id)
        {
            return _hobbyDal.GetElementById(id);
        }

        public List<Hobby> GetListAll()
        {
            return _hobbyDal.GetListAll();
        }

        public void Insert(Hobby t)
        {
            _hobbyDal.Insert(t);
        }

        public void Update(Hobby t)
        {
            _hobbyDal.Update(t);
        }

        public List<Hobby> GetHobbiesByUserId(int userId)
        {
            List<Hobby> hobbies = _hobbyDal.GetHobbiesByUserId(userId);

            return hobbies;
        }
    }
}
