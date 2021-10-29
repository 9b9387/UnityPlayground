using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

public class GesturesManager : MonoBehaviour
{
    public List<GestureRecognizer> gestures;
    private bool simulateTouches = true;
    private bool _hasActiveSimulatedMultitouch = true;
	private bool _hasActiveSimulatedTouch = true;
	private Vector3? _simulatedMultitouchStartPosition;
	private Vector3 _simulatedMousePosition;

	// Start is called before the first frame update
	void Start()
    {
        EnhancedTouchSupport.Enable();
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

		shouldProcessMouseInput();

	}


	private bool shouldProcessMouseInput()
	{
		if (!simulateTouches)
			return false;

		// check to see if the Unity Remote is active
		if (Input.touchCount > 0)
		{
			Debug.LogWarning("disabling touch simulation because we detected a Unity Remote connected");
			simulateTouches = false;
			return false;
		}

		// if enabled and alt is being held down we are simulating pinching
		if (_hasActiveSimulatedMultitouch || Input.GetKey(KeyCode.LeftAlt) || Input.GetKeyUp(KeyCode.LeftAlt))
		{
			Debug.Log("xxx");
			if (Input.GetKeyDown(KeyCode.LeftAlt))
			{
				Debug.Log("LeftAlt");
				_simulatedMultitouchStartPosition = Input.mousePosition;
			}
			else if (Input.GetKey(KeyCode.LeftShift))
			{
				// calculate the last mouse position from the simulated position and shift the start position acordingly
				var lastMousePosition = _simulatedMultitouchStartPosition.Value + (_simulatedMultitouchStartPosition.Value - _simulatedMousePosition);
				var diff = Input.mousePosition - lastMousePosition;
				_simulatedMultitouchStartPosition += diff;
			}

			if (Input.GetKey(KeyCode.LeftAlt) || Input.GetKeyUp(KeyCode.LeftAlt))
			{
				var diff = new Vector3(Input.mousePosition.x - _simulatedMultitouchStartPosition.Value.x, Input.mousePosition.y - _simulatedMultitouchStartPosition.Value.y);
				_simulatedMousePosition = _simulatedMultitouchStartPosition.Value - diff;
			}

			TouchPhase? touchPhase = null;
			if (Input.GetKey(KeyCode.LeftAlt) && Input.GetMouseButton(0))
			{
				// if we haven't started yet, add a touch began, else move
				if (!_hasActiveSimulatedMultitouch)
				{
					_hasActiveSimulatedMultitouch = true;
					touchPhase = TouchPhase.Began;
				}
				else
				{
					touchPhase = TouchPhase.Moved;
				}
			}

			if ((Input.GetKeyUp(KeyCode.LeftAlt) || Input.GetMouseButtonUp(0)) && _hasActiveSimulatedMultitouch)
			{
				touchPhase = TouchPhase.Ended;
				_hasActiveSimulatedMultitouch = false;
			}


			if (touchPhase.HasValue)
			{
				// we need to set up a second touch
				//_liveTouches.Add(_touchCache[1].populateWithPosition(_simulatedMousePosition, touchPhase.Value));
			}

			if (Input.GetKeyUp(KeyCode.LeftAlt))
			{
				_simulatedMultitouchStartPosition = null;
			}
		}

        _hasActiveSimulatedTouch = Input.GetMouseButton(0);

		return true;
	}


	// this is for debugging only while in the editor
	private void OnDrawGizmos()
	{
        //Gizmos.DrawIcon(Vector3.zero, "greenPoint.png", false);

        //if (drawTouches)
        {
            // draw a green point for all active touches, including the touches from Unity remote
            foreach (Touch touch in Touch.activeTouches)
            {
                if (touch.phase == UnityEngine.InputSystem.TouchPhase.Began
                    || touch.phase == UnityEngine.InputSystem.TouchPhase.Moved
                    || touch.phase == UnityEngine.InputSystem.TouchPhase.Stationary)
                {
                    var touchPos = Camera.main.ScreenToWorldPoint(new Vector3(touch.screenPosition.x, touch.screenPosition.y, Camera.main.farClipPlane));
                    Gizmos.DrawIcon(touchPos, "greenPoint.png", false);
                }
            }

            if (_simulatedMultitouchStartPosition.HasValue && !_hasActiveSimulatedTouch)
            {
                var mousePos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.farClipPlane));
                Gizmos.DrawIcon(mousePos, "redPoint.png", false);

                var simulatedPos = Camera.main.ScreenToWorldPoint(new Vector3(_simulatedMousePosition.x, _simulatedMousePosition.y, Camera.main.farClipPlane));
                Gizmos.DrawIcon(simulatedPos, "redPoint.png", false);
            }
        }
    }
}
