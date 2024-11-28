using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Windows;

namespace Wordpad.Files
{
    public static class VisualExtensions
    {
        public static void DrawToPrintPage(this Visual visual, PrintPageEventArgs e)
        {
            if (visual == null)
                throw new ArgumentNullException(nameof(visual));

            // Kết xuất Visual sang Bitmap
            RenderTargetBitmap rtb = new RenderTargetBitmap(
                (int)e.PageBounds.Width,
                (int)e.PageBounds.Height,
                96, // DPI ngang
                96, // DPI dọc
                PixelFormats.Pbgra32
            );

            DrawingVisual dv = new DrawingVisual();
            using (DrawingContext dc = dv.RenderOpen())
            {
                VisualBrush vb = new VisualBrush(visual);
                dc.DrawRectangle(vb, null, new Rect(new System.Windows.Point(), new System.Windows.Size(rtb.Width, rtb.Height)));
            }
            rtb.Render(dv);

            // Chuyển BitmapSource sang Bitmap
            var bitmap = new System.Drawing.Bitmap(rtb.PixelWidth, rtb.PixelHeight, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            var data = bitmap.LockBits(new System.Drawing.Rectangle(0, 0, bitmap.Width, bitmap.Height), System.Drawing.Imaging.ImageLockMode.WriteOnly, bitmap.PixelFormat);

            rtb.CopyPixels(Int32Rect.Empty, data.Scan0, data.Height * data.Stride, data.Stride);
            bitmap.UnlockBits(data);

            // Vẽ Bitmap lên Graphics
            e.Graphics.DrawImage(bitmap, e.MarginBounds.Left, e.MarginBounds.Top, e.MarginBounds.Width, e.MarginBounds.Height);
        }
    }

}
