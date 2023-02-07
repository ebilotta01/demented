using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelNo : MonoBehaviour
{

    public Text displayText;
    void Start()
    {
        displayText.text = Level.LevelNumber.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
