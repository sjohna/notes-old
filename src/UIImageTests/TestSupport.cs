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
    public class TestSupport : IDisposable
    {
        public static string TestFilesRootDirectory = "../../../TestFiles";

        public bool ImagesAreEqual(Image<Rgba32> image1, Image<Rgba32> image2)
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

        public Image<Rgba32> DiffImage(Image<Rgba32> image1, Image<Rgba32> image2)
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

        public static string GetFailingOutputFileForTest(string testSuiteName, string testName)
        {
            return Path.Combine(TestFilesRootDirectory, testSuiteName, "FailingOutput", testName + ".png");
        }

        public void DoTest(UIImageTestInfo testInfo)
        {
            if (File.Exists(testInfo.DiffFilePath)) File.Delete(testInfo.DiffFilePath);
            if (File.Exists(testInfo.FailingOutputFilePath)) File.Delete(testInfo.FailingOutputFilePath);
            if (File.Exists(testInfo.OutputFilePath)) File.Delete(testInfo.OutputFilePath);

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
                    File.Copy(testInfo.OutputFilePath, testInfo.FailingOutputFilePath);
                    Assert.Fail("Output image is different from reference.");
                }
            }
            else
            {
                Assert.Inconclusive("No reference image for test.");
            }
        }

        //private Dictionary<(int width, int height), Window> windowCache = new Dictionary<(int width, int height), Window>();

        public UIImageTestInfo CreateTestInfo(String testSuiteName, string testName, int windowWidth, int windowHeight)
        {
            //if (!windowCache.ContainsKey((windowWidth, windowHeight)))
            //{
            //    windowCache[(windowWidth, windowHeight)] = new Window(0, 0, windowWidth, windowHeight, "Notes", false);
            //}

            return new UIImageTestInfo(testSuiteName, testName, windowWidth, windowHeight, new Window(0, 0, windowWidth, windowHeight, "Notes", false));
        }

        public void Dispose()
        {
            //foreach (var window in windowCache)
            //{
            //    window.Value.Dispose();
            //}
        }
    }
}
