using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace C4I.Applications.SCE.Interfaces
{
    /// <summary>
    /// A LineString is a Curve with linear interpolation between points. Each consecutive pair of points defines a
    /// line segment.
    /// </summary>
    [Serializable]
    public class LineString : Curve,ISpatialEntity
    {
        private IList<Point> _Vertices;

        /// <summary>
        /// Initializes an instance of a LineString from a set of vertices
        /// </summary>
        /// <param name="vertices"></param>
        public LineString(IList<Point> vertices)
        {
            _Vertices = vertices;
        }

        /// <summary>
        /// Initializes an instance of a LineString
        /// </summary>
        public LineString()
            : this(new Collection<Point>())
        {
        }

        /// <summary>
        /// Initializes an instance of a LineString
        /// </summary>
        /// <param name="points"></param>
        public LineString(IEnumerable<double[]> points)
        {
            Collection<Point> vertices = new Collection<Point>();

            foreach (double[] point in points)
                vertices.Add(new Point(point));

            _Vertices = vertices;
        }

        /// <summary>
        /// Gets or sets the collection of vertices in this Geometry
        /// </summary>
        public virtual IList<Point> Vertices
        {
            get { return _Vertices; }
            set { _Vertices = value; }
        }

        /// <summary>
        /// Returns the vertice where this Geometry begins
        /// </summary>
        /// <returns>First vertice in LineString</returns>
        public override Point StartPoint
        {
            get
            {
                if (_Vertices.Count == 0)
                    throw new ApplicationException("No startpoint found: LineString has no vertices.");
                return _Vertices[0];
            }
        }

        /// <summary>
        /// Gets the vertice where this Geometry ends
        /// </summary>
        /// <returns>Last vertice in LineString</returns>
        public override Point EndPoint
        {
            get
            {
                if (_Vertices.Count == 0)
                    throw new ApplicationException("No endpoint found: LineString has no vertices.");
                return _Vertices[_Vertices.Count - 1];
            }
        }

        /// <summary>
        /// Returns true if this LineString is closed and simple
        /// </summary>
        public override bool IsRing
        {
            get { return (IsClosed && IsSimple()); }
        }

        /// <summary>
        /// The length of this LineString, as measured in the spatial reference system of this LineString.
        /// </summary>
        public override double Length
        {
            get
            {
                if (Vertices.Count < 2)
                    return 0;
                double sum = 0;
                for (int i = 1; i < Vertices.Count; i++)
                    sum += Vertices[i].Distance(Vertices[i - 1]);
                return sum;
            }
        }

        #region OpenGIS Methods

        /// <summary>
        /// The number of points in this LineString.
        /// </summary>
        /// <remarks>This method is supplied as part of the OpenGIS Simple Features Specification</remarks>
        public virtual int NumPoints
        {
            get { return _Vertices.Count; }
        }

        /// <summary>
        /// Returns the specified point N in this Linestring.
        /// </summary>
        /// <remarks>This method is supplied as part of the OpenGIS Simple Features Specification</remarks>
        /// <param name="N"></param>
        /// <returns></returns>
        public Point Point(int N)
        {
            return _Vertices[N];
        }

        #endregion

        /// <summary>
        /// The position of a point on the line, parameterised by length.
        /// </summary>
        /// <param name="t">Distance down the line</param>
        /// <returns>Point at line at distance t from StartPoint</returns>
        public override Point Value(double t)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The minimum bounding box for this Geometry.
        /// </summary>
        /// <returns>BoundingBox for this geometry</returns>
        public override BoundingBox GetBoundingBox()
        {
            if (Vertices == null || Vertices.Count == 0)
                return null;
            BoundingBox bbox = new BoundingBox(Vertices[0], Vertices[0]);
            for (int i = 1; i < Vertices.Count; i++)
            {
                bbox.Min.X = Vertices[i].X < bbox.Min.X ? Vertices[i].X : bbox.Min.X;
                bbox.Min.Y = Vertices[i].Y < bbox.Min.Y ? Vertices[i].Y : bbox.Min.Y;
                bbox.Max.X = Vertices[i].X > bbox.Max.X ? Vertices[i].X : bbox.Max.X;
                bbox.Max.Y = Vertices[i].Y > bbox.Max.Y ? Vertices[i].Y : bbox.Max.Y;
            }
            return bbox;
        }

        /// <summary>
        /// Return a copy of this geometry
        /// </summary>
        /// <returns>Copy of Geometry</returns>
        public new LineString Clone()
        {
            LineString l = new LineString();
            for (int i = 0; i < _Vertices.Count; i++)
                l.Vertices.Add(_Vertices[i].Clone());
            return l;
        }

        #region "Inherited methods from abstract class Geometry"

        public override GeometryType2 GeometryType
        {
            get
            {
                return GeometryType2.LineString;
            }
        }

        /// <summary>
        /// Checks whether this instance is spatially equal to the LineString 'l'
        /// </summary>
        /// <param name="l">LineString to compare to</param>
        /// <returns>true of the objects are spatially equal</returns>
        public bool Equals(LineString l)
        {
            if (l == null)
                return false;
            if (l.Vertices.Count != Vertices.Count)
                return false;
            for (int i = 0; i < l.Vertices.Count; i++)
                if (!l.Vertices[i].Equals(Vertices[i]))
                    return false;
            return true;
        }

        /// <summary>
        /// Serves as a hash function for a particular type. <see cref="GetHashCode"/> is suitable for use 
        /// in hashing algorithms and data structures like a hash table.
        /// </summary>
        /// <returns>A hash code for the current <see cref="GetHashCode"/>.</returns>
        public override int GetHashCode()
        {
            int hash = 0;
            for (int i = 0; i < Vertices.Count; i++)
                hash = hash ^ Vertices[i].GetHashCode();
            return hash;
        }

        /// <summary>
        /// If true, then this Geometry represents the empty point set, Ø, for the coordinate space. 
        /// </summary>
        /// <returns>Returns 'true' if this Geometry is the empty geometry</returns>
        public override bool IsEmpty()
        {
            return _Vertices == null || _Vertices.Count == 0;
        }

        /// <summary>
        ///  Returns 'true' if this Geometry has no anomalous geometric points, such as self
        /// intersection or self tangency. The description of each instantiable geometric class will include the specific
        /// conditions that cause an instance of that class to be classified as not simple.
        /// </summary>
        /// <returns>true if the geometry is simple</returns>
        public override bool IsSimple()
        {
            //Collection<Point> verts = new Collection<Point>(_Vertices.Count);
            Collection<Point> verts = new Collection<Point>();

            for (int i = 0; i < _Vertices.Count; i++)
                //if (!verts.Exists(delegate(C4I.Applications.SCE.Interfaces.Point p) { return p.Equals(_Vertices[i]); }))
                if (0 != verts.IndexOf(_Vertices[i]))
                    verts.Add(_Vertices[i]);

            return (verts.Count == _Vertices.Count - (IsClosed ? 1 : 0));
        }
        #endregion

        public IEnumerator<Point> PointCollection
        {
            get
            {
                IEnumerator<Point> enumarator = _Vertices.GetEnumerator();
                return enumarator;
            }
        }

        public void AddPoint(Point p)
        {
            _Vertices.Add(p);
        }

        public void RemovePoint(Point p)
        {
            _Vertices.Remove(p);
        }

        public void SetPoint(int index, Point newPoint)
        {
            _Vertices[index] = newPoint; 
        }

        public string OGCType
        {
            get { return "Polyline"; }
        }
    }
}