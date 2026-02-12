using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NetTopologySuite.IO;
using GisTools.Core.Entities;
using GisTools.Core.Geometry;
using GisTools.Core.Converters;
using GisTools.Core.Managers;

namespace GisTools.Core.Readers
{
    public static class ShapefileReader
    {
        public static List<Feature> Read(string shpPath)
        {
            var features = new List<Feature>();

            ValidateFileExists(shpPath);

            using (var reader = new ShapefileDataReader(shpPath, GisEngine.Factory))
            {
                var header = reader.DbaseHeader;

                while (reader.Read())
                {
                    var ntsGeometry = reader.Geometry;
                    if (ntsGeometry == null) continue;

                    var attributes = new Dictionary<string, object>();

                    for (int i = 0; i < header.NumFields; i++)
                    {
                        string fieldName = header.Fields[i].Name;
                        object value = reader.GetValue(i+1); // WTF
                        attributes[fieldName] = value;
                    }

                    for (int i = 0; i < ntsGeometry.NumGeometries; i++)
                    {
                        var subGeometry = ntsGeometry.GetGeometryN(i);
                        var internalGeometry = NtsGeometryConverter.ToInternalGeometry(subGeometry);

                        if (internalGeometry != null)
                        {
                            var attrsCopy = new Dictionary<string, object>(attributes);
                            features.Add(new Feature(internalGeometry, attrsCopy));
                        }
                    }
                }
            }

            return features;
        }
        private static void ValidateFileExists(string path)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentNullException(nameof(path));

            if (!File.Exists(path))
                throw new FileNotFoundException($"Shapefile not found at: {path}");
        }
    }
}