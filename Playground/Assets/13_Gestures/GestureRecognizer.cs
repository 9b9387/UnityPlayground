using System.Collections;
using System.Collections.Generic;
using Owlet;
using UnityEngine;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

public class GestureRecognizer
{
    protected readonly List<Touch> trackedTouches = new List<Touch>();
    private readonly HashSet<GestureRecognizer> failGestures = new HashSet<GestureRecognizer>();
    private readonly HashSet<GestureRecognizer> requireGestureRecognizersToFailThatHaveFailed = new HashSet<GestureRecognizer>();
    private readonly HashSet<GestureRecognizer> requireGestureRecognizersToFail = new HashSet<GestureRecognizer>();

    private GestureRecognizerState state = GestureRecognizerState.Possible;

    public GestureRecognizer()
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
            FailGesture();
            return;
        }
    }

    private bool HasAllRequiredFailGesturesToEndFromEndPending()
    {
        return requireGestureRecognizersToFail.SetEquals(requireGestureRecognizersToFailThatHaveFailed);
    }

    private void FailGesture()
    {
        state = GestureRecognizerState.Failed;
        StateChanged();
        foreach (GestureRecognizer gesture in failGestures)
        {
            gesture.requireGestureRecognizersToFailThatHaveFailed.Add(this);
            if (gesture.state == GestureRecognizerState.EndPending)
            {
                if (gesture.HasAllRequiredFailGesturesToEndFromEndPending())
                {
                    gesture.SetState(GestureRecognizerState.Ended);
                }
            }
        }
    }

    public virtual void Reset()
    {
        ResetInternal(true);
    }

    private void ResetInternal(bool clearCurrentTrackedTouches)
    {
        //if (clearCurrentTrackedTouches)
        //{
        //    currentTrackedTouches.Clear();
        //}
        requireGestureRecognizersToFailThatHaveFailed.Clear();
        //touchStartLocations.Clear();
        //StartFocusX = PrevFocusX = StartFocusY = PrevFocusY = float.MinValue;
        //FocusX = FocusY = DeltaX = DeltaY = DistanceX = DistanceY = 0.0f;
        //Pressure = 0.0f;
        //velocityTracker.Reset();
        //RemoveFromActiveGestures();
        SetState(GestureRecognizerState.Possible);
    }

    protected virtual void StateChanged()
    {

    }
}
