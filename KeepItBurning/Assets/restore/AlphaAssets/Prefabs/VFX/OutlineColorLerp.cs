using UnityEngine;

public class OutlineColorLerp : MonoBehaviour
{
    public Material outlineMaterial;        // Shader Graph ile yaptığın outline materyali
    public Color colorA = Color.yellow;     // İlk renk
    public Color colorB = Color.red;        // İkinci renk
    public float speed = 2f;                // Geçiş hızı

    private void Update()
    {
        if (outlineMaterial != null)
        {
            // Renkleri sinüs dalgası ile yumuşak biçimde değiştir
            float t = (Mathf.Sin(Time.time * speed) + 1f) / 2f;
            Color lerpedColor = Color.Lerp(colorA, colorB, t);

            // Shader Graph içinde "_outlineColor" parametresini değiştir
            outlineMaterial.SetColor("_outline_Color", lerpedColor);
        }
    }
}
