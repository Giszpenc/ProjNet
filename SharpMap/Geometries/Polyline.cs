using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;

namespace C4I.Applications.SCE.Interfaces
{
    /// <summary>
    /// A MultiLineString is a MultiCurve whose elements are LineStrings.
    /// </summary>
    [Serializable]
    public class Polyline : MultiCurve, ISpatialEntity
    {
        private IList<LineString> _LineStrings;

        [DataMember]
        private List<Point> _PointCollection;

        /// <summary>
        /// Initializes an instance of a MultiLineString
        /// </summary>
        public Polyline()
        {
            _LineStrings = new Collection<LineString>();
            _PointCollection = new List<Point>();
        }

        /// <summary>
        /// Collection of <see cref="LineString">LineStrings</see> in the <see cref="Polyline"/>
        /// </summary>
        public IList<LineString> LineStrings
        {
            get { return _LineStrings; }
            set { _LineStrings = value; }
        }

        /// <summary>
        /// Returns an indexed geometry in the collection
        /// </summary>
        /// <param name="index">Geometry index</param>
        /// <returns>Geometry at index</returns>
        public new LineString this[int index]
        {
            get { return _LineStrings[index]; }
        }

        /// <summary>
        /// Returns true if all LineStrings in this MultiLineString is closed (StartPoint=EndPoint for each LineString in this MultiLineString)
        /// </summary>
        public override bool IsClosed
        {
            get
            {
                for (int i = 0; i < _LineStrings.Count; i++)
                    if (!_LineStrings[i].IsClosed)
                        return false;
                return true;
            }
        }

        /// <summary>
        /// The length of this MultiLineString which is equal to the sum of the lengths of the element LineStrings.
        /// </summary>
        public override double Length
        {
            get
            {
                double l = 0;
                for (int i = 0; i < _LineStrings.Count; i++)
                    l += _LineStrings[i].Length;
                return l;
            }
        }

        /// <summary>
        /// Returns the number of geometries in the collection.
        /// </summary>
        public override int NumGeometries
        {
            get { return _LineStrings.Count; }
        }

        /// <summary>
        /// If true, then this Geometry represents the empty point set, Ø, for the coordinate space. 
        /// </summary>
        /// <returns>Returns 'true' if this Geometry is the empty geometry</returns>
        public override bool IsEmpty()
        {
            if (_LineStrings == null || _LineStrings.Count == 0)
                return true;

            for (int i = 0; i < _LineStrings.Count; i++)
                if (!_LineStrings[i].IsEmpty())
                    return false;

            return true;
        }


        /// <summary>
        /// Returns an indexed geometry in the collection
        /// </summary>
        /// <param name="N">Geometry index</param>
        /// <returns>Geometry at index N</returns>
        public override Geometry Geometry(int N)
        {
            return _LineStrings[N];
        }

        /// <summary>
        /// The minimum bounding box for this Geometry.
        /// </summary>
        /// <returns></returns>
        public override BoundingBox GetBoundingBox()
        {
            if (_LineStrings == null || _LineStrings.Count == 0)
                return null;
            BoundingBox bbox = _LineStrings[0].GetBoundingBox();
            for (int i = 1; i < _LineStrings.Count; i++)
                bbox = bbox.Join(_LineStrings[i].GetBoundingBox());
            return bbox;
        }

        /// <summary>
        /// Return a copy of this geometry
        /// </summary>
        /// <returns>Copy of Geometry</returns>
        public new Polyline Clone()
        {
            Polyline geoms = new Polyline();
            for (int i = 0; i < _LineStrings.Count; i++)
                geoms.LineStrings.Add(_LineStrings[i].Clone());
            return geoms;
        }

        /// <summary>
        /// Gets an enumerator for enumerating the geometries in the GeometryCollection
        /// </summary>
        /// <returns></returns>
        public override IEnumerator<Geometry> GetEnumerator()
        {
            foreach (LineString l in _LineStrings)
                yield return l;
        }

        public override GeometryType2 GeometryType
        {
            get
            {
                return GeometryType2.MultiLineString;
            }
        }

        public IEnumerator<Point> PointCollection
        {
            get
            {
                IEnumerator<Point> enumarator = _PointCollection.GetEnumerator();
                return enumarator;
            }
        }

        public List<Point> PointCollectionList
        {
            get { return _PointCollection.ToList(); }
        }

        public string OGCType
        {
            get
            {
                return "Polyline";
            }
            private set
            {
            }
        }

        public void AddPoint(Point p)
        {
            _PointCollection.Add(p);
        }

        public void RemovePoint(Point p)
        {
            _PointCollection.Remove(p);
        }

        public void SetPoint(int index, Point newPoint)
        {
            _PointCollection[index] = newPoint;
        }

    }
}