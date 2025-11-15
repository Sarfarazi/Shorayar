using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Council.Core.Entities;
using Council.Core.Interfaces;
using Council.Data.Contexts;

namespace Council.Service.DBServices
{
    public class PeriodService:CRUDServices<CouncilPeriod>,ICouncilPeriod
    {
        MainContext mainDB;
        public PeriodService()
        {
            mainDB = new MainContext();
        }        
        public void DeactiveAllPeriod()
        {
            var items = this.All().ToList();            
            foreach (var item in items)
            {
                if (item.IsActive)
                {
                    item.IsActive = false;
                    this.Update(item);
                }                
            }           
        }
        public CouncilPeriod ActivePeriod(CouncilPeriod period)
        {           
            return Update(period);
        }
    }
}
