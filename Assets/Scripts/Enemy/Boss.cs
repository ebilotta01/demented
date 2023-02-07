using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Boss {
    public static float health;    
    public static float MaxHealth = 200.0f;
    public static float damage = 25.0f;
    // speed parameters
    public static float minSpeed = Player.playerSpeed * 0.8f;
    public static float maxSpeed = Player.playerSpeed * 1.5f;
    public static float b = 0.25f * (5.0f * maxSpeed - minSpeed);
    public static float a = 0.25f * (maxSpeed - minSpeed);
    

    public static void Clear(){
        MaxHealth = 200.0f;
        damage = 25.0f;
    }
}

