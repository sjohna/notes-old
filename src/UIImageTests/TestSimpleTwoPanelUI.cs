using Notes;
using NUnit.Framework;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using static UIImageTests.TestSupport;

namespace UIImageTests
{
    [TestFixture]
    public class TestSimpleTwoPanelUI
    {
        UIImageTestInfo testInfo;

        [TearDown]
        public void TearDown()
        {
            if (testInfo != null) testInfo.Dispose();
        }

        [Test]
        public void EmptyString_TextRenderer()
        {
            testInfo = new UIImageTestInfo(nameof(TestSimpleTwoPanelUI), nameof(EmptyString_TextRenderer), 1280, 800);
            DoTest(testInfo);
        }
    }
}
