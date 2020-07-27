using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PhysicalUIBlockController : MonoBehaviour
{
    public delegate void Action(); 
    public Action action = DoNothing;
    private Canvas canvas;
    private TMPro.TextMeshProUGUI textMesh;
 
    // Start is called before the first frame update
    void Start() {}

    static void DoNothing() {
        Debug.Log("No action assigned: doing nothing");
    }

    void OnMouseDown() {
        action();
    }

    public void SetText(string text) {
        canvas = transform.GetChild(0).GetComponent<Canvas>();
        textMesh = canvas.transform.GetChild(0).GetComponent<TMPro.TextMeshProUGUI>();
        textMesh.SetText(text);
    }

    public void SetAction(Action method) {
        action = method;
    }
}
