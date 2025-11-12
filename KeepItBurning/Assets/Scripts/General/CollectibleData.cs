using System;
using UnityEngine;

namespace General
{
    [Serializable]
    public struct CollectibleData
    {
        [Tooltip("A unique identifier for the item (e.g., 'FireWoodLogs').")]
        public int ID;
        
        [Tooltip("The amount of fuel or resource value the item provides.")]
        public float FuelValue;
        
        public CollectibleData(int id, float fuelValue)
        {
            ID = id;
            FuelValue = fuelValue;
        }
    }
}