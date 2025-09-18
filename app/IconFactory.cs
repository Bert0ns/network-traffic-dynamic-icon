using System.Drawing.Drawing2D;
using System.Drawing.Text;


namespace network_traffic_dynamic_icon.app
{
    public static class IconFactory
    {
        // Crea una piccola icona 32x32 con due righe: D (download) e U (upload)
        public static Icon MakeIcon(long downBps, long upBps)
        {
            using var bmp = new Bitmap(32, 32);
            using var g = Graphics.FromImage(bmp);
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
            g.Clear(Color.Transparent);

            string d = Shorten(downBps);
            string u = Shorten(upBps);

            using var font = new Font("Segoe UI", 8, FontStyle.Bold, GraphicsUnit.Point);
            using var brushDown = new SolidBrush(Color.DeepSkyBlue);
            using var brushUp = new SolidBrush(Color.LimeGreen);

            g.DrawString(d, font, brushDown, new PointF(0, 0));
            g.DrawString(u, font, brushUp, new PointF(0, 14));

            // Converte in icona
            nint hIcon = bmp.GetHicon();
            return Icon.FromHandle(hIcon);
        }

        private static string Shorten(long bps)
        {
            if (bps < 0) return "0";
            double v = bps;
            string unit = "B";
            if (v >= 1024)
            {
                v /= 1024;
                unit = "K";
            }
            if (v >= 1024)
            {
                v /= 1024;
                unit = "M";
            }
            return v < 10 ? $"{v:0.0}{unit}" : $"{v:0}{unit}";
        }
    }
}
