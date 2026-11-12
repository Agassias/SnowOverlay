using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Media;
using Avalonia.Threading;

namespace SnowOverlay;

public class SnowCanvas : Control
{
    struct Flake { public double X, Y, Vy, Drift, Phase, R; }

    const int FlakeCount = 250;
    readonly Flake[] _flakes = new Flake[FlakeCount];
    readonly Random _rnd = new();
    readonly IBrush _brush = new SolidColorBrush(Color.FromArgb(230, 255, 255, 255));
    readonly DispatcherTimer _timer = new() { Interval = TimeSpan.FromMilliseconds(16) };
    DateTime _last = DateTime.UtcNow;
    double _time;
    bool _inited;

    public SnowCanvas()
    {
        IsHitTestVisible = false;
        _timer.Tick += (_, _) => Tick();
        _timer.Start();
    }

    void Respawn(ref Flake f, bool anywhere)
    {
        f.X = _rnd.NextDouble() * Bounds.Width;
        f.Y = anywhere ? _rnd.NextDouble() * Bounds.Height : -10;
        f.R = 1.5 + _rnd.NextDouble() * 2.5;
        f.Vy = 40 + f.R * 18 + _rnd.NextDouble() * 20; // крупные падают быстрее
        f.Drift = 10 + _rnd.NextDouble() * 20;
        f.Phase = _rnd.NextDouble() * Math.PI * 2;
    }

    void Tick()
    {
        if (OperatingSystem.IsWindows() && Win32.ExitHotkeyPressed())
        {
            (Application.Current?.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime)?.Shutdown();
            return;
        }

        if (Bounds.Width < 1 || Bounds.Height < 1) return;

        var now = DateTime.UtcNow;
        double dt = Math.Min((now - _last).TotalSeconds, 0.05);
        _last = now;
        _time += dt;

        if (!_inited)
        {
            for (int i = 0; i < FlakeCount; i++) Respawn(ref _flakes[i], anywhere: true);
            _inited = true;
        }

        for (int i = 0; i < FlakeCount; i++)
        {
            ref var f = ref _flakes[i];
            f.Y += f.Vy * dt;
            f.X += Math.Sin(_time * 1.2 + f.Phase) * f.Drift * dt;
            if (f.Y > Bounds.Height + 5) Respawn(ref f, anywhere: false);
        }

        InvalidateVisual();
    }

    public override void Render(DrawingContext ctx)
    {
        if (!_inited) return;
        foreach (var f in _flakes)
            ctx.DrawEllipse(_brush, null, new Point(f.X, f.Y), f.R, f.R);
    }
}
