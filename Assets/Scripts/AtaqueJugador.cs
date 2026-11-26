
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class AtaqueJugador : MonoBehaviour
{
    [Header("Ataque rápido")]
    public int danioRapido = 1;
    public float cadenciaRapida = 0.3f;

    [Header("Ataque con carga")]
    public int danioCarga = 3;
    public int pulsacionesNecesarias = 3;
    public float ventanaEntrePulsaciones = 0.5f;
    public float cooldownCarga = 3f;

    [Header("Alcance")]
    public float alcance = 2f;

    private Collider col;
    private float siguienteRapido = 0f;
    private float siguienteCarga = 0f;
    private float direccion = 1f;
    private int contadorPulsaciones = 0;
    private float ultimaPulsacion = -10f;

    AnimacionPersonaje animController; 

    void Start()
    {
        col = GetComponent<Collider>();
        animController = this.gameObject.GetComponent<AnimacionPersonaje>();
    }

    void Update()
    {
        if (Keyboard.current == null)
        {
            return;
        }

        if (Keyboard.current.aKey.isPressed)
        {
            direccion = -1f;
        }

        if (Keyboard.current.dKey.isPressed)
        {
            direccion = 1f;
        }

        if (Keyboard.current.backspaceKey.wasPressedThisFrame)
        {
            AtaqueRapido();
        }

        if (Keyboard.current.enterKey.wasPressedThisFrame ||
            Keyboard.current.numpadEnterKey.wasPressedThisFrame)
        {
            if (Time.time - ultimaPulsacion <= ventanaEntrePulsaciones)
            {
                contadorPulsaciones++;
            }
            else
            {
                contadorPulsaciones = 1;
            }

            ultimaPulsacion = Time.time;
            Debug.Log("Pulsación Enter: " + contadorPulsaciones);

            if (contadorPulsaciones >= pulsacionesNecesarias)
            {
                contadorPulsaciones = 0;
                AtaqueConCarga();
            }
        }
    }

    void AtaqueRapido()
    {
        if (Time.time < siguienteRapido)
        {
            return;
        }

        siguienteRapido = Time.time + cadenciaRapida;
        Debug.Log("Ataque rápido");

        Golpear(danioRapido);
    }

    void AtaqueConCarga()
    {
        animController.AnimationStatesManager("enterKey");
        if (Time.time < siguienteCarga)
        {
            Debug.Log("Carga en cooldown");
            return;
        }

        siguienteCarga = Time.time + cooldownCarga;
        Debug.Log("Ataque con carga");

        Golpear(danioCarga);
    }

    void Golpear(int danio)
    {
        Collider[] golpeados = Physics.OverlapBox(
            CentroGolpe(),
            MitadGolpe()
        );

        HashSet<VidaEnemigo> yaGolpeados = new HashSet<VidaEnemigo>();
        HashSet<BossGolemAI> bossesGolpeados = new HashSet<BossGolemAI>();

        foreach (Collider c in golpeados)
        {
            VidaEnemigo vida = c.GetComponentInParent<VidaEnemigo>();

            if (vida != null && yaGolpeados.Add(vida))
            {
                vida.RecibirDanio(danio);
            }

            BossGolemAI boss = c.GetComponentInParent<BossGolemAI>();

            if (boss != null && bossesGolpeados.Add(boss))
            {
                boss.RecibirDanio(danio);
            }
        }
    }

    Vector3 CentroGolpe()
    {
        Bounds b = col.bounds;

        return b.center + Vector3.right * direccion * (b.extents.x + alcance / 2f);
    }

    Vector3 MitadGolpe()
    {
        return new Vector3(
            alcance / 2f,
            col.bounds.extents.y,
            1.5f
        );
    }

    void OnDrawGizmosSelected()
    {
        if (col == null)
        {
            col = GetComponent<Collider>();
        }

        if (col == null)
        {
            return;
        }

        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(CentroGolpe(), MitadGolpe() * 2f);
    }
}
