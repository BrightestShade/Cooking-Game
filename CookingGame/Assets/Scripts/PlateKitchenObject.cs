using UnityEngine;
using System.Collections.Generic;
using System;
using Unity.Netcode;

public class PlateKitchenObject : KitchenObject
{
    public event EventHandler<OnIngredientAddedEventArgs> OnIngredientAdded;

    public class OnIngredientAddedEventArgs : EventArgs
    {
        public KitchenObjectsSO kitchenObjectSO;
    }

    [SerializeField] private List<KitchenObjectsSO> validKitchenObjectSOList;

    private List<KitchenObjectsSO> kitchenObjectSOList;

    protected override void Awake()
    {
        base.Awake();
        kitchenObjectSOList = new List<KitchenObjectsSO>();
    }

    public bool TryAddIngredient(KitchenObjectsSO kitchenObjectSO)
    {
        if (!validKitchenObjectSOList.Contains(kitchenObjectSO))
        {
            // Not a valid ingredient
            return false;
        }

        if (kitchenObjectSOList.Contains(kitchenObjectSO))
        {
            // Already has this ingredient
            return false;
        }
        else
        {
          
            AddIngredientServerRpc(
                KitchenGameMultiplayer.Instance.GetKitchenObjectSOIndex(kitchenObjectSO)
                );

            return true;
        }

           
    }

    [ServerRpc(RequireOwnership = false)]
    private void AddIngredientServerRpc(int kitchenObjectSOIndex)
    {
        AddIngredientClientRpc(kitchenObjectSOIndex);
    }

    [ClientRpc]
    private void AddIngredientClientRpc(int kitchenObjectSOIndex)
    {
        KitchenObjectsSO kitchenObjectSO = KitchenGameMultiplayer.Instance.GetKitchenObjectSOFromIndex(kitchenObjectSOIndex);
        kitchenObjectSOList.Add(kitchenObjectSO);

        OnIngredientAdded?.Invoke(this, new OnIngredientAddedEventArgs
        {
            kitchenObjectSO = kitchenObjectSO
        });
    }

    public List<KitchenObjectsSO> GetKitchenObjectSOList()
    {
        return kitchenObjectSOList;
    }
}