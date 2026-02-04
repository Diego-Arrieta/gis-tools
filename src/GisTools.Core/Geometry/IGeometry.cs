using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GisTools.Core.Geometry
{
    public interface IGeometry
    {
        string GeometryType { get; }
    }
}