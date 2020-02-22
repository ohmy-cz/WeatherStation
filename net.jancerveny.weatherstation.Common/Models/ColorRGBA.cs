using System;
using System.Collections.Generic;
using System.Text;

namespace net.jancerveny.weatherstation.Common.Models
{
    public class ColorRGBA
    {
        public byte R { get;  set; }
        public byte G { get; private set; }
        public byte B { get; private set; }
        public float A { get; private set; }
        public ColorRGBA() 
        {
        }

        public ColorRGBA(byte r, byte g, byte b, float a)
        {
            R = r;
            G = g;
            B = b;
            A = a;
        }

        public override string ToString()
        {
            return $"rgba({R},{G},{B},{A})";
        }
    }
}
