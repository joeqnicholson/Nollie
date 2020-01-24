using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class turnCube : MonoBehaviour
{
  public Transform tCube;

    void Update()
    {
        tCube.rotation = transform.rotation;
        tCube.position = transform.position;
    }
}
