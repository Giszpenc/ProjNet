using System;
using System.Runtime.Serialization;
using System.Collections.Generic;

namespace C4I.Applications.SCE.Interfaces
{
    /// <summary>
    /// A Point is a 0-dimensional geometry and represents a single location in 3D coordinate space. A Point has a x coordinate
    /// value, a y-coordinate value and a z-coordinate value. The boundary of a Point is the empty set.
    /// </summary>
    [Serializable]
    //Orginally - Point3D
    public class Point : Geometry, ISpatialEntity, IComparable<Point>, IPuntal
    {
        public static readonly Point EmptyPoint = new Point(9999, 9999, 9999);
        private double _Z;
        private bool _IsEmpty = false;
        private double _X;
        private double _Y;
        private bool _inited;

        /// <summary>
        /// Initializes a new PointInternal
        /// </summary>
        /// <param name="x">X coordinate</param>
        /// <param name="y">Y coordinate</param>
        /// <param name="z">Z coordinate</param>
        public Point(double x, double y, double z)
        {
            X = x;
            Y = y;
            Z = z;
        }


        /// <summary>
        /// Create a new point by a douuble[] array
        /// </summary>
        /// <param name="point"></param>
        public Point(double[] point)
        {
            if (point.Length != 3)
                throw new Exception("Only 3 dimensions are supported for points");
            X = point[0];
            Y = point[1];
            Z = point[2];
        }

        public Point()
        {
            _X = 0;
            _Y = 0;
            _Z = 0;
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
                _inited = true;
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
                _inited = true;
                _IsEmpty = false;
            }
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
        public static Point FromDMS(double longDegrees, double longMinutes, double longSeconds,
                                    double latDegrees, double latMinutes, double latSeconds)
        {
            double x = longDegrees + longMinutes / 60 + longSeconds / 3600;
            double y = latDegrees + latMinutes / 60 + latSeconds / 3600;
            return new Point(x, y, 0);
        }


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
        /// If true, then this Geometry represents the empty PointInternal set, Ø, for the coordinate space. 
        /// </summary>
        /// <returns>Returns 'true' if this Geometry is the empty geometry</returns>
        public override bool IsEmpty()
        {
            return _IsEmpty;
        }

        /// <summary>
        /// Returns 'true' if this Geometry has no anomalous geometric PointInternals, such as self
        /// intersection or self tangency. The description of each instantiable geometric class will include the specific
        /// conditions that cause an instance of that class to be classified as not simple.
        /// </summary>
        /// <returns>true if the geometry is simple</returns>
        public override bool IsSimple()
        {
            return true;
        }

        /// <summary>
        /// Returns the distance between this PointInternal and a <see cref="BoundingBox"/>
        /// </summary>
        /// <param name="box"></param>
        /// <returns></returns>
        public double Distance(BoundingBox box)
        {
            return box.Distance(this);
        }

        #endregion

        /// <summary>
        /// Gets or sets the Z coordinate of the point
        /// </summary>
        public double Z
        {
            get
            {
                if (!IsEmpty())
                    return _Z;
                else throw new ApplicationException("PointInternal is empty");
            }
            set
            {
                _Z = value;
                _inited = true;
                SetIsEmpty = false;
            }
        }

        /// <summary>
        /// Returns part of coordinate. Index 0 = X, Index 1 = Y, , Index 2 = Z
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public double this[uint index]
        {
            get
            {
                if (_IsEmpty)
                    throw new ApplicationException("PointInternal is empty");
                if (index == 0)
                    return X;
                if (index == 1)
                    return Y;
                if (index == 2)
                    return Z;

                throw new IndexOutOfRangeException("Point has three dimensions - index " + index + " is out of bounds");
            }
            set
            {
                if (index == 0)
                {
                    X = value;
                }
                else if (index == 1)
                {
                    Y = value;
                }
                else if (index == 2)
                {
                    Z = value;
                }
                else throw new IndexOutOfRangeException("Point has three dimensions - index " + index + " is out of bounds");
            }
        }

        /// <summary>
        /// Returns the number of ordinates for this point
        /// </summary>
        public int NumOrdinates
        {
            get { return 3; }
        }

        #region Operators

        /// <summary>
        /// Vector + Vector
        /// </summary>
        /// <param name="v1">Vector</param>
        /// <param name="v2">Vector</param>
        /// <returns></returns>
        public static Point operator +(Point v1, Point v2)
        {
            return new Point(v1.X + v2.X, v1.Y + v2.Y, v1.Z + v2.Z);
        }


        /// <summary>
        /// Vector - Vector
        /// </summary>
        /// <param name="v1">Vector</param>
        /// <param name="v2">Vector</param>
        /// <returns>Cross product</returns>
        public static Point operator -(Point v1, Point v2)
        {
            return new Point(v1.X - v2.X, v1.Y - v2.Y, v1.Z - v2.Z);
        }

        /// <summary>
        /// Vector * Scalar
        /// </summary>
        /// <param name="m">Vector</param>
        /// <param name="d">Scalar (double)</param>
        /// <returns></returns>
        public static Point operator *(Point m, double d)
        {
            return new Point(m.X * d, m.Y * d, m.Z * d);
        }

        #endregion

        #region "Inherited methods from abstract class Geometry"

        /// <summary>
        /// Checks whether this instance is spatially equal to the PointInternal 'o'
        /// </summary>
        /// <param name="p">PointInternal to compare to</param>
        /// <returns></returns>
        public bool Equals(Point p)
        {
            bool notNull = p != null;
            bool eX = p.X == _X;
            bool eY = p.Y == _Y;
            bool eZ = p.Z == _Z;
            bool eEmpty = _IsEmpty == p.IsEmpty();

            bool result = notNull && eX && eY && eZ && eEmpty;
            return result;
        }

        /// <summary>
        /// Serves as a hash function for a particular type. <see cref="GetHashCode"/> is suitable for use 
        /// in hashing algorithms and data structures like a hash table.
        /// </summary>
        /// <returns>A hash code for the current <see cref="GetHashCode"/>.</returns>
        public override int GetHashCode()
        {
            return _X.GetHashCode() ^ _Y.GetHashCode() ^ _Z.GetHashCode() ^ _IsEmpty.GetHashCode();
        }

        /// <summary>
        /// Returns the distance between this geometry instance and another geometry, as
        /// measured in the spatial reference system of this instance.
        /// </summary>
        /// <param name="geom"></param>
        /// <returns></returns>
        public override double Distance(Geometry geom)
        {
            if (geom.GetType() == typeof(Point))
            {
                Point p = geom as Point;
                return Math.Sqrt(Math.Pow(X - p.X, 2) + Math.Pow(Y - p.Y, 2) + Math.Pow(Z - p.Z, 2));
            }
            else
            {
                throw new ArgumentException("Geom provided for distance calculation is not point");
            }
        }

        /// <summary>
        /// The minimum bounding box for this Geometry.
        /// </summary>
        /// <returns></returns>
        public override BoundingBox GetBoundingBox()
        {
            return new BoundingBox(X, Y, X, Y);
        }

        #endregion

        /// <summary>
        /// exports a point into a 3-dimensional double array
        /// </summary>
        /// <returns></returns>
        public double[] ToDoubleArray()
        {
            return new double[3] { X, Y, Z };
        }

        /// <summary>
        /// This method must be overridden using 'public new [derived_data_type] Clone()'
        /// </summary>
        /// <returns>Clone</returns>
        public new Point Clone()
        {
            return new Point(X, Y, Z);
        }

        /// <summary>
        /// Checks whether the two points are spatially equal
        /// </summary>
        /// <param name="p1">PointInternal 1</param>
        /// <param name="p2">PointInternal 2</param>
        /// <returns>true if the points a spatially equal</returns>
        public bool Equals(Point p1, Point p2)
        {
            return (p1.X == p2.X && p1.Y == p2.Y && p1.Z == p2.Z);
        }

        /// <summary>
        /// Comparator used for ordering point first by ascending X, then by ascending Y and then by ascending Z.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public virtual int CompareTo(Point other)
        {
            if (X < other.X || X == other.X && Y < other.Y || X == other.X && Y == other.Y && Z < other.Z)
                return -1;
            else if (X > other.X || X == other.X && Y > other.Y || X == other.X && Y == other.Y && Z > other.Z)
                return 1;
            else // (this.X == other.X && this.Y == other.Y && this.Z == other.Z)
                return 0;
        }

        public IEnumerator<Point> PointCollection
        {
            get
            {
                List<Point> _pointCollectionList = new List<Point>();
                _pointCollectionList.Add(this);
                IEnumerator<Point> retval = _pointCollectionList.GetEnumerator();
                return retval;
            }
        }

        [DataMember]
        public string OGCType
        {
            get
            {
                return "Point";
            }
            private set
            {
            }
        }

        public void AddPoint(Point p)
        {

            if (!_inited)
            {
                X = p.X;
                Y = p.Y;
                Z = p.Z;
                _inited = true;
            }
            else
            {
                throw new Exception("PointInternal already created , can not add point after X,Y,Z properties called or SetPoint method called.");
            }
        }

        public void RemovePoint(Point p)
        {
            throw new NotImplementedException();
        }

        public void SetPoint(int index, Point newPoint)
        {
            if (index == 0)
            {
                X = newPoint.X;
                Y = newPoint.Y;
                Z = newPoint.Z;
                _inited = true;
            }
            else
            {
                throw new IndexOutOfRangeException();
            }
        }

    }
}