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
    /// <summary>
    /// Lógica interna para SistemaOperacional.xaml
    /// </summary>
    public partial class SistemaOperacional : Window
    {
        // Classe para representar um processo
        private class Process
        {
            public string Name { get; set; }
            public int ExecutionTime { get; set; }
            public int RemainingTime { get; set; }
            public int Priority { get; set; }
            public int ArrivalTime { get; set; }
            public ProcessState State { get; set; }
            public Border UIElement { get; set; }

            public Process(string name, int executionTime, int priority, int arrivalTime)
            {
                Name = name;
                ExecutionTime = executionTime;
                RemainingTime = executionTime;
                Priority = priority;
                ArrivalTime = arrivalTime;
                State = ProcessState.Ready;
            }
        }

        private enum ProcessState
        {
            Ready,
            Running,
            Waiting,
            Terminated
        }

        private enum SchedulingAlgorithm
        {
            FCFS,
            SJF,
            RoundRobin,
            Priority
        }

        // Variáveis do simulador
        private List<Process> processList;
        private Queue<Process> readyQueue;
        private Process currentProcess;
        private SchedulingAlgorithm currentAlgorithm;
        private DispatcherTimer simulationTimer;
        private int elapsedTime;
        private int processCounter;
        private const int TIME_QUANTUM = 50; // Para Round Robin
        private int currentQuantum;

        public SistemaOperacional()
        {
            InitializeComponent();
            InitializeSimulator();
        }

        private void InitializeSimulator()
        {
            processList = new List<Process>();
            readyQueue = new Queue<Process>();
            currentProcess = null;
            currentAlgorithm = SchedulingAlgorithm.FCFS;
            elapsedTime = 0;
            processCounter = 1;
            currentQuantum = 0;

            // Configurar timer
            simulationTimer = new DispatcherTimer();
            simulationTimer.Interval = TimeSpan.FromMilliseconds(10);
            simulationTimer.Tick += SimulationTimer_Tick;

            UpdateStatus();
        }

        private void SchedulingComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SchedulingComboBox == null || SchedulingComboBox.SelectedIndex < 0) return;

            switch (SchedulingComboBox.SelectedIndex)
            {
                case 0: // FCFS
                    currentAlgorithm = SchedulingAlgorithm.FCFS;
                    if (AlgorithmDescription != null)
                        AlgorithmDescription.Text = "FCFS: Os processos são executados na ordem de chegada, sem interrupção. Simples, mas pode causar espera longa para processos curtos.";
                    break;
                case 1: // SJF
                    currentAlgorithm = SchedulingAlgorithm.SJF;
                    if (AlgorithmDescription != null)
                        AlgorithmDescription.Text = "SJF: O processo com menor tempo de execução é executado primeiro. Minimiza tempo médio de espera, mas pode causar starvation.";
                    break;
                case 2: // Round Robin
                    currentAlgorithm = SchedulingAlgorithm.RoundRobin;
                    if (AlgorithmDescription != null)
                        AlgorithmDescription.Text = $"Round Robin: Cada processo recebe uma fatia de tempo ({TIME_QUANTUM}ms). Garante justiça e responsividade, ideal para sistemas interativos.";
                    break;
                case 3: // Priority
                    currentAlgorithm = SchedulingAlgorithm.Priority;
                    if (AlgorithmDescription != null)
                        AlgorithmDescription.Text = "Prioridade: Processos com maior prioridade (número menor) executam primeiro. Comum em sistemas de tempo real.";
                    break;
            }
        }

        private void AddProcess_Click(object sender, RoutedEventArgs e)
        {
            // Validar entrada
            if (string.IsNullOrWhiteSpace(ProcessNameTextBox.Text))
            {
                MessageBox.Show("Por favor, insira um nome para o processo.", "Erro",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!int.TryParse(ProcessTimeTextBox.Text, out int executionTime) || executionTime <= 0)
            {
                MessageBox.Show("Por favor, insira um tempo de execução válido (maior que 0).", "Erro",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!int.TryParse(ProcessPriorityTextBox.Text, out int priority) || priority < 1)
            {
                MessageBox.Show("Por favor, insira uma prioridade válida (1 ou maior).", "Erro",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Criar processo
            Process newProcess = new Process(
                ProcessNameTextBox.Text,
                executionTime,
                priority,
                elapsedTime
            );

            // Criar elemento UI para o processo
            Border processUI = CreateProcessUI(newProcess);
            newProcess.UIElement = processUI;

            // Adicionar à lista
            processList.Add(newProcess);

            // Atualizar UI
            if (ProcessListPanel.Children.Count == 1 &&
                ProcessListPanel.Children[0] is Border firstBorder)
            {
                var textBlock = FindVisualChild<TextBlock>(firstBorder);
                if (textBlock != null && textBlock.Text == "Nenhum processo adicionado")
                {
                    ProcessListPanel.Children.Clear();
                }
            }

            ProcessListPanel.Children.Add(processUI);

            // Incrementar contador e limpar campos
            processCounter++;
            ProcessNameTextBox.Text = $"P{processCounter}";
            ProcessTimeTextBox.Text = "100";
            ProcessPriorityTextBox.Text = "1";

            UpdateStatus();
        }

        private Border CreateProcessUI(Process process)
        {
            Border border = new Border
            {
                Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#F8F9FA")),
                BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#BDC3C7")),
                BorderThickness = new Thickness(1),
                CornerRadius = new CornerRadius(6),
                Padding = new Thickness(12),
                Margin = new Thickness(0, 3, 0, 3)
            };

            Grid grid = new Grid();
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

            StackPanel infoPanel = new StackPanel();

            TextBlock nameText = new TextBlock
            {
                Text = $"{process.Name} - {process.ExecutionTime}ms",
                FontSize = 12,
                FontWeight = FontWeights.Bold,
                Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#2C3E50"))
            };

            TextBlock detailsText = new TextBlock
            {
                Text = $"Prioridade: {process.Priority} | Estado: Pronto",
                FontSize = 10,
                Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#7F8C8D")),
                Margin = new Thickness(0, 3, 0, 0)
            };

            infoPanel.Children.Add(nameText);
            infoPanel.Children.Add(detailsText);

            // Criar botão de remover com template inline
            Button removeButton = new Button
            {
                Content = "✕",
                Width = 25,
                Height = 25,
                Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#E74C3C")),
                Foreground = Brushes.White,
                BorderThickness = new Thickness(0),
                Cursor = System.Windows.Input.Cursors.Hand,
                FontWeight = FontWeights.Bold,
                FontSize = 14,
                Tag = process
            };

            // Aplicar template diretamente ao botão
            removeButton.Template = CreateButtonTemplate();
            removeButton.Click += RemoveProcess_Click;

            Grid.SetColumn(infoPanel, 0);
            Grid.SetColumn(removeButton, 1);

            grid.Children.Add(infoPanel);
            grid.Children.Add(removeButton);

            border.Child = grid;

            return border;
        }

        private ControlTemplate CreateButtonTemplate()
        {
            ControlTemplate template = new ControlTemplate(typeof(Button));

            FrameworkElementFactory border = new FrameworkElementFactory(typeof(Border));
            border.SetValue(Border.BackgroundProperty, new TemplateBindingExtension(Button.BackgroundProperty));
            border.SetValue(Border.CornerRadiusProperty, new CornerRadius(3));
            border.SetValue(Border.PaddingProperty, new Thickness(0));

            FrameworkElementFactory contentPresenter = new FrameworkElementFactory(typeof(ContentPresenter));
            contentPresenter.SetValue(ContentPresenter.HorizontalAlignmentProperty, HorizontalAlignment.Center);
            contentPresenter.SetValue(ContentPresenter.VerticalAlignmentProperty, VerticalAlignment.Center);

            border.AppendChild(contentPresenter);
            template.VisualTree = border;

            return template;
        }

        private void RemoveProcess_Click(object sender, RoutedEventArgs e)
        {
            if (simulationTimer != null && simulationTimer.IsEnabled)
            {
                MessageBox.Show("Não é possível remover processos durante a simulação.", "Aviso",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            Button button = sender as Button;
            Process process = button?.Tag as Process;

            if (process == null) return;

            processList.Remove(process);
            ProcessListPanel.Children.Remove(process.UIElement);

            if (ProcessListPanel.Children.Count == 0)
            {
                AddEmptyProcessMessage();
            }

            UpdateStatus();
        }

        private void AddEmptyProcessMessage()
        {
            Border emptyBorder = new Border
            {
                Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#F8F9FA")),
                BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#BDC3C7")),
                BorderThickness = new Thickness(1),
                CornerRadius = new CornerRadius(6),
                Padding = new Thickness(12),
                Margin = new Thickness(0, 3, 0, 3)
            };

            TextBlock emptyText = new TextBlock
            {
                Text = "Nenhum processo adicionado",
                FontSize = 12,
                Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#7F8C8D")),
                FontStyle = FontStyles.Italic
            };

            emptyBorder.Child = emptyText;
            ProcessListPanel.Children.Add(emptyBorder);
        }

        private void StartSimulation_Click(object sender, RoutedEventArgs e)
        {
            if (processList.Count == 0)
            {
                MessageBox.Show("Adicione pelo menos um processo para iniciar a simulação.", "Aviso",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            if (simulationTimer.IsEnabled)
            {
                // Pausar
                simulationTimer.Stop();
                StartButton.Content = "▶ CONTINUAR";
                StatusText.Text = "Simulação pausada";
            }
            else
            {
                // Iniciar ou continuar
                if (readyQueue.Count == 0 && currentProcess == null)
                {
                    // Primeira execução
                    InitializeQueue();
                }

                simulationTimer.Start();
                StartButton.Content = "⏸ PAUSAR";
                StatusText.Text = "Simulação em execução...";

                AddProcessButton.IsEnabled = false;
                SchedulingComboBox.IsEnabled = false;
            }
        }

        private void InitializeQueue()
        {
            readyQueue.Clear();
            elapsedTime = 0;
            currentQuantum = 0;

            // Ordenar processos baseado no algoritmo
            List<Process> orderedProcesses = new List<Process>(processList);

            switch (currentAlgorithm)
            {
                case SchedulingAlgorithm.FCFS:
                    // Já está na ordem de chegada
                    break;
                case SchedulingAlgorithm.SJF:
                    orderedProcesses = orderedProcesses.OrderBy(p => p.ExecutionTime).ToList();
                    break;
                case SchedulingAlgorithm.RoundRobin:
                    // Ordem de chegada
                    break;
                case SchedulingAlgorithm.Priority:
                    orderedProcesses = orderedProcesses.OrderBy(p => p.Priority).ToList();
                    break;
            }

            // Adicionar à fila
            foreach (var process in orderedProcesses)
            {
                process.RemainingTime = process.ExecutionTime;
                process.State = ProcessState.Ready;
                readyQueue.Enqueue(process);
                UpdateProcessUI(process);
            }
        }

        private void SimulationTimer_Tick(object sender, EventArgs e)
        {
            elapsedTime += 10;

            // Se não há processo atual, pegar o próximo da fila
            if (currentProcess == null)
            {
                if (readyQueue.Count > 0)
                {
                    currentProcess = readyQueue.Dequeue();
                    currentProcess.State = ProcessState.Running;
                    currentQuantum = 0;
                    UpdateProcessUI(currentProcess);
                }
                else
                {
                    // Todos os processos terminaram
                    FinishSimulation();
                    return;
                }
            }

            // Executar processo atual
            if (currentProcess != null)
            {
                currentProcess.RemainingTime -= 10;
                currentQuantum += 10;

                // Atualizar UI
                UpdateStatus();

                // Verificar se o processo terminou
                if (currentProcess.RemainingTime <= 0)
                {
                    currentProcess.State = ProcessState.Terminated;
                    UpdateProcessUI(currentProcess);
                    currentProcess = null;
                    currentQuantum = 0;
                }
                // Round Robin: verificar quantum
                else if (currentAlgorithm == SchedulingAlgorithm.RoundRobin &&
                         currentQuantum >= TIME_QUANTUM &&
                         readyQueue.Count > 0)
                {
                    // Retornar à fila
                    currentProcess.State = ProcessState.Ready;
                    UpdateProcessUI(currentProcess);
                    readyQueue.Enqueue(currentProcess);
                    currentProcess = null;
                    currentQuantum = 0;
                }
            }
        }

        private void FinishSimulation()
        {
            simulationTimer.Stop();
            StartButton.Content = "▶ INICIAR";
            StatusText.Text = "Simulação concluída!";
            CurrentProcessText.Text = "Processo atual: Nenhum";

            AddProcessButton.IsEnabled = true;
            SchedulingComboBox.IsEnabled = true;

            MessageBox.Show($"Simulação concluída!\n\nTempo total: {elapsedTime}ms\nProcessos executados: {processList.Count}",
                "Simulação Concluída", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void ResetSimulation_Click(object sender, RoutedEventArgs e)
        {
            if (simulationTimer != null)
            {
                simulationTimer.Stop();
            }

            // Resetar todos os processos
            foreach (var process in processList)
            {
                process.RemainingTime = process.ExecutionTime;
                process.State = ProcessState.Ready;
                UpdateProcessUI(process);
            }

            currentProcess = null;
            readyQueue.Clear();
            elapsedTime = 0;
            currentQuantum = 0;

            StartButton.Content = "▶ INICIAR";
            AddProcessButton.IsEnabled = true;
            SchedulingComboBox.IsEnabled = true;

            UpdateStatus();
        }

        private void UpdateStatus()
        {
            if (currentProcess != null)
            {
                CurrentProcessText.Text = $"Processo atual: {currentProcess.Name} ({currentProcess.RemainingTime}ms restantes)";
            }
            else
            {
                CurrentProcessText.Text = "Processo atual: Nenhum";
            }

            TimeElapsedText.Text = $"Tempo decorrido: {elapsedTime}ms";

            if (simulationTimer != null && !simulationTimer.IsEnabled && processList.Count > 0 && elapsedTime == 0)
            {
                StatusText.Text = $"Pronto para iniciar ({processList.Count} processo(s) na fila)";
            }
        }

        private void UpdateProcessUI(Process process)
        {
            if (process?.UIElement == null) return;

            var grid = process.UIElement.Child as Grid;
            if (grid == null || grid.Children.Count < 2) return;

            var infoPanel = grid.Children[0] as StackPanel;
            if (infoPanel == null || infoPanel.Children.Count < 2) return;

            var detailsText = infoPanel.Children[1] as TextBlock;
            if (detailsText == null) return;

            string state = "";
            Color borderColor;

            switch (process.State)
            {
                case ProcessState.Ready:
                    state = "Pronto";
                    borderColor = (Color)ColorConverter.ConvertFromString("#BDC3C7");
                    break;
                case ProcessState.Running:
                    state = "Executando";
                    borderColor = (Color)ColorConverter.ConvertFromString("#27AE60");
                    break;
                case ProcessState.Waiting:
                    state = "Esperando";
                    borderColor = (Color)ColorConverter.ConvertFromString("#F39C12");
                    break;
                case ProcessState.Terminated:
                    state = "Terminado";
                    borderColor = (Color)ColorConverter.ConvertFromString("#95A5A6");
                    break;
                default:
                    state = "Desconhecido";
                    borderColor = (Color)ColorConverter.ConvertFromString("#BDC3C7");
                    break;
            }

            detailsText.Text = $"Prioridade: {process.Priority} | Estado: {state} | Restante: {process.RemainingTime}ms";
            process.UIElement.BorderBrush = new SolidColorBrush(borderColor);
            process.UIElement.BorderThickness = new Thickness(2);
        }

        private void ExercisesButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Exercícios práticos sobre Sistemas Operacionais em breve!", "Em Desenvolvimento",
                MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void QuizButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var testeWindow = new SistemaOperacionalTeste();
                testeWindow.Show();
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Erro ao abrir o teste: {ex.Message}",
                    "Erro",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        // Método auxiliar para encontrar elementos filhos
        private T FindVisualChild<T>(DependencyObject parent) where T : DependencyObject
        {
            if (parent == null) return null;

            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                if (child != null && child is T)
                {
                    return (T)child;
                }

                var childOfChild = FindVisualChild<T>(child);
                if (childOfChild != null)
                {
                    return childOfChild;
                }
            }
            return null;
        }

        protected override void OnClosed(EventArgs e)
        {
            if (simulationTimer != null)
            {
                simulationTimer.Stop();
            }
            base.OnClosed(e);
        }
    }
}