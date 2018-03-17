using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Text;

namespace Gabriel.Cat.S.Extension
{
   public static class ExtensionPixelFormat
    {
        public static bool IsArgb(this PixelFormat format)
        {
            bool isArgb = false;
            switch (format)
            {
                case PixelFormat.Format16bppArgb1555:
                case PixelFormat.Format32bppArgb:
                case PixelFormat.Format32bppPArgb:
                case PixelFormat.Format64bppArgb:
                case PixelFormat.Format64bppPArgb:
                    isArgb = true;
                    break;
            }
            return isArgb;
        }
    }
}
