using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
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
            desktop.Switch();
        }

        public static void RemoveDesktop()
        {
            desktop.Remove();
        }
    }
}
