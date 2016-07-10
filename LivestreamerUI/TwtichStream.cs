using System;
using System.Collections.Generic;

namespace LivestreamerUI
{
    class TwitchStream
    {
        public Stream stream { get; set; }

        public class Stream
        {
            public string game { get; set; }
            public string _id { get; set; }
        }
    }
}
 