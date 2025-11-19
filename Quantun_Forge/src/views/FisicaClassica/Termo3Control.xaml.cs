using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Quantun_Forge.src.views.FisicaClassica
{
    public partial class Termo3Control : UserControl
    {
        private Random random = new Random();
        private List<Ellipse> particulas = new List<Ellipse>();
        private DispatcherTimer animationTimer;
        private int contadorSimulacoes = 0;
        private double tempMinima = 300;
        private bool isAnimating = false;

        public Termo3Control()
        {
            InitializeComponent();
            Loaded += Termo3Control_Loaded;

            // Timer para animação contínua
            animationTimer = new DispatcherTimer();
            animationTimer.Interval = TimeSpan.FromMilliseconds(50);
            animationTimer.Tick += AnimationTimer_Tick;
        }

        private void Termo3Control_Loaded(object sender, RoutedEventArgs e)
        {
            CriarParticulas();
            AtualizarVisual(SliderTemp.Value);
        }

        // CRIAR PARTÍCULAS DINAMICAMENTE
        private void CriarParticulas()
        {
            particulas.Clear();

            // Criar 30 partículas distribuídas no canvas
            for (int i = 0; i < 30; i++)
            {
                var particula = new Ellipse
                {
                    Width = 10,
                    Height = 10,
                    Fill = new SolidColorBrush(Color.FromRgb(231, 76, 60)),
                    Effect = new System.Windows.Media.Effects.DropShadowEffect
                    {
                        Color = Color.FromRgb(231, 76, 60),
                        BlurRadius = 8,
                        ShadowDepth = 0
                    }
                };

                // Posição aleatória
                double x = random.Next(50, 750);
                double y = random.Next(50, 350);

                Canvas.SetLeft(particula, x);
                Canvas.SetTop(particula, y);

                CanvasParticulas.Children.Add(particula);
                particulas.Add(particula);
            }
        }

        // SLIDER DE TEMPERATURA
        private void SliderTemp_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (TxtTempSlider == null) return;

            double temp = e.NewValue;
            double celsius = temp - 273;

            TxtTempSlider.Text = $"T = {temp:F0} K ({celsius:F0}°C)";

            AtualizarVisual(temp);

            // Atualizar temperatura mínima
            if (temp < tempMinima)
            {
                tempMinima = temp;
                TxtTempMinima.Text = $"{tempMinima:F1} K";
            }
        }

        // ATUALIZAR VISUALIZAÇÃO COMPLETA
        private void AtualizarVisual(double temperatura)
        {
            if (TxtTempDisplay == null) return;

            double celsius = temperatura - 273;

            // Atualizar displays de temperatura
            TxtTempDisplay.Text = $"{temperatura:F0} K ({celsius:F0}°C)";
            TxtTempResultado.Text = $"{temperatura:F0} K";

            // Calcular entropia relativa (0 a 100%)
            double entropiaRelativa = (temperatura / 300.0) * 100;
            TxtEntropiaPercent.Text = $"{entropiaRelativa:F1}%";
            TxtEntropiaResultado.Text = $"{entropiaRelativa:F1}%";

            // Atualizar barra de entropia
            double larguraEntropia = (entropiaRelativa / 100.0) * 200;
            var animBarra = new DoubleAnimation(larguraEntropia, TimeSpan.FromMilliseconds(600))
            {
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseOut }
            };
            BarraEntropiaInfo.BeginAnimation(FrameworkElement.WidthProperty, animBarra);

            // Atualizar estado do sistema
            AtualizarEstadoSistema(temperatura);

            // Atualizar agitação molecular
            AtualizarAgitacao(temperatura);

            // Atualizar cores das partículas
            AtualizarCoresParticulas(temperatura);

            // Atualizar marcador no gráfico
            AtualizarMarcadorGrafico(temperatura);

            // Atualizar interpretação
            AtualizarInterpretacao(temperatura, entropiaRelativa);

            // Atualizar análise detalhada
            AtualizarAnaliseDetalhada(temperatura);

            // Atualizar dica
            AtualizarDica(temperatura);

            // Incrementar contador
            contadorSimulacoes++;
            TxtContadorSimulacoes.Text = contadorSimulacoes.ToString();
        }

        // ATUALIZAR ESTADO DO SISTEMA
        private void AtualizarEstadoSistema(double temperatura)
        {
            string estado;
            Color cor;

            if (temperatura > 200)
            {
                estado = "🔥 Alta Temperatura";
                cor = Color.FromRgb(231, 76, 60);
            }
            else if (temperatura > 100)
            {
                estado = "🌡️ Temperatura Moderada";
                cor = Color.FromRgb(243, 156, 18);
            }
            else if (temperatura > 10)
            {
                estado = "❄️ Baixa Temperatura";
                cor = Color.FromRgb(52, 152, 219);
            }
            else
            {
                estado = "🧊 Próximo ao Zero Absoluto";
                cor = Color.FromRgb(155, 89, 182);
            }

            TxtEstadoSistema.Text = estado;
            TxtEstadoSistema.Foreground = new SolidColorBrush(cor);
        }

        // ATUALIZAR AGITAÇÃO MOLECULAR
        private void AtualizarAgitacao(double temperatura)
        {
            string agitacao;
            Color cor;

            if (temperatura > 200)
            {
                agitacao = "Muito Alta";
                cor = Color.FromRgb(231, 76, 60);
            }
            else if (temperatura > 100)
            {
                agitacao = "Alta";
                cor = Color.FromRgb(243, 156, 18);
            }
            else if (temperatura > 50)
            {
                agitacao = "Moderada";
                cor = Color.FromRgb(52, 152, 219);
            }
            else if (temperatura > 10)
            {
                agitacao = "Baixa";
                cor = Color.FromRgb(26, 188, 156);
            }
            else
            {
                agitacao = "Quase Nula";
                cor = Color.FromRgb(155, 89, 182);
            }

            TxtAgitacao.Text = agitacao;
            TxtAgitacao.Foreground = new SolidColorBrush(cor);
        }

        // ATUALIZAR CORES DAS PARTÍCULAS
        private void AtualizarCoresParticulas(double temperatura)
        {
            Color cor;

            if (temperatura > 200)
                cor = Color.FromRgb(231, 76, 60); // Vermelho
            else if (temperatura > 100)
                cor = Color.FromRgb(243, 156, 18); // Laranja
            else
                cor = Color.FromRgb(52, 152, 219); // Azul

            foreach (var particula in particulas)
            {
                var colorAnim = new ColorAnimation(cor, TimeSpan.FromMilliseconds(800));
                var brush = particula.Fill as SolidColorBrush;
                if (brush != null)
                {
                    brush.BeginAnimation(SolidColorBrush.ColorProperty, colorAnim);
                }

                // Atualizar efeito
                if (particula.Effect is System.Windows.Media.Effects.DropShadowEffect shadow)
                {
                    shadow.Color = cor;
                }
            }
        }

        // ATUALIZAR MARCADOR NO GRÁFICO
        private void AtualizarMarcadorGrafico(double temperatura)
        {
            // Calcular posição X no gráfico (0K = 50, 300K = 680)
            double x = 50 + (temperatura / 300.0) * 630;

            // Calcular posição Y aproximada na curva
            double t = temperatura / 300.0;
            double y = 180 - (t * t * 160); // Aproximação da curva

            var animX = new DoubleAnimation(x - 6, TimeSpan.FromMilliseconds(600))
            {
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseOut }
            };
            var animY = new DoubleAnimation(y - 6, TimeSpan.FromMilliseconds(600))
            {
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseOut }
            };

            MarcadorTemp.BeginAnimation(Canvas.LeftProperty, animX);
            MarcadorTemp.BeginAnimation(Canvas.TopProperty, animY);

            // Atualizar linha vertical
            var animLinha = new DoubleAnimation(x, TimeSpan.FromMilliseconds(600))
            {
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseOut }
            };
            LinhaMarcador.BeginAnimation(Line.X1Property, animLinha);
            LinhaMarcador.BeginAnimation(Line.X2Property, animLinha);
        }

        // ATUALIZAR INTERPRETAÇÃO
        private void AtualizarInterpretacao(double temperatura, double entropia)
        {
            string texto = "";

            if (temperatura > 200)
            {
                texto = $"🔥 Temperatura alta ({temperatura:F0} K). As partículas possuem grande energia cinética e se movem de forma caótica. A entropia está em {entropia:F1}% do máximo.";
            }
            else if (temperatura > 100)
            {
                texto = $"🌡️ Temperatura moderada ({temperatura:F0} K). As partículas ainda possuem movimento significativo, mas menos caótico. A entropia é de {entropia:F1}%.";
            }
            else if (temperatura > 10)
            {
                texto = $"❄️ Temperatura baixa ({temperatura:F0} K). O movimento molecular está bastante reduzido. A entropia caiu para {entropia:F1}%.";
            }
            else if (temperatura > 1)
            {
                texto = $"🧊 Temperatura muito baixa ({temperatura:F0} K). As partículas estão quase imóveis. A entropia é de apenas {entropia:F1}%.";
            }
            else
            {
                texto = $"🌌 Próximo ao zero absoluto ({temperatura:F1} K)! O movimento molecular é quase inexistente. A entropia tende a zero ({entropia:F2}%).";
            }

            TxtInterpretacao.Text = texto;
        }

        // ATUALIZAR ANÁLISE DETALHADA
        private void AtualizarAnaliseDetalhada(double temperatura)
        {
            string analise = "";

            if (temperatura < 1)
            {
                analise = "🌌 REGIME QUÂNTICO EXTREMO\n\n" +
                         "Nesta faixa de temperatura, os efeitos quânticos dominam completamente. " +
                         "As partículas ocupam o estado fundamental e o movimento térmico é praticamente inexistente. " +
                         "A entropia se aproxima de zero, validando a 3ª Lei da Termodinâmica.\n\n" +
                         "Fenômenos observados:\n" +
                         "• Condensação de Bose-Einstein\n" +
                         "• Supercondutividade\n" +
                         "• Superfluidez";
            }
            else if (temperatura < 10)
            {
                analise = "🧊 REGIME CRIOGÊNICO\n\n" +
                         "O sistema está em temperaturas criogênicas. O movimento molecular é extremamente limitado. " +
                         "A entropia é muito baixa, e o sistema está altamente ordenado.\n\n" +
                         "Aplicações nesta faixa:\n" +
                         "• Computação quântica\n" +
                         "• Estudos de supercondutores\n" +
                         "• Resfriamento de detectores de radiação";
            }
            else if (temperatura < 77)
            {
                analise = "❄️ TEMPERATURAS ULTRA-BAIXAS\n\n" +
                         $"Em {temperatura:F0} K, o sistema apresenta propriedades interessantes. " +
                         "O movimento térmico é significativamente reduzido, mas ainda presente.\n\n" +
                         "Características:\n" +
                         "• Baixa agitação molecular\n" +
                         "• Entropia reduzida\n" +
                         "• Propriedades mecânicas alteradas";
            }
            else if (temperatura < 200)
            {
                analise = "🌡️ REGIME DE TEMPERATURA MODERADA\n\n" +
                         "O sistema está em uma faixa de temperatura intermediária. " +
                         "As partículas possuem energia cinética moderada e a entropia é intermediária.\n\n" +
                         "Comportamento:\n" +
                         "• Movimento molecular visível\n" +
                         "• Entropia moderada\n" +
                         "• Transições de fase possíveis";
            }
            else
            {
                analise = "🔥 ALTA TEMPERATURA\n\n" +
                         "A temperatura está alta. As partículas possuem muita energia cinética e se movem caoticamente. " +
                         "A entropia é elevada, indicando alto grau de desordem no sistema.\n\n" +
                         "Características:\n" +
                         "• Grande agitação molecular\n" +
                         "• Alta entropia\n" +
                         "• Sistema altamente desordenado";
            }

            TxtAnaliseDetalhada.Text = analise;
        }

        // ATUALIZAR DICA
        private void AtualizarDica(double temperatura)
        {
            string[] dicas;

            if (temperatura < 10)
            {
                dicas = new string[]
                {
                    "Você está muito próximo do zero absoluto! Efeitos quânticos dominam nesta região.",
                    "A 3ª Lei estabelece que S → 0 quando T → 0 K para cristais perfeitos.",
                    "Impossível alcançar 0 K absoluto em um número finito de processos!",
                    "Nesta temperatura, fenômenos como supercondutividade podem ocorrer."
                };
            }
            else if (temperatura < 100)
            {
                dicas = new string[]
                {
                    "Temperaturas criogênicas permitem estudar fenômenos quânticos macroscópicos.",
                    "Conforme T diminui, a entropia diminui e o sistema se torna mais ordenado.",
                    "O nitrogênio líquido ferve a 77 K, útil para resfriamento criogênico.",
                    "Supercondutores de alta temperatura funcionam nesta faixa."
                };
            }
            else
            {
                dicas = new string[]
                {
                    "Conforme a temperatura diminui, as partículas se movem mais lentamente!",
                    "A entropia é uma medida da desordem do sistema.",
                    "Quanto menor a temperatura, menor a entropia do sistema.",
                    "A 3ª Lei foi formulada por Walther Nernst em 1906."
                };
            }

            TxtDica.Text = dicas[random.Next(dicas.Length)];
        }

        // ANIMAÇÃO DAS PARTÍCULAS
        private void AnimationTimer_Tick(object? sender, EventArgs e)
        {
            double temperatura = SliderTemp.Value;
            double velocidadeMax = temperatura / 30.0; // Velocidade proporcional à temperatura

            foreach (var particula in particulas)
            {
                double currentX = Canvas.GetLeft(particula);
                double currentY = Canvas.GetTop(particula);

                // Movimento aleatório proporcional à temperatura
                double dx = (random.NextDouble() - 0.5) * velocidadeMax;
                double dy = (random.NextDouble() - 0.5) * velocidadeMax;

                double newX = Math.Clamp(currentX + dx, 10, 790);
                double newY = Math.Clamp(currentY + dy, 10, 390);

                Canvas.SetLeft(particula, newX);
                Canvas.SetTop(particula, newY);
            }
        }

        // BOTÃO ANIMAR
        private void BtnAnimar_Click(object sender, RoutedEventArgs e)
        {
            if (!isAnimating)
            {
                animationTimer.Start();
                isAnimating = true;
            }
        }

        // BOTÃO PAUSAR
        private void BtnPausar_Click(object sender, RoutedEventArgs e)
        {
            if (isAnimating)
            {
                animationTimer.Stop();
                isAnimating = false;
            }
        }

        // BOTÃO RESETAR
        private void BtnResetar_Click(object sender, RoutedEventArgs e)
        {
            animationTimer.Stop();
            isAnimating = false;
            SliderTemp.Value = 300;
            tempMinima = 300;
            TxtTempMinima.Text = "300 K";
        }

        // CENÁRIOS PREDEFINIDOS
        private void BtnCenario1_Click(object sender, RoutedEventArgs e)
        {
            // Temperatura Ambiente
            SliderTemp.Value = 300;
        }

        private void BtnCenario2_Click(object sender, RoutedEventArgs e)
        {
            // Nitrogênio Líquido
            SliderTemp.Value = 77;
        }

        private void BtnCenario3_Click(object sender, RoutedEventArgs e)
        {
            // Hélio Líquido
            SliderTemp.Value = 4;
        }

        private void BtnCenario4_Click(object sender, RoutedEventArgs e)
        {
            // Quase Zero
            SliderTemp.Value = 0.1;
        }

        private void BtnCenario5_Click(object sender, RoutedEventArgs e)
        {
            // Recorde Laboratorial
            SliderTemp.Value = 0.001;
        }
    }
}