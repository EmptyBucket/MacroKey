using System;

namespace MacroKeyMVVM.Model.InputData.Mouse
{
    public enum MouseStates
    {
        MouseDown,
        MouseUp,
        MouseWheelDown,
        MouseWheelUp,
        MouseQuest
    }

    [Serializable]
    public struct MouseState : IInputState, IEquatable<MouseState>
    {
        public int State { get; }

        public MouseState(MouseStates state)
        {
            State = (int)state;
        }

        public static bool operator==(MouseState one, MouseState two) => one.State == two.State;

        public static bool operator!=(MouseState one, MouseState two) => one.State != two.State;

        public readonly static MouseState MouseDown = new MouseState(MouseStates.MouseDown);
        public readonly static MouseState MouseUp = new MouseState(MouseStates.MouseUp);
        public readonly static MouseState MouseWheelDown = new MouseState(MouseStates.MouseWheelDown);
        public readonly static MouseState MouseWheelUp = new MouseState(MouseStates.MouseWheelDown);
        public readonly static MouseState MouseQest = new MouseState(MouseStates.MouseQuest);

        public bool Equals(MouseState other) => this == other;

        public override bool Equals(object other) => other is MouseState && Equals((MouseState)other);

        public override int GetHashCode() => base.GetHashCode();
    }
}
