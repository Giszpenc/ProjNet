using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace C4I.Applications.SCE.Interfaces
{
    public interface ISpatialEntitiesFactory<T>
    {
        T ConvertToProviderSapatialType(ISpatialEntity entity);
        ISpatialEntity ConvertToSystemSpatialType(T entity);
        string CovnertToWKT(ISpatialEntity entity);
        ISpatialEntity ConvertFromWKT(string wkt,int srid);
        bool FixInvalidWKT(string wkt);
    }
}
