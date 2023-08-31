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
    public class SkillManager : ISkillService
    {
        private readonly ISkillDal _skillDal;

        public SkillManager(ISkillDal skillDal)
        {
            _skillDal = skillDal;
        }

        public void Delete(Skill t)
        {
            _skillDal.Delete(t);
        }

        public Skill GetElementById(int id)
        {
            return _skillDal.GetElementById(id);
        }

        public List<Skill> GetListAll()
        {
            return _skillDal.GetListAll();
        }

        public void Insert(Skill t)
        {
           _skillDal.Insert(t);
        }

        public void Update(Skill t)
        {
            _skillDal.Update(t);
        }

        public List<Skill> GetSkillsByUserId(int userId)
        {
            List<Skill> skills = _skillDal.GetSkillsByUserId(userId);

            return skills;
        }
    }
}
