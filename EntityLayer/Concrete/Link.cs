using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityLayer.Concrete
{
    public class Link
    {
        public int LinkId { get; set; }
        public int UserId { get; set; }
        public User? User{ get; set; }
        public string LName { get; set; }
        public string LUrl { get; set; }
    }
}
