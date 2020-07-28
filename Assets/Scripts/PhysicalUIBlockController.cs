using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PhysicalUIBlockController : MonoBehaviour
{
    // text
    public delegate void Action(); 
    public Action action = DoNothing;
    private Canvas canvas;
    private TMPro.TextMeshProUGUI textMesh;
 
    // movement
    public float frequency = 6.0f;
    public float maxRotation = 2.0f;

    private float localTime = 0.01f;
    private float targetXRotation = 0.0f;
    private float targetZRotation = 0.0f;

    // Start is called before the first frame update
    void Start() {}

    void Update() {
        localTime += Time.deltaTime * frequency;
        targetXRotation = Mathf.Sin(localTime) * maxRotation;
        targetZRotation = Mathf.Cos(localTime + Mathf.PI) * maxRotation;
    }

    void FixedUpdate() {
        transform.eulerAngles = new Vector3(targetXRotation, 0.0f, targetZRotation);
    }

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
