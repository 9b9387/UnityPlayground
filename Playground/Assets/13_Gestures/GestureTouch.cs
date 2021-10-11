// GestureTouch.cs
// Author: shihongyang shihongyang@weile.com
// Data: 2021/10/8
using System;

namespace Owlet
{
    public class GestureTouch : IComparable<GestureTouch>
    {
        public int id;
        public float previousX;
        public float previousY;
        public float pressure;
        public float screenX;
        public float screenY;
        public object platformSpecificTouch;
        public TouchPhase touchPhase;

        public GestureTouch(int platformSpecificId, float screenX, float screenY, float previousX, float previousY, float pressure, object platformSpecificTouch = null, TouchPhase touchPhase = TouchPhase.Unknown)
        {
            this.id = platformSpecificId;
            this.screenX = screenX;
            this.screenY = screenY;
            this.previousX = previousX;
            this.previousY = previousY;
            this.pressure = pressure;
            this.platformSpecificTouch = platformSpecificTouch;
            this.touchPhase = touchPhase;
        }

        public TouchPhase TouchPhase { get { return touchPhase; } }
        public int Id { get { return id; } }
        public float ScreenX { get { return screenX; } }
        public float X { get { return ScreenX; } }
        public float ScreenY { get { return screenY; } }
        public float Y { get { return ScreenY; } }
        public float Pressure { get { return pressure; } }

        public int CompareTo(GestureTouch other)
        {
            return this.id.CompareTo(other.id);
        }
    }
}