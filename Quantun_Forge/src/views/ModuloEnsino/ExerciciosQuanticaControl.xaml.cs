using System;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Quantun_Forge.src.views.ModuloEnsino
{
    /// <summary>
    /// Interaction logic for ExerciciosQuanticaControl.xaml
    /// </summary>
    public partial class ExerciciosQuanticaControl : Page
    {
        public ExerciciosQuanticaControl()
        {
            InitializeComponent();
            ConfigurarAcessibilidade();
        }

        /// <summary>
        /// Configura propriedades de acessibilidade para os cards
        /// </summary>
        private void ConfigurarAcessibilidade()
        {
            // Configurar TabIndex e acessibilidade para cada card
            QubitCard.Focusable = true;
          
            AutomationProperties.SetName(QubitCard, "Qubit: A Unidade Fundamental - A base da informação quântica");

            EmaranhamentoCard.Focusable = true;
    
            AutomationProperties.SetName(EmaranhamentoCard, "Emaranhamento: A Conexão Fantasmagórica");

            PortasQuanticasCard.Focusable = true;
          
            AutomationProperties.SetName(PortasQuanticasCard, "Portas Lógicas Quânticas: A Manipulação de Qubits");

            AlgoritmosQuanticosCard.Focusable = true;
           
            AutomationProperties.SetName(AlgoritmosQuanticosCard, "Algoritmos Quânticos: As Novas Receitas");

            MedicaoCard.Focusable = true;
           
            AutomationProperties.SetName(MedicaoCard, "Medição: Extraindo a Resposta Clássica");

            DecoerenciaCard.Focusable = true;
       
            AutomationProperties.SetName(DecoerenciaCard, "Decoerência: O Inimigo da Computação Quântica");

            CriptografiaQuanticaCard.Focusable = true;
            
            AutomationProperties.SetName(CriptografiaQuanticaCard, "Criptografia Quântica");

            // Adicionar suporte para navegação por teclado
            AdicionarSuporteNavegacaoTeclado(QubitCard, NavegarParaQubit);
            AdicionarSuporteNavegacaoTeclado(EmaranhamentoCard, NavegarParaEmaranhamento);
            AdicionarSuporteNavegacaoTeclado(PortasQuanticasCard, NavegarParaPortasQuanticas);
            AdicionarSuporteNavegacaoTeclado(AlgoritmosQuanticosCard, NavegarParaAlgoritmosQuanticos);
            AdicionarSuporteNavegacaoTeclado(MedicaoCard, NavegarParaMedicao);
            AdicionarSuporteNavegacaoTeclado(DecoerenciaCard, NavegarParaDecoerencia);
            AdicionarSuporteNavegacaoTeclado(CriptografiaQuanticaCard, NavegarParaCriptografiaQuantica);
        }

        /// <summary>
        /// Adiciona suporte para navegação por teclado (Enter e Space)
        /// </summary>
        private void AdicionarSuporteNavegacaoTeclado(UIElement elemento, Action acao)
        {
            elemento.KeyDown += (sender, e) =>
            {
                if (e.Key == Key.Enter || e.Key == Key.Space)
                {
                    acao();
                    e.Handled = true;
                }
            };

            // Adicionar feedback visual quando focado
            elemento.GotFocus += (sender, e) =>
            {
                if (sender is Border border)
                {
                    border.BorderBrush = new SolidColorBrush(Color.FromRgb(199, 125, 255)); // #C77DFF
                    border.BorderThickness = new Thickness(3);
                }
            };

            elemento.LostFocus += (sender, e) =>
            {
                if (sender is Border border)
                {
                    border.BorderBrush = new SolidColorBrush(Color.FromRgb(157, 78, 221)); // #9D4EDD
                    border.BorderThickness = new Thickness(2);
                }
            };
        }

        #region Event Handlers para Cliques

        private void QubitCard_Click(object sender, MouseButtonEventArgs e)
        {
            NavegarParaQubit();
        }

        private void EmaranhamentoCard_Click(object sender, MouseButtonEventArgs e)
        {
            NavegarParaEmaranhamento();
        }

        private void PortasQuanticasCard_Click(object sender, MouseButtonEventArgs e)
        {
            NavegarParaPortasQuanticas();
        }

        private void AlgoritmosQuanticosCard_Click(object sender, MouseButtonEventArgs e)
        {
            NavegarParaAlgoritmosQuanticos();
        }

        private void MedicaoCard_Click(object sender, MouseButtonEventArgs e)
        {
            NavegarParaMedicao();
        }

        private void DecoerenciaCard_Click(object sender, MouseButtonEventArgs e)
        {
            NavegarParaDecoerencia();
        }

        private void CriptografiaQuanticaCard_Click(object sender, MouseButtonEventArgs e)
        {
            NavegarParaCriptografiaQuantica();
        }

        #endregion

        #region Métodos de Navegação

        /// <summary>
        /// Navega para o módulo de Qubit
        /// </summary>
        private void NavegarParaQubit()
        {
            try
            {
                var paginaConteudo = CriarPaginaConteudo(
                    "Qubit: A Unidade Fundamental",
                    "A base da informação quântica e superposição de estados",
                    "Neste módulo você aprenderá sobre:\n\n" +
                    "• O que é um qubit e como difere de um bit clássico\n" +
                    "• Superposição quântica: |0⟩, |1⟩ e estados intermediários\n" +
                    "• Representação na Esfera de Bloch\n" +
                    "• Notação de Dirac (bra-ket)\n" +
                    "• Propriedades fundamentais dos qubits\n" +
                    "• Implementações físicas de qubits"
                );

                NavigationService?.Navigate(paginaConteudo);
            }
            catch (Exception ex)
            {
                MostrarMensagemErro("Qubit", ex.Message);
            }
        }

        /// <summary>
        /// Navega para o módulo de Emaranhamento
        /// </summary>
        private void NavegarParaEmaranhamento()
        {
            try
            {
                var paginaConteudo = CriarPaginaConteudo(
                    "Emaranhamento: A Conexão 'Fantasmagórica'",
                    "A correlação quântica que desafia a física clássica",
                    "Neste módulo você aprenderá sobre:\n\n" +
                    "• O fenômeno do emaranhamento quântico\n" +
                    "• Estados de Bell e pares EPR\n" +
                    "• Não-localidade e o paradoxo EPR\n" +
                    "• Desigualdades de Bell\n" +
                    "• Aplicações do emaranhamento\n" +
                    "• Teletransporte quântico"
                );

                NavigationService?.Navigate(paginaConteudo);
            }
            catch (Exception ex)
            {
                MostrarMensagemErro("Emaranhamento", ex.Message);
            }
        }

        /// <summary>
        /// Navega para o módulo de Portas Lógicas Quânticas
        /// </summary>
        private void NavegarParaPortasQuanticas()
        {
            try
            {
                var paginaConteudo = CriarPaginaConteudo(
                    "Portas Lógicas Quânticas",
                    "Operações unitárias que transformam estados quânticos",
                    "Neste módulo você aprenderá sobre:\n\n" +
                    "• Portas quânticas de um qubit (X, Y, Z, H, T, S)\n" +
                    "• Porta Hadamard e criação de superposição\n" +
                    "• Portas de rotação (Rx, Ry, Rz)\n" +
                    "• Portas de múltiplos qubits (CNOT, CCNOT/Toffoli)\n" +
                    "• Universalidade quântica\n" +
                    "• Circuitos quânticos"
                );

                NavigationService?.Navigate(paginaConteudo);
            }
            catch (Exception ex)
            {
                MostrarMensagemErro("Portas Lógicas Quânticas", ex.Message);
            }
        }

        /// <summary>
        /// Navega para o módulo de Algoritmos Quânticos
        /// </summary>
        private void NavegarParaAlgoritmosQuanticos()
        {
            try
            {
                var paginaConteudo = CriarPaginaConteudo(
                    "Algoritmos Quânticos: As Novas 'Receitas'",
                    "Shor, Grover e outros algoritmos revolucionários",
                    "Neste módulo você aprenderá sobre:\n\n" +
                    "• Algoritmo de Deutsch-Jozsa\n" +
                    "• Algoritmo de Grover (busca quântica)\n" +
                    "• Algoritmo de Shor (fatoração)\n" +
                    "• Transformada de Fourier Quântica (QFT)\n" +
                    "• Algoritmo VQE (Variational Quantum Eigensolver)\n" +
                    "• Vantagem quântica e supremacia quântica"
                );

                NavigationService?.Navigate(paginaConteudo);
            }
            catch (Exception ex)
            {
                MostrarMensagemErro("Algoritmos Quânticos", ex.Message);
            }
        }

        /// <summary>
        /// Navega para o módulo de Medição
        /// </summary>
        private void NavegarParaMedicao()
        {
            try
            {
                var paginaConteudo = CriarPaginaConteudo(
                    "Medição: Extraindo a Resposta Clássica",
                    "O colapso da função de onda e extração de resultados",
                    "Neste módulo você aprenderá sobre:\n\n" +
                    "• O postulado da medição quântica\n" +
                    "• Colapso da função de onda\n" +
                    "• Bases de medição (computacional, Hadamard, etc.)\n" +
                    "• Probabilidades de medição\n" +
                    "• Medição projetiva vs. POVM\n" +
                    "• Efeito da medição nos estados quânticos"
                );

                NavigationService?.Navigate(paginaConteudo);
            }
            catch (Exception ex)
            {
                MostrarMensagemErro("Medição", ex.Message);
            }
        }

        /// <summary>
        /// Navega para o módulo de Decoerência
        /// </summary>
        private void NavegarParaDecoerencia()
        {
            try
            {
                var paginaConteudo = CriarPaginaConteudo(
                    "Decoerência: O Inimigo da Computação Quântica",
                    "O desafio da perda de informação quântica",
                    "Neste módulo você aprenderá sobre:\n\n" +
                    "• O que é decoerência quântica\n" +
                    "• Interação com o ambiente\n" +
                    "• Tempo de coerência (T1 e T2)\n" +
                    "• Códigos de correção de erros quânticos\n" +
                    "• Técnicas de mitigação de erros\n" +
                    "• Computação quântica tolerante a falhas"
                );

                NavigationService?.Navigate(paginaConteudo);
            }
            catch (Exception ex)
            {
                MostrarMensagemErro("Decoerência", ex.Message);
            }
        }

        /// <summary>
        /// Navega para o módulo de Criptografia Quântica
        /// </summary>
        private void NavegarParaCriptografiaQuantica()
        {
            try
            {
                var paginaConteudo = CriarPaginaConteudo(
                    "Criptografia Quântica",
                    "Segurança fundamentada nas leis da física quântica",
                    "Neste módulo você aprenderá sobre:\n\n" +
                    "• Distribuição de chaves quânticas (QKD)\n" +
                    "• Protocolo BB84\n" +
                    "• Protocolo E91 baseado em emaranhamento\n" +
                    "• Segurança teórica da informação\n" +
                    "• Ataques e contramedidas\n" +
                    "• Redes quânticas e internet quântica"
                );

                NavigationService?.Navigate(paginaConteudo);
            }
            catch (Exception ex)
            {
                MostrarMensagemErro("Criptografia Quântica", ex.Message);
            }
        }

        #endregion

        #region Métodos Auxiliares

        /// <summary>
        /// Cria uma página de conteúdo genérica para exibir informações do módulo
        /// </summary>
        private Page CriarPaginaConteudo(string titulo, string subtitulo, string conteudo)
        {
            var page = new Page
            {
                Background = new SolidColorBrush(Color.FromRgb(15, 15, 35))
            };

            var scrollViewer = new ScrollViewer
            {
                VerticalScrollBarVisibility = ScrollBarVisibility.Auto
            };

            var grid = new Grid
            {
                Margin = new Thickness(40)
            };

            var stackPanel = new StackPanel();

            // Botão Voltar
            var btnVoltar = new Button
            {
                Content = "← Voltar",
                Width = 100,
                Height = 35,
                HorizontalAlignment = HorizontalAlignment.Left,
                Margin = new Thickness(0, 0, 0, 20),
                Background = new SolidColorBrush(Color.FromRgb(157, 78, 221)),
                Foreground = Brushes.White,
                BorderThickness = new Thickness(0),
                Cursor = Cursors.Hand,
                FontSize = 14,
                FontWeight = FontWeights.Bold
            };
            btnVoltar.Click += (s, e) => NavigationService?.GoBack();

            // Título
            var txtTitulo = new TextBlock
            {
                Text = titulo,
                FontSize = 32,
                FontWeight = FontWeights.Bold,
                Foreground = new SolidColorBrush(Color.FromRgb(157, 78, 221)),
                Margin = new Thickness(0, 0, 0, 10),
                TextWrapping = TextWrapping.Wrap
            };

            // Subtítulo
            var txtSubtitulo = new TextBlock
            {
                Text = subtitulo,
                FontSize = 16,
                Foreground = new SolidColorBrush(Color.FromRgb(184, 184, 209)),
                Margin = new Thickness(0, 0, 0, 30),
                TextWrapping = TextWrapping.Wrap
            };

            // Conteúdo
            var txtConteudo = new TextBlock
            {
                Text = conteudo,
                FontSize = 14,
                Foreground = Brushes.White,
                TextWrapping = TextWrapping.Wrap,
                LineHeight = 22
            };

            stackPanel.Children.Add(btnVoltar);
            stackPanel.Children.Add(txtTitulo);
            stackPanel.Children.Add(txtSubtitulo);
            stackPanel.Children.Add(txtConteudo);

            grid.Children.Add(stackPanel);
            scrollViewer.Content = grid;
            page.Content = scrollViewer;

            return page;
        }

        /// <summary>
        /// Exibe mensagem de erro quando a navegação falha
        /// </summary>
        private void MostrarMensagemErro(string modulo, string erro)
        {
            MessageBox.Show(
                $"Não foi possível carregar o módulo '{modulo}'.\n\nErro: {erro}",
                "Erro de Navegação",
                MessageBoxButton.OK,
                MessageBoxImage.Error
            );
        }

        #endregion
    }
}