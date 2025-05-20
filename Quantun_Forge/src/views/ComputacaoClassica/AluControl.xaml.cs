using System;
using System.Windows;
using System.Windows.Controls;

namespace Quantun_Forge.src.views
{
    public partial class AluControl : UserControl
    {
        public AluControl()
        {
            InitializeComponent();
        }

        private void Calcular_Click(object sender, RoutedEventArgs e)
        {
            if (!double.TryParse(txtOperando1.Text, out double op1) ||
                !double.TryParse(txtOperando2.Text, out double op2))
            {
                lblResultado.Text = "Entradas inválidas";
                lblBinario.Text = "";
                return;
            }

            string operacao = (cmbOperacao.SelectedItem as ComboBoxItem)?.Content.ToString();
            double resultado = 0;

            lblR1.Text = op1.ToString();
            lblR2.Text = op2.ToString();

            bool isOperacaoLogica = operacao == "AND" || operacao == "OR" || operacao == "XOR";

            if (isOperacaoLogica && (op1 % 1 != 0 || op2 % 1 != 0))
            {
                lblResultado.Text = "Operações lógicas exigem inteiros.";
                lblBinario.Text = "";
                return;
            }

            switch (operacao)
            {
                case "Soma": resultado = op1 + op2; break;
                case "Subtração": resultado = op1 - op2; break;
                case "Multiplicação": resultado = op1 * op2; break;
                case "Divisão": resultado = op2 != 0 ? op1 / op2 : double.NaN; break;
                case "AND": resultado = (int)op1 & (int)op2; break;
                case "OR": resultado = (int)op1 | (int)op2; break;
                case "XOR": resultado = (int)op1 ^ (int)op2; break;
                default:
                    lblResultado.Text = "Operação inválida";
                    lblBinario.Text = "";
                    return;
            }

            lblResultado.Text = $"Resultado: {resultado}";
            lblBinario.Text = "";

            // Adiciona histórico numérico
            lstHistorico.Items.Insert(0, $"{op1} {operacao} {op2} = {resultado}");

            // Somente mostra binário se todos forem inteiros
            if (op1 % 1 == 0 && op2 % 1 == 0 && resultado % 1 == 0)
            {
                string bin1 = Convert.ToString((int)op1, 2).PadLeft(8, '0');
                string bin2 = Convert.ToString((int)op2, 2).PadLeft(8, '0');
                string binR = Convert.ToString((int)resultado, 2).PadLeft(8, '0');

                lstHistorico.Items.Insert(1, $"({bin1}) {operacao} ({bin2}) = ({binR})");
                lblBinario.Text = $"Binário: {binR}";
            }
        }
    }
}
