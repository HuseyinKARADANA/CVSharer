using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityLayer.Concrete
{
    public class Language
    {
        public int LanguageId { get; set; }
        public int UserId { get; set; }//User FK Chanced
        public User? User { get; set; }
        public string LangName { get; set; }
        public int LangPercentage { get; set; }
    }
}
