using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InvernaderoEntry : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        Planta p = other.gameObject.GetComponent<Planta>();
        if (p != null)
        {
            p.invernadero();
        }
    }
}
