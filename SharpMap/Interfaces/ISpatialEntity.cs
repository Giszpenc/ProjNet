using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using C4I.Applications.SCE.Interfaces;

namespace C4I.Applications.SCE.Interfaces
{

    public interface ISpatialEntity
    {
        [Obsolete("Use base class methods")]
        IEnumerator<Point> PointCollection
        {
            get;
        }

        [Obsolete("Use base class methods")]
        void AddPoint(Point p);

        [Obsolete("Use base class methods")]
        void RemovePoint(Point p);

        [Obsolete("Use base class methods")]
        void SetPoint(int index, Point newPoint);

        [DataMember]
        string OGCType
        {
            get;
        }

        [DataMember]
        int SRID
        {
            get;
            set;
        }


    }


}


