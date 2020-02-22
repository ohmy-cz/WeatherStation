using System;
using System.Collections.Generic;
using System.Text;

namespace net.jancerveny.weatherstation.Common.Models
{
    public class PhilipsHueConfiguration
    {
        public string BridgeIp { get; private set; }
        public string AppKey { get; private set; }
    }
}
