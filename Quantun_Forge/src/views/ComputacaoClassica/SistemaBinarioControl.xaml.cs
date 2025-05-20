using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Quantun_Forge.src.views
{
    public partial class SistemaBinarioControl : UserControl
    {
        public SistemaBinarioControl()
        {
            InitializeComponent();
        }

        // Conversão Decimal → Binário / Hex / Oct / ASCII
        private void Converter_Click(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(txtDecimal.Text, out int valor) && valor >= 0)
            {
                string binario = Convert.ToString(valor, 2);
                string hex = Convert.ToString(valor, 16).ToUpper();
                string oct = Convert.ToString(valor, 8);
                string ascii = valor <= 255 ? ((char)valor).ToString() : "(inválido)";

                lblBinario.Text = $"Binário: {binario}";
                lblHex.Text = $"Hexadecimal: 0x{hex}";
                lblOct.Text = $"Octal: {oct}";
                lblAscii.Text = $"ASCII: {ascii}";

                MostrarBits(binario);
            }
            else
            {
                lblBinario.Text = "Valor inválido.";
                lblHex.Text = lblOct.Text = lblAscii.Text = "";
                BitDisplay.Children.Clear();
                BitLabels.Children.Clear();
            }
        }

        // Conversão Binário → Decimal
        private void ConverterBinario_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string bin = txtBinario.Text.Trim();
                if (string.IsNullOrWhiteSpace(bin) || bin.Contains(' ') || !System.Text.RegularExpressions.Regex.IsMatch(bin, "^[01]+$"))
                {
                    lblDecimal.Text = "Binário inválido.";
                    return;
                }

                int dec = Convert.ToInt32(bin, 2);
                lblDecimal.Text = $"Decimal: {dec}";
            }
            catch
            {
                lblDecimal.Text = "Erro na conversão.";
            }
        }

        // Visualização dos bits
        private void MostrarBits(string binario)
        {
            BitDisplay.Children.Clear();
            BitLabels.Children.Clear();

            binario = binario.PadLeft(8, '0'); // garante 8 bits

            for (int i = 0; i < binario.Length; i++)
            {
                char bit = binario[i];
                int peso = (int)Math.Pow(2, binario.Length - 1 - i);

                // Bloco visual
                Border borda = new Border
                {
                    Width = 30,
                    Height = 30,
                    Background = bit == '1' ? Brushes.LimeGreen : Brushes.DimGray,
                    Margin = new Thickness(4),
                    CornerRadius = new CornerRadius(4),
                    Child = new TextBlock
                    {
                        Text = bit.ToString(),
                        Foreground = Brushes.White,
                        FontWeight = FontWeights.Bold,
                        FontSize = 16,
                        HorizontalAlignment = HorizontalAlignment.Center,
                        VerticalAlignment = VerticalAlignment.Center
                    }
                };

                BitDisplay.Children.Add(borda);

                // Peso abaixo do bit
                TextBlock lblPeso = new TextBlock
                {
                    Text = peso.ToString(),
                    Foreground = Brushes.LightGray,
                    FontSize = 12,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    Margin = new Thickness(4, 2, 4, 0)
                };
                BitLabels.Children.Add(lblPeso);
            }
        }
    }
}
