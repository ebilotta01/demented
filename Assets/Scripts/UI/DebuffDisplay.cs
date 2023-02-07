using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebuffDisplay : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        for(int i = 0; i < 5; i++) {
            Hide(i);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Hide(int index) {
        Debug.Log("Hiding " + this.transform.GetChild(index).name);
        this.transform.GetChild(index).GetComponent<CanvasGroup>().alpha = 0f; //this makes everything transparent
        this.transform.GetChild(index).GetComponent<CanvasGroup>().blocksRaycasts = false; //this prevents the UI element to receive input events
    }
    public void Show(int index) {
        Debug.Log("Showing " + this.transform.GetChild(index).name);
        this.transform.GetChild(index).GetComponent<CanvasGroup>().alpha = 1f; //this makes everything transparent
        this.transform.GetChild(index).GetComponent<CanvasGroup>().blocksRaycasts = true; //this prevents the UI element to receive input events
    }
}
