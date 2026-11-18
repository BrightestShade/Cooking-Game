using UnityEngine;
using System;
using System.Collections.Generic;

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
                if(player.GetKitchenObject().TryGetPlate(out PlateKitchenObject plateKitchenObject))
                {
                    // player is holding a plate
                    
                    if (plateKitchenObject.TryAddIngredient(GetKitchenObject().GetKitchenObjectsSO()))
                    {
                        KitchenObject.DestroyKitchenObject(GetKitchenObject());
                    }
                   
                }
                else
                {
                    // player is not carrying a plate but has a different kitchen object
                    if (GetKitchenObject().TryGetPlate(out plateKitchenObject))
                    {
                        // counter is holding a plate
                        if (plateKitchenObject.TryAddIngredient(player.GetKitchenObject().GetKitchenObjectsSO()))
                        {
                            KitchenObject.DestroyKitchenObject(player.GetKitchenObject());
                        }
                    }
                }
            }
            else
            {
                // player has nothing in hands
                GetKitchenObject().SetKitchenObjectParent(player);
            }
        }

    }

}
