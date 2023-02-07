using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Chest : MonoBehaviour
{
    private PlayerController player;
    [SerializeField]
    public GameObject[] itemPrefabs;
    Vector3 chestTransform;
    // Start is called before the first frame update
    public int chestHealth = 3;
    public HealthBar healthBar;
    void Start()
    {
        chestTransform = this.gameObject.transform.position;
        chestTransform.y += 1;
        //player = GameObject.Find("Player").GetComponent<PlayerController>();
    }
    
    private void Update() {
        if(chestHealth <= 0) {
            spawnItem();
            Destroy(this.gameObject);
        }
    }

    public void spawnItem() {
        int item = Random.Range(0, Player.itemList.Length);
        GameObject spawned;
        spawned = GameObject.Instantiate(itemPrefabs[item], chestTransform, Quaternion.identity);
    }

    void OnTriggerEnter(Collider collider){
        if(collider.gameObject.tag == "Bullet") {
            chestHealth--;
            healthBar.SetHealth(chestHealth);
        }
    }
}
