using System;
using System.Xml.Serialization;

namespace Domain.Models.FCSearch
{
    public class PersonInfoExtend
    {

        [XmlElement("id")]
        public int Id { get; set; }

        [XmlElement("enable")]
        public Boolean Enable { get; set; }

        [XmlElement("name")]
        public string Name { get; set; }

        [XmlElement("value")]
        public string Value { get; set; }
    }
}