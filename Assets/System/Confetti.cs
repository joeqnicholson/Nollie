using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Confetti : MonoBehaviour
{
    float count = 0.2f;
    void Update()
    {
        count -= Time.deltaTime;
        if(count <= 0)
        {
            Destroy(gameObject);
        }
    }
}
