using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MaxRev.Gdal.Core;
using OSGeo.OGR;

namespace GisTools.Core.Managers
{
    public static class GisEngine
    {
        private static bool _isConfigured = false;

        public static void Initialize()
        {
            if (_isConfigured) return;

            GdalBase.ConfigureAll();
            Ogr.RegisterAll();

            _isConfigured = true;
        }
    }
}
