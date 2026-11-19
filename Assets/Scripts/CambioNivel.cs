using UnityEngine;

public class CambioNivel : MonoBehaviour
{
    public string nombreEscena = "Nivel2";
    public IrisTransicion iris;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            iris.CambiarEscena(nombreEscena);
        }
    }
}