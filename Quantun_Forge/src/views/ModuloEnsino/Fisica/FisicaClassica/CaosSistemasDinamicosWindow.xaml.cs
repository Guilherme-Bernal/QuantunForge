using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Quantun_Forge.src.views.ModuloEnsino.Fisica.FisicaClassica
{
    public partial class CaosSistemasDinamicosWindow : Window
    {
        private int currentSimulation = 0; // 0: Logistic Map, 1: Lorenz, 2: Butterfly, 3: Mandelbrot

        public CaosSistemasDinamicosWindow()
        {
            InitializeComponent();
            UpdateSimulationInfo();
        }

        private void ChaosSelector_Changed(object sender, SelectionChangedEventArgs e)
        {
            if (ChaosSelector == null) return;

            currentSimulation = ChaosSelector.SelectedIndex;
            UpdateSimulationInfo();
            ChaosCanvas.Children.Clear();
        }

        private void UpdateSimulationInfo()
        {
            if (CurrentSimulationInfo == null || ObservationText == null) return;

            switch (currentSimulation)
            {
                case 0: // Mapa Logístico
                    CurrentSimulationInfo.Text = "Mapa Logístico: xₙ₊₁ = r·xₙ(1-xₙ)\n" +
                                                "Diagrama de bifurcação mostra como o comportamento muda com r.\n" +
                                                "Para r > 3.57: comportamento caótico!";
                    ObservationText.Text = "Observe como pequenas mudanças no parâmetro r causam grandes mudanças no comportamento. " +
                                          "A transição de ordem para caos é gradual através de bifurcações.";
                    ParameterRPanel.Visibility = Visibility.Visible;
                    InitialConditionPanel.Visibility = Visibility.Visible;
                    break;

                case 1: // Lorenz
                    CurrentSimulationInfo.Text = "Atrator de Lorenz (Sistema de 3 EDOs)\n" +
                                                "dx/dt = σ(y-x), dy/dt = x(ρ-z)-y, dz/dt = xy-βz\n" +
                                                "Forma clássica de 'borboleta' no espaço 3D.";
                    ObservationText.Text = "O atrator de Lorenz nunca se repete exatamente. " +
                                          "As trajetórias orbitam em torno de dois pontos, criando a forma de borboleta.";
                    ParameterRPanel.Visibility = Visibility.Collapsed;
                    InitialConditionPanel.Visibility = Visibility.Collapsed;
                    break;

                case 2: // Efeito Borboleta
                    CurrentSimulationInfo.Text = "Efeito Borboleta: Sensibilidade às Condições Iniciais\n" +
                                                "Duas trajetórias com diferença inicial de 0.000001\n" +
                                                "Observe como divergem exponencialmente!";
                    ObservationText.Text = "Ambas as trajetórias seguem as mesmas leis físicas, mas uma pequena " +
                                          "diferença inicial cresce exponencialmente com o tempo.";
                    ParameterRPanel.Visibility = Visibility.Visible;
                    InitialConditionPanel.Visibility = Visibility.Visible;
                    break;

                case 3: // Mandelbrot
                    CurrentSimulationInfo.Text = "Conjunto de Mandelbrot: zₙ₊₁ = zₙ² + c\n" +
                                                "Fractal complexo com estrutura infinitamente detalhada.\n" +
                                                "Fronteira entre ordem e caos.";
                    ObservationText.Text = "Cada ponto é colorido conforme o número de iterações necessárias para divergir. " +
                                          "O conjunto possui autossimilaridade em todas as escalas.";
                    ParameterRPanel.Visibility = Visibility.Collapsed;
                    InitialConditionPanel.Visibility = Visibility.Collapsed;
                    break;
            }
        }

        private void Parameter_Changed(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (ParameterRValue != null && ParameterR != null)
                ParameterRValue.Text = ParameterR.Value.ToString("F2");

            if (InitialConditionValue != null && InitialCondition != null)
                InitialConditionValue.Text = InitialCondition.Value.ToString("F2");
        }

        private void Simulate_Click(object sender, RoutedEventArgs e)
        {
            ChaosCanvas.Children.Clear();

            switch (currentSimulation)
            {
                case 0:
                    DrawLogisticMap();
                    break;
                case 1:
                    DrawLorenzAttractor();
                    break;
                case 2:
                    DrawButterflyEffect();
                    break;
                case 3:
                    DrawMandelbrotSet();
                    break;
            }
        }

        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            ChaosCanvas.Children.Clear();
        }

        #region Mapa Logístico
        private void DrawLogisticMap()
        {
            double width = ChaosCanvas.ActualWidth;
            double height = ChaosCanvas.ActualHeight;

            if (width <= 0 || height <= 0) return;

            double x0 = InitialCondition.Value;
            int iterations = 200;
            int skip = 100; // Pular transientes

            // Desenhar para múltiplos valores de r
            for (double r = 2.5; r <= 4.0; r += 0.005)
            {
                double x = x0;

                // Pular transientes
                for (int i = 0; i < skip; i++)
                {
                    x = r * x * (1 - x);
                }

                // Plotar pontos
                for (int i = 0; i < iterations; i++)
                {
                    x = r * x * (1 - x);

                    double px = ((r - 2.5) / 1.5) * width;
                    double py = height - (x * height);

                    Ellipse dot = new Ellipse
                    {
                        Width = 1,
                        Height = 1,
                        Fill = new SolidColorBrush(Color.FromRgb(230, 126, 34))
                    };

                    Canvas.SetLeft(dot, px);
                    Canvas.SetTop(dot, py);
                    ChaosCanvas.Children.Add(dot);
                }
            }

            // Adicionar linha vertical no valor atual de r
            double currentR = ParameterR.Value;
            if (currentR >= 2.5 && currentR <= 4.0)
            {
                double lineX = ((currentR - 2.5) / 1.5) * width;
                Line marker = new Line
                {
                    X1 = lineX,
                    Y1 = 0,
                    X2 = lineX,
                    Y2 = height,
                    Stroke = Brushes.Red,
                    StrokeThickness = 2
                };
                ChaosCanvas.Children.Add(marker);
            }

            // Adicionar eixos e labels
            AddAxisLabels("r", "x", "2.5", "4.0", "0", "1");
        }
        #endregion

        #region Atrator de Lorenz
        private void DrawLorenzAttractor()
        {
            double width = ChaosCanvas.ActualWidth;
            double height = ChaosCanvas.ActualHeight;

            if (width <= 0 || height <= 0) return;

            // Parâmetros de Lorenz
            double sigma = 10.0;
            double rho = 28.0;
            double beta = 8.0 / 3.0;

            // Condições iniciais
            double x = 0.1, y = 0.0, z = 0.0;
            double dt = 0.01;
            int steps = 3000;

            List<Point> points = new List<Point>();

            for (int i = 0; i < steps; i++)
            {
                double dx = sigma * (y - x);
                double dy = x * (rho - z) - y;
                double dz = x * y - beta * z;

                x += dx * dt;
                y += dy * dt;
                z += dz * dt;

                // Projeção 2D (x-z)
                double px = (x + 20) * width / 40.0;
                double py = height - (z * height / 50.0);

                points.Add(new Point(px, py));
            }

            // Desenhar trajetória
            for (int i = 1; i < points.Count; i++)
            {
                Line line = new Line
                {
                    X1 = points[i - 1].X,
                    Y1 = points[i - 1].Y,
                    X2 = points[i].X,
                    Y2 = points[i].Y,
                    Stroke = new SolidColorBrush(Color.FromRgb(22, 160, 133)),
                    StrokeThickness = 0.5,
                    Opacity = Math.Min(1.0, i / 500.0)
                };
                ChaosCanvas.Children.Add(line);
            }
        }
        #endregion

        #region Efeito Borboleta
        private void DrawButterflyEffect()
        {
            double width = ChaosCanvas.ActualWidth;
            double height = ChaosCanvas.ActualHeight;

            if (width <= 0 || height <= 0) return;

            double r = ParameterR.Value;
            double x1 = InitialCondition.Value;
            double x2 = InitialCondition.Value + 0.000001; // Pequena diferença

            int iterations = 50;
            List<Point> trajectory1 = new List<Point>();
            List<Point> trajectory2 = new List<Point>();

            // Calcular trajetórias
            for (int i = 0; i < iterations; i++)
            {
                double px = (i / (double)iterations) * width;
                double py1 = height - (x1 * height);
                double py2 = height - (x2 * height);

                trajectory1.Add(new Point(px, py1));
                trajectory2.Add(new Point(px, py2));

                x1 = r * x1 * (1 - x1);
                x2 = r * x2 * (1 - x2);
            }

            // Desenhar trajetória 1 (azul)
            DrawTrajectory(trajectory1, Color.FromRgb(52, 152, 219), "Trajetória 1");

            // Desenhar trajetória 2 (vermelha)
            DrawTrajectory(trajectory2, Color.FromRgb(231, 76, 60), "Trajetória 2");

            // Adicionar legenda
            AddLegend();
        }

        private void DrawTrajectory(List<Point> points, Color color, string label)
        {
            for (int i = 1; i < points.Count; i++)
            {
                Line line = new Line
                {
                    X1 = points[i - 1].X,
                    Y1 = points[i - 1].Y,
                    X2 = points[i].X,
                    Y2 = points[i].Y,
                    Stroke = new SolidColorBrush(color),
                    StrokeThickness = 2
                };
                ChaosCanvas.Children.Add(line);
            }

            // Ponto final
            Ellipse finalDot = new Ellipse
            {
                Width = 6,
                Height = 6,
                Fill = new SolidColorBrush(color)
            };
            Canvas.SetLeft(finalDot, points[points.Count - 1].X - 3);
            Canvas.SetTop(finalDot, points[points.Count - 1].Y - 3);
            ChaosCanvas.Children.Add(finalDot);
        }

        private void AddLegend()
        {
            double legendX = 10;
            double legendY = 10;

            // Legenda Trajetória 1
            Rectangle rect1 = new Rectangle
            {
                Width = 20,
                Height = 3,
                Fill = new SolidColorBrush(Color.FromRgb(52, 152, 219))
            };
            Canvas.SetLeft(rect1, legendX);
            Canvas.SetTop(rect1, legendY);
            ChaosCanvas.Children.Add(rect1);

            TextBlock text1 = new TextBlock
            {
                Text = "x₀ = " + InitialCondition.Value.ToString("F6"),
                FontSize = 10,
                Foreground = Brushes.Black
            };
            Canvas.SetLeft(text1, legendX + 25);
            Canvas.SetTop(text1, legendY - 5);
            ChaosCanvas.Children.Add(text1);

            // Legenda Trajetória 2
            Rectangle rect2 = new Rectangle
            {
                Width = 20,
                Height = 3,
                Fill = new SolidColorBrush(Color.FromRgb(231, 76, 60))
            };
            Canvas.SetLeft(rect2, legendX);
            Canvas.SetTop(rect2, legendY + 20);
            ChaosCanvas.Children.Add(rect2);

            TextBlock text2 = new TextBlock
            {
                Text = "x₀ = " + (InitialCondition.Value + 0.000001).ToString("F6"),
                FontSize = 10,
                Foreground = Brushes.Black
            };
            Canvas.SetLeft(text2, legendX + 25);
            Canvas.SetTop(text2, legendY + 15);
            ChaosCanvas.Children.Add(text2);
        }
        #endregion

        #region Conjunto de Mandelbrot
        private void DrawMandelbrotSet()
        {
            double width = ChaosCanvas.ActualWidth;
            double height = ChaosCanvas.ActualHeight;

            if (width <= 0 || height <= 0) return;

            int maxIterations = 50;
            double xMin = -2.5, xMax = 1.0;
            double yMin = -1.25, yMax = 1.25;

            int pixelSize = 3; // Para acelerar o desenho

            for (int px = 0; px < width; px += pixelSize)
            {
                for (int py = 0; py < height; py += pixelSize)
                {
                    // Mapear pixel para coordenada complexa
                    double x0 = xMin + (px / width) * (xMax - xMin);
                    double y0 = yMin + (py / height) * (yMax - yMin);

                    double x = 0, y = 0;
                    int iteration = 0;

                    while (x * x + y * y <= 4 && iteration < maxIterations)
                    {
                        double xTemp = x * x - y * y + x0;
                        y = 2 * x * y + y0;
                        x = xTemp;
                        iteration++;
                    }

                    // Colorir baseado no número de iterações
                    Color color;
                    if (iteration == maxIterations)
                    {
                        color = Colors.Black; // Pertence ao conjunto
                    }
                    else
                    {
                        // Gradiente de cor
                        byte intensity = (byte)((iteration * 255) / maxIterations);
                        color = Color.FromRgb(intensity, (byte)(intensity / 2), (byte)(255 - intensity));
                    }

                    Rectangle pixel = new Rectangle
                    {
                        Width = pixelSize,
                        Height = pixelSize,
                        Fill = new SolidColorBrush(color)
                    };

                    Canvas.SetLeft(pixel, px);
                    Canvas.SetTop(pixel, py);
                    ChaosCanvas.Children.Add(pixel);
                }
            }
        }
        #endregion

        #region Utilitários
        private void AddAxisLabels(string xLabel, string yLabel, string xMin, string xMax, string yMin, string yMax)
        {
            double width = ChaosCanvas.ActualWidth;
            double height = ChaosCanvas.ActualHeight;

            // Label eixo X
            TextBlock xLabelText = new TextBlock
            {
                Text = xLabel,
                FontSize = 12,
                FontWeight = FontWeights.Bold,
                Foreground = Brushes.Black
            };
            Canvas.SetLeft(xLabelText, width / 2);
            Canvas.SetTop(xLabelText, height - 20);
            ChaosCanvas.Children.Add(xLabelText);

            // Labels mínimo e máximo X
            TextBlock xMinText = new TextBlock { Text = xMin, FontSize = 10, Foreground = Brushes.Gray };
            Canvas.SetLeft(xMinText, 5);
            Canvas.SetTop(xMinText, height - 15);
            ChaosCanvas.Children.Add(xMinText);

            TextBlock xMaxText = new TextBlock { Text = xMax, FontSize = 10, Foreground = Brushes.Gray };
            Canvas.SetLeft(xMaxText, width - 25);
            Canvas.SetTop(xMaxText, height - 15);
            ChaosCanvas.Children.Add(xMaxText);

            // Label eixo Y
            TextBlock yLabelText = new TextBlock
            {
                Text = yLabel,
                FontSize = 12,
                FontWeight = FontWeights.Bold,
                Foreground = Brushes.Black
            };
            Canvas.SetLeft(yLabelText, 5);
            Canvas.SetTop(yLabelText, height / 2);
            ChaosCanvas.Children.Add(yLabelText);

            // Labels mínimo e máximo Y
            TextBlock yMinText = new TextBlock { Text = yMin, FontSize = 10, Foreground = Brushes.Gray };
            Canvas.SetLeft(yMinText, 5);
            Canvas.SetTop(yMinText, height - 30);
            ChaosCanvas.Children.Add(yMinText);

            TextBlock yMaxText = new TextBlock { Text = yMax, FontSize = 10, Foreground = Brushes.Gray };
            Canvas.SetLeft(yMaxText, 5);
            Canvas.SetTop(yMaxText, 5);
            ChaosCanvas.Children.Add(yMaxText);
        }
        #endregion

        #region Botões de Navegação
        private void ExercisesButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Exercícios sobre Teoria do Caos:\n\n" +
                          "1. Calcule o expoente de Lyapunov para o mapa logístico com r=3.8\n" +
                          "2. Determine os pontos fixos do sistema de Lorenz\n" +
                          "3. Investigue a dimensão fractal do conjunto de Mandelbrot\n" +
                          "4. Compare trajetórias com diferentes condições iniciais",
                          "Exercícios", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void QuizButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Quiz - Teoria do Caos:\n\n" +
                          "1. O que é o Efeito Borboleta?\n" +
                          "2. Qual a diferença entre caos e aleatoriedade?\n" +
                          "3. O que é um atrator estranho?\n" +
                          "4. Como identificar um sistema caótico?\n" +
                          "5. O que são bifurcações?",
                          "Quiz", MessageBoxButton.OK, MessageBoxImage.Question);
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        #endregion
    }
}