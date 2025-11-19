using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Quantun_Forge.src.views.ModuloEnsino.Fisica.FisicaClassica
{
    public partial class RelatividadeWindow : Window
    {
        private int currentSimulation = 0;
        private const double G = 6.674e-11; // Constante gravitacional (m³/(kg·s²))
        private const double c = 299792458; // Velocidade da luz (m/s)
        private const double M_sun = 1.989e30; // Massa solar (kg)
        private bool isInitialized = false; // Flag de inicialização

        public RelatividadeWindow()
        {
            InitializeComponent();
            isInitialized = true; // Marca como inicializado
            UpdateSimulationInfo();
        }

        private void RelativitySelector_Changed(object sender, SelectionChangedEventArgs e)
        {
            // CORREÇÃO: Verificar inicialização e controles
            if (!isInitialized || RelativitySelector == null) return;

            currentSimulation = RelativitySelector.SelectedIndex;
            UpdateSimulationInfo();

            // CORREÇÃO: Verificar Canvas antes de limpar
            if (RelativityCanvas != null)
            {
                RelativityCanvas.Children.Clear();
            }
        }

        private void UpdateSimulationInfo()
        {
            // CORREÇÃO: Verificar TODOS os controles necessários
            if (CurrentSimulationInfo == null || ObservationText == null ||
                MassPanel == null || DistancePanel == null || VelocityPanel == null) return;

            switch (currentSimulation)
            {
                case 0: // Curvatura do Espaço-Tempo
                    CurrentSimulationInfo.Text = "Curvatura do Espaço-Tempo: visualização da deformação da geometria " +
                                                "causada por uma massa. Linhas de grade representam o espaço-tempo curvo.";
                    ObservationText.Text = "Quanto maior a massa, maior a curvatura. Próximo ao horizonte de eventos, " +
                                          "a curvatura se torna extrema. O espaço-tempo é literalmente 'puxado' pela gravidade.";
                    MassPanel.Visibility = Visibility.Visible;
                    DistancePanel.Visibility = Visibility.Collapsed;
                    VelocityPanel.Visibility = Visibility.Collapsed;
                    break;

                case 1: // Dilatação do Tempo
                    CurrentSimulationInfo.Text = "Dilatação do Tempo Gravitacional: mostra como o tempo passa mais devagar " +
                                                "próximo a campos gravitacionais intensos. Efeito medido em GPS!";
                    ObservationText.Text = "No horizonte de eventos, o tempo 'congela' para um observador distante. " +
                                          "Esta é a razão pela qual GPS precisa de correções relativísticas (+38 μs/dia).";
                    MassPanel.Visibility = Visibility.Visible;
                    DistancePanel.Visibility = Visibility.Visible;
                    VelocityPanel.Visibility = Visibility.Collapsed;
                    break;

                case 2: // Deflexão da Luz
                    CurrentSimulationInfo.Text = "Deflexão da Luz (Lente Gravitacional): massas desviam a trajetória da luz. " +
                                                "Primeira confirmação experimental da Relatividade Geral em 1919!";
                    ObservationText.Text = "Quanto mais próximo do objeto massivo, maior o desvio. Galáxias podem criar " +
                                          "'Anéis de Einstein' quando perfeitamente alinhadas com uma fonte distante.";
                    MassPanel.Visibility = Visibility.Visible;
                    DistancePanel.Visibility = Visibility.Collapsed;
                    VelocityPanel.Visibility = Visibility.Visible;

                    // CORREÇÃO: Verificar se Children existe antes de acessar
                    if (VelocityPanel.Children.Count > 0 && VelocityPanel.Children[0] is TextBlock)
                    {
                        ((TextBlock)VelocityPanel.Children[0]).Text = "Parâmetro de Impacto:";
                    }
                    break;

                case 3: // Órbitas
                    CurrentSimulationInfo.Text = "Órbitas ao Redor de Buraco Negro: trajetórias de partículas no espaço-tempo " +
                                                "curvo. ISCO (órbita circular mais interna estável) em r = 3r_s.";
                    ObservationText.Text = "Órbitas muito próximas são instáveis. Entre 1.5r_s e 3r_s existem órbitas circulares " +
                                          "instáveis. Fótons orbitam em r = 1.5r_s formando a 'esfera de fótons'.";
                    MassPanel.Visibility = Visibility.Visible;
                    DistancePanel.Visibility = Visibility.Visible;
                    VelocityPanel.Visibility = Visibility.Collapsed;
                    break;

                case 4: // Ondas Gravitacionais
                    CurrentSimulationInfo.Text = "Ondas Gravitacionais: ondulações no espaço-tempo propagando à velocidade da luz. " +
                                                "Detectadas pela primeira vez em 2015 (GW150914).";
                    ObservationText.Text = "Ondas gravitacionais esticam e comprimem o espaço alternadamente. A detecção " +
                                          "requer sensibilidade extrema: deslocamentos de 10⁻²¹ metros!";
                    MassPanel.Visibility = Visibility.Collapsed;
                    DistancePanel.Visibility = Visibility.Collapsed;
                    VelocityPanel.Visibility = Visibility.Collapsed;
                    break;
            }
        }

        private void Parameter_Changed(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            // CORREÇÃO: Verificar cada controle antes de usar
            if (MassValue != null && MassSlider != null)
                MassValue.Text = MassSlider.Value.ToString("F0") + " M☉";

            if (DistanceValue != null && DistanceSlider != null)
                DistanceValue.Text = DistanceSlider.Value.ToString("F1") + " r_s";

            if (VelocityValue != null && VelocitySlider != null)
                VelocityValue.Text = VelocitySlider.Value.ToString("F1");
        }

        private void Simulate_Click(object sender, RoutedEventArgs e)
        {
            // CORREÇÃO: Verificar Canvas antes de limpar
            if (RelativityCanvas == null) return;

            RelativityCanvas.Children.Clear();

            switch (currentSimulation)
            {
                case 0:
                    DrawSpacetimeCurvature();
                    break;
                case 1:
                    DrawTimeDilation();
                    break;
                case 2:
                    DrawLightDeflection();
                    break;
                case 3:
                    DrawOrbits();
                    break;
                case 4:
                    DrawGravitationalWaves();
                    break;
            }
        }

        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            // CORREÇÃO: Verificar Canvas antes de limpar
            if (RelativityCanvas != null)
            {
                RelativityCanvas.Children.Clear();
            }
        }

        #region Curvatura do Espaço-Tempo
        private void DrawSpacetimeCurvature()
        {
            if (RelativityCanvas == null || MassSlider == null) return;

            double width = RelativityCanvas.ActualWidth;
            double height = RelativityCanvas.ActualHeight;

            if (width <= 0 || height <= 0) return;

            double mass = MassSlider.Value;
            double centerX = width / 2;
            double centerY = height / 2;

            // Raio de Schwarzschild normalizado para visualização
            double r_s_visual = mass * 3;

            // Desenhar grade curvada
            int gridLines = 15;
            for (int i = 0; i < gridLines; i++)
            {
                double y = (i / (double)(gridLines - 1)) * height;

                List<Point> linePoints = new List<Point>();
                for (double x = 0; x <= width; x += 5)
                {
                    double dx = x - centerX;
                    double dy = y - centerY;
                    double distance = Math.Sqrt(dx * dx + dy * dy);

                    // Calcular curvatura (aproximação)
                    double curvature = 0;
                    if (distance > r_s_visual / 2)
                    {
                        curvature = (r_s_visual * r_s_visual) / (distance * distance) * 30;
                    }
                    else
                    {
                        curvature = 60; // Máxima curvatura
                    }

                    double newY = y + curvature;
                    linePoints.Add(new Point(x, newY));
                }

                DrawCurve(linePoints, Color.FromRgb(189, 195, 199), 1);
            }

            // Linhas verticais
            for (int i = 0; i < gridLines; i++)
            {
                double x = (i / (double)(gridLines - 1)) * width;

                List<Point> linePoints = new List<Point>();
                for (double y = 0; y <= height; y += 5)
                {
                    double dx = x - centerX;
                    double dy = y - centerY;
                    double distance = Math.Sqrt(dx * dx + dy * dy);

                    double curvature = 0;
                    if (distance > r_s_visual / 2)
                    {
                        curvature = (r_s_visual * r_s_visual) / (distance * distance) * 30;
                    }
                    else
                    {
                        curvature = 60;
                    }

                    double newY = y + curvature;
                    linePoints.Add(new Point(x, newY));
                }

                DrawCurve(linePoints, Color.FromRgb(189, 195, 199), 1);
            }

            // Desenhar o objeto massivo (buraco negro ou estrela)
            Ellipse blackHole = new Ellipse
            {
                Width = r_s_visual,
                Height = r_s_visual,
                Fill = new RadialGradientBrush
                {
                    GradientStops = new GradientStopCollection
                    {
                        new GradientStop(Colors.Black, 0.0),
                        new GradientStop(Color.FromRgb(52, 152, 219), 0.7),
                        new GradientStop(Color.FromArgb(100, 52, 152, 219), 1.0)
                    }
                },
                Stroke = Brushes.DarkBlue,
                StrokeThickness = 2
            };

            Canvas.SetLeft(blackHole, centerX - r_s_visual / 2);
            Canvas.SetTop(blackHole, centerY - r_s_visual / 2);
            RelativityCanvas.Children.Add(blackHole);

            // Label
            AddLabel(centerX, centerY + r_s_visual / 2 + 20, $"{mass:F0} M☉", Colors.Black);
        }
        #endregion

        #region Dilatação do Tempo
        private void DrawTimeDilation()
        {
            if (RelativityCanvas == null || MassSlider == null || DistanceSlider == null) return;

            double width = RelativityCanvas.ActualWidth;
            double height = RelativityCanvas.ActualHeight;

            if (width <= 0 || height <= 0) return;

            double mass = MassSlider.Value;
            double r_ratio = DistanceSlider.Value; // Em unidades de r_s

            // Gráfico: Fator de dilatação vs distância
            List<Point> points = new List<Point>();

            double minR = 1.01; // Ligeiramente acima do horizonte
            double maxR = 10;

            for (double r = minR; r <= maxR; r += 0.05)
            {
                // Fator de dilatação: t_0 = t_infinity * sqrt(1 - r_s/r)
                double factor = Math.Sqrt(1 - 1.0 / r);

                double x = ((r - minR) / (maxR - minR)) * width;
                double y = height - (factor * (height - 40));

                points.Add(new Point(x, y));
            }

            DrawCurve(points, Color.FromRgb(231, 76, 60), 3);

            // Marcar posição atual
            double currentFactor = Math.Sqrt(1 - 1.0 / r_ratio);
            double currentX = ((r_ratio - minR) / (maxR - minR)) * width;
            double currentY = height - (currentFactor * (height - 40));

            Ellipse marker = new Ellipse
            {
                Width = 10,
                Height = 10,
                Fill = Brushes.Blue,
                Stroke = Brushes.DarkBlue,
                StrokeThickness = 2
            };

            Canvas.SetLeft(marker, currentX - 5);
            Canvas.SetTop(marker, currentY - 5);
            RelativityCanvas.Children.Add(marker);

            // Informações
            AddLabel(width / 2, 20, $"Fator de dilatação: {currentFactor:F3}", Colors.Black);
            AddLabel(width / 2, 40, $"1 segundo lá = {1 / currentFactor:F2} segundos aqui", Color.FromRgb(231, 76, 60));

            // Eixos
            AddAxisLabels("Distância (r_s)", "Fator de dilatação", "1", "10", "0", "1");

            // Linha do horizonte
            DrawVerticalDashedLine(0, height, Colors.Red, "Horizonte");
        }
        #endregion

        #region Deflexão da Luz
        private void DrawLightDeflection()
        {
            if (RelativityCanvas == null || MassSlider == null || VelocitySlider == null) return;

            double width = RelativityCanvas.ActualWidth;
            double height = RelativityCanvas.ActualHeight;

            if (width <= 0 || height <= 0) return;

            double mass = MassSlider.Value;
            double impactParam = VelocitySlider.Value;

            double centerX = width / 2;
            double centerY = height / 2;
            double r_s_visual = mass * 2;

            // Desenhar objeto massivo
            Ellipse star = new Ellipse
            {
                Width = r_s_visual * 2,
                Height = r_s_visual * 2,
                Fill = new RadialGradientBrush
                {
                    GradientStops = new GradientStopCollection
                    {
                        new GradientStop(Color.FromRgb(241, 196, 15), 0.0),
                        new GradientStop(Color.FromRgb(243, 156, 18), 0.5),
                        new GradientStop(Color.FromRgb(230, 126, 34), 1.0)
                    }
                }
            };

            Canvas.SetLeft(star, centerX - r_s_visual);
            Canvas.SetTop(star, centerY - r_s_visual);
            RelativityCanvas.Children.Add(star);

            // Trajetória da luz SEM deflexão (linha reta)
            double startY = centerY - impactParam * 20;
            Line straightPath = new Line
            {
                X1 = 0,
                Y1 = startY,
                X2 = width,
                Y2 = startY,
                Stroke = new SolidColorBrush(Color.FromArgb(100, 189, 195, 199)),
                StrokeThickness = 2,
                StrokeDashArray = new DoubleCollection(new double[] { 5, 3 })
            };
            RelativityCanvas.Children.Add(straightPath);

            // Trajetória da luz COM deflexão (curva)
            List<Point> deflectedPath = new List<Point>();

            for (double x = 0; x <= width; x += 2)
            {
                double dx = x - centerX;
                double distance = Math.Sqrt(dx * dx + (impactParam * 20) * (impactParam * 20));

                // Ângulo de deflexão aproximado
                double deflection = 0;
                if (distance > r_s_visual)
                {
                    deflection = (4 * mass * 0.5) / (impactParam * 20) * (centerX / distance);
                }

                double y = startY + deflection;
                deflectedPath.Add(new Point(x, y));
            }

            DrawCurve(deflectedPath, Color.FromRgb(52, 152, 219), 3);

            // Calcular ângulo total de deflexão
            double totalDeflection = (4 * mass * 0.5) / (impactParam * 20) * 2;

            // Labels
            AddLabel(50, height - 30, "Sem curvatura", Color.FromRgb(189, 195, 199));
            AddLabel(50, height - 50, "Com curvatura", Color.FromRgb(52, 152, 219));
            AddLabel(width / 2, 20, $"Deflexão: {totalDeflection:F2} unidades", Colors.Black);
        }
        #endregion

        #region Órbitas
        private void DrawOrbits()
        {
            if (RelativityCanvas == null || MassSlider == null || DistanceSlider == null) return;

            double width = RelativityCanvas.ActualWidth;
            double height = RelativityCanvas.ActualHeight;

            if (width <= 0 || height <= 0) return;

            double mass = MassSlider.Value;
            double r_ratio = DistanceSlider.Value;

            double centerX = width / 2;
            double centerY = height / 2;
            double r_s_visual = mass * 2;

            // Buraco negro
            Ellipse blackHole = new Ellipse
            {
                Width = r_s_visual * 2,
                Height = r_s_visual * 2,
                Fill = Brushes.Black,
                Stroke = Brushes.DarkBlue,
                StrokeThickness = 3
            };

            Canvas.SetLeft(blackHole, centerX - r_s_visual);
            Canvas.SetTop(blackHole, centerY - r_s_visual);
            RelativityCanvas.Children.Add(blackHole);

            // Horizonte de eventos
            DrawCircle(centerX, centerY, r_s_visual, Color.FromRgb(231, 76, 60), 2, true);
            AddLabel(centerX + r_s_visual, centerY, "r_s", Color.FromRgb(231, 76, 60));

            // Órbita de fóton (r = 1.5 r_s)
            DrawCircle(centerX, centerY, r_s_visual * 1.5, Color.FromRgb(241, 196, 15), 2, true);
            AddLabel(centerX + r_s_visual * 1.5, centerY, "1.5 r_s (fótons)", Color.FromRgb(241, 196, 15));

            // ISCO (r = 3 r_s)
            DrawCircle(centerX, centerY, r_s_visual * 3, Color.FromRgb(46, 204, 113), 2, true);
            AddLabel(centerX + r_s_visual * 3, centerY, "3 r_s (ISCO)", Color.FromRgb(46, 204, 113));

            // Órbita selecionada
            double orbitRadius = r_s_visual * r_ratio;
            DrawCircle(centerX, centerY, orbitRadius, Color.FromRgb(52, 152, 219), 3, false);

            // Partícula na órbita
            double angle = 0;
            Ellipse particle = new Ellipse
            {
                Width = 8,
                Height = 8,
                Fill = Brushes.Cyan
            };

            Canvas.SetLeft(particle, centerX + orbitRadius * Math.Cos(angle) - 4);
            Canvas.SetTop(particle, centerY + orbitRadius * Math.Sin(angle) - 4);
            RelativityCanvas.Children.Add(particle);

            // Status da órbita
            string status;
            Color statusColor;
            if (r_ratio < 1.0)
            {
                status = "Dentro do horizonte!";
                statusColor = Colors.Red;
            }
            else if (r_ratio < 1.5)
            {
                status = "Captura inevitável";
                statusColor = Colors.OrangeRed;
            }
            else if (r_ratio < 3.0)
            {
                status = "Órbita instável";
                statusColor = Colors.Orange;
            }
            else
            {
                status = "Órbita estável";
                statusColor = Colors.Green;
            }

            AddLabel(width / 2, 20, $"r = {r_ratio:F1} r_s - {status}", statusColor);
        }

        private void DrawCircle(double cx, double cy, double radius, Color color, double thickness, bool dashed)
        {
            if (RelativityCanvas == null) return;

            Ellipse circle = new Ellipse
            {
                Width = radius * 2,
                Height = radius * 2,
                Stroke = new SolidColorBrush(color),
                StrokeThickness = thickness,
                Fill = Brushes.Transparent
            };

            if (dashed)
            {
                circle.StrokeDashArray = new DoubleCollection(new double[] { 5, 3 });
            }

            Canvas.SetLeft(circle, cx - radius);
            Canvas.SetTop(circle, cy - radius);
            RelativityCanvas.Children.Add(circle);
        }
        #endregion

        #region Ondas Gravitacionais
        private void DrawGravitationalWaves()
        {
            if (RelativityCanvas == null) return;

            double width = RelativityCanvas.ActualWidth;
            double height = RelativityCanvas.ActualHeight;

            if (width <= 0 || height <= 0) return;

            double centerY = height / 2;

            // Desenhar múltiplas ondas com diferentes fases
            for (int wave = 0; wave < 3; wave++)
            {
                List<Point> points = new List<Point>();
                double phaseShift = wave * Math.PI * 2 / 3;
                double amplitude = 30 + wave * 10;

                for (double x = 0; x <= width; x += 2)
                {
                    double y = centerY + amplitude * Math.Sin(2 * Math.PI * x / 100 + phaseShift);
                    points.Add(new Point(x, y));
                }

                byte alpha = (byte)(255 - wave * 50);
                DrawCurve(points, Color.FromArgb(alpha, 52, 152, 219), 2);
            }

            // Efeito de deformação (+ polarização)
            double strainAmplitude = 20;

            // Desenhar elipse sendo deformada
            for (int i = 0; i < 8; i++)
            {
                double phase = i * Math.PI / 4;
                double strain = strainAmplitude * Math.Sin(phase);

                Ellipse deformedCircle = new Ellipse
                {
                    Width = 40 + strain,
                    Height = 40 - strain,
                    Stroke = new SolidColorBrush(Color.FromArgb(100, 231, 76, 60)),
                    StrokeThickness = 1.5,
                    Fill = Brushes.Transparent
                };

                double x = 80 + i * 60;
                Canvas.SetLeft(deformedCircle, x - (40 + strain) / 2);
                Canvas.SetTop(deformedCircle, height - 80 - (40 - strain) / 2);
                RelativityCanvas.Children.Add(deformedCircle);
            }

            // Informações
            AddLabel(width / 2, 20, "Ondas Gravitacionais: h+ polarização", Colors.Black);
            AddLabel(width / 2, height - 40, "Deformação do espaço-tempo propagando", Color.FromRgb(231, 76, 60));
            AddLabel(20, height / 2 - 60, "Amplitude: ~10⁻²¹", Color.FromRgb(52, 152, 219));
        }
        #endregion

        #region Utilitários
        private void DrawCurve(List<Point> points, Color color, double thickness)
        {
            if (RelativityCanvas == null) return;

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
                RelativityCanvas.Children.Add(line);
            }
        }

        private void DrawVerticalDashedLine(double x, double height, Color color, string label)
        {
            if (RelativityCanvas == null) return;

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
            RelativityCanvas.Children.Add(line);

            if (!string.IsNullOrEmpty(label))
            {
                AddLabel(x + 5, 10, label, color);
            }
        }

        private void AddLabel(double x, double y, string text, Color color)
        {
            if (RelativityCanvas == null) return;

            TextBlock label = new TextBlock
            {
                Text = text,
                FontSize = 11,
                FontWeight = FontWeights.Bold,
                Foreground = new SolidColorBrush(color),
                Background = new SolidColorBrush(Color.FromArgb(200, 255, 255, 255)),
                Padding = new Thickness(5, 2, 5, 2)
            };

            Canvas.SetLeft(label, x);
            Canvas.SetTop(label, y);
            RelativityCanvas.Children.Add(label);
        }

        private void AddAxisLabels(string xLabel, string yLabel, string xMin, string xMax, string yMin, string yMax)
        {
            if (RelativityCanvas == null) return;

            double width = RelativityCanvas.ActualWidth;
            double height = RelativityCanvas.ActualHeight;

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
            RelativityCanvas.Children.Add(xLabelText);

            // Labels mínimo e máximo X
            if (!string.IsNullOrEmpty(xMin))
            {
                TextBlock xMinText = new TextBlock { Text = xMin, FontSize = 9, Foreground = Brushes.Gray };
                Canvas.SetLeft(xMinText, 5);
                Canvas.SetTop(xMinText, height - 15);
                RelativityCanvas.Children.Add(xMinText);
            }

            if (!string.IsNullOrEmpty(xMax))
            {
                TextBlock xMaxText = new TextBlock { Text = xMax, FontSize = 9, Foreground = Brushes.Gray };
                Canvas.SetLeft(xMaxText, width - 25);
                Canvas.SetTop(xMaxText, height - 15);
                RelativityCanvas.Children.Add(xMaxText);
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
            RelativityCanvas.Children.Add(yLabelText);

            // Labels mínimo e máximo Y
            if (!string.IsNullOrEmpty(yMin))
            {
                TextBlock yMinText = new TextBlock { Text = yMin, FontSize = 9, Foreground = Brushes.Gray };
                Canvas.SetLeft(yMinText, 5);
                Canvas.SetTop(yMinText, height - 30);
                RelativityCanvas.Children.Add(yMinText);
            }

            if (!string.IsNullOrEmpty(yMax))
            {
                TextBlock yMaxText = new TextBlock { Text = yMax, FontSize = 9, Foreground = Brushes.Gray };
                Canvas.SetLeft(yMaxText, 5);
                Canvas.SetTop(yMaxText, 25);
                RelativityCanvas.Children.Add(yMaxText);
            }
        }
        #endregion

        #region Botões de Navegação
        private void ExercisesButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Exercícios sobre Relatividade Geral:\n\n" +
                          "1. Calcule o raio de Schwarzschild para um buraco negro de 10 M☉\n" +
                          "2. Determine o fator de dilatação do tempo a r = 2 r_s\n" +
                          "3. Calcule o ângulo de deflexão da luz pelo Sol\n" +
                          "4. Encontre a ISCO (órbita circular mais interna) para Schwarzschild\n" +
                          "5. Estime a energia emitida em ondas gravitacionais por fusão de BHs\n" +
                          "6. Derive a métrica de Schwarzschild a partir das equações de Einstein",
                          "Exercícios", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void QuizButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Quiz - Relatividade Geral:\n\n" +
                          "1. O que é o princípio de equivalência?\n" +
                          "2. Como massa curva o espaço-tempo?\n" +
                          "3. O que acontece no horizonte de eventos?\n" +
                          "4. Por que luz se curva próximo a massas?\n" +
                          "5. O que são ondas gravitacionais?\n" +
                          "6. Qual a diferença entre Schwarzschild e Kerr?\n" +
                          "7. Como GPS usa Relatividade Geral?\n" +
                          "8. O que é a ISCO?",
                          "Quiz", MessageBoxButton.OK, MessageBoxImage.Question);
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        #endregion
    }
}