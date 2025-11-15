using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Council.Core.Entities
{
    public class Role : BaseEntity
    {
        public Role()
        {
            JustOne = false;
            //RoleID = Guid.NewGuid().ToString().Replace("-", "");
        }
        //public string RoleID { get; set; }

        [Required]
        public string RoleTitle { get; set; }

        [Required]
        public string RoleDesc { get; set; }

        public bool JustOne { get; set; }

        public int Order { get; set; }

        public virtual ICollection<UserRole> UserRole { get; set; }
    }
}
