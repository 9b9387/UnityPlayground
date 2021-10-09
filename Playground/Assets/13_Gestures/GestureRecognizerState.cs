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
}