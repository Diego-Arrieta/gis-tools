using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GisTools.Core.Entities;
using GisTools.Core.Managers;
using GisTools.Core.Geometry;
using OSGeo.OGR;

namespace GisTools.Core.Writers
{
    public static class ShapefileWriter
    {
        public static string WritePoints(string outputPath, List<GisFeature> features)
        {
            try
            {
                GisEngine.Initialize();

                Driver driver = Ogr.GetDriverByName("ESRI Shapefile");
                if (driver == null) return "Error: Driver not found.";

                if (File.Exists(outputPath))
                {
                    try { driver.DeleteDataSource(outputPath); } catch { }
                }

                using (DataSource ds = driver.CreateDataSource(outputPath, null))
                {
                    if (ds == null) return "Error: Couldn't create a file.";

                    using (Layer layer = ds.CreateLayer("Points", null, wkbGeometryType.wkbPoint, null))
                    {
                        if (features.Count > 0)
                        {
                            foreach (var key in features[0].Attributes.Keys)
                            {
                                string fieldName = key.Length > 10 ? key.Substring(0, 10) : key;
                                using (FieldDefn field = new FieldDefn(fieldName, FieldType.OFTString))
                                {
                                    field.SetWidth(100);
                                    layer.CreateField(field, 1);
                                }
                            }
                        }

                        FeatureDefn layerDefn = layer.GetLayerDefn();
                        foreach (var item in features)
                        {
                            using (Feature feat = new Feature(layerDefn))
                            {
                                // Geometry
                                using (OSGeo.OGR.Geometry geom = new OSGeo.OGR.Geometry(wkbGeometryType.wkbPoint))
                                {
                                    geom.AddPoint(item.Geometry.X, item.Geometry.Y, item.Geometry.Z);
                                    feat.SetGeometry(geom);
                                }

                                // Attibutes
                                foreach (var attr in item.Attributes)
                                {
                                    string fieldName = attr.Key.Length > 10 ? attr.Key.Substring(0, 10) : attr.Key;
                                    int idx = layerDefn.GetFieldIndex(fieldName);
                                    if (idx != -1) feat.SetField(idx, attr.Value?.ToString());
                                }
                                layer.CreateFeature(feat);
                            }
                        }
                    }
                }
                return "Success";
            }
            catch (Exception ex)
            {
                return $"Error: {ex.Message}";
            }
        }
    }
}
