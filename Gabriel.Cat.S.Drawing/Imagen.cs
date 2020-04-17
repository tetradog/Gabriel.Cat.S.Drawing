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
    delegate int GetPosicion(ImageFragment fragment);
    public class Collage : ImageFragment, IEnumerable<ImageFragment>
    {
        Llista<ImageFragment> fragments;
        Bitmap bmp;

        public Collage()
        {
            Base = new ImageBase(new Bitmap(1,1));
            bmp = default;
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
                if (bmp == default)
                    CrearCollage();
                return bmp;
            }
        }
        public override byte[] ArgbValues
        {
            get
            {
                return Image.GetBytes();
            }
        }
        /// <summary>
        /// Es la imagen donde las demás se posicionarán
        /// </summary>
        public ImageBase Base { get; set; }
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
            int xFinal = Base.Image.Width, xInicial = 0;
            int yFinal = Base.Image.Height, yInicial = 0;
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
            bmp = CrearCollage(new Rectangle(0, 0, width, height));
            return bmp;
        }
        public Bitmap CrearCollage(Rectangle rctImgResultado)
        {
            Rectangle rctBase;//no se porque pero me recorta la imagen!!
            Bitmap bmpTotal = new Bitmap(rctImgResultado.Width, rctImgResultado.Height, ImageBase.DefaultPixelFormat);//tiene que caber todo en la imagen
            fragments.SortByQuickSort();//deberia poner los de la Z mas grande los primeros
            if (fragments.Count > 0)
            {
                rctBase = new Rectangle(GetXMasPequeña(), GetYMasPequeña(), Base.Image.Width, Base.Image.Height);
                bmpTotal.SetFragment(Base.Image, rctBase.Location);
                for (int i = 0; i < fragments.Count; i++)//todo se basa en la imagen base del fondo osea es relativo a él
                    bmpTotal.SetFragment(fragments[i].Image, rctBase.RelativeToAbsolute(new Point(fragments[i].Location.X, fragments[i].Location.Y)));
            }
            bmp = bmpTotal;
            return bmpTotal;
        }

        private int GetYMasPequeña()
        {
            return GetComun((f) => f.Location.Y);
        }

        private int GetXMasPequeña()
        {
            return GetComun((f) => f.Location.X);
        }
        private int GetComun(GetPosicion getPosicion)
        {
            int pos = 0;
            int aux;
            fragments.SortByBubble();
            for (int i = 0; i < fragments.Count; i++)
            {
                aux = getPosicion(fragments[i]);
                if (aux < pos)
                    pos = aux;
            }
            return -pos;
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