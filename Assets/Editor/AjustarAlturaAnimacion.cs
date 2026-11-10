using System.Linq;
using UnityEditor;
using UnityEngine;

// Colocar en Assets/Editor/AjustarAlturaAnimacion.cs
// Crea una copia editable de un clip y sube la curva de altura (Y) del hueso raiz.
public class AjustarAlturaAnimacion : EditorWindow
{
    AnimationClip origen;
    float offsetY = 0.3f;
    float inicioRampa = 0f;   // segundos. Antes de este tiempo no se aplica offset
    float finRampa = 0f;      // segundos. Desde este tiempo se aplica el offset completo
    string carpetaSalida = "Assets/Animations";

    [MenuItem("Tools/Ajustar altura de animacion")]
    static void Abrir()
    {
        GetWindow<AjustarAlturaAnimacion>("Ajustar altura");
    }

    void OnEnable()
    {
        UsarSeleccion();
    }

    void UsarSeleccion()
    {
        if (Selection.activeObject is AnimationClip clip)
        {
            origen = clip;
            return;
        }

        if (Selection.activeObject != null)
        {
            string ruta = AssetDatabase.GetAssetPath(Selection.activeObject);
            origen = AssetDatabase.LoadAllAssetsAtPath(ruta)
                .OfType<AnimationClip>()
                .FirstOrDefault(c => !c.name.StartsWith("__preview__"));
        }
    }

    void OnGUI()
    {
        EditorGUILayout.Space();
        origen = (AnimationClip)EditorGUILayout.ObjectField("Clip origen", origen, typeof(AnimationClip), false);
        if (GUILayout.Button("Usar clip seleccionado en Project")) UsarSeleccion();

        EditorGUILayout.Space();
        offsetY = EditorGUILayout.FloatField("Offset Y", offsetY);
        inicioRampa = EditorGUILayout.FloatField("Inicio de rampa (s)", inicioRampa);
        finRampa = EditorGUILayout.FloatField("Fin de rampa (s)", finRampa);
        EditorGUILayout.HelpBox(
            "Con inicio y fin en 0, el offset se aplica a todo el clip.\n" +
            "Para no elevar la pose inicial: inicio = momento en que empieza a caer, fin = momento en que ya esta en el suelo.",
            MessageType.Info);

        carpetaSalida = EditorGUILayout.TextField("Carpeta de salida", carpetaSalida);

        EditorGUILayout.Space();
        GUI.enabled = origen != null;
        if (GUILayout.Button("Crear copia con la altura ajustada")) Aplicar();
        GUI.enabled = true;
    }

    float Peso(float t)
    {
        if (finRampa <= inicioRampa) return 1f;
        return Mathf.Clamp01((t - inicioRampa) / (finRampa - inicioRampa));
    }

    static bool EsAlturaRaiz(EditorCurveBinding b)
    {
        // Generic: posicion local Y de un hueso. Humanoid: altura del root.
        return (b.type == typeof(Transform) && b.propertyName == "m_LocalPosition.y")
            || b.propertyName == "RootT.y";
    }

    void Aplicar()
    {
        var bindings = AnimationUtility.GetCurveBindings(origen);
        if (!bindings.Any(EsAlturaRaiz))
        {
            Debug.LogWarning("No se encontro ninguna curva de altura en el clip. Curvas disponibles:\n" +
                string.Join("\n", bindings.Select(b => b.path + " / " + b.propertyName)));
            return;
        }

        var copia = new AnimationClip
        {
            frameRate = origen.frameRate,
            wrapMode = origen.wrapMode
        };

        int ajustadas = 0;
        foreach (var b in bindings)
        {
            AnimationCurve curva = AnimationUtility.GetEditorCurve(origen, b);

            if (EsAlturaRaiz(b))
            {
                Keyframe[] keys = curva.keys;
                for (int i = 0; i < keys.Length; i++)
                    keys[i].value += offsetY * Peso(keys[i].time);
                curva.keys = keys;
                ajustadas++;
                Debug.Log("Curva ajustada: " + b.path + " / " + b.propertyName);
            }

            AnimationUtility.SetEditorCurve(copia, b, curva);
        }

        AnimationUtility.SetAnimationClipSettings(copia, AnimationUtility.GetAnimationClipSettings(origen));
        AnimationUtility.SetAnimationEvents(copia, AnimationUtility.GetAnimationEvents(origen));

        if (!AssetDatabase.IsValidFolder(carpetaSalida))
            carpetaSalida = "Assets";

        string ruta = AssetDatabase.GenerateUniqueAssetPath(carpetaSalida + "/" + origen.name + "_Ajustado.anim");
        AssetDatabase.CreateAsset(copia, ruta);
        AssetDatabase.SaveAssets();

        Selection.activeObject = copia;
        EditorGUIUtility.PingObject(copia);
        Debug.Log("Clip creado: " + ruta + " (" + ajustadas + " curva/s de altura ajustada/s)");
    }
}
