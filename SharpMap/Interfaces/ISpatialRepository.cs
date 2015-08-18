using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace C4I.Applications.SCE.Interfaces
{
    public interface ISpatialRepository
    {
        void AddLayer(string layerName, string tablePath);

        void RemoveLayer(string layerName);

        DataTableReader GetLayer(string layerName);

        DataTable InspectLayer(string tablePath);
    }
}
