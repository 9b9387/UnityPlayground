// GestureRecognizer.cs
// Author: shihongyang shihongyang@weile.com
// Data: 2021/10/8
using System;
using System.Collections.Generic;

namespace Owlet
{
    public class GestureRecognizer : IDisposable
    {
        private GestureRecognizerState state = GestureRecognizerState.Possible;
        public GestureRecognizerState State { get { return state; } }
        private readonly List<GestureTouch> currentTrackedTouches = new List<GestureTouch>();
        public float FocusX { get; private set; }
        public float FocusY { get; private set; }


        private bool justFailed;
        private bool justEnded;
        private bool enabled = true;
        private int maximumNumberOfTouchesToTrack = 1;
        private int minimumNumberOfTouchesToTrack = 1;
        private int lastTrackTouchCount;
        private bool isRestarting;

        private readonly System.Collections.ObjectModel.ReadOnlyCollection<GestureTouch> currentTrackedTouchesReadOnly;
        public System.Collections.ObjectModel.ReadOnlyCollection<GestureTouch> CurrentTrackedTouches { get { return currentTrackedTouchesReadOnly; } }

        public delegate void GestureRecognizerStateUpdatedDelegate(GestureRecognizer gesture);
        public event GestureRecognizerStateUpdatedDelegate StateUpdated;
        private readonly HashSet<GestureRecognizer> requireGestureRecognizersToFailThatHaveFailed = new HashSet<GestureRecognizer>();

        internal static readonly HashSet<GestureRecognizer> ActiveGestures = new HashSet<GestureRecognizer>();
        private readonly HashSet<GestureRecognizer> failGestures = new HashSet<GestureRecognizer>();
        private readonly HashSet<GestureRecognizer> requireGestureRecognizersToFail = new HashSet<GestureRecognizer>();
        public bool ClearTrackedTouchesOnEndOrFail { get; set; }
        public bool ReceivedAdditionalTouches { get; set; }
        private readonly List<GestureTouch> tempTouches = new List<GestureTouch>();
        private readonly List<KeyValuePair<float, float>> touchStartLocations = new List<KeyValuePair<float, float>>();
        private readonly HashSet<int> ignoreTouchIds = new HashSet<int>();

        public GestureRecognizer()
        {
            state = GestureRecognizerState.Possible;
            //PlatformSpecificViewScale = 1.0f;
            //StartFocusX = StartFocusY = float.MinValue;
            currentTrackedTouchesReadOnly = new System.Collections.ObjectModel.ReadOnlyCollection<GestureTouch>(currentTrackedTouches);
            //AllowSimultaneousExecutionIfPlatformSpecificViewsAreDifferent = true;
        }

        ~GestureRecognizer()
        {
            Dispose();
        }

        public virtual void Dispose()
        {
        }

        public bool Enabled
        {
            get { return enabled; }
            set
            {
                enabled = value;
                Reset();
            }
        }

        public virtual void Reset()
        {
            ResetInternal(true);
        }

        private void ResetInternal(bool clearCurrentTrackedTouches)
        {
            if (clearCurrentTrackedTouches)
            {
                currentTrackedTouches.Clear();
            }
            //requireGestureRecognizersToFailThatHaveFailed.Clear();
            //touchStartLocations.Clear();
            //StartFocusX = PrevFocusX = StartFocusY = PrevFocusY = float.MinValue;
            //FocusX = FocusY = DeltaX = DeltaY = DistanceX = DistanceY = 0.0f;
            //Pressure = 0.0f;
            //velocityTracker.Reset();
            //RemoveFromActiveGestures();
            //SetState(GestureRecognizerState.Possible);
        }

        public int MaximumNumberOfTouchesToTrack
        {
            get { return maximumNumberOfTouchesToTrack; }
            set
            {
                maximumNumberOfTouchesToTrack = (value < 1 ? 1 : value);
                if (maximumNumberOfTouchesToTrack < minimumNumberOfTouchesToTrack)
                {
                    minimumNumberOfTouchesToTrack = maximumNumberOfTouchesToTrack;
                }
            }
        }

        public void ProcessTouchesBegan(ICollection<GestureTouch> touches)
        {
            justFailed = false;
            justEnded = false;

            if (!Enabled || touches == null || touches.Count == 0)
            {
                return;
            }
            // if the gesture is possible (hasn't started executing) try to track the touches
            else if ((State == GestureRecognizerState.Possible || State == GestureRecognizerState.Began || State == GestureRecognizerState.Executing)
                && TrackTouches(touches) > 0)
            {
                if (CurrentTrackedTouches.Count > MaximumNumberOfTouchesToTrack)
                {
                    SetState(GestureRecognizerState.Failed);
                }
                else
                {
                    TouchesBegan(touches);
                }
            }
        }

        protected int TrackTouches(IEnumerable<GestureTouch> touches)
        {
            return TrackTouchesInternal(touches);
        }

        private int TrackTouchesInternal(IEnumerable<GestureTouch> touches)
        {
            int count = 0;
            foreach (GestureTouch touch in touches)
            {
                // always track all touches in possible state, allows failing gesture if too many touches
                // do not track higher than the max touch count if in another state
                if ((State == GestureRecognizerState.Possible || currentTrackedTouches.Count < MaximumNumberOfTouchesToTrack) &&
                    !currentTrackedTouches.Contains(touch))
                {
                    currentTrackedTouches.Add(touch);
                    count++;
                }
            }
            if (currentTrackedTouches.Count > 1)
            {
                currentTrackedTouches.Sort();
            }
            return count;
        }

        private bool RequiredGesturesToFailAllowsEndPending()
        {
            if (requireGestureRecognizersToFail.Count > 0)
            {
                using (HashSet<GestureRecognizer>.Enumerator gestureToFailEnumerator = requireGestureRecognizersToFail.GetEnumerator())
                {
                    while (gestureToFailEnumerator.MoveNext())
                    {
                        // if the require fail gesture is possible and
                        // the require fail gesture has touches or just ended and
                        // the require fail gesture has not jus failed
                        // then requre end pending state for failed gesture check
                        bool isPossible = gestureToFailEnumerator.Current.State == GestureRecognizerState.Possible ||
                            gestureToFailEnumerator.Current.State == GestureRecognizerState.Began ||
                            gestureToFailEnumerator.Current.State == GestureRecognizerState.Executing;
                        bool isTrackingTouches = gestureToFailEnumerator.Current.CurrentTrackedTouches.Count != 0;
                        bool justEnded = gestureToFailEnumerator.Current.justEnded;
                        bool justFailed = gestureToFailEnumerator.Current.justFailed;
                        bool requireEndPending = isPossible && (isTrackingTouches || justEnded) && !justFailed;
                        if (requireEndPending)
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        protected bool SetState(GestureRecognizerState value)
        {
            // this.Log("To state: " + value + ": " + this.ToString());

            if (value == GestureRecognizerState.Failed)
            {
                FailGestureNow();
                return true;
            }
            // if we are trying to execute from a non-executing state and there are gestures already executing,
            // we need to make sure we are allowed to execute simultaneously
            //else if (!CanExecuteGestureWithOtherGesturesOrFail(value))
            //{
            //    //this.Log("Failed to execute simultaneously");
            //    return false;
            //}
            else if
            (
                value == GestureRecognizerState.Ended && RequiredGesturesToFailAllowsEndPending()
            )
            {
                // this.Log("END PENDING: " + this);

                // the other gesture will end the state when it fails, or fail this gesture if it executes
                state = GestureRecognizerState.EndPending;
                return false;
            }
            else
            {
                if (value == GestureRecognizerState.Began || value == GestureRecognizerState.Executing)
                {
                    state = value;
                    ActiveGestures.Add(this);
                    UpdateTouchState(value == GestureRecognizerState.Executing);
                    StateChanged();
                }
                else if (value == GestureRecognizerState.Ended)
                {
                    EndGesture();

                    // end after a one frame delay, this allows multiple gestures to properly
                    // fail if no simulatenous execution allowed and there were multiple ending at the same frame
                    ActiveGestures.Add(this);
                    RunActionAfterDelay(0.001f, RemoveFromActiveGestures);
                }
                else
                {
                    state = value;
                    StateChanged();
                }
            }

            return true;
        }

        public static void RunActionAfterDelay(float seconds, Action action)
        {
            //RunActionAfterDelayInternal(seconds, action);
        }

        public virtual bool ResetOnEnd { get { return true; } }


        private void EndGesture()
        {
            state = GestureRecognizerState.Ended;
            ReceivedAdditionalTouches = false;
            lastTrackTouchCount = 0;
            StateChanged();
            if (ResetOnEnd)
            {
                ResetInternal(ClearTrackedTouchesOnEndOrFail);
            }
            else
            {
                SetState(GestureRecognizerState.Possible);
                touchStartLocations.Clear();
                RemoveFromActiveGestures();
                requireGestureRecognizersToFailThatHaveFailed.Clear();
            }

            // if this gesture is a fail gesture for another gesture, reset that gesture as this gesture has ended and not failed
            foreach (GestureRecognizer gesture in failGestures)
            {
                gesture.FailGestureNow();
            }
        }

        private void UpdateTouchState(bool executing)
        {
            if (executing && lastTrackTouchCount != CurrentTrackedTouches.Count)
            {
                ReceivedAdditionalTouches = true;
                lastTrackTouchCount = CurrentTrackedTouches.Count;
            }
            else
            {
                ReceivedAdditionalTouches = false;
            }
        }

        protected virtual void StateChanged()
        {
            if (StateUpdated != null)
            {
                StateUpdated(this);
            }
        }
        private bool HasAllRequiredFailGesturesToEndFromEndPending()
        {
            return requireGestureRecognizersToFail.SetEquals(requireGestureRecognizersToFailThatHaveFailed);
        }
        private void FailGestureNow()
        {
            state = GestureRecognizerState.Failed;
            RemoveFromActiveGestures();
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
            ResetInternal(ClearTrackedTouchesOnEndOrFail);
            justFailed = true;
            lastTrackTouchCount = 0;
            ReceivedAdditionalTouches = false;
        }

        private void RemoveFromActiveGestures()
        {
            ActiveGestures.Remove(this);
        }

        protected virtual void TouchesBegan(IEnumerable<GestureTouch> touches)
        {

        }

        /// <summary>
        /// Call with the touches that moved
        /// </summary>
        /// <param name="touches">Touches that moved</param>
        public void ProcessTouchesMoved(ICollection<GestureTouch> touches)
        {
            if (!Enabled || touches == null || touches.Count == 0 || !TouchesIntersect(touches, currentTrackedTouches))
            {
                return;
            }
            else if (CurrentTrackedTouches.Count > MaximumNumberOfTouchesToTrack ||
                (State != GestureRecognizerState.Possible && State != GestureRecognizerState.Began && State != GestureRecognizerState.Executing))
            {
                SetState(GestureRecognizerState.Failed);
            }
            else if (!EndGestureRestart(touches))
            {
                UpdateTrackedTouches(touches);
                TouchesMoved();
            }
        }

        protected virtual void TouchesMoved()
        {

        }

        private void UpdateTrackedTouches(IEnumerable<GestureTouch> touches)
        {
            int count = 0;
            foreach (GestureTouch touch in touches)
            {
                for (int i = 0; i < currentTrackedTouches.Count; i++)
                {
                    if (currentTrackedTouches[i].Id == touch.Id)
                    {
                        currentTrackedTouches[i] = touch;
                        count++;
                        break;
                    }
                }
            }
            if (count != 0)
            {
                currentTrackedTouches.Sort();
            }
        }

        public bool EndGestureRestart(ICollection<GestureTouch> touches)
        {
            if (isRestarting)
            {
                foreach (GestureTouch touch in touches)
                {
                    if (CurrentTrackedTouches.Contains(touch))
                    {
                        tempTouches.Add(touch);
                    }
                }
                currentTrackedTouches.Clear();
                ProcessTouchesBegan(tempTouches);
                tempTouches.Clear();
                isRestarting = false;
                return true;
            }
            return false;
        }

        private bool TouchesIntersect(IEnumerable<GestureTouch> collection, List<GestureTouch> list)
        {
            foreach (GestureTouch t in collection)
            {
                for (int i = 0; i < list.Count; i++)
                {
                    if (list[i].Id == t.Id)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public bool TrackedTouchCountIsWithinRange
        {
            get { return currentTrackedTouches.Count >= minimumNumberOfTouchesToTrack && currentTrackedTouches.Count <= maximumNumberOfTouchesToTrack; }
        }
        /// <summary>
        /// Call with the touches that ended
        /// </summary>
        /// <param name="touches">Touches that ended</param>
        public void ProcessTouchesEnded(ICollection<GestureTouch> touches)
        {
            if (!Enabled || touches == null || touches.Count == 0)
            {
                return;
            }

            try
            {
                foreach (GestureTouch touch in touches)
                {
                    ignoreTouchIds.Remove(touch.Id);
                }

                // if we have the wrong number of tracked touches or haven't started the gesture, fail
                if (!TrackedTouchCountIsWithinRange ||
                    (State != GestureRecognizerState.Possible && State != GestureRecognizerState.Began && State != GestureRecognizerState.Executing))
                {
                    FailGestureNow();
                }
                // if we don have touches we care about, process the end touches
                else if (TouchesIntersect(touches, currentTrackedTouches))
                {
                    UpdateTrackedTouches(touches);
                    TouchesEnded();
                }
            }
            finally
            {
                StopTrackingTouches(touches);
                justEnded = true;
            }
        }

        private int StopTrackingTouches(ICollection<GestureTouch> touches)
        {
            if (touches == null || touches.Count == 0)
            {
                return 0;
            }
            int count = 0;
            foreach (GestureTouch t in touches)
            {
                for (int i = 0; i < currentTrackedTouches.Count; i++)
                {
                    if (currentTrackedTouches[i].Id == t.Id)
                    {
                        currentTrackedTouches.RemoveAt(i);
                        count++;
                        break;
                    }
                }
            }
            return count;
        }

        protected virtual void TouchesEnded()
        {

        }

        /// <summary>
        /// Process cancelled touches
        /// </summary>
        /// <param name="touches">Touches</param>
        public void ProcessTouchesCancelled(ICollection<GestureTouch> touches)
        {
            if (!Enabled || touches == null || touches.Count == 0 || !TouchesIntersect(touches, currentTrackedTouches))
            {
                return;
            }

            try
            {
                foreach (GestureTouch t in touches)
                {
                    if (currentTrackedTouches.Contains(t))
                    {
                        SetState(GestureRecognizerState.Failed);
                        return;
                    }
                }
            }
            finally
            {
                StopTrackingTouches(touches);
                justEnded = true;
            }
        }
    }
}