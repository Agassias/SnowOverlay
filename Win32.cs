using System.Runtime.InteropServices;

namespace SnowOverlay;

// Всё Windows-специфичное живёт тут. Позже вынесем за интерфейсы для macOS/Linux.
static class Win32
{
    const int GWL_EXSTYLE = -20;
    const int WS_EX_TRANSPARENT = 0x00000020; // клики проходят сквозь окно
    const int WS_EX_LAYERED     = 0x00080000; // нужен для click-through
    const int WS_EX_TOOLWINDOW  = 0x00000080; // не показывать в Alt+Tab
    const int WS_EX_NOACTIVATE  = 0x08000000; // окно никогда не забирает фокус

    [DllImport("user32.dll")] static extern int GetWindowLongW(IntPtr hWnd, int nIndex);
    [DllImport("user32.dll")] static extern int SetWindowLongW(IntPtr hWnd, int nIndex, int dwNewLong);
    [DllImport("user32.dll")] static extern short GetAsyncKeyState(int vKey);

    public static void MakeClickThrough(IntPtr hwnd)
    {
        int style = GetWindowLongW(hwnd, GWL_EXSTYLE);
        style |= WS_EX_TRANSPARENT | WS_EX_LAYERED | WS_EX_TOOLWINDOW | WS_EX_NOACTIVATE;
        SetWindowLongW(hwnd, GWL_EXSTYLE, style);
    }

    // Аварийный выход: Ctrl+Shift+Q (окно сквозное, кликнуть по нему нельзя)
    public static bool ExitHotkeyPressed() =>
        (GetAsyncKeyState(0x11) & 0x8000) != 0 && // Ctrl
        (GetAsyncKeyState(0x10) & 0x8000) != 0 && // Shift
        (GetAsyncKeyState(0x51) & 0x8000) != 0;   // Q
}
