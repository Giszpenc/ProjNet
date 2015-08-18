using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using Microsoft.SqlServer.Types;
using SMGeometry = C4I.Applications.SCE.Interfaces.Geometry;
using SMGeometryCollection = C4I.Applications.SCE.Interfaces.GeometryCollection;
using SMGeometryType = C4I.Applications.SCE.Interfaces.GeometryType2;
using SMLinearRing = C4I.Applications.SCE.Interfaces.LinearRing;
using SMLineString = C4I.Applications.SCE.Interfaces.LineString;
using SMMultiLineString = C4I.Applications.SCE.Interfaces.Polyline;
using SMMultiPoint = C4I.Applications.SCE.Interfaces.MultiPoint;
using SMMultiPolygon = C4I.Applications.SCE.Interfaces.MultiPolygon;
using SMPoint = C4I.Applications.SCE.Interfaces.Point;
using SMPolygon = C4I.Applications.SCE.Interfaces.Polygon;
using C4I.Applications.SCE.Configuration;


namespace C4I.Applications.SCE.Interfaces
{
    public static class SpatialDataFactoryNew
    {
        public static ISpatialEntity FromWKT(string wkt, int srid)
        {
            SqlGeometry geometry = SqlGeometry.STGeomFromText(new SqlChars(wkt), srid);
            geometry = geometry.MakeValid();
            ISpatialEntity retval = (ISpatialEntity)ToSharpMapGeometry(geometry);
            return retval;
        }

        public static ISpatialEntity FromWKTExtention(this string wkt)
        {
            return FromWKT(wkt, SceConfiguration.Instance.SRID);
        }

        public static ISpatialEntity FromWKTExtention(this string wkt, int srid)
        {
            return FromWKT(wkt, srid);
        }

        public static string ToWKT(Geometry entity)
        {
            SqlGeometry geometry = ((Geometry)entity).AsSqlServerGeometry();
            geometry = geometry.MakeValid();
            char[] value = geometry.AsTextZM().Value;
            string retval = new string(value);
            return retval;
        }

        public static string ToWKTExtention(this Geometry entity)
        {
            return ToWKT(entity);
        }

        internal static SqlGeometry ToSqlServerGeometry(SMGeometry smGeometry)
        {
            SqlGeometryBuilder builder = new SqlGeometryBuilder();
            builder.SetSrid(smGeometry.SRID);
            SharpMapGeometryToSqlGeometry(builder, smGeometry);
            SqlGeometry geometry = builder.ConstructedGeometry;
            geometry = geometry.MakeValid();
            return geometry;
        }

        internal static SqlGeometry AsSqlServerGeometry(this SMGeometry smGeometry)
        {
            SqlGeometry geometry = ToSqlServerGeometry(smGeometry);
            return geometry;
        }

        private static void SharpMapGeometryToSqlGeometry(SqlGeometryBuilder geomBuilder, SMGeometry smGeometry)
        {

            switch (smGeometry.GeometryType)
            {
                case SMGeometryType.Point:
                    SharpMapPointToSqlGeometry(geomBuilder, smGeometry as SMPoint);
                    break;
                case SMGeometryType.LineString:
                    SharpMapLineStringToSqlGeometry(geomBuilder, smGeometry as SMLineString);
                    break;
                case SMGeometryType.Polygon:
                    SharpMapPolygonToSqlGeometry(geomBuilder, smGeometry as SMPolygon);
                    break;
                case SMGeometryType.MultiPoint:
                    SharpMapMultiPointToSqlGeometry(geomBuilder, smGeometry as SMMultiPoint);
                    break;
                case SMGeometryType.MultiLineString:
                    SharpMapMultiLineStringToSqlGeometry(geomBuilder, smGeometry as SMMultiLineString);
                    break;
                case SMGeometryType.MultiPolygon:
                    SharpMapMultiPolygonToSqlGeometry(geomBuilder, smGeometry as SMMultiPolygon);
                    break;
                case SMGeometryType.GeometryCollection:
                    SharpMapGeometryCollectionToSqlGeometry(geomBuilder, smGeometry as SMGeometryCollection);
                    break;
                default:
                    throw new ArgumentException(
                        String.Format("Cannot convert '{0}' geometry type", smGeometry.GeometryType), "smGeometry");
            }
        }

        private static void SharpMapGeometryCollectionToSqlGeometry(SqlGeometryBuilder geomBuilder, SMGeometryCollection geometryCollection)
        {
            geomBuilder.BeginGeometry(OpenGisGeometryType.GeometryCollection);
            for (int i = 0; i < geometryCollection.NumGeometries; i++)
                SharpMapGeometryToSqlGeometry(geomBuilder, geometryCollection.Geometry(i));
            geomBuilder.EndGeometry();
        }

        private static void SharpMapMultiPolygonToSqlGeometry(SqlGeometryBuilder geomBuilder, SMMultiPolygon multiPolygon)
        {
            geomBuilder.BeginGeometry(OpenGisGeometryType.MultiPolygon);
            for (int i = 0; i < multiPolygon.NumGeometries; i++)
                SharpMapPolygonToSqlGeometry(geomBuilder, multiPolygon.Geometry(i) as SMPolygon);
            geomBuilder.EndGeometry();
        }

        private static void SharpMapMultiLineStringToSqlGeometry(SqlGeometryBuilder geomBuilder, SMMultiLineString multiLineString)
        {
            geomBuilder.SetSrid(multiLineString.SRID);
            geomBuilder.BeginGeometry(OpenGisGeometryType.MultiLineString);
            for (int i = 0; i < multiLineString.NumGeometries; i++)
                SharpMapLineStringToSqlGeometry(geomBuilder, multiLineString.Geometry(i) as SMLineString);
            geomBuilder.EndGeometry();
        }

        private static void SharpMapMultiPointToSqlGeometry(SqlGeometryBuilder geomBuilder, SMMultiPoint multiPoint)
        {
            // geomBuilder.SetSrid(multiPoint.SRID);
            geomBuilder.BeginGeometry(OpenGisGeometryType.MultiPoint);
            for (int i = 0; i < multiPoint.NumGeometries; i++)
                SharpMapPointToSqlGeometry(geomBuilder, multiPoint.Geometry(i));
            geomBuilder.EndGeometry();
        }

        private static void SharpMapPointToSqlGeometry(SqlGeometryBuilder geomBuilder, SMPoint point)
        {
            try
            {
                geomBuilder.BeginGeometry(OpenGisGeometryType.Point);
                geomBuilder.BeginFigure(point.X, point.Y, point.Z, null);
                geomBuilder.EndFigure();
                geomBuilder.EndGeometry();
            }
            catch (Exception ex)
            {
                string exMessage = string.Format("Failed Point X [{0}] Y [{1}] Z[{2}]", point.X, point.Y, point.Z);
                Exception wrappingException = new Exception(exMessage, ex);
                throw wrappingException;
            }
        }

        private static void SharpMapLineStringToSqlGeometry(SqlGeometryBuilder geomBuilder, SMLineString lineString)
        {
            geomBuilder.BeginGeometry(OpenGisGeometryType.LineString);
            SMPoint point = lineString.StartPoint;
            geomBuilder.BeginFigure(point.X, point.Y);
            for (int i = 1; i < lineString.NumPoints; i++)
            {
                point = lineString.Point(i);
                geomBuilder.AddLine(point.X, point.Y);
            }
            geomBuilder.EndFigure();
            geomBuilder.EndGeometry();
        }

        private static void SharpMapPolygonToSqlGeometry(SqlGeometryBuilder geomBuilder, SMPolygon polygon)
        {
            geomBuilder.BeginGeometry(OpenGisGeometryType.Polygon);
            AddRing(geomBuilder, polygon.ExteriorRing);
            for (int i = 0; i < polygon.NumInteriorRing; i++)
                AddRing(geomBuilder, polygon.InteriorRing(i));
            geomBuilder.EndGeometry();
        }

        private static void AddRing(SqlGeometryBuilder builder, SMLinearRing linearRing)
        {
            SMPoint pt = linearRing.StartPoint;

            builder.BeginFigure(pt.X, pt.Y);
            for (int i = 1; i < linearRing.NumPoints; i++)
            {
                pt = linearRing.Point(i);
                builder.AddLine(pt.X, pt.Y);
            }
            builder.EndFigure();
        }

        public static SMGeometry ToSharpMapGeometry(SqlGeometry geometry)
        {
            OpenGisGeometryType geometryType =
                (OpenGisGeometryType)Enum.Parse(typeof(OpenGisGeometryType), (string)geometry.STGeometryType());
            switch (geometryType)
            {
                case OpenGisGeometryType.Point:
                    return SqlGeometryToSharpMapPoint(geometry);
                case OpenGisGeometryType.LineString:
                    return SqlGeometryToSharpMapLineString(geometry);
                case OpenGisGeometryType.Polygon:
                    return SqlGeometryToSharpMapPolygon(geometry);
                case OpenGisGeometryType.MultiPoint:
                    return SqlGeometryToSharpMapMultiPoint(geometry);
                case OpenGisGeometryType.MultiLineString:
                    return SqlGeometryToSharpMapMultiLineString(geometry);
                case OpenGisGeometryType.MultiPolygon:
                    return SqlGeometryToSharpMapMultiPolygon(geometry);
                case OpenGisGeometryType.GeometryCollection:
                    return SqlGeometryToSharpMapGeometryCollection(geometry);
            }
            throw new ArgumentException(string.Format("Cannot convert SqlServer '{0}' to Sharpmap.Geometry", geometry.STGeometryType()), "geometry");
        }

        public static SMGeometry AsSharpMapGeometry(SqlGeometry geometry)
        {
            return ToSharpMapGeometry(geometry);
        }

        public static SMGeometry AsSCEGeometryExtention(this SqlGeometry geometry)
        {
            return ToSharpMapGeometry(geometry);
        }

        private static SMGeometryCollection SqlGeometryToSharpMapGeometryCollection(SqlGeometry geometry)
        {
            SMGeometryCollection geometryCollection = new SMGeometryCollection();
            for (int i = 0; i < geometry.STNumGeometries(); i++)
                geometryCollection.Collection.Add(ToSharpMapGeometry(geometry.STGeometryN(i)));
            return geometryCollection;
        }

        private static SMMultiPolygon SqlGeometryToSharpMapMultiPolygon(SqlGeometry geometry)
        {
            SMMultiPolygon multiPolygon = new SMMultiPolygon();
            for (int i = 1; i <= geometry.STNumGeometries(); i++)
                multiPolygon.Polygons.Add((SMPolygon)SqlGeometryToSharpMapPolygon(geometry.STGeometryN(i)));
            return multiPolygon;
        }

        private static SMMultiLineString SqlGeometryToSharpMapMultiLineString(SqlGeometry geometry)
        {
            SMMultiLineString multiLineString = new SMMultiLineString();
            for (int i = 0; i < geometry.STNumGeometries(); i++)
                multiLineString.LineStrings.Add((SMLineString)SqlGeometryToSharpMapLineString(geometry.STGeometryN(i)));
            return multiLineString;
        }

        private static SMGeometry SqlGeometryToSharpMapMultiPoint(SqlGeometry geometry)
        {
            SMMultiPoint multiPoint = new SMMultiPoint();
            for (int i = 1; i <= geometry.STNumGeometries(); i++)
                multiPoint.Points.Add((SMPoint)SqlGeometryToSharpMapPoint(geometry.STGeometryN(i)));
            return multiPoint;
        }

        private static SMGeometry SqlGeometryToSharpMapPoint(SqlGeometry geometry)
        {
            double z = !geometry.Z.IsNull ? (double)geometry.Z : 0;
            return new SMPoint((double)geometry.STX, (double)geometry.STY, z);
        }

        private static IEnumerable<SMPoint> GetPointsEnumerable(SqlGeometry geometry)
        {
            foreach (SMPoint point in GetPoints(geometry))
                yield return point;
        }

        private static IList<SMPoint> GetPoints(SqlGeometry geometry)
        {
            int pointsNum = (int)geometry.STNumPoints();
            SMPoint[] pts = new SMPoint[pointsNum];

            for (int i = 1; i <= pointsNum; i++)
            {
                SqlGeometry ptGeometry = geometry.STPointN(i);
                double z = ptGeometry.Z == SqlDouble.Null ? (double)ptGeometry.Z : 0;
                pts[i - 1] = new SMPoint((double)ptGeometry.STX, (double)ptGeometry.STY, z);
            }
            return pts;
        }

        private static SMGeometry SqlGeometryToSharpMapLineString(SqlGeometry geometry)
        {
            return new SMLineString(GetPoints(geometry));
        }

        private static SMGeometry SqlGeometryToSharpMapPolygon(SqlGeometry geometry)
        {
            //exterior ring
            SMLinearRing exterior = new SMLinearRing(GetPoints(geometry.STExteriorRing()));
            SMLinearRing[] interior = null;
            if (geometry.STNumInteriorRing() > 0)
            {
                interior = new SMLinearRing[(int)geometry.STNumInteriorRing()];
                for (int i = 1; i <= geometry.STNumInteriorRing(); i++)
                    interior[i - 1] = new SMLinearRing(GetPoints(geometry.STInteriorRingN(i)));
            }
            return new SMPolygon(exterior, interior);
        }
    }
}
