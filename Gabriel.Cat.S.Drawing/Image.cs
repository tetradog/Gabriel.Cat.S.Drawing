using Gabriel.Cat.S.Extension;
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
                b = alpha * byteBFore + (MAX - alpha) * byteBBack >> 8;
                g = alpha * byteGFore + (MAX - alpha) * byteGBack >> 8;
                r = alpha * byteRFore + (MAX - alpha) * byteRBack >> 8;
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

        #region SetFragment por acabar
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
            const int R = 1, G = R + 1, B = G + 1, A = B + 1;
            Color aux;
            for (int j = 0; j < totalPixelesLinea; j++)
            {
                //pongo cada pixel
                if (*ptrBmpFragmento == SINTRANSPARENCIA)
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
                    if (*ptrBmpFragmento != TRANSPARENTE)//si no es transparente es que tengo que mezclarlo sino es que lo tengo que saltar :D
                    {
                        //tengo que mezclarlo
                        aux = MezclaPixels(*(ptrBmpTotal + R), *(ptrBmpTotal + G), *(ptrBmpTotal + B), SINTRANSPARENCIA, *(ptrBmpFragmento + R), *(ptrBmpFragmento + G), *(ptrBmpFragmento + B), *(ptrBmpFragmento + A));

                        *(ptrBmpTotal + R) = aux.R;
                        *(ptrBmpTotal + G) = aux.G;
                        *(ptrBmpTotal + B) = aux.B;
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
            const int R = 1, G = R + 1, B = G + 1, A = B + 1;
            Color aux;
            bool isArgb = bytesPixel == ARGB;

            //pongo cada pixel
            for (int j = 0; j < totalPixelesLinea; j++)
            {
                if (!isArgb || *ptrBmpFragmento == SINTRANSPARENCIA)
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
                    if (*ptrBmpFragmento != TRANSPARENTE)
                    {
                        //tengo que mezclarlo
                        aux = MezclaPixels(*(ptrBmpTotal + R), *(ptrBmpTotal + G), *(ptrBmpTotal + B), *(ptrBmpTotal + A), *(ptrBmpFragmento + R), *(ptrBmpFragmento + G), *(ptrBmpFragmento + B), *(ptrBmpFragmento + A));

                        *(ptrBmpTotal + A) = aux.A;
                        *(ptrBmpTotal + R) = aux.R;
                        *(ptrBmpTotal + G) = aux.G;
                        *(ptrBmpTotal + B) = aux.B;
                    }

                    ptrBmpTotal += ARGB;
                    ptrBmpFragmento += ARGB;
                }

            }



        }
        public static void SetFragment(this Bitmap bmpTotal, Bitmap bmpFragmento, Point posicionFragmento = default)
        {
            unsafe
            {

                bmpTotal.TrataBytes((ptrBmpTotal) => {
                    fixed (byte* ptrBmpFragmento = bmpFragmento.GetBytes())
                    {

                        SetFragment(ptrBmpTotal, bmpTotal.Height, bmpTotal.Width, bmpTotal.IsArgb(), ptrBmpFragmento, bmpFragmento.Height, bmpFragmento.Width, bmpFragmento.IsArgb(), posicionFragmento);

                    }

                });


            }
        }
        #endregion

    }

}