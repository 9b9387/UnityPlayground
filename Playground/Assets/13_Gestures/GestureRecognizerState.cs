// GestureRecognizerState.cs
// Author: shihongyang shihongyang@weile.com
// Data: 2021/10/8

namespace Owlet
{
    public enum GestureRecognizerState
    {
        /// <summary>
        /// Gesture is possible
        /// </summary>
        Possible = 1,

        /// <summary>
        /// Gesture has started
        /// </summary>
        Began = 2,

        /// <summary>
        /// Gesture is executing
        /// </summary>
        Executing = 4,

        /// <summary>
        /// Gesture has ended
        /// </summary>
        Ended = 8,

        /// <summary>
        /// End is pending, if the dependant gesture fails
        /// </summary>
        EndPending = 16,

        /// <summary>
        /// Gesture has failed
        /// </summary>
        Failed = 32
    }


    /// <summary>
    /// 识别状态
    /// </summary>
    public enum RecognizeState
    {
        Unknown = 0x00,
        Succeeded = 0x01,
        Failed = 0x02,
        Waiting = 0x03
    }
    /// <summary>
    /// 触摸状态
    /// </summary>
    public enum TouchState
    {
        Untouched = 0x00,
        Began = 0x10,
        Ended = 0x20
    }

    /// <summary>
    /// 手势状态
    /// </summary>
    public class GestureState
    {
        /// <summary>
        /// 识别状态掩码
        /// </summary>
        private static readonly int recognizeMask = 0x0F;
        /// <summary>
        /// 触摸状态掩码
        /// </summary>
        private static readonly int touchMask = 0xF0;
        /// <summary>
        /// 状态值
        /// </summary>
        private int state = (int)RecognizeState.Unknown | (int)TouchState.Untouched;
        /// <summary>
        /// 更新识别状态
        /// </summary>
        /// <param name="recognizeState"></param>
        public void UpdateRecognizeState(RecognizeState recognizeState)
        {
            state = state & touchMask | (int)recognizeState;
        }
        /// <summary>
        /// 更新触摸状态
        /// </summary>
        /// <param name="touchState"></param>
        public void UpdateTouchState(TouchState touchState)
        {
            state = state & recognizeMask | (int)touchState;
        }
        /// <summary>
        /// 触摸状态是否为touchState
        /// </summary>
        /// <param name="touchState"></param>
        /// <returns></returns>
        public bool IsTouchState(TouchState touchState)
        {
            return (state & touchMask) == (int)touchState;
        }
        /// <summary>
        /// 识别状态是否为recognizeState
        /// </summary>
        /// <param name="recognizeState"></param>
        /// <returns></returns>
        public bool IsRecognizeState(RecognizeState recognizeState)
        {
            return (state & recognizeMask) == (int)recognizeState;
        }
        /// <summary>
        /// 重置手势状态
        /// </summary>
        public void Reset()
        {
            state = (int)RecognizeState.Unknown | (int)TouchState.Untouched;
        }
    }
}