using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace C4I.Applications.SCE.Interfaces
{
    public interface ISpatialComputation
    {
        List<int> Intersect(string intersectingLayerName,int featureId,string intersectedLayer);

        List<int> Intersect(string intersectingLayerName,string intersectedLayerName);

        List<int> Intersect(ISpatialEntity geometry,string intersectedLayerName);

        List<int> WithinDistance(string intersectingLayerName,int featureId,string withinDistanceLayer);

        List<int> WithinDistance(string intersectingLayerName,string intersectedLayerName);

        List<int> WithinDistance(ISpatialEntity geometry,string intersectedLayerName);

        List<int> FormationCameras(string layerName,Point point,float radius,int amount);

        List<int> GetActiveCamerasCoveringLocation(Point point, float radius, int amount, string floorDescription = "");

        bool Intersect(ISpatialEntity entityA,ISpatialEntity entityB);
    }
}
