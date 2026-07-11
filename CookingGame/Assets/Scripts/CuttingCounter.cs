using UnityEngine;

public class CuttingCounter : BaseCounter 
{

    [SerializeField] private KitchenObjectsSO cutKitchenObjectSO;
 
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
            if (player.HasKitchenObject())
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

    public override void InteractAlternate(Player player)
    {
        if (HasKitchenObject())
        {
            // There is a kitchen object here
            GetKitchenObject().DestroySelf();

            KitchenObject.SpawnKitchenObject(cutKitchenObjectSO, this);
        }

    }

}
