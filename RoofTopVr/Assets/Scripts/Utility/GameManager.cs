using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance = null;

    private const int maxPlantas = 100;

    private const int maxTemp = 25;
    private const int minTemp = 10;

    private Luz luzTerraza = Luz.Directa;

    private int potenciaHumidificador = 20; //Pasar al humidificador

    //Cuanto calienta el radiador, si se modifica aqui se modifica en todas
    private int potenciaCalentador = 20;    //Pasar al radiador

    private int humedadTerraza = 10;     //La humedad que haría en la terraza

    private int temperaturaTerraza = 21;     //La temperatura que haría en la terraza

    GameObject[] plantas;
    int numPlantas = 0;

    private void Awake()
    {
        if (instance == null) instance = this;
        else if (instance != this) Destroy(gameObject);

        //Esto quizas reviente más tarde, esperemos que no
        plantas = new GameObject[maxPlantas];
    }

    private void Start()
    {

    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            humedadTerraza++;
            Debug.Log(humedadTerraza);
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            humedadTerraza--;
            Debug.Log(humedadTerraza);
        }
    }

    //Luego habrá que instanciar las plantas que toquen, bla bla bla
    public void finDia()
    {
        for(int i = 0; i < numPlantas; i++)
        {
            plantas[i].GetComponent<Planta>().finDelDia();

            randomizeSet();

            plantas[i].GetComponent<Planta>().setTerraza(temperaturaTerraza, humedadTerraza, potenciaCalentador, potenciaHumidificador);
        }
    }

    //Randomiza el estado de la terraza
    private void randomizeSet()
    {
        int varaicionTempertarua = Random.Range(1, 6);//Como mucho 5 grados
        int variacionHumedad = Random.Range(1, 11);//Como mucho 10 %

        int aumenta = Random.Range(0, 2);

        if (aumenta == 1)
        {
            temperaturaTerraza += varaicionTempertarua;
            potenciaCalentador += (varaicionTempertarua / 3);
        }
        else
        {
            temperaturaTerraza -= varaicionTempertarua;
            potenciaCalentador -= (varaicionTempertarua / 3);
        }

        int aumentaHumedad = Random.Range(0, 2);

        if (aumentaHumedad == 1)
        {
            humedadTerraza += variacionHumedad;
            potenciaHumidificador += (variacionHumedad / 3);
        }
        else
        {
            humedadTerraza -= variacionHumedad;
            potenciaHumidificador -= (variacionHumedad / 3);
        }


    }
        
    public Luz luzTerr() { return luzTerraza; }

    public int humedad() { return humedadTerraza; }

    public int temperatura() { return temperaturaTerraza; }

    public int rad() { return potenciaCalentador; }

    public int hum() { return potenciaHumidificador; }

    public void setPlanta(GameObject p)
    {
        plantas[numPlantas] = p;
        numPlantas++;
    }

}
