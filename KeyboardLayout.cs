using System;
using System.Runtime.InteropServices;
using System.Threading;

public delegate void KeyboardLayoutChanged(int newCultureInfo);

internal class KeyboardLayoutWatcher : IDisposable
{
    private readonly Timer _timer;
    private int _currentLayout = 1033;


    public KeyboardLayoutChanged KeyboardLayoutChanged;

    public KeyboardLayoutWatcher()
    {
        _timer = new Timer(new TimerCallback(CheckKeyboardLayout), null, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1));
    }

    [DllImport("user32.dll")] private static extern IntPtr GetForegroundWindow();
    [DllImport("user32.dll")] private static extern uint GetWindowThreadProcessId(IntPtr hwnd, IntPtr proccess);
    [DllImport("user32.dll")] private static extern IntPtr GetKeyboardLayout(uint thread);
    public int GetCurrentKeyboardLayout()
    {
        try
        {
            IntPtr foregroundWindow = GetForegroundWindow(); // get active window
            uint foregroundProcess = GetWindowThreadProcessId(foregroundWindow, IntPtr.Zero); // get process id of this active window
            int keyboardLayout = GetKeyboardLayout(foregroundProcess).ToInt32() & 0xFFFF; // get layout of this process which is the active window

            if (keyboardLayout == 0)
            {
                // something has gone wrong - just assume English xD
                keyboardLayout = 1033;
            }
            return keyboardLayout;
        }
        catch
        {
            // if something goes wrong - just assume English xD
            return 1033;
        }
    }

    public static string GetCurrentKeyboardLayoutFormat(int layoutCode)
    {
        switch (layoutCode)
        {
            case 1033:
                return "English";

            case 1049:
                return "Russian";

            case 1037:
                return "Hebrew";

            default:
                return "ERROR";
        }
    }

    private void CheckKeyboardLayout(object sender)
    {
        int layout = GetCurrentKeyboardLayout();
        if (_currentLayout != layout && KeyboardLayoutChanged != null)
        {
            KeyboardLayoutChanged(layout);
            _currentLayout = layout;
        }

    }

    private void ReleaseUnmanagedResources()
    {
        _timer.Dispose();
    }

    public void Dispose()
    {
        ReleaseUnmanagedResources();
        GC.SuppressFinalize(this);
    }

    ~KeyboardLayoutWatcher()
    {
        ReleaseUnmanagedResources();
    }
}