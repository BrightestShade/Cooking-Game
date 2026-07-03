using UnityEngine;

public class KitchenObject : MonoBehaviour
{
    [SerializeField] KitchenObjectsSO kitchenObjectSO;

    public KitchenObjectsSO GetKitchenObjectsSO()
    {
        return kitchenObjectSO;
    }
}
