using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GisTools.Core.Entities;
using GisTools.Core.Geometry;
using GisTools.Core.Helpers;
using GisTools.Core.Managers;
using OSGeo.OGR;

namespace GisTools.Core.Writers
{
    public static class ShapefileWriter
    {
        public static string WritePoints(string path, List<GisFeature> features)
            => WriteFeaturesCore(path, features, wkbGeometryType.wkbPoint);

        public static string WriteLines(string path, List<GisFeature> features)
            => WriteFeaturesCore(path, features, wkbGeometryType.wkbLineString);

        public static string WritePolygons(string path, List<GisFeature> features)
            => WriteFeaturesCore(path, features, wkbGeometryType.wkbPolygon);

        public static string WriteFeaturesCore(string outputPath, List<GisFeature> features, wkbGeometryType gdalType)
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
                    if (ds == null) return "Error: Create file failed.";

                    using (Layer layer = ds.CreateLayer("Layer1", null, gdalType, null))
                    {
                        if (features.Count > 0)
                        {
                            foreach (var key in features[0].Attributes.Keys)
                            {
                                string fieldName = key.Length > 10 ? key.Substring(0, 10) : key;
                                using (FieldDefn field = new FieldDefn(fieldName, FieldType.OFTString))
                                {
                                    field.SetWidth(254);
                                    layer.CreateField(field, 1);
                                }
                            }
                        }

                        FeatureDefn layerDefinition = layer.GetLayerDefn();

                        foreach (var item in features)
                        {
                            using (Feature feat = new Feature(layerDefinition))
                            {
                                OSGeo.OGR.Geometry gdalGeom = GdalGeometryConverter.ToGdalGeometry(item.Geometry);

                                if (gdalGeom != null)
                                {
                                    feat.SetGeometry(gdalGeom);
                                    gdalGeom.Dispose();
                                }

                                foreach (var attr in item.Attributes)
                                {
                                    string fieldName = attr.Key.Length > 10 ? attr.Key.Substring(0, 10) : attr.Key;
                                    int idx = layerDefinition.GetFieldIndex(fieldName);
                                    if (idx != -1 && attr.Value != null)
                                        feat.SetField(idx, attr.Value.ToString());
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
