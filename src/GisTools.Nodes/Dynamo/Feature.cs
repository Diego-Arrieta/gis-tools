using System;
using System.Collections.Generic;
using Autodesk.DesignScript.Runtime;
using Dynamo;
using CoreEntities = GisTools.Core.Entities;
using CoreGeometry = GisTools.Core.Geometry;
using DynamoGeometry = Autodesk.DesignScript.Geometry.Geometry;

namespace Dynamo
{
    public class Feature
    {
        internal CoreEntities.Feature InternalFeature { get; }

        internal Feature(CoreEntities.Feature internalFeature)
        {
            InternalFeature = internalFeature;
        }

        #region Create Nodes

        /// <summary>
        /// Creates a new GisFeature from Geometry and Data.
        /// </summary>
        /// <param name="geometry">Dynamo Point, Curve, or Polygon.</param>
        /// <param name="keys">List of field names (e.g. "ID", "Type").</param>
        /// <param name="values">List of values for those fields.</param>
        public static Feature Create(DynamoGeometry geometry, List<string> keys, List<object> values)
        {
            if (geometry == null) throw new ArgumentNullException(nameof(geometry));

            var coreGeometry = GeometryConverter.ToInternal(geometry);

            if (coreGeometry == null)
                throw new ArgumentException("Geometry conversion failed or type is not supported.");

            var attributes = new Dictionary<string, object>();

            if (keys != null && values != null)
            {
                if (keys.Count != values.Count)
                    throw new ArgumentException("Keys and Values lists must have the same count.");

                for (int i = 0; i < keys.Count; i++)
                {
                    attributes[keys[i]] = values[i];
                }
            }

            return new Feature(new CoreEntities.Feature(coreGeometry, attributes));
        }
        
        #endregion

        #region Query Nodes

        /// <summary>
        /// Gets the geometry type (Point, LineString, Polygon).
        /// </summary>
        public string Type => InternalFeature.Geometry?.GeometryType ?? "Unknown";

        /// <summary>
        /// Gets all attributes/data as a Dictionary.
        /// </summary>
        public Dictionary<string, object> Data => new Dictionary<string, object>(InternalFeature.Attributes);

        #endregion

        #region Action Nodes

        /// <summary>
        /// Extract the Dynamo Geometry from this Feature.
        /// Note: This is a method because geometry conversion is computationally expensive.
        /// </summary>
        public DynamoGeometry Geometry()
        {
            return GeometryConverter.ToDynamo(InternalFeature.Geometry);
        }

        /// <summary>
        /// Gets a specific value by field name.
        /// </summary>
        public object GetValue(string key)
        {
            if (InternalFeature.Attributes.TryGetValue(key, out object val))
            {
                return val;
            }
            return null;
        }

        /// <summary>
        /// Sets or updates a value for a specific field.
        /// Returns the same Feature to allow node chaining.
        /// </summary>
        public Feature SetValue(string key, object value)
        {
            if (string.IsNullOrEmpty(key)) return this;
            InternalFeature.Attributes[key] = value;
            return this;
        }

        #endregion
        public override string ToString()
        {
            return $"Feature({Type}, Attrs: {InternalFeature.Attributes.Count})";
        }
    }
}