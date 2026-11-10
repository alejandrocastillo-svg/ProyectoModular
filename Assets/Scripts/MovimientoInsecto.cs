
using UnityEngine;

public class MovimientoInsecto : MonoBehaviour
{
    public Transform jugador;

    public float velocidad = 10f;
    public float velocidadPatrulla = 2f;

    public float distanciaDeteccion = 50f;
    public float distanciaPatrulla = 5f;

    public float alturaSobreJugador = 0.7f;
    public float alturaExtraMaxima = 2f;
    public float distanciaParaAlturaMaxima = 20f;

    public float alturaMinimaSuelo = 1.5f;

    private Rigidbody rb;

    private Vector3 posicionInicial;

    private float direccionPatrulla = 1f;

    private bool persiguiendo = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        posicionInicial = transform.position;
    }

    void FixedUpdate()
    {
        if (jugador == null)
        {
            return;
        }

        // Calcular distancia horizontal
        Vector3 posicionJugador = jugador.position;
        Vector3 posicionInsecto = transform.position;

        posicionJugador.y = 0f;
        posicionInsecto.y = 0f;

        float distancia = Vector3.Distance(
            posicionInsecto,
            posicionJugador
        );

        // Detectar al jugador
        if (!persiguiendo && distancia < distanciaDeteccion)
        {
            persiguiendo = true;
        }

        if (persiguiendo)
        {
            // Calcular altura según la distancia al jugador
            float porcentajeDistancia = Mathf.Clamp01(
                distancia / distanciaParaAlturaMaxima
            );

            float alturaExtra = porcentajeDistancia * alturaExtraMaxima;

            float alturaObjetivo = jugador.position.y
                + alturaSobreJugador
                + alturaExtra;

            // Evitar que el insecto baje demasiado
            alturaObjetivo = Mathf.Max(
                alturaObjetivo,
                alturaMinimaSuelo
            );

            // Crear posición objetivo
            Vector3 objetivo = new Vector3(
                jugador.position.x,
                alturaObjetivo,
                jugador.position.z
            );

            // Perseguir al jugador
            Vector3 direccion = (objetivo - transform.position).normalized;

            rb.linearVelocity = direccion * velocidad;
        }
        else
        {
            // Patrullar de izquierda a derecha
            if (transform.position.x > posicionInicial.x + distanciaPatrulla)
            {
                direccionPatrulla = -1f;
            }

            if (transform.position.x < posicionInicial.x - distanciaPatrulla)
            {
                direccionPatrulla = 1f;
            }

            rb.linearVelocity = new Vector3(
                direccionPatrulla * velocidadPatrulla,
                0f,
                0f
            );
        }
    }
}
