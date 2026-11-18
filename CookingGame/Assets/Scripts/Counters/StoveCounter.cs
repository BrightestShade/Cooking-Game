using System;
using UnityEngine;
using Unity.Netcode;


public class StoveCounter : BaseCounter, IHasProgress
{
    public event EventHandler<OnStateChangedEventArgs> OnStateChanged;
    public event EventHandler<IHasProgress.OnProgressChangedEventArgs> OnProgressChanged;

    public class OnStateChangedEventArgs : EventArgs
    {
        public State state;
    }

    public enum State
    {
        Idle,
        Frying,
        Fried,
        Burned,
    }

    [SerializeField] private FryingRecipeSO[] fryingRecipeSOArray;
    [SerializeField] private BurningRecipeSO[] burningRecipeSOArray;

    private NetworkVariable<State> state = new NetworkVariable<State>(State.Idle);

    private NetworkVariable<float> fryingTimer = new NetworkVariable<float>(0f);
    private NetworkVariable<float> burningTimer = new NetworkVariable<float>(0f);

    private FryingRecipeSO fryingRecipeSO;
    private BurningRecipeSO burningRecipeSO;

  

    public override void OnNetworkSpawn()
    {
        fryingTimer.OnValueChanged += FryingTimer_OnValueChanged;
        burningTimer.OnValueChanged += BurningTimer_OnValueChanged;
        state.OnValueChanged += State_OnValueChanged;
    }



    private void FryingTimer_OnValueChanged(float previousValue, float newValue)
    {
        float fryingTimerMax = fryingRecipeSO != null ? fryingRecipeSO.fryingTimerMax : 1f;
        OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs
        {
            progressNormalized = fryingTimer.Value / fryingTimerMax
        });
    }

    private void BurningTimer_OnValueChanged(float previousValue, float newValue)
    {
        float burningTimerMax = burningRecipeSO != null ? burningRecipeSO.burningTimerMax : 1f;
        OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs
        {
            progressNormalized = burningTimer.Value / burningTimerMax
        });
    }

    private void State_OnValueChanged(State previousState, State newState)
    {
        OnStateChanged?.Invoke(this, new OnStateChangedEventArgs
        {
            state = state.Value
        });

        if(state.Value == State.Burned || state.Value == State.Idle)
        {
            OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs
            {
                progressNormalized = 0f

            });
        }
    }

    private void Update()
    {
        if (!IsServer)
        {
            return;
        }

        if (HasKitchenObject())
        {
            switch (state.Value)
            {
                case State.Idle:

                    break;

                case State.Frying:
                    fryingTimer.Value += Time.deltaTime;

                    if (fryingTimer.Value > fryingRecipeSO.fryingTimerMax)
                    {
                        //Fried
                        KitchenObject.DestroyKitchenObject(GetKitchenObject());
                      

                        KitchenObject.SpawnKitchenObject(fryingRecipeSO.output, this);
                        
                        state.Value = State.Fried;
                        burningTimer.Value = 0f;
                        SetBurningRecipeSOClientRpc(
                            KitchenGameMultiplayer.Instance.GetKitchenObjectSOIndex(GetKitchenObject().GetKitchenObjectsSO())
                            );
                       

                    }
                    break;

                case State.Fried:
                    burningTimer.Value += Time.deltaTime;

                    OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs
                    {
                        progressNormalized = burningTimer.Value / burningRecipeSO.burningTimerMax
                    });

                    if (burningTimer.Value > burningRecipeSO.burningTimerMax)
                    {

                        KitchenObject.DestroyKitchenObject(GetKitchenObject());

                        KitchenObject.SpawnKitchenObject(burningRecipeSO.output, this);


                       
                        state.Value = State.Burned;

                    }
                    break;
                case State.Burned:
                    SetStateIdleServerRpc();
                    break;
            }

            
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
                    KitchenObject kitchenObject = player.GetKitchenObject();
                    kitchenObject.SetKitchenObjectParent(this);

                    InteractLogicPlaceOBjectOnCounterServerRpc(
                    KitchenGameMultiplayer.Instance.GetKitchenObjectSOIndex(kitchenObject.GetKitchenObjectsSO())
                    );

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

                        SetStateIdleServerRpc();

                    }

                }
            }
            else
            {
                // player has nothing in hands
                GetKitchenObject().SetKitchenObjectParent(player);

              

                SetStateIdleServerRpc();


            }

        }
    }
    [ServerRpc(RequireOwnership = false)]
    private void SetStateIdleServerRpc()
    {
        state.Value = State.Idle;
        fryingTimer.Value = 0f;
        burningTimer.Value = 0f;

        ResetProgressBarClientRpc();
    }

    [ClientRpc]
    private void ResetProgressBarClientRpc()
    {
        OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs
        {
            progressNormalized = 0f
        });
    }

    [ServerRpc(RequireOwnership = false)]
    private void InteractLogicPlaceOBjectOnCounterServerRpc(int kitchenObjectSOIndex)
    {
        fryingTimer.Value = 0f;
        state.Value = State.Frying;

        SetFryingRecipeSOClientRpc(kitchenObjectSOIndex);
       
    }

    [ClientRpc]
    private void SetFryingRecipeSOClientRpc(int kitchenObjectSOIndex)
    {
        KitchenObjectsSO kitchenObjectSO = KitchenGameMultiplayer.Instance.GetKitchenObjectSOFromIndex(kitchenObjectSOIndex);
        fryingRecipeSO = GetFryingRecipeSOWithInput(kitchenObjectSO);

    }

    [ClientRpc]
    private void SetBurningRecipeSOClientRpc(int kitchenObjectSOIndex)
    {
        KitchenObjectsSO kitchenObjectSO = KitchenGameMultiplayer.Instance.GetKitchenObjectSOFromIndex(kitchenObjectSOIndex);
        burningRecipeSO = GetBurningRecipeSOWithInput(kitchenObjectSO);

    }

    private bool HasRecipeWithInput(KitchenObjectsSO inputKitchenObjectSO)
      {
        FryingRecipeSO fryingRecipeSO = GetFryingRecipeSOWithInput(inputKitchenObjectSO);
        return fryingRecipeSO != null;
        
      }

    private KitchenObjectsSO GetOutputForInput(KitchenObjectsSO inputKitchenObjectSO) // this function might be uneeded not sure 
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

    private BurningRecipeSO GetBurningRecipeSOWithInput(KitchenObjectsSO inputKitchenObjectSO)
    {
        foreach (BurningRecipeSO burningRecipeSO in burningRecipeSOArray)
        {
            if (burningRecipeSO.input == inputKitchenObjectSO)
            {
                return burningRecipeSO;
            }
        }
        return null;
    }

    public bool IsFried()
    {
        return state.Value == State.Fried;
    }
}       
