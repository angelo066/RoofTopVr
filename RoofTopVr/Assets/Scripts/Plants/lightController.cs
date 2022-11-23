using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class lightController : MonoBehaviour
{
    public Luz lightValue;

    private void OnTriggerEnter(Collider other)
    {
        Planta p = other.GetComponent<Planta>();

        if (p != null)
        {
            p.setLuz(lightValue);

            Debug.Log("Cambio Luz");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Planta p = other.GetComponent<Planta>();

        if (p != null)
        {
            p.setLuz(Luz.Directa);
        }
    }
}
