using Gabriel.Cat.S.Extension;
using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace Gabriel.Cat.S.Drawing
{
    public class ImageBase
    {
        public const bool ISARGB = true;
        public static readonly PixelFormat DefaultPixelFormat = PixelFormat.Format32bppArgb;
        byte[] bmpArray;

        public ImageBase():this(new Bitmap(1, 1))
        {
        }

        public ImageBase(Bitmap bmp)
        {

            if (bmp == null)
                throw new NullReferenceException("La imagen no puede ser null");
            Image = bmp.Clone(new Rectangle(new Point(), bmp.Size), DefaultPixelFormat);//asi todos tienen el mismo PixelFormat :)
            bmpArray = Image.GetBytes();
        }

        public virtual byte[] Array
        {
            get
            {
                   
                return bmpArray;
            }
            set
            {
                Image.SetBytes(value);
                bmpArray = value;
            }
        }


        public Bitmap Image { get; private set; }


    }

}