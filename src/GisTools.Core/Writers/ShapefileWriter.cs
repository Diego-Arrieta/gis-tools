using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using GisTools.Core.Converters;
using GisTools.Core.Entities;
using GisTools.Core.Managers;
using NetTopologySuite.Features;
using NetTopologySuite.IO;

namespace GisTools.Core.IO
{
    public static class ShapefileWriter
    {
        public static void Write(string filePath, List<Entities.Feature> features)
        {
            if (features == null || !features.Any()) return;

            EnsureDirectoryExists(filePath);

            var ntsFeatures = ConvertToNtsFeatures(features);

            if (!ntsFeatures.Any()) return;

            ValidateGeometryConsistency(ntsFeatures);

            WriteFile(filePath, ntsFeatures);
        }

        private static void EnsureDirectoryExists(string filePath)
        {
            var directory = Path.GetDirectoryName(filePath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
        }

        private static List<NetTopologySuite.Features.Feature> ConvertToNtsFeatures(List<Entities.Feature> gisFeatures)
        {
            var result = new List<NetTopologySuite.Features.Feature>();

            foreach (var item in gisFeatures)
            {
                var geometry = NtsGeometryConverter.ToNtsGeometry(item.Geometry);
                if (geometry == null) continue;

                var attributes = CreateAttributesTable(item.Attributes);
                result.Add(new NetTopologySuite.Features.Feature(geometry, attributes));
            }

            return result;
        }

        private static AttributesTable CreateAttributesTable(Dictionary<string, object> sourceData)
        {
            var table = new AttributesTable();

            foreach (var pair in sourceData)
            {
                table.Add(pair.Key, pair.Value);
            }

            if (table.Count == 0)
            {
                table.Add("ID", 0);
            }

            return table;
        }

        private static void ValidateGeometryConsistency(List<NetTopologySuite.Features.Feature> features)
        {
            string expectedType = features.First().Geometry.GeometryType;

            if (features.Any(f => f.Geometry.GeometryType != expectedType))
            {
                throw new InvalidOperationException($"Shapefiles do not support mixed geometry types. Expected: {expectedType}");
            }
        }

        private static void WriteFile(string filePath, List<NetTopologySuite.Features.Feature> features)
        {
            var writer = new ShapefileDataWriter(filePath, GisEngine.Factory)
            {
                Header = ShapefileDataWriter.GetHeader(features.First(), features.Count)
            };

            writer.Write(features);
        }
    }
}