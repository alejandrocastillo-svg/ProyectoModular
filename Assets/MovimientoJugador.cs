
using UnityEngine;
using UnityEngine.InputSystem;

public class MovimientoJugador : MonoBehaviour
{
    public float velocidadMovimiento = 5f;

    void Update()
    {
        if (Keyboard.current == null)
        {
            return;
        }

        if (Keyboard.current.aKey.isPressed)
        {
            transform.Translate(
                Vector3.left * velocidadMovimiento * Time.deltaTime
            );
        }

        if (Keyboard.current.dKey.isPressed)
        {
            transform.Translate(
                Vector3.right * velocidadMovimiento * Time.deltaTime
            );
        }
    }
}
