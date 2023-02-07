using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour
{
    private float speed = 50f;
    private float timeToDestroy = 1f;

    public Vector3 target {get; set; }
    public bool hit {get; set; }

    void Update() {
        transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);
        if(hit) {
            // Debug.Log("HIT");
            Destroy(gameObject, timeToDestroy);
        }
    }
}
