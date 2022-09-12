using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Android;
public class scrSonidos : MonoBehaviour
{
    public GameObject Rebote1;
    public GameObject Rebote2;
    public GameObject Rebote3;
    public GameObject Rebote4;
    public GameObject Click1;
    public GameObject Click2;
    public GameObject Click3;
    public GameObject ClickJugar;
    float tiempoEliminarSonido = 3f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        tiempoEliminarSonido -= 1 * Time.deltaTime;

        if (tiempoEliminarSonido < 0)
        {
            EliminaObjetosSonido();
        }
        
    }

    public void GeneraSonidoDeRebote()
    {
        int rand;

        rand = Random.Range(1, 5);

        switch (rand)
        {
            case 1:
                Instantiate(Rebote1);
                break;

            case 2:
                Instantiate(Rebote2);
                break;

            case 3:
                Instantiate(Rebote3);
                break;

            case 4:
                Instantiate(Rebote4);
                break;

        }
    }

    public void GeneraSonidoDeClick()
    {
        int rand;

        rand = Random.Range(1, 4);

        switch (rand)
        {
            case 1:
                Instantiate(Click1);
                break;

            case 2:
                Instantiate(Click2);
                break;

            case 3:
                Instantiate(Click3);
                break;
        }
    }

    public void GeneraSonidoClickJugar()
    {
        Instantiate(ClickJugar);
    }

    private void EliminaObjetosSonido()
    {
        GameObject[] sonidos = GameObject.FindGameObjectsWithTag("SonidoRebote");

        if (sonidos != null)
            for (int i = 0; i < sonidos.Length; i++)
            {
                Destroy(sonidos[i]);
            }

        tiempoEliminarSonido = 3f;
    }
}
