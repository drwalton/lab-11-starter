using UnityEngine;

public class CollectibleItem : MonoBehaviour
{
    public int scoreValue = 10;

    public virtual void OnCollect()
    {
        // This function just destroys the item right away.
        // It's been added so that in the future we can add collectible items that 
        // do different things when collected. We can do this by inheriting from CollectibleItem
        // and overriding this function.
        Destroy(this.gameObject);
    }
}
