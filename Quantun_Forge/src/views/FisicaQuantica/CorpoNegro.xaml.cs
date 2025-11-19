using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Quantun_Forge.src.views.FisicaQuantica
{
    /// <summary>
    /// Simulador Visual de Corpo Negro e Radiação de Planck
    /// </summary>
    public partial class CorpoNegro : UserControl
    {
        #region Constantes Físicas

        private const double PLANCK_CONSTANT = 6.62607015e-34;      // J·s
        private const double SPEED_OF_LIGHT = 299792458;            // m/s
        private const double BOLTZMANN_CONSTANT = 1.380649e-23;    // J/K
        private const double WIEN_CONSTANT = 2.897771955e-3;       // m·K
        private const double STEFAN_BOLTZMANN = 5.670374419e-8;    // W/(m²·K⁴)

        #endregion

        #region Variáveis de Estado

        private double temperaturaAtual = 3000;
        private DispatcherTimer animationTimer;
        private DispatcherTimer flamesTimer;
        private double wavePhase = 0;

        #endregion

        public CorpoNegro()
        {
            InitializeComponent();
            Loaded += CorpoNegro_Loaded;
        }

        private void CorpoNegro_Loaded(object sender, RoutedEventArgs e)
        {
            InicializarSimulador();
        }

        #region Inicialização

        private void InicializarSimulador()
        {
            // Timers para animações
            animationTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(50)
            };
            animationTimer.Tick += AnimationTimer_Tick;
            animationTimer.Start();

            flamesTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(100)
            };
            flamesTimer.Tick += FlamesTimer_Tick;
            flamesTimer.Start();

            // Desenhar curva inicial
            AtualizarVisualizacao();
        }

        #endregion

        #region Cálculos Físicos

        /// <summary>
        /// Lei de Planck: Intensidade espectral
        /// I(λ,T) = (2hc²/λ⁵) / (e^(hc/λkT) - 1)
        /// </summary>
        private double CalcularIntensidadePlanck(double wavelength, double temperatura)
        {
            double numerador = 2.0 * PLANCK_CONSTANT * Math.Pow(SPEED_OF_LIGHT, 2) / Math.Pow(wavelength, 5);
            double expoente = (PLANCK_CONSTANT * SPEED_OF_LIGHT) / (wavelength * BOLTZMANN_CONSTANT * temperatura);

            // Evitar overflow
            if (expoente > 700) return 0;

            double denominador = Math.Exp(expoente) - 1.0;

            if (denominador <= 0) return 0;

            return numerador / denominador;
        }

        /// <summary>
        /// Aproximação de Wien (válida para λ curtos)
        /// I(λ,T) ≈ (2hc²/λ⁵) · e^(-hc/λkT)
        /// </summary>
        private double CalcularIntensidadeWien(double wavelength, double temperatura)
        {
            double numerador = 2.0 * PLANCK_CONSTANT * Math.Pow(SPEED_OF_LIGHT, 2) / Math.Pow(wavelength, 5);
            double expoente = -(PLANCK_CONSTANT * SPEED_OF_LIGHT) / (wavelength * BOLTZMANN_CONSTANT * temperatura);

            if (expoente < -700) return 0;

            return numerador * Math.Exp(expoente);
        }

        /// <summary>
        /// Lei de Rayleigh-Jeans (teoria clássica - FALHA no UV!)
        /// I(λ,T) = 2ckT/λ⁴
        /// </summary>
        private double CalcularIntensidadeRayleighJeans(double wavelength, double temperatura)
        {
            return (2.0 * SPEED_OF_LIGHT * BOLTZMANN_CONSTANT * temperatura) / Math.Pow(wavelength, 4);
        }

        /// <summary>
        /// Lei de Wien: λmax · T = constante
        /// </summary>
        private double CalcularLambdaMax(double temperatura)
        {
            return WIEN_CONSTANT / temperatura;
        }

        /// <summary>
        /// Lei de Stefan-Boltzmann: P/A = σT⁴
        /// </summary>
        private double CalcularPotenciaTotal(double temperatura)
        {
            return STEFAN_BOLTZMANN * Math.Pow(temperatura, 4);
        }

        /// <summary>
        /// Gera pontos da curva
        /// </summary>
        private List<(double lambda, double intensidade)> GerarCurvaPlanck(
            double temperatura,
            int numPontos = 200,
            double lambdaMin = 100,
            double lambdaMax = 3000)
        {
            var pontos = new List<(double, double)>();
            double step = (lambdaMax - lambdaMin) / numPontos;
            double maxIntensidade = 0;

            var intensidades = new List<double>();
            for (int i = 0; i < numPontos; i++)
            {
                double lambda_nm = lambdaMin + i * step;
                double lambda_m = lambda_nm * 1e-9;
                double intensidade = CalcularIntensidadePlanck(lambda_m, temperatura);
                intensidades.Add(intensidade);
                if (intensidade > maxIntensidade)
                    maxIntensidade = intensidade;
            }

            for (int i = 0; i < numPontos; i++)
            {
                double lambda_nm = lambdaMin + i * step;
                double intensidadeNormalizada = maxIntensidade > 0 ? intensidades[i] / maxIntensidade : 0;
                pontos.Add((lambda_nm, intensidadeNormalizada));
            }

            return pontos;
        }

        /// <summary>
        /// Converte temperatura para cor RGB
        /// </summary>
        private Color TemperaturaParaCor(double temperatura)
        {
            // Algoritmo baseado em temperatura de cor
            double temp = temperatura / 100;
            double red, green, blue;

            // Vermelho
            if (temp <= 66)
                red = 255;
            else
            {
                red = temp - 60;
                red = 329.698727446 * Math.Pow(red, -0.1332047592);
                red = Math.Max(0, Math.Min(255, red));
            }

            // Verde
            if (temp <= 66)
            {
                green = temp;
                green = 99.4708025861 * Math.Log(green) - 161.1195681661;
            }
            else
            {
                green = temp - 60;
                green = 288.1221695283 * Math.Pow(green, -0.0755148492);
            }
            green = Math.Max(0, Math.Min(255, green));

            // Azul
            if (temp >= 66)
                blue = 255;
            else if (temp <= 19)
                blue = 0;
            else
            {
                blue = temp - 10;
                blue = 138.5177312231 * Math.Log(blue) - 305.0447927307;
                blue = Math.Max(0, Math.Min(255, blue));
            }

            return Color.FromRgb((byte)red, (byte)green, (byte)blue);
        }

        #endregion

        #region Atualização de Visualização

        private void AtualizarVisualizacao()
        {
            // Atualizar cor do corpo negro
            Color cor = TemperaturaParaCor(temperaturaAtual);
            BlackBodyObject.Fill = new SolidColorBrush(cor);
            ColorDisplay.Fill = new SolidColorBrush(cor);

            // Atualizar efeito de brilho
            GlowEffect.Color = cor;
            GlowEffect.BlurRadius = 40 + (temperaturaAtual / 200);

            ColorDisplay.Effect = new DropShadowEffect
            {
                Color = cor,
                BlurRadius = 20,
                ShadowDepth = 0
            };

            // Calcular e mostrar λmax
            double lambdaMax = CalcularLambdaMax(temperaturaAtual);
            double lambdaMaxNm = lambdaMax * 1e9;
            TxtLambdaMax.Text = $"{lambdaMaxNm:F0} nm";

            // Calcular potência total
            double potencia = CalcularPotenciaTotal(temperaturaAtual);
            TxtIntensidadeTotal.Text = $"{potencia:E2} W/m²";

            // Atualizar displays de temperatura
            TxtTempDisplay.Text = $"{temperaturaAtual:F0} K";
            TxtTemperaturaObjeto.Text = $"{temperaturaAtual:F0} K";
            TxtTempStefan.Text = $"{temperaturaAtual:F0} K";
            TxtPotenciaStefan.Text = $"{potencia:E2} W/m²";

            // Desenhar curva de Planck
            DesenharCurvaPlanck();
        }

        private void DesenharCurvaPlanck()
        {
            if (PlanckCurveCanvas == null) return;

            PlanckCurveCanvas.Children.Clear();

            double canvasWidth = PlanckCurveCanvas.ActualWidth > 0 ? PlanckCurveCanvas.ActualWidth : 800;
            double canvasHeight = PlanckCurveCanvas.ActualHeight > 0 ? PlanckCurveCanvas.ActualHeight : 350;

            // Desenhar eixos
            DesenharEixos(canvasWidth, canvasHeight);

            // Desenhar curva de Planck
            if (ChkMostrarPlanck.IsChecked == true)
            {
                DesenharCurva(temperaturaAtual, Color.FromRgb(52, 152, 219), 3, canvasWidth, canvasHeight, TipoCurva.Planck);
            }

            // Desenhar curva de Wien
            if (ChkMostrarWien.IsChecked == true)
            {
                DesenharCurva(temperaturaAtual, Color.FromRgb(243, 156, 18), 2, canvasWidth, canvasHeight, TipoCurva.Wien);
            }

            // Desenhar curva de Rayleigh-Jeans
            if (ChkMostrarRayleigh.IsChecked == true)
            {
                DesenharCurva(temperaturaAtual, Color.FromRgb(231, 76, 60), 2, canvasWidth, canvasHeight, TipoCurva.RayleighJeans);
            }

            // Marcar λmax
            if (ChkMostrarLambdaMax.IsChecked == true)
            {
                double lambdaMax = CalcularLambdaMax(temperaturaAtual);
                double lambdaMaxNm = lambdaMax * 1e9;
                DesenharLambdaMax(lambdaMaxNm, canvasWidth, canvasHeight);
            }
        }

        private enum TipoCurva
        {
            Planck,
            Wien,
            RayleighJeans
        }

        private void DesenharCurva(double temperatura, Color cor, double espessura, double width, double height, TipoCurva tipo)
        {
            var polyline = new Polyline
            {
                Stroke = new SolidColorBrush(cor),
                StrokeThickness = espessura,
                StrokeLineJoin = PenLineJoin.Round
            };

            int numPontos = 200;
            double lambdaMin = 100;
            double lambdaMax = 3000;
            double step = (lambdaMax - lambdaMin) / numPontos;

            double maxIntensidade = 0;
            var intensidades = new List<double>();

            // Calcular intensidades
            for (int i = 0; i < numPontos; i++)
            {
                double lambda_nm = lambdaMin + i * step;
                double lambda_m = lambda_nm * 1e-9;
                double intensidade = 0;

                switch (tipo)
                {
                    case TipoCurva.Planck:
                        intensidade = CalcularIntensidadePlanck(lambda_m, temperatura);
                        break;
                    case TipoCurva.Wien:
                        intensidade = CalcularIntensidadeWien(lambda_m, temperatura);
                        break;
                    case TipoCurva.RayleighJeans:
                        intensidade = CalcularIntensidadeRayleighJeans(lambda_m, temperatura);
                        break;
                }

                intensidades.Add(intensidade);
                if (intensidade > maxIntensidade && intensidade < double.MaxValue / 2)
                    maxIntensidade = intensidade;
            }

            // Desenhar pontos
            for (int i = 0; i < numPontos; i++)
            {
                double lambda_nm = lambdaMin + i * step;
                double x = 50 + ((lambda_nm - lambdaMin) / (lambdaMax - lambdaMin)) * (width - 100);

                double intensidadeNormalizada = maxIntensidade > 0 ? intensidades[i] / maxIntensidade : 0;
                intensidadeNormalizada = Math.Min(1.0, intensidadeNormalizada);

                double y = height - 30 - (intensidadeNormalizada * (height - 60));

                polyline.Points.Add(new Point(x, y));
            }

            PlanckCurveCanvas.Children.Add(polyline);
        }

        private void DesenharEixos(double width, double height)
        {
            // Eixo X
            var eixoX = new Line
            {
                X1 = 50,
                Y1 = height - 30,
                X2 = width - 30,
                Y2 = height - 30,
                Stroke = Brushes.Black,
                StrokeThickness = 2
            };
            PlanckCurveCanvas.Children.Add(eixoX);

            // Eixo Y
            var eixoY = new Line
            {
                X1 = 50,
                Y1 = 30,
                X2 = 50,
                Y2 = height - 30,
                Stroke = Brushes.Black,
                StrokeThickness = 2
            };
            PlanckCurveCanvas.Children.Add(eixoY);

            // Labels
            var labelX = new TextBlock
            {
                Text = "λ (nm)",
                FontWeight = FontWeights.Bold,
                FontSize = 12,
                Foreground = Brushes.Black
            };
            Canvas.SetLeft(labelX, width - 80);
            Canvas.SetTop(labelX, height - 25);
            PlanckCurveCanvas.Children.Add(labelX);

            var labelY = new TextBlock
            {
                Text = "I(λ,T)",
                FontWeight = FontWeights.Bold,
                FontSize = 12,
                Foreground = Brushes.Black
            };
            Canvas.SetLeft(labelY, 10);
            Canvas.SetTop(labelY, 35);
            PlanckCurveCanvas.Children.Add(labelY);

            // Marcações do eixo X
            for (int lambda = 500; lambda <= 2500; lambda += 500)
            {
                double x = 50 + ((lambda - 100.0) / 2900.0) * (width - 100);

                var tick = new Line
                {
                    X1 = x,
                    Y1 = height - 30,
                    X2 = x,
                    Y2 = height - 25,
                    Stroke = Brushes.Black,
                    StrokeThickness = 1
                };
                PlanckCurveCanvas.Children.Add(tick);

                var label = new TextBlock
                {
                    Text = lambda.ToString(),
                    FontSize = 9,
                    Foreground = Brushes.Gray
                };
                Canvas.SetLeft(label, x - 12);
                Canvas.SetTop(label, height - 20);
                PlanckCurveCanvas.Children.Add(label);
            }
        }

        private void DesenharLambdaMax(double lambdaMaxNm, double width, double height)
        {
            double x = 50 + ((lambdaMaxNm - 100.0) / 2900.0) * (width - 100);

            if (x < 50 || x > width - 30) return;

            var linha = new Line
            {
                X1 = x,
                Y1 = 30,
                X2 = x,
                Y2 = height - 30,
                Stroke = new SolidColorBrush(Color.FromRgb(155, 89, 182)),
                StrokeThickness = 2,
                StrokeDashArray = new DoubleCollection { 5, 3 }
            };
            PlanckCurveCanvas.Children.Add(linha);

            var label = new TextBlock
            {
                Text = "λmax",
                FontSize = 11,
                FontWeight = FontWeights.Bold,
                Foreground = new SolidColorBrush(Color.FromRgb(155, 89, 182))
            };
            Canvas.SetLeft(label, x - 15);
            Canvas.SetTop(label, 10);
            PlanckCurveCanvas.Children.Add(label);
        }

        #endregion

        #region Animações

        private void AnimationTimer_Tick(object sender, EventArgs e)
        {
            wavePhase += 0.3;
            if (wavePhase > 2 * Math.PI) wavePhase = 0;

            DesenharOndasCalor();
            DesenharOndasRadiacao();
        }

        private void DesenharOndasCalor()
        {
            HeatWavesCanvas.Children.Clear();

            // Ondas de calor subindo
            for (int i = 0; i < 3; i++)
            {
                var polyline = new Polyline
                {
                    Stroke = new SolidColorBrush(Color.FromArgb(100, 255, 140, 0)),
                    StrokeThickness = 2
                };

                double yOffset = (wavePhase + i * Math.PI / 3) % (Math.PI * 2);
                double yBase = 100 - (yOffset / (Math.PI * 2)) * 100;

                for (int j = 0; j <= 20; j++)
                {
                    double x = 130 + j * 2;
                    double y = yBase + 5 * Math.Sin(j * 0.5 + wavePhase);
                    polyline.Points.Add(new Point(x, y));
                }

                HeatWavesCanvas.Children.Add(polyline);
            }
        }

        private void DesenharOndasRadiacao()
        {
            RadiationWavesCanvas.Children.Clear();

            Color cor = TemperaturaParaCor(temperaturaAtual);

            // Ondas eletromagnéticas saindo
            for (int i = 0; i < 5; i++)
            {
                double xOffset = (wavePhase + i * Math.PI / 5) % (Math.PI * 2);
                double xStart = (xOffset / (Math.PI * 2)) * 400;

                var polyline = new Polyline
                {
                    Stroke = new SolidColorBrush(Color.FromArgb(150, cor.R, cor.G, cor.B)),
                    StrokeThickness = 2
                };

                for (int j = 0; j <= 30; j++)
                {
                    double x = xStart + j * 5;
                    double y = 100 + 20 * Math.Sin(j * 0.3 + wavePhase);
                    polyline.Points.Add(new Point(x, y));
                }

                RadiationWavesCanvas.Children.Add(polyline);
            }
        }

        private void FlamesTimer_Tick(object sender, EventArgs e)
        {
            DesenharChamas();
        }

        private void DesenharChamas()
        {
            FlamesCanvas.Children.Clear();

            Random rand = new Random();
            Color cor = TemperaturaParaCor(temperaturaAtual);

            // Chamas animadas
            for (int i = 0; i < 5; i++)
            {
                var flame = new Polygon
                {
                    Fill = new SolidColorBrush(Color.FromArgb(180, cor.R, cor.G, cor.B)),
                    Stroke = new SolidColorBrush(Color.FromArgb(200, 255, 140, 0)),
                    StrokeThickness = 1
                };

                double x = 90 + i * 25 + rand.Next(-5, 6);
                double height = 20 + rand.Next(0, 20);

                flame.Points.Add(new Point(x, 0));
                flame.Points.Add(new Point(x + 10, -height));
                flame.Points.Add(new Point(x + 20, 0));

                FlamesCanvas.Children.Add(flame);
            }
        }

        #endregion

        #region Eventos dos Controles

        private void SliderTemperatura_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (!IsLoaded) return;

            temperaturaAtual = e.NewValue;
            TxtTemperaturaSlider.Text = $"{temperaturaAtual:F0} K";

            AtualizarVisualizacao();
        }

        private void ChkCurvas_Changed(object sender, RoutedEventArgs e)
        {
            if (!IsLoaded) return;
            DesenharCurvaPlanck();
        }

        private void AnimarTemperatura_Click(object sender, RoutedEventArgs e)
        {
            // Animar de temperatura baixa para alta
            var anim = new DoubleAnimation
            {
                From = 1000,
                To = 10000,
                Duration = TimeSpan.FromSeconds(5),
                AutoReverse = true
            };

            SliderTemperatura.BeginAnimation(Slider.ValueProperty, anim);
        }

        #endregion

        #region Temperaturas de Referência

        private void TempVela_Click(object sender, RoutedEventArgs e)
        {
            SliderTemperatura.Value = 1800;
            MessageBox.Show(
                "Chama de Vela: ~1800K\n\n" +
                "Cor: Laranja avermelhado\n" +
                "λmax ≈ 1610 nm (Infravermelho próximo)",
                "Temperatura: Vela",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }

        private void TempLampada_Click(object sender, RoutedEventArgs e)
        {
            SliderTemperatura.Value = 2700;
            MessageBox.Show(
                "Lâmpada Incandescente: ~2700K\n\n" +
                "Cor: Branco quente/amarelado\n" +
                "λmax ≈ 1073 nm (Infravermelho)",
                "Temperatura: Lâmpada",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }

        private void TempLava_Click(object sender, RoutedEventArgs e)
        {
            SliderTemperatura.Value = 1500;
            MessageBox.Show(
                "Lava Vulcânica: ~1500K\n\n" +
                "Cor: Vermelho intenso\n" +
                "λmax ≈ 1932 nm (Infravermelho)",
                "Temperatura: Lava",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }

        private void TempSol_Click(object sender, RoutedEventArgs e)
        {
            SliderTemperatura.Value = 5778;
            MessageBox.Show(
                "Sol (superfície): 5778K\n\n" +
                "Cor: Branco (pico no verde!)\n" +
                "λmax ≈ 501 nm (Verde)\n\n" +
                "Por isso nossos olhos evoluíram\n" +
                "para serem mais sensíveis ao verde!",
                "Temperatura: Sol",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }

        private void TempSirius_Click(object sender, RoutedEventArgs e)
        {
            SliderTemperatura.Value = 9940;
            MessageBox.Show(
                "Sirius (Estrela): ~9940K\n\n" +
                "Cor: Branco-azulado\n" +
                "λmax ≈ 291 nm (UV)\n\n" +
                "É a estrela mais brilhante\n" +
                "no céu noturno!",
                "Temperatura: Sirius",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }

        private void TempEstrelaAzul_Click(object sender, RoutedEventArgs e)
        {
            SliderTemperatura.Value = 10000;
            MessageBox.Show(
                "Estrela Azul: ~10000K\n\n" +
                "Cor: Azul intenso\n" +
                "λmax ≈ 290 nm (UV)\n\n" +
                "Estrelas mais quentes são azuis,\n" +
                "mais frias são vermelhas!",
                "Temperatura: Estrela Azul",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }

        #endregion
    }
}