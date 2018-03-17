using Gabriel.Cat.S.Drawing;
using Gabriel.Cat.S.Utilitats;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text;

namespace Gabriel.Cat.S.Extension
{
    public  static unsafe class BitmapExtension
    {
        delegate byte[] MetodoColor(byte[] colorValue, byte[] colorKey);
        delegate void MetodoTrataMientoPixel(ref byte r, ref byte g, ref byte b);

        public static unsafe void SetFragment(byte* bmpTotalInicio, int alturaBmpTotal, int anchuraBmpTotal, bool bmpTotalIsArgb, byte* bmpFragmentoInicio, int alturaBmpFragmento, int anchuraBmpFragmento, bool bmpFragmentoIsArgb, System.Drawing.Point posicionFragmento)
        {
            const int OPCIONIGUALES = 0;
            const int OPCIONSINCON = 1;
            const int OPCIONCONSIN = 2;
            //por acabar
            int totalPixelesLinea;
            int lineas = alturaBmpTotal - posicionFragmento.Y;//pensar en las posiciones negativas
            byte*[] ptrsBmpTotal;
            byte*[] ptrsBmpFragmento;
            int bytesPixelBmpTotal = bmpTotalIsArgb ? 4 : 3;
            int bytesPixelBmpFragmento = bmpFragmentoIsArgb ? 4 : 3;
            int opcion;
            //tener en cuenta las posiciones negativas del fragmento...
            if (lineas > 0)
            {//si no esta dentro de la imagen no hace falta hacer nada :D
                if (lineas < alturaBmpFragmento)
                    lineas = alturaBmpFragmento;
                totalPixelesLinea = anchuraBmpTotal - posicionFragmento.X;
                if (totalPixelesLinea < anchuraBmpFragmento)
                    totalPixelesLinea = anchuraBmpFragmento;

                ptrsBmpFragmento = new byte*[lineas];
                ptrsBmpTotal = new byte*[lineas];
                //posiciono todos los punteros

                //pongo  la opcion aqui asi solo se escoge una vez y no en cada linea como estaba antes :)
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
                            SetLineaCS(ptrsBmpTotal[i], ptrsBmpFragmento[i], totalPixelesLinea);
                        }
                        break;
                }
            }
        }
        static unsafe void SetLineaSC(byte* ptrBmpTotal, byte* ptrBmpFragmento, int totalPixelesLinea)
        {
            const int RGB = 3;
            for (int j = 0; j < totalPixelesLinea; j++)
            {
                //pongo cada pixel
                ptrBmpFragmento++;//me salto el byte de la transparencia porque la imagenTotal no tiene
                for (int k = 0; k < RGB; k++)
                {
                    *ptrBmpTotal = *ptrBmpFragmento;
                    ptrBmpTotal++;
                    ptrBmpFragmento++;
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

            //pongo cada pixel
            for (int j = 0; j < totalPixelesLinea; j++)
            {

                //pongo cada byte
                for (int k = 0; k < bytesPixel; k++)
                {
                    *ptrBmpTotal = *ptrBmpFragmento;
                    ptrBmpTotal++;
                    ptrBmpFragmento++;
                }
            }



        }
        public static void SetFragment(this Bitmap bmpTotal, Bitmap bmpFragmento, Point posicionFragmento)
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
       
        public static BitmapAnimated ToAnimatedBitmap(this IEnumerable<Bitmap> bmpsToAnimate, bool repetirSiempre = true)
        {
            return bmpsToAnimate.ToAnimatedBitmap(repetirSiempre, 500);
        }
        public static BitmapAnimated ToAnimatedBitmap(this IEnumerable<Bitmap> bmpsToAnimate, bool repetirSiempre = true, params int[] delay)
        {
            return new BitmapAnimated(bmpsToAnimate, delay) { AnimarCiclicamente = repetirSiempre };
        }
        public static Bitmap RandomPixels(this Bitmap imgRandom)
        {
            const int MAXPRIMERO = 19;
            return imgRandom.RandomPixels(Convert.ToInt32(Math.Sqrt(imgRandom.Width) % MAXPRIMERO));
        }
        public static Bitmap RandomPixels(this Bitmap imgRandom, int cuadradosPorLinea)
        {
            //hay un bug y no lo veo... no hace cuadrados...
            unsafe
            {
                imgRandom.TrataBytes((MetodoTratarBytePointer)((bytesImg) =>
                {
                    const int PRIMERODEFAULT = 13;//al ser un numero Primo no hay problemas
                    System.Drawing.Color[] cuadrados;
                    System.Drawing.Color colorActual;
                    int a = 3, r = 0, g = 1, b = 2;
                    int lenght = imgRandom.LengthBytes();
                    int pixel = imgRandom.IsArgb() ? 4 : 3;
                    int pixelsLineasHechas;
                    int sumaX;
                    int numPixeles;
                    int posicionCuadrado = 0;
                    if (cuadradosPorLinea < 1)
                        cuadradosPorLinea = PRIMERODEFAULT;
                    else
                        cuadradosPorLinea = cuadradosPorLinea.DamePrimeroCercano();
                    numPixeles = imgRandom.Width / cuadradosPorLinea;
                    numPixeles = numPixeles.DamePrimeroCercano();
                    cuadrados = DamePixeles(cuadradosPorLinea);
                    colorActual = cuadrados[posicionCuadrado];
                    for (int y = 0, xMax = imgRandom.Width * pixel; y < imgRandom.Height; y++)
                    {
                        pixelsLineasHechas = y * xMax;
                        if (y % numPixeles == 0)
                        {
                            cuadrados = DamePixeles(cuadradosPorLinea);
                        }
                        for (int x = 0; x < xMax; x += pixel)
                        {
                            if (x % numPixeles == 0)
                            {
                                colorActual = cuadrados[++posicionCuadrado % cuadrados.Length];
                            }
                            sumaX = pixelsLineasHechas + x;
                            if (pixel == 4)
                            {
                                bytesImg[sumaX + a] = byte.MaxValue;
                            }
                            bytesImg[sumaX + r] = colorActual.R;
                            bytesImg[sumaX + g] = colorActual.G;
                            bytesImg[sumaX + b] = colorActual.B;
                        }
                    }
                })
                                    );
            }
            return imgRandom;
        }
        private static System.Drawing.Color[] DamePixeles(int numPixeles)
        {
            System.Drawing.Color[] pixeles = new System.Drawing.Color[numPixeles];
            for (int i = 0; i < pixeles.Length; i++)
                pixeles[i] = System.Drawing.Color.FromArgb(MiRandom.Next());
            return pixeles;
        }
        public static byte[] GetBytes(this Bitmap bmp)
        {
            BitmapData bmpData = bmp.LockBits();
            // Get the address of the first line.
            IntPtr ptr = bmpData.Scan0;

            // Declare an array to hold the bytes of the bitmap.
            int bytes = Math.Abs(bmpData.Stride) * bmp.Height;

            byte[] rgbValues = new byte[bytes];

            // Copy the RGB values into the array.
            ptr.CopyTo(rgbValues);
            // Unlock the bits.
            bmp.UnlockBits(bmpData);
            return rgbValues;
        }
        public static void SetBytes(this Bitmap bmp, byte[] rgbValues)
        {
            BitmapData bmpData = bmp.LockBits();
            // Get the address of the first line.
            IntPtr ptr = bmpData.Scan0;

            // Declare an array to hold the bytes of the bitmap.
            int bytes = Math.Abs(bmpData.Stride) * bmp.Height;
            if (bytes != rgbValues.Length)
                throw new Exception("La array de bytes no se corresponde a la imagen");

            // Copy the RGB values back to the bitmap
            rgbValues.CopyTo(ptr);

            // Unlock the bits.
            bmp.UnlockBits(bmpData);

        }
        public static BitmapData LockBits(this Bitmap bmp)
        {
            return bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), System.Drawing.Imaging.ImageLockMode.ReadWrite, bmp.PixelFormat);
        }
        public static MemoryStream ToStream(this Bitmap bmp, bool useRawFormat = false)
        {
            MemoryStream memory;
            ImageFormat format = useRawFormat ? bmp.RawFormat : bmp.IsArgb() ? ImageFormat.Png : ImageFormat.Jpeg;//no se porque aun pero no funciona...mejor pasarla a png
            memory = ToStream(bmp, format);
            return memory;

        }
        public static MemoryStream ToStream(this Bitmap bmp, ImageFormat format)
        {
            MemoryStream stream = new MemoryStream();
            string path;
            FileStream fs;
            try
            {
                new Bitmap(bmp).Save(stream, format);
            }
            catch (Exception ex)
            {
                path = System.IO.Path.GetRandomFileName();
                format = bmp.IsArgb() ? ImageFormat.Png : ImageFormat.Jpeg;
                new Bitmap(bmp).Save(path, format);
                fs = File.OpenRead(path);
                new Bitmap(fs).Save(stream, format);
                fs.Close();
                File.Delete(path);
            }
            return new MemoryStream(stream.GetAllBytes());
        }

        public static void TrataBytes(this Bitmap bmp, MetodoTratarByteArray metodo)
        {
            BitmapData bmpData = bmp.LockBits();
            // Get the address of the first line.
            IntPtr ptr = bmpData.Scan0;

            // Declare an array to hold the bytes of the bitmap.
            int bytes = System.Math.Abs(bmpData.Stride) * bmp.Height;

            byte[] rgbValues = new byte[bytes];

            // Copy the RGB values into the array.
            ptr.CopyTo(rgbValues);
            if (metodo != null)
            {
                metodo(rgbValues);//se modifican los bytes :D
                                  // Copy the RGB values back to the bitmap
                rgbValues.CopyTo(ptr);
            }
            // Unlock the bits.
            bmp.UnlockBits(bmpData);

        }
        public static unsafe void TrataBytes(this Bitmap bmp, MetodoTratarBytePointer metodo)
        {

            BitmapData bmpData = bmp.LockBits();
            // Get the address of the first line.

            IntPtr ptr = bmpData.Scan0;
            if (metodo != null)
            {
                metodo((byte*)ptr.ToPointer());//se modifican los bytes :D
            }
            // Unlock the bits.
            bmp.UnlockBits(bmpData);

        }
        public static int LengthBytes(this Bitmap bmp)
        {
            int multiplicadorPixel = bmp.IsArgb() ? 4 : 3;
            return bmp.Height * bmp.Width * multiplicadorPixel;
        }
        public static bool IsArgb(this Bitmap bmp)
        {
            return bmp.PixelFormat.IsArgb();
        }

        unsafe delegate void TratarImg(byte* ptrImg);
        #region BitmapImportado
        /// <summary>
        /// Recorta una imagen en formato Bitmap
        /// </summary>
        /// <param name="localizacion">localizacion de la esquina izquierda de arriba</param>
        /// <param name="tamaño">tamaño del rectangulo</param>
        /// <param name="bitmapARecortar">bitmap para recortar</param>
        /// <returns>bitmap resultado del recorte</returns>
        public static Bitmap Recortar(this Bitmap bitmapARecortar, Point localizacion, Size tamaño)
        {

            Rectangle rect = new Rectangle(localizacion.X, localizacion.Y, tamaño.Width, tamaño.Height);
            Bitmap cropped = bitmapARecortar.Clone(rect, bitmapARecortar.PixelFormat);
            return cropped;

        }
        public static Bitmap Escala(this Bitmap imgAEscalar, decimal escala)
        {
            return Resize(imgAEscalar, new Size(Convert.ToInt32(imgAEscalar.Size.Width * escala), Convert.ToInt32(imgAEscalar.Size.Height * escala)));
        }
        public static Bitmap Resize(this Bitmap imgToResize, Size size)
        {
            Bitmap bmpResized;
            try
            {
                bmpResized = new Bitmap(size.Width, size.Height);
                using (Graphics g = Graphics.FromImage((System.Drawing.Image)bmpResized))
                {
                    g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                    g.DrawImage(imgToResize, 0, 0, size.Width, size.Height);
                }

            }
            catch
            {
                bmpResized = imgToResize;
            }

            return bmpResized;
        }




        public static Color[,] GetColorMatriu(this Bitmap bmp)
        {
            Color[,] matriz = new Color[bmp.Width, bmp.Height];
            ulong posicion = 0;
            byte a, r, g, b;
            unsafe
            {
                bmp.TrataBytes(((MetodoTratarBytePointer)((ptrBytesBmp) =>
                {

                    byte* ptBytesBmp = ptrBytesBmp;
                    for (int y = 0, yFinal = bmp.Width; y < yFinal; y++)
                        for (int x = 0, xFinal = bmp.Height; x < xFinal; x++, posicion += 4)
                        {
                            a = *ptBytesBmp;
                            ptBytesBmp++;
                            r = *ptBytesBmp;
                            ptBytesBmp++;
                            g = *ptBytesBmp;
                            ptBytesBmp++;
                            b = *ptBytesBmp;
                            ptBytesBmp++;
                            matriz[x, y] = Color.FromArgb(a, r, g, b);
                        }

                })));
            }
            return matriz;
        }
        public static Bitmap GetBitmap(this Color[,] array)
        {
            Bitmap bmp = new Bitmap(array.GetLength(DimensionMatriz.X), array.GetLength(DimensionMatriz.Y));
            unsafe
            {
                bmp.TrataBytes(((MetodoTratarBytePointer)((ptrBytesBmp) =>
                {
                    ulong posicion = 0;
                    byte* ptBytesBmp = ptrBytesBmp;
                    for (ulong y = 0, yFinal = (ulong)array.GetLongLength((int)DimensionMatriz.Y); y < yFinal; y++)
                        for (ulong x = 0, xFinal = (ulong)array.GetLongLength((int)DimensionMatriz.X); x < xFinal; x++, posicion += 4)
                        {
                            *ptBytesBmp = array[x, y].A;
                            ptBytesBmp++;
                            *ptBytesBmp = array[x, y].R;
                            ptBytesBmp++;
                            *ptBytesBmp = array[x, y].G;
                            ptBytesBmp++;
                            *ptBytesBmp = array[x, y].B;
                            ptBytesBmp++;
                        }

                })));
            }
            return bmp;
        }
        public static byte[,] GetMatriuBytes(this Bitmap bmp)
        {
            byte[] bytesArray = bmp.GetBytes();
            return bytesArray.ToMatriu(bmp.Height, DimensionMatriz.Y);
        }
        public static void SetMatriuBytes(this Bitmap bmp, byte[,] matriuBytes)
        {
            if (bmp.Height * bmp.Width * (bmp.IsArgb() ? 4 : 3) != matriuBytes.GetLength(DimensionMatriz.Y) * matriuBytes.GetLength(DimensionMatriz.X))
                throw new Exception("La matriz no tiene las medidas de la imagen");
            unsafe
            {
                bmp.TrataBytes(((MetodoTratarBytePointer)((ptrBytesBmp) =>
                {
                    byte* ptBytesBmp = ptrBytesBmp;
                    for (long y = 0, yFinal = matriuBytes.GetLongLength((int)DimensionMatriz.Y); y < yFinal; y++)
                        for (long x = 0, xFinal = matriuBytes.GetLongLength((int)DimensionMatriz.X); x < xFinal; x++)
                        {
                            *ptBytesBmp = matriuBytes[x, y];
                            ptBytesBmp++;
                        }


                })));
            }
        }


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
        public static Bitmap Clone(this Bitmap bmp, PixelFormat format)
        {
            return bmp.Clone(new Rectangle(new Point(), bmp.Size), format);
        }
        #endregion

        public static void CambiarPixel(this Bitmap bmp, Color aEnontrar, Color aDefinir)
        {
            bmp.CambiarPixel(new KeyValuePair<Color, Color>[] { new KeyValuePair<Color, Color>(aEnontrar, aDefinir) });
        }
        public static void CambiarPixel(this Bitmap bmp, IEnumerable<KeyValuePair<Color, Color>> colorsKeyValue)
        {
            MetodoColor metodo = (colorValue, colorKey) =>
            {
                return colorValue;
            };
            ICambiaPixel(bmp, colorsKeyValue, metodo);
        }
        public static void EfectoPixel(this Bitmap bmp, Color aMezclarConTodos, bool saltarsePixelsTransparentes = true)
        {
            const int TOTALARGBBYTES = 4;
            int incremento = bmp.IsArgb() ? 4 : 3;
            int aux;
            bool mezclar = true;
            const byte TRANSPARENTE = 0x00;
            unsafe
            {
                bmp.TrataBytes(((MetodoTratarBytePointer)((ptrbyteArray) =>
                {
                    byte* ptByteArray = ptrbyteArray;
                    for (int i = 0, iFinal = bmp.LengthBytes(); i < iFinal; i += incremento)
                    {
                        if (incremento == TOTALARGBBYTES)
                        {
                            if (saltarsePixelsTransparentes)
                                mezclar = *ptByteArray != TRANSPARENTE;
                            if (mezclar)
                            {
                                //MEZCLO LA A
                                aux = *ptByteArray + aMezclarConTodos.A;
                                if (aux > 255) aux = 255;
                                *ptByteArray = (byte)aux;
                                ptByteArray++;
                            }
                        }
                        if (mezclar)
                        {
                            //MEZCLO LA R
                            aux = *ptByteArray + aMezclarConTodos.R;
                            if (aux > 255) aux = 255;
                            *ptByteArray = (byte)aux;
                            ptByteArray++;
                            //MEZCLO LA G
                            aux = *ptByteArray + aMezclarConTodos.G;
                            if (aux > 255) aux = 255;
                            *ptByteArray = (byte)aux;
                            ptByteArray++;
                            //MEZCLO LA B
                            aux = *ptByteArray + aMezclarConTodos.B;
                            if (aux > 255) aux = 255;
                            *ptByteArray = (byte)aux;
                            ptByteArray++;
                        }
                    }
                })));
            }
        }
        public static void MezclaPixel(this Bitmap bmp, Color aEnontrar, Color aDefinir)
        {
            bmp.MezclaPixel(new KeyValuePair<Color, Color>[] { new KeyValuePair<Color, Color>(aEnontrar, aDefinir) });
        }
        public static void MezclaPixel(this Bitmap bmp, IEnumerable<KeyValuePair<Color, Color>> colorsKeyValue)
        {
            MetodoColor metodo = (colorValue, arrayKey) =>
            {
                const int TOTALBYTESCOLOR = 4;
                byte[] colorMezclado = null;
                int aux;
                if (colorValue != null && arrayKey != null)
                {
                    unsafe
                    {
                        colorMezclado = new byte[TOTALBYTESCOLOR];
                        fixed (byte* ptrColorMezclado = colorMezclado, ptrArrayKey = arrayKey)
                        {
                            byte* ptColorMezclado = ptrColorMezclado, ptArrayKey = ptrArrayKey;
                            for (int i = 0; i < TOTALBYTESCOLOR; i++)
                            {

                                aux = colorValue[i] + *ptArrayKey;
                                aux /= 2;
                                *ptColorMezclado = (byte)aux;
                                //if (aux[i] > 255) aux[i] = 255;
                                ptColorMezclado++;
                                ptArrayKey++;

                            }
                        }
                    }

                }
                /*   else if (colorValue != null)
                       colorMezclado = colorValue;
                   else
                       colorMezclado =Color.FromArgb(Serializar.ToInt( arrayKey));*/
                return colorMezclado;
            };
            ICambiaPixel(bmp, colorsKeyValue, metodo);
        }
        static void ICambiaPixel(Bitmap bmp, IEnumerable<KeyValuePair<Color, Color>> colorsKeyValue, MetodoColor metodo)
        {
            const int TOTALBYTESCOLOR = 4;
            DiccionarioColor2 diccionario = new DiccionarioColor2(colorsKeyValue);
            byte[] colorLeido;
            byte[] colorObtenido;
            const byte AOPACA = 0xFF;
            int incremento = bmp.IsArgb() ? 4 : 3;
            unsafe
            {
                bmp.TrataBytes(((MetodoTratarBytePointer)((ptrBytesBmp) =>
                {
                    byte* ptColorLeido, ptColorObtenido;
                    byte* ptBytesBmp = ptrBytesBmp;
                    for (int i = 0, iFin = bmp.LengthBytes(); i < iFin; i += incremento)
                    {
                        colorLeido = new byte[4];
                        fixed (byte* ptrColorLeido = colorLeido)
                        {
                            ptColorLeido = ptrColorLeido;
                            if (incremento == TOTALBYTESCOLOR)
                            {
                                *ptColorLeido = *ptBytesBmp;
                                ptBytesBmp++;
                            }
                            else
                            {
                                *ptColorLeido = AOPACA;

                            }
                            ptColorLeido++;
                            for (int j = 1; j < incremento; j++)
                            {
                                *ptColorLeido = *ptBytesBmp;
                                ptBytesBmp++;
                                ptColorLeido++;
                            }
                            ptBytesBmp -= incremento;//vuelvo a poner el puntero al principio del color para sobreescribirlo con el nuevo
                        }

                        colorObtenido = metodo(diccionario.ObtenerPrimero(colorLeido), colorLeido);
                        if (colorObtenido != null)
                        {
                            fixed (byte* ptrColorObtenido = colorObtenido)
                            {
                                ptColorObtenido = ptrColorObtenido;
                                for (int j = 0; j < incremento; j++)
                                {
                                    *ptBytesBmp = *ptColorObtenido;
                                    ptBytesBmp++;
                                    ptColorObtenido++;
                                }


                            }
                        }

                    }
                })));
            }
        }

    }
}