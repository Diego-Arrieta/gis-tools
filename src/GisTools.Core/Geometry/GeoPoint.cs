using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GisTools.Core.Geometry
{
    public struct GeoPoint
    {
        public double X { get; }
        public double Y { get; }
        public double Z { get; }

        public GeoPoint(double x, double y, double z = 0)
        {
            X = x;
            Y = y;
            Z = z;
        }
    }
}
