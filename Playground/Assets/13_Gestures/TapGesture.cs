// TapGesture.cs
// Author: shihongyang shihongyang@weile.com
// Data: 2021/10/19
using System;
using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

public class TapGesture : GestureRecognizer
{
    public TapGesture()
    {
    }

    protected override void ProcessTouchBegan(Touch touch)
    {
        state = Owlet.GestureRecognizerState.Began;
    }

    protected override void ProcessTouchesMoved(Touch touch)
    {
        if (Vector2.Distance(touch.startScreenPosition, touch.screenPosition) > 30f)
        {
            Debug.Log("tap gesture failed2.");
            state = Owlet.GestureRecognizerState.Failed;
        }
    }

    protected override void ProcessTouchesEnded(Touch touch)
    {
        if (state == Owlet.GestureRecognizerState.Began)
        {
            if(touch.time - touch.startTime > 0.3f)
            {
                Debug.Log("tap gesture failed1.");
                state = Owlet.GestureRecognizerState.Failed;
            }
            else
            {
                state = Owlet.GestureRecognizerState.Ended;
                Debug.Log("trigger tap gesture.");
            }
        }
    }
}
