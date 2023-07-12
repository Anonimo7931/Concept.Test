using System.Collections.Generic;
using System.Xml.Serialization;
using System;

namespace Domain.Models.FDSearch
{
    public class MatchElement
    {
        public int FDID { get; set; }

        [XmlElement("bornTime")]
        public DateTime BornTime { get; set; }

        [XmlElement("name")]
        public string Name { get; set; }

        [XmlElement("sex")]
        public string Sex { get; set; }

        [XmlElement("certificateType")]
        public string CertificateType { get; set; }

        [XmlElement("certificateNumber")]
        public string CertificateNumber { get; set; }


        [XmlElement("picURL")]
        public string PicURL { get; set; }

        public string PID { get; set; }


        public List<PersonInfoExtend> PersonInfoExtendList { get; set; }
    }
}
