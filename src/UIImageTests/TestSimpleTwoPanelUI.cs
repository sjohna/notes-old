using Notes;
using Notes.UserInterfaces;
using NUnit.Framework;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using static UIImageTests.TestSupport;
using Notes.MarkdigRenderers;

namespace UIImageTests
{
    [TestFixture]
    public class TestSimpleTwoPanelUI
    {
        private UIImageTestInfo testInfo;

        [TearDown]
        public void TearDown()
        {
            if (testInfo != null) testInfo.Dispose();
        }

        public static IEnumerable<object[]> TestCaseParameters()
        {
            yield return new object[] { "AST", 1280, 800 };
            yield return new object[] { "Plain text", 1280, 800 };
            yield return new object[] { "AST", 500, 500 };
            yield return new object[] { "Plain text", 500, 500 };
            yield return new object[] { "AST", 200, 200 };
            yield return new object[] { "Plain text", 200, 200 };
            yield return new object[] { "AST", 200, 1000 };
            yield return new object[] { "Plain text", 200, 1000 };
        }

        [Test]
        [TestCaseSource(nameof(TestCaseParameters))]
        public void EmptyInputText(string renderType, int windowWidth, int windowHeight)
        {
            var testName = $"{nameof(EmptyInputText)}_{renderType.Replace(" ", "")}Renderer";
            testInfo = new UIImageTestInfo(nameof(TestSimpleTwoPanelUI), testName, windowWidth, windowHeight);

            var userInterface = new SimpleTwoPanelUI(testInfo.Window.SDLWindow);

            userInterface.CurrentRenderType = renderType;

            if (renderType == "AST")
            {
                userInterface.Renderer = new MarkdigASTRenderer();
            }
            else if (renderType == "Plain text")
            {
                userInterface.Renderer = new MarkdigPlainTextRenderer();
            }

            testInfo.Window.UserInterface = userInterface;
            DoTest(testInfo);
        }

        [Test]
        [TestCaseSource(nameof(TestCaseParameters))]
        public void SingleLineInputText(string renderType, int windowWidth, int windowHeight)
        {
            var testName = $"{nameof(SingleLineInputText)}_{renderType.Replace(" ", "")}Renderer";
            testInfo = new UIImageTestInfo(nameof(TestSimpleTwoPanelUI), testName, windowWidth, windowHeight);

            var userInterface = new SimpleTwoPanelUI(testInfo.Window.SDLWindow);

            userInterface.CurrentRenderType = renderType;

            if (renderType == "AST")
            {
                userInterface.Renderer = new MarkdigASTRenderer();
            }
            else if (renderType == "Plain text")
            {
                userInterface.Renderer = new MarkdigPlainTextRenderer();
            }

            userInterface.InputText = "This is a single line!";

            testInfo.Window.UserInterface = userInterface;
            DoTest(testInfo);
        }

        [Test]
        [TestCaseSource(nameof(TestCaseParameters))]
        public void MultipleParagraphInputText(string renderType, int windowWidth, int windowHeight)
        {
            var testName = $"{nameof(MultipleParagraphInputText)}_{renderType.Replace(" ", "")}Renderer";
            testInfo = new UIImageTestInfo(nameof(TestSimpleTwoPanelUI), testName, windowWidth, windowHeight);

            var userInterface = new SimpleTwoPanelUI(testInfo.Window.SDLWindow);

            userInterface.CurrentRenderType = renderType;

            if (renderType == "AST")
            {
                userInterface.Renderer = new MarkdigASTRenderer();
            }
            else if (renderType == "Plain text")
            {
                userInterface.Renderer = new MarkdigPlainTextRenderer();
            }

            userInterface.InputText = @"This is a single line!

This is another line!

This is a third line!";

            testInfo.Window.UserInterface = userInterface;
            DoTest(testInfo);
        }

        [Test]
        [TestCaseSource(nameof(TestCaseParameters))]
        public void ListInInputText(string renderType, int windowWidth, int windowHeight)
        {
            var testName = $"{nameof(ListInInputText)}_{renderType.Replace(" ", "")}Renderer";
            testInfo = new UIImageTestInfo(nameof(TestSimpleTwoPanelUI), testName, windowWidth, windowHeight);

            var userInterface = new SimpleTwoPanelUI(testInfo.Window.SDLWindow);

            userInterface.CurrentRenderType = renderType;

            if (renderType == "AST")
            {
                userInterface.Renderer = new MarkdigASTRenderer();
            }
            else if (renderType == "Plain text")
            {
                userInterface.Renderer = new MarkdigPlainTextRenderer();
            }

            userInterface.InputText = @"List of things:
 - thing one
 - thing two
   - thing two a
   - thing two b
 - thing three
   - thing three a
     - thing three a one";

            testInfo.Window.UserInterface = userInterface;
            DoTest(testInfo);
        }

        [Test]
        [TestCaseSource(nameof(TestCaseParameters))]
        public void BoldAndItalicInputText(string renderType, int windowWidth, int windowHeight)
        {
            var testName = $"{nameof(BoldAndItalicInputText)}_{renderType.Replace(" ", "")}Renderer";
            testInfo = new UIImageTestInfo(nameof(TestSimpleTwoPanelUI), testName, windowWidth, windowHeight);

            var userInterface = new SimpleTwoPanelUI(testInfo.Window.SDLWindow);

            userInterface.CurrentRenderType = renderType;

            if (renderType == "AST")
            {
                userInterface.Renderer = new MarkdigASTRenderer();
            }
            else if (renderType == "Plain text")
            {
                userInterface.Renderer = new MarkdigPlainTextRenderer();
            }

            userInterface.InputText = @"This is *italic* and this is **bold** and this is ***bold italic***!";

            testInfo.Window.UserInterface = userInterface;
            DoTest(testInfo);
        }
    }
}
