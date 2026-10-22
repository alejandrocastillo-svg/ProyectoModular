
using UnityEngine;
using UnityEngine.InputSystem;

public class SaltoJugador : MonoBehaviour
{
    public float fuerzaSalto = 5f;

    private Rigidbody rb;
    private int contadorSaltos = 0;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (Keyboard.current != null &&
            Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            if (contadorSaltos < 2)
            {
                rb.AddForce(
                    Vector3.up * fuerzaSalto, // Modificar la fuerza que se aplica en el segundo salto 
                    ForceMode.Impulse
                );

                contadorSaltos++;
                Debug.Log("Saltos realizados: " + contadorSaltos);
            }
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.name == "Suelo")
        {
            contadorSaltos = 0;
            Debug.Log("Saltos reiniciados");
        }
    }
}
