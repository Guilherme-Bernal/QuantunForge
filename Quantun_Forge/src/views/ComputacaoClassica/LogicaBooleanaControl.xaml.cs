// LogicaBooleanaControl.xaml.cs
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace Quantun_Forge.src.views
{
    public partial class LogicaBooleanaControl : UserControl
    {
        private string portaSelecionada = "";

        public LogicaBooleanaControl()
        {
            InitializeComponent();
            AtualizarTodasPortas();
        }

        // Atualiza todas as portas lógicas quando as entradas mudam
        private void AtualizarResultado(object sender, RoutedEventArgs e)
        {
            AtualizarTodasPortas();
        }

        private void AtualizarTodasPortas()
        {
            bool a = toggleA.IsChecked == true;
            bool b = toggleB.IsChecked == true;
            bool c = toggleC.IsChecked == true;
            bool d = toggleD.IsChecked == true;

            // AND
            bool resultAND = a && b;
            txtResultAND.Text = resultAND ? "1" : "0";
            txtResultAND.Foreground = resultAND ? Brushes.Green : new SolidColorBrush(Color.FromRgb(231, 76, 60));

            // OR
            bool resultOR = a || b;
            txtResultOR.Text = resultOR ? "1" : "0";
            txtResultOR.Foreground = resultOR ? Brushes.Green : new SolidColorBrush(Color.FromRgb(231, 76, 60));

            // NOT (usando A)
            bool resultNOT = !a;
            txtResultNOT.Text = resultNOT ? "1" : "0";
            txtResultNOT.Foreground = resultNOT ? Brushes.Green : new SolidColorBrush(Color.FromRgb(231, 76, 60));

            // NAND
            bool resultNAND = !(a && b);
            txtResultNAND.Text = resultNAND ? "1" : "0";
            txtResultNAND.Foreground = resultNAND ? Brushes.Green : new SolidColorBrush(Color.FromRgb(231, 76, 60));

            // NOR
            bool resultNOR = !(a || b);
            txtResultNOR.Text = resultNOR ? "1" : "0";
            txtResultNOR.Foreground = resultNOR ? Brushes.Green : new SolidColorBrush(Color.FromRgb(231, 76, 60));

            // XOR
            bool resultXOR = a ^ b;
            txtResultXOR.Text = resultXOR ? "1" : "0";
            txtResultXOR.Foreground = resultXOR ? Brushes.Green : new SolidColorBrush(Color.FromRgb(231, 76, 60));
        }

        // Handlers para clique nas portas lógicas
        private void PortaAND_Click(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            portaSelecionada = "AND";
            GerarTabelaPorta("A ∧ B", (a, b, c, d) => a && b);
        }

        private void PortaOR_Click(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            portaSelecionada = "OR";
            GerarTabelaPorta("A ∨ B", (a, b, c, d) => a || b);
        }

        private void PortaNOT_Click(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            portaSelecionada = "NOT";
            GerarTabelaPortaNOT();
        }

        private void PortaNAND_Click(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            portaSelecionada = "NAND";
            GerarTabelaPorta("¬(A ∧ B)", (a, b, c, d) => !(a && b));
        }

        private void PortaNOR_Click(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            portaSelecionada = "NOR";
            GerarTabelaPorta("¬(A ∨ B)", (a, b, c, d) => !(a || b));
        }

        private void PortaXOR_Click(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            portaSelecionada = "XOR";
            GerarTabelaPorta("A ⊕ B", (a, b, c, d) => a ^ b);
        }

        // Gera tabela verdade para porta NOT (apenas 1 entrada)
        private void GerarTabelaPortaNOT()
        {
            var tabela = new List<TabelaLinhaNOT>();

            tabela.Add(new TabelaLinhaNOT { A = false, Resultado = true });
            tabela.Add(new TabelaLinhaNOT { A = true, Resultado = false });

            dataGridTabela.ItemsSource = tabela;
            txtTituloTabela.Text = "- ¬A";
            borderTabelaVerdade.Visibility = Visibility.Visible;
            AnimarControle(borderTabelaVerdade);
        }

        // Gera tabela verdade para portas com 2 entradas
        private void GerarTabelaPorta(string expressao, Func<bool, bool, bool, bool, bool> funcao)
        {
            var tabela = new List<TabelaLinha2Entradas>();

            for (int i = 0; i < 4; i++)
            {
                bool a = (i & 2) != 0;
                bool b = (i & 1) != 0;
                bool resultado = funcao(a, b, false, false);

                tabela.Add(new TabelaLinha2Entradas
                {
                    A = a,
                    B = b,
                    Resultado = resultado
                });
            }

            dataGridTabela.ItemsSource = tabela;
            txtTituloTabela.Text = $"- {expressao}";
            borderTabelaVerdade.Visibility = Visibility.Visible;
            AnimarControle(borderTabelaVerdade);
        }

        // Avalia expressão customizada
        private void AvaliarExpressao_Click(object sender, RoutedEventArgs e)
        {
            string expressao = txtExpressao.Text.Trim();

            if (string.IsNullOrWhiteSpace(expressao))
            {
                MessageBox.Show("Digite uma expressão válida!\n\nExemplos:\nA & B\n(A | B) & !C\nA ^ B", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Verifica se não contém atribuições (=)
            if (expressao.Contains("="))
            {
                MessageBox.Show("Expressão inválida!\n\nNão use '=' na expressão.\nUse os botões de entrada para definir os valores.\n\nExemplos de expressões válidas:\nA & B\n(A | B) & !C\nA ^ B", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            bool a = toggleA.IsChecked == true;
            bool b = toggleB.IsChecked == true;
            bool c = toggleC.IsChecked == true;
            bool d = toggleD.IsChecked == true;

            try
            {
                bool resultado = AvaliarExpressaoBooleana(expressao, a, b, c, d);

                txtResultadoExpressao.Text = resultado ? "1 (TRUE)" : "0 (FALSE)";
                txtResultadoExpressao.Foreground = resultado ? Brushes.Green : new SolidColorBrush(Color.FromRgb(231, 76, 60));
                borderResultadoExpressao.Visibility = Visibility.Visible;
                AnimarControle(borderResultadoExpressao);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao avaliar expressão: Expressão inválida: {expressao}\n\nVerifique a sintaxe!\n\nOperadores válidos:\n& (AND)\n| (OR)\n! (NOT)\n^ (XOR)\n\nExemplo: (A & B) | !C", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Gera tabela verdade completa para expressão customizada
        private void GerarTabela_Click(object sender, RoutedEventArgs e)
        {
            string expressao = txtExpressao.Text.Trim();

            if (string.IsNullOrWhiteSpace(expressao))
            {
                MessageBox.Show("Digite uma expressão válida!\n\nExemplos:\nA & B\n(A | B) & !C\nA ^ B", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Verifica se não contém atribuições (=)
            if (expressao.Contains("="))
            {
                MessageBox.Show("Expressão inválida!\n\nNão use '=' na expressão.\nUse os botões de entrada para definir os valores.\n\nExemplos de expressões válidas:\nA & B\n(A | B) & !C\nA ^ B", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            try
            {
                // Detecta quantas variáveis são usadas
                bool usaA = expressao.ToUpper().Contains("A");
                bool usaB = expressao.ToUpper().Contains("B");
                bool usaC = expressao.ToUpper().Contains("C");
                bool usaD = expressao.ToUpper().Contains("D");

                int numVariaveis = (usaA ? 1 : 0) + (usaB ? 1 : 0) + (usaC ? 1 : 0) + (usaD ? 1 : 0);

                if (numVariaveis == 0)
                {
                    MessageBox.Show("A expressão não contém variáveis válidas (A, B, C, D)!", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                int numLinhas = (int)Math.Pow(2, numVariaveis);

                var tabela = new List<TabelaLinha4Entradas>();

                for (int i = 0; i < numLinhas; i++)
                {
                    bool a = usaA && ((i & 8) != 0);
                    bool b = usaB && ((i & 4) != 0);
                    bool c = usaC && ((i & 2) != 0);
                    bool d = usaD && ((i & 1) != 0);

                    bool resultado = AvaliarExpressaoBooleana(expressao, a, b, c, d);

                    tabela.Add(new TabelaLinha4Entradas
                    {
                        A = a,
                        B = b,
                        C = c,
                        D = d,
                        Resultado = resultado
                    });
                }

                dataGridTabela.ItemsSource = tabela;
                txtTituloTabela.Text = $"- {expressao}";
                borderTabelaVerdade.Visibility = Visibility.Visible;
                AnimarControle(borderTabelaVerdade);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao gerar tabela!\n\nVerifique a sintaxe da expressão.\n\nOperadores válidos:\n& (AND)\n| (OR)\n! (NOT)\n^ (XOR)\n\nExemplo: (A & B) | !C", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Avalia expressão booleana usando os operadores
        private bool AvaliarExpressaoBooleana(string expressao, bool a, bool b, bool c, bool d)
        {
            // Substitui variáveis pelos valores
            string expr = expressao.ToUpper()
                .Replace("A", a ? "1" : "0")
                .Replace("B", b ? "1" : "0")
                .Replace("C", c ? "1" : "0")
                .Replace("D", d ? "1" : "0");

            // Avalia a expressão
            return AvaliarExpressaoRecursiva(expr);
        }

        // Avaliador recursivo simples de expressões booleanas
        private bool AvaliarExpressaoRecursiva(string expr)
        {
            expr = expr.Trim();

            // Remove parênteses externos
            while (expr.StartsWith("(") && expr.EndsWith(")"))
            {
                int count = 0;
                bool valido = true;
                for (int i = 0; i < expr.Length - 1; i++)
                {
                    if (expr[i] == '(') count++;
                    if (expr[i] == ')') count--;
                    if (count == 0)
                    {
                        valido = false;
                        break;
                    }
                }
                if (valido)
                    expr = expr.Substring(1, expr.Length - 2).Trim();
                else
                    break;
            }

            // Processa NOT
            if (expr.StartsWith("!"))
            {
                return !AvaliarExpressaoRecursiva(expr.Substring(1));
            }

            // Processa OR (menor precedência)
            int nivelParenteses = 0;
            for (int i = expr.Length - 1; i >= 0; i--)
            {
                if (expr[i] == ')') nivelParenteses++;
                if (expr[i] == '(') nivelParenteses--;

                if (nivelParenteses == 0 && expr[i] == '|')
                {
                    return AvaliarExpressaoRecursiva(expr.Substring(0, i)) ||
                           AvaliarExpressaoRecursiva(expr.Substring(i + 1));
                }
            }

            // Processa XOR
            nivelParenteses = 0;
            for (int i = expr.Length - 1; i >= 0; i--)
            {
                if (expr[i] == ')') nivelParenteses++;
                if (expr[i] == '(') nivelParenteses--;

                if (nivelParenteses == 0 && expr[i] == '^')
                {
                    return AvaliarExpressaoRecursiva(expr.Substring(0, i)) ^
                           AvaliarExpressaoRecursiva(expr.Substring(i + 1));
                }
            }

            // Processa AND (maior precedência)
            nivelParenteses = 0;
            for (int i = expr.Length - 1; i >= 0; i--)
            {
                if (expr[i] == ')') nivelParenteses++;
                if (expr[i] == '(') nivelParenteses--;

                if (nivelParenteses == 0 && expr[i] == '&')
                {
                    return AvaliarExpressaoRecursiva(expr.Substring(0, i)) &&
                           AvaliarExpressaoRecursiva(expr.Substring(i + 1));
                }
            }

            // Valor literal
            if (expr == "1") return true;
            if (expr == "0") return false;

            throw new Exception("Expressão inválida: " + expr);
        }

        private void LimparExpressao_Click(object sender, RoutedEventArgs e)
        {
            txtExpressao.Text = "";
            borderResultadoExpressao.Visibility = Visibility.Collapsed;
            borderTabelaVerdade.Visibility = Visibility.Collapsed;
            dataGridTabela.ItemsSource = null;
        }

        private void AnimarControle(UIElement elemento)
        {
            var fade = new DoubleAnimation
            {
                From = 0.0,
                To = 1.0,
                Duration = TimeSpan.FromSeconds(0.3)
            };
            elemento.BeginAnimation(OpacityProperty, fade);
        }

        // Classes para tabelas verdade
        public class TabelaLinhaNOT
        {
            public bool A { get; set; }
            public bool Resultado { get; set; }
        }

        public class TabelaLinha2Entradas
        {
            public bool A { get; set; }
            public bool B { get; set; }
            public bool Resultado { get; set; }
        }

        public class TabelaLinha4Entradas
        {
            public bool A { get; set; }
            public bool B { get; set; }
            public bool C { get; set; }
            public bool D { get; set; }
            public bool Resultado { get; set; }
        }
    }
}