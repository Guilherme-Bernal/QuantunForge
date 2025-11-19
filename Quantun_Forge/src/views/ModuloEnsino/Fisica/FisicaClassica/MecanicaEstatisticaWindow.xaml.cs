using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Quantun_Forge.src.views.ModuloEnsino.Fisica.FisicaClassica
{
    public partial class MecanicaEstatisticaWindow : Window
    {
        private int currentSimulation = 0;
        private const double kB = 1.38e-23; // Constante de Boltzmann (J/K)
        private const double eV_to_J = 1.602e-19; // Conversão eV para Joules
        private bool isInitialized = false; // Flag para controlar inicialização

        public MecanicaEstatisticaWindow()
        {
            InitializeComponent();
            isInitialized = true; // Marca como inicializado
            UpdateSimulationInfo();
        }

        private void StatisticsSelector_Changed(object sender, SelectionChangedEventArgs e)
        {
            // CORREÇÃO: Verificar se está inicializado E se os controles não são nulos
            if (!isInitialized || StatisticsSelector == null) return;

            currentSimulation = StatisticsSelector.SelectedIndex;
            UpdateSimulationInfo();

            // CORREÇÃO: Verificar se Canvas não é nulo antes de limpar
            if (StatisticsCanvas != null)
            {
                StatisticsCanvas.Children.Clear();
            }
        }

        private void UpdateSimulationInfo()
        {
            // CORREÇÃO: Verificar TODOS os controles necessários
            if (CurrentSimulationInfo == null || ObservationText == null ||
                TemperaturePanel == null || ParticlesPanel == null ||
                ChemicalPotentialPanel == null) return;

            switch (currentSimulation)
            {
                case 0: // Maxwell-Boltzmann
                    CurrentSimulationInfo.Text = "Distribuição de Maxwell-Boltzmann: descreve a distribuição de velocidades " +
                                                "de partículas clássicas em equilíbrio térmico. Válida para gases ideais a altas temperaturas.";
                    ObservationText.Text = "Observe como a distribuição muda com a temperatura. Em altas temperaturas, " +
                                          "mais partículas têm energias elevadas. O pico se desloca para energias maiores.";
                    TemperaturePanel.Visibility = Visibility.Visible;
                    ParticlesPanel.Visibility = Visibility.Collapsed;
                    ChemicalPotentialPanel.Visibility = Visibility.Collapsed;
                    break;

                case 1: // Bose-Einstein
                    CurrentSimulationInfo.Text = "Distribuição de Bose-Einstein: válida para bósons (partículas de spin inteiro). " +
                                                "Permite múltiplas partículas no mesmo estado quântico. Fenômeno: Condensação de BE.";
                    ObservationText.Text = "Em baixas temperaturas, ocorre a condensação de Bose-Einstein: uma fração macroscópica " +
                                          "das partículas ocupa o estado fundamental. Compare com Maxwell-Boltzmann!";
                    TemperaturePanel.Visibility = Visibility.Visible;
                    ParticlesPanel.Visibility = Visibility.Collapsed;
                    ChemicalPotentialPanel.Visibility = Visibility.Visible;
                    break;

                case 2: // Fermi-Dirac
                    CurrentSimulationInfo.Text = "Distribuição de Fermi-Dirac: válida para férmions (spin semi-inteiro). " +
                                                "Princípio de Exclusão de Pauli: máximo 1 partícula por estado. Forma o 'Mar de Fermi'.";
                    ObservationText.Text = "A T=0, todos os estados até a energia de Fermi estão ocupados. A função de " +
                                          "distribuição varia suavemente em torno de μ (potencial químico).";
                    TemperaturePanel.Visibility = Visibility.Visible;
                    ParticlesPanel.Visibility = Visibility.Collapsed;
                    ChemicalPotentialPanel.Visibility = Visibility.Visible;
                    break;

                case 3: // Modelo de Ising
                    CurrentSimulationInfo.Text = "Modelo de Ising 2D: sistema de spins em uma rede quadrada. " +
                                                "Mostra transição de fase ferromagnética. Sistema paradigmático da mecânica estatística.";
                    ObservationText.Text = "Abaixo da temperatura crítica (Tc ≈ 2.27 J/kB), os spins se alinham espontaneamente " +
                                          "(ordem). Acima de Tc, desordem térmica prevalece (paramagnetismo).";
                    TemperaturePanel.Visibility = Visibility.Visible;
                    ParticlesPanel.Visibility = Visibility.Collapsed;
                    ChemicalPotentialPanel.Visibility = Visibility.Collapsed;
                    break;

                case 4: // Diagrama de Fase
                    CurrentSimulationInfo.Text = "Diagrama de Fase Pressão-Temperatura: mostra as regiões de estabilidade " +
                                                "das diferentes fases (sólido, líquido, gás). Ponto triplo e ponto crítico.";
                    ObservationText.Text = "O ponto triplo é onde as três fases coexistem. O ponto crítico marca o fim " +
                                          "da distinção entre líquido e gás. Acima dele, temos um fluido supercrítico.";
                    TemperaturePanel.Visibility = Visibility.Collapsed;
                    ParticlesPanel.Visibility = Visibility.Collapsed;
                    ChemicalPotentialPanel.Visibility = Visibility.Collapsed;
                    break;
            }
        }

        private void Parameter_Changed(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            // CORREÇÃO: Verificar cada controle antes de usar
            if (TemperatureValue != null && TemperatureSlider != null)
                TemperatureValue.Text = TemperatureSlider.Value.ToString("F0") + " K";

            if (ParticlesValue != null && ParticlesSlider != null)
                ParticlesValue.Text = ParticlesSlider.Value.ToString("F0");

            if (ChemicalPotentialValue != null && ChemicalPotentialSlider != null)
                ChemicalPotentialValue.Text = ChemicalPotentialSlider.Value.ToString("F2") + " eV";
        }

        private void Simulate_Click(object sender, RoutedEventArgs e)
        {
            // CORREÇÃO: Verificar se Canvas existe
            if (StatisticsCanvas == null) return;

            StatisticsCanvas.Children.Clear();

            switch (currentSimulation)
            {
                case 0:
                    DrawMaxwellBoltzmannDistribution();
                    break;
                case 1:
                    DrawBoseEinsteinDistribution();
                    break;
                case 2:
                    DrawFermiDiracDistribution();
                    break;
                case 3:
                    DrawIsingModel();
                    break;
                case 4:
                    DrawPhaseDiagram();
                    break;
            }
        }

        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            // CORREÇÃO: Verificar se Canvas existe
            if (StatisticsCanvas != null)
            {
                StatisticsCanvas.Children.Clear();
            }
        }

        #region Maxwell-Boltzmann
        private void DrawMaxwellBoltzmannDistribution()
        {
            if (StatisticsCanvas == null || TemperatureSlider == null) return;

            double width = StatisticsCanvas.ActualWidth;
            double height = StatisticsCanvas.ActualHeight;

            if (width <= 0 || height <= 0) return;

            double T = TemperatureSlider.Value;
            double m = 4.65e-26; // Massa de uma molécula de N2 (kg)

            List<Point> points = new List<Point>();

            double maxSpeed = 1500; // m/s
            double maxProb = 0;

            // Calcular pontos e encontrar máximo
            List<double> probabilities = new List<double>();
            for (double v = 0; v <= maxSpeed; v += 5)
            {
                double prob = MaxwellBoltzmannSpeed(v, T, m);
                probabilities.Add(prob);
                if (prob > maxProb) maxProb = prob;
            }

            // Plotar distribuição
            int index = 0;
            for (double v = 0; v <= maxSpeed; v += 5)
            {
                double prob = probabilities[index++];
                double x = (v / maxSpeed) * width;
                double y = height - (prob / maxProb) * (height - 40);

                points.Add(new Point(x, y));
            }

            // Desenhar curva
            DrawCurve(points, Color.FromRgb(155, 89, 182), 2);

            // Velocidade média
            double vAvg = Math.Sqrt(8 * kB * T / (Math.PI * m));
            double xAvg = (vAvg / maxSpeed) * width;
            DrawVerticalLine(xAvg, height, Colors.Red, "v̄", true);

            // Velocidade mais provável
            double vMostProb = Math.Sqrt(2 * kB * T / m);
            double xMostProb = (vMostProb / maxSpeed) * width;
            DrawVerticalLine(xMostProb, height, Colors.Green, "v_mp", false);

            // Eixos
            AddAxisLabels("Velocidade (m/s)", "Probabilidade", "0", maxSpeed.ToString("F0"), "", "");
        }

        private double MaxwellBoltzmannSpeed(double v, double T, double m)
        {
            double factor = m / (2 * Math.PI * kB * T);
            return 4 * Math.PI * v * v * Math.Pow(factor, 1.5) * Math.Exp(-m * v * v / (2 * kB * T));
        }
        #endregion

        #region Bose-Einstein
        private void DrawBoseEinsteinDistribution()
        {
            if (StatisticsCanvas == null || TemperatureSlider == null || ChemicalPotentialSlider == null) return;

            double width = StatisticsCanvas.ActualWidth;
            double height = StatisticsCanvas.ActualHeight;

            if (width <= 0 || height <= 0) return;

            double T = TemperatureSlider.Value;
            double mu = ChemicalPotentialSlider.Value * eV_to_J / kB / T; // Normalizado por kBT

            List<Point> points = new List<Point>();

            double maxEnergy = 10; // Em unidades de kBT
            double maxOccupation = 0;

            // Calcular pontos
            List<double> occupations = new List<double>();
            for (double E = 0; E <= maxEnergy; E += 0.1)
            {
                double occupation = BoseEinsteinOccupation(E, mu);
                occupations.Add(occupation);
                if (occupation > maxOccupation && occupation < 100) maxOccupation = occupation;
            }

            if (maxOccupation == 0) maxOccupation = 1;

            // Plotar
            int index = 0;
            for (double E = 0; E <= maxEnergy; E += 0.1)
            {
                double occupation = occupations[index++];
                if (occupation > 100) occupation = 100; // Limitar

                double x = (E / maxEnergy) * width;
                double y = height - (occupation / maxOccupation) * (height - 40);

                points.Add(new Point(x, y));
            }

            DrawCurve(points, Color.FromRgb(52, 152, 219), 2);

            // Potencial químico
            if (mu >= 0 && mu <= maxEnergy)
            {
                double xMu = (mu / maxEnergy) * width;
                DrawVerticalLine(xMu, height, Colors.Purple, "μ", true);
            }

            AddAxisLabels("Energia (k_B T)", "Ocupação", "0", maxEnergy.ToString("F0"), "", "");
        }

        private double BoseEinsteinOccupation(double E, double mu)
        {
            double exponent = E - mu;
            if (exponent <= 0.01) return 100; // Condensação
            return 1.0 / (Math.Exp(exponent) - 1);
        }
        #endregion

        #region Fermi-Dirac
        private void DrawFermiDiracDistribution()
        {
            if (StatisticsCanvas == null || TemperatureSlider == null || ChemicalPotentialSlider == null) return;

            double width = StatisticsCanvas.ActualWidth;
            double height = StatisticsCanvas.ActualHeight;

            if (width <= 0 || height <= 0) return;

            double T = TemperatureSlider.Value;
            double mu = ChemicalPotentialSlider.Value * eV_to_J / kB / T; // Normalizado

            List<Point> points = new List<Point>();

            double maxEnergy = 10; // Em unidades de kBT

            // Calcular e plotar
            for (double E = 0; E <= maxEnergy; E += 0.1)
            {
                double occupation = FermiDiracOccupation(E, mu);

                double x = (E / maxEnergy) * width;
                double y = height - occupation * (height - 40);

                points.Add(new Point(x, y));
            }

            DrawCurve(points, Color.FromRgb(231, 76, 60), 2);

            // Energia de Fermi (μ)
            if (mu >= 0 && mu <= maxEnergy)
            {
                double xMu = (mu / maxEnergy) * width;
                DrawVerticalLine(xMu, height, Colors.DarkBlue, "E_F", true);
            }

            AddAxisLabels("Energia (k_B T)", "Ocupação (0-1)", "0", maxEnergy.ToString("F0"), "0", "1");
        }

        private double FermiDiracOccupation(double E, double mu)
        {
            double exponent = E - mu;
            return 1.0 / (Math.Exp(exponent) + 1);
        }
        #endregion

        #region Modelo de Ising
        private void DrawIsingModel()
        {
            if (StatisticsCanvas == null || TemperatureSlider == null) return;

            double width = StatisticsCanvas.ActualWidth;
            double height = StatisticsCanvas.ActualHeight;

            if (width <= 0 || height <= 0) return;

            double T = TemperatureSlider.Value;
            double Tc = 500; // Temperatura crítica aproximada em K
            double J = 1.0; // Energia de acoplamento

            int gridSize = 40;
            double cellSize = Math.Min(width, height) / gridSize;

            // Criar rede de spins
            int[,] spins = new int[gridSize, gridSize];
            Random rand = new Random();

            // Inicializar spins aleatoriamente ou ordenados
            if (T > Tc)
            {
                // Fase desordenada
                for (int i = 0; i < gridSize; i++)
                {
                    for (int j = 0; j < gridSize; j++)
                    {
                        spins[i, j] = rand.NextDouble() > 0.5 ? 1 : -1;
                    }
                }
            }
            else
            {
                // Fase ordenada (com pequena desordem)
                int dominantSpin = 1;
                for (int i = 0; i < gridSize; i++)
                {
                    for (int j = 0; j < gridSize; j++)
                    {
                        spins[i, j] = rand.NextDouble() > 0.1 ? dominantSpin : -dominantSpin;
                    }
                }
            }

            // Aplicar algumas iterações de Monte Carlo
            int iterations = 500;
            for (int iter = 0; iter < iterations; iter++)
            {
                int i = rand.Next(gridSize);
                int j = rand.Next(gridSize);

                // Calcular energia de flip
                int neighbors = 0;
                if (i > 0) neighbors += spins[i - 1, j];
                if (i < gridSize - 1) neighbors += spins[i + 1, j];
                if (j > 0) neighbors += spins[i, j - 1];
                if (j < gridSize - 1) neighbors += spins[i, j + 1];

                double deltaE = 2 * J * spins[i, j] * neighbors;

                // Aceitar ou rejeitar flip
                if (deltaE < 0 || rand.NextDouble() < Math.Exp(-deltaE / T))
                {
                    spins[i, j] *= -1;
                }
            }

            // Desenhar rede
            for (int i = 0; i < gridSize; i++)
            {
                for (int j = 0; j < gridSize; j++)
                {
                    Rectangle cell = new Rectangle
                    {
                        Width = cellSize,
                        Height = cellSize,
                        Fill = spins[i, j] == 1 ? Brushes.Blue : Brushes.Red
                    };

                    Canvas.SetLeft(cell, j * cellSize);
                    Canvas.SetTop(cell, i * cellSize);
                    StatisticsCanvas.Children.Add(cell);
                }
            }

            // Calcular magnetização
            int totalSpin = 0;
            for (int i = 0; i < gridSize; i++)
            {
                for (int j = 0; j < gridSize; j++)
                {
                    totalSpin += spins[i, j];
                }
            }

            double magnetization = Math.Abs(totalSpin) / (double)(gridSize * gridSize);

            // Mostrar informação
            TextBlock info = new TextBlock
            {
                Text = $"T = {T:F0} K | Tc ≈ {Tc} K\nMagnetização: {magnetization:F2}",
                FontSize = 11,
                FontWeight = FontWeights.Bold,
                Foreground = Brushes.Black,
                Background = new SolidColorBrush(Color.FromArgb(200, 255, 255, 255)),
                Padding = new Thickness(8)
            };

            Canvas.SetLeft(info, 10);
            Canvas.SetTop(info, 10);
            StatisticsCanvas.Children.Add(info);

            // Legenda
            AddLegendBox(width - 100, height - 60, "Spin +1", Colors.Blue);
            AddLegendBox(width - 100, height - 35, "Spin -1", Colors.Red);
        }

        private void AddLegendBox(double x, double y, string text, Color color)
        {
            if (StatisticsCanvas == null) return;

            Rectangle rect = new Rectangle
            {
                Width = 15,
                Height = 15,
                Fill = new SolidColorBrush(color)
            };
            Canvas.SetLeft(rect, x);
            Canvas.SetTop(rect, y);
            StatisticsCanvas.Children.Add(rect);

            TextBlock label = new TextBlock
            {
                Text = text,
                FontSize = 10,
                Foreground = Brushes.Black
            };
            Canvas.SetLeft(label, x + 20);
            Canvas.SetTop(label, y);
            StatisticsCanvas.Children.Add(label);
        }
        #endregion

        #region Diagrama de Fase
        private void DrawPhaseDiagram()
        {
            if (StatisticsCanvas == null) return;

            double width = StatisticsCanvas.ActualWidth;
            double height = StatisticsCanvas.ActualHeight;

            if (width <= 0 || height <= 0) return;

            // Eixos
            double margin = 50;
            double plotWidth = width - 2 * margin;
            double plotHeight = height - 2 * margin;

            // Fundo
            Rectangle background = new Rectangle
            {
                Width = width,
                Height = height,
                Fill = Brushes.White
            };
            StatisticsCanvas.Children.Add(background);

            // Eixos X e Y
            Line xAxis = new Line
            {
                X1 = margin,
                Y1 = height - margin,
                X2 = width - margin,
                Y2 = height - margin,
                Stroke = Brushes.Black,
                StrokeThickness = 2
            };
            StatisticsCanvas.Children.Add(xAxis);

            Line yAxis = new Line
            {
                X1 = margin,
                Y1 = margin,
                X2 = margin,
                Y2 = height - margin,
                Stroke = Brushes.Black,
                StrokeThickness = 2
            };
            StatisticsCanvas.Children.Add(yAxis);

            // Curva de sublimação (Sólido-Gás)
            List<Point> sublimation = new List<Point>();
            for (double t = 0.1; t <= 0.4; t += 0.01)
            {
                double p = Math.Exp(10 * t - 2);
                double x = margin + t * plotWidth;
                double y = height - margin - (p / 10) * plotHeight;
                if (y >= margin && y <= height - margin)
                    sublimation.Add(new Point(x, y));
            }
            DrawCurve(sublimation, Colors.Blue, 2);

            // Curva de fusão (Sólido-Líquido)
            List<Point> melting = new List<Point>();
            for (double t = 0.4; t <= 0.9; t += 0.01)
            {
                double p = 2 + 8 * (t - 0.4);
                double x = margin + t * plotWidth;
                double y = height - margin - (p / 10) * plotHeight;
                if (y >= margin && y <= height - margin)
                    melting.Add(new Point(x, y));
            }
            DrawCurve(melting, Colors.Purple, 2);

            // Curva de vaporização (Líquido-Gás)
            List<Point> vaporization = new List<Point>();
            for (double t = 0.4; t <= 0.85; t += 0.01)
            {
                double p = Math.Exp(8 * (t - 0.3)) * 0.3;
                if (p > 10) break;
                double x = margin + t * plotWidth;
                double y = height - margin - (p / 10) * plotHeight;
                if (y >= margin && y <= height - margin)
                    vaporization.Add(new Point(x, y));
            }
            DrawCurve(vaporization, Colors.Red, 2);

            // Regiões
            AddRegionLabel(margin + 0.2 * plotWidth, margin + 0.3 * plotHeight, "SÓLIDO", Colors.LightBlue);
            AddRegionLabel(margin + 0.6 * plotWidth, margin + 0.3 * plotHeight, "LÍQUIDO", Colors.LightCyan);
            AddRegionLabel(margin + 0.6 * plotWidth, height - margin - 0.2 * plotHeight, "GÁS", Colors.LightYellow);

            // Pontos especiais
            double tripleX = margin + 0.4 * plotWidth;
            double tripleY = height - margin - 0.2 * plotHeight;
            AddSpecialPoint(tripleX, tripleY, "Ponto Triplo", Colors.Green);

            double criticalX = margin + 0.85 * plotWidth;
            double criticalY = height - margin - 0.7 * plotHeight;
            AddSpecialPoint(criticalX, criticalY, "Ponto Crítico", Colors.DarkRed);

            // Labels dos eixos
            TextBlock xLabel = new TextBlock
            {
                Text = "Temperatura (K)",
                FontSize = 12,
                FontWeight = FontWeights.Bold,
                Foreground = Brushes.Black
            };
            Canvas.SetLeft(xLabel, width / 2 - 50);
            Canvas.SetTop(xLabel, height - 20);
            StatisticsCanvas.Children.Add(xLabel);

            TextBlock yLabel = new TextBlock
            {
                Text = "Pressão (atm)",
                FontSize = 12,
                FontWeight = FontWeights.Bold,
                Foreground = Brushes.Black
            };
            Canvas.SetLeft(yLabel, 5);
            Canvas.SetTop(yLabel, height / 2);
            StatisticsCanvas.Children.Add(yLabel);
        }

        private void AddRegionLabel(double x, double y, string text, Color color)
        {
            if (StatisticsCanvas == null) return;

            TextBlock label = new TextBlock
            {
                Text = text,
                FontSize = 14,
                FontWeight = FontWeights.Bold,
                Foreground = new SolidColorBrush(color),
                Background = new SolidColorBrush(Color.FromArgb(150, 255, 255, 255)),
                Padding = new Thickness(5)
            };
            Canvas.SetLeft(label, x);
            Canvas.SetTop(label, y);
            StatisticsCanvas.Children.Add(label);
        }

        private void AddSpecialPoint(double x, double y, string label, Color color)
        {
            if (StatisticsCanvas == null) return;

            Ellipse point = new Ellipse
            {
                Width = 8,
                Height = 8,
                Fill = new SolidColorBrush(color),
                Stroke = Brushes.Black,
                StrokeThickness = 2
            };
            Canvas.SetLeft(point, x - 4);
            Canvas.SetTop(point, y - 4);
            StatisticsCanvas.Children.Add(point);

            TextBlock text = new TextBlock
            {
                Text = label,
                FontSize = 10,
                FontWeight = FontWeights.Bold,
                Foreground = new SolidColorBrush(color),
                Background = new SolidColorBrush(Color.FromArgb(200, 255, 255, 255)),
                Padding = new Thickness(3)
            };
            Canvas.SetLeft(text, x + 10);
            Canvas.SetTop(text, y - 8);
            StatisticsCanvas.Children.Add(text);
        }
        #endregion

        #region Utilitários
        private void DrawCurve(List<Point> points, Color color, double thickness)
        {
            if (StatisticsCanvas == null) return;

            for (int i = 1; i < points.Count; i++)
            {
                Line line = new Line
                {
                    X1 = points[i - 1].X,
                    Y1 = points[i - 1].Y,
                    X2 = points[i].X,
                    Y2 = points[i].Y,
                    Stroke = new SolidColorBrush(color),
                    StrokeThickness = thickness
                };
                StatisticsCanvas.Children.Add(line);
            }
        }

        private void DrawVerticalLine(double x, double height, Color color, string label, bool showAbove)
        {
            if (StatisticsCanvas == null) return;

            Line line = new Line
            {
                X1 = x,
                Y1 = 0,
                X2 = x,
                Y2 = height,
                Stroke = new SolidColorBrush(color),
                StrokeThickness = 2,
                StrokeDashArray = new DoubleCollection(new double[] { 5, 3 })
            };
            StatisticsCanvas.Children.Add(line);

            TextBlock text = new TextBlock
            {
                Text = label,
                FontSize = 11,
                FontWeight = FontWeights.Bold,
                Foreground = new SolidColorBrush(color),
                Background = new SolidColorBrush(Color.FromArgb(180, 255, 255, 255)),
                Padding = new Thickness(3)
            };

            Canvas.SetLeft(text, x + 5);
            Canvas.SetTop(text, showAbove ? 10 : 30);
            StatisticsCanvas.Children.Add(text);
        }

        private void AddAxisLabels(string xLabel, string yLabel, string xMin, string xMax, string yMin, string yMax)
        {
            if (StatisticsCanvas == null) return;

            double width = StatisticsCanvas.ActualWidth;
            double height = StatisticsCanvas.ActualHeight;

            // Label eixo X
            TextBlock xLabelText = new TextBlock
            {
                Text = xLabel,
                FontSize = 11,
                FontWeight = FontWeights.Bold,
                Foreground = Brushes.Black
            };
            Canvas.SetLeft(xLabelText, width / 2 - 40);
            Canvas.SetTop(xLabelText, height - 20);
            StatisticsCanvas.Children.Add(xLabelText);

            // Labels mínimo e máximo X
            if (!string.IsNullOrEmpty(xMin))
            {
                TextBlock xMinText = new TextBlock { Text = xMin, FontSize = 9, Foreground = Brushes.Gray };
                Canvas.SetLeft(xMinText, 5);
                Canvas.SetTop(xMinText, height - 15);
                StatisticsCanvas.Children.Add(xMinText);
            }

            if (!string.IsNullOrEmpty(xMax))
            {
                TextBlock xMaxText = new TextBlock { Text = xMax, FontSize = 9, Foreground = Brushes.Gray };
                Canvas.SetLeft(xMaxText, width - 35);
                Canvas.SetTop(xMaxText, height - 15);
                StatisticsCanvas.Children.Add(xMaxText);
            }

            // Label eixo Y
            TextBlock yLabelText = new TextBlock
            {
                Text = yLabel,
                FontSize = 11,
                FontWeight = FontWeights.Bold,
                Foreground = Brushes.Black
            };
            Canvas.SetLeft(yLabelText, 5);
            Canvas.SetTop(yLabelText, 5);
            StatisticsCanvas.Children.Add(yLabelText);

            // Labels mínimo e máximo Y
            if (!string.IsNullOrEmpty(yMin))
            {
                TextBlock yMinText = new TextBlock { Text = yMin, FontSize = 9, Foreground = Brushes.Gray };
                Canvas.SetLeft(yMinText, 5);
                Canvas.SetTop(yMinText, height - 30);
                StatisticsCanvas.Children.Add(yMinText);
            }

            if (!string.IsNullOrEmpty(yMax))
            {
                TextBlock yMaxText = new TextBlock { Text = yMax, FontSize = 9, Foreground = Brushes.Gray };
                Canvas.SetLeft(yMaxText, 5);
                Canvas.SetTop(yMaxText, 25);
                StatisticsCanvas.Children.Add(yMaxText);
            }
        }
        #endregion

        #region Botões de Navegação
        private void ExercisesButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Exercícios sobre Mecânica Estatística:\n\n" +
                          "1. Calcule a entropia de Boltzmann para um sistema com Ω = 10²³ microestados\n" +
                          "2. Derive a função de partição para um oscilador harmônico quântico\n" +
                          "3. Compare as distribuições MB, BE e FD para T = 300 K\n" +
                          "4. Calcule a temperatura crítica do modelo de Ising 2D\n" +
                          "5. Determine a energia média usando o teorema de equipartição",
                          "Exercícios", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void QuizButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Quiz - Mecânica Estatística:\n\n" +
                          "1. O que significa S = k_B ln Ω?\n" +
                          "2. Qual a diferença entre bósons e férmions?\n" +
                          "3. O que é a função de partição?\n" +
                          "4. Explique o princípio de exclusão de Pauli\n" +
                          "5. O que acontece no ponto crítico?\n" +
                          "6. Qual o significado físico do potencial químico?",
                          "Quiz", MessageBoxButton.OK, MessageBoxImage.Question);
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        #endregion
    }
}