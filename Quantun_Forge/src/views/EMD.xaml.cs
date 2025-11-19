using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace Quantun_Forge.src.views
{
    public partial class EMD : UserControl
    {
        // Estado quântico atual (Simulador 1) - CORRIGIDO: inicialização inline
        private Complex alpha = new Complex(0, 0);
        private Complex beta = new Complex(0, 0);
        private bool hasState = false;
        private bool isMeasured = false;
        private Random random = new Random();

        // Estado do Simulador 2 (Decoerência)
        private double currentNoiseLevel = 0;
        private bool decoherenceInitialized = false;

        public EMD()
        {
            InitializeComponent();
        }

        #region Simulador 1: Medição e Colapso

        // Criar estado |+⟩
        private void BtnCreatePlus_Click(object sender, RoutedEventArgs e)
        {
            // |+⟩ = (|0⟩ + |1⟩)/√2
            alpha = new Complex(1.0 / Math.Sqrt(2), 0);
            beta = new Complex(1.0 / Math.Sqrt(2), 0);
            hasState = true;
            isMeasured = false;

            UpdateMeasureUI();
            AnimateStateCreation(Math.PI / 2, 0); // Equador, phi = 0
        }

        // Criar estado |−⟩
        private void BtnCreateMinus_Click(object sender, RoutedEventArgs e)
        {
            // |−⟩ = (|0⟩ − |1⟩)/√2
            alpha = new Complex(1.0 / Math.Sqrt(2), 0);
            beta = new Complex(-1.0 / Math.Sqrt(2), 0);
            hasState = true;
            isMeasured = false;

            UpdateMeasureUI();
            AnimateStateCreation(Math.PI / 2, Math.PI); // Equador, phi = π
        }

        // Criar estado customizado
        private void BtnCreateCustom_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new CustomStateDialog();
            if (dialog.ShowDialog() == true)
            {
                double theta = dialog.Theta;
                double phi = dialog.Phi;

                // Conversão de coordenadas esféricas para estado quântico
                alpha = new Complex(Math.Cos(theta / 2), 0);
                beta = new Complex(
                    Math.Sin(theta / 2) * Math.Cos(phi),
                    Math.Sin(theta / 2) * Math.Sin(phi)
                );

                hasState = true;
                isMeasured = false;

                UpdateMeasureUI();
                AnimateStateCreation(theta, phi);
            }
        }

        // Medir o qubit
        private void BtnMeasure_Click(object sender, RoutedEventArgs e)
        {
            if (!hasState || isMeasured)
                return;

            // Calcular probabilidades
            double prob0 = alpha.MagnitudeSquared();
            double prob1 = beta.MagnitudeSquared();

            // Simular medição
            double randomValue = random.NextDouble();
            int result = (randomValue < prob0) ? 0 : 1;

            isMeasured = true;

            // Atualizar UI
            TxtMeasureResult.Text = $"Resultado: |{result}⟩";
            BtnMeasure.IsEnabled = false;

            // Animar colapso
            AnimateCollapse(result);
        }

        // Resetar simulador 1
        private void BtnResetMeasure_Click(object sender, RoutedEventArgs e)
        {
            hasState = false;
            isMeasured = false;
            alpha = new Complex(0, 0);
            beta = new Complex(0, 0);

            TxtCurrentState.Text = "|ψ⟩ = ?";
            TxtProbabilities.Text = "Crie um estado primeiro";
            TxtMeasureResult.Text = "";
            BtnMeasure.IsEnabled = false;

            // Esconder visualização
            MeasureStateVector.Opacity = 0;
            MeasureStatePoint.Opacity = 0;
            SuperpositionCloud.Opacity = 0;

            TxtVisualizationStatus.Text = "Estado inicial:\nCrie uma superposição para começar";
        }

        // Atualizar interface do simulador 1
        private void UpdateMeasureUI()
        {
            // Atualizar texto do estado
            string stateText = FormatQuantumState(alpha, beta);
            TxtCurrentState.Text = stateText;

            // Calcular e mostrar probabilidades
            double prob0 = alpha.MagnitudeSquared() * 100;
            double prob1 = beta.MagnitudeSquared() * 100;
            TxtProbabilities.Text = $"P(0) = {prob0:F1}%, P(1) = {prob1:F1}%";

            // Habilitar botão de medição
            BtnMeasure.IsEnabled = true;
            TxtMeasureResult.Text = "";

            // Atualizar status
            TxtVisualizationStatus.Text = $"Estado em superposição criado!\nP(|0⟩) = {prob0:F1}%, P(|1⟩) = {prob1:F1}%";
        }

        // Animar criação do estado
        private void AnimateStateCreation(double theta, double phi)
        {
            // Calcular coordenadas na esfera de Bloch
            double radius = 100; // raio da esfera
            double centerX = 175;
            double centerY = 175;

            // Converter coordenadas esféricas para cartesianas
            double x = radius * Math.Sin(theta) * Math.Cos(phi);
            double y = radius * Math.Sin(theta) * Math.Sin(phi);
            double z = radius * Math.Cos(theta);

            // Projetar em 2D (vista de cima, com perspectiva)
            double screenX = centerX + x;
            double screenY = centerY - z;

            // Animar vetor
            var vectorAnim = new DoubleAnimation(1, TimeSpan.FromMilliseconds(500));
            MeasureStateVector.BeginAnimation(Line.OpacityProperty, vectorAnim);

            var x2Anim = new DoubleAnimation(screenX, TimeSpan.FromMilliseconds(500));
            var y2Anim = new DoubleAnimation(screenY, TimeSpan.FromMilliseconds(500));
            MeasureStateVector.BeginAnimation(Line.X2Property, x2Anim);
            MeasureStateVector.BeginAnimation(Line.Y2Property, y2Anim);

            // Animar ponto
            var pointAnim = new DoubleAnimation(1, TimeSpan.FromMilliseconds(500));
            MeasureStatePoint.BeginAnimation(Ellipse.OpacityProperty, pointAnim);

            var pointXAnim = new DoubleAnimation(screenX - 10, TimeSpan.FromMilliseconds(500));
            var pointYAnim = new DoubleAnimation(screenY - 10, TimeSpan.FromMilliseconds(500));
            MeasureStatePoint.BeginAnimation(Canvas.LeftProperty, pointXAnim);
            MeasureStatePoint.BeginAnimation(Canvas.TopProperty, pointYAnim);

            // Mostrar nuvem de superposição (se não for |0⟩ ou |1⟩ puro)
            if (theta > 0.1 && theta < Math.PI - 0.1)
            {
                var cloudAnim = new DoubleAnimation(0.3, TimeSpan.FromMilliseconds(500));
                SuperpositionCloud.BeginAnimation(Ellipse.OpacityProperty, cloudAnim);
            }
        }

        // Animar colapso após medição
        private void AnimateCollapse(int result)
        {
            double targetY = (result == 0) ? 25 : 325; // |0⟩ no topo, |1⟩ embaixo

            // Fade out da nuvem
            var cloudFadeOut = new DoubleAnimation(0, TimeSpan.FromMilliseconds(300));
            SuperpositionCloud.BeginAnimation(Ellipse.OpacityProperty, cloudFadeOut);

            // Mover vetor para o polo
            var y2Anim = new DoubleAnimation(targetY, TimeSpan.FromMilliseconds(800))
            {
                EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut }
            };
            MeasureStateVector.BeginAnimation(Line.Y2Property, y2Anim);

            var x2Anim = new DoubleAnimation(175, TimeSpan.FromMilliseconds(800))
            {
                EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut }
            };
            MeasureStateVector.BeginAnimation(Line.X2Property, x2Anim);

            // Mover ponto
            var pointYAnim = new DoubleAnimation(targetY - 10, TimeSpan.FromMilliseconds(800))
            {
                EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut }
            };
            MeasureStatePoint.BeginAnimation(Canvas.TopProperty, pointYAnim);

            var pointXAnim = new DoubleAnimation(165, TimeSpan.FromMilliseconds(800))
            {
                EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut }
            };
            MeasureStatePoint.BeginAnimation(Canvas.LeftProperty, pointXAnim);

            // Mudar cor para vermelho (colapso)
            var colorAnim = new ColorAnimation(Colors.Red, TimeSpan.FromMilliseconds(300));
            var brush = new SolidColorBrush();
            brush.BeginAnimation(SolidColorBrush.ColorProperty, colorAnim);
            MeasureStateVector.Stroke = brush;

            // Atualizar status
            TxtVisualizationStatus.Text = $"Estado colapsou para |{result}⟩!\nSuperposição destruída pela medição.";
        }

        #endregion

        #region Simulador 2: Decoerência

        // Criar estado inicial para decoerência
        private void BtnDecoherencePlus_Click(object sender, RoutedEventArgs e)
        {
            decoherenceInitialized = true;
            SliderNoise.Value = 0;

            // Resetar visualização
            UpdateDecoherenceVisualization(0);

            // Animar aparecimento do vetor ideal e com ruído
            var fadeIn = new DoubleAnimation(0.3, TimeSpan.FromMilliseconds(500));
            IdealStateVector.BeginAnimation(Line.OpacityProperty, fadeIn);

            var fadeInNoisy = new DoubleAnimation(1, TimeSpan.FromMilliseconds(500));
            NoisyStateVector.BeginAnimation(Line.OpacityProperty, fadeInNoisy);
            NoisyStatePoint.BeginAnimation(Ellipse.OpacityProperty, fadeInNoisy);
        }

        // Slider de ruído mudou
        private void SliderNoise_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (!decoherenceInitialized)
                return;

            currentNoiseLevel = e.NewValue;
            UpdateDecoherenceVisualization(currentNoiseLevel);
        }

        // Atualizar visualização da decoerência
        private void UpdateDecoherenceVisualization(double noisePercent)
        {
            // Atualizar textos
            string noiseStatus = noisePercent switch
            {
                0 => "Estado Puro",
                < 25 => "Decoerência Leve",
                < 50 => "Decoerência Moderada",
                < 75 => "Decoerência Severa",
                _ => "Mistura Clássica"
            };

            TxtNoiseLevel.Text = $"Ruído: {noisePercent:F0}% ({noiseStatus})";

            // Atualizar coerência
            double coherence = 100 - noisePercent;
            CoherenceBar.Value = coherence;
            TxtCoherence.Text = $"Coerência: {coherence:F0}%";

            // Atualizar status
            TxtDecoherenceStatus.Text = noisePercent switch
            {
                0 => "Estado puro preservado",
                < 30 => "Perda leve de coerência",
                < 60 => "Coerência significativamente reduzida",
                < 90 => "Quase completamente decoerente",
                _ => "Mistura estatística clássica"
            };

            // Atualizar tempos T1 e T2
            if (noisePercent > 0)
            {
                double t1 = 100 / noisePercent; // Inversamente proporcional
                double t2 = t1 * 0.7; // T2 é tipicamente menor que T1

                TxtT1.Text = $"T₁ (relaxação): {t1:F1} μs";
                TxtT2.Text = $"T₂ (dephasing): {t2:F1} μs";
            }
            else
            {
                TxtT1.Text = "T₁ (relaxação): infinito";
                TxtT2.Text = "T₂ (dephasing): infinito";
            }

            // Atualizar visualização na esfera
            UpdateBlochSphereDecoherence(noisePercent);
        }

        // Atualizar esfera de Bloch com decoerência
        private void UpdateBlochSphereDecoherence(double noisePercent)
        {
            // Calcular "encolhimento" do vetor
            // Com decoerência, o vetor vai em direção ao centro da esfera
            double shrinkFactor = 1.0 - (noisePercent / 100.0);

            // Vetor ideal permanece no equador (|+⟩)
            double idealEndX = 275; // 175 (centro) + 100 (raio)
            double idealEndY = 175; // no equador

            // Vetor com ruído "encolhe"
            double noisyEndX = 175 + (idealEndX - 175) * shrinkFactor;
            double noisyEndY = idealEndY;

            // Animar o encolhimento
            var x2Anim = new DoubleAnimation(noisyEndX, TimeSpan.FromMilliseconds(200));
            NoisyStateVector.BeginAnimation(Line.X2Property, x2Anim);

            var pointXAnim = new DoubleAnimation(noisyEndX - 10, TimeSpan.FromMilliseconds(200));
            NoisyStatePoint.BeginAnimation(Canvas.LeftProperty, pointXAnim);

            // Mudar opacidade do vetor ruidoso
            var opacityAnim = new DoubleAnimation(Math.Max(0.3, shrinkFactor), TimeSpan.FromMilliseconds(200));
            NoisyStateVector.BeginAnimation(Line.OpacityProperty, opacityAnim);

            // Mostrar ondas de ruído proporcionais ao nível de ruído
            double waveOpacity = noisePercent / 100.0;
            var waveAnim = new DoubleAnimation(waveOpacity, TimeSpan.FromMilliseconds(200));
            NoiseWave1.BeginAnimation(Path.OpacityProperty, waveAnim);
            NoiseWave2.BeginAnimation(Path.OpacityProperty, waveAnim);

            // Animar ondas (pequena oscilação)
            if (noisePercent > 0)
            {
                AnimateNoiseWaves();
            }
        }

        // Animar ondas de ruído
        private void AnimateNoiseWaves()
        {
            var storyboard = new Storyboard { RepeatBehavior = RepeatBehavior.Forever };

            // Onda 1 - movimento vertical
            var wave1Anim = new DoubleAnimation
            {
                From = 100,
                To = 110,
                Duration = TimeSpan.FromMilliseconds(800),
                AutoReverse = true
            };
            Storyboard.SetTarget(wave1Anim, NoiseWave1);
            Storyboard.SetTargetProperty(wave1Anim, new PropertyPath(Canvas.TopProperty));
            storyboard.Children.Add(wave1Anim);

            // Onda 2 - movimento vertical invertido
            var wave2Anim = new DoubleAnimation
            {
                From = 220,
                To = 210,
                Duration = TimeSpan.FromMilliseconds(800),
                AutoReverse = true
            };
            Storyboard.SetTarget(wave2Anim, NoiseWave2);
            Storyboard.SetTargetProperty(wave2Anim, new PropertyPath(Canvas.TopProperty));
            storyboard.Children.Add(wave2Anim);

            storyboard.Begin();
        }

        #endregion

        #region Utilitários

        // Formatar estado quântico para exibição
        private string FormatQuantumState(Complex a, Complex b)
        {
            string result = "|ψ⟩ = ";

            // Primeira componente (α|0⟩)
            if (Math.Abs(a.Real - 1.0) < 0.001 && Math.Abs(a.Imaginary) < 0.001)
            {
                result += "|0⟩";
            }
            else if (Math.Abs(a.Real - 1.0 / Math.Sqrt(2)) < 0.001 && Math.Abs(a.Imaginary) < 0.001)
            {
                result += "(1/√2)|0⟩";
            }
            else
            {
                result += $"({a.Real:F2})|0⟩";
            }

            // Segunda componente (β|1⟩)
            if (b.Real >= 0)
            {
                result += " + ";
            }
            else
            {
                result += " - ";
            }

            double betaMag = Math.Abs(b.Real);
            if (Math.Abs(betaMag - 1.0) < 0.001)
            {
                result += "|1⟩";
            }
            else if (Math.Abs(betaMag - 1.0 / Math.Sqrt(2)) < 0.001)
            {
                result += "(1/√2)|1⟩";
            }
            else
            {
                result += $"({betaMag:F2})|1⟩";
            }

            return result;
        }

        #endregion
    }

    #region Classes Auxiliares

    // Classe para representar números complexos
    public class Complex
    {
        public double Real { get; set; }
        public double Imaginary { get; set; }

        public Complex(double real, double imaginary)
        {
            Real = real;
            Imaginary = imaginary;
        }

        public double MagnitudeSquared()
        {
            return Real * Real + Imaginary * Imaginary;
        }

        public double Magnitude()
        {
            return Math.Sqrt(MagnitudeSquared());
        }
    }

    // Diálogo para criar estado customizado
    public class CustomStateDialog : Window
    {
        public double Theta { get; private set; }
        public double Phi { get; private set; }

        private Slider thetaSlider = new Slider();
        private Slider phiSlider = new Slider();
        private TextBlock thetaText = new TextBlock();
        private TextBlock phiText = new TextBlock();

        public CustomStateDialog()
        {
            Title = "Estado Quântico Customizado";
            Width = 400;
            Height = 300;
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            ResizeMode = ResizeMode.NoResize;

            var grid = new Grid { Margin = new Thickness(20) };
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

            // Título
            var title = new TextBlock
            {
                Text = "Defina os ângulos na Esfera de Bloch:",
                FontSize = 14,
                FontWeight = FontWeights.Bold,
                Margin = new Thickness(0, 0, 0, 20)
            };
            Grid.SetRow(title, 0);
            grid.Children.Add(title);

            // Theta
            var thetaLabel = new TextBlock
            {
                Text = "θ (theta) - Ângulo polar:",
                FontWeight = FontWeights.SemiBold,
                Margin = new Thickness(0, 0, 0, 5)
            };
            Grid.SetRow(thetaLabel, 1);
            grid.Children.Add(thetaLabel);

            thetaSlider.Minimum = 0;
            thetaSlider.Maximum = Math.PI;
            thetaSlider.Value = Math.PI / 2;
            thetaSlider.TickFrequency = Math.PI / 8;
            thetaSlider.IsSnapToTickEnabled = false;
            thetaSlider.ValueChanged += ThetaSlider_ValueChanged;
            Grid.SetRow(thetaSlider, 2);
            grid.Children.Add(thetaSlider);

            thetaText.Text = $"θ = {Math.PI / 2:F2} rad ({90:F0}°)";
            thetaText.Margin = new Thickness(0, 5, 0, 15);
            thetaText.Foreground = new SolidColorBrush(Color.FromRgb(52, 152, 219));
            Grid.SetRow(thetaText, 3);
            grid.Children.Add(thetaText);

            // Phi
            var phiLabel = new TextBlock
            {
                Text = "φ (phi) - Ângulo azimutal:",
                FontWeight = FontWeights.SemiBold,
                Margin = new Thickness(0, 0, 0, 5)
            };
            Grid.SetRow(phiLabel, 4);
            grid.Children.Add(phiLabel);

            phiSlider.Minimum = 0;
            phiSlider.Maximum = 2 * Math.PI;
            phiSlider.Value = 0;
            phiSlider.TickFrequency = Math.PI / 4;
            phiSlider.IsSnapToTickEnabled = false;
            phiSlider.ValueChanged += PhiSlider_ValueChanged;
            Grid.SetRow(phiSlider, 5);
            grid.Children.Add(phiSlider);

            phiText.Text = $"φ = {0:F2} rad ({0:F0}°)";
            phiText.Margin = new Thickness(0, 5, 0, 0);
            phiText.Foreground = new SolidColorBrush(Color.FromRgb(155, 89, 182));
            Grid.SetRow(phiText, 6);
            grid.Children.Add(phiText);

            // Botões
            var buttonPanel = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                HorizontalAlignment = HorizontalAlignment.Right,
                Margin = new Thickness(0, 20, 0, 0)
            };

            var okButton = new Button
            {
                Content = "Criar Estado",
                Width = 100,
                Height = 35,
                Margin = new Thickness(0, 0, 10, 0),
                Background = new SolidColorBrush(Color.FromRgb(52, 152, 219)),
                Foreground = Brushes.White,
                BorderThickness = new Thickness(0),
                Cursor = System.Windows.Input.Cursors.Hand
            };
            okButton.Click += OkButton_Click;

            var cancelButton = new Button
            {
                Content = "Cancelar",
                Width = 100,
                Height = 35,
                Background = new SolidColorBrush(Color.FromRgb(127, 140, 141)),
                Foreground = Brushes.White,
                BorderThickness = new Thickness(0),
                Cursor = System.Windows.Input.Cursors.Hand
            };
            cancelButton.Click += CancelButton_Click;

            buttonPanel.Children.Add(okButton);
            buttonPanel.Children.Add(cancelButton);
            Grid.SetRow(buttonPanel, 7);
            grid.Children.Add(buttonPanel);

            Content = grid;
        }

        private void ThetaSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            double degrees = e.NewValue * 180 / Math.PI;
            thetaText.Text = $"θ = {e.NewValue:F2} rad ({degrees:F0}°)";
        }

        private void PhiSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            double degrees = e.NewValue * 180 / Math.PI;
            phiText.Text = $"φ = {e.NewValue:F2} rad ({degrees:F0}°)";
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            Theta = thetaSlider.Value;
            Phi = phiSlider.Value;
            DialogResult = true;
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }

    #endregion
}