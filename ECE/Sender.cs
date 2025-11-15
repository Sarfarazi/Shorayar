using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace ECE
{
    public class Sender : IOrganization
    {
        [XmlAttribute]
        [Display(Name ="نام سازمان *")]
        [Required(ErrorMessage ="نام سازمان ارسال کننده را وارد کنید")]
        public string Organization { get; set; }

        [XmlAttribute]
        [Display(Name = "نام واحد(های) سازمانی")]
        public string Department { get; set; }

        [XmlAttribute]
        [Display(Name = "پست سازمانی")]
        public string Position { get; set; }

        [XmlAttribute]
        [Display(Name = "نام و نام خانوادگی امضا کننده")]
        public string Name { get; set; }

        [XmlAttribute]
        [Display(Name = "کد فرستنده *")]
        [Required(ErrorMessage = "کد منحصر به فرد فرستنده را وارد کنید")]
        public string Code { get; set; }

        [XmlAttribute]
        [Display(Name = "توضیحات")]
        public string Any { get; set; }
        public Sender()
        {

        }
        public Sender(string organization, string code)
        {
            Organization = organization;
            Code = code;
        }
        public Sender(string organization,string department,string position, string name, string code)
        {
            Organization = organization;
            Department = department;
            Position = position;
            Name = name;
            Code = code;
        }
    }
}
