using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;


public class BoxDrawer : MonoBehaviour
{
    
    private static Texture2D _staticRectTexture;
    private static GUIStyle _staticRectStyle;
    
    void Start()
    {
        
    }

    void Update()
    {
        // GameObject objToSpawn = new GameObject("Cool GameObject made from Code");
        // objToSpawn.AddComponent<UIPolygon>();
    }

    public void OnGUI()
    {
        GUIDrawRect(new Rect(0, 0, 50, 50), Color.black);
    }
    
    private static void GUIDrawRect( Rect position, Color color )
    {
        if( !_staticRectTexture )
        {
            _staticRectTexture = new Texture2D( 1, 1 );
        }
 
        if( _staticRectStyle == null )
        {
            _staticRectStyle = new GUIStyle();
        }
 
        _staticRectTexture.SetPixel( 0, 0, color );
        _staticRectTexture.Apply();
 
        _staticRectStyle.normal.background = _staticRectTexture;
 
        GUI.Box( position, GUIContent.none, _staticRectStyle );
    }
}
