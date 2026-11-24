using UnityEngine;

public class VidaEnemigo : MonoBehaviour
{
    public int vidaMaxima = 3;

    private int vidaActual;

    void Awake()
    {
        vidaActual = vidaMaxima;
    }

    public void RecibirDanio(int cantidad)
    {
        vidaActual -= cantidad;
        Debug.Log(gameObject.name + " vida: " + vidaActual);

        if (vidaActual <= 0)
        {
            Destroy(gameObject);
        }
    }
}