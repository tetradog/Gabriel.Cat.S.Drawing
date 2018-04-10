using Gabriel.Cat.S.Drawing;
using Gabriel.Cat.S.Utilitats;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace Gabriel.Cat.S.Binaris
{
    public class ImageFragmentBinario : ElementoComplejoBinarioNullable
    {
        public ImageFragmentBinario()
        {
            base.Partes.Add(ElementoBinario.ElementosTipoAceptado(Serializar.TiposAceptados.PointZ));
            base.Partes.Add(ElementoBinario.ElementosTipoAceptado(Serializar.TiposAceptados.Bitmap));
        }

        protected override IList IGetPartsObject(object obj)
        {
            ImageFragment image=obj as ImageFragment;
            return new object[] {image.Location,image.Image };
        }

        protected override object JGetObject(MemoryStream bytes)
        {
            object[] parts = GetPartsObject(bytes);
            return new ImageFragment(parts[1] as Bitmap, (PointZ)parts[0]);
        }
    }
}