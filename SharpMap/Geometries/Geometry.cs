using System;
using System.Data.SqlTypes;
using C4I.Applications.SCE.Interfaces.CoordinateSystems;
using Microsoft.SqlServer.Types;

namespace C4I.Applications.SCE.Interfaces
{
    /// <summary>
    /// <see cref="Geometry"/> is the root class of the Geometry Object Model hierarchy.
    /// <see cref="Geometry"/> is an abstract (non-instantiable) class.
    /// </summary>
    /// <remarks>
    /// <para>The instantiable subclasses of <see cref="Geometry"/> defined in the specification are restricted to 0, 1 and twodimensional
    /// geometric objects that exist in two-dimensional coordinate space (R^2).</para>
    /// <para>All instantiable geometry classes described in this specification are defined so that valid instances of a
    /// geometry class are topologically closed (i.e. all defined geometries include their boundary).</para>
    /// </remarks>
    [Serializable]
    public abstract class Geometry : IGeometry, IEquatable<Geometry>
    {
        private ICoordinateSystem _SpatialReference = ProjectedCoordinateSystem.WGS84_UTM(40, true);
        private SqlGeometry _sqlGeometry;

        #region IGeometry Members

        /// <summary>
        /// Gets or sets the spatial reference system associated with the <see cref="Geometry"/>.
        /// A <see cref="Geometry"/> may not have had a spatial reference system defined for
        /// it, in which case *spatialRef will be NULL.
        /// </summary>

        public ICoordinateSystem SpatialReference
        {
            get { return _SpatialReference; }
            set { _SpatialReference = value; }
        }

        // The following are methods that should be implemented on a geometry object according to
        // the OpenGIS Simple Features Specification

        /// <summary>
        /// Returns 'true' if this Geometry is 'spatially equal' to anotherGeometry
        /// </summary>
        public virtual bool Equals(Geometry other)
        {
            return SpatialRelations.Equals(this, other);
        }

        #endregion

        #region "Basic Methods on Geometry"

        /// <summary>
        ///  The inherent dimension of this <see cref="Geometry"/> object, which must be less than or equal
        ///  to the coordinate dimension.
        /// </summary>
        /// <remarks>This specification is restricted to geometries in two-dimensional coordinate space.</remarks>
        public abstract int Dimension
        {
            get;
        }

        /// <summary>
        /// User Data Associated with the geometry object
        /// </summary>
        public object UserData { get; set; }


        /// <summary>
        /// The minimum bounding box for this <see cref="Geometry"/>, returned as a <see cref="Geometry"/>. The
        /// polygon is defined by the corner points of the bounding box ((MINX, MINY), (MAXX, MINY), (MAXX,
        /// MAXY), (MINX, MAXY), (MINX, MINY)).
        /// </summary>
        /// <remarks>The envelope is actually the <see cref="BoundingBox"/> converted into a polygon.</remarks>
        /// <seealso cref="GetBoundingBox"/>
        public Geometry Envelope()
        {
            BoundingBox box = GetBoundingBox();
            Polygon envelope = new Polygon();
            envelope.ExteriorRing.Vertices.Add(box.Min); //minx miny
            envelope.ExteriorRing.Vertices.Add(new Point(box.Max.X, box.Min.Y, 0)); //maxx minu
            envelope.ExteriorRing.Vertices.Add(box.Max); //maxx maxy
            envelope.ExteriorRing.Vertices.Add(new Point(box.Min.X, box.Max.Y, 0)); //minx maxy
            envelope.ExteriorRing.Vertices.Add(envelope.ExteriorRing.StartPoint); //close ring
            return envelope;
        }

        public void GeocalculationInit()
        {
            UpdateSqlGeometry();
        }

        /// <summary>
        /// The minimum bounding box for this <see cref="Geometry"/>, returned as a <see cref="BoundingBox"/>.
        /// </summary>
        /// <returns></returns>
        public abstract BoundingBox GetBoundingBox();

        /// <summary>
        /// Returns 'true' if this <see cref="Geometry"/> is the empty geometry . If true, then this
        /// <see cref="Geometry"/> represents the empty point set, Ø, for the coordinate space. 
        /// </summary>
        public abstract bool IsEmpty();

        /// <summary>
        ///  Returns 'true' if this Geometry has no anomalous geometric points, such as self
        /// intersection or self tangency. The description of each instantiable geometric class will include the specific
        /// conditions that cause an instance of that class to be classified as not simple.
        /// </summary>
        public virtual bool IsSimple()
        {
            UpdateSqlGeometry();
            SqlBoolean sqlboolean = SqlGeometry.STIsSimple();
            bool retval = sqlboolean.Value;
            return retval;
        }

        /// <summary>
        /// Returns the closure of the combinatorial boundary of this <see cref="Geometry"/>. The
        /// combinatorial boundary is defined as described in section 3.12.3.2 of [1]. Because the result of this function
        /// is a closure, and hence topologically closed, the resulting boundary can be represented using
        /// representational geometry primitives
        /// </summary>
        public virtual Geometry Boundary()
        {
            UpdateSqlGeometry();
            SqlGeometry boundary = SqlGeometry.STBoundary();
            Geometry retval = boundary.AsSCEGeometryExtention();
            return retval;
        }

        /// <summary>
        /// Gets the geometry type of this class
        /// </summary>
        public virtual GeometryType2 GeometryType
        {
            get { return GeometryType2.Geometry; }
        }

        #endregion

        #region "Methods for testing Spatial Relations between geometric objects"

        /// <summary>
        /// Returns 'true' if this Geometry is ‘spatially disjoint’ from another <see cref="Geometry"/>.
        /// </summary>
        public virtual bool Disjoint(Geometry geom)
        {
            UpdateSqlGeometry();
            geom.UpdateSqlGeometry();
            SqlBoolean result = SqlGeometry.STDisjoint(geom.SqlGeometry);
            bool retval = result.Value;
            return retval; //return SpatialRelations.Disjoint(this, geom);
        }

        /// <summary>
        /// Returns 'true' if this <see cref="Geometry"/> ‘spatially intersects’ another <see cref="Geometry"/>.
        /// </summary>
        public virtual bool Intersects(Geometry geom)
        {
            UpdateSqlGeometry();
            geom.UpdateSqlGeometry();
            SqlBoolean result = SqlGeometry.STIntersects(geom.SqlGeometry);
            bool retval = result.Value;
            return retval;
        }

        /// <summary>
        /// Returns 'true' if this <see cref="Geometry"/> ‘spatially touches’ another <see cref="Geometry"/>.
        /// </summary>
        public virtual bool Touches(Geometry geom)
        {
            UpdateSqlGeometry();
            geom.UpdateSqlGeometry();
            SqlBoolean result = SqlGeometry.STTouches(geom.SqlGeometry);
            bool retval = result.Value;
            return retval;
        }

        /// <summary>
        /// Returns 'true' if this <see cref="Geometry"/> ‘spatially crosses’ another <see cref="Geometry"/>.
        /// </summary>
        public virtual bool Crosses(Geometry geom)
        {
            UpdateSqlGeometry();
            geom.UpdateSqlGeometry();
            SqlBoolean result = SqlGeometry.STCrosses(geom.SqlGeometry);
            bool retval = result.Value;
            return retval;
        }

        /// <summary>
        /// Returns 'true' if this <see cref="Geometry"/> is ‘spatially within’ another <see cref="Geometry"/>.
        /// </summary>
        public virtual bool Within(Geometry geom)
        {
            UpdateSqlGeometry();
            geom.UpdateSqlGeometry();
            SqlBoolean sqlBoolean = SqlGeometry.STIntersects(geom.SqlGeometry); //Within caused problems , can be tested again , there is a difference between the two methods but on the operational side mostly one needs intersects anyway.
            bool retval = sqlBoolean.Value;
            return retval;
        }

        /// <summary>
        /// Returns 'true' if this <see cref="Geometry"/> ‘spatially contains’ another <see cref="Geometry"/>.
        /// </summary>
        public virtual bool Contains(Geometry geom)
        {
            UpdateSqlGeometry();
            geom.UpdateSqlGeometry();
            SqlBoolean sqlBoolean = SqlGeometry.STContains(geom.SqlGeometry);
            bool retval = sqlBoolean.Value;
            return retval;
        }

        /// <summary>
        /// Returns 'true' if this <see cref="Geometry"/> 'spatially overlaps' another <see cref="Geometry"/>.
        /// </summary>
        public virtual bool Overlaps(Geometry geom)
        {
            UpdateSqlGeometry();
            geom.UpdateSqlGeometry();
            SqlBoolean sqlBoolean = SqlGeometry.STOverlaps(geom.SqlGeometry);
            bool retval = sqlBoolean.Value;
            return retval;
        }


        /// <summary>
        /// Returns 'true' if this <see cref="Geometry"/> is spatially related to another <see cref="Geometry"/>, by testing
        /// for intersections between the Interior, Boundary and Exterior of the two geometries
        /// as specified by the values in the intersectionPatternMatrix
        /// </summary>
        /// <param name="other"><see cref="Geometry"/> to relate to</param>
        /// <param name="intersectionPattern">Intersection Pattern</param>
        /// <returns>True if spatially related</returns>
        public bool Relate(Geometry other, string intersectionPattern)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region "Methods that support Spatial Analysis"

        /// <summary>
        /// Returns the shortest distance between any two points in the two geometries
        /// as calculated in the spatial reference system of this Geometry.
        /// </summary>
        public virtual double Distance(Geometry geom)
        {
            UpdateSqlGeometry();
            SqlGeometry sqlGeo1 = this.AsSqlServerGeometry();
            SqlGeometry sqlGeo2 = geom.AsSqlServerGeometry();
            double retval = sqlGeo1.STDistance(sqlGeo2).Value;
            return retval;
        }

        /// <summary>
        /// Returns a geometry that represents all points whose distance from this Geometry
        /// is less than or equal to distance. Calculations are in the Spatial Reference
        /// System of this Geometry.
        /// </summary>
        /// <param name="d">Buffer distance</param>
        public virtual Geometry Buffer(double d)
        {
            UpdateSqlGeometry();
            SqlGeometry calcResult = SqlGeometry.STBuffer(d);
            Geometry retval = calcResult.AsSCEGeometryExtention();
            return retval;
        }


        /// <summary>
        /// Geometry—Returns a geometry that represents the convex hull of this Geometry.
        /// </summary>
        public virtual Geometry ConvexHull()
        {
            UpdateSqlGeometry();
            SqlGeometry convexHull = SqlGeometry.STConvexHull();
            Geometry retval = convexHull.AsSCEGeometryExtention();
            return retval;
        }

        /// <summary>
        /// Returns a geometry that represents the point set intersection of this Geometry
        /// with anotherGeometry.
        /// </summary>
        public virtual Geometry Intersection(Geometry geom)
        {
            UpdateSqlGeometry();
            SqlGeometry result = SqlGeometry.STIntersection(geom.SqlGeometry);
            Geometry retval = result.AsSCEGeometryExtention();
            return retval;
        }

        /// <summary>
        /// Returns a geometry that represents the point set union of this Geometry with anotherGeometry.
        /// </summary>
        public virtual Geometry Union(Geometry geom)
        {
            UpdateSqlGeometry();
            SqlGeometry calcResult = SqlGeometry.STUnion(geom.SqlGeometry);
            Geometry retval = calcResult.AsSCEGeometryExtention();
            return retval;
        }

        /// <summary>
        /// Returns a geometry that represents the point set difference of this Geometry with anotherGeometry.
        /// </summary>
        public virtual Geometry Difference(Geometry geom)
        {
            UpdateSqlGeometry();
            SqlGeometry difference = SqlGeometry.STDifference(geom.SqlGeometry);
            Geometry retval = difference.AsSCEGeometryExtention();
            return retval;
        }

        /// <summary>
        /// Returns a geometry that represents the point set symmetric difference of this Geometry with anotherGeometry.
        /// </summary>
        public virtual Geometry SymDifference(Geometry geom)
        {
            UpdateSqlGeometry();
            SqlGeometry difference = SqlGeometry.STSymDifference(geom.SqlGeometry);
            Geometry retval = difference.AsSCEGeometryExtention();
            return retval;
        }

        #endregion

        /// <summary>
        /// This method must be overridden using 'public new [derived_data_type] Clone()'
        /// </summary>
        /// <returns>Copy of Geometry</returns>
        public Geometry Clone()
        {
            UpdateSqlGeometry();
            string wkt = this.ToWKTExtention();
            Geometry geo = (Geometry)wkt.FromWKTExtention();
            return geo;
        }

        /// <summary>
        /// Determines whether the specified <see cref="Object"/> is equal to the current <see cref="Object"/>.
        /// </summary>
        /// <param name="obj">The <see cref="Object"/> to compare with the current <see cref="Object"/>.</param>
        /// <returns>true if the specified <see cref="Object"/> is equal to the current <see cref="Object"/>; otherwise, false</returns>
        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            else
            {
                Geometry g = obj as Geometry;
                if (g == null)
                    return false;
                else
                    return Equals(g);
            }
        }

        /// <summary>
        /// Serves as a hash function for a particular type. <see cref="GetHashCode"/> is suitable for use 
        /// in hashing algorithms and data structures like a hash table.
        /// </summary>
        /// <returns>A hash code for the current <see cref="GetHashCode"/>.</returns>
        public override int GetHashCode()
        {
            return GetHashCode();
        }

        public int SRID
        {
            get
            {
                return (int)SpatialReference.AuthorityCode;
            }
            set
            {
                int zone = value - 32600;
                SpatialReference = ProjectedCoordinateSystem.WGS84_UTM(zone, true);
            }
        }

        protected void UpdateSqlGeometry()
        {
            _sqlGeometry = this.AsSqlServerGeometry();
        }

        internal SqlGeometry SqlGeometry
        {
            get
            {
                return _sqlGeometry;
            }
        }

        public override string ToString()
        {
            string retval = "WKT_ERROR";
            try
            {
                retval = this.ToWKTExtention();
            }
            catch { }
            return retval;
        }
    }
}