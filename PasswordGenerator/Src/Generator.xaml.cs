using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Security.Cryptography;

namespace PasswordGenerator.Src
{
    public partial class Generator : Window
    {
        private const string lowerCase = "abcdefghijklmnopqrstuvwxyz";
        private const string upperCase = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        private const string numberCase = "1234567890";
        private const string symbolCase = "!@#$%^&*";
        private const string symbolCaseU = "<>(){}";

        public Generator()
        {
            InitializeComponent();
            comboBox.SelectedIndex = 0;
            upperCaseC.IsChecked = true;
            TypeSelector.SelectedIndex = 2;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder builder = new StringBuilder();
            if ((bool)upperCaseC.IsChecked)
            {
                builder.Append(upperCase);
            }
            if ((bool)lowerCaseC.IsChecked)
            {
                builder.Append(lowerCase);
            }
            if ((bool)numberC.IsChecked)
            {
                builder.Append(numberCase);
            }
            if ((bool)symbolC.IsChecked)
            {
                builder.Append(symbolCase);
            }
            if ((bool)symbolUC.IsChecked)
            {
                builder.Append(symbolCaseU);
            }
            if(builder.Length != 0)
            {
                Generate(builder.ToString());
            } 
        }

        private void Generate(String valid)
        {
            int lengthOutput = Int32.Parse(comboBox.Text);
            StringBuilder output = new StringBuilder();
            using (RNGCryptoServiceProvider random = new RNGCryptoServiceProvider())
            {
                while (output.Length != lengthOutput)
                {
                    byte[] buffer = new byte[1];
                    random.GetBytes(buffer);
                    char cur = (char)buffer[0];
                    if (valid.Contains(cur))
                    {
                        output.Append(cur);
                    }
                }
            }
            domainField.IsEnabled = true;
            usernameField.IsEnabled = true;
            submit.IsEnabled = true;
            commentField.IsEnabled = true;
            outputField.Text = output.ToString();
            Clipboard.SetText(output.ToString());
        }

        private void Submit_Click(object sender, RoutedEventArgs e)
        {
            var table = Application.Current.Windows.OfType<TableView>().FirstOrDefault();
            table?.Modify(new Domain(domainField.Text, usernameField.Text, outputField.Text, DateTime.Now,
                commentField.Text, (Type) Enum.Parse(typeof(Type), TypeSelector.Text, true)));
            Close();
        }
    }
}
