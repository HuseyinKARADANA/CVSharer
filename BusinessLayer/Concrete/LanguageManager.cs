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
    public class LanguageManager : ILanguageService
    {
        private readonly ILanguageDal _languageDal;

        public LanguageManager(ILanguageDal languageDal)
        {
            _languageDal = languageDal;
        }

        public void Delete(Language t)
        {
            _languageDal.Delete(t);
        }

        public Language GetElementById(int id)
        {
          return _languageDal.GetElementById(id);
        }

        public List<Language> GetListAll()
        {
            return _languageDal.GetListAll();
        }

        public void Insert(Language t)
        {
            _languageDal.Insert(t);
        }

        public void Update(Language t)
        {
            _languageDal.Update(t);
        }

        public List<Language> GetLanguagesByUserId(int userId)
        {
            List<Language> languages = _languageDal.GetLanguagesByUserId(userId);

            return languages;
        }
    }
}
