using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Council.Core.Entities;
using Council.Core.Interfaces;

namespace Council.Service.DBServices
{
    public class CommissionServices : CRUDServices<Commission>, ICommission
    {
        UserServices userService = new UserServices();
        public void Create(Commission commission, string chairManID)
        {
            var user = userService.Get<string>(chairManID);
            commission.CommissionChairman = user;
            this.Create(commission);
        }

        public void Edit(Commission commission, string chairManID)
        {
            var _commission = this.Get<string>(commission.ID);
            var chairman = userService.Get<string>(chairManID);

            _commission.Name = commission.Name;
            _commission.Tittle = commission.Tittle;
            _commission.CommissionChairman = chairman;
            this.Save();
        }

        public List<User> GetMembers(string commisionId)
        {
            var result = new List<User>();
            var commission = this.Get<string>(commisionId);
            if (commission.Members != null)
                result.AddRange(commission.Members);
            result.Add(commission.CommissionChairman);

            return result.Distinct().ToList();
        }
        public string AddCommissionMember(string memberId, string commissionId)
        {
            var commission = this.Get<string>(commissionId);
            var member = userService.Get<string>(memberId);
            commission.Members.Add(member);
            Save();
            return "Ok";
        }
        public string RemoveCommissionMember(string memberId, string commissionId)
        {
            var commission = this.Get<string>(commissionId);
            var member = userService.Get<string>(memberId);
            commission.Members.Remove(member);
            Save();
            return "Ok";
        }
        public Commission GetCommisionByChairmanId(string bossId)
        {
            return this.All().FirstOrDefault(c => c.CommissionChairman.ID == bossId);
        }
        public string GetCommisionNameByChairmanId(string bossId)
        {
            var commision = GetCommisionByChairmanId(bossId);
            return commision == null ? null : commision.Name;
        }

        public string GetCommisionIDByChairmanId(string bossId)
        {
            var commision = GetCommisionByChairmanId(bossId);
            return commision == null ? null : commision.ID;
        }
    }
}
