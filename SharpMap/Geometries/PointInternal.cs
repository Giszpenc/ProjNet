using System;
using System.Collections.Generic;

namespace C4I.Applications.SCE.Interfaces
{
    /// <summary>
    /// A PointInternal is a 0-dimensional geometry and represents a single location in 2D coordinate space. A PointInternal has a x coordinate
    /// value and a y-coordinate value. The boundary of a PointInternal is the empty set.
    /// </summary>
    [Serializable]
    internal class PointInternal : Geometry, IComparable<PointInternal>, IPuntal, ISpatialEntity
    {


        private bool _IsEmpty = false;
        private double _X;
        private double _Y;

        /// <summary>
        /// Initializes a new PointInternal
        /// </summary>
        /// <param name="x">X coordinate</param>
        /// <param name="y">Y coordinate</param>
        public PointInternal(double x, double y)
        {
            _X = x;
            _Y = y;
        }



        /// <summary>
        /// Sets whether this object is empty
        /// </summary>
        protected bool SetIsEmpty
        {
            set { _IsEmpty = value; }
        }

        /// <summary>
        /// Gets or sets the X coordinate of the PointInternal
        /// </summary>
        public double X
        {
            get
            {
                if (!_IsEmpty)
                    return _X;
                else throw new ApplicationException("PointInternal is empty");
            }
            set
            {
                _X = value;
                _IsEmpty = false;
            }
        }

        /// <summary>
        /// Gets or sets the Y coordinate of the PointInternal
        /// </summary>
        public double Y
        {
            get
            {
                if (!_IsEmpty)
                    return _Y;
                else throw new ApplicationException("PointInternal is empty");
            }
            set
            {
                _Y = value;
                _IsEmpty = false;
            }
        }

        /// <summary>
        /// Returns part of coordinate. Index 0 = X, Index 1 = Y
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public virtual double this[uint index]
        {
            get
            {
                if (_IsEmpty)
                    throw new ApplicationException("PointInternal is empty");
                else if (index == 0)
                    return X;
                else if
                    (index == 1)
                    return Y;
                else
                    throw (new Exception("PointInternal index out of bounds"));
            }
            set
            {
                if (index == 0)
                    X = value;
                else if (index == 1)
                    Y = value;
                else
                    throw (new Exception("PointInternal index out of bounds"));
                _IsEmpty = false;
            }
        }

        /// <summary>
        /// Returns the number of ordinates for this PointInternal
        /// </summary>
        public virtual int NumOrdinates
        {
            get { return 2; }
        }

        /// <summary>
        /// If true, then this Geometry represents the empty PointInternal set, Ø, for the coordinate space. 
        /// </summary>
        /// <returns>Returns 'true' if this Geometry is the empty geometry</returns>
        public override bool IsEmpty()
        {
            return _IsEmpty;
        }

        /// <summary>
        /// exports a PointInternal into a 2-dimensional double array
        /// </summary>
        /// <returns></returns>
        public double[] ToDoubleArray()
        {
            return new double[2] { _X, _Y };
        }

        /// <summary>
        /// Returns a PointInternal based on degrees, minutes and seconds notation.
        /// For western or southern coordinates, add minus '-' in front of all longitude and/or latitude values
        /// </summary>
        /// <param name="longDegrees">Longitude degrees</param>
        /// <param name="longMinutes">Longitude minutes</param>
        /// <param name="longSeconds">Longitude seconds</param>
        /// <param name="latDegrees">Latitude degrees</param>
        /// <param name="latMinutes">Latitude minutes</param>
        /// <param name="latSeconds">Latitude seconds</param>
        /// <returns>PointInternal</returns>
        public static PointInternal FromDMS(double longDegrees, double longMinutes, double longSeconds,
                                    double latDegrees, double latMinutes, double latSeconds)
        {
            return new PointInternal(longDegrees + longMinutes / 60 + longSeconds / 3600,
                             latDegrees + latMinutes / 60 + latSeconds / 3600);
        }

        /// <summary>
        /// Returns a 2D <see cref="PointInternal"/> instance from this <see cref="PointInternal3D"/>
        /// </summary>
        /// <returns><see cref="PointInternal"/></returns>
        public PointInternal AsPointInternal()
        {
            return new PointInternal(_X, _Y);
        }

        /// <summary>
        /// This method must be overridden using 'public new [derived_data_type] Clone()'
        /// </summary>
        /// <returns>Clone</returns>
        public new PointInternal Clone()
        {
            return new PointInternal(X, Y);
        }

        #region Operators

        /// <summary>
        /// Vector + Vector
        /// </summary>
        /// <param name="v1">Vector</param>
        /// <param name="v2">Vector</param>
        /// <returns></returns>
        public static PointInternal operator +(PointInternal v1, PointInternal v2)
        {
            return new PointInternal(v1.X + v2.X, v1.Y + v2.Y);
        }


        /// <summary>
        /// Vector - Vector
        /// </summary>
        /// <param name="v1">Vector</param>
        /// <param name="v2">Vector</param>
        /// <returns>Cross product</returns>
        public static PointInternal operator -(PointInternal v1, PointInternal v2)
        {
            return new PointInternal(v1.X - v2.X, v1.Y - v2.Y);
        }

        /// <summary>
        /// Vector * Scalar
        /// </summary>
        /// <param name="m">Vector</param>
        /// <param name="d">Scalar (double)</param>
        /// <returns></returns>
        public static PointInternal operator *(PointInternal m, double d)
        {
            return new PointInternal(m.X * d, m.Y * d);
        }

        #endregion

        #region "Inherited methods from abstract class Geometry"

        public override GeometryType2 GeometryType
        {
            get
            {
                return GeometryType2.Point;
            }
        }

        /// <summary>
        ///  The inherent dimension of this Geometry object, which must be less than or equal to the coordinate dimension.
        /// </summary>
        public override int Dimension
        {
            get { return 0; }
        }

        /// <summary>
        /// Checks whether this instance is spatially equal to the PointInternal 'o'
        /// </summary>
        /// <param name="p">PointInternal to compare to</param>
        /// <returns></returns>
        public virtual bool Equals(PointInternal p)
        {
            return p != null && p.X == _X && p.Y == _Y && _IsEmpty == p.IsEmpty();
        }

        /// <summary>
        /// Serves as a hash function for a particular type. <see cref="GetHashCode"/> is suitable for use 
        /// in hashing algorithms and data structures like a hash table.
        /// </summary>
        /// <returns>A hash code for the current <see cref="GetHashCode"/>.</returns>
        public override int GetHashCode()
        {
            return _X.GetHashCode() ^ _Y.GetHashCode() ^ _IsEmpty.GetHashCode();
        }


        /// <summary>
        /// The minimum bounding box for this Geometry.
        /// </summary>
        /// <returns></returns>
        public override BoundingBox GetBoundingBox()
        {
            return new BoundingBox(X, Y, X, Y);
        }

        /// <summary>
        /// Checks whether this PointInternal touches a <see cref="BoundingBox"/>
        /// </summary>
        /// <param name="box">box</param>
        /// <returns>true if they touch</returns>
        public bool Touches(BoundingBox box)
        {
            return box.Touches(this);
        }
        /// <summary>
        /// Returns true if this instance contains 'geom'
        /// </summary>
        /// <param name="geom">Geometry</param>
        /// <returns>True if geom is contained by this instance</returns>
        public override bool Contains(Geometry geom)
        {
            return false;
        }

        #endregion

        public int CompareTo(PointInternal other)
        {
            throw new NotImplementedException();
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

        public string OGCType
        {
            get { throw new NotImplementedException(); }
        }
    }
}