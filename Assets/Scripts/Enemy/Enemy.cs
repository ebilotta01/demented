using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Enemy {
    public static float health = 100.0f;    
    public static float damage = 20.0f;
    //public static float minSpeed = 2.0f;
    public static float minSpeed = Player.playerSpeed * 0.8f;
    public static float maxSpeed = Player.playerSpeed * 1.5f;
    public static float knockback = 50.0f;


    public static void Clear(){
        health = 100.0f;
        damage = 20.0f;
    }

}
