using Gabriel.Cat.S.Drawing;
using Gabriel.Cat.S.Utilitats;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text;

namespace Gabriel.Cat.S.Extension
{
    public static class ExtensionBitmap
    {
        unsafe delegate void TratarImg(byte* ptrImg);
        public static Bitmap ChangeColor(this Bitmap bmp, FiltroImagen color)
        {
            const int INCREMENTO = 4;
            Bitmap bmpResultado = bmp.Clone(PixelFormat.Format32bppArgb);//asi hago que todas las imagenes tengan el mismo formato :D
            TratarImg metodo = null;

            unsafe
            {
                switch (color)
                {
                    case FiltroImagen.Red:
                        metodo = ToRojo; break;
                    case FiltroImagen.Green:
                        metodo = ToVerde; break;
                    case FiltroImagen.Blue:
                        metodo = ToAzul; break;
                    case FiltroImagen.Sepia:
                        metodo = ToSepia; break;
                    case FiltroImagen.GrayScale:
                        metodo = ToEscalaDeGrises; break;
                    case FiltroImagen.Inverted:
                        metodo = ToInvertido; break;
                }



                bmpResultado.TrataBytes((ptrBytesBmpResultado) =>
                {
                    for (byte* ptrFin = ptrBytesBmpResultado + bmp.Width * bmp.Height * INCREMENTO; ptrBytesBmpResultado != ptrFin; ptrBytesBmpResultado += INCREMENTO)
                    {
                        metodo(ptrBytesBmpResultado);
                    }
                });

            }
            return bmpResultado;

        }
        #region Optimizacion

        unsafe static void ToRojo(byte* ptImg)
        {
            ptImg[Pixel.G] = 0x0;
            ptImg[Pixel.B] = 0x0;
        }
        unsafe static void ToAzul(byte* ptImg)
        {
            ptImg[Pixel.G] = 0x0;
            ptImg[Pixel.R] = 0x0;
        }
        unsafe static void ToVerde(byte* ptImg)
        {
            ptImg[Pixel.R] = 0x0;
            ptImg[Pixel.B] = 0x0;
        }
        unsafe static void ToInvertido(byte* ptImg)
        {
            ptImg[Pixel.R] = (byte)(255 - ptImg[Pixel.R]);
            ptImg[Pixel.G] = (byte)(255 - ptImg[Pixel.G]);
            ptImg[Pixel.B] = (byte)(255 - ptImg[Pixel.B]);
        }
        unsafe static void ToEscalaDeGrises(byte* ptImg)
        {
            ptImg[Pixel.R] = Convert.ToByte(0.2126 * ptImg[Pixel.R] + 0.7152 * ptImg[Pixel.G] + 0.0722 * ptImg[Pixel.B]);
            ptImg[Pixel.G] = ptImg[Pixel.R];
            ptImg[Pixel.B] = ptImg[Pixel.G];
        }
        unsafe static void ToSepia(byte* ptImg)
        {
            int rInt = Convert.ToInt32(ptImg[Pixel.R] * 0.393 + ptImg[Pixel.G] * 0.769 + ptImg[Pixel.B] * 0.189);
            int gInt = Convert.ToInt32(ptImg[Pixel.R] * 0.349 + ptImg[Pixel.G] * 0.686 + ptImg[Pixel.B] * 0.168);
            int bInt = Convert.ToInt32(ptImg[Pixel.R] * 0.272 + ptImg[Pixel.G] * 0.534 + ptImg[Pixel.B] * 0.131);
            if (rInt > 255)
                rInt = 255;
            if (gInt > 255)
                gInt = 255;
            if (bInt > 255)
                bInt = 255;
            ptImg[Pixel.R] = (byte)rInt;
            ptImg[Pixel.G] = (byte)gInt;
            ptImg[Pixel.B] = (byte)bInt;
        }


        #endregion
    }
}
