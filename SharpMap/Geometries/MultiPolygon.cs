using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;

namespace C4I.Applications.SCE.Interfaces
{
    /// <summary>
    /// A MultiPolygon is a MultiSurface whose elements are Polygons.
    /// </summary>
    [Serializable]
    public class MultiPolygon : MultiSurface, ISpatialEntity
    {
        private IList<Polygon> _Polygons;

        /// <summary>
        /// Instantiates a MultiPolygon
        /// </summary>
        public MultiPolygon()
        {
            _Polygons = new Collection<Polygon>();
        }

        /// <summary>
        /// Collection of polygons in the multipolygon
        /// </summary>
        public IList<Polygon> Polygons
        {
            get { return _Polygons; }
            set
            {
                _Polygons = value;
            }
        }

        /// <summary>
        /// Returns an indexed geometry in the collection
        /// </summary>
        /// <param name="index">Geometry index</param>
        /// <returns>Geometry at index</returns>
        public new Polygon this[int index]
        {
            get { return _Polygons[index]; }
        }

        /// <summary>
        /// Returns summed area of the Polygons in the MultiPolygon collection
        /// </summary>
        public override double Area
        {
            get
            {
                double result = 0;
                for (int i = 0; i < _Polygons.Count; i++)
                    result += _Polygons[i].Area;
                return result;
            }
        }

        /// <summary>
        /// A point guaranteed to be on this Surface.
        /// </summary>
        public override Point PointOnSurface
        {
            get { throw new NotImplementedException(); }
        }

        /// <summary>
        /// Returns the number of geometries in the collection.
        /// </summary>
        public override int NumGeometries
        {
            get { return _Polygons.Count; }
        }

        /// <summary>
        /// If true, then this Geometry represents the empty point set, Ø, for the coordinate space. 
        /// </summary>
        /// <returns>Returns 'true' if this Geometry is the empty geometry</returns>
        public override bool IsEmpty()
        {
            if (_Polygons == null || _Polygons.Count == 0)
                return true;
            for (int i = 0; i < _Polygons.Count; i++)
                if (!_Polygons[i].IsEmpty())
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
            return _Polygons[N];
        }

        /// <summary>
        /// Returns the bounding box of the object
        /// </summary>
        /// <returns>bounding box</returns>
        public override BoundingBox GetBoundingBox()
        {
            if (_Polygons == null || _Polygons.Count == 0)
                return null;
            BoundingBox bbox = Polygons[0].GetBoundingBox();
            for (int i = 1; i < Polygons.Count; i++)
                bbox = bbox.Join(Polygons[i].GetBoundingBox());
            return bbox;
        }

        /// <summary>
        /// Return a copy of this geometry
        /// </summary>
        /// <returns>Copy of Geometry</returns>
        public new MultiPolygon Clone()
        {
            MultiPolygon geoms = new MultiPolygon();
            for (int i = 0; i < _Polygons.Count; i++)
                geoms.Polygons.Add(_Polygons[i].Clone());
            return geoms;
        }

        /// <summary>
        /// Gets an enumerator for enumerating the geometries in the GeometryCollection
        /// </summary>
        /// <returns></returns>
        public override IEnumerator<Geometry> GetEnumerator()
        {
            foreach (Polygon p in _Polygons)
                yield return p;
        }

        public override GeometryType2 GeometryType
        {
            get
            {
                return GeometryType2.MultiPolygon;
            }
        }


        public IEnumerator<Point> PointCollection
        {
            get { throw new NotImplementedException(); }
        }

        public void AddPoint(Point p)
        {
            throw new NotImplementedException();
        }

        public void RemovePoint(Point p)
        {
            throw new NotImplementedException();
        }

        public void SetPoint(int index, Point newPoint)
        {
            throw new NotImplementedException();
        }

        [DataMember]
        public string OGCType
        {
            get { return "MULTIPOLYGON"; }
        }

        public new int SRID
        {
            get;
            set;
        }
    }
}