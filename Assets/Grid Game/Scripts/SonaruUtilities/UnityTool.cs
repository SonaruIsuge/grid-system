using System.Collections.Generic;
using UnityEngine;

namespace SonaruUtilities
{
    public static class UnityTool
    {
        public static Rect RectTransformToScreenSpace(RectTransform transform)
        {
            Vector2 size = Vector2.Scale(transform.rect.size, transform.lossyScale);
            Rect rect = new Rect(transform.position.x, Screen.height - transform.position.y, size.x, size.y);
            rect.x -= (transform.pivot.x * size.x);
            rect.y -= ((1.0f - transform.pivot.y) * size.y);
            return rect;
        }


        public static Vector2 GetOrthographicCameraWorldSize(Camera camera)
        {
            if (!camera.orthographic) return Vector2.zero;

            var height = camera.orthographicSize * 2.0f;
            return new Vector2(camera.aspect * height, height);
        }


        public static List<T> GetNoRepeatElement<T>(int requireNum, List<T> fromList)
        {
            var copy = new List<T>(fromList);
            var result = new List<T>();
            
            for (var i = 0; i < requireNum; i++)
            {
                var choose = Random.Range(0, copy.Count);
                result.Add(copy[choose]);
                copy.RemoveAt(choose);
            }

            return result;
        }


        public static void ChangeSecToHMS(float inSec, out int hour, out int minute, out float second)
        {
            hour = 0;
            minute = 0;
            second = 0;
            if(inSec < 0) return;
            
            second = inSec % 60;
            minute = (int)inSec / 60;
            if(minute < 60) return;

            hour = minute / 60;
            minute %= 60;
        }
    }
}