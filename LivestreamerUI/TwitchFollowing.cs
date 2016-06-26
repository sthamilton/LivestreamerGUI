using System.Collections.Generic;
using System.Net;

namespace LivestreamerUI
{
    class TwitchFollowing
    { 
        public List<Follows> follows { get; set; }

        public class Follows
        {
            public Channel channel { get; set; }
            public Links _links { get; set; }

            public class Channel
            {
                public string name { get; set; }
                public string game { get; set; }
            }

            public class Links
            {
                public string self { get; set; }
            }

        }
    }
}
