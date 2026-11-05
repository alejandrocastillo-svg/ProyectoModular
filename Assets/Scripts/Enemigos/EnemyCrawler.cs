
using UnityEngine;

public class EnemyCrawler : MonoBehaviour
{
    public float velocidad = 2f;

    public Transform jugador;

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        if (jugador == null)
        {
            return;
        }

        float diferenciaX = jugador.position.x - transform.position.x;

        float direccion = Mathf.Sign(diferenciaX);

        if (Mathf.Abs(diferenciaX) < 0.1f)
        {
            rb.linearVelocity = new Vector3(0f, rb.linearVelocity.y, 0f);
            return;
        }

        rb.linearVelocity = new Vector3(
            direccion * velocidad,
            rb.linearVelocity.y,
            0f
        );
    }
}
