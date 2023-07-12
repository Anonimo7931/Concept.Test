using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Domain.Models.FCSearch
{
    public class FaceMatchInfo
    {
        public int FDID { get; set; }

        public string FDname { get; set; }

        [XmlElement("thresholdValue")]
        public int ThresholdValue { get; set; }

        [XmlElement("bornTime")]
        public DateTime BornTime { get; set; }

        [XmlElement("name")]
        public string Name { get; set; }

        [XmlElement("sex")]
        public string Sex { get; set; }

        [XmlElement("certificateType")]
        public string CertificateType { get; set; }

        [XmlElement("certificateNumber")]
        public int CertificateNumber { get; set; }

        [XmlElement("picURL")]
        public string PicURL { get; set; }

        public int PID { get; set; }

        public List<PersonInfoExtend> PersonInfoExtendList { get; set;}  
    }
}