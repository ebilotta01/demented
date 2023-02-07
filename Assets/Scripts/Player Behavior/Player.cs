using System.Collections;
using System.Collections.Generic;
using UnityEngine;

static public class Player
{

    public static int[] itemList = {0,0,0,0,0};
    public static bool isColliding = false;
    [SerializeField]
    static public float damage = 7.0f;
    [SerializeField]
    static public float health = 100.0f; 
    static public float maxHealth = 100.0f;
    [SerializeField]
    static public float playerSpeed = 10.0f;
    static public int enemiesKilled = 0;

    static public float shotCooldown = 2f;
    static public int regen = 1;


    static public bool inBossRoom = false;

    // public static ItemList itemDisplay;
    public static void Clear(){
        for(int i = 0; i < itemList.Length; i ++){
            itemList[i] = 0;
        }
        damage = 10.0f;
        health = 100.0f;
        maxHealth = 100.0f;
        playerSpeed = 10.0f;
        enemiesKilled = 0;
        shotCooldown = 2f;
        regen = 1;
        inBossRoom = false;
    }


    public static void toString(){
        Debug.Log("Health " + health + " maxHealth " + maxHealth + "Damage " + damage + " Speed " + playerSpeed + "Shot Cooldown " + shotCooldown + " enemiesKilled " + enemiesKilled);
        for(int i = 0; i < itemList.Length; i ++){
            Debug.Log(i + " " +itemList[i]);
        }
    }

   public static void UpdatePlayer(int item, int q) { 
        itemList[item] += q;
        Item.Modifier modifier = Item.getModifierByID(item);
        for(int i = 0; i < q; i ++){    
            // itemDisplay.AddItemToList(item); 
            switch(modifier) {
                case Item.Modifier.Health: 
                    maxHealth += 50.0f;
                    break;
                case Item.Modifier.Damage:
                    damage *= 1.1f;
                    break;
                case Item.Modifier.Speed:
                    playerSpeed += 5.0f;
                    break;
                case Item.Modifier.AttackSpeed:
                    shotCooldown -= Player.shotCooldown * 0.2f;
                    break;
                case Item.Modifier.HealthRegen:
                    regen *= 2;
                    break;
                default:
                    break;
            }
        }
    }

    public static float GetPlayerAttribute(Item.Modifier modifier) {          
        switch(modifier) {
            case Item.Modifier.Health: return maxHealth;
            case Item.Modifier.Damage: return damage;
            case Item.Modifier.Speed: return playerSpeed;
            case Item.Modifier.AttackSpeed: return shotCooldown;
            case Item.Modifier.HealthRegen: return (float)regen;
            default: return -1;
        }
    }
    public static void SetPlayerAttribute(Item.Modifier debuff, float originalValue) {  
        //Debug.Log(debuff + " / " + originalValue);        
        switch(debuff) {
            case Item.Modifier.Health: Player.maxHealth = originalValue;
            break;
            case Item.Modifier.Damage: Player.damage = originalValue;
            break;
            case Item.Modifier.Speed: Player.playerSpeed = originalValue;
            break;
            case Item.Modifier.AttackSpeed: Player.shotCooldown = originalValue;
            break;
            case Item.Modifier.HealthRegen: Player.regen = (int)originalValue;
            break;
            default:
                break;
        }
    }
    
    public static void DebuffPlayer(Item.Modifier modifier) { 
        //Item.Modifier modifier = Item.getModifierByID(item);    
        switch(modifier) {
            case Item.Modifier.Health: 
                maxHealth -= 50.0f;
                //reverse health regen coroutine
                break;
            case Item.Modifier.Damage:
                damage = damage * 0.75f;
                break;
            case Item.Modifier.Speed:
                playerSpeed = playerSpeed * 0.6f;
                break;
            case Item.Modifier.AttackSpeed:
                shotCooldown = shotCooldown * 1.2f;
                break;
            case Item.Modifier.HealthRegen:
                regen = 0;
                break;
            default:
                break;
        }
    }
}
