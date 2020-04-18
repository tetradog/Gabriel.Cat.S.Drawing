using Gabriel.Cat.S.Extension;
using Gabriel.Cat.S.Utilitats;
using System;
using System.Drawing;

namespace Gabriel.Cat.S.Extension
{
    public static class Imagen
    {


        #region Pixels
        public static Color ToRed(this Utilitats.V2.Color pixel)
        {
            return ToRed(pixel.Red, 0, 0);

        }
        public static Color ToBlue(this Utilitats.V2.Color pixel)
        {
            return ToBlue(0, 0, pixel.Blue);
        }
        public static Color ToGreen(this Utilitats.V2.Color pixel)
        {
            return ToGreen(0, pixel.Green, 0);
        }
        public static Color ToEscalaGrises(this Utilitats.V2.Color pixel)
        {
            return ToGrayScale(pixel.Red, pixel.Green, pixel.Blue);

        }
        public static Color ToInverted(this Utilitats.V2.Color pixel)
        {
            return ToInverted((byte)(255 - pixel.Red), (byte)(255 - pixel.Green), (byte)(255 - pixel.Blue));
        }
        public static Color ToSepia(this Utilitats.V2.Color pixel)
        {
            return ToSepia(pixel.Red, pixel.Green, pixel.Blue);
        }
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
        public static Color Mezclar(this Utilitats.V2.Color pixel1, Utilitats.V2.Color pixel2)
        {
            return MezclaPixels(pixel1.Red, pixel1.Green, pixel1.Blue, pixel1.Alfa, pixel2.Red, pixel2.Green, pixel2.Blue, pixel2.Alfa);
        }
        public static Color Mezclar(this Color pixel1, Color pixel2)
        {
            return MezclaPixels(pixel1.R, pixel1.G, pixel1.B, pixel1.A, pixel2.R, pixel2.G, pixel2.B, pixel2.A);
        }
        public static Color MezclaPixels(byte byteRBack, byte byteGBack, byte byteBBack, byte byteABack, byte byteRFore, byte byteGFore, byte byteBFore, byte byteAFore)
        {
            const int MAX = byte.MaxValue;
            const int MIN = byte.MinValue;
            int a, r, g, b;
            int alpha;

            if (byteABack != MIN && byteAFore != MIN)
            {//si los dos no son transparentes los mezclo
                alpha = byteAFore + 1;
                b = Math.Abs(alpha * byteBFore + (MAX - alpha) * byteBBack >> 8);
                g = Math.Abs(alpha * byteGFore + (MAX - alpha) * byteGBack >> 8);
                r = Math.Abs(alpha * byteRFore + (MAX - alpha) * byteRBack >> 8);
                a = byteAFore;

                if (byteABack == MAX)
                    a = MAX;
                if (a > MAX)
                    a = MAX;
                if (r > MAX)
                    r = MAX;
                if (g > MAX)
                    g = MAX;
                if (b > MAX)
                    b = MAX;
            }
            else if (byteABack < byteAFore)
            {//si uno es transparente pongo el colo que se ve
                r = byteRFore;
                g = byteGFore;
                b = byteBFore;
                a = byteAFore;
            }
            else
            {
                r = byteRBack;
                g = byteGBack;
                b = byteBBack;
                a = byteABack;
            }

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

        #region SetFragment 
        #region Sobrecarga
        public static unsafe void SetFragment(byte[] bmpTotal, Size sizeTotal, bool bmpTotalIsArgb, byte* bmpFragmentoInicio, Size sizeFragment, bool bmpFragmentoIsArgb, Utilitats.PointZ posicionFragmento)
        {
            fixed (byte* bmpTotalInicio = bmpTotal)
                SetFragment(bmpTotalInicio, sizeTotal.Height, sizeTotal.Width, bmpTotalIsArgb, bmpFragmentoInicio, sizeFragment.Height, sizeFragment.Width, bmpFragmentoIsArgb, new Point(posicionFragmento.X, posicionFragmento.Y));
        }
        public static unsafe void SetFragment(byte[] bmpTotal, int alturaBmpTotal, int anchuraBmpTotal, bool bmpTotalIsArgb, byte* bmpFragmentoInicio, int alturaBmpFragmento, int anchuraBmpFragmento, bool bmpFragmentoIsArgb, Utilitats.PointZ posicionFragmento)
        {
            fixed (byte* bmpTotalInicio = bmpTotal)
                SetFragment(bmpTotalInicio, alturaBmpTotal, anchuraBmpTotal, bmpTotalIsArgb, bmpFragmentoInicio, alturaBmpFragmento, anchuraBmpFragmento, bmpFragmentoIsArgb, new Point(posicionFragmento.X, posicionFragmento.Y));
        }
        public static unsafe void SetFragment(byte[] bmpTotal, Size sizeTotal, bool bmpTotalIsArgb, byte* bmpFragmentoInicio, Size sizeFragment, bool bmpFragmentoIsArgb, System.Drawing.Point posicionFragmento)
        {
            fixed (byte* bmpTotalInicio = bmpTotal)
                SetFragment(bmpTotalInicio, sizeTotal.Height, sizeTotal.Width, bmpTotalIsArgb, bmpFragmentoInicio, sizeFragment.Height, sizeFragment.Width, bmpFragmentoIsArgb, posicionFragmento);
        }
        public static void SetFragment(byte[] bmpTotal, Size sizeTotal, bool bmpTotalIsArgb, byte[] bmpFragmento, Size sizeFragment, bool bmpFragmentoIsArgb, Utilitats.PointZ posicionFragmento)
        {
            unsafe
            {
                fixed (byte* bmpTotalInicio = bmpTotal)
                    SetFragment(bmpTotalInicio, sizeTotal.Height, sizeTotal.Width, bmpTotalIsArgb, bmpFragmento, sizeFragment.Height, sizeFragment.Width, bmpFragmentoIsArgb, posicionFragmento);
            }
        }
        public static void SetFragment(byte[] bmpTotal, int alturaBmpTotal, int anchuraBmpTotal, bool bmpTotalIsArgb, byte[] bmpFragmento, int alturaBmpFragmento, int anchuraBmpFragmento, bool bmpFragmentoIsArgb, Utilitats.PointZ posicionFragmento)
        {
            unsafe
            {
                fixed (byte* bmpTotalInicio = bmpTotal)
                    SetFragment(bmpTotalInicio, alturaBmpTotal, anchuraBmpTotal, bmpTotalIsArgb, bmpFragmento, alturaBmpFragmento, anchuraBmpFragmento, bmpFragmentoIsArgb, posicionFragmento);
            }
        }
        public static void SetFragment(byte[] bmpTotal, Size sizeTotal, bool bmpTotalIsArgb, byte[] bmpFragmento, Size sizeFragment, bool bmpFragmentoIsArgb, System.Drawing.Point posicionFragmento)
        {
            unsafe
            {
                fixed (byte* bmpTotalInicio = bmpTotal)
                    SetFragment(bmpTotalInicio, sizeTotal, bmpTotalIsArgb, bmpFragmento, sizeFragment, bmpFragmentoIsArgb, posicionFragmento);
            }
        }
        public static unsafe void SetFragment(byte* bmpTotalInicio, Size sizeTotal, bool bmpTotalIsArgb, byte[] bmpFragmento, Size sizeFragment, bool bmpFragmentoIsArgb, Utilitats.PointZ posicionFragmento)
        {
            fixed (byte* bmpFragmentoInicio = bmpFragmento)
                SetFragment(bmpTotalInicio, sizeTotal.Height, sizeTotal.Width, bmpTotalIsArgb, bmpFragmentoInicio, sizeFragment.Height, sizeFragment.Width, bmpFragmentoIsArgb, new Point(posicionFragmento.X, posicionFragmento.Y));
        }
        public static unsafe void SetFragment(byte* bmpTotalInicio, int alturaBmpTotal, int anchuraBmpTotal, bool bmpTotalIsArgb, byte[] bmpFragmento, int alturaBmpFragmento, int anchuraBmpFragmento, bool bmpFragmentoIsArgb, Utilitats.PointZ posicionFragmento)
        {
            fixed (byte* bmpFragmentoInicio = bmpFragmento)
                SetFragment(bmpTotalInicio, alturaBmpTotal, anchuraBmpTotal, bmpTotalIsArgb, bmpFragmentoInicio, alturaBmpFragmento, anchuraBmpFragmento, bmpFragmentoIsArgb, new Point(posicionFragmento.X, posicionFragmento.Y));
        }
        public static unsafe void SetFragment(byte* bmpTotalInicio, Size sizeTotal, bool bmpTotalIsArgb, byte[] bmpFragmento, Size sizeFragment, bool bmpFragmentoIsArgb, System.Drawing.Point posicionFragmento)
        {
            fixed (byte* bmpFragmentoInicio = bmpFragmento)
                SetFragment(bmpTotalInicio, sizeTotal.Height, sizeTotal.Width, bmpTotalIsArgb, bmpFragmentoInicio, sizeFragment.Height, sizeFragment.Width, bmpFragmentoIsArgb, posicionFragmento);
        }

        public static unsafe void SetFragment(byte* bmpTotalInicio, Size sizeTotal, bool bmpTotalIsArgb, byte* bmpFragmentoInicio, Size sizeFragment, bool bmpFragmentoIsArgb, Utilitats.PointZ posicionFragmento)
        {
            SetFragment(bmpTotalInicio, sizeTotal.Height, sizeTotal.Width, bmpTotalIsArgb, bmpFragmentoInicio, sizeFragment.Height, sizeFragment.Width, bmpFragmentoIsArgb, new Point(posicionFragmento.X, posicionFragmento.Y));
        }
        public static unsafe void SetFragment(byte* bmpTotalInicio, int alturaBmpTotal, int anchuraBmpTotal, bool bmpTotalIsArgb, byte* bmpFragmentoInicio, int alturaBmpFragmento, int anchuraBmpFragmento, bool bmpFragmentoIsArgb, Utilitats.PointZ posicionFragmento)
        {
            SetFragment(bmpTotalInicio, alturaBmpTotal, anchuraBmpTotal, bmpTotalIsArgb, bmpFragmentoInicio, alturaBmpFragmento, anchuraBmpFragmento, bmpFragmentoIsArgb, new Point(posicionFragmento.X, posicionFragmento.Y));
        }
        public static unsafe void SetFragment(byte* bmpTotalInicio, Size sizeTotal, bool bmpTotalIsArgb, byte* bmpFragmentoInicio, Size sizeFragment, bool bmpFragmentoIsArgb, System.Drawing.Point posicionFragmento)
        {

            SetFragment(bmpTotalInicio, sizeTotal.Height, sizeTotal.Width, bmpTotalIsArgb, bmpFragmentoInicio, sizeFragment.Height, sizeFragment.Width, bmpFragmentoIsArgb, posicionFragmento);
        }
        #endregion
        public static unsafe void SetFragment(byte* bmpTotalInicio, int alturaBmpTotal, int anchuraBmpTotal, bool bmpTotalIsArgb, byte* bmpFragmentoInicio, int alturaBmpFragmento, int anchuraBmpFragmento, bool bmpFragmentoIsArgb, System.Drawing.Point posicionFragmento)
        {
            #region Variables
            const int OPCIONIGUALES = 0;
            const int OPCIONSINCON = 1;
            const int OPCIONCONSIN = 2;
            //por acabar
            int opcion;
            int totalPixelesLinea;
            int lineas;
            int inicioParteFragmentoLinea;
            int inicioParteTotalLinea;
            byte*[] ptrsBmpTotal;
            byte*[] ptrsBmpFragmento;
            int bytesPixelBmpTotal = bmpTotalIsArgb ? 4 : 3;
            int bytesPixelBmpFragmento = bmpFragmentoIsArgb ? 4 : 3;
            int bytesLineaBmpTotal = bytesPixelBmpTotal * anchuraBmpTotal;
            int bytesLineaBmpFragmento = bytesPixelBmpFragmento * anchuraBmpFragmento;
            #endregion
            //tener en cuenta las posiciones negativas del fragmento...
            if (posicionFragmento.Y >= 0 && posicionFragmento.Y < alturaBmpTotal || posicionFragmento.Y + alturaBmpFragmento > 0 && posicionFragmento.Y < 0)
            {
                if (posicionFragmento.X >= 0 && posicionFragmento.X < anchuraBmpTotal || posicionFragmento.X + anchuraBmpFragmento > 0 && posicionFragmento.X < 0)
                {//si no esta dentro de la imagen no hace falta hacer nada :D
                    #region Medidas Parte a poner

                    if (posicionFragmento.Y < 0)
                    {
                        if (alturaBmpTotal > posicionFragmento.Y + alturaBmpFragmento)
                        {
                            lineas = posicionFragmento.Y + alturaBmpFragmento;
                        }
                        else lineas = alturaBmpTotal;

                    }
                    else if (posicionFragmento.Y + alturaBmpFragmento > alturaBmpTotal)
                    {
                        lineas = alturaBmpTotal - posicionFragmento.Y;
                    }
                    else
                    {
                        lineas = alturaBmpFragmento;
                    }

                    if (posicionFragmento.X < 0)
                    {
                        if (anchuraBmpTotal > posicionFragmento.X + anchuraBmpFragmento)
                        {
                            totalPixelesLinea = posicionFragmento.X + anchuraBmpFragmento;
                        }
                        else totalPixelesLinea = anchuraBmpTotal;

                    }
                    else if (posicionFragmento.X + anchuraBmpFragmento > anchuraBmpTotal)
                    {
                        totalPixelesLinea = anchuraBmpTotal - posicionFragmento.X;
                    }
                    else
                    {
                        totalPixelesLinea = anchuraBmpFragmento;
                    }

                    ptrsBmpFragmento = new byte*[lineas];
                    ptrsBmpTotal = new byte*[lineas];
                    #endregion
                    #region Posiciono Ptrs
                    //posiciono ptrs hasta la linea donde empieza
                    if (posicionFragmento.Y < 0)
                    {
                        //posiciono el fragmento
                        bmpFragmentoInicio += (bytesLineaBmpFragmento * posicionFragmento.Y);

                    }
                    else
                    {
                        //posiciono el grande
                        bmpTotalInicio += (bytesLineaBmpTotal * posicionFragmento.Y);

                    }
                    //pongo los inicios tener en cuenta los bytes por pixel
                    if (posicionFragmento.X < 0)
                    {
                        inicioParteFragmentoLinea = posicionFragmento.X * (-1) * bytesPixelBmpFragmento;
                        inicioParteTotalLinea = 0;
                    }
                    else
                    {
                        inicioParteTotalLinea = posicionFragmento.X * bytesPixelBmpTotal;
                        inicioParteFragmentoLinea = 0;
                    }

                    for (int i = 0; i < lineas; i++)
                    {
                        ptrsBmpFragmento[i] = bmpFragmentoInicio + inicioParteFragmentoLinea;
                        bmpFragmentoInicio += bytesLineaBmpFragmento;
                        ptrsBmpTotal[i] = bmpTotalInicio + inicioParteTotalLinea;
                        bmpTotalInicio += bytesLineaBmpTotal;
                    }
                    #endregion
                    #region Poner Lineas
                    opcion = bmpTotalIsArgb.Equals(bmpFragmentoIsArgb) ? OPCIONIGUALES : bmpTotalIsArgb ? OPCIONCONSIN : OPCIONSINCON;

                    switch (opcion)
                    {
                        //pongo las lineas
                        case OPCIONIGUALES:
                            for (int i = 0; i < lineas; i++)
                            {
                                SetLinea(ptrsBmpTotal[i], bytesPixelBmpTotal, ptrsBmpFragmento[i], totalPixelesLinea);
                            }
                            break;
                        case OPCIONCONSIN:
                            for (int i = 0; i < lineas; i++)
                            {
                                SetLineaCS(ptrsBmpTotal[i], ptrsBmpFragmento[i], totalPixelesLinea);
                            }
                            break;
                        case OPCIONSINCON:
                            for (int i = 0; i < lineas; i++)
                            {
                                SetLineaSC(ptrsBmpTotal[i], ptrsBmpFragmento[i], totalPixelesLinea);
                            }
                            break;
                    }
                    #endregion
                }
            }
        }
        static unsafe void SetLineaSC(byte* ptrBmpTotal, byte* ptrBmpFragmento, int totalPixelesLinea)
        {
            const byte SINTRANSPARENCIA = 0xFF;
            const byte TRANSPARENTE = 0x0;
            const int RGB = 3;
            const int ARGB = RGB + 1;
            Color aux;
            byte* ptSinTransparencia;
            for (int j = 0; j < totalPixelesLinea; j++)
            {
                ptSinTransparencia = ptrBmpFragmento + Pixel.A;
                //pongo cada pixel
                if (*ptSinTransparencia == SINTRANSPARENCIA)
                {
                    ptrBmpFragmento++;//me salto el byte de la transparencia porque la imagenTotal no tiene
                    for (int k = 0; k < RGB; k++)
                    {
                        *ptrBmpTotal = *ptrBmpFragmento;
                        ptrBmpTotal++;
                        ptrBmpFragmento++;
                    }
                }
                else
                {
                    if (*ptSinTransparencia != TRANSPARENTE)//si no es transparente es que tengo que mezclarlo sino es que lo tengo que saltar :D
                    {
                        //tengo que mezclarlo
                        aux = MezclaPixels(ptrBmpTotal[Pixel.R], ptrBmpTotal[Pixel.G], ptrBmpTotal[Pixel.B], SINTRANSPARENCIA, ptrBmpFragmento[Pixel.R], ptrBmpFragmento[Pixel.G], ptrBmpFragmento[Pixel.B], ptrBmpFragmento[Pixel.A]);

                        *(ptrBmpTotal + Pixel.R) = aux.R;
                        *(ptrBmpTotal + Pixel.G) = aux.G;
                        *(ptrBmpTotal + Pixel.B) = aux.B;
                    }

                    ptrBmpTotal += RGB;
                    ptrBmpFragmento += ARGB;
                }
            }

        }
        static unsafe void SetLineaCS(byte* ptrBmpTotal, byte* ptrBmpFragmento, int totalPixelesLinea)
        {

            const byte SINTRANSPARENCIA = 0xFF;
            const int RGB = 3;
            for (int j = 0; j < totalPixelesLinea; j++)
            {
                //pongo cada pixel
                *ptrBmpTotal = SINTRANSPARENCIA;//como no tiene transparencia pongo el byte de la transparencia a sin
                ptrBmpTotal++;

                for (int k = 0; k < RGB; k++)
                {
                    *ptrBmpTotal = *ptrBmpFragmento;
                    ptrBmpTotal++;
                    ptrBmpFragmento++;
                }


            }
        }
        static unsafe void SetLinea(byte* ptrBmpTotal, int bytesPixel, byte* ptrBmpFragmento, int totalPixelesLinea)
        {
            const byte SINTRANSPARENCIA = 0xFF;
            const byte TRANSPARENTE = 0x0;
            const int RGB = 3;
            const int ARGB = RGB + 1;
            Color aux;
            bool isArgb = bytesPixel == ARGB;
            byte* ptSinTransparencia;
            //pongo cada pixel
            for (int j = 0; j < totalPixelesLinea; j++)
            {
                ptSinTransparencia = ptrBmpFragmento + Pixel.A;
                if (!isArgb || *ptSinTransparencia == SINTRANSPARENCIA)
                {
                    //pongo cada byte
                    for (int k = 0; k < bytesPixel; k++)
                    {
                        *ptrBmpTotal = *ptrBmpFragmento;
                        ptrBmpTotal++;
                        ptrBmpFragmento++;
                    }
                }
                else
                {
                    if (*ptSinTransparencia != TRANSPARENTE)
                    {
                        //tengo que mezclarlo
                        aux = MezclaPixels(ptrBmpTotal[Pixel.R], ptrBmpTotal[Pixel.G], ptrBmpTotal[Pixel.B], ptrBmpTotal[Pixel.A], ptrBmpFragmento[Pixel.R], ptrBmpFragmento[Pixel.G], ptrBmpFragmento[Pixel.B], ptrBmpFragmento[Pixel.A]);
                        *(ptrBmpTotal + Pixel.A) = aux.A;
                        *(ptrBmpTotal + Pixel.R) = aux.R;
                        *(ptrBmpTotal + Pixel.G) = aux.G;
                        *(ptrBmpTotal + Pixel.B) = aux.B;
                    }

                    ptrBmpTotal += ARGB;
                    ptrBmpFragmento += ARGB;
                }

            }



        }
        public static void SetFragment(this Bitmap bmpTotal, Bitmap bmpFragmento, Point posicionFragmento = default)
        {
            Drawing.ImageBase img = new Drawing.ImageBase(bmpFragmento);
            SetFragment(bmpTotal, img.Array, bmpFragmento.Size, Drawing.ImageBase.ISARGB, posicionFragmento);
        }
        public static void SetFragment(this Bitmap bmpTotal, byte[] bytesFragmento,Size sizeFragmento,bool isArgbFragmento, Point posicionFragmento = default)
        {
            unsafe
            {

                bmpTotal.TrataBytes((ptrBmpTotal) =>
                {
                    fixed (byte* ptrBmpFragmento = bytesFragmento)
                    {

                        SetFragment(ptrBmpTotal, bmpTotal.Height, bmpTotal.Width, bmpTotal.IsArgb(), ptrBmpFragmento, sizeFragmento.Height, sizeFragmento.Width, isArgbFragmento, posicionFragmento);

                    }

                });


            }
        }
        #endregion
        #region Por Hacer
        //public static Rectangle GetRectangleImgWithoutTransparentMarc(this Bitmap bmp)
        //{
        //    int x = 0, y = 0, width = bmp.Width, height = bmp.Height;
        //    unsafe
        //    {
        //        byte*[] ptrs;
        //        Utilitats.V2.Color* ptColor;
        //        if (width < height)
        //        {
        //            //height
        //        }
        //        else
        //        {
        //            //width
        //        }

        //    }
        //    return new Rectangle(x, y, width, height);
        //}
        //public static Bitmap GetImgWithoutTransparentMarc(this Bitmap bmp)
        //{
        //    return bmp.Recorta(bmp.GetRectangleImgWithoutTransparentMarc());
        //}
        #endregion
        public static Bitmap Recorta(this Bitmap bmp,Rectangle rctRecorte)
        {
            Bitmap bmpSinMarco = new Bitmap(rctRecorte.Width, rctRecorte.Height, Drawing.ImageBase.DefaultPixelFormat);
            Drawing.ImageBase imgBase = new Drawing.ImageBase(bmp);
            bmpSinMarco.SetFragment(imgBase.Array,bmp.Size,Drawing.ImageBase.ISARGB, rctRecorte.Location);
            return bmpSinMarco;
        }
    }

}