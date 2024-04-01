using Gabriel.Cat.S.Drawing;
using Gabriel.Cat.S.Extension;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Gabriel.Cat.S.Binaris
{
    public class CollageBinario : ElementoIListBinario<ImageFragment>
    {
        public CollageBinario() : base(new ImageFragmentBinario(), LongitudBinaria.UInt)
        {

        }
        protected override byte[] JGetBytes(object obj)
        {
            Collage collage = obj as Collage;
            if (collage == null)
                throw new TipoException();

            return base.GetBytes(((IEnumerable<ImageFragment>)collage).ToArray());
        }
        protected override object JGetObject(MemoryStream bytes)
        {
            return new Collage(((object[])base.GetObject(bytes)).Casting<ImageFragment>().ToArray());
        }
    }
}