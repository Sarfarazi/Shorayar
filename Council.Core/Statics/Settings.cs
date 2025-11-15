using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Management;

namespace Council.Core.Statics
{
    public static class Settings
    {
        public static string CityName
        {
            get
            {
                return "میبد";
            }
        }
        public static string LatinCityName
        {
            get
            {
                return "Meybod";
            }
        }
        public static string NationalBanner
        {
            get
            {
                return "اقتصاد مقاومتی ، تولید و اشتغال";
            }
        }
        public static int AllLetter { get { return 1; } }
        public static int CouncilLetter { get { return 2; } }
        public static int OutLetter { get { return 3; } }
        public static int ArchivedLetter { get { return 4; } }
        public static int RemovedLetter { get { return 5; } }
        public static int NormalLetter { get { return 6; } }

    }

    public static class GetPCInformations{
        public static String GetProcessorId()
        {
            ManagementClass mc = new ManagementClass("win32_processor");
            ManagementObjectCollection moc = mc.GetInstances();
            String Id = String.Empty;
            foreach (ManagementObject mo in moc)
            {
                Id = mo.Properties["processorID"].Value.ToString();
                break;
            }
            return Id;
        }
    }
}
