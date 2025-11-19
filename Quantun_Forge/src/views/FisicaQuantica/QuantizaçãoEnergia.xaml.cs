using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace Quantun_Forge.src.views.FisicaQuantica
{
    /// <summary>
    /// Lógica interna para QuantizaçãoEnergia.xaml
    /// Simulador de Quantização de Energia sem Q#
    /// </summary>
    public partial class QuantizaçãoEnergia : UserControl
    {
        #region Constantes Físicas

        private const double PLANCK_CONSTANT = 6.62607015e-34;      // J·s
        private const double SPEED_OF_LIGHT = 299792458;            // m/s
        private const double BOLTZMANN_CONSTANT = 1.380649e-23;    // J/K
        private const double WIEN_CONSTANT = 2.897771955e-3;       // m·K
        private const double ELECTRON_VOLT = 1.602176634e-19;      // J
        private const double ENERGIA_FUNDAMENTAL = -13.6;          // eV (Hidrogênio)

        #endregion

        #region Variáveis de Estado

        private int nivelAtual = 1;
        private double temperaturaAtual = 3000;
        private readonly double[] posYNiveis = { 350, 270, 190, 110, 50 }; // Posições Y dos níveis

        #endregion

        public QuantizaçãoEnergia()
        {
            InitializeComponent();
            Loaded += QuantizacaoEnergia_Loaded;
        }

        private void QuantizacaoEnergia_Loaded(object sender, RoutedEventArgs e)
        {
            InicializarSimulador();
        }

        #region Inicialização

        private void InicializarSimulador()
        {
            // Configurar estado inicial
            AtualizarEstadoAtual();
            DestacarNivelAtual();

            // Desenhar gráficos
            DesenharCurvaPlanck();
            AtualizarCorTemperatura();

            // Limpar informações de transição
            TxtTransicao.Text = "Aguardando transição...";
            TxtFotonEmitido.Text = "ΔE = 0.00 eV";
        }

        #endregion

        #region Cálculos Físicos - Níveis de Energia

        /// <summary>
        /// Calcula a energia de um nível quântico
        /// Eₙ = E₁ / n²
        /// </summary>
        private double CalcularEnergiaNivel(int n)
        {
            return ENERGIA_FUNDAMENTAL / (n * n);
        }

        /// <summary>
        /// Calcula a diferença de energia entre dois níveis
        /// ΔE = |Ef - Ei|
        /// </summary>
        private double CalcularEnergiaTransicao(int nivelInicial, int nivelFinal)
        {
            double energiaInicial = CalcularEnergiaNivel(nivelInicial);
            double energiaFinal = CalcularEnergiaNivel(nivelFinal);
            return Math.Abs(energiaFinal - energiaInicial);
        }

        /// <summary>
        /// Verifica se a transição emite fóton (descida) ou absorve (subida)
        /// </summary>
        private bool TransicaoEmiteFoton(int nivelInicial, int nivelFinal)
        {
            return nivelFinal < nivelInicial;
        }

        /// <summary>
        /// Converte energia de eV para Joules
        /// </summary>
        private double eVParaJoules(double energiaEV)
        {
            return energiaEV * ELECTRON_VOLT;
        }

        /// <summary>
        /// Calcula o comprimento de onda correspondente a uma energia
        /// λ = hc/E
        /// </summary>
        private double EnergiaParaLambda(double energiaJoules)
        {
            return (PLANCK_CONSTANT * SPEED_OF_LIGHT) / energiaJoules;
        }

        #endregion

        #region Cálculos Físicos - Corpo Negro

        /// <summary>
        /// Calcula o comprimento de onda máximo pela Lei de Wien
        /// λmax = b / T
        /// </summary>
        private double CalcularLambdaMaxWien(double temperatura)
        {
            return WIEN_CONSTANT / temperatura;
        }

        /// <summary>
        /// Calcula a intensidade espectral pela Lei de Planck
        /// I(λ,T) = (2hc²/λ⁵) / (e^(hc/λkT) - 1)
        /// </summary>
        private double CalcularIntensidadePlanck(double wavelength, double temperatura)
        {
            double numerador = 2.0 * PLANCK_CONSTANT * Math.Pow(SPEED_OF_LIGHT, 2) / Math.Pow(wavelength, 5);
            double expoente = (PLANCK_CONSTANT * SPEED_OF_LIGHT) / (wavelength * BOLTZMANN_CONSTANT * temperatura);
            double denominador = Math.Exp(expoente) - 1.0;

            return numerador / denominador;
        }

        /// <summary>
        /// Gera pontos da curva de Planck
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

            // Calcular intensidades
            var intensidades = new List<double>();
            for (int i = 0; i < numPontos; i++)
            {
                double lambda_nm = lambdaMin + i * step;
                double lambda_m = lambda_nm * 1e-9; // Converter para metros
                double intensidade = CalcularIntensidadePlanck(lambda_m, temperatura);
                intensidades.Add(intensidade);
                if (intensidade > maxIntensidade)
                    maxIntensidade = intensidade;
            }

            // Normalizar
            for (int i = 0; i < numPontos; i++)
            {
                double lambda_nm = lambdaMin + i * step;
                double intensidadeNormalizada = intensidades[i] / maxIntensidade;
                pontos.Add((lambda_nm, intensidadeNormalizada));
            }

            return pontos;
        }

        /// <summary>
        /// Converte comprimento de onda para cor RGB
        /// </summary>
        private Color LambdaParaCor(double wavelength)
        {
            double r = 0, g = 0, b = 0;

            // Espectro visível: 380-750 nm
            if (wavelength >= 380 && wavelength < 440)
            {
                r = -(wavelength - 440) / (440 - 380);
                g = 0;
                b = 1;
            }
            else if (wavelength >= 440 && wavelength < 490)
            {
                r = 0;
                g = (wavelength - 440) / (490 - 440);
                b = 1;
            }
            else if (wavelength >= 490 && wavelength < 510)
            {
                r = 0;
                g = 1;
                b = -(wavelength - 510) / (510 - 490);
            }
            else if (wavelength >= 510 && wavelength < 580)
            {
                r = (wavelength - 510) / (580 - 510);
                g = 1;
                b = 0;
            }
            else if (wavelength >= 580 && wavelength < 645)
            {
                r = 1;
                g = -(wavelength - 645) / (645 - 580);
                b = 0;
            }
            else if (wavelength >= 645 && wavelength <= 750)
            {
                r = 1;
                g = 0;
                b = 0;
            }
            else if (wavelength > 750 && wavelength < 1000)
            {
                // Infravermelho próximo
                r = 0.5;
                g = 0;
                b = 0;
            }
            else if (wavelength < 380)
            {
                // Ultravioleta
                r = 0.3;
                g = 0;
                b = 0.5;
            }
            else
            {
                // Infravermelho distante
                r = 0.3;
                g = 0;
                b = 0;
            }

            // Ajuste de intensidade
            double factor;
            if (wavelength >= 380 && wavelength < 420)
                factor = 0.3 + 0.7 * (wavelength - 380) / (420 - 380);
            else if (wavelength >= 420 && wavelength < 700)
                factor = 1.0;
            else if (wavelength >= 700 && wavelength <= 750)
                factor = 0.3 + 0.7 * (750 - wavelength) / (750 - 700);
            else
                factor = 0.3;

            byte R = (byte)(255 * r * factor);
            byte G = (byte)(255 * g * factor);
            byte B = (byte)(255 * b * factor);

            return Color.FromRgb(R, G, B);
        }

        #endregion

        #region Transições entre Níveis

        private void TransicaoNivel1_Click(object sender, RoutedEventArgs e)
        {
            RealizarTransicao(1);
        }

        private void TransicaoNivel2_Click(object sender, RoutedEventArgs e)
        {
            RealizarTransicao(2);
        }

        private void TransicaoNivel3_Click(object sender, RoutedEventArgs e)
        {
            RealizarTransicao(3);
        }

        private void TransicaoNivel4_Click(object sender, RoutedEventArgs e)
        {
            RealizarTransicao(4);
        }

        private void TransicaoNivel5_Click(object sender, RoutedEventArgs e)
        {
            RealizarTransicao(5);
        }

        /// <summary>
        /// Executa a transição entre níveis
        /// </summary>
        private void RealizarTransicao(int nivelDestino)
        {
            if (nivelDestino == nivelAtual)
            {
                MessageBox.Show(
                    $"O elétron já está no nível n={nivelAtual}!",
                    "Mesma Posição",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
                return;
            }

            // Calcular energia da transição
            double deltaE = CalcularEnergiaTransicao(nivelAtual, nivelDestino);
            bool emiteFoton = TransicaoEmiteFoton(nivelAtual, nivelDestino);

            // Animar transição
            AnimarEletron(nivelAtual, nivelDestino);

            if (emiteFoton)
            {
                AnimarFoton();
            }

            // Atualizar informações
            AtualizarInfoTransicao(nivelAtual, nivelDestino, deltaE, emiteFoton);

            // Atualizar estado
            nivelAtual = nivelDestino;
            AtualizarEstadoAtual();
            DestacarNivelAtual();
        }

        #endregion

        #region Animações

        /// <summary>
        /// Anima o movimento do elétron
        /// </summary>
        private void AnimarEletron(int nivelInicial, int nivelFinal)
        {
            double yInicial = posYNiveis[nivelInicial - 1];
            double yFinal = posYNiveis[nivelFinal - 1];

            var anim = new DoubleAnimation
            {
                From = yInicial,
                To = yFinal,
                Duration = TimeSpan.FromSeconds(0.8),
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseInOut }
            };

            Electron.BeginAnimation(Canvas.TopProperty, anim);
        }

        /// <summary>
        /// Anima a emissão do fóton
        /// </summary>
        private void AnimarFoton()
        {
            // Animação de opacidade (piscar)
            var opacityAnim = new DoubleAnimation
            {
                From = 0,
                To = 1,
                Duration = TimeSpan.FromSeconds(0.3),
                AutoReverse = true
            };

            // Animação de posição (movimento)
            var posAnim = new DoubleAnimation
            {
                From = 410,
                To = 600,
                Duration = TimeSpan.FromSeconds(0.6)
            };

            PhotonPath.BeginAnimation(OpacityProperty, opacityAnim);
            PhotonPath.BeginAnimation(Canvas.LeftProperty, posAnim);
        }

        #endregion

        #region Atualização de Interface

        /// <summary>
        /// Atualiza as informações do estado atual
        /// </summary>
        private void AtualizarEstadoAtual()
        {
            double energia = CalcularEnergiaNivel(nivelAtual);
            TxtEstadoAtual.Text = $"Nível n = {nivelAtual}";
            TxtEnergiaAtual.Text = $"E₁ = {energia:F2} eV";
        }

        /// <summary>
        /// Atualiza as informações da transição
        /// </summary>
        private void AtualizarInfoTransicao(int nivelInicial, int nivelFinal, double deltaE, bool emiteFoton)
        {
            string tipoTransicao = emiteFoton ? "Emissão" : "Absorção";
            string seta = emiteFoton ? "↓" : "↑";

            TxtTransicao.Text = $"{tipoTransicao}: n={nivelInicial} {seta} n={nivelFinal}";

            // Calcular comprimento de onda
            double energiaJoules = eVParaJoules(deltaE);
            double lambda = EnergiaParaLambda(energiaJoules);
            double lambdaNm = lambda * 1e9;

            TxtFotonEmitido.Text = $"ΔE = {deltaE:F2} eV (λ ≈ {lambdaNm:F0} nm)";
        }

        /// <summary>
        /// Destaca visualmente o nível atual
        /// </summary>
        private void DestacarNivelAtual()
        {
            // Resetar todos os níveis
            Level1.Opacity = 0.5;
            Level2.Opacity = 0.5;
            Level3.Opacity = 0.5;
            Level4.Opacity = 0.5;
            Level5.Opacity = 0.5;

            // Destacar nível atual
            switch (nivelAtual)
            {
                case 1: Level1.Opacity = 1.0; break;
                case 2: Level2.Opacity = 1.0; break;
                case 3: Level3.Opacity = 1.0; break;
                case 4: Level4.Opacity = 1.0; break;
                case 5: Level5.Opacity = 1.0; break;
            }
        }

        #endregion

        #region Corpo Negro - Controles

        private void SliderTemperatura_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (!IsLoaded) return;

            temperaturaAtual = e.NewValue;
            TxtTemperaturaSlider.Text = $"{temperaturaAtual:F0} K";
            TxtTemperatura.Text = $"{temperaturaAtual:F0} K";

            DesenharCurvaPlanck();
            AtualizarCorTemperatura();
        }

        private void TempVela_Click(object sender, RoutedEventArgs e)
        {
            SliderTemperatura.Value = 1800;
        }

        private void TempLampada_Click(object sender, RoutedEventArgs e)
        {
            SliderTemperatura.Value = 3000;
        }

        private void TempSol_Click(object sender, RoutedEventArgs e)
        {
            SliderTemperatura.Value = 5778;
        }

        private void TempEstrela_Click(object sender, RoutedEventArgs e)
        {
            SliderTemperatura.Value = 10000;
        }

        #endregion

        #region Visualização do Espectro

        /// <summary>
        /// Desenha a curva de Planck
        /// </summary>
        private void DesenharCurvaPlanck()
        {
            if (SpectrumCanvas == null) return;

            SpectrumCanvas.Children.Clear();

            // Desenhar eixos
            DesenharEixos();

            // Gerar e desenhar curva
            var pontos = GerarCurvaPlanck(temperaturaAtual, 200, 100, 3000);

            var polyline = new Polyline
            {
                Stroke = new SolidColorBrush(Color.FromRgb(231, 76, 60)),
                StrokeThickness = 3,
                StrokeLineJoin = PenLineJoin.Round
            };

            double canvasWidth = SpectrumCanvas.ActualWidth > 0 ? SpectrumCanvas.ActualWidth : 800;
            double canvasHeight = SpectrumCanvas.ActualHeight > 0 ? SpectrumCanvas.ActualHeight : 250;

            foreach (var ponto in pontos)
            {
                double x = 50 + (ponto.lambda - 100) / 2900.0 * (canvasWidth - 100);
                double y = canvasHeight - 30 - (ponto.intensidade * (canvasHeight - 50));
                polyline.Points.Add(new Point(x, y));
            }

            SpectrumCanvas.Children.Add(polyline);

            // Linha do lambda máximo
            double lambdaMax = CalcularLambdaMaxWien(temperaturaAtual);
            double lambdaMaxNm = lambdaMax * 1e9;
            TxtLambdaMax.Text = $"{lambdaMaxNm:F0} nm";

            if (lambdaMaxNm >= 100 && lambdaMaxNm <= 3000)
            {
                double xMax = 50 + (lambdaMaxNm - 100) / 2900.0 * (canvasWidth - 100);
                var linhaMax = new Line
                {
                    X1 = xMax,
                    Y1 = 30,
                    X2 = xMax,
                    Y2 = canvasHeight - 30,
                    Stroke = new SolidColorBrush(Color.FromRgb(243, 156, 18)),
                    StrokeThickness = 2,
                    StrokeDashArray = new DoubleCollection { 5, 3 }
                };
                SpectrumCanvas.Children.Add(linhaMax);

                // Label do lambda máximo
                var labelMax = new TextBlock
                {
                    Text = "λmax",
                    FontSize = 10,
                    FontWeight = FontWeights.Bold,
                    Foreground = new SolidColorBrush(Color.FromRgb(243, 156, 18))
                };
                Canvas.SetLeft(labelMax, xMax - 15);
                Canvas.SetTop(labelMax, 10);
                SpectrumCanvas.Children.Add(labelMax);
            }
        }

        /// <summary>
        /// Desenha os eixos do gráfico
        /// </summary>
        private void DesenharEixos()
        {
            double canvasWidth = SpectrumCanvas.ActualWidth > 0 ? SpectrumCanvas.ActualWidth : 800;
            double canvasHeight = SpectrumCanvas.ActualHeight > 0 ? SpectrumCanvas.ActualHeight : 250;

            // Eixo X
            var eixoX = new Line
            {
                X1 = 50,
                Y1 = canvasHeight - 30,
                X2 = canvasWidth - 30,
                Y2 = canvasHeight - 30,
                Stroke = Brushes.Black,
                StrokeThickness = 2
            };
            SpectrumCanvas.Children.Add(eixoX);

            // Eixo Y
            var eixoY = new Line
            {
                X1 = 50,
                Y1 = 30,
                X2 = 50,
                Y2 = canvasHeight - 30,
                Stroke = Brushes.Black,
                StrokeThickness = 2
            };
            SpectrumCanvas.Children.Add(eixoY);

            // Labels dos eixos
            var labelX = new TextBlock
            {
                Text = "λ (nm)",
                FontWeight = FontWeights.Bold,
                FontSize = 11,
                Foreground = Brushes.Black
            };
            Canvas.SetLeft(labelX, canvasWidth - 80);
            Canvas.SetTop(labelX, canvasHeight - 25);
            SpectrumCanvas.Children.Add(labelX);

            var labelY = new TextBlock
            {
                Text = "I(λ)",
                FontWeight = FontWeights.Bold,
                FontSize = 11,
                Foreground = Brushes.Black
            };
            Canvas.SetLeft(labelY, 15);
            Canvas.SetTop(labelY, 35);
            SpectrumCanvas.Children.Add(labelY);

            // Marcações do eixo X
            for (int i = 500; i <= 2500; i += 500)
            {
                double x = 50 + (i - 100) / 2900.0 * (canvasWidth - 100);

                var tick = new Line
                {
                    X1 = x,
                    Y1 = canvasHeight - 30,
                    X2 = x,
                    Y2 = canvasHeight - 25,
                    Stroke = Brushes.Black,
                    StrokeThickness = 1
                };
                SpectrumCanvas.Children.Add(tick);

                var label = new TextBlock
                {
                    Text = i.ToString(),
                    FontSize = 9,
                    Foreground = Brushes.Gray
                };
                Canvas.SetLeft(label, x - 12);
                Canvas.SetTop(label, canvasHeight - 20);
                SpectrumCanvas.Children.Add(label);
            }
        }

        /// <summary>
        /// Atualiza a cor do preview
        /// </summary>
        private void AtualizarCorTemperatura()
        {
            double lambdaMax = CalcularLambdaMaxWien(temperaturaAtual);
            double lambdaMaxNm = lambdaMax * 1e9;

            Color cor = LambdaParaCor(lambdaMaxNm);
            ColorPreview.Fill = new SolidColorBrush(cor);
        }

        #endregion

        #region Resetar

        private void Resetar_Click(object sender, RoutedEventArgs e)
        {
            // Resetar para estado fundamental
            nivelAtual = 1;

            // Reposicionar elétron
            Canvas.SetTop(Electron, posYNiveis[0]);

            // Atualizar interface
            AtualizarEstadoAtual();
            DestacarNivelAtual();

            // Limpar transição
            TxtTransicao.Text = "Aguardando transição...";
            TxtFotonEmitido.Text = "ΔE = 0.00 eV";

            // Resetar fóton
            PhotonPath.Opacity = 0;
        }

        #endregion
    }
}