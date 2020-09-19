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
    }
}
