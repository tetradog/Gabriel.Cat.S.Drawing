using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace Gabriel.Cat.S.Extension
{
   public static class ExtensionRectangle
    {
        public static Point GetRelativePoint(this Rectangle rect, Point point)
        {
            return new Point(rect.Left - point.X, rect.Top - point.Y);
        }
    }
}
