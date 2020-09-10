using System;
using System.Collections.Generic;
using System.Text;
using Veldrid;

namespace Notes
{
    public static class VeldridExtensions
    {
        public static TextureDescription GetDescription(this Texture texture)
        {
            return new TextureDescription(
                texture.Width, texture.Height,
                texture.Depth, texture.MipLevels, texture.ArrayLayers,
                texture.Format, texture.Usage, texture.Type, texture.SampleCount
            );
        }
    }
}
