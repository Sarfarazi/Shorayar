using Council.Core.Entities;
using Council.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Council.Service.DBServices
{
    public class SystemSettingsService : CRUDServices<SystemSettings>
    {
        public void DeactiveAllSettings()
        {
            var items = this.All().ToList();
            foreach (var item in items)
            {
                if (item.Used)
                {
                    item.Used = false;
                    this.Update(item);
                }
            }
        }


        public bool ISCurrentSetting(string ID)
        {
            return this.All().Any(m => m.ID == ID && m.Used);
        }
    }
}
