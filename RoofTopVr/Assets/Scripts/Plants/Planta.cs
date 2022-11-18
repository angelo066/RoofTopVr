using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Tipos de plantas que tenemos en el juego (Añadir aqui si se quieren meter mas)
public enum tipo { Monstera, Violeta, Corona, Tomatera, Anturio, Jade, Peyote};

public class Planta : MonoBehaviour
{
    //Cuanto calienta el radiador, si se modifica aqui se modifica en todas
    private const  int potenciaCalentador = 20;

    private const  int temperaturaTerraza = 21;     //La temperatura que haría en la terraza

    //Representa la planta que es
    public tipo tipo_;
    //Representa como de dificil es para la planta PERDER agua
    public int retencion;

    //Representa como de dificil es para la planta COGER agua de la maceta
    public int resistencia;

    //A que temperatura esta ahora mismpo la planta
    private int temperatura_Actual = temperaturaTerraza;        //En grados centigrados

    //Como de humedad esta ahora mismo la planta
    private int humedad_Actual = 0;                 //Puede ser que estas inicializaciones haya que cambiarlas más tarde (Son los valores con los que empiezan las plantas)

    //Cuanta agua tiene la planta ahora mismo
    private int agua_Actual = 0;

    //El agua que tiene ahora mismo la maceta, no lo hago en otro script, porque lo unico que tendría sería esta variable, y 
    //Como no vamos a poder tener plantas sin maceta, es irrelevante,
    private int aguaEnMaceta = 0;

    //Los rangos OPTIMOS de la planta en estos tres aspectos (Son arrays de dos porque C# no tiene pair)
    //  ***** IMPORTATE : El elemento 0 es el MINIMO y el elemento 1 es el MÁXIMO *****
    public int [] rangoTemperatura = new int[2];
    public int [] rangoHumedad = new int[2];
    public int [] rangoAgua = new int[2];

    //Será el objeto de la escena 
    GameObject calentador;

    //Para que sepamos si el calentador/humificador tienen que funcionar o no
    bool enInvernadero = false;

    private void Update()
    {
        //Como de lejos estamos del radiador
        Vector3 DistRad = transform.position - calentador.transform.position;

        //Si esta en el invernadero le subimos la temperatura en funcion a como de cerca esta del calentador (MAXIMA = temperatura_Actual, ahora mismo 20, + potenciaCalentador)
        if (enInvernadero)
        {
            temperatura_Actual = temperaturaTerraza + (potenciaCalentador - (int)DistRad.magnitude);
            Debug.Log("Cama dentro");
        }
        else Debug.Log("Cama fuera");

    }

    private void Start()
    {
        calentador = GameObject.FindGameObjectWithTag("Calentador");
    }

    //Si esta dentro del invernadero y sale, lo saca, y viceversa.
    public void invernadero() { enInvernadero = !enInvernadero; }
}
