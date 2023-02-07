using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossHealthBar : MonoBehaviour
{
 
    public Slider slider;
    public Text displayText;
    [SerializeField]
    void Start() {
        SetHealth(); 
    }
    public void SetHealth(){
        slider.value = Boss.health;
        slider.maxValue = Boss.MaxHealth;
        displayText.text = ((int)Boss.health).ToString() + "/" + ((int)Boss.MaxHealth).ToString();
    }

}