using Gabriel.Cat.S.Drawing;
using Gabriel.Cat.S.Utilitats;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text;

namespace Gabriel.Cat.S.Extension
{
    public  static unsafe class BitmapExtension
    {
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

    }
}
