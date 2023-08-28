using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityLayer.Concrete
{
    public class Skill
    {
        public int SkillId { get; set; }
        public int UserId { get; set; }//User FK Chanced
        public User? User { get; set; }

        public string SName { get; set; }
        public string SPercentage { get; set; }
    }
}
