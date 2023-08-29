using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityLayer.DTOs
{
    public class VerifyDTO
    {
        public string verifyCode { get; set; }
        public string userCode { get; set; }
        public RegisterDTO registerDTO { get; set; }
    }
}
