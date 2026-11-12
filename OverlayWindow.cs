using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;

namespace SnowOverlay;

public class OverlayWindow : Window
{
    public OverlayWindow()
    {
        SystemDecorations = SystemDecorations.None;
        Background = Brushes.Transparent;
        TransparencyLevelHint = new[] { WindowTransparencyLevel.Transparent };
        Topmost = true;
        ShowInTaskbar = false;
        CanResize = false;
        ShowActivated = false;
        Focusable = false;
        Content = new SnowCanvas();

        Opened += (_, _) =>
        {
            var screen = Screens.ScreenFromWindow(this) ?? Screens.Primary;
            if (screen != null)
            {
                Position = screen.Bounds.Position;
                Width = screen.Bounds.Width / screen.Scaling;
                Height = screen.Bounds.Height / screen.Scaling;
            }

            if (OperatingSystem.IsWindows() && TryGetPlatformHandle() is { } h)
                Win32.MakeClickThrough(h.Handle);
        };
    }
}
