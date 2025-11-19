using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Quantun_Forge.src.views.ModuloEnsino.Fisica.FisicaClassica
{
    public partial class MecanicaAnaliticaWindow : Window
    {
        private DispatcherTimer animationTimer;
        private double time = 0;
        private const double dt = 0.05; // Passo de tempo

        public MecanicaAnaliticaWindow()
        {
            InitializeComponent();
            InitializeCanvas();

            animationTimer = new DispatcherTimer();
            animationTimer.Interval = TimeSpan.FromMilliseconds(50);
            animationTimer.Tick += AnimationTimer_Tick;
        }

        // Inicializa o canvas com eixos
        private void InitializeCanvas()
        {
            DrawAxes();
        }

        // Desenha eixos do espaço de fase
        private void DrawAxes()
        {
            PhaseSpaceCanvas.Children.Clear();

            double width = PhaseSpaceCanvas.ActualWidth > 0 ? PhaseSpaceCanvas.ActualWidth : 350;
            double height = PhaseSpaceCanvas.ActualHeight > 0 ? PhaseSpaceCanvas.ActualHeight : 300;

            double centerX = width / 2;
            double centerY = height / 2;

            // Eixo horizontal (q)
            Line xAxis = new Line
            {
                X1 = 0,
                Y1 = centerY,
                X2 = width,
                Y2 = centerY,
                Stroke = new SolidColorBrush(Color.FromRgb(189, 195, 199)),
                StrokeThickness = 1
            };
            PhaseSpaceCanvas.Children.Add(xAxis);

            // Eixo vertical (p)
            Line yAxis = new Line
            {
                X1 = centerX,
                Y1 = 0,
                X2 = centerX,
                Y2 = height,
                Stroke = new SolidColorBrush(Color.FromRgb(189, 195, 199)),
                StrokeThickness = 1
            };
            PhaseSpaceCanvas.Children.Add(yAxis);

            // Labels
            TextBlock qLabel = new TextBlock
            {
                Text = "q",
                FontSize = 12,
                Foreground = new SolidColorBrush(Color.FromRgb(127, 140, 141)),
                FontWeight = FontWeights.Bold
            };
            Canvas.SetLeft(qLabel, width - 20);
            Canvas.SetTop(qLabel, centerY + 5);
            PhaseSpaceCanvas.Children.Add(qLabel);

            TextBlock pLabel = new TextBlock
            {
                Text = "p",
                FontSize = 12,
                Foreground = new SolidColorBrush(Color.FromRgb(127, 140, 141)),
                FontWeight = FontWeights.Bold
            };
            Canvas.SetLeft(pLabel, centerX + 5);
            Canvas.SetTop(pLabel, 5);
            PhaseSpaceCanvas.Children.Add(pLabel);
        }

        // Seleção de sistema mudou
        private void SystemSelector_Changed(object sender, SelectionChangedEventArgs e)
        {
            if (SystemSelector == null || SystemSelector.SelectedIndex == -1) return;
            if (CurrentSystemFormula == null || InterpretationText == null) return;

            switch (SystemSelector.SelectedIndex)
            {
                case 0: // Oscilador Harmônico
                    CurrentSystemFormula.Text = "Lagrangiana: L = ½mẋ² - ½kx²\n" +
                                               "Hamiltoniana: H = p²/2m + ½kx²\n" +
                                               "Espaço de Fase: Elipse";
                    InterpretationText.Text = "Trajetória elíptica no espaço de fase. A energia se conserva, " +
                                            "oscilando entre cinética e potencial.";
                    break;

                case 1: // Pêndulo Simples
                    CurrentSystemFormula.Text = "Lagrangiana: L = ½ml²θ̇² + mgl·cos(θ)\n" +
                                               "Hamiltoniana: H = p²/2ml² - mgl·cos(θ)\n" +
                                               "Espaço de Fase: Curvas senoidais";
                    InterpretationText.Text = "Para pequenas amplitudes, comporta-se como oscilador harmônico. " +
                                            "Grandes energias levam a rotações completas.";
                    break;

                case 2: // Partícula Livre
                    CurrentSystemFormula.Text = "Lagrangiana: L = ½mẋ²\n" +
                                               "Hamiltoniana: H = p²/2m\n" +
                                               "Espaço de Fase: Linha horizontal";
                    InterpretationText.Text = "Momento constante (linha horizontal no espaço de fase). " +
                                            "Movimento retilíneo uniforme.";
                    break;

                case 3: // Pêndulo Duplo
                    CurrentSystemFormula.Text = "Sistema com 2 graus de liberdade\n" +
                                               "Espaço de fase 4D: (θ₁, θ₂, p₁, p₂)\n" +
                                               "Comportamento caótico!";
                    InterpretationText.Text = "Sistema não-linear altamente sensível a condições iniciais. " +
                                            "Pequenas mudanças levam a trajetórias completamente diferentes.";
                    break;
            }

            time = 0;
            if (PhaseSpaceCanvas != null)
            {
                DrawAxes();
            }
        }

        // Parâmetros mudaram
        private void Parameter_Changed(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (EnergyValue != null)
            {
                EnergyValue.Text = EnergySlider.Value.ToString("F1");
            }
        }

        // Botão SIMULAR
        private void Simulate_Click(object sender, RoutedEventArgs e)
        {
            time = 0;
            DrawAxes();

            int systemType = SystemSelector.SelectedIndex;
            double energy = EnergySlider.Value;

            switch (systemType)
            {
                case 0: // Oscilador Harmônico
                    DrawHarmonicOscillator(energy);
                    break;

                case 1: // Pêndulo
                    DrawPendulum(energy);
                    break;

                case 2: // Partícula Livre
                    DrawFreeParticle(energy);
                    break;

                case 3: // Pêndulo Duplo
                    DrawDoublePendulum(energy);
                    break;
            }
        }

        // Desenha trajetória do oscilador harmônico no espaço de fase
        private void DrawHarmonicOscillator(double energy)
        {
            double width = PhaseSpaceCanvas.ActualWidth > 0 ? PhaseSpaceCanvas.ActualWidth : 350;
            double height = PhaseSpaceCanvas.ActualHeight > 0 ? PhaseSpaceCanvas.ActualHeight : 300;

            double centerX = width / 2;
            double centerY = height / 2;

            // Parâmetros: m = 1, k = 1, ω = 1
            double m = 1.0;
            double k = 1.0;
            double omega = Math.Sqrt(k / m);

            // Amplitude baseada na energia: E = ½kA²
            double A = Math.Sqrt(2 * energy / k);
            double scaleQ = Math.Min(width, height) / 3 / Math.Max(A, 1);
            double scaleP = scaleQ;

            Polyline trajectory = new Polyline
            {
                Stroke = new SolidColorBrush(Color.FromRgb(142, 68, 173)),
                StrokeThickness = 2
            };

            // Desenha elipse (trajetória no espaço de fase)
            for (double t = 0; t <= 2 * Math.PI; t += 0.05)
            {
                double q = A * Math.Cos(omega * t);
                double p = -m * omega * A * Math.Sin(omega * t);

                double x = centerX + q * scaleQ;
                double y = centerY - p * scaleP;

                trajectory.Points.Add(new Point(x, y));
            }

            PhaseSpaceCanvas.Children.Add(trajectory);

            // Ponto inicial
            Ellipse startPoint = new Ellipse
            {
                Width = 8,
                Height = 8,
                Fill = new SolidColorBrush(Color.FromRgb(231, 76, 60))
            };
            Canvas.SetLeft(startPoint, centerX + A * scaleQ - 4);
            Canvas.SetTop(startPoint, centerY - 4);
            PhaseSpaceCanvas.Children.Add(startPoint);
        }

        // Desenha trajetória do pêndulo
        private void DrawPendulum(double energy)
        {
            double width = PhaseSpaceCanvas.ActualWidth > 0 ? PhaseSpaceCanvas.ActualWidth : 350;
            double height = PhaseSpaceCanvas.ActualHeight > 0 ? PhaseSpaceCanvas.ActualHeight : 300;

            double centerX = width / 2;
            double centerY = height / 2;

            double g = 9.8;
            double l = 1.0;
            double m = 1.0;

            double scale = Math.Min(width, height) / 8;

            Polyline trajectory = new Polyline
            {
                Stroke = new SolidColorBrush(Color.FromRgb(142, 68, 173)),
                StrokeThickness = 2
            };

            // Pequena amplitude: comportamento harmônico
            double maxAngle = Math.Min(Math.Sqrt(2 * energy / (m * g * l)), Math.PI / 2);

            for (double t = 0; t <= 10; t += 0.05)
            {
                double theta = maxAngle * Math.Sin(Math.Sqrt(g / l) * t);
                double thetaDot = maxAngle * Math.Sqrt(g / l) * Math.Cos(Math.Sqrt(g / l) * t);
                double p = m * l * l * thetaDot;

                double x = centerX + theta * scale * 30;
                double y = centerY - p * scale;

                trajectory.Points.Add(new Point(x, y));
            }

            PhaseSpaceCanvas.Children.Add(trajectory);
        }

        // Desenha trajetória de partícula livre
        private void DrawFreeParticle(double energy)
        {
            double width = PhaseSpaceCanvas.ActualWidth > 0 ? PhaseSpaceCanvas.ActualWidth : 350;
            double height = PhaseSpaceCanvas.ActualHeight > 0 ? PhaseSpaceCanvas.ActualHeight : 300;

            double centerX = width / 2;
            double centerY = height / 2;

            double m = 1.0;
            double p = Math.Sqrt(2 * m * energy); // p = √(2mE)

            double scale = Math.Min(width, height) / 6;

            // Linha horizontal (momento constante)
            Line trajectory = new Line
            {
                X1 = 20,
                Y1 = centerY - p * scale,
                X2 = width - 20,
                Y2 = centerY - p * scale,
                Stroke = new SolidColorBrush(Color.FromRgb(142, 68, 173)),
                StrokeThickness = 2
            };
            PhaseSpaceCanvas.Children.Add(trajectory);

            // Setas indicando direção
            TextBlock arrow = new TextBlock
            {
                Text = "→",
                FontSize = 20,
                Foreground = new SolidColorBrush(Color.FromRgb(231, 76, 60)),
                FontWeight = FontWeights.Bold
            };
            Canvas.SetLeft(arrow, width - 40);
            Canvas.SetTop(arrow, centerY - p * scale - 15);
            PhaseSpaceCanvas.Children.Add(arrow);
        }

        // Desenha pêndulo duplo (simplificado)
        private void DrawDoublePendulum(double energy)
        {
            double width = PhaseSpaceCanvas.ActualWidth > 0 ? PhaseSpaceCanvas.ActualWidth : 350;
            double height = PhaseSpaceCanvas.ActualHeight > 0 ? PhaseSpaceCanvas.ActualHeight : 300;

            double centerX = width / 2;
            double centerY = height / 2;

            // Desenha uma trajetória caótica simplificada
            Random rand = new Random();
            double scale = Math.Min(width, height) / 6;

            for (int i = 0; i < 3; i++)
            {
                Polyline trajectory = new Polyline
                {
                    Stroke = new SolidColorBrush(Color.FromRgb(
                        (byte)(142 + rand.Next(-30, 30)),
                        (byte)(68 + rand.Next(-30, 30)),
                        (byte)(173 + rand.Next(-30, 30))
                    )),
                    StrokeThickness = 1.5,
                    Opacity = 0.7
                };

                double q = (rand.NextDouble() - 0.5) * 2;
                double p = (rand.NextDouble() - 0.5) * 2;

                for (int j = 0; j < 500; j++)
                {
                    // Dinâmica caótica simplificada
                    double dq = p * 0.1;
                    double dp = -Math.Sin(q * energy) * 0.1 + (rand.NextDouble() - 0.5) * 0.05;

                    q += dq;
                    p += dp;

                    double x = centerX + q * scale;
                    double y = centerY - p * scale;

                    if (x > 0 && x < width && y > 0 && y < height)
                    {
                        trajectory.Points.Add(new Point(x, y));
                    }
                }

                PhaseSpaceCanvas.Children.Add(trajectory);
            }

            // Aviso
            TextBlock warning = new TextBlock
            {
                Text = "⚠ Comportamento Caótico!",
                FontSize = 11,
                Foreground = new SolidColorBrush(Color.FromRgb(231, 76, 60)),
                FontWeight = FontWeights.Bold
            };
            Canvas.SetLeft(warning, 10);
            Canvas.SetTop(warning, height - 25);
            PhaseSpaceCanvas.Children.Add(warning);
        }

        // Timer para animação
        private void AnimationTimer_Tick(object sender, EventArgs e)
        {
            time += dt;
            // Pode adicionar animação aqui se desejar
        }

        // Botão LIMPAR
        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            time = 0;
            DrawAxes();
        }

        // Botão EXERCÍCIOS
        private void ExercisesButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(
                "📝 EXERCÍCIOS - MECÂNICA ANALÍTICA\n\n" +
                "1. Derive as equações de movimento para um pêndulo usando o formalismo lagrangiano.\n\n" +
                "2. Mostre que H = T + V para um sistema conservativo.\n\n" +
                "3. Calcule os colchetes de Poisson {q, p} e {H, q}.\n\n" +
                "4. Para um oscilador harmônico com L = ½mẋ² - ½kx², encontre:\n" +
                "   a) O momento conjugado p\n" +
                "   b) A hamiltoniana H\n" +
                "   c) As equações de Hamilton\n\n" +
                "5. Use o Teorema de Noether para demonstrar a conservação do momento linear.",
                "Exercícios - Mecânica Analítica",
                MessageBoxButton.OK,
                MessageBoxImage.Information
            );
        }

        // Botão QUIZ
        private void QuizButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(
                "🧪 QUIZ - MECÂNICA ANALÍTICA\n\n" +
                "1. O que é a Lagrangiana?\n" +
                "   R: L = T - V (energia cinética menos potencial)\n\n" +
                "2. Qual o princípio fundamental que leva às equações de Euler-Lagrange?\n" +
                "   R: Princípio da ação mínima (δS = 0)\n\n" +
                "3. O que a Hamiltoniana representa em sistemas conservativos?\n" +
                "   R: A energia total do sistema (H = T + V)\n\n" +
                "4. O que o Teorema de Noether relaciona?\n" +
                "   R: Simetrias com leis de conservação\n\n" +
                "5. Quantas dimensões tem o espaço de fase para N partículas em 3D?\n" +
                "   R: 6N dimensões (3N coordenadas + 3N momentos)",
                "Quiz - Mecânica Analítica",
                MessageBoxButton.OK,
                MessageBoxImage.Question
            );
        }

        // Botão VOLTAR
        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}