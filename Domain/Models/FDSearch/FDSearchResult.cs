using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Domain.Models.FDSearch
{
    [XmlRoot("FDSearchResult", Namespace = "http://www.hikvision.com/ver20/XMLSchema")]
    public class FDSearchResult
    {
        [XmlElement("searchID")]
        public Guid SearchID { get; set; }

        [XmlElement("responseStatus")]
        public Boolean ResponseStatus { get; set; }

        [XmlElement("responseStatusStrg")]
        public string ResponseStatusStrg { get; set; }

        [XmlElement("numOfMatches")]
        public int NumOfMatches { get; set; }

        [XmlElement("totalMatches")]
        public int TotalMatches { get; set; }

        public List<MatchElement> MatchList { get; set; }
    }
}
