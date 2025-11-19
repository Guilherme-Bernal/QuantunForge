using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using Quantun_Forge.src.views.ModuloEnsino.Computacao.ComputacaoClassica.Testes;

namespace Quantun_Forge.src.views.ModuloEnsino.Computacao.ComputacaoClassica
{
    public partial class Turing : Window
    {
        // Classe para representar uma transição da Máquina de Turing
        private class Transition
        {
            public string CurrentState { get; set; }
            public char ReadSymbol { get; set; }
            public char WriteSymbol { get; set; }
            public string MoveDirection { get; set; }
            public string NextState { get; set; }
        }

        // Classe para representar um exemplo de Máquina de Turing
        private class TuringExample
        {
            public string Name { get; set; }
            public string Description { get; set; }
            public List<char> InitialTape { get; set; }
            public List<Transition> Transitions { get; set; }
            public string InitialState { get; set; }
            public List<string> AcceptStates { get; set; }
            public List<string> RejectStates { get; set; }
        }

        private List<char> tape;
        private int headPosition;
        private string currentState;
        private int stepCount;
        private List<Transition> transitions;
        private TuringExample currentExample;
        private bool isRunning;
        private DispatcherTimer runTimer;
        private List<string> acceptStates;
        private List<string> rejectStates;

        public Turing()
        {
            InitializeComponent();
            InitializeSimulator();
            Loaded += Turing_Loaded; // Adiciona evento de carregamento
        }

        private void Turing_Loaded(object sender, RoutedEventArgs e)
        {
            // Carrega o exemplo apenas após a janela estar completamente carregada
            LoadExample(0);
        }

        /// <summary>
        /// Inicializa o simulador
        /// </summary>
        private void InitializeSimulator()
        {
            runTimer = new DispatcherTimer();
            runTimer.Interval = TimeSpan.FromMilliseconds(500);
            runTimer.Tick += RunTimer_Tick;
        }

        /// <summary>
        /// Carrega um exemplo específico
        /// </summary>
        private void LoadExample(int index)
        {
            switch (index)
            {
                case 0: // Verificar número binário par
                    currentExample = new TuringExample
                    {
                        Name = "Verificar número binário par",
                        Description = "Verifica se um número binário é par (termina em 0)",
                        InitialTape = new List<char> { '1', '0', '1', '0' },
                        InitialState = "q0",
                        AcceptStates = new List<string> { "qAccept" },
                        RejectStates = new List<string> { "qReject" },
                        Transitions = new List<Transition>
                        {
                            new Transition { CurrentState = "q0", ReadSymbol = '0', WriteSymbol = '0', MoveDirection = "R", NextState = "q0" },
                            new Transition { CurrentState = "q0", ReadSymbol = '1', WriteSymbol = '1', MoveDirection = "R", NextState = "q0" },
                            new Transition { CurrentState = "q0", ReadSymbol = '_', WriteSymbol = '_', MoveDirection = "L", NextState = "q1" },
                            new Transition { CurrentState = "q1", ReadSymbol = '0', WriteSymbol = '0', MoveDirection = "R", NextState = "qAccept" },
                            new Transition { CurrentState = "q1", ReadSymbol = '1', WriteSymbol = '1', MoveDirection = "R", NextState = "qReject" }
                        }
                    };
                    break;

                case 1: // Inverter bits (NOT)
                    currentExample = new TuringExample
                    {
                        Name = "Inverter bits (NOT)",
                        Description = "Inverte todos os bits: 0 vira 1 e 1 vira 0",
                        InitialTape = new List<char> { '1', '0', '1', '1' },
                        InitialState = "q0",
                        AcceptStates = new List<string> { "qAccept" },
                        RejectStates = new List<string> { },
                        Transitions = new List<Transition>
                        {
                            new Transition { CurrentState = "q0", ReadSymbol = '0', WriteSymbol = '1', MoveDirection = "R", NextState = "q0" },
                            new Transition { CurrentState = "q0", ReadSymbol = '1', WriteSymbol = '0', MoveDirection = "R", NextState = "q0" },
                            new Transition { CurrentState = "q0", ReadSymbol = '_', WriteSymbol = '_', MoveDirection = "L", NextState = "qAccept" }
                        }
                    };
                    break;

                case 2: // Adicionar 1
                    currentExample = new TuringExample
                    {
                        Name = "Adicionar 1 ao número binário",
                        Description = "Incrementa um número binário em 1",
                        InitialTape = new List<char> { '1', '1', '1' },
                        InitialState = "q0",
                        AcceptStates = new List<string> { "qAccept" },
                        RejectStates = new List<string> { },
                        Transitions = new List<Transition>
                        {
                            new Transition { CurrentState = "q0", ReadSymbol = '0', WriteSymbol = '0', MoveDirection = "R", NextState = "q0" },
                            new Transition { CurrentState = "q0", ReadSymbol = '1', WriteSymbol = '1', MoveDirection = "R", NextState = "q0" },
                            new Transition { CurrentState = "q0", ReadSymbol = '_', WriteSymbol = '_', MoveDirection = "L", NextState = "q1" },
                            new Transition { CurrentState = "q1", ReadSymbol = '0', WriteSymbol = '1', MoveDirection = "L", NextState = "qAccept" },
                            new Transition { CurrentState = "q1", ReadSymbol = '1', WriteSymbol = '0', MoveDirection = "L", NextState = "q1" },
                            new Transition { CurrentState = "q1", ReadSymbol = '_', WriteSymbol = '1', MoveDirection = "R", NextState = "qAccept" }
                        }
                    };
                    break;

                case 3: // Reconhecer palíndromo
                    currentExample = new TuringExample
                    {
                        Name = "Reconhecer palíndromo",
                        Description = "Verifica se uma sequência é um palíndromo (lê igual de trás para frente)",
                        InitialTape = new List<char> { '1', '0', '1' },
                        InitialState = "q0",
                        AcceptStates = new List<string> { "qAccept" },
                        RejectStates = new List<string> { "qReject" },
                        Transitions = new List<Transition>
                        {
                            new Transition { CurrentState = "q0", ReadSymbol = '0', WriteSymbol = 'X', MoveDirection = "R", NextState = "q1_0" },
                            new Transition { CurrentState = "q0", ReadSymbol = '1', WriteSymbol = 'X', MoveDirection = "R", NextState = "q1_1" },
                            new Transition { CurrentState = "q0", ReadSymbol = 'X', WriteSymbol = 'X', MoveDirection = "R", NextState = "qAccept" },
                            new Transition { CurrentState = "q1_0", ReadSymbol = '0', WriteSymbol = '0', MoveDirection = "R", NextState = "q1_0" },
                            new Transition { CurrentState = "q1_0", ReadSymbol = '1', WriteSymbol = '1', MoveDirection = "R", NextState = "q1_0" },
                            new Transition { CurrentState = "q1_0", ReadSymbol = 'X', WriteSymbol = 'X', MoveDirection = "L", NextState = "q2_0" },
                            new Transition { CurrentState = "q2_0", ReadSymbol = '0', WriteSymbol = 'X', MoveDirection = "L", NextState = "q3" },
                            new Transition { CurrentState = "q2_0", ReadSymbol = '1', WriteSymbol = '1', MoveDirection = "R", NextState = "qReject" },
                            new Transition { CurrentState = "q1_1", ReadSymbol = '0', WriteSymbol = '0', MoveDirection = "R", NextState = "q1_1" },
                            new Transition { CurrentState = "q1_1", ReadSymbol = '1', WriteSymbol = '1', MoveDirection = "R", NextState = "q1_1" },
                            new Transition { CurrentState = "q1_1", ReadSymbol = 'X', WriteSymbol = 'X', MoveDirection = "L", NextState = "q2_1" },
                            new Transition { CurrentState = "q2_1", ReadSymbol = '1', WriteSymbol = 'X', MoveDirection = "L", NextState = "q3" },
                            new Transition { CurrentState = "q2_1", ReadSymbol = '0', WriteSymbol = '0', MoveDirection = "R", NextState = "qReject" },
                            new Transition { CurrentState = "q3", ReadSymbol = '0', WriteSymbol = '0', MoveDirection = "L", NextState = "q3" },
                            new Transition { CurrentState = "q3", ReadSymbol = '1', WriteSymbol = '1', MoveDirection = "L", NextState = "q3" },
                            new Transition { CurrentState = "q3", ReadSymbol = 'X', WriteSymbol = 'X', MoveDirection = "R", NextState = "q0" }
                        }
                    };
                    break;
            }

            // Verifica se o controle está inicializado antes de usar
            if (ExampleDescription != null)
            {
                ExampleDescription.Text = currentExample.Description;
            }

            ResetSimulator();
        }

        private void ResetSimulator()
        {
            tape = new List<char> { '_', '_', '_' };
            tape.AddRange(currentExample.InitialTape);
            tape.AddRange(new List<char> { '_', '_', '_' });

            headPosition = 3;
            currentState = currentExample.InitialState;
            stepCount = 0;
            transitions = currentExample.Transitions;
            acceptStates = currentExample.AcceptStates;
            rejectStates = currentExample.RejectStates;
            isRunning = false;

            UpdateDisplay();

            if (StatusPanel != null) StatusPanel.Visibility = Visibility.Collapsed;
            if (ActionDescription != null) ActionDescription.Text = "Aguardando início da execução...";
            if (StepButton != null) StepButton.IsEnabled = true;
            if (RunButton != null) RunButton.IsEnabled = true;
            if (StopButton != null) StopButton.IsEnabled = false;
        }

        private void UpdateDisplay()
        {
            if (TapePanel == null) return;

            TapePanel.Children.Clear();

            for (int i = 0; i < tape.Count; i++)
            {
                Border cell = new Border
                {
                    Style = (Style)FindResource("TapeCell")
                };

                if (i == headPosition)
                {
                    cell.Background = new SolidColorBrush(Color.FromRgb(255, 243, 224));
                    cell.BorderBrush = new SolidColorBrush(Color.FromRgb(230, 126, 34));
                    cell.BorderThickness = new Thickness(3);
                }

                TextBlock text = new TextBlock
                {
                    Text = tape[i].ToString(),
                    FontSize = 20,
                    FontWeight = FontWeights.Bold,
                    FontFamily = new FontFamily("Consolas"),
                    Foreground = tape[i] == '_' ? new SolidColorBrush(Color.FromRgb(189, 195, 199)) : new SolidColorBrush(Color.FromRgb(44, 62, 80)),
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center
                };

                cell.Child = text;
                TapePanel.Children.Add(cell);
            }

            if (CurrentState != null) CurrentState.Text = currentState;
            if (CurrentSymbol != null) CurrentSymbol.Text = tape[headPosition].ToString();
            if (StepCounter != null) StepCounter.Text = stepCount.ToString();
        }

        private bool ExecuteStep()
        {
            if (acceptStates.Contains(currentState))
            {
                ShowStatus(true, "ACEITO! Entrada válida.");
                return false;
            }

            if (rejectStates.Contains(currentState))
            {
                ShowStatus(false, "REJEITADO! Entrada inválida.");
                return false;
            }

            char currentSymbol = tape[headPosition];
            var transition = transitions.FirstOrDefault(t =>
                t.CurrentState == currentState && t.ReadSymbol == currentSymbol);

            if (transition == null)
            {
                ShowStatus(false, "Sem transição definida. Máquina parou.");
                return false;
            }

            tape[headPosition] = transition.WriteSymbol;

            if (transition.MoveDirection == "L")
            {
                headPosition--;
                if (headPosition < 0)
                {
                    tape.Insert(0, '_');
                    headPosition = 0;
                }
            }
            else
            {
                headPosition++;
                if (headPosition >= tape.Count)
                {
                    tape.Add('_');
                }
            }

            string previousState = currentState;
            currentState = transition.NextState;
            stepCount++;

            if (ActionDescription != null)
            {
                ActionDescription.Text = $"Estado {previousState} → {currentState}\n" +
                                       $"Escreveu '{transition.WriteSymbol}' e moveu para " +
                                       $"{(transition.MoveDirection == "L" ? "ESQUERDA" : "DIREITA")}";
            }

            UpdateDisplay();
            return true;
        }

        private void ShowStatus(bool accepted, string message)
        {
            if (StatusPanel == null) return;

            StatusPanel.Visibility = Visibility.Visible;

            if (accepted)
            {
                StatusPanel.Background = new SolidColorBrush(Color.FromRgb(213, 244, 230));
                StatusPanel.BorderBrush = new SolidColorBrush(Color.FromRgb(39, 174, 96));
                if (StatusIcon != null) StatusIcon.Text = "✓";
                if (StatusIcon != null) StatusIcon.Foreground = new SolidColorBrush(Color.FromRgb(39, 174, 96));
                if (StatusMessage != null) StatusMessage.Text = message;
                if (StatusMessage != null) StatusMessage.Foreground = new SolidColorBrush(Color.FromRgb(39, 174, 96));
            }
            else
            {
                StatusPanel.Background = new SolidColorBrush(Color.FromRgb(250, 219, 216));
                StatusPanel.BorderBrush = new SolidColorBrush(Color.FromRgb(231, 76, 60));
                if (StatusIcon != null) StatusIcon.Text = "✗";
                if (StatusIcon != null) StatusIcon.Foreground = new SolidColorBrush(Color.FromRgb(231, 76, 60));
                if (StatusMessage != null) StatusMessage.Text = message;
                if (StatusMessage != null) StatusMessage.Foreground = new SolidColorBrush(Color.FromRgb(231, 76, 60));
            }

            if (ActionDescription != null) ActionDescription.Text = message;
            if (StepButton != null) StepButton.IsEnabled = false;
            if (RunButton != null) RunButton.IsEnabled = false;
        }

        private void RunTimer_Tick(object sender, EventArgs e)
        {
            if (!ExecuteStep())
            {
                runTimer.Stop();
                isRunning = false;
                if (StopButton != null) StopButton.IsEnabled = false;
                if (RunButton != null) RunButton.Content = "▶▶ EXECUTAR";
            }
        }

        private void StepButton_Click(object sender, RoutedEventArgs e)
        {
            ExecuteStep();
        }

        private void RunButton_Click(object sender, RoutedEventArgs e)
        {
            if (isRunning)
            {
                runTimer.Stop();
                isRunning = false;
                StopButton.IsEnabled = false;
                RunButton.Content = "▶▶ EXECUTAR";
            }
            else
            {
                runTimer.Start();
                isRunning = true;
                StopButton.IsEnabled = true;
                RunButton.Content = "⏸ PAUSAR";
                StepButton.IsEnabled = false;
            }
        }

        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            runTimer.Stop();
            isRunning = false;
            StopButton.IsEnabled = false;
            RunButton.Content = "▶▶ EXECUTAR";
            StepButton.IsEnabled = true;
        }

        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            if (isRunning)
            {
                runTimer.Stop();
                isRunning = false;
            }
            ResetSimulator();
            if (RunButton != null) RunButton.Content = "▶▶ EXECUTAR";
        }

        private void ExampleComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ExampleComboBox.SelectedIndex >= 0)
            {
                if (isRunning)
                {
                    runTimer.Stop();
                    isRunning = false;
                }
                LoadExample(ExampleComboBox.SelectedIndex);
                if (RunButton != null) RunButton.Content = "▶▶ EXECUTAR";
            }
        }

        private void btnExePratica_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var testeWindow = new TuringTeste();
                testeWindow.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Erro ao abrir exercícios: {ex.Message}",
                    "Erro",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );
            }
        }
    }
}