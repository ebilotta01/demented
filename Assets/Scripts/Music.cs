using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Music : MonoBehaviour
{

    [SerializeField]
    public AudioSource source;
    [SerializeField]
    public Menu pauseMenu;
    // Start is called before the first frame update
    void Start()
    {
        source.Play();
    }

    // Update is called once per frame
    void Update()
    {
        // if(pauseMenu.isPaused) {
        //     Debug.Log("entered");
        //     source.Pause();
        //     if(!pauseMenu.isPaused) {
        //         source.UnPause();
        //     }
        // }
    }
}
