using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

public class GesturesManager : MonoBehaviour
{
    private readonly List<GestureRecognizer> gestures = new List<GestureRecognizer>();

    // Start is called before the first frame update
    void Start()
    {
        EnhancedTouchSupport.Enable();

        gestures.Add(new TapGesture());
    }

    // Update is called once per frame
    void Update()
    {
        var touches = Touch.activeTouches;
        if(touches.Count == 0)
        {
            return;
        }

        var deltaTime = Time.deltaTime;
        for (int i = 0; i < gestures.Count; i++)
        {
            gestures[i].TrackTouches(touches);
            gestures[i].OnUpdate(deltaTime);
        }
    }
}
