using Council.Core.Entities;
using Council.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Council.Service.DBServices
{
    public class MeetingServices : CRUDServices<Meeting>, IMeeting
    {
        UserServices userService = new UserServices();
        public IQueryable<Meeting> Search(string filter)
        {
            return All().Where(m => m.Peoples.Contains(filter) || m.Subject.Contains(filter) || m.Content.Contains(filter)).OrderByDescending(l => l.CreatedOn);
        }
      

    }
}
