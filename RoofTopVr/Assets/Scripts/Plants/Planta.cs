using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Reseteamos humedad y temperatura cuando pasa el dia o lo dejamos igual.
/// Cuanta agua pierden las plantas al pasar el dia
/// Mirar lo de la poda
/// Que diferencias queremos que sean criticas
/// </summary>


//Tipos de plantas que tenemos en el juego (Añadir aqui si se quieren meter mas)
public enum tipo { Monstera, Violeta, Corona, Tomatera, Anturio, Jade, Peyote };
public enum Plagas { Ninguna, Aranias, Moscas, Gusanos };
public enum Luz { Directa, Indirecta, Penumbra };

public class Planta : MonoBehaviour
{
    GameManager gM;

    Luz luzTerraza;

    int potenciaHumidificador;

    //Cuanto calienta el radiador, si se modifica aqui se modifica en todas
    int potenciaCalentador;

    int humedadTerraza;     //La humedad que haría en la terraza

    int temperaturaTerraza;     //La temperatura que haría en la terraza

    //Representa la planta que es
    public tipo tipo_;

    Plagas plag = Plagas.Ninguna;

    //Luz actual
    Luz l;

    //Tanto la retencion como la resistencia se representan como porcentaje

    //Representa como de dificil es para la planta PERDER agua
    public int retencion;

    //Representa como de dificil es para la planta COGER agua de la maceta
    public int resistencia;

    //Nivel 1,2 y 3 de como de mal esta la planta respecto a sus rangos ideales. (Pueden ser negativas)
    public int[] diferenciasHumedad;
    public int[] diferenciasAgua;
    public int[] diferenciasTemperatura;
    public int[] diferenciasLuz;

    //A que temperatura esta ahora mismpo la planta
    private int temperatura_Actual;        //En grados centigrados

    //Como de humedad esta ahora mismo la planta
    private int humedad_Actual;                 //Puede ser que estas inicializaciones haya que cambiarlas más tarde (Son los valores con los que empiezan las plantas)

    //Cuanta agua tiene la planta ahora mismo
    private int agua_Actual = 0;

    //El agua que tiene ahora mismo la maceta, no lo hago en otro script, porque lo unico que tendría sería esta variable, y 
    //Como no vamos a poder tener plantas sin maceta, es irrelevante,
    private int aguaEnTierra = 0;

    //La humedad que le añadimos con el flusflus
    private int humedadAcumulada = 0;

    //Los rangos OPTIMOS de la planta en estos tres aspectos (Son arrays de dos porque C# no tiene pair)
    //  ***** IMPORTATE : El elemento 0 es el MINIMO y el elemento 1 es el MÁXIMO *****
    //SOLO SON POSITIVAS
    public int[] rangoTemperatura = new int[2];
    public int[] rangoHumedad = new int[2];
    public int[] rangoAgua = new int[2];

    public Luz perfectLight;

    //Será el objeto de la escena 
    GameObject calentador;

    GameObject humidificador;

    //Para que sepamos si el calentador/humificador tienen que funcionar o no
    bool enInvernadero = false;

    //empieza sana
    private int salud_Agua = 0;
    private int salud_Temperatura = 0;
    private int salud_Humedad = 0;
    private int salud_Luz = 0;


    //Componentes para graficos
    GameObject hojas;
    MeshFilter modelo;
    MeshRenderer render;

    //Materiales para los cambios de estado
    [SerializeField]
    Material plantaPeq;
    [SerializeField]
    Material plantaMed;
    [SerializeField]
    Material plantaGrand;
    Material plantaMuerta;

    //Prefabs para los cambios de modelo
    [SerializeField]
    GameObject pequeña;
    [SerializeField]
    GameObject mediana;
    [SerializeField]
    GameObject grande;
    [SerializeField]
    GameObject pequeñaMuerta;
    [SerializeField]
    GameObject medianaMuerta;
    [SerializeField]
    GameObject grandeMuerta;

    //Colores 
    [SerializeField]
    Color verde;
    [SerializeField]
    Color colorSeco;
    [SerializeField]
    Color colorMuySeco;
    [SerializeField]
    Color colorPodrido;
    [SerializeField]
    Color colorMuyPodrido;




    private void Update()
    {

        //Esto es para debugear
        if (Input.GetKeyDown("space"))
        {
            Debug.Log(render.material);
            render.material.SetColor("_Color", colorMuySeco);
        }


        //Esto es para debugear
        if (Input.GetKeyDown(KeyCode.R))
        {
            flusflus(-10);
            Debug.Log(humedad_Actual);
        }

        //Esto es para debugear
        if (Input.GetKeyDown(KeyCode.Z))
        {
            finDelDia();
        }
    }

    private void Start()
    {
        gM = GameManager.instance;

        hojas = transform.GetChild(3).gameObject;
        modelo = hojas.GetComponent<MeshFilter>();
        render = hojas.GetComponent<MeshRenderer>();
        
        calentador = GameObject.FindGameObjectWithTag("Calentador");
        humidificador = GameObject.FindGameObjectWithTag("Humificador");

        temperaturaTerraza = gM.temperatura();
        humedadTerraza = gM.humedad();
        luzTerraza = gM.luzTerr();
        potenciaCalentador = gM.rad();
        potenciaHumidificador = gM.hum();

        temperatura_Actual = temperaturaTerraza;
        humedad_Actual = humedadTerraza;

        gM.setPlanta(gameObject);

    }

    //Para el humidificador
    private void CalculaHumedad()
    {
        if (enInvernadero)
        {
            Vector3 DistHum = transform.position - humidificador.transform.position;
            humedad_Actual = humedadTerraza + (potenciaHumidificador - (int)DistHum.magnitude);
        }
    }

    //Para el radiador
    private void CalculaTemperatura()
    {
        //Si esta en el invernadero le subimos la temperatura en funcion a como de cerca esta del calentador (MAXIMA = temperatura_Actual, ahora mismo 20, + potenciaCalentador)
        if (enInvernadero)
        {
            //Como de lejos estamos del radiador
            Vector3 DistRad = transform.position - calentador.transform.position;
            temperatura_Actual = temperaturaTerraza + (potenciaCalentador - (int)DistRad.magnitude);
        }
        else
        {
            
            temperatura_Actual = gM.temperatura();
        }
    }

    private void CalculaLuz()
    {
        if(l != perfectLight)
        {
            if (l == Luz.Penumbra) salud_Luz += 10;
            else salud_Luz += 5;
        }
    }

    //Si esta dentro del invernadero y sale, lo saca, y viceversa.
    public void invernadero()
    {
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
        aguaEnTierra += cantidad;
    }

    public void setRadiador(int cantidad) { potenciaCalentador = cantidad; }

    public void calculoAgua()
    {
        //El agua que la planta se resiste a coger
        int agua_NoAbsorbida = (aguaEnTierra * resistencia) / 100;

        //El agua que absorbe la planta
        int agua_Absorbida = aguaEnTierra - agua_NoAbsorbida;

        //Le sumamos el agua que absorbe
        agua_Actual += agua_Absorbida;
                                         
        //El porcentaje de agua que se queda
        int agua_Perdida = (agua_Actual * retencion) / 100;

        agua_Actual = agua_Actual - agua_Perdida;
        //Le restamos a las tierra el agua que abosorbe la planta
        aguaEnTierra -= agua_Absorbida;
    }

    public void setLuz(Luz nL) { l = nL; }

    //Llamarlo cuando impacten las particulas del flusflus
    public void flusflus(int cantidad)
    {
        humedad_Actual += cantidad;
        humedadAcumulada += cantidad;
    }

    void resetTemperatura() { temperatura_Actual = temperaturaTerraza; }
    void resetHumedad() { humedad_Actual = humedadTerraza + humedadAcumulada; }

    public void finDelDia()
    {
        //Comprobar los valores actuales con los rangos ideales

        CalculaHumedad();

        //CalculaLuz();

        //CalculaTemperatura();

        //calculoAgua();


        estadoHumedad();

        //estadoLuz();

        //estadoTemperatura();

        //estadoAgua();
    }
    public void setTerraza(int temperatura, int humedad, int rad, int hum)
    {
        temperaturaTerraza = temperatura;
        humedadTerraza = humedad;
        potenciaCalentador = rad;
        potenciaHumidificador = hum;
    }

    //Cálculos de fin del día
    private void estadoAgua()
    {
        if (agua_Actual <= rangoAgua[1] && agua_Actual >= rangoAgua[0])
        {
            //Tamos gucci, la planta crece
        }
        else
        {
            //Calculamos la difrencia del dia actual
            int diferencia = 0;

            //SI es mayor que la maxima
            if(agua_Actual > rangoAgua[1]) diferencia = agua_Actual - rangoAgua[1];
            else diferencia = agua_Actual  - rangoAgua[0]; //Es meor que el minimo

            //Se la añadimos a la diferencia general
            salud_Agua += diferencia;

            //Comprobamos el estado en funcion de la diferencia
            int estado = calculaDiferencia(salud_Agua, diferenciasAgua);

            switch (estado)
            {
                //Muy seca
                case -3:
                    render.material.SetColor("_Color", colorMuySeco);

                    break;
                // Bastante seca
                case -2:
                    render.material.SetColor("_Color", colorSeco);

                    break;
                //Un poco seca
                case -1:

                    break;
                //Te pasas un poco
                case 1:

                    break;
                //Te pasas bastate
                case 2:
                    render.material.SetColor("_Color", colorPodrido);

                    break;
                //Te pasas mucho
                case 3:
                    render.material.SetColor("_Color", colorMuyPodrido);

                    break;

                default:
                    break;
            }
        }
    }

    private void estadoTemperatura()
    {
        if (temperatura_Actual <= rangoTemperatura[1] && temperatura_Actual >= rangoTemperatura[0])
        {
            //Tamos gucci, la planta crece
        }
        else
        {
            int diferencia = 0;

            if (temperatura_Actual > rangoTemperatura[1]) diferencia = temperatura_Actual - rangoTemperatura[1];
            else diferencia = temperatura_Actual - rangoTemperatura[0]; //Es meor que el minimo

            salud_Temperatura += diferencia;

            int estado = calculaDiferencia(salud_Temperatura, diferenciasTemperatura);

            switch (estado)
            {
                //Muy fria
                case -3:
                    render.material.SetColor("_Color", colorMuyPodrido);
                    break;
                // Bastante fria
                case -2:
                    render.material.SetColor("_Color", colorPodrido);
                    break;
                //Un poco fria
                case -1:

                    break;
                //Te pasas un poco
                case 1:
                    break;
                //Te pasas bastate
                case 2:
                    render.material.SetColor("_Color", colorSeco);
                    //Te secas que te cagas
                    //Material = seca
                    break;
                //Te pasas mucho
                case 3:
                    render.material.SetColor("_Color", colorMuySeco);

                    break;

                default:
                    break;
            }
        }
    }

    private void estadoHumedad()
    {
        if (humedad_Actual <= rangoHumedad[1] && humedad_Actual >= rangoHumedad[0])
        {
            //Tamos gucci, la planta crece
        }
        else
        {
            //Debug.Log(humedad_Actual);
            //Debug.Log(rangoHumedad[1]);
            int diferencia = humedad_Actual - rangoHumedad[1];

            if (humedad_Actual > rangoHumedad[1]) diferencia = humedad_Actual - rangoHumedad[1];
            else diferencia = humedad_Actual - rangoHumedad[0]; //Es meor que el minimo

            salud_Humedad += diferencia;

            int estado = calculaDiferencia(salud_Humedad, diferenciasHumedad);


            switch (estado)
            {
                //Muy seca
                case -3:
                    render.material.SetColor("_Color", colorMuySeco);
                    break;
                // Bastante seca
                case -2:
                    render.material.SetColor("_Color", colorSeco);
                    break;
                //Un poco seca
                case -1:
                    break;
                //Te pasas un poco
                case 1:
                    break;
                //Te pasas bastate
                case 2:
                    render.material.SetColor("_Color", colorPodrido);
                    break;
                //Te pasas mucho
                case 3:
                    render.material.SetColor("_Color", colorMuyPodrido);
                    break;

                default:
                    break;
            }
        }
    }

    private void estadoLuz()
    {
        if (l == perfectLight)
        {
            //Tamos gucci, la planta crece
        }
        else
        {
            if(l == Luz.Penumbra) render.material.SetColor("_Color", colorPodrido);
            else render.material.SetColor("_Color", colorSeco);
        }
    }

    //Te devuelve en un número el estado de la planta en el atributo que quieras
    private int calculaDiferencia(int diferencia, int[] diferencias)
    {
        if (diferencia >= diferencias[2])
        {
            return 3;
            //Mega húmeda
        }
        else if (diferencia >= diferencias[1])
        {
            return 2;
            //Bastante húmeda
        }
        else if (diferencia >= diferencias[0])
        {
            return 1;
            //Se pasa un poquito
        }//La diferencia es negativa
        else if (diferencia <= -diferencias[2])
        {
            return -3;
            //Mega seca
        }
        else if (diferencia <= -diferencias[1])
        {
            return -2;
            //Bastante seca
        }
        else //No quedan más opciones
        {
            return -1;
            //Un poquito seca
        }
    }

    //Método para cuando cambian las condiciones de la terraza

}
