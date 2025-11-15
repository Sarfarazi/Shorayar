using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Council.Core.Models
{
    class UserViewModels
    {
    }
   
    public class SmallUserInfo
    {
        public string ID { get; set; }
        public string ApplicationUserID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }
        public string GroupName { get; set; }
        public bool IsActive { get; set; }
        public string OrganPostName { get; set; }
        public string Picture { get; set; }
    }
    public class UserAndUserName
    {
        public string ID { get; set; }

        [Display(Name = "نام")]
        public string FirstName { get; set; }

        [Display(Name = "نام خانوادگی")]
        public string LastName { get; set; }
        public string ApplicationUserID { get; set; }
        public bool IsCouncilBoss { get; set; }
        public bool IsCouncilGuest { get; set; }       
        public bool IsWriter1 { get; set; }
        public bool IsWriter2 { get; set; }
        public bool IsOnline { get; set; }
        public bool IsOtherCouncilMember { get; set; }
        public bool BossHelper { get; set; }

        [Display(Name = "نوع کاربری")]
        public bool IsCouncilMember { get; set; }

        [Display(Name = "فعال؟")]
        public bool IsActive { get; set; }

        [Display(Name = "نام کاربری")]
        public string UserName { get; set; }

        [Display(Name = "تلفن")]
        public string Tell { get; set; }
    }

}
