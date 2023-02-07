using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour {
    [SerializeField]
    public GameObject crosshair;
    public bool isPaused;
    InputAction pauseAction;
    private PlayerInput playerUIInput;
    [SerializeField]
    public CanvasGroup canvasGroup;
    // Start is called before the first frame update
    void Start()
    { 
        isPaused = false;
        playerUIInput = GameObject.Find("PLAYER").GetComponent<PlayerInput>();
        pauseAction = playerUIInput.actions["Pause"];
        Hide();
    }

    // Update is called once per frame
    void Update()
    {
        if(pauseAction.triggered && !isPaused) {
            Pause();
            return;
        }
        if(isPaused && pauseAction.triggered) {
            UnPause();
        }
    }
    public void Pause() {
        Time.timeScale = 0;
        Cursor.visible = true;
        crosshair.SetActive(false); 
        isPaused = true;
        Show();
    }
    public void UnPause() {
        Time.timeScale = 1;
        Cursor.visible = false;
        crosshair.SetActive(true);
        isPaused = false;
        Hide();
    }

    void Hide() {
        canvasGroup.alpha = 0f; //this makes everything transparent
        canvasGroup.blocksRaycasts = false; //this prevents the UI element to receive input events
    }
    void Show() {
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;
    } 

    public void QuitToMain() {
        Player.Clear();
        Level.Clear();
        SceneManager.LoadScene("MainMenu");
    }
}
