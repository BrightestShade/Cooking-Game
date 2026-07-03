using UnityEngine;

public class KitchenObject : MonoBehaviour
{
    [SerializeField] KitchenObjectsSO kitchenObjectSO;

    private ClearCounter clearCounter;

    public KitchenObjectsSO GetKitchenObjectsSO()
    {
        return kitchenObjectSO;
    }

    public void SetClearCounter(ClearCounter clearCounter)
    {   if (this.clearCounter != null) //
        {//
            this.clearCounter.ClearKitchenObject();// This piece clears kitchen object from old clearCounter
        }//


        this.clearCounter = clearCounter;

        if (clearCounter.HasKitchenObject()) // ensures we know if there is an error where the object is moved to a counter that already has a kitchenObject
        {
            Debug.LogError("Counter already has a kitchen object");
        }

        clearCounter.SetKitchenObject(this);// adds the kitchen to the new clear counter

        transform.parent = clearCounter.GetKitchenObjectFollowTransform();// this piece 
        transform.localPosition = Vector3.zero; // updates the visual
    }

 
    
    
    public ClearCounter GetClearCounter()
    {
        return clearCounter;
    }

    

}
