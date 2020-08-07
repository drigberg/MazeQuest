using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PhysicalUIBlockInputController : MonoBehaviour
{
    // text
    public InputField inputField;
    private Canvas canvas;

    // movement
    public float frequency = 3.0f;
    public float maxRotation = 2.0f;

    private float localTime = 0.01f;
    private float targetXRotation = 0.0f;
    private float targetZRotation = 0.0f;

    // Start is called before the first frame update
    void Awake() {
        canvas = transform.GetChild(0).GetComponent<Canvas>();
        inputField = canvas.transform.GetChild(0).GetComponent<InputField>();
    }

    void Update() {
        localTime += Time.deltaTime * frequency;
        targetXRotation = Mathf.Sin(localTime) * maxRotation;
        targetZRotation = Mathf.Cos(localTime + Mathf.PI) * maxRotation;
        SetActive();
    }

    void FixedUpdate() {
        transform.eulerAngles = new Vector3(targetXRotation, 0.0f, targetZRotation);
    }

    public void SetActive() {
        inputField.Select();
        inputField.ActivateInputField();
    }
}
