using System;
using UnityEngine;

namespace General
{
    [Serializable]
    public struct CollectibleData
    {
        [Tooltip("A unique identifier for the item (e.g., 'FireWoodLogs').")]
        public string ID;
        
        [Tooltip("The amount of fuel or resource value the item provides.")]
        public float FuelValue;
        
        public CollectibleData(string id, float fuelValue)
        {
            ID = id;
            FuelValue = fuelValue;
        }
    }
}