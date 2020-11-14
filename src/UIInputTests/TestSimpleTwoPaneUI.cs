using Notes;
using Notes.UserInterfaces;
using NUnit.Framework;
using Veldrid;

namespace UIInputTests
{
    [TestFixture]
    public class TestSimpleTwoPaneUI
    {
        [Test]
        public void InputLettersToNoteEditor()
        {
            var window = new Window(0, 0, 100, 100, "Test", false);
            var userInterface = new SimpleTwoPanelUI();

            window.UserInterface = userInterface;
            userInterface.SetNoteEditorKeyboardFocus();

            window.RenderFrame(0f, new InputSnapshotForTests());

            var snapshot = new InputSnapshotForTests();

            snapshot.AddKeyCharPress('a'); snapshot.AddKeyEvent(new KeyEvent(Key.A, true, ModifierKeys.None));
            snapshot.AddKeyCharPress('s'); snapshot.AddKeyEvent(new KeyEvent(Key.S, true, ModifierKeys.None));
            snapshot.AddKeyCharPress('d'); snapshot.AddKeyEvent(new KeyEvent(Key.D, true, ModifierKeys.None));
            snapshot.AddKeyCharPress('f'); snapshot.AddKeyEvent(new KeyEvent(Key.F, true, ModifierKeys.None));

            window.RenderFrame(0f, snapshot);

            Assert.AreEqual("asdf", userInterface.Note.Text);

            window.Dispose();
        }

        [Test]
        public void InputLettersAndNewLinesToNoteEditor()
        {
            var window = new Window(0, 0, 100, 100, "Test", false);
            var userInterface = new SimpleTwoPanelUI();

            window.UserInterface = userInterface;
            userInterface.SetNoteEditorKeyboardFocus();

            window.RenderFrame(0f, new InputSnapshotForTests());

            var snapshot = new InputSnapshotForTests();

            snapshot.AddKeyCharPress('a'); snapshot.AddKeyEvent(new KeyEvent(Key.A, true, ModifierKeys.None));
            snapshot.AddKeyCharPress('s'); snapshot.AddKeyEvent(new KeyEvent(Key.S, true, ModifierKeys.None));
            snapshot.AddKeyCharPress('d'); snapshot.AddKeyEvent(new KeyEvent(Key.D, true, ModifierKeys.None));
            snapshot.AddKeyCharPress('f'); snapshot.AddKeyEvent(new KeyEvent(Key.F, true, ModifierKeys.None));
            window.RenderFrame(0f, snapshot);
            snapshot.Clear();

            snapshot.AddKeyEvent(new KeyEvent(Key.Enter, true, ModifierKeys.None));
            window.RenderFrame(0f, snapshot);
            snapshot.Clear();

            snapshot.AddKeyEvent(new KeyEvent(Key.Enter, true, ModifierKeys.None));

            window.RenderFrame(0f, snapshot);

            Assert.AreEqual("asdf\n\n", userInterface.Note.Text);

            window.Dispose();
        }

        [Test]
        public void InputNewlinesRespectIndentation()
        {
            var window = new Window(0, 0, 100, 100, "Test", false);
            var userInterface = new SimpleTwoPanelUI();

            window.UserInterface = userInterface;
            userInterface.SetNoteEditorKeyboardFocus();

            float time = 1f / 60f;

            window.RenderFrame(time, new InputSnapshotForTests());

            var snapshot = new InputSnapshotForTests();

            snapshot.AddKeyCharPress(' '); snapshot.AddKeyEvent(new KeyEvent(Key.Space, true, ModifierKeys.None));
            snapshot.AddKeyCharPress(' '); snapshot.AddKeyEvent(new KeyEvent(Key.Space, true, ModifierKeys.None));
            snapshot.AddKeyCharPress('a'); snapshot.AddKeyEvent(new KeyEvent(Key.A, true, ModifierKeys.None));
            snapshot.AddKeyCharPress('s'); snapshot.AddKeyEvent(new KeyEvent(Key.S, true, ModifierKeys.None));
            snapshot.AddKeyCharPress('d'); snapshot.AddKeyEvent(new KeyEvent(Key.D, true, ModifierKeys.None));
            snapshot.AddKeyCharPress('f'); snapshot.AddKeyEvent(new KeyEvent(Key.F, true, ModifierKeys.None));
            window.RenderFrame(time, snapshot); snapshot = new InputSnapshotForTests();

            snapshot.AddKeyEvent(new KeyEvent(Key.Enter, true, ModifierKeys.None)); // no KeyCharPress on newline, just key event for enter
            window.RenderFrame(time, snapshot); snapshot = new InputSnapshotForTests();
            snapshot.AddKeyEvent(new KeyEvent(Key.Enter, false, ModifierKeys.None)); // no KeyCharPress on newline, just key event for enter
            window.RenderFrame(time, snapshot); snapshot = new InputSnapshotForTests();

            snapshot.AddKeyCharPress('a'); snapshot.AddKeyEvent(new KeyEvent(Key.A, true, ModifierKeys.None));
            snapshot.AddKeyCharPress('s'); snapshot.AddKeyEvent(new KeyEvent(Key.S, true, ModifierKeys.None));
            snapshot.AddKeyCharPress('d'); snapshot.AddKeyEvent(new KeyEvent(Key.D, true, ModifierKeys.None));
            snapshot.AddKeyCharPress('f'); snapshot.AddKeyEvent(new KeyEvent(Key.F, true, ModifierKeys.None));

            window.RenderFrame(time, snapshot);

            Assert.AreEqual("  asdf\n  asdf", userInterface.Note.Text);

            window.Dispose();
        }
    }
}
