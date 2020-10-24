using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using Veldrid;

namespace UIInputTests
{
    public class InputSnapshotForTests : Veldrid.InputSnapshot
    {
        private List<KeyEvent> _keyEvents = new List<KeyEvent>();
        public IReadOnlyList<KeyEvent> KeyEvents => _keyEvents.AsReadOnly();

        private List<MouseEvent> _mouseEvents = new List<MouseEvent>();
        public IReadOnlyList<MouseEvent> MouseEvents => _mouseEvents.AsReadOnly();

        private List<char> _keyCharPresses = new List<char>();
        public IReadOnlyList<char> KeyCharPresses => _keyCharPresses.AsReadOnly();

        public Vector2 MousePosition { get; private set; }

        public float WheelDelta { get; private set; }

        public bool IsMouseDown(MouseButton button)
        {
            return false;
        }

        public InputSnapshotForTests()
        {
            MousePosition = new Vector2(1, 1);
            WheelDelta = 0.0f;
        }

        public void Clear()
        {
            _keyEvents.Clear();
            _mouseEvents.Clear();
            _keyCharPresses.Clear();
        }

        public void AddKeyCharPress(char key)
        {
            _keyCharPresses.Add(key);
        }

        public void AddKeyEvent(KeyEvent keyEvent)
        {
            _keyEvents.Add(keyEvent);
        }
    }
}
