using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;

namespace MulticlientCreator;

internal static class SeovaTheme
{
    public static readonly Color Bg = Color.FromArgb(28, 28, 30);
    public static readonly Color Bar = Color.FromArgb(37, 37, 40);
    public static readonly Color Input = Color.FromArgb(44, 44, 48);
    public static readonly Color Fg = Color.FromArgb(228, 228, 232);
    public static readonly Color Dim = Color.FromArgb(140, 140, 148);
    public static readonly Color Teal = Color.FromArgb(74, 214, 196);
    public static readonly Color Magenta = Color.FromArgb(176, 92, 208);

    private const string LogoResource = "MulticlientCreator.logo.ico";

    [DllImport("dwmapi.dll")]
    private static extern int DwmSetWindowAttribute(IntPtr hwnd, int attribute, ref int value, int size);

    public static Image? Logo(int size)
    {
        try
        {
            using var s = typeof(SeovaTheme).Assembly.GetManifestResourceStream(LogoResource);
            if (s == null) return null;
            using var ico = new Icon(s, new Size(size, size));
            return ico.ToBitmap();
        }
        catch { return null; }
    }

    public static void Apply(Form f)
    {
        f.BackColor = Bg;
        f.ForeColor = Fg;
        try
        {
            using var s = typeof(SeovaTheme).Assembly.GetManifestResourceStream(LogoResource);
            if (s != null) f.Icon = new Icon(s);
        }
        catch { }
        f.HandleCreated += (_, _) => { try { int on = 1; DwmSetWindowAttribute(f.Handle, 20, ref on, sizeof(int)); } catch { } };
    }

    public static Panel Header(string title, string? subtitle = null)
    {
        var host = new Panel { Dock = DockStyle.Top, Height = subtitle == null ? 52 : 60, BackColor = Bar };
        var logo = new PictureBox { Image = Logo(32), SizeMode = PictureBoxSizeMode.Zoom, Size = new Size(32, 32), Location = new Point(12, subtitle == null ? 10 : 12), BackColor = Color.Transparent };
        var lbl = new Label { Text = title, ForeColor = Fg, Font = new Font("Segoe UI Semibold", 12f, FontStyle.Bold), AutoSize = true, Location = new Point(54, subtitle == null ? 14 : 10) };
        host.Controls.Add(logo);
        host.Controls.Add(lbl);
        if (subtitle != null)
            host.Controls.Add(new Label { Text = subtitle, ForeColor = Dim, Font = new Font("Segoe UI", 8.5f), AutoSize = true, Location = new Point(56, 36) });

        var accent = new Panel { Dock = DockStyle.Bottom, Height = 2 };
        accent.Paint += (_, e) => { using var br = new LinearGradientBrush(accent.ClientRectangle, Teal, Magenta, 0f); e.Graphics.FillRectangle(br, accent.ClientRectangle); };
        host.Controls.Add(accent);
        return host;
    }

    public static GradientButton Button(string text, int width, int height) => new(Teal, Magenta)
    {
        Text = text,
        Size = new Size(width, height),
        Font = new Font("Segoe UI Semibold", 9.5f, FontStyle.Bold),
        Cursor = Cursors.Hand
    };

    public sealed class GradientButton : Control
    {
        private readonly Color _c1, _c2;
        private bool _hover;
        public GradientButton(Color c1, Color c2)
        {
            _c1 = c1; _c2 = c2;
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.UserPaint | ControlStyles.ResizeRedraw, true);
            MouseEnter += (_, _) => { _hover = true; Invalidate(); };
            MouseLeave += (_, _) => { _hover = false; Invalidate(); };
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            var rect = new Rectangle(0, 0, Width - 1, Height - 1);
            int r = Math.Min(16, Height / 2), d = r * 2;
            using var path = new GraphicsPath();
            path.AddArc(rect.X, rect.Y, d, d, 180, 90);
            path.AddArc(rect.Right - d, rect.Y, d, d, 270, 90);
            path.AddArc(rect.Right - d, rect.Bottom - d, d, d, 0, 90);
            path.AddArc(rect.X, rect.Bottom - d, d, d, 90, 90);
            path.CloseFigure();
            Color c1 = _hover ? L(_c1) : _c1, c2 = _hover ? L(_c2) : _c2;
            using var br = new LinearGradientBrush(rect, c1, c2, 0f);
            g.FillPath(br, path);
            TextRenderer.DrawText(g, Text, Font, ClientRectangle, Color.White,
                TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
        }
        private static Color L(Color c) => Color.FromArgb(Math.Min(255, c.R + 28), Math.Min(255, c.G + 28), Math.Min(255, c.B + 28));
    }
}
