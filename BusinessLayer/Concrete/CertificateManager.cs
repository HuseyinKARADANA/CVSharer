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
    public class CertificateManager : ICertificateService
    {
        private readonly ICertificateDal _certificateDal;


        public CertificateManager(ICertificateDal certificateDal)
        {
            _certificateDal = certificateDal;
        }

        public void Delete(Certificate t)
        {
            _certificateDal.Delete(t);
        }

        public Certificate GetElementById(int id)
        {
            return _certificateDal.GetElementById(id);
        }

        public List<Certificate> GetListAll()
        {
            return _certificateDal.GetListAll();
        }

        public void Insert(Certificate t)
        {
            _certificateDal.Insert(t);
        }

        public void Update(Certificate t)
        {
            _certificateDal.Update(t);
        }

        public List<Certificate> GetCertificatesByUserId(int userId)
        {
            List<Certificate> certificates = _certificateDal.GetCertificatesByUserId(userId);

            return certificates;
        }
    }
}
