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
        public void InputLettersToTextBox()
        {
            var window = new Window(0, 0, 100, 100, "Test", false);
            var userInterface = new SimpleTwoPanelUI();

            window.UserInterface = userInterface;
            userInterface.SetNoteEditorKeyboardFocus();

            window.RenderFrame(new InputSnapshotForTests());

            var snapshot = new InputSnapshotForTests();

            snapshot.AddKeyCharPress('a'); snapshot.AddKeyEvent(new KeyEvent(Key.A, true, ModifierKeys.None));
            snapshot.AddKeyCharPress('s'); snapshot.AddKeyEvent(new KeyEvent(Key.S, true, ModifierKeys.None));
            snapshot.AddKeyCharPress('d'); snapshot.AddKeyEvent(new KeyEvent(Key.D, true, ModifierKeys.None));
            snapshot.AddKeyCharPress('f'); snapshot.AddKeyEvent(new KeyEvent(Key.F, true, ModifierKeys.None));

            window.RenderFrame(snapshot);

            Assert.AreEqual("asdf", userInterface.Note.Text);
        }
    }
}
