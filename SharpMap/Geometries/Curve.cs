using System;
namespace C4I.Applications.SCE.Interfaces
{
    /// <summary>
    /// A Curve is a one-dimensional geometric object usually stored as a sequence of points,
    /// with the subtype of Curve specifying the form of the interpolation between points.
    /// </summary>
    /// 
    [Serializable]
    public abstract class Curve : Geometry, ILineal
    {
        /// <summary>
        ///  The inherent dimension of this Geometry object, which must be less than or equal to the coordinate dimension.
        /// </summary>
        public override int Dimension
        {
            get { return 1; }
        }

        /// <summary>
        /// The length of this Curve in its associated spatial reference.
        /// </summary>
        public abstract double Length { get; }

        /// <summary>
        /// The start point of this Curve.
        /// </summary>
        public abstract Point StartPoint { get; }

        /// <summary>
        /// The end point of this Curve.
        /// </summary>
        public abstract Point EndPoint { get; }

        /// <summary>
        /// Returns true if this Curve is closed (StartPoint = EndPoint).
        /// </summary>
        public bool IsClosed
        {
            get { return (StartPoint.Equals(EndPoint)); }
        }

        /// <summary>
        /// true if this Curve is closed (StartPoint = EndPoint) and
        /// this Curve is simple (does not pass through the same point more than once).
        /// </summary>
        public abstract bool IsRing { get; }

        /// <summary>
        /// The position of a point on the line, parameterised by length.
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public abstract Point Value(double t);

        public override GeometryType2 GeometryType
        {
            get
            {
                return GeometryType2.Curve;
            }
        }
    }
}