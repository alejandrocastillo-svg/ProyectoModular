using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class IrisTransicion : MonoBehaviour
{
    [SerializeField] private float duracion = 0.8f;
    [SerializeField] private float radioAbierto = 1.2f;

    private Material material;
    private bool cerrando = false;
    private static readonly int RadioID = Shader.PropertyToID("_Radio");

    void Awake()
    {
        // Instancia del material para no modificar el asset original
        Image imagen = GetComponent<Image>();
        material = new Material(imagen.material);
        imagen.material = material;

        // La escena empieza con el iris totalmente abierto (sin efecto visible)
        material.SetFloat(RadioID, radioAbierto);
    }

    // Llamar a este método cuando el jugador toque la bandera
    public void CambiarEscena(string nombreEscena)
    {
        if (cerrando) return;
        cerrando = true;
        StartCoroutine(CerrarYCargar(nombreEscena));
    }

    private IEnumerator CerrarYCargar(string nombreEscena)
    {
        yield return Animar(radioAbierto, 0f);
        SceneManager.LoadScene(nombreEscena);
    }

    private IEnumerator Animar(float desde, float hasta)
    {
        float t = 0f;
        while (t < duracion)
        {
            t += Time.unscaledDeltaTime;
            float k = Mathf.SmoothStep(0f, 1f, t / duracion);
            material.SetFloat(RadioID, Mathf.Lerp(desde, hasta, k));
            yield return null;
        }
        material.SetFloat(RadioID, hasta);
    }
}