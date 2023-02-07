using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthBar : MonoBehaviour
{
 
    public Slider slider;
    public Text displayText;

    public void SetMaxHealth() {
        slider.maxValue = Player.maxHealth;
        displayText.text = "    " + Player.health.ToString() + "/" + Player.maxHealth.ToString();
    }
    public void SetHealth(){
        slider.value = Player.health;
        displayText.text = "    " + Player.health.ToString() + "/" + Player.maxHealth.ToString();
    }

}
