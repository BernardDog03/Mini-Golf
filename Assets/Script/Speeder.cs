using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Speeder : MonoBehaviour
{
    [SerializeField] float force;
    
    bool isSpeeding;
    private void OnCollisionEnter(Collision other)
    {
        Speed(other.collider);
    }

    private void OnTriggerEnter(Collider other)
    {
        Speed(other);
    }

    public void Speed(Collider col)
    {
        if(isSpeeding == false 
            && col.transform.CompareTag("Ball") 
            && col.transform.TryGetComponent<BallController>(out var ball))
        {
            ball.AddForce(force * this.transform.forward, ForceMode.Impulse);
            isSpeeding = true;
            Invoke("Reset", 0.3f);
        }
    }

    public void Reset()
    {
        isSpeeding = false;
    }
}
