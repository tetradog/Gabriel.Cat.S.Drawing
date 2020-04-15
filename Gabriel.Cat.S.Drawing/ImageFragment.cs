using Gabriel.Cat.S.Utilitats;
using System;
using System.Drawing;

namespace Gabriel.Cat.S.Drawing
{
    public class ImageFragment : IComparable, IComparable<ImageFragment>
    {
        PointZ localizacion;
        ImageBase imagen;

        protected ImageFragment()
        { }
        public ImageFragment(Bitmap imagen, int x = 0, int y = 0, int z = 0)
            : this(imagen, new PointZ(x, y, z))
        {

        }
        public ImageFragment(Bitmap imagen, Point localizacion, int capa = 0)
            : this(imagen, new PointZ(localizacion.X , localizacion.Y , capa))
        {

        }
        public ImageFragment(Bitmap imagen, PointZ localizacion)
        {
            if (imagen == null)
                throw new NullReferenceException("La imagen no puede ser null");
            this.imagen = new ImageBase(imagen);
            Location = localizacion;
            IsVisible = true;
        }

        public virtual byte[] ArgbValues
        {
            get { return imagen.Array; }
        }
        public bool IsVisible { get; set; }
        public PointZ Location
        {
            get
            {
                return localizacion;
            }
            set
            {
                localizacion = value;
            }
        }

        public virtual Bitmap Image
        {
            get
            {
                return imagen.Image;
            }
        }

        #region IComparable implementation
        public int CompareTo(ImageFragment other)
        {
            int compareTo;
            if (other != null)
                compareTo = Location.CompareTo(other.Location);
            else
                compareTo = -1;
            return compareTo;
        }
        public int CompareTo(Object other)
        {
            return CompareTo(other as ImageFragment);
        }

        public bool IsInRectangle(Rectangle rctImgResultado)
        {
            return rctImgResultado.Contains(new Rectangle(this.Location.X, this.Location.Y, Image.Width, Image.Height));
        }
        #endregion



    }

}