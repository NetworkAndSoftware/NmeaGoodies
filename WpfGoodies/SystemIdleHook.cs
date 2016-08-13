using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Experiment3.Helpers
{
  /// <summary>
  ///   Based on http://weblogs.asp.net/jdanforth/detecting-idle-time-with-global-mouse-and-keyboard-hooks-in-wpf
  /// </summary>
  public static class SystemIdleHook
  {
    private delegate int HookProc(int nCode, IntPtr wParam, IntPtr lParam);

    private enum HookType
    {
      GlobalKeyboard = 13,
      GlobalMouse = 14
    }

    // ReSharper disable once UnusedMember.Local
    private static readonly Destructor Finalise = new Destructor();
    private static int _hHookKbd;
    private static int _hHookMouse;
    private static DateTime _lastActiveTime = DateTime.Now;

    public static TimeSpan IdleTime => DateTime.Now - _lastActiveTime;

    private static event HookProc MouseHookProcedure;
    private static event HookProc KbdHookProcedure;

    //Use this function to install thread-specific hook.
    [DllImport("user32.dll", CharSet = CharSet.Auto,
      CallingConvention = CallingConvention.StdCall)]
    private static extern int SetWindowsHookEx(int idHook, HookProc lpfn,
      IntPtr hInstance, int threadId);

    //Call this function to uninstall the hook.
    [DllImport("user32.dll", CharSet = CharSet.Auto,
      CallingConvention = CallingConvention.StdCall)]
    private static extern bool UnhookWindowsHookEx(int idHook);

    //Use this function to pass the hook information to next hook procedure in chain.
    [DllImport("user32.dll", CharSet = CharSet.Auto,
      CallingConvention = CallingConvention.StdCall)]
    private static extern int CallNextHookEx(int idHook, int nCode,
      IntPtr wParam, IntPtr lParam);

    //Use this hook to get the module handle, needed for WPF environment
    [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
    private static extern IntPtr GetModuleHandle(string lpModuleName);

    private static int MouseHookProc(int nCode, IntPtr wParam, IntPtr lParam)
    {
      //user is active, at least with the mouse
      _lastActiveTime = DateTime.Now;

      //just return the next hook
      return CallNextHookEx(_hHookMouse, nCode, wParam, lParam);
    }

    private static int KbdHookProc(int nCode, IntPtr wParam, IntPtr lParam)
    {
      //user is active, at least with the keyboard
      _lastActiveTime = DateTime.Now;

      //just return the next hook
      return CallNextHookEx(_hHookKbd, nCode, wParam, lParam);
    }

    public static bool Enabled
    {
      set
      {
        if (value)
          Hook();
        else
          Unhook();
      }
    }

    private static void Hook()
    {
      using (var currentProcess = Process.GetCurrentProcess())
      using (var mainModule = currentProcess.MainModule)
      {
        if (_hHookMouse == 0)
        {
          // Create an instance of HookProc.
          MouseHookProcedure = MouseHookProc;
          // Create an instance of HookProc.
          KbdHookProcedure = KbdHookProc;

          //register a global hook
          _hHookMouse = SetWindowsHookEx((int)HookType.GlobalMouse,
            MouseHookProcedure,
            GetModuleHandle(mainModule.ModuleName),
            0);
          if (_hHookMouse == 0)
          {
            Unhook();
            throw new ApplicationException("SetWindowsHookEx() failed for the mouse");
          }
        }

        if (_hHookKbd == 0)
        {
          //register a global hook
          _hHookKbd = SetWindowsHookEx((int)HookType.GlobalKeyboard,
            KbdHookProcedure,
            GetModuleHandle(mainModule.ModuleName),
            0);
          if (_hHookKbd == 0)
          {
            Unhook();
            throw new ApplicationException("SetWindowsHookEx() failed for the keyboard");
          }
        }
      }
    }

    private static void Unhook()
    {
      if (_hHookMouse != 0)
      {
        var ret = UnhookWindowsHookEx(_hHookMouse);
        if (ret == false)
        { // don't care
          // throw new ApplicationException("UnhookWindowsHookEx() failed for the mouse");
        }
        _hHookMouse = 0;
      }

      if (_hHookKbd != 0)
      {
        var ret = UnhookWindowsHookEx(_hHookKbd);
        if (ret == false)
        { // don't care
          // throw new ApplicationException("UnhookWindowsHookEx() failed for the keyboard");
        }
        _hHookKbd = 0;
      }
    }

    private sealed class Destructor
    {
      ~Destructor()
      {
        Unhook();
      }
    }
  }
}