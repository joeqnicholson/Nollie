using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed;
    public float damage = 5;
    public GameObject hitEffect;

    void Update()
    {
        transform.position += transform.forward *Time.deltaTime * speed;
    }
    private void OnTriggerEnter(Collider other)
    {
        Destroy(gameObject);
        Instantiate(hitEffect,transform.position,transform.rotation);
    }
}
