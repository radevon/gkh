using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MonoIndication
{
    public class UserInfoMembership
    {
        public int UserId { get; set; }
        public String UserName { get; set; }
        public String Description { get; set; }
        public DateTime? CreateDate { get; set; }
        public DateTime? LastPasswordFailureDate { get; set; }

        public string[] Roles { get; set; }
        
    }
}