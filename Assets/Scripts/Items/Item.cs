using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// public class Item : MonoBehaviour
// {
//     [SerializeField]
//     public int itemId;
//     [SerializeField]
//     public enum Modifier {
//         AttackSpeed,
//         HealthRegen,
//         Health,
//         Damage,
//         Speed
//     } 
//     public string itemName;

//     public Modifier modifier;
//     public bool addedToUI = false;


//     public void GetItemDetails() {
//         Debug.Log(this.itemId);
//         Debug.Log(this.itemName);
//         Debug.Log(this.modifier);
//     }
// }

public static class Item
{
    public enum Modifier {
        AttackSpeed,
        Health,
        Damage,
        Speed,
        HealthRegen

    }

    public static Modifier getModifierByID(int id){
        return (Modifier) id;
    }
    
}
