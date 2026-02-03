using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GisTools.Core.Geometry;

namespace GisTools.Core.Entities
{
    public class GisFeature
    {
        public GeoPoint Geometry { get; set; }

        public Dictionary<string, object> Attributes { get; set; }

        public GisFeature(GeoPoint geometry)
        {
            Geometry = geometry;
            Attributes = new Dictionary<string, object>();
        }
    }
}
