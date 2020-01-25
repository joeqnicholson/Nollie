using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Donut : MonoBehaviour
{

    public GameObject Confetti;
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Pickup();
        }
    }

    void Pickup()
    {

        Nollie.power += 4;

        Instantiate(Confetti, transform.position, transform.rotation);
        Destroy(gameObject);
    }
}
