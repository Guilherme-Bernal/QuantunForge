using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace Quantun_Forge.src.views
{
    public partial class PortasLogicasControl : UserControl
    {
        public PortasLogicasControl()
        {
            InitializeComponent();
            cmbPorta.SelectedIndex = 0;
        }

        private void Entrada_Changed(object sender, RoutedEventArgs e)
        {
            AtualizarResultado();
        }

        private void AtualizarResultado()
        {
            bool a = chkA.IsChecked ?? false;
            bool b = chkB.IsChecked ?? false;
            string operacao = (cmbPorta.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "";

            bool resultado = operacao switch
            {
                "AND" => a && b,
                "OR" => a || b,
                "NOT (A)" => !a,
                "XOR" => a ^ b,
                "NAND" => !(a && b),
                "NOR" => !(a || b),
                "XNOR" => !(a ^ b),
                _ => false
            };

            txtResultado.Text = resultado ? "1" : "0";
        }

        private void MostrarTabela_Click(object sender, RoutedEventArgs e)
        {
            string operacao = (cmbPorta.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "";
            var tabela = new List<TabelaVerdadeLinha>();

            if (operacao == "NOT (A)")
            {
                for (int i = 0; i <= 1; i++)
                {
                    bool a = i == 1;
                    bool r = !a;
                    tabela.Add(new TabelaVerdadeLinha { A = a, Resultado = r });
                }
            }
            else
            {
                for (int i = 0; i < 4; i++)
                {
                    bool a = (i & 2) != 0;
                    bool b = (i & 1) != 0;
                    bool r = operacao switch
                    {
                        "AND" => a && b,
                        "OR" => a || b,
                        "XOR" => a ^ b,
                        "NAND" => !(a && b),
                        "NOR" => !(a || b),
                        "XNOR" => !(a ^ b),
                        _ => false
                    };
                    tabela.Add(new TabelaVerdadeLinha { A = a, B = b, Resultado = r });
                }
            }

            dgTabelaVerdade.ItemsSource = tabela;
            dgTabelaVerdade.Visibility = Visibility.Visible;
        }

        public class TabelaVerdadeLinha
        {
            public bool A { get; set; }
            public bool B { get; set; }
            public bool Resultado { get; set; }
        }
    }
}
