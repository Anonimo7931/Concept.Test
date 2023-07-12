using System;
using System.Collections.Generic;

namespace Domain.Models
{
    public class FCSearchDescription
    {
        public Guid SearchID { get; set; }
        public DateTime SnapStartTime { get; set; }
        public DateTime SnapEndTime { get; set; }
        public int MaxResults { get; set; }
        public int SearchResultPosition { get; set; }
        public List<Channel> ChannelList { get; set; }
    }
}