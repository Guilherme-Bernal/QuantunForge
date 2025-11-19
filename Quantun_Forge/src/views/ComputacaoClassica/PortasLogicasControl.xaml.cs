// PortasLogicasControl.xaml.cs
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace Quantun_Forge.src.views
{
    public partial class PortasLogicasControl : UserControl
    {
        private string portaAtual = "";

        public PortasLogicasControl()
        {
            InitializeComponent();
            InicializarEstadoInicial();
        }

        private void InicializarEstadoInicial()
        {
            txtPortaSelecionada.Text = "Selecione uma porta lógica";
            txtResultado.Text = "--";
            txtDescricaoPorta.Text = "Selecione uma porta lógica para ver sua descrição e funcionamento.";
            txtAplicacoes.Text = "As portas lógicas são usadas em processadores, memórias, circuitos aritméticos, multiplexadores e em todos os sistemas digitais.";
        }

        private void Entrada_Changed(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(portaAtual))
            {
                AtualizarResultado();
            }
        }

        // Seletores de portas
        private void SelecionarAND(object sender, RoutedEventArgs e)
        {
            portaAtual = "AND";
            ConfigurarPorta("AND", "A ∧ B",
                "Porta AND: Retorna 1 (TRUE) apenas quando TODAS as entradas são 1. É como uma multiplicação lógica.",
                "Usada em circuitos de controle, sistemas de segurança (precisa de múltiplas condições), máscaras de bits e operações aritméticas.");
            AtualizarResultado();
        }

        private void SelecionarOR(object sender, RoutedEventArgs e)
        {
            portaAtual = "OR";
            ConfigurarPorta("OR", "A ∨ B",
                "Porta OR: Retorna 1 (TRUE) quando PELO MENOS UMA entrada é 1. É como uma adição lógica.",
                "Usada em sistemas de alarme (múltiplos sensores), seleção de sinais, barramentos de dados e circuitos de prioridade.");
            AtualizarResultado();
        }

        private void SelecionarNOT(object sender, RoutedEventArgs e)
        {
            portaAtual = "NOT";
            ConfigurarPorta("NOT", "¬A",
                "Porta NOT (Inversor): Inverte o valor da entrada. Se entrada é 0, saída é 1, e vice-versa.",
                "Usada para inversão de sinais, criação de sinais complementares, circuitos de clock e negação lógica.");
            AtualizarResultado();
        }

        private void SelecionarXOR(object sender, RoutedEventArgs e)
        {
            portaAtual = "XOR";
            ConfigurarPorta("XOR", "A ⊕ B",
                "Porta XOR (OU Exclusivo): Retorna 1 quando as entradas são DIFERENTES. É a base para comparações.",
                "Usada em somadores binários, detecção de paridade, cifragem, comparadores e circuitos aritméticos.");
            AtualizarResultado();
        }

        private void SelecionarNAND(object sender, RoutedEventArgs e)
        {
            portaAtual = "NAND";
            ConfigurarPorta("NAND", "¬(A ∧ B)",
                "Porta NAND: É o inverso da porta AND. Retorna 0 apenas quando todas as entradas são 1. É uma porta UNIVERSAL!",
                "Porta universal - pode implementar qualquer função lógica. Usada em memórias Flash, flip-flops e circuitos integrados.");
            AtualizarResultado();
        }

        private void SelecionarNOR(object sender, RoutedEventArgs e)
        {
            portaAtual = "NOR";
            ConfigurarPorta("NOR", "¬(A ∨ B)",
                "Porta NOR: É o inverso da porta OR. Retorna 1 apenas quando todas as entradas são 0. Também é uma porta UNIVERSAL!",
                "Porta universal alternativa. Usada em memórias, decodificadores, circuitos de detecção de zero e flip-flops.");
            AtualizarResultado();
        }

        private void SelecionarXNOR(object sender, RoutedEventArgs e)
        {
            portaAtual = "XNOR";
            ConfigurarPorta("XNOR", "¬(A ⊕ B)",
                "Porta XNOR (Coincidência): Retorna 1 quando as entradas são IGUAIS. É o inverso da XOR.",
                "Usada em comparadores de igualdade, detectores de erro, circuitos de verificação e sistemas de validação.");
            AtualizarResultado();
        }

        private void SelecionarBUFFER(object sender, RoutedEventArgs e)
        {
            portaAtual = "BUFFER";
            ConfigurarPorta("BUFFER", "A",
                "Porta BUFFER: Apenas repete a entrada na saída. Usada para amplificação de sinal e isolamento.",
                "Usada para aumentar corrente de saída, isolamento de circuitos, sincronização de sinais e drivers de linha.");
            AtualizarResultado();
        }

        private void ConfigurarPorta(string nome, string simbolo, string descricao, string aplicacoes)
        {
            txtPortaSelecionada.Text = $"{nome} ({simbolo})";
            txtDescricaoPorta.Text = descricao;
            txtAplicacoes.Text = aplicacoes;
        }

        private void AtualizarResultado()
        {
            bool a = toggleA.IsChecked == true;
            bool b = toggleB.IsChecked == true;

            bool resultado = portaAtual switch
            {
                "AND" => a && b,
                "OR" => a || b,
                "NOT" => !a,
                "XOR" => a ^ b,
                "NAND" => !(a && b),
                "NOR" => !(a || b),
                "XNOR" => !(a ^ b),
                "BUFFER" => a,
                _ => false
            };

            // Atualiza texto e cor do resultado
            txtResultado.Text = resultado ? "1" : "0";
            txtResultado.Foreground = resultado ?
                new SolidColorBrush(Color.FromRgb(39, 174, 96)) :  // Verde
                new SolidColorBrush(Color.FromRgb(231, 76, 60));   // Vermelho

            // Atualiza descrição
            string entradaA = a ? "1" : "0";
            string entradaB = b ? "1" : "0";
            string resultadoStr = resultado ? "1 (TRUE)" : "0 (FALSE)";

            if (portaAtual == "NOT" || portaAtual == "BUFFER")
            {
                txtResultadoDescricao.Text = $"Entrada: A={entradaA} → Saída: {resultadoStr}";
            }
            else
            {
                txtResultadoDescricao.Text = $"Entradas: A={entradaA}, B={entradaB} → Saída: {resultadoStr}";
            }

            AnimarControle(txtResultado);
        }

        private void MostrarTabela_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(portaAtual))
            {
                MessageBox.Show("Selecione uma porta lógica primeiro!", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            GerarTabelaVerdade();
        }

        private void GerarTabelaVerdade()
        {
            // Para portas com 1 entrada (NOT e BUFFER)
            if (portaAtual == "NOT" || portaAtual == "BUFFER")
            {
                var tabela = new List<TabelaVerdade1Entrada>();

                for (int i = 0; i <= 1; i++)
                {
                    bool a = i == 1;
                    bool resultado = portaAtual switch
                    {
                        "NOT" => !a,
                        "BUFFER" => a,
                        _ => false
                    };

                    tabela.Add(new TabelaVerdade1Entrada
                    {
                        A = a,
                        Resultado = resultado
                    });
                }

                dataGridTabela.ItemsSource = tabela;
            }
            // Para portas com 2 entradas
            else
            {
                var tabela = new List<TabelaVerdade2Entradas>();

                for (int i = 0; i < 4; i++)
                {
                    bool a = (i & 2) != 0;
                    bool b = (i & 1) != 0;

                    bool resultado = portaAtual switch
                    {
                        "AND" => a && b,
                        "OR" => a || b,
                        "XOR" => a ^ b,
                        "NAND" => !(a && b),
                        "NOR" => !(a || b),
                        "XNOR" => !(a ^ b),
                        _ => false
                    };

                    tabela.Add(new TabelaVerdade2Entradas
                    {
                        A = a,
                        B = b,
                        Resultado = resultado
                    });
                }

                dataGridTabela.ItemsSource = tabela;
            }

            txtTituloTabela.Text = $"- Porta {portaAtual}";
            borderTabelaVerdade.Visibility = Visibility.Visible;
            AnimarControle(borderTabelaVerdade);

            // Scroll até a tabela
            borderTabelaVerdade.BringIntoView();
        }

        private void AnimarControle(UIElement elemento)
        {
            var fade = new DoubleAnimation
            {
                From = 0.5,
                To = 1.0,
                Duration = TimeSpan.FromSeconds(0.3)
            };
            elemento.BeginAnimation(OpacityProperty, fade);
        }

        // Classes para tabelas verdade
        public class TabelaVerdade1Entrada
        {
            public bool A { get; set; }
            public bool Resultado { get; set; }
        }

        public class TabelaVerdade2Entradas
        {
            public bool A { get; set; }
            public bool B { get; set; }
            public bool Resultado { get; set; }
        }
    }
}