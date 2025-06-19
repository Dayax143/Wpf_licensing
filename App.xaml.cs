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

        private void SaveLicenseToRegistry(string licenseKey)
        {
            RegistryKey key = Registry.CurrentUser.CreateSubKey(@"Software\MyWpfApp");
            key.SetValue("LicenseKey", licenseKey);
            key.SetValue("LicenseDate", DateTime.Now.ToString("yyyy-MM-dd"));
            key.Close();
        }


        private bool IsLicenseExpired()
        {
            RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Software\MyWpfApp");
            if (key == null) return true;

            object dateValue = key.GetValue("LicenseDate");
            if (dateValue == null) return true;

            DateTime installDate;
            if (!DateTime.TryParse(dateValue.ToString(), out installDate)) return true;

            return (DateTime.Now - installDate).TotalDays > 30; // Expired if more than 30 days
        }


        private bool IsLicenseValid(string inputKey)
        {
            //const string expectedKey = "WPF-KEY-2025-VALID"; // You can hash or validate more securely

            RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Software\MyWpfApp");
            if (key == null) return true;
            object dateValue = key.GetValue("LicenseDate");

            if (dateValue == null) return true;

            DateTime installDate;
            if (!DateTime.TryParse(dateValue.ToString(), out installDate)) return true;

            return (DateTime.Now - installDate).TotalDays > 30; // Expired if more than 30 days
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
            else if (IsLicenseExpired())
            {
                MessageBox.Show("Your license has expired already");
                Environment.Exit(0);
            }
            // Proceed with loading your main window or dashboard
            // new MainWindow().Show();
        }

    }

}
