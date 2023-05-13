using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScriptGoal : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Goal");
    }
}
