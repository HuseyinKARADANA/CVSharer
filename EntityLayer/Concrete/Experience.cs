using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityLayer.Concrete
{
    public class Experience
    {
        public int ExperienceId { get; set; }
        public int UserId { get; set; }//User FK Chanced
        public User? User { get; set; }
        public string ExName { get; set; }
        [Column(TypeName = "Date")]
        public DateTime StartDate { get; set; }
        [Column(TypeName = "Date")]
        public DateTime? EndDate { get; set; }
    }
}
