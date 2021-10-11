using System.Collections;
using System.Collections.Generic;
using Owlet;
using UnityEngine;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

public class GesturesRecognizer
{
    protected readonly List<Touch> trackedTouches = new List<Touch>();

    private GestureRecognizerState state = GestureRecognizerState.Possible;

    public GesturesRecognizer()
    {

    }

    public void TrackTouches(IReadOnlyList<Touch> touches)
    {
        for (int i = 0; i < touches.Count; i++)
        {
            var touch = touches[i];
            switch (touch.phase)
            {
                case UnityEngine.InputSystem.TouchPhase.Began:
                    {
                        ProcessTouchBegan(touches);
                        break;
                    }
                case UnityEngine.InputSystem.TouchPhase.Moved:
                case UnityEngine.InputSystem.TouchPhase.Stationary:
                    {
                        ProcessTouchesMoved();
                        break;
                    }
                case UnityEngine.InputSystem.TouchPhase.Ended:
                case UnityEngine.InputSystem.TouchPhase.Canceled:
                    {
                        ProcessTouchesEnded();
                        break;
                    }
                case UnityEngine.InputSystem.TouchPhase.None:
                default: break;
            }
        }
    }

    protected virtual void ProcessTouchBegan(IReadOnlyList<Touch> touches)
    {
        foreach (var touch in touches)
        {
            if(state == GestureRecognizerState.Possible && trackedTouches.Contains(touch) == false)
            {
                trackedTouches.Add(touch);
            }
        }
    }

    protected virtual void ProcessTouchesMoved()
    {

    }

    protected virtual void ProcessTouchesEnded()
    {
        if(trackedTouches.Count == 0)
        {
            SetState(GestureRecognizerState.Ended);
            return;
        }

        SetState(GestureRecognizerState.Failed);
    }

    protected void SetState(GestureRecognizerState value)
    {
        if (value == GestureRecognizerState.Failed)
        {
            FailGestureNow();
            return;
        }
    }
}
