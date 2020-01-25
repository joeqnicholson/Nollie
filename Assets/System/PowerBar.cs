using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PowerBar : MonoBehaviour
{
    public Text powerText;

    void Update()
    {
        powerText.text = Nollie.power.ToString();
    }
}
