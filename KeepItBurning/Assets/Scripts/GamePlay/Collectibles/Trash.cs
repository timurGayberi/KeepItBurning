using General;

namespace GamePlay.Collectibles
{
    public class Trash : CollectibleBase
    {
        protected override void OnCollected() // <== players inventory. 
        {
            //inventory.SetHasTrash(true);
        }
    }
}