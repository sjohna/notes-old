using Notes;
using NUnit.Framework;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace UIImageTests
{
    public static class TestSupport
    {
        public static string TestFilesRootDirectory = "../../../TestFiles";

        public static bool ImagesAreEqual(Image<Rgba32> image1, Image<Rgba32> image2)
        {
            if (image1.Height != image2.Height || image1.Width != image2.Width)
            {
                return false;
            }

            for (int i = 0; i < image1.Height; ++i)
            {
                for (int j = 0; j < image1.Width; ++j)
                {
                    if (image1[j, i] != image2[j, i])
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        public static Image<Rgba32> DiffImage(Image<Rgba32> image1, Image<Rgba32> image2)
        {
            if (image1.Height != image2.Height || image1.Width != image2.Width)
            {
                throw new Exception("Cannot generate diff image, image dimensions are not the same.");
            }

            var ret = new Image<Rgba32>(image1.Width, image1.Height);

            for (int i = 0; i < image1.Height; ++i)
            {
                for (int j = 0; j < image1.Width; ++j)
                {
                    if (image1[j, i] != image2[j, i])
                    {
                        ret[j, i] = Color.Red;
                    }
                    else
                    {
                        ret[j, i] = Color.Black;
                    }
                }
            }

            return ret;
        }

        public static string GetDiffFileForTest(string testSuiteName, string testName)
        {
            return Path.Combine(TestFilesRootDirectory, testSuiteName, "Diff", testName + ".png");
        }

        public static string GetOutputFileForTest(string testSuiteName, string testName)
        {
            return Path.Combine(TestFilesRootDirectory, testSuiteName, "Output", testName + ".png");
        }

        public static string GetReferenceFileForTest(string testSuiteName, string testName)
        {
            return Path.Combine(TestFilesRootDirectory, testSuiteName, "Reference", testName + ".png");
        }

        public static void DoTest(UIImageTestInfo testInfo)
        {
            testInfo.Window.SaveScreenToFile(testInfo.OutputFilePath);

            if (File.Exists(testInfo.ReferenceFilePath))
            {
                var referenceImage = Image.Load<Rgba32>(testInfo.ReferenceFilePath);
                var outputImage = Image.Load<Rgba32>(testInfo.OutputFilePath);

                if (ImagesAreEqual(referenceImage, outputImage))
                {
                    Assert.Pass();
                }
                else
                {
                    var diffImage = DiffImage(referenceImage, outputImage);
                    diffImage.SaveAsPng(testInfo.DiffFilePath);
                    Assert.Fail("Output image is different from reference.");
                }
            }
            else
            {
                Assert.Inconclusive("No reference image for test.");
            }
        }
    }
}
