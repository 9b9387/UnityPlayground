// TapGestureRecognizer.cs
// Author: shihongyang shihongyang@weile.com
// Data: 2021/10/9
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Owlet
{ 
    public class TapGestureRecognizer : GestureRecognizer
    {
        public TapGestureRecognizer()
        {
        }

        protected override void TouchesBegan(IEnumerable<GestureTouch> touches)
        {
            base.TouchesBegan(touches);
            Debug.Log("TouchesBegan");
            SetState(GestureRecognizerState.Began);

        }

        protected override void TouchesEnded()
        {
            base.TouchesEnded();
            Debug.Log("TouchesEnded");
        }

        protected override void TouchesMoved()
        {
            base.TouchesMoved();
            Debug.Log("TouchesMoved");
        }
    }
}