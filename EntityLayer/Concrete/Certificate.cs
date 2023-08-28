using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityLayer.Concrete
{
    public class Certificate
    {
        public int CertificateId { get; set; }
        public int UserId { get; set; }//User FK Chanced
        public User? User { get; set; }
        public string CName { get; set; }
        public string Institution { get; set; }
        public string Url { get; set; }
        [Column(TypeName = "Date")]
        public DateTime StartDate { get; set; }
        [Column(TypeName = "Date")]
        public DateTime? EndDate { get; set; }
    }
}
