using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Quantun_Forge.src.views.FisicaQuantica
{
    /// <summary>
    /// Simulador Visual de Dualidade Onda-Partícula
    /// Demonstra o comportamento dual da luz e matéria
    /// </summary>
    public partial class DualidadeOndaParticula : UserControl
    {
        #region Constantes Físicas

        private const double PLANCK_CONSTANT = 6.62607015e-34;  // J·s
        private const double ELECTRON_MASS = 9.10938356e-31;    // kg
        private const double SPEED_OF_LIGHT = 299792458;        // m/s

        #endregion

        #region Enums

        private enum ModoVisualizacao
        {
            Onda,
            Particula,
            Dual
        }

        #endregion

        #region Variáveis de Estado

        private ModoVisualizacao modoAtual = ModoVisualizacao.Onda;
        private DispatcherTimer simulationTimer;
        private DispatcherTimer waveTimer;
        private Random random = new Random();

        private bool simulacaoAtiva = false;
        private int deteccoes = 0;
        private double velocidade = 1.0e6; // m/s
        private double massa = ELECTRON_MASS;

        private List<Ellipse> particulas = new List<Ellipse>();
        private int[] distribuicaoIntensidade = new int[300]; // Array para padrão de interferência

        private double wavePhase = 0;
        private double distanciaFendas = 35;

        #endregion

        public DualidadeOndaParticula()
        {
            InitializeComponent();
            InicializarSimulador();
        }

        #region Inicialização

        private void InicializarSimulador()
        {
            // Timer principal da simulação
            simulationTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(50)
            };
            simulationTimer.Tick += SimulationTimer_Tick;

            // Timer para animação de ondas
            waveTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(30)
            };
            waveTimer.Tick += WaveTimer_Tick;

            // Inicializar visualizações
            DesenharOndasComparativas();
            DesenharParticulasComparativas();
            AtualizarLambdaDeBroglie();
        }

        #endregion

        #region Cálculos Físicos

        /// <summary>
        /// Calcula o comprimento de onda de De Broglie
        /// λ = h / (m·v)
        /// </summary>
        private double CalcularLambdaDeBroglie(double massa, double velocidade)
        {
            return PLANCK_CONSTANT / (massa * velocidade);
        }

        /// <summary>
        /// Calcula a posição de interferência construtiva/destrutiva
        /// </summary>
        private double CalcularPosicaoInterferencia(int ordem, double lambda, double distanciaFendas, double distanciaTela)
        {
            // Fórmula simplificada: y = (ordem * lambda * L) / d
            return (ordem * lambda * distanciaTela) / distanciaFendas;
        }

        #endregion

        #region Controles de Modo

        private void ToggleModoOnda_Checked(object sender, RoutedEventArgs e)
        {
            if (!IsLoaded) return;

            modoAtual = ModoVisualizacao.Onda;
            TxtModoAtual.Text = "Onda";
            TxtModoAtual.Foreground = new SolidColorBrush(Color.FromRgb(52, 152, 219));

            ToggleModoParticula.IsChecked = false;
            ToggleModoDual.IsChecked = false;
        }

        private void ToggleModoOnda_Unchecked(object sender, RoutedEventArgs e)
        {
            if (ToggleModoParticula.IsChecked == false && ToggleModoDual.IsChecked == false)
                ToggleModoOnda.IsChecked = true;
        }

        private void ToggleModoParticula_Checked(object sender, RoutedEventArgs e)
        {
            if (!IsLoaded) return;

            modoAtual = ModoVisualizacao.Particula;
            TxtModoAtual.Text = "Partícula";
            TxtModoAtual.Foreground = new SolidColorBrush(Color.FromRgb(243, 156, 18));

            ToggleModoOnda.IsChecked = false;
            ToggleModoDual.IsChecked = false;
        }

        private void ToggleModoParticula_Unchecked(object sender, RoutedEventArgs e)
        {
            if (ToggleModoOnda.IsChecked == false && ToggleModoDual.IsChecked == false)
                ToggleModoParticula.IsChecked = true;
        }

        private void ToggleModoDual_Checked(object sender, RoutedEventArgs e)
        {
            if (!IsLoaded) return;

            modoAtual = ModoVisualizacao.Dual;
            TxtModoAtual.Text = "Dual";
            TxtModoAtual.Foreground = new SolidColorBrush(Color.FromRgb(156, 39, 176));

            ToggleModoOnda.IsChecked = false;
            ToggleModoParticula.IsChecked = false;
        }

        private void ToggleModoDual_Unchecked(object sender, RoutedEventArgs e)
        {
            if (ToggleModoOnda.IsChecked == false && ToggleModoParticula.IsChecked == false)
                ToggleModoDual.IsChecked = true;
        }

        #endregion

        #region Controles de Simulação

        private void Iniciar_Click(object sender, RoutedEventArgs e)
        {
            if (!simulacaoAtiva)
            {
                simulacaoAtiva = true;
                TxtStatus.Text = "Executando";
                TxtStatus.Foreground = new SolidColorBrush(Color.FromRgb(46, 204, 113));

                simulationTimer.Start();
                if (modoAtual == ModoVisualizacao.Onda || modoAtual == ModoVisualizacao.Dual)
                {
                    waveTimer.Start();
                }
            }
        }

        private void Pausar_Click(object sender, RoutedEventArgs e)
        {
            simulacaoAtiva = false;
            TxtStatus.Text = "Pausado";
            TxtStatus.Foreground = new SolidColorBrush(Color.FromRgb(243, 156, 18));

            simulationTimer.Stop();
            waveTimer.Stop();
        }

        private void Limpar_Click(object sender, RoutedEventArgs e)
        {
            Pausar_Click(sender, e);

            // Limpar canvas
            ParticlesCanvas.Children.Clear();
            InterferenceCanvas.Children.Clear();
            particulas.Clear();

            // Resetar contadores
            deteccoes = 0;
            TxtDeteccoes.Text = "0";
            Array.Clear(distribuicaoIntensidade, 0, distribuicaoIntensidade.Length);

            TxtStatus.Text = "Parado";
            TxtStatus.Foreground = new SolidColorBrush(Color.FromRgb(149, 165, 166));
        }

        #endregion

        #region Timers de Simulação

        private void SimulationTimer_Tick(object sender, EventArgs e)
        {
            switch (modoAtual)
            {
                case ModoVisualizacao.Onda:
                    SimularComportamentoOnda();
                    break;

                case ModoVisualizacao.Particula:
                    SimularComportamentoParticula();
                    break;

                case ModoVisualizacao.Dual:
                    SimularComportamentoDual();
                    break;
            }
        }

        private void WaveTimer_Tick(object sender, EventArgs e)
        {
            DesenharOndasPropagando();
        }

        #endregion

        #region Simulação de Comportamentos

        /// <summary>
        /// Simula o comportamento ondulatório
        /// </summary>
        private void SimularComportamentoOnda()
        {
            // Ondas já são desenhadas pelo WaveTimer
            // Aqui podemos adicionar efeitos adicionais
        }

        /// <summary>
        /// Simula o comportamento de partícula
        /// </summary>
        private void SimularComportamentoParticula()
        {
            // Criar nova partícula
            int intensidade = (int)SliderIntensidade.Value;

            if (random.Next(0, 11) <= intensidade)
            {
                CriarEEmitirParticula();
            }

            // Atualizar partículas existentes
            AtualizarParticulasExistentes();
        }

        /// <summary>
        /// Simula comportamento dual (partículas criando padrão de onda)
        /// </summary>
        private void SimularComportamentoDual()
        {
            // Emitir partículas que constroem padrão de interferência
            int intensidade = (int)SliderIntensidade.Value;

            if (random.Next(0, 15) <= intensidade)
            {
                CriarEEmitirParticulaComInterferencia();
            }

            AtualizarParticulasExistentes();
            AtualizarPadraoInterferencia();
        }

        #endregion

        #region Criação e Animação de Partículas

        private void CriarEEmitirParticula()
        {
            var particula = new Ellipse
            {
                Width = 6,
                Height = 6,
                Fill = new SolidColorBrush(Color.FromRgb(255, 215, 0)),
                Stroke = new SolidColorBrush(Color.FromRgb(218, 165, 32)),
                StrokeThickness = 1
            };

            double startX = 70;
            double startY = 200;

            Canvas.SetLeft(particula, startX);
            Canvas.SetTop(particula, startY);
            Canvas.SetZIndex(particula, 10);

            ParticlesCanvas.Children.Add(particula);
            particulas.Add(particula);

            AnimarParticula(particula);
        }

        private void CriarEEmitirParticulaComInterferencia()
        {
            var particula = new Ellipse
            {
                Width = 5,
                Height = 5,
                Fill = new SolidColorBrush(Color.FromArgb(200, 156, 39, 176)),
                Stroke = new SolidColorBrush(Color.FromRgb(123, 31, 162)),
                StrokeThickness = 1
            };

            double startX = 70;
            double startY = 200 + random.Next(-5, 6);

            Canvas.SetLeft(particula, startX);
            Canvas.SetTop(particula, startY);
            Canvas.SetZIndex(particula, 10);

            ParticlesCanvas.Children.Add(particula);
            particulas.Add(particula);

            AnimarParticulaComInterferencia(particula);
        }

        private void AnimarParticula(Ellipse particula)
        {
            // Verificar fendas abertas
            bool fendaSuperiorAberta = ChkFendaSuperior.IsChecked == true;
            bool fendaInferiorAberta = ChkFendaInferior.IsChecked == true;

            double targetX = 730;
            double targetY = 200;

            // Se apenas uma fenda está aberta, direcionar para ela
            if (fendaSuperiorAberta && !fendaInferiorAberta)
            {
                targetY = 182.5; // Posição da fenda superior
            }
            else if (!fendaSuperiorAberta && fendaInferiorAberta)
            {
                targetY = 217.5; // Posição da fenda inferior
            }
            else if (fendaSuperiorAberta && fendaInferiorAberta)
            {
                // Ambas abertas: escolher aleatoriamente
                targetY = random.Next(0, 2) == 0 ? 182.5 : 217.5;
                targetY += random.Next(-10, 11); // Adicionar espalhamento
            }

            var animX = new DoubleAnimation
            {
                To = targetX,
                Duration = TimeSpan.FromSeconds(2),
                EasingFunction = new QuadraticEase()
            };

            var animY = new DoubleAnimation
            {
                To = targetY,
                Duration = TimeSpan.FromSeconds(2),
                EasingFunction = new QuadraticEase()
            };

            animX.Completed += (s, e) =>
            {
                RegistrarDeteccao(targetY);
                ParticlesCanvas.Children.Remove(particula);
                particulas.Remove(particula);
            };

            particula.BeginAnimation(Canvas.LeftProperty, animX);
            particula.BeginAnimation(Canvas.TopProperty, animY);
        }

        private void AnimarParticulaComInterferencia(Ellipse particula)
        {
            bool fendaSuperiorAberta = ChkFendaSuperior.IsChecked == true;
            bool fendaInferiorAberta = ChkFendaInferior.IsChecked == true;

            if (!fendaSuperiorAberta && !fendaInferiorAberta)
            {
                ParticlesCanvas.Children.Remove(particula);
                particulas.Remove(particula);
                return;
            }

            double targetX = 730;
            double lambda = CalcularLambdaDeBroglie(massa, velocidade) * 1e9; // nm

            // Calcular posição com padrão de interferência
            double targetY = 200;

            if (fendaSuperiorAberta && fendaInferiorAberta)
            {
                // Padrão de interferência
                int ordem = random.Next(-5, 6);
                double deslocamento = CalcularPosicaoInterferencia(ordem, lambda, distanciaFendas, 450) / 10;
                targetY += deslocamento + random.NextDouble() * 10 - 5;
            }
            else if (fendaSuperiorAberta)
            {
                targetY = 182.5 + random.Next(-15, 16);
            }
            else
            {
                targetY = 217.5 + random.Next(-15, 16);
            }

            // Garantir que fica dentro dos limites
            targetY = Math.Max(50, Math.Min(350, targetY));

            var animX = new DoubleAnimation
            {
                To = targetX,
                Duration = TimeSpan.FromSeconds(2.5),
                EasingFunction = new QuadraticEase()
            };

            var animY = new DoubleAnimation
            {
                To = targetY,
                Duration = TimeSpan.FromSeconds(2.5),
                EasingFunction = new QuadraticEase()
            };

            animX.Completed += (s, e) =>
            {
                RegistrarDeteccaoComInterferencia(targetY);
                ParticlesCanvas.Children.Remove(particula);
                particulas.Remove(particula);
            };

            particula.BeginAnimation(Canvas.LeftProperty, animX);
            particula.BeginAnimation(Canvas.TopProperty, animY);
        }

        private void AtualizarParticulasExistentes()
        {
            // Remove partículas que saíram da tela
            var particulasRemover = new List<Ellipse>();

            foreach (var particula in particulas)
            {
                double left = Canvas.GetLeft(particula);
                if (double.IsNaN(left) || left > 800)
                {
                    particulasRemover.Add(particula);
                }
            }

            foreach (var particula in particulasRemover)
            {
                ParticlesCanvas.Children.Remove(particula);
                particulas.Remove(particula);
            }
        }

        #endregion

        #region Detecção e Padrão de Interferência

        private void RegistrarDeteccao(double posY)
        {
            deteccoes++;
            TxtDeteccoes.Text = deteccoes.ToString();

            // Desenhar ponto na tela de detecção
            var ponto = new Ellipse
            {
                Width = 3,
                Height = 3,
                Fill = Brushes.Red
            };

            Canvas.SetLeft(ponto, 5);
            Canvas.SetTop(ponto, posY - 50);

            InterferenceCanvas.Children.Add(ponto);
        }

        private void RegistrarDeteccaoComInterferencia(double posY)
        {
            deteccoes++;
            TxtDeteccoes.Text = deteccoes.ToString();

            // Registrar na distribuição
            int index = (int)((posY - 50) / 300.0 * distribuicaoIntensidade.Length);
            if (index >= 0 && index < distribuicaoIntensidade.Length)
            {
                distribuicaoIntensidade[index]++;
            }

            // Desenhar ponto
            var ponto = new Ellipse
            {
                Width = 2,
                Height = 2,
                Fill = new SolidColorBrush(Color.FromArgb(150, 156, 39, 176))
            };

            Canvas.SetLeft(ponto, random.Next(0, 20));
            Canvas.SetTop(ponto, posY - 50);

            InterferenceCanvas.Children.Add(ponto);
        }

        private void AtualizarPadraoInterferencia()
        {
            // A cada 50 detecções, redesenhar o padrão
            if (deteccoes % 50 == 0 && deteccoes > 0)
            {
                // Aqui poderia desenhar um gráfico de intensidade
                // Por simplicidade, os pontos individuais já formam o padrão
            }
        }

        #endregion

        #region Desenho de Ondas

        private void DesenharOndasPropagando()
        {
            ParticlesCanvas.Children.Clear();

            wavePhase += 0.2;
            if (wavePhase > 2 * Math.PI) wavePhase = 0;

            bool fendaSuperiorAberta = ChkFendaSuperior.IsChecked == true;
            bool fendaInferiorAberta = ChkFendaInferior.IsChecked == true;

            // Onda indo até as fendas
            DesenharOnda(70, 200, 280, 200, wavePhase, Colors.Cyan, 2);

            // Ondas saindo das fendas
            if (fendaSuperiorAberta)
            {
                DesenharOndaCircular(290, 182.5, wavePhase, Colors.Cyan);
            }

            if (fendaInferiorAberta)
            {
                DesenharOndaCircular(290, 217.5, wavePhase, Colors.Cyan);
            }
        }

        private void DesenharOnda(double x1, double y1, double x2, double y2, double phase, Color cor, double amplitude)
        {
            var polyline = new Polyline
            {
                Stroke = new SolidColorBrush(cor),
                StrokeThickness = 2,
                Opacity = 0.7
            };

            int numPontos = 50;
            double deltaX = (x2 - x1) / numPontos;

            for (int i = 0; i <= numPontos; i++)
            {
                double x = x1 + i * deltaX;
                double y = y1 + amplitude * Math.Sin((i * 0.5) + phase);
                polyline.Points.Add(new Point(x, y));
            }

            ParticlesCanvas.Children.Add(polyline);
        }

        private void DesenharOndaCircular(double centerX, double centerY, double phase, Color cor)
        {
            // Desenhar frentes de onda circulares
            for (int i = 1; i <= 5; i++)
            {
                double radius = 30 * i + (phase * 10);
                if (radius > 500) continue;

                var ellipse = new Ellipse
                {
                    Width = radius * 2,
                    Height = radius * 2,
                    Stroke = new SolidColorBrush(Color.FromArgb((byte)(50 / i), cor.R, cor.G, cor.B)),
                    StrokeThickness = 1.5
                };

                Canvas.SetLeft(ellipse, centerX - radius);
                Canvas.SetTop(ellipse, centerY - radius);

                ParticlesCanvas.Children.Add(ellipse);
            }
        }

        private void DesenharOndasComparativas()
        {
            WaveCanvas.Children.Clear();

            // Desenhar onda senoidal
            var polyline = new Polyline
            {
                Stroke = new SolidColorBrush(Color.FromRgb(33, 150, 243)),
                StrokeThickness = 3
            };

            double width = WaveCanvas.ActualWidth > 0 ? WaveCanvas.ActualWidth : 350;
            double height = WaveCanvas.ActualHeight > 0 ? WaveCanvas.ActualHeight : 150;

            for (int i = 0; i <= 100; i++)
            {
                double x = (i / 100.0) * width;
                double y = height / 2 + (height / 3) * Math.Sin((i / 100.0) * 4 * Math.PI);
                polyline.Points.Add(new Point(x, y));
            }

            WaveCanvas.Children.Add(polyline);

            // Adicionar labels
            var label = new TextBlock
            {
                Text = "λ",
                FontSize = 16,
                FontWeight = FontWeights.Bold,
                Foreground = new SolidColorBrush(Color.FromRgb(33, 150, 243))
            };
            Canvas.SetLeft(label, width / 2);
            Canvas.SetTop(label, 10);
            WaveCanvas.Children.Add(label);
        }

        private void DesenharParticulasComparativas()
        {
            ParticleCanvas.Children.Clear();

            double width = ParticleCanvas.ActualWidth > 0 ? ParticleCanvas.ActualWidth : 350;
            double height = ParticleCanvas.ActualHeight > 0 ? ParticleCanvas.ActualHeight : 150;

            // Desenhar várias partículas
            for (int i = 0; i < 20; i++)
            {
                var particula = new Ellipse
                {
                    Width = 8,
                    Height = 8,
                    Fill = new SolidColorBrush(Color.FromRgb(255, 152, 0))
                };

                double x = random.NextDouble() * width;
                double y = random.NextDouble() * height;

                Canvas.SetLeft(particula, x);
                Canvas.SetTop(particula, y);

                ParticleCanvas.Children.Add(particula);
            }
        }

        #endregion

        #region Sliders e Ajustes

        private void SliderVelocidade_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (!IsLoaded) return;

            velocidade = e.NewValue * 1e5; // Converter para m/s
            TxtVelocidadeSlider.Text = $"{e.NewValue:F1}×10⁵ m/s";
            TxtVelocidade.Text = $"{velocidade:E2} m/s";

            AtualizarLambdaDeBroglie();
        }

        private void SliderDistanciaFendas_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (!IsLoaded) return;

            distanciaFendas = e.NewValue;
            TxtDistanciaFendas.Text = $"{e.NewValue:F0} unidades";

            // Atualizar posição visual das fendas
            Canvas.SetTop(SlitTop, 200 - distanciaFendas / 2 - 12.5);
            Canvas.SetTop(SlitBottom, 200 + distanciaFendas / 2 - 12.5);
        }

        private void ChkFenda_Changed(object sender, RoutedEventArgs e)
        {
            if (!IsLoaded) return;

            bool superior = ChkFendaSuperior.IsChecked == true;
            bool inferior = ChkFendaInferior.IsChecked == true;

            SlitTop.Opacity = superior ? 0.5 : 0.1;
            SlitBottom.Opacity = inferior ? 0.5 : 0.1;
        }

        private void AtualizarLambdaDeBroglie()
        {
            double lambda = CalcularLambdaDeBroglie(massa, velocidade);
            double lambdaNm = lambda * 1e9; // Converter para nanômetros

            TxtLambdaBroglie.Text = $"{lambdaNm:F3} nm";
        }

        #endregion

        #region Cenários Pré-definidos

        private void CenarioFotons_Click(object sender, RoutedEventArgs e)
        {
            massa = PLANCK_CONSTANT / SPEED_OF_LIGHT; // Fóton: p = E/c
            velocidade = SPEED_OF_LIGHT;

            TxtMassa.Text = "Fóton (sem massa de repouso)";
            TxtVelocidade.Text = "3.0×10⁸ m/s";

            double lambda = 500e-9; // 500 nm (luz verde)
            TxtLambdaBroglie.Text = "500 nm";

            SliderVelocidade.Value = 10;

            MessageBox.Show(
                "Fótons são partículas de luz sem massa de repouso!\n\n" +
                "λ ≈ 400-700 nm (luz visível)",
                "Cenário: Fótons",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }

        private void CenarioEletrons_Click(object sender, RoutedEventArgs e)
        {
            massa = ELECTRON_MASS;
            velocidade = 1.0e6;

            TxtMassa.Text = "9.11×10⁻³¹ kg";
            TxtVelocidade.Text = "1.0×10⁶ m/s";

            SliderVelocidade.Value = 10;
            AtualizarLambdaDeBroglie();

            MessageBox.Show(
                "Elétrons são as partículas que comprovaram\n" +
                "experimentalmente a dualidade onda-partícula!\n\n" +
                "λ ≈ 0.7 nm (similar a raios-X)",
                "Cenário: Elétrons",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }

        private void CenarioAtomos_Click(object sender, RoutedEventArgs e)
        {
            massa = 1.67e-27; // Próton/Nêutron
            velocidade = 1000;

            TxtMassa.Text = "1.67×10⁻²⁷ kg";
            TxtVelocidade.Text = "1.0×10³ m/s";

            SliderVelocidade.Value = 1;
            AtualizarLambdaDeBroglie();

            MessageBox.Show(
                "Até átomos completos exibem comportamento ondulatório!\n\n" +
                "Experimentos com C₆₀ (fulereno) já foram realizados.",
                "Cenário: Átomos",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }

        private void CenarioBola_Click(object sender, RoutedEventArgs e)
        {
            massa = 0.058; // Bola de tênis: ~58g
            velocidade = 30;

            TxtMassa.Text = "0.058 kg";
            TxtVelocidade.Text = "30 m/s";

            SliderVelocidade.Value = 1;
            AtualizarLambdaDeBroglie();

            MessageBox.Show(
                "Objetos macroscópicos também têm comprimento de onda,\n" +
                "mas é tão pequeno que é impossível detectar!\n\n" +
                "λ ≈ 10⁻³⁴ m (indetectável)",
                "Cenário: Bola de Tênis",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }

        #endregion
    }
}