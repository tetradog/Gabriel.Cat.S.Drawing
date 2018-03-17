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
        Bitmap bmp;

        byte[] bmpArray;
        public ImageBase(Bitmap bmp)
        {

            if (bmp == null)
                throw new NullReferenceException("La imagen no puede ser null");
            this.bmp = bmp.Clone(new Rectangle(new Point(), bmp.Size), DefaultPixelFormat);//asi todos tienen el mismo PixelFormat :)

        }

        public virtual byte[] Array
        {
            get
            {
                if (bmpArray == null)
                    bmpArray = bmp.GetBytes();
                return bmpArray;
            }
            set
            {
                bmp.SetBytes(value);
                bmpArray = value;
            }
        }


        public Bitmap Image
        {
            get
            {
                return bmp;
            }
        }


    }

}