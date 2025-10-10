using PdfSharp.Fonts;
using System.Drawing.Text;
using System.Globalization;

namespace LocalPortService.Core.Helper
{
    public class FileFontResolver : IFontResolver // FontResolverBase
    {
        public string DefaultFontName => throw new NotImplementedException();

        public byte[] GetFont(string faceName)
        {
            string fontDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Fonts);
            string path = Path.Combine(fontDirectory, faceName);
            string tiffLocation = Path.Combine(AppContext.BaseDirectory, "font/code128.ttf");

            if(faceName == "kaiu.ttf")
            {
                byte[] font1Bytes = ReadFontFile(path);
                return font1Bytes;
            }
            else if(faceName == "code128.ttf")
            {
                byte[] font2Bytes = ReadFontFile(tiffLocation);
                return font2Bytes;
            }
            else
            {
                return Array.Empty<byte>();
            }
        }

        public FontResolverInfo ResolveTypeface(string familyName, bool isBold, bool isItalic)
        {
            if (familyName.Equals("標楷體", StringComparison.CurrentCultureIgnoreCase))
            {
                if (isBold && isItalic)
                {
                    return new FontResolverInfo("kaiu.ttf");
                }
                else if (isBold)
                {
                    return new FontResolverInfo("kaiu.ttf");
                }
                else if (isItalic)
                {
                    return new FontResolverInfo("kaiu.ttf");
                }
                else
                {
                    return new FontResolverInfo("kaiu.ttf");
                }
            }
            else if (familyName.Equals("code128", StringComparison.CurrentCultureIgnoreCase))
            {
                return new FontResolverInfo("code128.ttf");
            }
            return null;
        }

        private byte[] ReadFontFile(string filePath)
        {
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException("Font file not found", filePath);
            }

            using (var ms = new MemoryStream())
            {
                using (var fs = File.OpenRead(filePath))
                {
                    fs.CopyTo(ms);
                    ms.Position = 0;
                    return ms.ToArray();
                }
            }
        }
    }
}
