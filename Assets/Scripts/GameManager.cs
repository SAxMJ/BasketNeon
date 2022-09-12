using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Android;

/*Un GameManager es algo que mantiene un registro del estado del juego, gestiona los sistemas de menú/pausa, 
registra y almacena información para varios propósitos (ajustes de audio/vídeo, enlaces de control, 
datos de guardado del juego). */

public class GameManager : MonoBehaviour
{

    public static GameManager Instance;

    void Awake()
    {
            if (Instance == null)
            {
                Instance = this;
            }
    }

    Camera cam;
    public ScriptPelotas scrBola;
    public ScriptPelotas scrBola2;
    [SerializeField] float fuerzaLanzadoR = 4f,distanciaR;
    [SerializeField] float fuerzaLanzadoA = 4f, distanciaA;
    //bool deslizando = false;
    Vector2 puntoInicioA, puntoFinalA, direccionA, fuerzaA;
    Vector2 puntoInicioR, puntoFinalR, direccionR, fuerzaR;
    int puntosAzul=0, puntosRosa=0;
    int numTouchAzul, numTouchRosa;
    bool initLanzamientoRosa = false, initLanzamientoAzul = false;

    public Text ptsA, ptsR;
    public Text textTiempoJuego;

    public GameObject canastaA, canastaR;
    public GameObject mulTextA, mulTextR;
    public GameObject boostPor2, boost2Pelotas;

    public float tiempoBoost; //El tiempo para que aparezca un boost será entre 10 y 30 segundos
    public bool flagTiempoBoost = false, boostInstanciado = false;
    public int multIntA, multIntR;
    public float tiempoJuego;
    int puntosPelotaBoostAzul = 0;
    int puntosPelotaBoostRosa = 0;

    public string prefsTiempoJuego="tempJuego";
    public string guardaPuntosAzul = "puntosAzul";
    public string guardaPuntosRosa = "puntosRosa";

    Touch touchAzul, touchRosa;

    // Start is called before the first frame update
    void Start()
    {
        cam = Camera.main;
        RecuperarTiempoJuego();

        modificaCanastaAzul();
        modificaCanastaRosa();

        scrBola.DesactivaFisicas();
        scrBola.SueloTocado(1);
        scrBola2.DesactivaFisicas();
        scrBola2.SueloTocado(1);

        puntosAzul = scrBola.getPuntuacion();
        puntosRosa = scrBola2.getPuntuacion();

        sumaMarcadorAzul();
        sumaMarcadorRosa();

        multIntA = 1;
        multIntR = 1;

        mulTextA.GetComponent<Renderer>().enabled = false;
        mulTextR.GetComponent<Renderer>().enabled = false;
    }  

    // Update is called once per frame
    void Update()
    {


        if (tiempoJuego > 0)
        {
            ActualizaTiempoJuego();

            //Si el flag está a false, quiere decir que hay que crear un nuevo tiempo para que aparezca el boost
            if(!flagTiempoBoost)
            {
                GeneraTiempoBoostRand();
            }
            //Si no, entonces quiere decir que ya hay un tiempo y tenemos que disminuir el contador de tiempo
            else
            {
                DisminuyeTiempoBoostRand();
            }

            //Ahora comprobamos si el tiempo de boost es 0 o menor que 0, lo que indicaría que hay que instanciar un boost para cada jugador
            if(tiempoBoost<=0 && boostInstanciado==false)
            {
                InstanciaBoost();
            }

               
            //Comprobamos si hay que modificar la canasta y volvemos a dejar el flag a false para que 
            //La bola lo pueda poner a true de nuevo cuando sea necesario
            if (scrBola2.managerModificaCanasta == true)
            {
                scrBola2.managerModificaCanasta = false;
                Invoke("modificaCanastaRosa", 0.8f);
                multIntR = 1;
            }

            if (scrBola.managerModificaCanasta == true)
            {
                multIntA = 1;
                scrBola.managerModificaCanasta = false;
                Invoke("modificaCanastaAzul", 0.8f);
            }

            //Comprobamos si se ha modificado alguna de las variables del multiplicador y si es así mostramos el multiplicador correspondiente
            if (multIntA < scrBola.multiplicador)
            {
                multiplicadorAzul();
            }

            if (multIntR < scrBola2.multiplicador)
            {
                //Debug.Log("Se cumple la condicion");
                multiplicadorRosa();
            }
                                                                
            if (scrBola.sueloTocado == 1)
            {
                Debug.Log("El valor de suelo tocado es: " + scrBola.sueloTocado);

                if (Input.touchCount > 0)
                {

                        //if (Input.touchCount > 0 && cam.ScreenToWorldPoint(Input.touches[0].position).x < 0 && deslizandoAzul == false)
                        if (!initLanzamientoAzul)
                        {
                            numTouchAzul = Input.touchCount;
                            touchAzul = Input.GetTouch(numTouchAzul-1);
                        }
                        else
                        {
                            numTouchAzul = Input.touchCount;
                            touchAzul = Input.GetTouch(numTouchAzul-1);
                        }

                    if (cam.ScreenToWorldPoint(Input.touches[numTouchAzul - 1].position).x < 0)
                    {

                        // Handle finger movements based on TouchPhase
                        switch (touchAzul.phase)
                        {
                            //When a touch has first been detected, change the message and record the starting position
                            case TouchPhase.Began:

                                multIntA = 1;
                                InicioDeslizarMovilA(touchAzul);


                                initLanzamientoAzul = true;
                                // Record initial touch position.

                                break;

                            //Determine if the touch is a moving touch
                            case TouchPhase.Moved:
                                DeslizandoMovilA();
                                break;

                            case TouchPhase.Ended:
                                FinDeslizarMovilA();

                                //Si nos movemos a la camara del contrario  no cambiaremos la variable de suelo tocado para pocer volver a lanzar
                                Debug.Log("EL VALOR DE PUNTO FINAL X ES " + puntoFinalA.x);
                                if (puntoFinalA.x < 0 && puntoInicioA.x<0)
                                    scrBola.SueloTocado(0);

                                initLanzamientoAzul = false;

                                break;
                        }
                    }
                }

            }



            //CODIGO QUE CONTROLA LANZAMIENTO DE LA BOLA ROSA
            if (scrBola2.sueloTocado == 1)
            {
                if (Input.touchCount > 0)
                {
                    if (!initLanzamientoRosa)
                    {
                        numTouchRosa = Input.touchCount;
                        touchRosa = Input.GetTouch(numTouchRosa-1);
                    }
                    else
                    {
                        numTouchRosa = Input.touchCount;
                        touchRosa = Input.GetTouch(numTouchRosa-1);
                    }



                    if (cam.ScreenToWorldPoint(Input.touches[numTouchRosa-1].position).x > 0)
                    {
                        // Handle finger movements based on TouchPhase
                        switch (touchRosa.phase)
                        {
                            //When a touch has first been detected, change the message and record the starting position
                            case TouchPhase.Began:

                                multIntA = 1;
                                Debug.Log("Fuera" + touchRosa.phase);
                                InicioDeslizarMovilR(touchRosa);

                                initLanzamientoRosa = true;
                                break;

                            //Determine if the touch is a moving touch
                            case TouchPhase.Moved:

                                DeslizandoMovilR();

                                break;

                            case TouchPhase.Ended:

                                FinDeslizarMovilR();

                                //Si nos movemos a la camara del contrario  no cambiaremos la variable de suelo tocado para pocer volver a lanzar
                                if (puntoFinalR.x > 0 && puntoInicioR.x>0)
                                    scrBola2.SueloTocado(0);

                                initLanzamientoRosa = false;
                                break;
                        }
                    }
                }
            }
//#endif




            //Comprobamos si ha cambiado la puntuación y por lo tanto se ha encestado
            if (puntosAzul < scrBola.getPuntuacion())
            {
                sumaMarcadorAzul();
                AlmacenaPuntosAzul();
            }

            if (puntosRosa < scrBola2.getPuntuacion())
            {
                sumaMarcadorRosa();
                AlmacenaPuntosRosa();
            }

            //Comprobamos si ha cambiado la infomración referente a lo puntos de las pelotas boost
            if(puntosPelotaBoostAzul<scrBola.puntosPelotaBoost)
            {
                sumaMarcadorAzulPorBoost();
                AlmacenaPuntosAzul();
            }

            //Comprobamos si ha cambiado la infomración referente a lo puntos de las pelotas boost
            if (puntosPelotaBoostRosa < scrBola2.puntosPelotaBoost)
            {
                sumaMarcadorRosaPorBoost();
                AlmacenaPuntosRosa();
            }
        }

        else
        {
            AlmacenaPuntosAzul();
            AlmacenaPuntosRosa();
            CompruebaGanador();
        }

    }


    /*
    //ESTAS SERÁN LAS FUNCIONES DE DESLIZAR PARA ORDENADOR
    void InicioDeslizar()
    {
        //Transformamos un punto del espacio de la pantalla al espacio del mundo(que es un sistema de coordenadas)
        //y como parámetro le introducimos la posición del ratón.
        puntoInicio = cam.ScreenToWorldPoint(Input.mousePosition);

        if(puntoInicio.x<0)
            scrBola.DesactivaFisicas();

        if(puntoInicio.x>0)
            scrBola2.DesactivaFisicas();

        //bug.Log("entro");
    }

    void Deslizando()
    {
        //Calculamos la distancia, la dirección y la fuerza en función de los valores del punto inicial y final
        //con los que vamos a hacer el tiro
        puntoFinal= cam.ScreenToWorldPoint(Input.mousePosition);
        distancia = Vector2.Distance(puntoInicio, puntoFinal);
        direccion = (puntoInicio - puntoFinal).normalized;
        fuerza = direccion * distancia * fuerzaLanzado;

        //bug.DrawLine(puntoInicio,puntoFinal);
    }

    void FinDeslizar()
    {
        if (puntoInicio.x < 0 && puntoFinal.x < 0)
        {
            scrBola.ActivaFisicas();
            //Cuando soltamos para lanzar llamamos a la función que ejerce la fuerza sobre la bola
            scrBola.Empuje(fuerza);
        }

        if (puntoInicio.x > 0 && puntoFinal.x > 0)
        {
            scrBola2.ActivaFisicas();
            //Cuando soltamos para lanzar llamamos a la función que ejerce la fuerza sobre la bola
            scrBola2.Empuje(fuerza);
        }
    }

    */


    //ESTAS SERÁN LAS FUNCIONES DE DESLIZAR PARA MOVIL
    void InicioDeslizarMovilA(Touch touch)
    {
        //Transformamos un punto del espacio de la pantalla al espacio del mundo(que es un sistema de coordenadas)
        //y como parámetro le introducimos la posición del ratón.
        puntoInicioA = cam.ScreenToWorldPoint(touch.position);
        scrBola.DesactivaFisicas();
    }

    void InicioDeslizarMovilR(Touch touch)
    {
        //Transformamos un punto del espacio de la pantalla al espacio del mundo(que es un sistema de coordenadas)
        //y como parámetro le introducimos la posición del ratón.
        puntoInicioR = cam.ScreenToWorldPoint(touch.position);
        scrBola2.DesactivaFisicas();
    }

    void DeslizandoMovilA()
    {
        //Calculamos la distancia, la dirección y la fuerza en función de los valores del punto inicial y final
        //con los que vamos a hacer el tiro
        puntoFinalA = cam.ScreenToWorldPoint(Input.mousePosition);
        distanciaA = Vector2.Distance(puntoInicioA, puntoFinalA);
        direccionA = (puntoInicioA - puntoFinalA).normalized;
        fuerzaA = direccionA * distanciaA * fuerzaLanzadoA;
    }

    void DeslizandoMovilR()
    {
        //Calculamos la distancia, la dirección y la fuerza en función de los valores del punto inicial y final
        //con los que vamos a hacer el tiro
        puntoFinalR = cam.ScreenToWorldPoint(Input.mousePosition);
        distanciaR = Vector2.Distance(puntoInicioR, puntoFinalR);
        direccionR = (puntoInicioR - puntoFinalR).normalized;
        fuerzaR = direccionR * distanciaR * fuerzaLanzadoR;
    }

    void FinDeslizarMovilA()
    {
            scrBola.ActivaFisicas();
            //Cuando soltamos para lanzar llamamos a la función que ejerce la fuerza sobre la bola
            scrBola.Empuje(fuerzaA);
     }
    void FinDeslizarMovilR()
    {
             scrBola2.ActivaFisicas();
            //Cuando soltamos para lanzar llamamos a la función que ejerce la fuerza sobre la bola
            scrBola2.Empuje(fuerzaR);
    }

    //Cambiamos la información del marcador y actualizamos la variable de puntos
    private void sumaMarcadorAzul()
    {
        puntosAzul = scrBola.getPuntuacion();
        ptsA.text = puntosAzul.ToString("");
        Invoke("modificaCanastaAzul", 0.8f);
    }


    //Si se ha encestado con un boost, se modifican y actualizan todas las puntuaciones correspondientes
    //para que no se produzcan errores inesperados
    private void sumaMarcadorAzulPorBoost()
    {
        puntosAzul = scrBola.puntuacion + scrBola.puntosPelotaBoost;
        scrBola.puntuacion += scrBola.puntosPelotaBoost;
        puntosPelotaBoostAzul = scrBola.puntosPelotaBoost;
        ptsA.text = puntosAzul.ToString("");
    }

    private void sumaMarcadorRosa()
    {

        puntosRosa = scrBola2.getPuntuacion();
        ptsR.text = puntosRosa.ToString("");
        Invoke("modificaCanastaRosa", 0.8f);
    }


    //Si se ha encestado con un boost, se modifican y actualizan todas las puntuaciones correspondientes
    //para que no se produzcan errores inesperados
    private void sumaMarcadorRosaPorBoost()
    {
        puntosRosa = scrBola2.puntuacion + scrBola2.puntosPelotaBoost;
        scrBola2.puntuacion += scrBola2.puntosPelotaBoost;
        puntosPelotaBoostRosa = scrBola2.puntosPelotaBoost;
        ptsR.text = puntosRosa.ToString("");
    }

    //Modificamos la posición de la canasta y eliminamos el multiplicador si lo hay
    private void modificaCanastaAzul()
    {
        //si el multiplicador es mayor que 1 quiere decir que se ha dinbujado un multiplicador en la canasta
        mulTextA.GetComponent<Renderer>().enabled=false;
        canastaA.transform.position = new Vector2(-0.96f,Random.Range(-3.18f, 4.1f));
    }

    private void modificaCanastaRosa()
    {
        mulTextR.GetComponent<Renderer>().enabled = false;
        canastaR.transform.position = new Vector2(0.99f,Random.Range(-3.18f, 4.1f));
    }
    

    private void multiplicadorRosa()
    {
        multIntR = scrBola2.multiplicador;

        mulTextR.transform.position = new Vector2(canastaR.transform.position.x, canastaR.transform.position.y + 1f);
        mulTextR.GetComponent<TextMesh>().text = "×" + multIntR.ToString("");

        if (mulTextR.GetComponent<Renderer>().enabled == false)
            mulTextR.GetComponent<Renderer>().enabled = true;
    }

    private void multiplicadorAzul()
    {
        multIntA = scrBola.multiplicador;

        mulTextA.transform.position = new Vector2(canastaA.transform.position.x-0.5f, canastaA.transform.position.y + 1f);
        mulTextA.GetComponent<TextMesh>().text = "×" + multIntA.ToString("");

        if (mulTextA.GetComponent<Renderer>().enabled == false)
            mulTextA.GetComponent<Renderer>().enabled = true;
    }

    private void RecuperarTiempoJuego()
    {
        tiempoJuego=PlayerPrefs.GetFloat(prefsTiempoJuego, 0);
        textTiempoJuego.text = ((int)tiempoJuego).ToString("");
    }

    private void ActualizaTiempoJuego()
    {
        tiempoJuego -= 1 * Time.deltaTime;
        textTiempoJuego.text = ((int)tiempoJuego).ToString("");
    }
   
    private void CompruebaGanador()
    {
        if(puntosAzul>puntosRosa)
        {
            SceneManager.LoadScene(3);
        }
        else if(puntosRosa>puntosAzul)
        {
            SceneManager.LoadScene(4);
        }
        else
        {
            SceneManager.LoadScene(5);
        }
    }

    private void AlmacenaPuntosAzul()
    {
        PlayerPrefs.SetInt(guardaPuntosAzul, puntosAzul);
    }

    private void AlmacenaPuntosRosa()
    {
        PlayerPrefs.SetInt(guardaPuntosRosa, puntosRosa);
    }

    private void GeneraTiempoBoostRand()
    {
        tiempoBoost=Random.Range(10f, 30f);
        flagTiempoBoost = true;
    }

    private void DisminuyeTiempoBoostRand()
    {
        tiempoBoost -= 1 * Time.deltaTime;
    }

    //Para instanciar un boost lo que hacemos es, generamos un número aleatorio entre 1 y 2 para cada jugador
    //y dependiendo del resultado, le instanciaremos una doble pelota o un doble multiplicador
    private void InstanciaBoost()
    {
        int a, r;

        a = Random.Range(1, 3);
        r = Random.Range(1, 3);

        //boostInstanciado = true;
        flagTiempoBoost = false;

        DestruirLosBoosInstanciadosSiHay();

        if (a == 1)
        {
            Instantiate(boost2Pelotas, new Vector2(Random.Range(-8.5f, -1.5f), Random.Range(-3f, 4.5f)), transform.rotation);
        }
        else
        {
            Instantiate(boostPor2, new Vector2(Random.Range(-8.5f, -1.5f), Random.Range(-3f, 4.5f)), transform.rotation);
        }

        if(r==1)
        {
            Instantiate(boost2Pelotas, new Vector2(Random.Range(1.52f, 8.4f), Random.Range(-3f, 4.5f)), transform.rotation);
        }
        else
        {
            Instantiate(boost2Pelotas, new Vector2(Random.Range(1.52f, 8.4f), Random.Range(-3f, 4.5f)), transform.rotation);
        }
    }

    private void DestruirLosBoosInstanciadosSiHay()
    {
        GameObject[] boost = GameObject.FindGameObjectsWithTag("BoostDoblePelota");

        if(boost!=null)
            for (int i = 0; i < boost.Length; i++)
            {
                Destroy(boost[i]);
            }


        GameObject[] boost2 = GameObject.FindGameObjectsWithTag("BoostMultPor2");

        if(boost2!=null)
            for (int i = 0; i < boost2.Length; i++)
            {
                Destroy(boost2[i]);
            }

    }
}


