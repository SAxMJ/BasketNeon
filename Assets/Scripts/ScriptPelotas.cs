using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Android;

public class ScriptPelotas : MonoBehaviour
{
    public Rigidbody2D rb;
    public CircleCollider2D collid;
    public Vector3 position { get { return transform.position; } }
    public GameManager gm;
    public int sueloTocado=1;
    public GameObject sonidos;
    public GameObject particulas;
    public GameObject particulasRoto1,particulasRoto2,fragmentos,particulasRebote,particulasRecogerBoost;
    public GameObject bp;z
    //Es el objeto PelotaBoost 
    TrailRenderer colaPelota;
    public int puntuacion=0;
    public int multiplicador = 1;
    public int reboteSuelo = 1;
    public bool managerModificaCanasta = false;
    public bool boostPelotaRecogido = false, boostMultiplicadorRecogido = false, boostMultiplicadorActivo = false;
    public bool pelotaBoostCreada = false;
    public int puntosPelotaBoost=0;

    //Las dunciones awake son llamadas cuando se carga el GameObject en la escena, es inicializado, etc
    //Será llamado cada vez que la pelota se destrulla y haya que crear una nueva
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        collid = GetComponent<CircleCollider2D>();
        colaPelota = GetComponent<TrailRenderer>();
        desactivarColaPelota();

        puntuacion = 0;
        multiplicador = 1;
        reboteSuelo = 1;
        Debug.Log("Start Multplicador " + multiplicador);
        if (this.gameObject.name == "PelotaAzul")
            NuevaCestaPelotaAzul();
        if (this.gameObject.name == "PelotaRosa")
            NuevaCestaPelotaRosa();

    }


    public void IncrementaPuntuacionDePelotaBoost(int punt)
    {
        puntosPelotaBoost += punt;
    }

    //Función llamada para darle un empuje a la bola
    public void Empuje(Vector2 fuerza)
    {
        rb.AddForce(fuerza, ForceMode2D.Impulse);

        if (boostPelotaRecogido)
        {
            InstanciaYLanzamientoPelotaBoost(fuerza);
        }  

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
       sueloTocado=valor;
    }



    //Al colocar la pelota de nuevo, mostramos la pelota y reseteamos el multiplicador y el contador de rebotes del suelo
    private void NuevaCestaPelotaAzul()
    {
        transform.position = new Vector2(Random.Range(-8.5f, -1.5f), -4);
        this.gameObject.GetComponent<Renderer>().enabled = true;
        activarColaPelota();

        multiplicador = 1;
        reboteSuelo = 1;

        //Si el multiplicador esta activo ya hay que desactivarlo
        if (boostMultiplicadorActivo)
        {
            boostMultiplicadorActivo = false;
        }

        //Si habíamos recogido el boost multiplicador, lo usaremos en este siguiente tiro
        if (boostMultiplicadorRecogido)
        {
            boostMultiplicadorRecogido = false;
            boostMultiplicadorActivo = true;
        }
    }


    private void NuevaCestaPelotaRosa()
    {
        transform.position = new Vector2(Random.Range(1.52f, 8.4f), -4);
        this.gameObject.GetComponent<Renderer>().enabled = true;
        activarColaPelota();

        multiplicador = 1;
        reboteSuelo = 1;

        //Si el multiplicador esta activo ya hay que desactivarlo
        if (boostMultiplicadorActivo)
        {
            boostMultiplicadorActivo = false;
        }
        //Si habíamos recogido el boost multiplicador, lo usaremos en este siguiente tiro
        if (boostMultiplicadorRecogido)
        {
            boostMultiplicadorRecogido = false;
            boostMultiplicadorActivo = true;
        }
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
        sonidos.GetComponent<scrSonidos>().GeneraSonidoDeRebote();
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

                if (this.gameObject.name == "PelotaAzul")
                {
                    DesactivaFisicas();
                    this.gameObject.GetComponent<Renderer>().enabled = false;
                    desactivarColaPelota();
                                                                                                                    //Utilizao el cuaternion para que las partículas sigan su movimiento correcto
                    Instantiate(particulasRoto1, new Vector2(this.transform.position.x, this.transform.position.y), new Quaternion(0f,0f,0f,0f));
                    Instantiate(particulasRoto2, new Vector2(this.transform.position.x, this.transform.position.y), new Quaternion(0f, 0f, 0f, 0f));
                    Instantiate(fragmentos, new Vector2(this.transform.position.x, this.transform.position.y+1f), new Quaternion(0f, 0f, 0f, 0f));
                    Invoke("NuevaCestaPelotaAzul", 1f);

                }

                if (this.gameObject.name == "PelotaRosa")
                {
                    DesactivaFisicas();
                    this.gameObject.GetComponent<Renderer>().enabled = false;
                    desactivarColaPelota();

                    Instantiate(particulasRoto1, new Vector2(this.transform.position.x, this.transform.position.y), new Quaternion(0f, 0f, 0f, 0f));
                    Instantiate(particulasRoto2, new Vector2(this.transform.position.x, this.transform.position.y), new Quaternion(0f, 0f, 0f, 0f));
                    Instantiate(fragmentos, new Vector2(this.transform.position.x, this.transform.position.y + 1f), new Quaternion(0f, 0f, 0f, 0f));
                    Invoke("NuevaCestaPelotaRosa", 1f);
                }
            }
        }

        //Cada vez que rebotemos con una parez incrementaremos el multiplicador que se aplicará si encestamos
        if (collision.gameObject.tag == "Pared")
        {
            ++multiplicador;
            Debug.Log("Pared Multplicador " + multiplicador);

            Instantiate(particulasRebote, new Vector2(this.transform.position.x,this.transform.position.y),transform.rotation);
        }

        if (collision.gameObject.tag == "Cesta")
        {
            //Si no tenemos activo el boost multiplicador, calcularemos normal, si no, multiplicaremos por 4 en lugar de dos
            if (!boostMultiplicadorActivo)
            {
                puntuacion = puntuacion + (2 * multiplicador);
            }
            else
            {
                puntuacion = puntuacion + (4 * multiplicador);
                boostMultiplicadorActivo = false;
            }


            //Esperamos 1 segundo antes de llamar a la función correspondiente
            if (this.gameObject.name == "PelotaAzul")
            {
                this.gameObject.GetComponent<Renderer>().enabled = false;
                desactivarColaPelota();
                Invoke("NuevaCestaPelotaAzul", 1f);

            }

            if (this.gameObject.name == "PelotaRosa")
            {
                this.gameObject.GetComponent<Renderer>().enabled = false;
                desactivarColaPelota();
                Invoke("NuevaCestaPelotaRosa", 1f);
            }

            Instantiate(particulas, new Vector2(this.transform.position.x, this.transform.position.y + 1f), transform.rotation);

            DesactivaFisicas();
            SueloTocado(1);
        }
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
       if(collision.tag=="BoostMultPor2")
        {
            Destroy(collision.gameObject);
            boostMultiplicadorRecogido = true;
        }

        if (collision.tag == "BoostDoblePelota")
        {
            Destroy(collision.gameObject);
            boostPelotaRecogido = true;
        }

        //InstanciamosPartículas
        Instantiate(particulasRecogerBoost, new Vector2(collision.transform.position.x, collision.transform.position.y), transform.rotation);
    }

    public void InstanciaYLanzamientoPelotaBoost(Vector2 fuerza)
    {
   
        Instantiate(bp, new Vector2(transform.position.x, transform.position.y + 1.5f), transform.rotation);
        if (this.gameObject.name == "PelotaAzul")
        {
            GameObject b2p = GameObject.Find("PelotaBoostAzul(Clone)");
            b2p.GetComponent<scrPelotaBoost>().Start();
            b2p.GetComponent<scrPelotaBoost>().ActivaFisicas();
            b2p.GetComponent<scrPelotaBoost>().Empuje(fuerza);
            boostPelotaRecogido = false;
        }
        else
        {
            GameObject b2p = GameObject.Find("PelotaBoostRosa(Clone)");
            b2p.GetComponent<scrPelotaBoost>().Start();
            b2p.GetComponent<scrPelotaBoost>().ActivaFisicas();
            b2p.GetComponent<scrPelotaBoost>().Empuje(fuerza);
            boostPelotaRecogido = false;
        }

        pelotaBoostCreada = true;
    }

    public string getTag()
    {
        return this.tag;
    }

}
