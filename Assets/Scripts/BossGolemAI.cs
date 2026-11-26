
using UnityEngine;
using System.Collections;

public class BossGolemAI : MonoBehaviour
{
    [Header("Jugador")]
    public Transform jugador;

    [Header("Movimiento")]
    public float velocidad = 3f;
    public float distanciaAtaque = 4f;

    [Header("Doble machetazo")]
    public float velocidadMachetazo = 9f;
    public float duracionMachetazo = 0.35f;
    public float pausaEntreMachetazos = 0.3f;

    [Header("Salto")]
    public float fuerzaSalto = 12f;
    public float velocidadSaltoHorizontal = 7f;

    [Header("Carga giratoria")]
    public float velocidadCarga = 14f;
    public float duracionCarga = 0.6f;
    public float tiempoPreparacionCarga = 1f;

    [Header("Recuperacion")]
    public float recuperacionNormal = 1.5f;
    public float recuperacionCarga = 3f;

    [Header("Vida del Boss")]
    public float vidaMaxima = 100f;
    private float vidaActual;

    [Header("Generacion de enemigos")]
    public GameObject prefabEnemigo;
    public int enemigosPorTanda = 2;
    public int maximoEnemigosGenerados = 8;
    public float distanciaGeneracion = 3f;

    private int enemigosGenerados = 0;
    private int tandasGeneradas = 0;

    private Rigidbody rb;
    private bool atacando = false;
    private bool enSuelo = true;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        vidaActual = vidaMaxima;
    }

    void FixedUpdate()
    {
        if (jugador == null || atacando)
        {
            return;
        }

        float distancia = Mathf.Abs(
            jugador.position.x - transform.position.x
        );

        if (distancia > distanciaAtaque)
        {
            PerseguirJugador();
        }
        else
        {
            rb.linearVelocity = new Vector3(
                0f,
                rb.linearVelocity.y,
                0f
            );

            StartCoroutine(SeleccionarAtaque());
        }
    }

    void PerseguirJugador()
    {
        float direccion = Mathf.Sign(
            jugador.position.x - transform.position.x
        );

        rb.linearVelocity = new Vector3(
            direccion * velocidad,
            rb.linearVelocity.y,
            0f
        );
    }

    public void RecibirDanio(float cantidad)
{
    vidaActual -= cantidad;

    vidaActual = Mathf.Max(vidaActual, 0f);

    Debug.Log("Vida del Boss: " + vidaActual);

    ComprobarGeneracion();

    if (vidaActual <= 0f)
    {
        Debug.Log("Boss derrotado");
    }
}

    void ComprobarGeneracion()
    {
        float porcentajeVida = vidaActual / vidaMaxima;

        if (tandasGeneradas == 0 && porcentajeVida <= 0.75f)
        {
            GenerarTanda();
        }
        else if (tandasGeneradas == 1 && porcentajeVida <= 0.50f)
        {
            GenerarTanda();
        }
        else if (tandasGeneradas == 2 && porcentajeVida <= 0.25f)
        {
            GenerarTanda();
        }
        else if (tandasGeneradas == 3 && porcentajeVida <= 0.10f)
        {
            GenerarTanda();
        }
    }

    void GenerarTanda()
    {
        if (prefabEnemigo == null)
        {
            Debug.LogWarning("No se ha asignado el Prefab del enemigo");
            return;
        }

        for (int i = 0; i < enemigosPorTanda; i++)
        {
            if (enemigosGenerados >= maximoEnemigosGenerados)
            {
                return;
            }

            float direccion = (i == 0) ? -1f : 1f;

            Vector3 posicion = transform.position + new Vector3(
                direccion * distanciaGeneracion,
                0f,
                0f
            );

            Instantiate(
                prefabEnemigo,
                posicion,
                Quaternion.identity
            );

            enemigosGenerados++;
        }

        tandasGeneradas++;

        Debug.Log("El Boss ha generado una tanda de enemigos");
    }

    IEnumerator SeleccionarAtaque()
    {
        atacando = true;

        rb.linearVelocity = Vector3.zero;

        yield return new WaitForSeconds(1f);

        int ataque = Random.Range(0, 3);

        if (ataque == 0)
        {
            yield return StartCoroutine(DobleMachetazo());

            yield return new WaitForSeconds(recuperacionNormal);
        }
        else if (ataque == 1)
        {
            yield return StartCoroutine(AtaqueSalto());

            yield return new WaitForSeconds(recuperacionNormal);
        }
        else
        {
            yield return StartCoroutine(CargaGiratoria());

            yield return new WaitForSeconds(recuperacionCarga);
        }

        atacando = false;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Suelo"))
        {
            enSuelo = true;
        }
    }

    void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Suelo"))
        {
            enSuelo = true;
        }
    }

    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Suelo"))
        {
            enSuelo = false;
        }
    }

    IEnumerator DobleMachetazo()
    {
        for (int i = 0; i < 2; i++)
        {
            float direccion = Mathf.Sign(
                jugador.position.x - transform.position.x
            );

            float tiempo = 0f;

            while (tiempo < duracionMachetazo)
            {
                rb.linearVelocity = new Vector3(
                    direccion * velocidadMachetazo,
                    rb.linearVelocity.y,
                    0f
                );

                tiempo += Time.fixedDeltaTime;

                yield return new WaitForFixedUpdate();
            }

            rb.linearVelocity = new Vector3(
                0f,
                rb.linearVelocity.y,
                0f
            );

            if (i == 0)
            {
                yield return new WaitForSeconds(pausaEntreMachetazos);
            }
        }
    }

    IEnumerator AtaqueSalto()
    {
        float direccion = Mathf.Sign(
            jugador.position.x - transform.position.x
        );

        rb.linearVelocity = new Vector3(
            direccion * velocidadSaltoHorizontal,
            fuerzaSalto,
            0f
        );

        enSuelo = false;

        while (!enSuelo)
        {
            yield return null;
        }

        rb.linearVelocity = Vector3.zero;
    }

    IEnumerator CargaGiratoria()
    {
        rb.linearVelocity = Vector3.zero;

        yield return new WaitForSeconds(tiempoPreparacionCarga);

        float direccion = Mathf.Sign(
            jugador.position.x - transform.position.x
        );

        float tiempo = 0f;

        while (tiempo < duracionCarga)
        {
            rb.linearVelocity = new Vector3(
                direccion * velocidadCarga,
                rb.linearVelocity.y,
                0f
            );

            tiempo += Time.fixedDeltaTime;

            yield return new WaitForFixedUpdate();
        }

        rb.linearVelocity = Vector3.zero;
    }
}
