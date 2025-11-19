// SistemaBinarioControl.xaml.cs
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace Quantun_Forge.src.views
{
    public partial class SistemaBinarioControl : UserControl
    {
        private ToggleButton[] bitToggles = new ToggleButton[8];
        private Random random = new Random();

        public SistemaBinarioControl()
        {
            InitializeComponent();
            InicializarBits();
        }

        private void InicializarBits()
        {
            pnlBits.Children.Clear();

            // Cria 8 toggles para os bits (da esquerda para direita: bit 7 a bit 0)
            for (int i = 7; i >= 0; i--)
            {
                var toggle = new ToggleButton
                {
                    Style = (Style)FindResource("BitToggleStyle"),
                    Tag = i
                };

                toggle.Checked += BitToggle_Changed;
                toggle.Unchecked += BitToggle_Changed;

                bitToggles[i] = toggle;
                pnlBits.Children.Add(toggle);
            }

            AtualizarResultados();
        }

        private void BitToggle_Changed(object? sender, RoutedEventArgs e)
        {
            AtualizarResultados();
        }

        private void AtualizarResultados()
        {
            int valor = ObterValorAtual();

            // Atualiza todas as conversões
            txtResultDecimal.Text = valor.ToString();
            txtResultBinario.Text = Convert.ToString(valor, 2).PadLeft(8, '0');
            txtResultHex.Text = $"0x{valor:X2}";
            txtResultOctal.Text = Convert.ToString(valor, 8).PadLeft(3, '0');

            // ASCII (apenas caracteres imprimíveis)
            if (valor >= 32 && valor <= 126)
            {
                txtResultASCII.Text = $"'{(char)valor}'";
            }
            else if (valor == 10)
            {
                txtResultASCII.Text = "\\n (nova linha)";
            }
            else if (valor == 13)
            {
                txtResultASCII.Text = "\\r (retorno)";
            }
            else if (valor == 9)
            {
                txtResultASCII.Text = "\\t (tab)";
            }
            else
            {
                txtResultASCII.Text = "(não imprimível)";
            }

            AnimarElemento(txtResultDecimal);
        }

        private int ObterValorAtual()
        {
            int valor = 0;
            for (int i = 0; i < 8; i++)
            {
                if (bitToggles[i].IsChecked == true)
                {
                    valor += (1 << i);
                }
            }
            return valor;
        }

        private void DefinirValor(int valor)
        {
            // Limita entre 0 e 255
            valor = Math.Max(0, Math.Min(255, valor));

            for (int i = 0; i < 8; i++)
            {
                bitToggles[i].IsChecked = ((valor >> i) & 1) == 1;
            }

            AtualizarResultados();
        }

        // Botões de controle rápido
        private void InverterBits_Click(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < 8; i++)
            {
                bitToggles[i].IsChecked = !(bitToggles[i].IsChecked == true);
            }
            AtualizarResultados();
        }

        private void TodosON_Click(object sender, RoutedEventArgs e)
        {
            DefinirValor(255);
        }

        private void TodosOFF_Click(object sender, RoutedEventArgs e)
        {
            DefinirValor(0);
        }

        private void Aleatorio_Click(object sender, RoutedEventArgs e)
        {
            DefinirValor(random.Next(0, 256));
        }

        // Conversor Manual - Decimal para outras bases
        private void ConverterDecimal_Click(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(txtInputDecimal.Text, out int valor) && valor >= 0 && valor <= 255)
            {
                DefinirValor(valor);
                txtInputDecimal.Clear();
            }
            else
            {
                MessageBox.Show("Digite um valor entre 0 e 255!", "Valor Inválido", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        // Conversor Manual - Binário para Decimal
        private void ConverterBinario_Click(object sender, RoutedEventArgs e)
        {
            string binario = txtInputBinario.Text.Trim();

            // Valida se contém apenas 0s e 1s
            if (string.IsNullOrWhiteSpace(binario))
            {
                MessageBox.Show("Digite um número binário!", "Entrada Vazia", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            foreach (char c in binario)
            {
                if (c != '0' && c != '1')
                {
                    MessageBox.Show("Use apenas dígitos 0 e 1!", "Binário Inválido", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
            }

            try
            {
                int valor = Convert.ToInt32(binario, 2);

                if (valor > 255)
                {
                    MessageBox.Show("Valor excede 255! Use no máximo 8 bits.", "Valor Muito Grande", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                DefinirValor(valor);
                txtInputBinario.Clear();
            }
            catch
            {
                MessageBox.Show("Erro ao converter o número binário!", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Operações Binárias
        private void Somar_Click(object sender, RoutedEventArgs e)
        {
            if (ValidarOperandos(out int a, out int b))
            {
                int resultado = (a + b) & 0xFF; // Limita a 8 bits
                DefinirValor(resultado);
                txtResultOperacao.Text = $"{a} + {b} = {resultado} (Decimal)\n{Convert.ToString(a, 2).PadLeft(8, '0')} + {Convert.ToString(b, 2).PadLeft(8, '0')} = {Convert.ToString(resultado, 2).PadLeft(8, '0')} (Binário)";
                AnimarElemento(txtResultOperacao);
            }
        }

        private void Subtrair_Click(object sender, RoutedEventArgs e)
        {
            if (ValidarOperandos(out int a, out int b))
            {
                int resultado = Math.Max(0, a - b); // Não permite negativos
                DefinirValor(resultado);
                txtResultOperacao.Text = $"{a} - {b} = {resultado} (Decimal)\n{Convert.ToString(a, 2).PadLeft(8, '0')} - {Convert.ToString(b, 2).PadLeft(8, '0')} = {Convert.ToString(resultado, 2).PadLeft(8, '0')} (Binário)";
                AnimarElemento(txtResultOperacao);
            }
        }

        private void AND_Click(object sender, RoutedEventArgs e)
        {
            if (ValidarOperandos(out int a, out int b))
            {
                int resultado = a & b;
                DefinirValor(resultado);
                txtResultOperacao.Text = $"{a} AND {b} = {resultado} (Decimal)\n{Convert.ToString(a, 2).PadLeft(8, '0')} ∧ {Convert.ToString(b, 2).PadLeft(8, '0')} = {Convert.ToString(resultado, 2).PadLeft(8, '0')} (Binário)";
                AnimarElemento(txtResultOperacao);
            }
        }

        private void OR_Click(object sender, RoutedEventArgs e)
        {
            if (ValidarOperandos(out int a, out int b))
            {
                int resultado = a | b;
                DefinirValor(resultado);
                txtResultOperacao.Text = $"{a} OR {b} = {resultado} (Decimal)\n{Convert.ToString(a, 2).PadLeft(8, '0')} ∨ {Convert.ToString(b, 2).PadLeft(8, '0')} = {Convert.ToString(resultado, 2).PadLeft(8, '0')} (Binário)";
                AnimarElemento(txtResultOperacao);
            }
        }

        private void XOR_Click(object sender, RoutedEventArgs e)
        {
            if (ValidarOperandos(out int a, out int b))
            {
                int resultado = a ^ b;
                DefinirValor(resultado);
                txtResultOperacao.Text = $"{a} XOR {b} = {resultado} (Decimal)\n{Convert.ToString(a, 2).PadLeft(8, '0')} ⊕ {Convert.ToString(b, 2).PadLeft(8, '0')} = {Convert.ToString(resultado, 2).PadLeft(8, '0')} (Binário)";
                AnimarElemento(txtResultOperacao);
            }
        }

        private void ShiftLeft_Click(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(txtOpA.Text, out int a) && a >= 0 && a <= 255)
            {
                int resultado = (a << 1) & 0xFF; // Shift left 1 posição, limita a 8 bits
                DefinirValor(resultado);
                txtResultOperacao.Text = $"{a} << 1 = {resultado} (Decimal)\n{Convert.ToString(a, 2).PadLeft(8, '0')} << 1 = {Convert.ToString(resultado, 2).PadLeft(8, '0')} (Binário)";
                AnimarElemento(txtResultOperacao);
            }
            else
            {
                MessageBox.Show("Digite um valor válido no campo A (0-255)!", "Valor Inválido", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void ShiftRight_Click(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(txtOpA.Text, out int a) && a >= 0 && a <= 255)
            {
                int resultado = a >> 1; // Shift right 1 posição
                DefinirValor(resultado);
                txtResultOperacao.Text = $"{a} >> 1 = {resultado} (Decimal)\n{Convert.ToString(a, 2).PadLeft(8, '0')} >> 1 = {Convert.ToString(resultado, 2).PadLeft(8, '0')} (Binário)";
                AnimarElemento(txtResultOperacao);
            }
            else
            {
                MessageBox.Show("Digite um valor válido no campo A (0-255)!", "Valor Inválido", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private bool ValidarOperandos(out int a, out int b)
        {
            a = 0;
            b = 0;

            if (!int.TryParse(txtOpA.Text, out a) || a < 0 || a > 255)
            {
                MessageBox.Show("Digite um valor válido no campo A (0-255)!", "Valor Inválido", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (!int.TryParse(txtOpB.Text, out b) || b < 0 || b > 255)
            {
                MessageBox.Show("Digite um valor válido no campo B (0-255)!", "Valor Inválido", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            return true;
        }

        private void AnimarElemento(UIElement elemento)
        {
            var fade = new DoubleAnimation
            {
                From = 0.5,
                To = 1.0,
                Duration = TimeSpan.FromSeconds(0.3)
            };
            elemento.BeginAnimation(OpacityProperty, fade);
        }
    }
}