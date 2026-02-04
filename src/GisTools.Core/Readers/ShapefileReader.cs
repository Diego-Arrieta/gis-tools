using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GisTools.Core.Entities;
using GisTools.Core.Helpers;
using GisTools.Core.Managers;
using OSGeo.OGR;

namespace GisTools.Core.Readers
{
    public static class ShapefileReader
    {
        public static List<GisFeature> ReadAll(string path)
        {
            var results = new List<GisFeature>();

            try
            {
                GisEngine.Initialize();

                using (DataSource ds = Ogr.Open(path, 0))
                {
                    if (ds == null) return results;

                    using (Layer layer = ds.GetLayerByIndex(0))
                    {
                        if (layer == null) return results;

                        layer.ResetReading();

                        Feature feat;
                        while ((feat = layer.GetNextFeature()) != null)
                        {
                            var gdalGeom = feat.GetGeometryRef();
                            var internalGeom = GdalGeometryConverter.ToInternalGeometry(gdalGeom);

                            if (internalGeom != null)
                            {
                                var newFeature = new GisFeature(internalGeom);

                                int fieldCount = feat.GetFieldCount();
                                for (int i = 0; i < fieldCount; i++)
                                {
                                    string key = feat.GetFieldDefnRef(i).GetName();
                                    string value = feat.GetFieldAsString(i);
                                    newFeature.Attributes.Add(key, value);
                                }

                                results.Add(newFeature);
                            }
                            feat.Dispose();
                        }
                    }
                }
            }
            catch (Exception)
            {
                // Handle logging if needed
            }

            return results;
        }
    }
}
