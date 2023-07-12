
using System.Xml.Serialization;

namespace Domain.Models.ResponseStatusDelete
{
    [XmlRoot("ResponseStatus", Namespace = "http://www.hikvision.com/ver20/XMLSchema")]
    public class ResponseStatus
    {
        [XmlElement("requestURL")]
        public string RequestURL { get; set; }

        [XmlElement("statusCode")]
        public int StatusCode { get; set; }

        [XmlElement("statusString")]
        public string StatusString { get; set; }

        [XmlElement("subStatusCode")]
        public string SubStatusCode { get; set; }
    }
}
