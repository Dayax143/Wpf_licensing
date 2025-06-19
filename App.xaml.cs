using System.Configuration;
using System.Data;
using System.Windows;
using System.Windows.Input;
using Microsoft.Win32;
namespace Wpf_Licensing_registy_
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private const string RegistryPath = @"Software\MyWpfApp";
        private const string PredefinedKey = "WPF-KEY-2025-VALID";
        private bool IsLicenseStored()
        {
            using RegistryKey key = Registry.CurrentUser.OpenSubKey(RegistryPath);
            return key?.GetValue("LicenseKey") != null && key.GetValue("LicenseDate") != null;
        }

        private void SaveLicenseToRegistry(string licenseKey)
        {
            RegistryKey key = Registry.CurrentUser.CreateSubKey(RegistryPath);
            key.SetValue("LicenseKey", licenseKey);
            key.SetValue("LicenseDate", DateTime.Now.ToString("yyyy-MM-dd"));
            key.Close();
        }

        private bool IsLicenseExpired()
        {
            using RegistryKey key = Registry.CurrentUser.OpenSubKey(RegistryPath);
            if (key == null) return true;

            object dateValue = key.GetValue("LicenseDate");
            if (dateValue == null) return true;

            DateTime installDate;
            if (!DateTime.TryParse(dateValue.ToString(), out installDate)) return true;

            return (DateTime.Now - installDate).TotalDays > 30; // Expired if more than 30 days
        }


        private bool IsLicenseValid(string inputKey)
        {
            // You can hash or validate more securely
            return inputKey == PredefinedKey;
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            if (!IsLicenseStored())
            {
                string enteredKey = Microsoft.VisualBasic.Interaction.InputBox(
                    "Enter your license key to activate the application:",
                    "License Activation", "", -1, -1);

                if (string.IsNullOrWhiteSpace(enteredKey) || !IsLicenseValid(enteredKey))
                {
                    MessageBox.Show("Invalid license. Application will now close.");
                    Environment.Exit(0);
                }

                SaveLicenseToRegistry(enteredKey);
                MessageBox.Show("License registered successfully.");
            }
            else
            {
                using RegistryKey key = Registry.CurrentUser.OpenSubKey(RegistryPath);
                string storedKey = key?.GetValue("LicenseKey")?.ToString();

                if (storedKey != PredefinedKey)
                {
                    MessageBox.Show("License key has been tampered or is invalid.");
                    Environment.Exit(0);
                }

                if (IsLicenseExpired())
                {
                    MessageBox.Show("Your license has expired.");
                    Environment.Exit(0);
                }

                // All checks passed
                // new MainWindow().Show(); // Continue to app
            }
        }

        // Proceed with loading your main window or dashboard
        // new MainWindow().Show();
    }

}
