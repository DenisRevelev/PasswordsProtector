using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using WindowsDesktop;
namespace PasswordsProtector.Models.Services
{
    public class DesktopManager
    {

        private static VirtualDesktop desktop = VirtualDesktop.Create();

        public static async void InitializeComObjects()
        {
            try
            {
                await VirtualDesktopProvider.Default.Initialize(TaskScheduler.FromCurrentSynchronizationContext());
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Failed to initialize.");
            }

            VirtualDesktop.CurrentChanged += (sender, args) => System.Diagnostics.Debug.WriteLine($"Desktop changed: {args.NewDesktop.Id}");
        }

        public static void SwitchDesktop()
        {
            //VirtualDesktopHelper.MoveToDesktop(GetForegroundWindow(), desktop);
            desktop.Switch();
        }

        public static void RemoveDesktop()
        {
            desktop.Remove();
        }

        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();
    }
}
