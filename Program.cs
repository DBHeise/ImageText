

namespace ImageText
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Drawing.Imaging;
    using CommandLine;
    using nQuant;

    class Program
    {
        public class Options
        {
            [Option('i', "InputFile", Required = false, HelpText = "The input image file (if no input file is used a random 512x512 bitmap is generated)")]
            public String InputImage { get; set; }

            [Option('o', "OutputFile", Required = true, HelpText = "The output image file")]
            public String OutImage { get; set; }


            [Option('t', "Text", Required = true, HelpText = "The text to write on the image")]
            public String Text { get; set; }


            [Option('c', "ColorName", Required = false, HelpText = "The named color for writing the text", Default = "HotPink")]
            public String ColorName { get; set; }

            [Option('f', "FontName", Required = false, HelpText = "The font name to use when writing text", Default = "Arial")]
            public String FontName { get; set; }


            [Option('s', "FontSize", Required = false, HelpText = "The font size to use when writing text", Default = 30f)]
            public float FontSize { get; set; }


            [Option('a', "Angle", Required = false, HelpText = "The rotation angle to use when writing text", Default = -45f)]
            public float Angle { get; set; }

            [Option('q', "UseQuantizer", Required = false, HelpText = "Use Quantizer (to get smaller image file sizes)", Default = false)]
            public Boolean UseQuantizer { get; set; }

        }

        static void Main(string[] args)
        {
            Parser.Default
                .ParseArguments<Options>(args)
                .WithParsed(Run)
                .WithNotParsed(HandleParseError);
        }

        private static void HandleParseError(IEnumerable<Error> errs)
        {
            if (errs.IsVersion())
            {
                Console.WriteLine("Version Request");
                return;
            }

            if (errs.IsHelp())
            {
                Console.WriteLine("Help Request");
                return;
            }
            Console.WriteLine("Parser Fail");
        }

        static Bitmap GenerateRandomBitmap()
        {
            int width = 512;
            int height = 512;
            byte[] data = new byte[262144];

            (new Random()).NextBytes(data);

            Bitmap bmp = new Bitmap(width, height, PixelFormat.Format8bppIndexed);
            BitmapData bmpData = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.WriteOnly, bmp.PixelFormat);

            System.Runtime.InteropServices.Marshal.Copy(data, 0, bmpData.Scan0, data.Length);
            bmp.UnlockBits(bmpData);
            return bmp;
        }

        static void Run(Options cfg)
        {
            Image originalImage;
            if (String.IsNullOrWhiteSpace(cfg.InputImage) || !System.IO.File.Exists(cfg.InputImage))
            {
                originalImage = GenerateRandomBitmap();
            } else
            {
                originalImage = Image.FromFile(cfg.InputImage);
            }

            Color c = Color.FromName(cfg.ColorName);
            if (c.ToArgb() == 0)
            {
                c = Color.HotPink;
            }

            var quantizer = new WuQuantizer();
            
            using (Bitmap drawingBitmap = new Bitmap(originalImage.Width, originalImage.Height))
            using (Graphics gfx = Graphics.FromImage(drawingBitmap))
            using (Font fnt = new Font(cfg.FontName, cfg.FontSize))
            using (Brush b = new SolidBrush(c))
            {
                gfx.DrawImage(originalImage, 0, 0);

                SizeF sz = gfx.VisibleClipBounds.Size;
                gfx.TranslateTransform(sz.Width / 2, sz.Height / 2);
                gfx.RotateTransform(cfg.Angle);
                sz = gfx.MeasureString(cfg.Text, fnt);
                gfx.DrawString(cfg.Text, fnt, b, -(sz.Width / 2), -(sz.Height / 2));

                gfx.ResetTransform();
                if (cfg.UseQuantizer)
                {
                    using (var q = quantizer.QuantizeImage(drawingBitmap))
                    {
                        q.Save(cfg.OutImage);
                    }
                }
                else
                {
                    drawingBitmap.Save(cfg.OutImage);
                }
            }

            originalImage.Dispose();
        }
    }
}
