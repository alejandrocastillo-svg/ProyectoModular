using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform jugador;

    public Vector3 desplazamiento;

    void LateUpdate()
    {
        if (jugador != null)
        {
            transform.position = jugador.position + desplazamiento;
        }
    }
}