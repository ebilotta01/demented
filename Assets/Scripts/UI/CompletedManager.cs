using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class CompletedManager : MonoBehaviour
{
    // Start is called before the first frame update

    public UnityEngine.UI.Text completed;
    public UnityEngine.UI.Text ekilled;

    void Start()
    {
        Level.SetNextLevel();
        LoadGame.saveGame();
        string comp = "Level " + (Level.LevelNumber-1).ToString() + " Completed.";
        string ek = "Enemies Murdered: " + Player.enemiesKilled.ToString();
        
        completed.text = comp;
        ekilled.text = ek;
    }


    public void continute(){
        SceneManager.LoadScene("SampleScene");
    }

}
