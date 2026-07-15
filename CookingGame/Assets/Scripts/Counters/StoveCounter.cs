using UnityEngine;

public class StoveCounter : BaseCounter
{

    private enum State
    {
        Idle,
        Frying,
        Fried,
        Burned,
    }

    [SerializeField] private FryingRecipeSO[] fryingRecipeSOArray;

    private State state;
    private float fryingTimer;

    private FryingRecipeSO fryingRecipeSO;

    private void Start()
    {
        state = State.Idle; 
    }

    private void Update()
    {

        if (HasKitchenObject())
        {
            switch (state)
            {
                case State.Idle:

                    break;

                case State.Frying:
                    fryingTimer += Time.deltaTime;


                    if (fryingTimer > fryingRecipeSO.fryingTimerMax)
                    {

                        GetKitchenObject().DestroySelf();

                        KitchenObject.SpawnKitchenObject(fryingRecipeSO.output, this);

                        state = State.Fried;
                    }
                    break;

                case State.Fried:

                    break;
                case State.Burned:

                    break;
            }

            Debug.Log(state);
        }
       



    }

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
                    player.GetKitchenObject().SetKitchenObjectParent(this);


                    fryingRecipeSO = GetFryingRecipeSOWithInput(GetKitchenObject().GetKitchenObjectsSO());

                    state = State.Frying;
                    fryingTimer = 0f; 
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

            }
            else
            {
                // player has nothing in hands
                GetKitchenObject().SetKitchenObjectParent(player);
            }
        }
    }

      private bool HasRecipeWithInput(KitchenObjectsSO inputKitchenObjectSO)
    {
        FryingRecipeSO fryingRecipeSO = GetFryingRecipeSOWithInput(inputKitchenObjectSO);
        return fryingRecipeSO != null;
        
    }

    private KitchenObjectsSO GetOutputForInput(KitchenObjectsSO inputKitchenObjectSO)
    {
        FryingRecipeSO fryingRecipeSO = GetFryingRecipeSOWithInput(inputKitchenObjectSO);
        if(fryingRecipeSO != null)
        {
            return fryingRecipeSO.output;
        }
        else
        {
            return null;
        }

    }

    private FryingRecipeSO GetFryingRecipeSOWithInput(KitchenObjectsSO inputKitchenObjectSO)
    {
        foreach (FryingRecipeSO fryingRecipeSO in fryingRecipeSOArray)
        {
            if (fryingRecipeSO.input == inputKitchenObjectSO)
            {
                return fryingRecipeSO;
            }
        }
        return null;
    }

}       
