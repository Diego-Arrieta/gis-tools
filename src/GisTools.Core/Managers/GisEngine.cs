using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NetTopologySuite;
using NetTopologySuite.Geometries;

namespace GisTools.Core.Managers
{
    // TODO Evaluate if delete or not this class and use directly the NtsGeometryServices.Instance.CreateGeometryFactory(srid: 0) where needed
    public static class GisEngine
    {
        public static readonly GeometryFactory Factory = NtsGeometryServices.Instance.CreateGeometryFactory(srid: 0);
    }
}
