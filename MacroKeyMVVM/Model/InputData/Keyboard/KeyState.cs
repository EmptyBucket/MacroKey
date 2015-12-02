using System;

namespace MacroKeyMVVM.Model.InputData.Keyboard
{
    public enum KeyStates
    {
        KeyDown,
        KeyUp,
        KeyQuest
    }

    [Serializable]
    public struct KeyState : IInputState, IEquatable<KeyState>
    {
        public int State { get; }

        public KeyState(KeyStates keyState)
        {
            State = (int)keyState;
        }

        public readonly static KeyState KeyDown = new KeyState(KeyStates.KeyDown);
        public readonly static KeyState KeyUp = new KeyState(KeyStates.KeyUp);
        public readonly static KeyState KeyQuest = new KeyState(KeyStates.KeyQuest);

        public static bool operator==(KeyState one, KeyState two) => one.State == two.State;

        public static bool operator!=(KeyState one, KeyState two) => one.State != two.State;

        public bool Equals(KeyState other) => State == other.State;

        public override bool Equals(object obj) => obj is KeyState && Equals((KeyState)obj);

        public override int GetHashCode() => base.GetHashCode();
    }
}
