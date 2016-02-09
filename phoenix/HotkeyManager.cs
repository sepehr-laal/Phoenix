﻿namespace phoenix
{
    class HotkeyManager
    {
        const uint MOD_NOREPEAT    = 0x4000;
        const uint MOD_ALT         = 0x0001;

        public const int TOGGLE_FORCE_ALWAYS_ON_TOP_ID  = 1;
        public const int TOGGLE_CONTROL_PANEL_UI_ID     = 2;
        public const int TOGGLE_MONITORING_ID           = 3;
        public const int TAKE_SCREENSHOT_ID             = 4;

        public static void Register( System.IntPtr hWnd )
        {
            if (hWnd == System.IntPtr.Zero)
            {
                Logger.MainDialog.Warn("HotkeyManager received a null window handle");
                return;
            }

            NativeMethods.RegisterHotKey(hWnd, TOGGLE_FORCE_ALWAYS_ON_TOP_ID, MOD_ALT | MOD_NOREPEAT, (int)System.Windows.Forms.Keys.F12);
            NativeMethods.RegisterHotKey(hWnd, TOGGLE_CONTROL_PANEL_UI_ID, MOD_ALT | MOD_NOREPEAT, (int)System.Windows.Forms.Keys.F11);
            NativeMethods.RegisterHotKey(hWnd, TOGGLE_MONITORING_ID, MOD_ALT | MOD_NOREPEAT, (int)System.Windows.Forms.Keys.F10);
            NativeMethods.RegisterHotKey(hWnd, TAKE_SCREENSHOT_ID, MOD_ALT | MOD_NOREPEAT, (int)System.Windows.Forms.Keys.F9);
        }
    }
}
