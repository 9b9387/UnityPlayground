using System.Collections;
using System.Collections.Generic;
using Owlet;
using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

public class GestureManager : MonoBehaviour
{
    private readonly List<GestureRecognizer> gestures = new List<GestureRecognizer>();
    private readonly List<GestureRecognizer> gesturesTemp = new List<GestureRecognizer>();
    private readonly List<GestureTouch> currentTouches = new List<GestureTouch>();
    private readonly Dictionary<int, Vector2> previousTouchPositions = new Dictionary<int, Vector2>();
    private readonly List<GestureTouch> touchesBegan = new List<GestureTouch>();
    private readonly List<GestureTouch> touchesMoved = new List<GestureTouch>();
    private readonly List<GestureTouch> touchesEnded = new List<GestureTouch>();
    private readonly List<GestureTouch> previousTouches = new List<GestureTouch>();

    private TapGestureRecognizer tapGesture;

    // Start is called before the first frame update
    void Start()
    {
        EnhancedTouchSupport.Enable();
        CreateTapGesture();

        GestureRecognizer.MainThreadCallback = (float delay, System.Action callback) =>
        {
            StartCoroutine(MainThreadCallback(delay, callback));
        };
    }

    private IEnumerator MainThreadCallback(float delay, System.Action action)
    {
        if (action != null)
        {
            System.Diagnostics.Stopwatch timer = new System.Diagnostics.Stopwatch();
            timer.Start();
            yield return null;
            while ((float)timer.Elapsed.TotalSeconds < delay)
            {
                yield return null;
            }
            action();
        }
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(Touch.activeTouches.Count);

        //for (int i = 0; i < Touch.activeTouches.Count; i++)
        //{
        //    var touch = Touch.activeTouches[i];
        //    Debug.Log($"pahse: {touch.phase}");
        //}

        currentTouches.Clear();
        touchesBegan.Clear();
        touchesMoved.Clear();
        touchesEnded.Clear();

        ProcessTouches();

        //gesturesTemp.AddRange(gestures);
        //Debug.Log($"{currentTouches.Count} {touchesBegan.Count} {touchesMoved.Count} {touchesEnded.Count} {gestures.Count}");
        foreach (GestureRecognizer gesture in gestures)
        {
            gesture.ProcessTouchesBegan(touchesBegan);
            gesture.ProcessTouchesMoved(touchesMoved);
            gesture.ProcessTouchesEnded(touchesEnded);
        }
        //gesturesTemp.Clear();
    }

    private void ProcessTouches()
    {
        // process each touch in the Unity list of touches
        for (int i = 0; i < Touch.activeTouches.Count; i++)
        {
            Touch t = Touch.activeTouches[i];
            GestureTouch g = GestureTouchFromTouch(ref t);
            //string d = string.Format("Touch: {0} {1}", t.screenPosition, t.phase);
            //Debug.Log(d);
            FingersProcessTouch(ref g);
        }
    }

    private void FingersProcessTouch(ref GestureTouch g)
    {
        currentTouches.Add(g);

        // do our own touch up / down tracking, the user can reset touch state so that touches can begin again without a finger being lifted
        if (g.TouchPhase == Owlet.TouchPhase.Moved || g.TouchPhase == Owlet.TouchPhase.Stationary)
        {
            FingersContinueTouch(ref g);
        }
        else if (g.TouchPhase == Owlet.TouchPhase.Began)
        {
            FingersBeginTouch(ref g);
        }
        else
        {
            FingersEndTouch(ref g);
        }


    }

    private void FingersBeginTouch(ref GestureTouch g)
    {
        if (!previousTouches.Contains(g))
        {
            previousTouches.Add(g);
        }
        //Debug.Log("FingersBeginTouch");
        touchesBegan.Add(g);
        previousTouchPositions[g.Id] = new Vector2(g.X, g.Y);
    }

    private void FingersContinueTouch(ref GestureTouch g)
    {
        //Debug.Log("FingersContinueTouch");
        touchesMoved.Add(g);
        previousTouchPositions[g.Id] = new Vector2(g.X, g.Y);
    }

    private void FingersEndTouch(ref GestureTouch g, bool lost = false)
    {
        if (!lost)
        {
            //Debug.Log("FingersEndTouch");
            touchesEnded.Add(g);
        }
        previousTouchPositions.Remove(g.Id);
        previousTouches.Remove(g);
    }

    private GestureTouch GestureTouchFromTouch(ref Touch t)
    {
        // convert Unity touch to Gesture touch
        Vector2 prev;
        if (!previousTouchPositions.TryGetValue(t.touchId, out prev))
        {
            prev.x = t.screenPosition.x;
            prev.y = t.screenPosition.y;
        }
        Owlet.TouchPhase phase;
        switch (t.phase)
        {
            case UnityEngine.InputSystem.TouchPhase.Began:
                phase = Owlet.TouchPhase.Began;
                break;

            case UnityEngine.InputSystem.TouchPhase.Canceled:
                phase = Owlet.TouchPhase.Cancelled;
                break;

            case UnityEngine.InputSystem.TouchPhase.Ended:
                phase = Owlet.TouchPhase.Ended;
                break;

            case UnityEngine.InputSystem.TouchPhase.Moved:
                phase = Owlet.TouchPhase.Moved;
                break;

            case UnityEngine.InputSystem.TouchPhase.Stationary:
                phase = Owlet.TouchPhase.Stationary;
                break;

            default:
                phase = Owlet.TouchPhase.Unknown;
                break;
        }
        GestureTouch touch = new GestureTouch(t.touchId, t.screenPosition.x, t.screenPosition.y, prev.x, prev.y, t.pressure, t, phase);
        prev.x = t.screenPosition.x;
        prev.y = t.screenPosition.y;
        previousTouchPositions[t.touchId] = prev;
        return touch;
    }

    private void CreateTapGesture()
    {
        tapGesture = new TapGestureRecognizer();
        tapGesture.StateUpdated += TapGestureCallback;
        AddGesture(tapGesture);
    }

    public bool AddGesture(GestureRecognizer gesture)
    {
        if (gesture == null || gestures.Contains(gesture))
        {
            return false;
        }
        gestures.Add(gesture);
        return true;
    }

    private void TapGestureCallback(GestureRecognizer gesture)
    {
        if (gesture.State == GestureRecognizerState.Ended)
        {
            Debug.Log($"Tapped at {gesture.FocusX}, {gesture.FocusY}");
        }
    }
}
