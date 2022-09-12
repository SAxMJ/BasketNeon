using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using UnityEngine.Android;


public class scrPelotaBoost : MonoBehaviour
{
    public Rigidbody2D rb;
    public CircleCollider2D collid;
    public Vector3 position { get { return transform.position; } }
    public GameManager gm;
    public int sueloTocado = 1;
    public GameObject particulas;
    public GameObject particulasRoto1, particulasRoto2, fragmentos, particulasRebote;
    TrailRenderer colaPelota;
    public int puntuacion = 0;
    public int multiplicador = 1;
    public int reboteSuelo = 1;
    public bool managerModificaCanasta = false;
    public bool boostRecogido = false;
    public bool encestado = false;

    //Las dunciones awake son llamadas cuando se carga el GameObject en la escena, es inicializado, etc
    //Será llamado cada vez que la pelota se destrulla y haya que crear una nueva
    public void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        collid = GetComponent<CircleCollider2D>();
        colaPelota = GetComponent<TrailRenderer>();
        activarColaPelota();

        this.enabled = false;

        DesactivaFisicas();

        puntuacion = 0;
        multiplicador = 1;
        reboteSuelo = 1;
    }


    //Función llamada para darle un empuje a la bola
    public void Empuje(Vector2 fuerza)
    {
        rb.AddForce(fuerza, ForceMode2D.Impulse);

    }

    //Las dos siguientes funciones nos permitirán activar o desactivar las físicas de la pelota
    public void ActivaFisicas()
    {
        rb.isKinematic = false;
    }

    public void DesactivaFisicas()
    {
        rb.isKinematic = true;
        rb.angularVelocity = 0f;
        rb.velocity = Vector3.zero;
    }

    //Esta función nos permitirá cambiar el valor de la variable suelo tocado
    //Para poder comprobar posteriormente si hemos tocado el suelo y podemos volver a lanzar
    public void SueloTocado(int valor)
    {
        sueloTocado = valor;
    }

    public int getPuntuacion()
    {
        return puntuacion;
    }

    private void activarColaPelota()
    {
        colaPelota.enabled = true;
    }

    private void desactivarColaPelota()
    {
        colaPelota.enabled = false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Suelo")
        {

            if (reboteSuelo > 0)
            {
                ++multiplicador;
                reboteSuelo--;

                Instantiate(particulasRebote, new Vector2(this.transform.position.x, this.transform.position.y), transform.rotation);
            }
            else
            {
                SueloTocado(1);
                //Informamos al GameManager que recoloque la canasta
                managerModificaCanasta = true;

                DesactivaFisicas();
                this.gameObject.GetComponent<Renderer>().enabled = false;
                desactivarColaPelota();

                Destroy(this.gameObject);
                Instantiate(particulasRoto1, new Vector2(this.transform.position.x, this.transform.position.y), new Quaternion(0f, 0f, 0f, 0f));
                Instantiate(particulasRoto2, new Vector2(this.transform.position.x, this.transform.position.y), new Quaternion(0f, 0f, 0f, 0f));
                Instantiate(fragmentos, new Vector2(this.transform.position.x, this.transform.position.y + 1f), new Quaternion(0f, 0f, 0f, 0f));

            }
        }

        //Cada vez que rebotemos con una parez incrementaremos el multiplicador que se aplicará si encestamos
        if (collision.gameObject.tag == "Pared")
        {
            ++multiplicador;
            Debug.Log("Pared Multplicador " + multiplicador);

            Instantiate(particulasRebote, new Vector2(this.transform.position.x, this.transform.position.y), transform.rotation);
        }

        if (collision.gameObject.tag == "Cesta")
        {
            puntuacion = puntuacion + 2;

            //Ahora vamos a indicar a la pelota azul o rosa que tiene que modificar su variable que controla los puntos de las pelotas boost
            if(this.gameObject.tag=="PBA")
            {
                GameObject pelA = GameObject.FindGameObjectWithTag("PelA");
                pelA.GetComponent<ScriptPelotas>().IncrementaPuntuacionDePelotaBoost(puntuacion);
            }

            if(this.gameObject.tag=="PBR")
            {
                GameObject pelR = GameObject.FindGameObjectWithTag("PelR");
                pelR.GetComponent<ScriptPelotas>().IncrementaPuntuacionDePelotaBoost(puntuacion);
            }

            this.gameObject.GetComponent<Renderer>().enabled = false;
            desactivarColaPelota();

            Destroy(this.gameObject);
            Instantiate(particulas, new Vector2(this.transform.position.x, this.transform.position.y + 1f), transform.rotation);

        }
    }

    public void MuestraPelota()
    {
        this.enabled = true;
    }
}
