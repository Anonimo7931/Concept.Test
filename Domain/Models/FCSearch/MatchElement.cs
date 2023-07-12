using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Domain.Models.FCSearch
{
    public class MatchElement
    {
        [XmlElement("snapPicURL")]
        public string SnapPicURL { get; set; }

        [XmlElement("snapTime")]
        public DateTime SnapTime { get; set; }

        [XmlElement("facePicURL")]
        public string FacePicURL { get; set; }

        public List<FaceMatchInfo> FaceMatchInfoList { get; set; }


    }
}