using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakableObject : MonoBehaviour
{
    public float health;

    // Update is called once per frame
    void Update()
    {
        if (health <= 0) { Destroy(gameObject); }
    }

    private void OnTriggerEnter(Collider other)
    {
        print("hit");

        if (other.gameObject.tag == "Bullet")
        {
            health -= other.gameObject.GetComponent<Bullet>().damage;
            
        }        
    }
}
