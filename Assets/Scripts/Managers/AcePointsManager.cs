using System.Collections.Generic;
using Data;
using UnityEngine;

namespace Managers
{
    public class AcePointsManager : MonoSingleton<AcePointsManager>
    {
        [SerializeField] private List<AcePointController> points;

        public AcePointController GetAvailablePointBySymbol(CardSymbol targetSymbol)
        {
            foreach (var point in points)
            {
                if (point.symbol == targetSymbol)
                {
                    return point;
                }
            }

            return null;
        }
    }
}