using UnityEngine;
using UnityEngine.UI;

public class PlateIconsSingleUI : MonoBehaviour
{

    [SerializeField] private Image image;


    public void SetKitchenObjectsSO(KitchenObjectsSO kitchenObjectsSO)
    {
        image.sprite = kitchenObjectsSO.sprite;
    }
}
