using System;
using System.Linq;
using System.Text;
using System.Windows;
using System.Security.Cryptography;

namespace PasswordGenerator.Src
{
    public partial class Generator
    {
        private const string LowerCase = "abcdefghijklmnopqrstuvwxyz";
        private const string UpperCase = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        private const string NumberCase = "1234567890";
        private const string SymbolCase = "!@#$%^&*()=+?";
        private const string SymbolCaseA = ",.~`_-[]{}<>:;/\\|";
        private const string SymbolCaseU = "¡¢£¤¥¦§¨©ª«¬®¯°±²³´µ¶·¸¹º»¼½¾¿";

        public Domain Domain { set; get; }

        public Generator()
        {
            InitializeComponent();
            LengthComboBox.SelectedIndex = 0;
            UpperCaseC.IsChecked = true;
            TypeSelector.SelectedIndex = 2;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var builder = new StringBuilder();
            if (UpperCaseC.IsChecked != null && (bool) UpperCaseC.IsChecked) builder.Append(UpperCase);

            if (LowerCaseC.IsChecked != null && (bool) LowerCaseC.IsChecked) builder.Append(LowerCase);

            if (NumberC.IsChecked != null && (bool) NumberC.IsChecked) builder.Append(NumberCase);

            if (SymbolC.IsChecked != null && (bool) SymbolC.IsChecked) builder.Append(SymbolCase);

            if (SymbolAc.IsChecked != null && (bool) SymbolAc.IsChecked) builder.Append(SymbolCaseA);

            if (SymbolUc.IsChecked != null && (bool) SymbolUc.IsChecked) builder.Append(SymbolCaseU);

            if (builder.Length != 0) Generate(builder.ToString());
        }

        private void Generate(string valid)
        {
            var lengthOutput = int.Parse(LengthComboBox.Text);
            var output = new StringBuilder();
            using (var random = new RNGCryptoServiceProvider())
            {
                while (output.Length != lengthOutput)
                {
                    var buffer = new byte[1];
                    random.GetBytes(buffer);
                    var cur = (char) buffer[0];
                    if (valid.Contains(cur)) output.Append(cur);
                }
            }

            DomainField.IsEnabled = true;
            UsernameField.IsEnabled = true;
            Submit.IsEnabled = true;
            CommentField.IsEnabled = true;
            OutputField.Text = output.ToString();
            Clipboard.SetText(output.ToString());
        }

        private void Submit_Click(object sender, RoutedEventArgs e)
        {
            if (DomainField.Text.Equals("") || UsernameField.Text.Equals("") || OutputField.Text.Equals(""))
            {
                MessageBox.Show("Fields cannot be left blank!", "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                Domain = new Domain(DomainField.Text, UsernameField.Text, OutputField.Text, DateTime.Now,
                    CommentField.Text, (Type)Enum.Parse(typeof(Type), TypeSelector.Text, true));
                Close();
            }
        }
    }
}