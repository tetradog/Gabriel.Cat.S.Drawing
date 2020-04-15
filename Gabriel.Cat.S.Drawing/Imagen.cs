using Gabriel.Cat.S.Extension;
using Gabriel.Cat.S.Utilitats;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace Gabriel.Cat.S.Drawing
{
    public enum FiltroImagen
    {
        Red,
        Green,
        Blue,
        GrayScale,
        Sepia,
        Inverted
    }
    public class Collage : ImageFragment, IEnumerable<ImageFragment>
    {
        Llista<ImageFragment> fragments;

        public Collage()
        {
            fragments = new Llista<ImageFragment>();
        }
        public Collage(IList<ImageFragment> imgsCollage) : this()
        {
            fragments.AddRange(imgsCollage);
        }
        public override Bitmap Image
        {
            get
            {
                return CrearCollage();
            }
        }
        public override byte[] ArgbValues
        {
            get
            {
                return Image.GetBytes();
            }
        }
        public int Count
        {
            get { return fragments.Count; }
        }
        public ImageFragment this[int posicion]
        {
            get { return fragments[posicion]; }
            set { fragments[posicion] = value; }
        }
        public void Add(ImageFragment imgFragment)
        {
            fragments.Add(imgFragment);
        }
        public void Add(IList<ImageFragment> imgs)
        {
            fragments.AddRange(imgs);
        }
        public ImageFragment Add(Bitmap imatge, PointZ localizacio)
        {
            return Add(imatge, localizacio.X, localizacio.Y, localizacio.Z);
        }

        /// <summary>
        /// Añade una imagen al mosaico
        /// </summary>
        /// <param name="imagen">imagen para poner</param>
        /// <param name="localizacion">localización en la imagen</param>
        /// <param name="capa">produndidad a la que esta</param>
        /// <returns>devuelve null si no lo a podido añadir</returns>
        public ImageFragment Add(Bitmap imatge, Point localizacio, int capa = 0)
        {
            return Add(imatge, localizacio.X, localizacio.Y, capa);
        }
        public ImageFragment Add(Bitmap imagen, int x = 0, int y = 0, int z = 0)
        {
            if (imagen == null)
                throw new ArgumentNullException("imagen", "Se necesita una imagen");

            ImageFragment fragment = null;
            PointZ location = new PointZ(x, y, z);
            fragment = new ImageFragment(imagen, location);
            fragments.Add(fragment);


            return fragment;
        }
        public void RemoveAll()
        {
            fragments.Clear();
        }
        public void Remove(ImageFragment fragmento)
        {
            fragments.Remove(fragmento);
        }
        public ImageFragment Remove(int x = 0, int y = 0, int z = 0)
        {
            return Remove(new PointZ(x, y, z));
        }
        public ImageFragment Remove(Point localizacion, int capa = 0)
        {
            return Remove(new PointZ(localizacion.X, localizacion.Y, capa));
        }
        public ImageFragment Remove(PointZ localizacion)
        {
            ImageFragment fragmentoQuitado = GetFragment(localizacion);

            if (fragmentoQuitado != null)
                fragments.Remove(fragmentoQuitado);

            return fragmentoQuitado;
        }
        public ImageFragment GetFragment(PointZ location)
        {
            return GetFragment(location.X, location.Y, location.Z);
        }
        public ImageFragment GetFragment(int x, int y, int z)
        {
            List<ImageFragment> fragmentosCapaZero = new List<ImageFragment>();
            bool acabado = false;
            int pos = 0;
            Rectangle rectangle;
            ImageFragment fragmento = null;

            fragments.SortByBubble();
            while (pos < this.fragments.Count && !acabado)
            {

                if (this.fragments[pos].Location.Z > z)
                    acabado = true;
                else if (this.fragments[pos].Location.Z == z)
                    fragmentosCapaZero.Add(this.fragments[pos]);
                pos++;
            }
            for (int i = 0; i < fragmentosCapaZero.Count && fragmento == null; i++)
            {
                rectangle = new Rectangle(fragmentosCapaZero[i].Location.X, fragmentosCapaZero[i].Location.Y, fragmentosCapaZero[i].Image.Width, fragmentosCapaZero[i].Image.Height);
                if (rectangle.Contains(x, y))
                    fragmento = fragmentosCapaZero[i];

            }
            return fragmento;
        }
        public ImageFragment[] GetFragments(PointZ location)
        {
            return GetFragments(location.X, location.Y, location.Z);
        }
        public ImageFragment[] GetFragments(int x, int y, int z)
        {
            List<ImageFragment> fragmentosSeleccionados = new List<ImageFragment>();
            ImageFragment img;

            do
            {
                img = GetFragment(x, y, z);
                if (img != null)
                {//los quito para no molestar
                    fragmentosSeleccionados.Add(img);
                    fragments.Remove(img);
                }
            } while (img != null);

            fragments.AddRange(fragmentosSeleccionados);

            return fragmentosSeleccionados.ToArray();
        }


        public Bitmap CrearCollage()
        {
            int xFinal = 1, xInicial = 0;
            int yFinal = 1, yInicial = 0;
            int width, height;
            for (int i = 0; i < fragments.Count; i++)
            {
                if (xFinal < (fragments[i].Location.X + fragments[i].Image.Width))
                    xFinal = (fragments[i].Location.X + fragments[i].Image.Width);
                if (xInicial > fragments[i].Location.X)
                    xInicial = fragments[i].Location.X;
                if (yFinal < (fragments[i].Location.Y + fragments[i].Image.Height))
                    yFinal = (fragments[i].Location.Y + fragments[i].Image.Height);
                if (yInicial > fragments[i].Location.Y)
                    yInicial = fragments[i].Location.Y;
            }
            width = xFinal - xInicial;
            height = yFinal - yInicial;
            return CrearCollage(new Rectangle(xInicial, yInicial, width, height));
        }
        public Bitmap CrearCollage(Rectangle rctImgResultado)
        {
            Bitmap bmpTotal = new Bitmap(rctImgResultado.Width, rctImgResultado.Height,ImageBase.DefaultPixelFormat);
            fragments.SortByQuickSort();//deberia poner los de la Z mas grande los primeros
            unsafe
            {

                bmpTotal.TrataBytes((ptTotal) => {
                    for (int i = 0; i < fragments.Count; i++)
                    {
                        if (fragments[i].IsVisible)
                        {
                            fixed (byte* ptFragmento = fragments[i].ArgbValues)
                                BitmapExtension.SetFragment(ptTotal, bmpTotal.Height, bmpTotal.Width, ImageBase.ISARGB, ptFragmento, fragments[i].Image.Height, fragments[i].Image.Width, ImageBase.ISARGB, rctImgResultado.GetRelativePoint(new Point(fragments[i].Location.X, fragments[i].Location.Y)));
                        }
                    }


                });
            }
            return bmpTotal;
        }
        public IEnumerator<ImageFragment> GetEnumerator()
        {
            fragments.SortByBubble();
            return fragments.GetEnumerator();
        }

        IEnumerator<ImageFragment> IEnumerable<ImageFragment>.GetEnumerator()
        {
            return GetEnumerator();
        }
        
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

}