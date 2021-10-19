// TapGestureRecognizer_OLD.cs
// Author: shihongyang shihongyang@weile.com
// Data: 2021/10/9
using System.Collections.Generic;
using System.Diagnostics;
using Debug = UnityEngine.Debug;

namespace Owlet
{
    public class TapGestureRecognizer_OLD : GestureRecognizer_OLD
    {
        private int tapCount;
        private readonly Stopwatch timer = new Stopwatch();
        private readonly List<GestureTouch> tapTouches = new List<GestureTouch>();

        public TapGestureRecognizer_OLD()
        {
            NumberOfTapsRequired = 1;
            ThresholdSeconds = 0.4f;
            ThresholdUnits = 0.3f;
            TapTouches = tapTouches.AsReadOnly();
        }

        protected override void TouchesBegan(IEnumerable<GestureTouch> touches)
        {
            foreach (GestureTouch touch in touches)
            {
                if (!IgnoreTouch(touch.Id))
                {
                    Debug.Log("AAAA");
                    SetState(GestureRecognizerState.Failed);
                    return;
                }
            }

            CalculateFocus(CurrentTrackedTouches);
            timer.Reset();
            timer.Start();
            if (SendBeginState && TrackedTouchCountIsWithinRange)
            {
                SetState(GestureRecognizerState.Began);
            }
            else
            {
                SetState(GestureRecognizerState.Possible);
            }

            // track positions if this is the first tap
            if (tapCount == 0)
            {
                TrackCurrentTrackedTouchesStartLocations();
            }

            foreach (GestureTouch touch in touches)
            {
                tapTouches.Add(touch);
            }

        }

        protected override void StateChanged()
        {
            base.StateChanged();

            if (State == GestureRecognizerState.Failed || State == GestureRecognizerState.Ended)
            {
                tapCount = 0;
                timer.Reset();
                tapTouches.Clear();
            }
        }

        protected override void TouchesEnded()
        {
            if ((float)timer.Elapsed.TotalSeconds <= ThresholdSeconds)
            {
                CalculateFocus(CurrentTrackedTouches);

                // determine if any touch has moved too far
                bool touchesAreWithinDistance = AreTrackedTouchesWithinDistance(ThresholdUnits);

                if (touchesAreWithinDistance)
                {
                    if (++tapCount == NumberOfTapsRequired)
                    {
                        SetState(GestureRecognizerState.Ended);
                    }
                    else
                    {
                        // we need another tap, reset the timer and fail if we don't get another tap in time
                        timer.Reset();
                        timer.Start();
                        //FailGestureAfterDelayIfNoTap();
                    }
                }
                else
                {
                     Debug.Log("Distance exceeded: " + DistanceX + ", " + DistanceY);

                    SetState(GestureRecognizerState.Failed);
                }
            }
            else
            {
                 Debug.Log("Time elapsed");

                // too much time elapsed, fail the gesture
                SetState(GestureRecognizerState.Failed);
            }
        }

        protected override void TouchesMoved()
        {

            CalculateFocus(CurrentTrackedTouches);

            // if touch is down too long, it's not a tap, fail the gesture
            if (timer.Elapsed.TotalSeconds >= ThresholdSeconds)
            {
                SetState(GestureRecognizerState.Failed);
            }
        }

        /// <summary>
        /// How many taps must execute in order to end the gesture - default is 1.
        /// </summary>
        /// <value>The number of taps required to execute the gesture</value>
        public int NumberOfTapsRequired { get; set; }

        /// <summary>
        /// How many seconds can expire before the tap is released to still count as a tap - default is 0.4.
        /// </summary>
        /// <value>The threshold in seconds</value>
        public float ThresholdSeconds { get; set; }

        /// <summary>
        /// How many units away the tap down and up and subsequent taps can be to still be considered - must be greater than 0. Default is 0.3.
        /// </summary>
        /// <value>The threshold in units</value>
        public float ThresholdUnits { get; set; }

        /// <summary>
        /// Whether the tap gesture will immediately send a begin state when a touch is first down. Default is false.
        /// </summary>
        public bool SendBeginState { get; set; }

        /// <summary>
        /// Contains every touch that was involved in the tap
        /// </summary>
        public System.Collections.ObjectModel.ReadOnlyCollection<GestureTouch> TapTouches { get; private set; }
    }
}