using UnityEngine;
using System;
using Unity.Netcode;

public class CuttingCounter : BaseCounter, IHasProgress
{

    public static EventHandler OnAnyCut;

    new public static void ResetStaticData()
    {
        OnAnyCut = null;
    }

    public event EventHandler<IHasProgress.OnProgressChangedEventArgs> OnProgressChanged;

    public event EventHandler OnCut;

    [SerializeField] private CuttingRecipeSO[] cuttingRecipeSOArray;
    private int cuttingProgress;


   

  

    public override void Interact(Player player)
    {
        if (!HasKitchenObject())
        {
            // there is no kitchen object here
            if (player.HasKitchenObject())
            {
                //player is carrying something
                if (HasRecipeWithInput(player.GetKitchenObject().GetKitchenObjectsSO()))
                {
                    // player is carrying something that can be cut
                    KitchenObject kitchenObject = player.GetKitchenObject();
                    kitchenObject.SetKitchenObjectParent(this);


                    InteractLogicPlacedOnCounterServerRpc();
                }
                
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
                if (player.GetKitchenObject().TryGetPlate(out PlateKitchenObject plateKitchenObject))
                {
                    // player is holding a plate

                    if (plateKitchenObject.TryAddIngredient(GetKitchenObject().GetKitchenObjectsSO()))
                    {
                       KitchenObject.DestroyKitchenObject(GetKitchenObject());
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


    [ServerRpc(RequireOwnership = false)]
    private void InteractLogicPlacedOnCounterServerRpc()
    {
        InteractLogicPlacedOnCounterClientRpc();
    }

    [ClientRpc]
    private void InteractLogicPlacedOnCounterClientRpc()
    {
        cuttingProgress = 0;


        OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs
        {
            progressNormalized = 0f
        });
    }

    public override void InteractAlternate(Player player)
    {
        if (HasKitchenObject() && HasRecipeWithInput(GetKitchenObject().GetKitchenObjectsSO()))
        {
            // There is a kitchen object here & it can be cut
            CutObjectServerRpc();
            TestCuttingProgressDoneServerRpc();

        }

    }

    [ServerRpc(RequireOwnership = false)]
    private void CutObjectServerRpc()
    {
        CutObjectClientRpc();
    }

    [ClientRpc]
    private void CutObjectClientRpc()
    {
        cuttingProgress++;

        OnCut?.Invoke(this, EventArgs.Empty);
        // Debug.Log(OnAnyCut.GetInvocationList().Length);
        OnAnyCut?.Invoke(this, EventArgs.Empty);

        CuttingRecipeSO cuttingRecipeSO = GetCuttingRecipeSOWithInput(GetKitchenObject().GetKitchenObjectsSO());

        OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs
        {
            progressNormalized = (float)cuttingProgress / cuttingRecipeSO.cuttingProgressMax
        });

        
    }


    [ServerRpc(RequireOwnership = false)]
    private void TestCuttingProgressDoneServerRpc()
    {
        CuttingRecipeSO cuttingRecipeSO = GetCuttingRecipeSOWithInput(GetKitchenObject().GetKitchenObjectsSO());

        if (cuttingProgress >= cuttingRecipeSO.cuttingProgressMax)
        {
            KitchenObjectsSO outputKitchenObjectSO = GetOutputForInput(GetKitchenObject().GetKitchenObjectsSO());

            KitchenObject.DestroyKitchenObject(GetKitchenObject());


            KitchenObject.SpawnKitchenObject(outputKitchenObjectSO, this);
        }
    }

    private bool HasRecipeWithInput(KitchenObjectsSO inputKitchenObjectSO)
    {
        CuttingRecipeSO cuttingRecipeSO = GetCuttingRecipeSOWithInput(inputKitchenObjectSO);
        return cuttingRecipeSO != null;
        
    }

    private KitchenObjectsSO GetOutputForInput(KitchenObjectsSO inputKitchenObjectSO)
    {
        CuttingRecipeSO cuttingRecipeSO = GetCuttingRecipeSOWithInput(inputKitchenObjectSO);
        if(cuttingRecipeSO != null)
        {
            return cuttingRecipeSO.output;
        }
        else
        {
            return null;
        }

    }

    private CuttingRecipeSO GetCuttingRecipeSOWithInput(KitchenObjectsSO inputKitchenObjectSO)
    {
        foreach (CuttingRecipeSO cuttingRecipeSO in cuttingRecipeSOArray)
        {
            if (cuttingRecipeSO.input == inputKitchenObjectSO)
            {
                return cuttingRecipeSO;
            }
        }
        return null;
    }

}
