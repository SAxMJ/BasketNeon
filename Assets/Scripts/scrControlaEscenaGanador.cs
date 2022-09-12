using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Android;

public class scrControlaEscenaGanador : MonoBehaviour
{
    public string recuperaPuntosAzul = "puntosAzul";
    public string recuperaPuntosRosa = "puntosRosa";
    public Text textPuntosAzul, textPuntosRosa;
    int puntosAzul, puntosRosa;

    // Start is called before the first frame update
    void Start()
    {
        //Recuperamos los puntos de los jugadores almacenados en memoria
        puntosAzul = PlayerPrefs.GetInt(recuperaPuntosAzul, 0);
        puntosRosa = PlayerPrefs.GetInt(recuperaPuntosRosa, 0);

        textPuntosAzul.text = puntosAzul.ToString("") + " PTS";
        textPuntosRosa.text = puntosRosa.ToString("") + " PTS";

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void VolverMenu()
    {
        SceneManager.LoadScene(0);
    }

}
