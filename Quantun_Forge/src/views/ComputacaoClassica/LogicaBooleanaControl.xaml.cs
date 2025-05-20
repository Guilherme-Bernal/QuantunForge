// LogicaBooleanaControl.xaml.cs
using System;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Collections.Generic;
using System.Windows.Media.Animation;

namespace Quantun_Forge.src.views
{
    public partial class LogicaBooleanaControl : UserControl
    {
        public LogicaBooleanaControl()
        {
            InitializeComponent();
        }

        private void GerarTabela_Click(object sender, RoutedEventArgs e)
        {
            string expressao = txtExpressao.Text;
            var tabela = new List<TabelaLinha>();

            for (int i = 0; i < 8; i++)
            {
                bool a = (i & 4) != 0;
                bool b = (i & 2) != 0;
                bool c = (i & 1) != 0;
                bool resultado = AvaliarExpressao(expressao, a, b, c);

                tabela.Add(new TabelaLinha
                {
                    A = a,
                    B = b,
                    C = c,
                    Resultado = resultado
                });
            }

            dataGrid.ItemsSource = tabela;
            AnimarControle(dataGrid);
        }

        private bool AvaliarExpressao(string expressao, bool a, bool b, bool c)
        {
            try
            {
                string expr = expressao.ToLower()
                    .Replace("a", a ? "true" : "false")
                    .Replace("b", b ? "true" : "false")
                    .Replace("c", c ? "true" : "false")
                    .Replace("&&", "AND")
                    .Replace("||", "OR")
                    .Replace("!", "NOT ");

                var table = new DataTable();
                table.Columns.Add("", typeof(bool));
                table.Columns[0].Expression = expr;
                var row = table.NewRow();
                table.Rows.Add(row);
                return (bool)row[0];
            }
            catch
            {
                return false;
            }
        }

        private void AtualizarResultado(object sender, RoutedEventArgs e)
        {
            string expr = txtExpressao.Text;
            bool a = chkA.IsChecked == true;
            bool b = chkB.IsChecked == true;
            bool c = chkC.IsChecked == true;

            if (string.IsNullOrWhiteSpace(expr))
            {
                txtResultado.Text = "Digite uma expressão.";
                return;
            }

            bool resultado = AvaliarExpressao(expr, a, b, c);
            txtResultado.Text = resultado ? "Verdadeiro (1)" : "Falso (0)";
            AnimarControle(txtResultado);
        }

        private void LimparTudo_Click(object sender, RoutedEventArgs e)
        {
            chkA.IsChecked = false;
            chkB.IsChecked = false;
            chkC.IsChecked = false;
            txtExpressao.Text = "";
            txtResultado.Text = "";
            dataGrid.ItemsSource = null;
        }

        private void AnimarControle(UIElement elemento)
        {
            var fade = new DoubleAnimation
            {
                From = 0.0,
                To = 1.0,
                Duration = TimeSpan.FromSeconds(0.5)
            };
            elemento.BeginAnimation(OpacityProperty, fade);
        }

        public class TabelaLinha
        {
            public bool A { get; set; }
            public bool B { get; set; }
            public bool C { get; set; }
            public bool Resultado { get; set; }
        }
    }
}
