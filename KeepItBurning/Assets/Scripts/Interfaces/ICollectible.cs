using Player;
using UnityEngine;
using General;

namespace Interfaces
{
    public interface ICollectible
    {
        //public interface GetCollectibleData();
        
        void Collect(GameObject interactor);
        
        void Drop(GameObject interactor);
    }
}