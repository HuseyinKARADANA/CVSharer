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
    public class LinkManager : ILinkService
    {
        private readonly ILinkDal _linkDal;

        public LinkManager(ILinkDal linkDal)
        {
            _linkDal = linkDal;
        }

        public void Delete(Link t)
        {
            _linkDal.Delete(t); 
        }

        public Link GetElementById(int id)
        {
           return _linkDal.GetElementById(id);
        }

        public List<Link> GetListAll()
        {
            return _linkDal.GetListAll();
        }

        public void Insert(Link t)
        {
            _linkDal.Insert(t);
        }

        public void Update(Link t)
        {
            _linkDal.Update(t);
        }
    }
}
