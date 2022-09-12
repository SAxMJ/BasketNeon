using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Android;

public class scrTemporizador : MonoBehaviour
{
    // Start is called before the first frame update
    int cont = 0;
    int tempInt=3;
    public Text temtext;
    public Text texTiempoJuego;
    public string prefsTiempoJuego = "tempJuego";
    float tiempoJuego;
    void Start()
    {
        //Establecemos el valor del tiempo asigando para la partida
        EstableceTiempoJuego();

        temtext.text = tempInt.ToString("");
        Invoke("IniciarTemporizador", 0f);
    }

    private void IniciarTemporizador()
    {
        if (cont < 3)
            Invoke("ModificaTemporizador", 1f);

        if (cont >= 3)
            Invoke("CargaEscenaJugar",1f);

        cont++;
    }

    public void ModificaTemporizador()
    {
        tempInt--;
        temtext.text = tempInt.ToString("");
        IniciarTemporizador();
    }

    private void CargaEscenaJugar()
    {
        SceneManager.LoadScene(2);
    }

    private void EstableceTiempoJuego()
    {
        tiempoJuego=PlayerPrefs.GetFloat(prefsTiempoJuego, 0);
        texTiempoJuego.text = tiempoJuego.ToString("");
    }

}
