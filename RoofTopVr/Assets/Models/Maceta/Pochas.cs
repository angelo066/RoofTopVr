using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pochas : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField]
    Transform[] posiciones_disponibles;

    [SerializeField]
    Transform centro;

    [SerializeField]
    GameObject pocha;
    void Start()
    {
        for(int i = 0; i < posiciones_disponibles.Length; i++)
        {
            // Calculos para generar una planta
            Vector3 dir = -centro.position + posiciones_disponibles[i].position;
            //Debug.Log(dir);

            // Generar el prefab
            GameObject nueva_pocha = Instantiate(pocha, posiciones_disponibles[i].position, Quaternion.identity);
            nueva_pocha.transform.SetParent(posiciones_disponibles[i]);

            int aux = Random.Range(7, 15);
            float factor_escala = aux * 0.1f;
            nueva_pocha.transform.localScale *= factor_escala;

            Quaternion rot = Quaternion.LookRotation(dir);
            nueva_pocha.transform.rotation = rot;
        }

        

    }

    // Update is called once per frame
    void Update()
    {
       

    }

    
}
