using UnityEditor;
using UnityEngine;


namespace SonaruUtilities
{
    public static class EditorTool
    {
        public static void DrawUILine(Color color = default, int thickness = 1, int padding = 10, int margin = 0)
        {
#if UNITY_EDITOR
            color = color != default ? color : Color.grey;
            var r = EditorGUILayout.GetControlRect(false, GUILayout.Height(padding + thickness));
            r.height = thickness;
            r.y += padding * 0.5f;
 
            switch (margin)
            {
                // expand to maximum width
                case < 0:
                    r.x = 0;
                    r.width = EditorGUIUtility.currentViewWidth;
 
                    break;
                case > 0:
                    // shrink line width
                    r.x += margin;
                    r.width -= margin * 2;
 
                    break;
            }
 
            EditorGUI.DrawRect(r, color);
#endif
        }
    }
}