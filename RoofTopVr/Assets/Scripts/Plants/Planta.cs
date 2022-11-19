using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Tipos de plantas que tenemos en el juego (Añadir aqui si se quieren meter mas)
public enum tipo { Monstera, Violeta, Corona, Tomatera, Anturio, Jade, Peyote };

public class Planta : MonoBehaviour
{
    //Cuanto calienta el radiador, si se modifica aqui se modifica en todas
    private const int potenciaCalentador = 20;

    private const int potenciaHumidificador = 20;

    private const int temperaturaTerraza = 21;     //La temperatura que haría en la terraza

    private const int humedadTerraza = 10;     //La humedad que haría en la terraza

    //Representa la planta que es
    public tipo tipo_;

    //Tanto la retencion como la resistencia se representan como porcentaje

    //Representa como de dificil es para la planta PERDER agua
    public int retencion;

    //Representa como de dificil es para la planta COGER agua de la maceta
    public int resistencia;

    //A que temperatura esta ahora mismpo la planta
    private int temperatura_Actual = temperaturaTerraza;        //En grados centigrados

    //Como de humedad esta ahora mismo la planta
    private int humedad_Actual = humedadTerraza;                 //Puede ser que estas inicializaciones haya que cambiarlas más tarde (Son los valores con los que empiezan las plantas)

    //Cuanta agua tiene la planta ahora mismo
    private int agua_Actual = 0;

    //El agua que tiene ahora mismo la maceta, no lo hago en otro script, porque lo unico que tendría sería esta variable, y 
    //Como no vamos a poder tener plantas sin maceta, es irrelevante,
    private int aguaEnTierra = 0;

    //La humedad que le añadimos con el flusflus
    private int humedadAcumulada = 0;

    //Los rangos OPTIMOS de la planta en estos tres aspectos (Son arrays de dos porque C# no tiene pair)
    //  ***** IMPORTATE : El elemento 0 es el MINIMO y el elemento 1 es el MÁXIMO *****
    public int[] rangoTemperatura = new int[2];
    public int[] rangoHumedad = new int[2];
    public int[] rangoAgua = new int[2];

    //Será el objeto de la escena 
    GameObject calentador;

    GameObject humidificador;

    //Para que sepamos si el calentador/humificador tienen que funcionar o no
    bool enInvernadero = false;

    private void Update()
    {


        CalculaTemperatura();

        CalculaHumedad();

        Debug.Log(humedad_Actual);

        //Esto es para debugear
        if (Input.GetKeyDown("space"))
        {
            regada(10);
        }

        //Esto es para debugear
        if (Input.GetKeyDown(KeyCode.F))
        {
            flusflus(1);
        }
    }
    private void Start()
    {
        calentador = GameObject.FindGameObjectWithTag("Calentador");
        humidificador = GameObject.FindGameObjectWithTag("Humificador");
    }

    private void CalculaHumedad()
    {
        if (enInvernadero)
        {
            Vector3 DistHum = transform.position - humidificador.transform.position;
            humedad_Actual = humedadTerraza + (potenciaHumidificador - (int)DistHum.magnitude);
        }
    }


    private void CalculaTemperatura()
    {
        //Si esta en el invernadero le subimos la temperatura en funcion a como de cerca esta del calentador (MAXIMA = temperatura_Actual, ahora mismo 20, + potenciaCalentador)
        if (enInvernadero)
        {
            //Como de lejos estamos del radiador
            Vector3 DistRad = transform.position - calentador.transform.position;
            temperatura_Actual = temperaturaTerraza + (potenciaCalentador - (int)DistRad.magnitude);
        }
    }


    //Si esta dentro del invernadero y sale, lo saca, y viceversa.
    public void invernadero() { 
        enInvernadero = !enInvernadero;

        //Si sale del invernadero, lo ponemos a la humedad y temperatura de la terraza.
        if (!enInvernadero)
        {
            resetHumedad();
            resetTemperatura();
        }
    }

    //A este método hay que llamarlo cuando impacten las párticulas de agua
    //Añadir lo que se quita por la resistencia
    public void regada(int cantidad)
    {
        //El agua que la planta se resiste a coger
        int agua_Perdida = (cantidad * resistencia) / 100;

        //La tierra absorbe el agua
        aguaEnTierra += cantidad;

        //El agua que absorbe la planta
        int agua_Absorbida = aguaEnTierra - agua_Perdida;

        //Le sumamos el agua que absorbe
        agua_Actual += agua_Absorbida;

        //Le restamos a las tierra el agua que abosorbe la planta
        aguaEnTierra -= agua_Absorbida;

        Debug.Log(agua_Actual);
        Debug.Log(aguaEnTierra);
    }

    //Llamarlo cuando impacten las particulas del flusflus
    public void flusflus(int cantidad)
    {
        humedad_Actual++;
        humedadAcumulada++;
    }

    void resetTemperatura() { temperatura_Actual = temperaturaTerraza; }
    void resetHumedad() { humedad_Actual = humedadTerraza + humedadAcumulada; }
}
