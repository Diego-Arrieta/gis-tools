using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GisTools.Core.Geometry;

namespace GisTools.Core.Entities
{
    public class Feature
    {
        public IGeometry Geometry { get; set; }
        public Dictionary<string, object> Attributes { get; set; }

        public Feature(IGeometry geometry, Dictionary<string, object> attributes = null)
        {
            Geometry = geometry ?? throw new ArgumentNullException(nameof(geometry));
            Attributes = attributes ?? new Dictionary<string, object>();
        }
    }
}