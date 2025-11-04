// UiAlphaHitbox.cs (REEMPLAZA ENTERO)
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class UiAlphaHitbox : MonoBehaviour
{
    [Range(0f, 1f)] public float alphaThreshold = 0.2f;

    void Awake()
    {
        var img = GetComponent<Image>();
        var spr = img.sprite;
        if (spr == null)
        {
            Debug.LogWarning($"[UiAlphaHitbox] {name}: Image sin sprite.");
            return;
        }

        var tex = spr.texture;
        if (tex == null)
        {
            Debug.LogWarning($"[UiAlphaHitbox] {name}: Sprite sin texture.");
            return;
        }

        // Chequeos de seguridad para evitar la InvalidOperationException
        bool isReadable = false;
        Texture2D t2d = tex as Texture2D;
        if (t2d != null)
        {
            // isReadable existe en Texture2D
            isReadable = t2d.isReadable;

            // Detectar formatos “crunched”
            bool isCrunched =
                t2d.format == TextureFormat.DXT1Crunched ||
                t2d.format == TextureFormat.DXT5Crunched ||
                t2d.format == TextureFormat.ETC2_RGBA8Crunched;
            if (isCrunched)
            {
                Debug.LogError($"[UiAlphaHitbox] {name}: La textura usa Crunch Compression. Deshabilitala en el Importer.");
                return;
            }
        }

        if (!isReadable)
        {
            Debug.LogError($"[UiAlphaHitbox] {name}: La textura no es Read/Write. Activá Read/Write Enabled en el Importer.");
            return;
        }

        // Configurar el Image para raycast por alfa y malla ajustada
        img.alphaHitTestMinimumThreshold = alphaThreshold;
        img.useSpriteMesh = true;
    }
}
