using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Flicker: MonoBehaviour
{
    int wave;
    public int threshold;

    public Light l1;

    private void Start() {
        wave = 0;

    }

    void Update() {
;
        wave+= Random.Range(-10, 10);

        if(wave > 100 || wave < -100){
            wave = 0;
        }

        if(wave > threshold){
            l1.enabled = false;
        } 

        if (l1.enabled == false && wave < threshold){
            l1.enabled = true;
        }
        
    }


    public void playGame(){
        SceneManager.LoadScene("SampleScene");
    }





}

