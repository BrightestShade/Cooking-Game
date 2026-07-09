using UnityEngine;

public class ClearCounter : BaseCounter
{

    [SerializeField] private KitchenObjectsSO kitchenObjectSO;

   

    public override void Interact(Player player)
    {
       if (!HasKitchenObject())
       {
            // there is no kitchen object here
            if (player.HasKitchenObject())
            {
                // player is carrying something
                player.GetKitchenObject().SetKitchenObjectParent(this);
            }
            else
            {
                // player has nothing in hands 
                
            }
       }
       else
        {
            // is a kitchen object here
            if(player.HasKitchenObject())
            {
                // player is carrying something

            }
            else
            {
                // player has nothing in hands
                GetKitchenObject().SetKitchenObjectParent(player);
            }
        }

    }

}
