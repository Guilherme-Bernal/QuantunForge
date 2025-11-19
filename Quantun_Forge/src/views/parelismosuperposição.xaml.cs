using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Quantun_Forge.src.views
{
    public partial class parelismosuperposição : UserControl
    {
        // Estado atual
        private int n = 3; // Número de bits/qubits
        private int totalStates = 8; // 2^n
        private int currentClassicalState = 0; // Estado ativo no clássico
        private bool[] quantumSuperposition; // Estados em superposição

        // Para o processamento sequencial
        private int classicalProcessStep = 0;
        private DispatcherTimer? classicalTimer;

        // Para o processamento paralelo
        private bool quantumProcessed = false;

        // Dados para a lista de processamento
        private ObservableCollection<ProcessingItem> classicalItems = new ObservableCollection<ProcessingItem>();

        public parelismosuperposição()
        {
            InitializeComponent();

            totalStates = (int)Math.Pow(2, n);
            quantumSuperposition = new bool[totalStates];

            Loaded += ParalelismoSuperposicao_Loaded;
        }

        private void ParalelismoSuperposicao_Loaded(object sender, RoutedEventArgs e)
        {
            // Configurar lista de processamento
            if (ClassicalProcessingList != null)
            {
                ClassicalProcessingList.ItemsSource = classicalItems;
            }

            // Inicialização
            UpdateNValue();
            GenerateStateGrids();
        }

        #region Controle de N (número de bits/qubits)

        private void SliderN_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (!IsLoaded) return;

            n = (int)e.NewValue;
            UpdateNValue();
            GenerateStateGrids();
            ResetSimulators();
        }

        private void UpdateNValue()
        {
            if (TxtNValue == null || TxtNInfo == null) return;

            totalStates = (int)Math.Pow(2, n);
            quantumSuperposition = new bool[totalStates];

            TxtNValue.Text = n.ToString();
            TxtNInfo.Text = $"2^{n} = {totalStates} estados possíveis";
        }

        private void ResetSimulators()
        {
            currentClassicalState = 0;
            classicalProcessStep = 0;
            quantumProcessed = false;

            classicalItems.Clear();

            if (TxtClassicalTime != null)
                TxtClassicalTime.Text = "Tempo: 0 passos";

            if (TxtQuantumTime != null)
                TxtQuantumTime.Text = "Tempo: 0 passos";

            if (QuantumProcessingGrid != null)
                QuantumProcessingGrid.Children.Clear();
        }

        #endregion

        #region Simulador 1: Visualização de Estados

        private void GenerateStateGrids()
        {
            GenerateClassicalGrid();
            GenerateQuantumGrid();
        }

        private void GenerateClassicalGrid()
        {
            if (ClassicalStatesGrid == null || TxtClassicalInfo == null) return;

            ClassicalStatesGrid.Children.Clear();

            for (int i = 0; i < totalStates; i++)
            {
                int stateIndex = i; // Captura para o evento

                var border = new Border
                {
                    Width = 50,
                    Height = 50,
                    Margin = new Thickness(3),
                    CornerRadius = new CornerRadius(8),
                    BorderThickness = new Thickness(2),
                    BorderBrush = new SolidColorBrush(Color.FromRgb(25, 118, 210)),
                    Background = stateIndex == currentClassicalState ?
                        new SolidColorBrush(Color.FromRgb(231, 76, 60)) :
                        new SolidColorBrush(Color.FromRgb(236, 240, 241)),
                    Cursor = System.Windows.Input.Cursors.Hand
                };

                var textBlock = new TextBlock
                {
                    Text = Convert.ToString(i, 2).PadLeft(n, '0'),
                    FontSize = 12,
                    FontWeight = FontWeights.Bold,
                    Foreground = stateIndex == currentClassicalState ?
                        Brushes.White :
                        new SolidColorBrush(Color.FromRgb(44, 62, 80)),
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center
                };

                border.Child = textBlock;

                // Evento de clique
                border.MouseLeftButtonDown += (s, e) =>
                {
                    currentClassicalState = stateIndex;
                    GenerateClassicalGrid(); // Redesenha
                };

                ClassicalStatesGrid.Children.Add(border);
            }

            TxtClassicalInfo.Text = $"Apenas 1 estado ativo: {Convert.ToString(currentClassicalState, 2).PadLeft(n, '0')}";
        }

        private void GenerateQuantumGrid()
        {
            if (QuantumStatesGrid == null || TxtQuantumInfo == null) return;

            QuantumStatesGrid.Children.Clear();

            for (int i = 0; i < totalStates; i++)
            {
                bool isInSuperposition = quantumSuperposition[i];

                var border = new Border
                {
                    Width = 50,
                    Height = 50,
                    Margin = new Thickness(3),
                    CornerRadius = new CornerRadius(8),
                    BorderThickness = new Thickness(2),
                    BorderBrush = new SolidColorBrush(Color.FromRgb(123, 31, 162)),
                    Background = isInSuperposition ?
                        new SolidColorBrush(Color.FromRgb(243, 229, 245)) :
                        new SolidColorBrush(Color.FromRgb(236, 240, 241))
                };

                var textBlock = new TextBlock
                {
                    Text = Convert.ToString(i, 2).PadLeft(n, '0'),
                    FontSize = 12,
                    FontWeight = FontWeights.Bold,
                    Foreground = new SolidColorBrush(Color.FromRgb(44, 62, 80)),
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center
                };

                border.Child = textBlock;

                // Efeito de brilho se estiver em superposição
                if (isInSuperposition)
                {
                    border.Effect = new System.Windows.Media.Effects.DropShadowEffect
                    {
                        Color = Color.FromRgb(155, 89, 182),
                        BlurRadius = 10,
                        ShadowDepth = 0,
                        Opacity = 0.6
                    };
                }

                QuantumStatesGrid.Children.Add(border);
            }

            int activeCount = quantumSuperposition.Count(x => x);
            if (activeCount == 0)
            {
                TxtQuantumInfo.Text = "Clique em 'Criar Superposição' para ativar todos os estados!";
            }
            else
            {
                TxtQuantumInfo.Text = $"Todos os {activeCount} estados simultâneos!";
            }
        }

        private void BtnChangeClassicalState_Click(object sender, RoutedEventArgs e)
        {
            // Muda para o próximo estado
            currentClassicalState = (currentClassicalState + 1) % totalStates;
            GenerateClassicalGrid();

            // Anima o estado mudando
            AnimateStateChange();
        }

        private void AnimateStateChange()
        {
            if (ClassicalStatesGrid == null) return;

            var animation = new DoubleAnimation
            {
                From = 0.5,
                To = 1,
                Duration = TimeSpan.FromSeconds(0.3),
                AutoReverse = false
            };

            ClassicalStatesGrid.BeginAnimation(OpacityProperty, animation);
        }

        private void BtnCreateSuperposition_Click(object sender, RoutedEventArgs e)
        {
            // Ativa todos os estados
            for (int i = 0; i < totalStates; i++)
            {
                quantumSuperposition[i] = true;
            }

            GenerateQuantumGrid();
            AnimateSuperpositionCreation();
        }

        private void AnimateSuperpositionCreation()
        {
            if (QuantumStatesGrid == null) return;

            // Anima cada caixa aparecendo em sequência
            int delay = 0;
            foreach (UIElement child in QuantumStatesGrid.Children)
            {
                if (child is Border border)
                {
                    border.Opacity = 0;

                    var animation = new DoubleAnimation
                    {
                        From = 0,
                        To = 1,
                        Duration = TimeSpan.FromSeconds(0.3),
                        BeginTime = TimeSpan.FromMilliseconds(delay)
                    };

                    border.BeginAnimation(OpacityProperty, animation);
                    delay += 50;
                }
            }
        }

        #endregion

        #region Simulador 2: Processamento de Função

        private void BtnProcessClassical_Click(object sender, RoutedEventArgs e)
        {
            // Reset
            classicalItems.Clear();
            classicalProcessStep = 0;

            // Criar lista de inputs para processar
            for (int i = 0; i < totalStates; i++)
            {
                classicalItems.Add(new ProcessingItem
                {
                    Input = Convert.ToString(i, 2).PadLeft(n, '0'),
                    Output = "",
                    Color = Brushes.Gray
                });
            }

            // Iniciar processamento sequencial
            StartClassicalProcessing();
        }

        private void StartClassicalProcessing()
        {
            if (classicalTimer != null)
            {
                classicalTimer.Stop();
                classicalTimer = null;
            }

            classicalTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(500)
            };

            classicalTimer.Tick += ClassicalTimer_Tick;
            classicalTimer.Start();
        }

        private void ClassicalTimer_Tick(object? sender, EventArgs e)
        {
            if (classicalProcessStep >= classicalItems.Count)
            {
                classicalTimer?.Stop();
                if (TxtClassicalTime != null)
                    TxtClassicalTime.Text = $"Tempo: {totalStates} passos (sequencial)";
                return;
            }

            // Processar o item atual
            var item = classicalItems[classicalProcessStep];
            int x = Convert.ToInt32(item.Input, 2);
            int result = x % 2; // f(x) = x mod 2

            item.Output = result.ToString();
            item.Color = result == 0 ?
                new SolidColorBrush(Color.FromRgb(52, 152, 219)) :
                new SolidColorBrush(Color.FromRgb(231, 76, 60));

            // Forçar atualização visual
            classicalItems[classicalProcessStep] = new ProcessingItem
            {
                Input = item.Input,
                Output = item.Output,
                Color = item.Color
            };

            classicalProcessStep++;

            if (TxtClassicalTime != null)
                TxtClassicalTime.Text = $"Tempo: {classicalProcessStep} passos";
        }

        private void BtnProcessQuantum_Click(object sender, RoutedEventArgs e)
        {
            if (QuantumProcessingGrid == null || TxtQuantumTime == null) return;

            // Limpar grid anterior
            QuantumProcessingGrid.Children.Clear();

            // Criar superposição se ainda não existe
            bool hasSuperposition = quantumSuperposition.Any(x => x);
            if (!hasSuperposition)
            {
                for (int i = 0; i < totalStates; i++)
                {
                    quantumSuperposition[i] = true;
                }
            }

            // Aplicar função Uf em TODOS os estados simultaneamente
            for (int i = 0; i < totalStates; i++)
            {
                if (quantumSuperposition[i])
                {
                    int result = i % 2; // f(x) = x mod 2

                    var border = new Border
                    {
                        Width = 45,
                        Height = 60,
                        Margin = new Thickness(3),
                        CornerRadius = new CornerRadius(6),
                        BorderThickness = new Thickness(2),
                        BorderBrush = new SolidColorBrush(Color.FromRgb(155, 89, 182)),
                        Background = result == 0 ?
                            new SolidColorBrush(Color.FromRgb(227, 242, 253)) :
                            new SolidColorBrush(Color.FromRgb(255, 235, 238)),
                        Opacity = 0
                    };

                    var stack = new StackPanel
                    {
                        VerticalAlignment = VerticalAlignment.Center
                    };

                    var inputText = new TextBlock
                    {
                        Text = Convert.ToString(i, 2).PadLeft(n, '0'),
                        FontSize = 10,
                        FontWeight = FontWeights.Bold,
                        HorizontalAlignment = HorizontalAlignment.Center,
                        Foreground = new SolidColorBrush(Color.FromRgb(44, 62, 80))
                    };

                    var arrowText = new TextBlock
                    {
                        Text = "↓",
                        FontSize = 12,
                        HorizontalAlignment = HorizontalAlignment.Center,
                        Foreground = new SolidColorBrush(Color.FromRgb(155, 89, 182))
                    };

                    var outputText = new TextBlock
                    {
                        Text = result.ToString(),
                        FontSize = 14,
                        FontWeight = FontWeights.Bold,
                        HorizontalAlignment = HorizontalAlignment.Center,
                        Foreground = result == 0 ?
                            new SolidColorBrush(Color.FromRgb(52, 152, 219)) :
                            new SolidColorBrush(Color.FromRgb(231, 76, 60))
                    };

                    stack.Children.Add(inputText);
                    stack.Children.Add(arrowText);
                    stack.Children.Add(outputText);
                    border.Child = stack;

                    QuantumProcessingGrid.Children.Add(border);
                }
            }

            // Animar todas as caixas aparecendo SIMULTANEAMENTE
            AnimateQuantumProcessing();

            TxtQuantumTime.Text = "Tempo: 1 passo (paralelo!)";
            quantumProcessed = true;
        }

        private void AnimateQuantumProcessing()
        {
            if (QuantumProcessingGrid == null) return;

            var storyboard = new Storyboard();

            foreach (UIElement child in QuantumProcessingGrid.Children)
            {
                if (child is Border border)
                {
                    // Todas começam ao mesmo tempo!
                    var fadeIn = new DoubleAnimation
                    {
                        From = 0,
                        To = 1,
                        Duration = TimeSpan.FromSeconds(0.8),
                        EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut }
                    };

                    Storyboard.SetTarget(fadeIn, border);
                    Storyboard.SetTargetProperty(fadeIn, new PropertyPath(OpacityProperty));
                    storyboard.Children.Add(fadeIn);

                    // Efeito de escala
                    border.RenderTransform = new ScaleTransform(0.3, 0.3);
                    border.RenderTransformOrigin = new Point(0.5, 0.5);

                    var scaleX = new DoubleAnimation
                    {
                        From = 0.3,
                        To = 1,
                        Duration = TimeSpan.FromSeconds(0.8),
                        EasingFunction = new BackEase { EasingMode = EasingMode.EaseOut }
                    };

                    var scaleY = new DoubleAnimation
                    {
                        From = 0.3,
                        To = 1,
                        Duration = TimeSpan.FromSeconds(0.8),
                        EasingFunction = new BackEase { EasingMode = EasingMode.EaseOut }
                    };

                    Storyboard.SetTarget(scaleX, border);
                    Storyboard.SetTargetProperty(scaleX, new PropertyPath("(UIElement.RenderTransform).(ScaleTransform.ScaleX)"));
                    storyboard.Children.Add(scaleX);

                    Storyboard.SetTarget(scaleY, border);
                    Storyboard.SetTargetProperty(scaleY, new PropertyPath("(UIElement.RenderTransform).(ScaleTransform.ScaleY)"));
                    storyboard.Children.Add(scaleY);
                }
            }

            storyboard.Begin();
        }

        #endregion
    }

    #region Classes Auxiliares

    public class ProcessingItem
    {
        public string Input { get; set; } = "";
        public string Output { get; set; } = "";
        public Brush Color { get; set; } = Brushes.Gray;
    }

    #endregion
}