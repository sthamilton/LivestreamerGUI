using System.Collections.Generic;
using System.Net;

namespace LivestreamerUI
{
    class TwitchFollowing
    { 
        public List<Follows> follows { get; set; }
        public _Links _links { get; set; }

        public class Follows
        {
            public Channel channel { get; set; }

            public class Channel
            {
                public string name { get; set; }
                public string display_name { get; set; }
                public string game { get; set; }
                public string url { get; set; }
                public string _id { get; set; }
            }
        }

        public class _Links
        {
            public string next { get; set; }
        }
        
    }
}
