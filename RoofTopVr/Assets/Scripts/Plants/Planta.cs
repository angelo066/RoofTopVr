using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Tipos de plantas que tenemos en el juego (Añadir aqui si se quieren meter mas)
public enum tipo { Monstera, Violeta, Corona, Tomatera, Anturio, Jade, Peyote};
public class Planta : MonoBehaviour
{
    //Representa la planta que es
    public tipo tipo_;
    //Representa como de dificil es para la planta PERDER agua
    public int retencion;

    //Representa como de dificil es para la planta COGER agua de la maceta
    public int resistencia;

    //A que temperatura esta ahora mismpo la planta
    private int temperatura_Actual;

    //Como de humedad esta ahora mismo la planta
    private int humedad_Actual = 0;                 //Puede ser que estas inicializaciones haya que cambiarlas más tarde

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

}
