using System.Collections.Generic;
using Owlet;
using UnityEngine;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

public class GestureRecognizer : ScriptableObject
{
    protected readonly List<int> trackedTouches = new List<int>();
    private readonly HashSet<GestureRecognizer> failGestures = new HashSet<GestureRecognizer>();
    private readonly HashSet<GestureRecognizer> requireGestureRecognizersToFailThatHaveFailed = new HashSet<GestureRecognizer>();
    private readonly HashSet<GestureRecognizer> requireGestureRecognizersToFail = new HashSet<GestureRecognizer>();

    protected GestureState state;

    public int minRequireTouchCount = 1;
    public int maxRequireTouchCount = 1;

    public GestureRecognizer()
    {
        state = new GestureState();
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
                    if (state.IsTouchState(TouchState.Untouched)
                        && trackedTouches.Contains(touch.touchId) == false)
                    {
                        trackedTouches.Add(touch.touchId);
                        ProcessTouchBegan(touch);
                    }
                    break;
                }
                case UnityEngine.InputSystem.TouchPhase.Moved:
                case UnityEngine.InputSystem.TouchPhase.Stationary:
                {
                    if (trackedTouches.Contains(touch.touchId))
                    {
                        ProcessTouchesMoved(touch);
                    }
                    break;
                }
                case UnityEngine.InputSystem.TouchPhase.Ended:
                case UnityEngine.InputSystem.TouchPhase.Canceled:
                {
                    if (trackedTouches.Contains(touch.touchId))
                    {
                        ProcessTouchesEnded(touch);
                        trackedTouches.Remove(touch.touchId);
                        if(trackedTouches.Count == 0)
                        {
                            Reset();
                        }
                    }
                    break;
                }
                case UnityEngine.InputSystem.TouchPhase.None:
                default: break;
            }
        }
    }

    protected virtual void ProcessTouchBegan(Touch touch)
    {
        //Debug.Log($"ProcessTouchBegan {state} {touch.touchId}");
    }

    protected virtual void ProcessTouchesMoved(Touch touch)
    {
        //Debug.Log($"ProcessTouchesMoved {state} {touch.touchId}");
    }

    protected virtual void ProcessTouchesEnded(Touch touch)
    {
        //Debug.Log($"ProcessTouchesEnded {state} {touch.touchId}");
    }

    public virtual void OnUpdate(float delta)
    {

    }

    //protected void SetState(GestureRecognizerState value)
    //{
    //    if (value == GestureRecognizerState.Failed)
    //    {
    //        FailGesture();
    //        return;
    //    }

    //    if(value == GestureRecognizerState.Ended)
    //    {
    //        EndGesture();
    //        return;
    //    }

    //    state = value;
    //}

    //private bool HasAllRequiredFailGesturesToEndFromEndPending()
    //{
    //    return requireGestureRecognizersToFail.SetEquals(requireGestureRecognizersToFailThatHaveFailed);
    //}

    //protected void FailGesture()
    //{
    //    state = GestureRecognizerState.Failed;
    //    StateChanged();
    //    foreach (GestureRecognizer gesture in failGestures)
    //    {
    //        gesture.requireGestureRecognizersToFailThatHaveFailed.Add(this);
    //        if (gesture.state == GestureRecognizerState.EndPending)
    //        {
    //            if (gesture.HasAllRequiredFailGesturesToEndFromEndPending())
    //            {
    //                gesture.SetState(GestureRecognizerState.Ended);
    //            }
    //        }
    //    }
    //}

    //protected void EndGesture()
    //{

    //    Debug.Log("EndGesture");
    //    state = GestureRecognizerState.Ended;
    //    StateChanged();

    //    SetState(GestureRecognizerState.Possible);
    //}

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
        //SetState(GestureRecognizerState.Possible);
        state.Reset();
    }

    protected virtual void StateChanged()
    {

    }
}
