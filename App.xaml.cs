using System.Configuration;
using System.Data;
using System.Windows;
using Microsoft.Win32;
namespace Wpf_Licensing_registy_
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private bool IsLicenseStored()
        {
            RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Software\MyWpfApp");
            return key?.GetValue("LicenseKey") != null;
        }

        private string GetStoredLicense()
        {
            RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Software\MyWpfApp");
            return key?.GetValue("LicenseKey")?.ToString();
        }

        private void SaveLicenseToRegistry(string licenseKey)
        {
            RegistryKey key = Registry.CurrentUser.CreateSubKey(@"Software\MyWpfApp");
            key.SetValue("LicenseKey", licenseKey);
            key.Close();
        }

        private bool IsLicenseValid(string inputKey)
        {
            const string expectedKey = "WPF-KEY-2025-VAL"; // You can hash or validate more securely
            return inputKey == expectedKey;
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            if (!IsLicenseStored())
            {
                string enteredKey = Microsoft.VisualBasic.Interaction.InputBox(
                    "Enter your license key to activate the application:",
                    "License Activation",
                    "", -1, -1);

                if (string.IsNullOrWhiteSpace(enteredKey) || !IsLicenseValid(enteredKey))
                {
                    MessageBox.Show("Invalid license. Application will now close.");
                    Environment.Exit(0);
                }

                SaveLicenseToRegistry(enteredKey);
                MessageBox.Show("License activated successfully!");
            }
            else
            {
                string savedKey = GetStoredLicense();
                if (!IsLicenseValid(savedKey))
                {
                    MessageBox.Show("Stored license is invalid. Application will now close.");
                    Environment.Exit(0);
                }
            }

            // Proceed with loading your main window or dashboard
            // new MainWindow().Show();
        }

    }

}
