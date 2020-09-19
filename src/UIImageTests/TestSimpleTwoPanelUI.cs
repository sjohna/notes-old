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
        static readonly string testFileRootDirectory = "../../../TestFiles/TestSimpleTwoPanelUI";

        [SetUp]
        public void FixtureSetup()
        {
            EnsureTestDirectoriesExists();
        }

        [Test]
        public void EmptyString_TextRenderer()
        {
            // generate output
            var window = new Window(0, 0, 1280, 800, "Notes");
            var outputFileName = GetOutputFileForTest("EmptyString_TextRenderer");
            window.SaveScreenToFile(outputFileName);

            // get reference
            var referenceFileName = GetReferenceFileForTest("EmptyString_TextRenderer");

            if (File.Exists(referenceFileName))
            {
                var referenceImage = Image.Load<Rgba32>(referenceFileName);
                var outputImage = Image.Load<Rgba32>(outputFileName);

                if (ImagesAreEqual(referenceImage, outputImage))
                {
                    Assert.Pass();
                }
                else
                {
                    var diffFileName = GetDiffFileForTest("EmptyString_TextRenderer");
                    var diffImage = DiffImage(referenceImage, outputImage);
                    diffImage.SaveAsPng(diffFileName);
                    Assert.Fail("Output image is different from reference.");
                }
            }
            else
            {
                Assert.Inconclusive("No reference image for test.");
            }
        }

        private void EnsureTestDirectoriesExists()
        {
            if (!Directory.Exists(testFileRootDirectory))
            {
                Directory.CreateDirectory(testFileRootDirectory);
            }

            var outputDirectory = Path.Combine(testFileRootDirectory, "Output");
            var referenceDirectory = Path.Combine(testFileRootDirectory, "Reference");
            var diffDirectory = Path.Combine(testFileRootDirectory, "Diff");

            if (!Directory.Exists(outputDirectory))
            {
                Directory.CreateDirectory(outputDirectory);
            }

            if (!Directory.Exists(referenceDirectory))
            {
                Directory.CreateDirectory(referenceDirectory);
            }

            if (!Directory.Exists(diffDirectory))
            {
                Directory.CreateDirectory(diffDirectory);
            }
        }

        private string GetDiffFileForTest(string test)
        {
            return Path.Combine(testFileRootDirectory, "Diff", test + ".png");
        }

        private string GetOutputFileForTest(string test)
        {
            return Path.Combine(testFileRootDirectory, "Output", test + ".png");
        }

        private string GetReferenceFileForTest(string test)
        {
            return Path.Combine(testFileRootDirectory, "Reference", test + ".png");
        }
    }
}
