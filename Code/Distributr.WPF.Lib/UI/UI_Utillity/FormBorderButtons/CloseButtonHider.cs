using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

namespace Distributr.WPF.Lib.UI.UI_Utillity.FormBorderButtons
{
    internal sealed class CloseButtonHider
    {
        private readonly WindowInteropHelper interopHelper;
        private int windowLong;
        private bool dirty;

        public Window Window
        {
            get;
            private set;
        }
        public bool IsButtonHidden
        {
            get;
            private set;
        }

        private bool IsSourceInitialized
        {
            get
            {
                return this.WindowHandle != IntPtr.Zero;
            }
        }
        private IntPtr WindowHandle
        {
            get
            {
                return this.interopHelper.Handle;
            }
        }

        public CloseButtonHider(Window window)
        {
            //Contract.Requires<ArgumentNullException>(window != null);
            //Contract.Ensures(Object.Equals(this.Window, window));

            this.Window = window;
            this.interopHelper = new WindowInteropHelper(window);

            if (!this.IsSourceInitialized)
                this.Window.SourceInitialized += this.Window_SourceInitialized;
        }

        public void Hide()
        {
            //Contract.Ensures(this.IsButtonHidden);

            if (this.IsButtonHidden)
                return;

            this.IsButtonHidden = true;

            this.ApplyChanges();
        }
        public void Show()
        {
            //Contract.Ensures(!this.IsButtonHidden);

            if (!this.IsButtonHidden)
                return;

            this.IsButtonHidden = false;

            this.ApplyChanges();
        }

        private void ApplyChanges()
        {
            if (!this.IsSourceInitialized)
            {
                this.dirty = true;
                return;
            }

            this.GetWindowLong();
            this.SetSysMenu(!this.IsButtonHidden);
            this.ApplyWindowLong();

            this.dirty = false;
        }

        private void GetWindowLong()
        {
            //Contract.Requires(this.IsSourceInitialized);

            this.windowLong = Win32.GetWindowLong(this.WindowHandle, Win32.GWL_STYLE);
        }
        private void ApplyWindowLong()
        {
            //Contract.Assert(this.IsSourceInitialized);

            this.SetWindowLong();
            this.RefreshWindow();
        }
        private void RefreshWindow()
        {
            //Contract.Requires(this.IsSourceInitialized);

            int result = Win32.SetWindowPos(this.WindowHandle, IntPtr.Zero, 0, 0, 100, 100, Win32.SWP_FRAMECHANGED | Win32.SWP_NOMOVE | Win32.SWP_NOSIZE);
            if (result == 0)
                throw new Win32Exception(Marshal.GetLastWin32Error());
        }
        private void SetWindowLong()
        {
            //Contract.Requires(this.IsSourceInitialized);

            IntPtr result = Win32.SetWindowLongPtr(this.WindowHandle, Win32.GWL_STYLE, new IntPtr(this.windowLong));
            if (result == IntPtr.Zero)
                throw new Win32Exception(Marshal.GetLastWin32Error());
        }
        private void SetSysMenu(bool value)
        {
            this.windowLong = this.windowLong.Set(Win32.WS_SYSMENU, value);
        }

        private void Window_SourceInitialized(object sender, EventArgs e)
        {
            //this.Window.SourceInitialized -= this.Window_SourceInitialized;
            //Contract.Assume(this.IsSourceInitialized);

            if (this.dirty)
                this.ApplyChanges();
        }

        //[ContractInvariantMethod]
        private void CloseButtonHiderInvariant()
        {
            //Contract.Invariant(this.interopHelper != null);
            //Contract.Invariant(this.Window != null);
        }
    }

}
