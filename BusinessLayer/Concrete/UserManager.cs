using BusinessLayer.Abstract;
using DataAccessLayer.Abstract;
using EntityLayer.Concrete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Concrete
{
    public class UserManager : IUserService
    {
        private readonly IUserDal _userDal;

        public UserManager(IUserDal userDal)
        {
            _userDal = userDal;
        }

        public void Delete(User t)
        {
            _userDal.Delete(t);
        }

        public User GetElementById(int id)
        {
            return _userDal.GetElementById(id);
        }

        public List<User> GetListAll()
        {
            return _userDal.GetListAll();
        }

		public User GetUserByShareCode(string shareCode)
		{
            return _userDal.GetUserByShareCode(shareCode);
		}

		public void Insert(User t)
        {
            _userDal.Insert(t);
        }

        public void Update(User t)
        {
            _userDal.Update(t);
        }
        
    }
}
