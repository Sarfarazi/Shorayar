using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace ECE
{
    public class Receiver : IOrganization
    {
        [XmlAttribute]
        [Display(Name ="نام سازمان *")]
        [Required(ErrorMessage ="نام سازمان گیرنده را وارد کنید")]
        public string Organization { get; set; }

        [XmlAttribute]
        [Display(Name = "نام واحد(های) سازمانی")]
        public string Department { get; set; }

        [XmlAttribute]
        [Display(Name = "پست سازمانی")]
        public string Position { get; set; }

        [XmlAttribute]
        [Display(Name = "نام و نام خانوادگی")]
        public string Name { get; set; }

        [XmlAttribute]
        [Display(Name = "کد گیرنده *")]
        [Required(ErrorMessage = "کد گیرنده را وارد کنید")]
        public string Code { get; set; }

        [XmlAttribute]
        [Display(Name = "نوع دریافت نامه *")]
        [Required(ErrorMessage = "نوع دریافت نامه: Origin")]
        public string ReceiveType { get; set; }

        [XmlAttribute]
        [Display(Name = "توضیحات")]
        public string Any { get; set; }

        [XmlText]
        [Display(Name = "مقدار")]
        public string Value { get { return string.Empty; } }

        public Receiver()
        {

        }
        public Receiver(string organization, string code)
        {
            Organization = organization;
            Code = code;
        }
        public Receiver(string organization, string department, string position, string name, string code)
        {
            Organization = organization;
            Department = department;
            Position = position;
            Name = name;
            Code = code;
        }
    }
}
