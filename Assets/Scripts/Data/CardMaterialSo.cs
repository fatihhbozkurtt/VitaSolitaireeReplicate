using System.Collections.Generic;
using UnityEngine;

namespace Data
{
    [CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/CardMaterialSO", order = 1)]
    public class CardMaterialSo : ScriptableObject
    {
        public List<MaterialData> materialData;
    }

    [System.Serializable]
    public class MaterialData
    {
        public CardSymbol symbol;
        public List<Material> materials;
    }
}