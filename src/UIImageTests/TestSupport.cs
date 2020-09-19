using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Collections.Generic;
using System.Text;

namespace UIImageTests
{
    public static class TestSupport
    {
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
    }
}
