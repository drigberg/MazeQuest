using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetDetectorController : MonoBehaviour
{
    private bool triggered = false;
    public MazeGenerator parent;

    private void OnTriggerEnter(Collider other)
    {
        if (!triggered && other.GetComponent<PlayerController>() != null) {
            parent.OnSuccess();
        }
    }
}
