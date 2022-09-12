using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Android;
public class scrMenu : MonoBehaviour
{

    float tiempoJuego=60;
    //Le damos el nombre que tendrá la variable en memoria
    string prefsTiempoJuego="tempJuego";
    public InputField inpf;
    public GameObject Sonidos;
    // Start is called before the first frame update
    void Start()
    {
        //El tiempo predeterminado será de 60 segundos
        tiempoJuego = 60;
        inpf.GetComponentInChildren<Text>().text="TIEMPO: "+tiempoJuego.ToString("")+" seg";
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void IncrementaTiempo()
    {
        Sonidos.GetComponent<scrSonidos>().GeneraSonidoDeClick();
        if (tiempoJuego < 120)
        {
            tiempoJuego += 30;
            inpf.GetComponentInChildren<Text>().text = "TIEMPO: " + tiempoJuego.ToString("") + " seg";
        }
    }

    public void DecrementaTiempo()
    {
        Sonidos.GetComponent<scrSonidos>().GeneraSonidoDeClick();
        if (tiempoJuego > 30)
        {
            tiempoJuego -= 30;
            inpf.GetComponentInChildren<Text>().text = "TIEMPO: " + tiempoJuego.ToString("") + " seg";
        }
    }

    public void CambiarEscenaJugar()
    {
        Sonidos.GetComponent<scrSonidos>().GeneraSonidoClickJugar();
        //Nos permite guardar una variable en memoria, de esta forma podremos recuperar el valor
        //del tiempo en la escena de jugar
        PlayerPrefs.SetFloat(prefsTiempoJuego,tiempoJuego);
        SceneManager.LoadScene(1);
    }
}
