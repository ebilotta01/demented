using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemList : MonoBehaviour
{
    [SerializeField]
    public List<GameObject> itemSprites;
    [SerializeField]
    static int indexDistance = 10;
    public bool[] added;
    private int numItems;

    void Awake() {
        //float barsize = ItemBar.rect.width;
        // Debug.Log("child count " + transform.childCount);
        for(int i = 0; i < transform.childCount; i++) {
            itemSprites.Add(transform.GetChild(i).gameObject);
            //itemSprites.Insert(i-1, transform.GetChild(i).gameObject);
            // Debug.Log(itemSprites[i]);
        }
        this.added = new bool[Player.itemList.Length];
        for(int i = 0; i < this.added.Length; i++) {
            Debug.Log(this.added[i]);
        }
        numItems = 0;
    }

    public void AddItemToList(int item, int q) {
        Debug.Log("added item: " + item);
        //Debug.Log(itemSprites[item]);
        for(int i = 0; i < q; i++){
            if(this.added[item]){
                GameObject incremented = GameObject.Find(itemSprites[item].name);
                incremented.GetComponentInChildren<Text>().text = getItemCount(item);
                return;
            }
            else if(!this.added[item]) {
                Debug.Log("triggered not added");
                GameObject add = GameObject.Find(itemSprites[item].name);
                add.GetComponent<RectTransform>().localPosition = NormalizeXDistance();
                numItems++;
                add.GetComponentInChildren<Text>().text = getItemCount(item);
                this.added[item] = true;
                return;
            }
        }
        
    }
    Vector3 NormalizeXDistance() {
        int x = -(indexDistance + (100 * numItems));
       Debug.Log(x);
        return new Vector3(x, 0, 0);
    }

    string getItemCount(int itemId) {
        return Player.itemList[itemId].ToString();
    }



    string ItemToText(int id) {
        string[] names = {
            "Health Pill",
            "Hermes Boots",
            "Da-Da-Dagger"
        };
        return names[id] + ": " + Player.itemList[id] + "\n";
    }
    public string BuildStringDisplay() {
        string listExpression = "";
        for(int i = 0; i < Player.itemList.Length; i++) {
            if(Player.itemList[i] != 0) {
                listExpression += ItemToText(i);
            }
        }
        return listExpression;
    }


    
}
