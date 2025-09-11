using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorChange : MonoBehaviour
{
    public void CursorSpriteChange (Texture2D sprite, Vector2 hotspot)
    {
        Cursor.SetCursor(sprite, hotspot, CursorMode.Auto);
    }
}
