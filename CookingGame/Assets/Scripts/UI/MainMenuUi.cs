using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class MainMenuUi : MonoBehaviour
{
   [SerializeField] private Button playButton;
   [SerializeField] private Button quitButton;

    private void Awake()
    {
        EventSystem.current.SetSelectedGameObject(playButton.gameObject);
        playButton.onClick.AddListener(() =>
        {
            Loader.Load(Loader.Scene.GameScene);
        });

        quitButton.onClick.AddListener(() =>
        {
            Application.Quit();
        });

        Time.timeScale = 1f;

    }


    

}
