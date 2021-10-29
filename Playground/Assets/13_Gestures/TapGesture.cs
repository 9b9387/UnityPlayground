// TapGesture.cs
// Author: shihongyang shihongyang@weile.com
// Data: 2021/10/19
using Owlet;
using UnityEngine;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

[CreateAssetMenu(fileName = "Tap Gesture", menuName = "Gesture/Tap Gesture")]
public class TapGesture : GestureRecognizer
{
    public TapGesture()
    {
    }

    protected override void ProcessTouchBegan(Touch touch)
    {
        state.UpdateTouchState(TouchState.Touching);
    }

    protected override void ProcessTouchesMoved(Touch touch)
    {
        if(state.IsRecognizeState(RecognizeState.Failed))
        {
            return;
        }
        if (Vector2.Distance(touch.startScreenPosition, touch.screenPosition) > 30f)
        {
            Debug.Log("tap gesture failed2.");
            state.UpdateRecognizeState(RecognizeState.Failed);// = Owlet.GestureRecognizerState.Failed;
        }
    }

    protected override void ProcessTouchesEnded(Touch touch)
    {
        state.UpdateTouchState(TouchState.Finished);

        if (state.IsRecognizeState(RecognizeState.Failed))
        {
            return;
        }

        if (state.IsRecognizeState(RecognizeState.Unknown))//  == Owlet.GestureRecognizerState.Began)
        {
            if(touch.time - touch.startTime > 0.3f)
            {
                Debug.Log("tap gesture failed1.");
                state.UpdateRecognizeState(RecognizeState.Failed);
            }
            else
            {
                state.UpdateRecognizeState(RecognizeState.Succeeded);
                Debug.Log("trigger tap gesture.");
            }
        }
    }
}
