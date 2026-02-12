using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.DesignScript.Runtime;
using GisTools.Core.IO;
using GisTools.Core.Readers;
using CoreEntities = GisTools.Core.Entities;
using CoreGeometry = GisTools.Core.Geometry;
using DynamoGeometry = Autodesk.DesignScript.Geometry.Geometry;

namespace Dynamo
{
    public class Shapefile
    {
        private readonly List<Feature> _features;

        private Shapefile(List<Feature> features)
        {
            _features = features ?? new List<Feature>();
        }

        #region Create Nodes

        /// <summary>
        /// Reads a .shp file and loads its data into Dynamo.
        /// </summary>
        /// <param name="filePath">Path to the .shp file.</param>
        public static Shapefile LoadFromFile(string filePath)
        {
            var coreFeatures = ShapefileReader.Read(filePath);

            var dynamoFeatures = coreFeatures.Select(f => new Feature(f)).ToList();

            return new Shapefile(dynamoFeatures);
        }


        /// <summary>
        /// Writes a list of Features to a Shapefile.
        /// </summary>
        /// <param name="filePath">Output path for the .shp file.</param>
        /// <param name="features">List of Features to write.</param>
        public static string WriteFeatures(string filePath, List<Feature> features)
        {
            try
            {
                if (features == null || !features.Any()) return "Error: Feature list is empty.";

                var coreFeatures = features.Select(f => f.InternalFeature).ToList();

                ShapefileWriter.Write(filePath, coreFeatures);

                return "Success";
            }
            catch (Exception ex)
            {
                return $"Error: {ex.Message}";
            }
        }

        /// <summary>
        /// Creates a Shapefile directly from Geometry and Data lists (without creating Features manually).
        /// </summary>
        public static string WriteByData(string filePath,
            List<Autodesk.DesignScript.Geometry.Geometry> geometries,
            List<string> fieldNames,
            List<List<object>> values) // List of Lists for values (rows)
        {
            try
            {
                if (geometries.Count != values.Count)
                    return "Error: Geometry count does not match Values count.";

                var featuresToWrite = new List<CoreEntities.Feature>();

                for (int i = 0; i < geometries.Count; i++)
                {
                    // Create Feature on the fly
                    var tempFeature = Feature.Create(geometries[i], fieldNames, values[i]);
                    featuresToWrite.Add(tempFeature.InternalFeature);
                }

                ShapefileWriter.Write(filePath, featuresToWrite);
                return "Success";
            }
            catch (Exception ex)
            {
                return $"Error: {ex.Message}";
            }
        }
        
        #endregion
        
        #region Query Nodes

        /// <summary>
        /// Gets the total number of records/features.
        /// </summary>
        public int RecordCount => _features.Count;

        /// <summary>
        /// Gets the predominant geometry type (Point, Line, Polygon).
        /// </summary>
        public string GeometryType
        {
            get
            {
                if (_features.Count == 0) return "Empty";
                return _features[0].Type;
            }
        }

        #endregion

        #region Action Nodes

        

        /// <summary>
        /// Extracts all geometries from the shapefile as a list.
        /// </summary>
        public List<Autodesk.DesignScript.Geometry.Geometry> GetAllGeometries()
        {
            return _features.Select(f => f.Geometry()).ToList();
        }

        /// <summary>
        /// Gets the list of Features contained in this file.
        /// </summary>
        public List<Feature> GetFeatures()
        {
            return _features;
        }


        /// <summary>
        /// Gets the list of available field names (headers).
        /// </summary>
        public List<string> GetFieldNames()
        {
            if (_features.Count == 0) return new List<string>();
            return _features[0].Data.Keys.ToList();
        }

        /// <summary>
        /// Extracts all values for a specific field/column.
        /// Useful for data analysis in Dynamo.
        /// </summary>
        public List<object> GetValuesAtField(string fieldName)
        {
            var results = new List<object>();
            foreach (var f in _features)
            {
                results.Add(f.GetValue(fieldName));
            }
            return results;
        }
        
        #endregion

        public override string ToString()
        {
            return $"Shapefile(Count: {RecordCount}, Type: {GeometryType})";
        }

    }
}