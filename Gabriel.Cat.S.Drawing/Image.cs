using System;
using System.Drawing;

namespace Gabriel.Cat.S.Drawing
{
    public static class Image
    {


        #region Pixels
        public static Color ToRed(this Color pixel)
        {
            return ToRed(pixel.R, 0, 0);

        }
        public static Color ToBlue(this Color pixel)
        {
            return ToBlue(0, 0, pixel.B);
        }
        public static Color ToGreen(this Color pixel)
        {
            return ToGreen(0, pixel.G, 0);
        }
        public static Color ToEscalaGrises(this Color pixel)
        {
            return ToGrayScale(pixel.R, pixel.G, pixel.B);

        }
        public static Color ToInverted(this Color pixel)
        {
            return ToInverted((byte)(255 - pixel.R), (byte)(255 - pixel.G), (byte)(255 - pixel.B));
        }
        public static Color ToSepia(this Color pixel)
        {
            return ToSepia(pixel.R, pixel.G, pixel.B);
        }
        public static Color Mezclar(this Color pixel1, Color pixel2)
        {
            return MezclaPixels(pixel1.R, pixel1.G, pixel1.B, pixel1.A, pixel2.R, pixel2.G, pixel2.B, pixel2.A);
        }
        public static Color MezclaPixels(byte byteR1, byte byteG1, byte byteB1, byte byteA1, byte byteR2, byte byteG2, byte byteB2, byte byteA2)
        {
            int a, r, g, b;

            a = byteA1 + byteA2;
            r = byteR1 + byteR2;
            g = byteG1 + byteG2;
            b = byteB1 + byteB2;

            if (a > 255) a = 255;
            if (r > 255) r = 255;
            if (g > 255) g = 255;
            if (b > 255) b = 255;

            return Color.FromArgb(a, r, g, b);
        }

        public static Color ToRed(byte r, byte g, byte b)
        {
            return Color.FromArgb(0, 0, r);
        }
        public static Color ToBlue(byte r, byte g, byte b)
        {
            return Color.FromArgb(b, 0, 0);
        }
        public static Color ToGreen(byte r, byte g, byte b)
        {
            return Color.FromArgb(0, g, 0);
        }
        public static Color ToGrayScale(byte r, byte g, byte b)
        {
            int v = Convert.ToInt32(0.2126 * r + 0.7152 * g + 0.0722 * b);
            return Color.FromArgb(v, v, v);

        }

        public static Color ToInverted(byte r, byte g, byte b)
        {
            return Color.FromArgb(255 - r, 255 - g, 255 - b);
        }
        public static Color ToSepia(byte r, byte g, byte b)
        {
            int rInt = Convert.ToInt32(r * 0.393 + g * 0.769 + b * 0.189);
            int gInt = Convert.ToInt32(r * 0.349 + g * 0.686 + b * 0.168);
            int bInt = Convert.ToInt32(r * 0.272 + g * 0.534 + b * 0.131);
            if (rInt > 255)
                rInt = 255;
            if (gInt > 255)
                gInt = 255;
            if (bInt > 255)
                bInt = 255;
            r = (byte)rInt;
            g = (byte)gInt;
            b = (byte)bInt;
            return Color.FromArgb(r, g, b);
        }


        #endregion

    }

}