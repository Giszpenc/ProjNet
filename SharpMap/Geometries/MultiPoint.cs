using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace C4I.Applications.SCE.Interfaces
{
    /// <summary>
    /// A MultiPoint is a 0 dimensional geometric collection. The elements of a MultiPoint are
    /// restricted to Points. The points are not connected or ordered.
    /// </summary>
    /// 
    [Serializable]
    public class MultiPoint : GeometryCollection, IPuntal, ISpatialEntity
    {
        private List<Point> _Points;

        /// <summary>
        /// Initializes a new MultiPoint collection
        /// </summary>
        public MultiPoint()
        {
            _Points = new List<Point>();
        }

        /// <summary>
        /// Initializes a new MultiPoint collection
        /// </summary>		
        public MultiPoint(IEnumerable<Point> points)
        {
            _Points = new List<Point>();
            _Points.AddRange(points);
        }

        /// <summary>
        /// Gets the n'th point in the MultiPoint collection
        /// </summary>
        /// <param name="n">Index in collection</param>
        /// <returns>Point</returns>
        public new Point this[int n]
        {
            get { return _Points[n]; }
        }

        /// <summary>
        /// Gets or sets the MultiPoint collection
        /// </summary>
        public List<Point> Points
        {
            get { return _Points; }
            set { _Points = value; }
        }

        /// <summary>
        /// Returns the number of geometries in the collection.
        /// </summary>
        public override int NumGeometries
        {
            get { return _Points.Count; }
        }

        /// <summary>
        ///  The inherent dimension of this Geometry object, which must be less than or equal to the coordinate dimension.
        /// </summary>
        public override int Dimension
        {
            get { return 0; }
        }

        /// <summary>
        /// Returns an indexed geometry in the collection
        /// </summary>
        /// <param name="N">Geometry index</param>
        /// <returns>Geometry at index N</returns>
        public new Point Geometry(int N)
        {
            return _Points[N];
        }

        /// <summary>
        /// The minimum bounding box for this Geometry.
        /// </summary>
        /// <returns></returns>
        public override BoundingBox GetBoundingBox()
        {
            if (_Points == null || _Points.Count == 0)
                return null;
            BoundingBox bbox = new BoundingBox(_Points[0], _Points[0]);
            for (int i = 1; i < _Points.Count; i++)
            {
                bbox.Min.X = _Points[i].X < bbox.Min.X ? _Points[i].X : bbox.Min.X;
                bbox.Min.Y = _Points[i].Y < bbox.Min.Y ? _Points[i].Y : bbox.Min.Y;
                bbox.Max.X = _Points[i].X > bbox.Max.X ? _Points[i].X : bbox.Max.X;
                bbox.Max.Y = _Points[i].Y > bbox.Max.Y ? _Points[i].Y : bbox.Max.Y;
            }
            return bbox;
        }

        /// <summary>
        /// Return a copy of this geometry
        /// </summary>
        /// <returns>Copy of Geometry</returns>
        public new MultiPoint Clone()
        {
            MultiPoint geoms = new MultiPoint();
            for (int i = 0; i < _Points.Count; i++)
                geoms.Points.Add(_Points[i].Clone());
            return geoms;
        }

        /// <summary>
        /// Gets an enumerator for enumerating the geometries in the GeometryCollection
        /// </summary>
        /// <returns></returns>
        public override IEnumerator<Geometry> GetEnumerator()
        {
            foreach (Point p in _Points)
                yield return p;
        }

        public override GeometryType2 GeometryType
        {
            get
            {
                return GeometryType2.MultiPoint;
            }
        }

        public IEnumerator<Point> PointCollection
        {
            get { return _Points.GetEnumerator(); }
        }

        public void AddPoint(Point p)
        {
            _Points.Add(p);
        }

        public void RemovePoint(Point p)
        {
            _Points.Remove(p);
            //TODO:This was not tested for cases we get from one point to zero points.
        }

        public void SetPoint(int index, Point newPoint)
        {
            _Points[index] = newPoint;
        }

        public string OGCType
        {
            get { return "MULTIPOINT"; }
        }
    }
}