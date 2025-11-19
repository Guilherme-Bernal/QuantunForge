using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace Quantun_Forge.src.views.FisicaClassica
{
    public partial class Termo1Control : UserControl
    {
        private Random random = new Random();
        private int contadorSimulacoes = 0;
        private int contadorDesafios = 0;
        private double desafioQCorreto = 0;
        private double desafioWCorreto = 0;
        private double desafioUCorreto = 0;

        public Termo1Control()
        {
            InitializeComponent();
            AtualizarValoresAtuais();
        }

        // EVENTOS DOS SLIDERS
        private void SliderQ_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (TxtValorQ != null)
            {
                double q = SliderQ.Value;
                TxtValorQ.Text = $"Q = {(q >= 0 ? "+" : "")}{q:F0} J";
                AtualizarValoresAtuais();
            }
        }

        private void SliderW_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (TxtValorW != null)
            {
                double w = SliderW.Value;
                TxtValorW.Text = $"W = {(w >= 0 ? "+" : "")}{w:F0} J";
                AtualizarValoresAtuais();
            }
        }

        // ATUALIZAR VALORES DISPLAY
        private void AtualizarValoresAtuais()
        {
            if (TxtValoresAtuais != null)
            {
                double q = SliderQ.Value;
                double w = SliderW.Value;
                double deltaU = q - w;

                TxtValoresAtuais.Inlines.Clear();
                TxtValoresAtuais.Inlines.Add(new System.Windows.Documents.Run($"Q = {(q >= 0 ? "+" : "")}{q:F0} J")
                {
                    Foreground = new SolidColorBrush(Color.FromRgb(231, 76, 60)),
                    FontWeight = FontWeights.Bold
                });
                TxtValoresAtuais.Inlines.Add(new System.Windows.Documents.LineBreak());
                TxtValoresAtuais.Inlines.Add(new System.Windows.Documents.Run($"W = {(w >= 0 ? "+" : "")}{w:F0} J")
                {
                    Foreground = new SolidColorBrush(Color.FromRgb(39, 174, 96)),
                    FontWeight = FontWeights.Bold
                });
                TxtValoresAtuais.Inlines.Add(new System.Windows.Documents.LineBreak());
                TxtValoresAtuais.Inlines.Add(new System.Windows.Documents.Run($"ΔU = {(deltaU >= 0 ? "+" : "")}{deltaU:F0} J")
                {
                    Foreground = new SolidColorBrush(Color.FromRgb(52, 152, 219)),
                    FontWeight = FontWeights.Bold
                });
            }
        }

        // SIMULAR PROCESSO
        private void BtnCalcular_Click(object sender, RoutedEventArgs e)
        {
            double q = SliderQ.Value;
            double w = SliderW.Value;
            double deltaU = q - w;

            // Atualizar resultado
            TxtResultado.Text = $"ΔU = {(deltaU >= 0 ? "+" : "")}{deltaU:F0} J";

            // Interpretação física
            string interpretacao = GerarInterpretacao(q, w, deltaU);
            TxtInterpretacao.Text = interpretacao;

            // Análise detalhada
            string analiseDetalhada = GerarAnaliseDetalhada(q, w, deltaU);
            TxtAnaliseDetalhada.Text = analiseDetalhada;
            PainelAnalise.Visibility = Visibility.Visible;

            // Atualizar dica
            AtualizarDica();

            // Atualizar estado do sistema
            AtualizarEstadoSistema(q, w, deltaU);

            // Animações
            AnimarSistema(q, w, deltaU);
            AnimarBarras(q, w, deltaU);

            // Incrementar contador
            contadorSimulacoes++;
            TxtContadorSimulacoes.Text = contadorSimulacoes.ToString();
        }

        // GERAR INTERPRETAÇÃO
        private string GerarInterpretacao(double q, double w, double deltaU)
        {
            string texto = "";

            // Análise do calor
            if (q > 0)
                texto += "🔥 O sistema ABSORVE calor do ambiente. ";
            else if (q < 0)
                texto += "❄️ O sistema CEDE calor ao ambiente. ";
            else
                texto += "🔒 Não há troca de calor (processo adiabático). ";

            // Análise do trabalho
            if (w > 0)
                texto += "O sistema REALIZA trabalho (expansão do gás). ";
            else if (w < 0)
                texto += "Trabalho é REALIZADO SOBRE o sistema (compressão do gás). ";
            else
                texto += "Não há trabalho realizado (processo isovolumétrico). ";

            // Análise da energia interna
            texto += "\n\n";
            if (deltaU > 0)
                texto += $"✅ A energia interna AUMENTA em {Math.Abs(deltaU):F0} J. As moléculas ficam mais agitadas e a temperatura tende a subir!";
            else if (deltaU < 0)
                texto += $"❄️ A energia interna DIMINUI em {Math.Abs(deltaU):F0} J. As moléculas ficam menos agitadas e a temperatura tende a cair!";
            else
                texto += "⚖️ A energia interna NÃO VARIA (processo isotérmico). A temperatura permanece constante!";

            return texto;
        }

        // ANÁLISE DETALHADA
        private string GerarAnaliseDetalhada(double q, double w, double deltaU)
        {
            string texto = "";

            // Identificar tipo de processo
            if (Math.Abs(deltaU) < 10)
            {
                texto += "🌡️ PROCESSO ISOTÉRMICO (T = constante)\n";
                texto += "• ΔU ≈ 0, portanto Q ≈ W\n";
                texto += "• A temperatura permanece constante\n";
                texto += "• Aplicação: Ciclo de Carnot, expansão lenta de gases\n";
            }
            else if (Math.Abs(q) < 10)
            {
                texto += "🔒 PROCESSO ADIABÁTICO (Q = 0)\n";
                texto += "• Sem troca de calor com o ambiente\n";
                texto += "• ΔU = -W (toda variação de energia vem do trabalho)\n";
                texto += "• Aplicação: Compressão rápida em motores, expansão em turbinas\n";
            }
            else if (Math.Abs(w) < 10)
            {
                texto += "📊 PROCESSO ISOVOLUMÉTRICO (V = constante)\n";
                texto += "• Volume constante, W ≈ 0\n";
                texto += "• ΔU = Q (todo calor modifica a energia interna)\n";
                texto += "• Aplicação: Aquecimento de gases em recipientes fechados\n";
            }
            else
            {
                texto += "⚙️ PROCESSO ISOBÁRICO (P = constante)\n";
                texto += "• Pressão constante\n";
                texto += "• Q = ΔU + W (equação completa da 1ª Lei)\n";
                texto += "• Aplicação: Aquecimento de gases em cilindros com pistão livre\n";
            }

            // Eficiência energética
            texto += "\n📊 ANÁLISE ENERGÉTICA:\n";
            if (q > 0 && w > 0)
            {
                double eficiencia = (w / q) * 100;
                texto += $"• Eficiência: {eficiencia:F1}% do calor foi convertido em trabalho\n";
                texto += $"• {100 - eficiencia:F1}% do calor aumentou a energia interna";
            }
            else if (q < 0 && w < 0)
            {
                texto += "• Sistema está sendo comprimido e resfriado simultaneamente\n";
                texto += "• Processo típico de refrigeração";
            }
            else if (q > 0 && w < 0)
            {
                texto += "• Sistema absorve calor mas é comprimido\n";
                texto += "• Energia interna aumenta significativamente";
            }
            else if (q < 0 && w > 0)
            {
                texto += "• Sistema cede calor e realiza trabalho\n";
                texto += "• Energia interna diminui significativamente";
            }

            return texto;
        }

        // ATUALIZAR ESTADO DO SISTEMA
        private void AtualizarEstadoSistema(double q, double w, double deltaU)
        {
            string estado = "";

            if (Math.Abs(deltaU) < 10)
                estado = "🌡️ Processo Isotérmico";
            else if (Math.Abs(q) < 10)
                estado = "🔒 Processo Adiabático";
            else if (Math.Abs(w) < 10)
                estado = "📊 Processo Isovolumétrico";
            else if (q > 500 && w > 0)
                estado = "🔥 Aquecimento + Expansão";
            else if (q < -500 && w < 0)
                estado = "❄️ Resfriamento + Compressão";
            else if (deltaU > 0)
                estado = "⬆️ Energia Aumentando";
            else if (deltaU < 0)
                estado = "⬇️ Energia Diminuindo";
            else
                estado = "⚡ Sistema em Equilíbrio";

            TxtEstadoSistema.Text = estado;
        }

        // ATUALIZAR DICA
        private void AtualizarDica()
        {
            string[] dicas = new string[]
            {
                "A energia interna de um gás ideal depende apenas de sua temperatura!",
                "Em processos cíclicos, a variação total de energia interna é sempre zero (ΔU = 0)!",
                "A 1ª Lei é uma aplicação do princípio de conservação de energia!",
                "O trabalho realizado por um gás em expansão é sempre positivo (W > 0)!",
                "Em um processo adiabático, toda variação de energia vem do trabalho realizado!",
                "Máquinas térmicas reais sempre têm eficiência menor que 100%!",
                "A convenção de sinais é importante: Q > 0 = absorve, W > 0 = expansão!",
                "Em um processo isotérmico, Q = W, e a temperatura não varia!",
                "Processos isovolumétricos não realizam trabalho pois o volume não muda!"
            };

            TxtDica.Text = dicas[random.Next(dicas.Length)];
        }

        // ANIMAÇÕES DO SISTEMA
        private void AnimarSistema(double q, double w, double deltaU)
        {
            // Animar chama (calor)
            if (q > 100)
            {
                var fadeIn = new DoubleAnimation(0, 0.9, TimeSpan.FromMilliseconds(600));
                Chama.BeginAnimation(UIElement.OpacityProperty, fadeIn);
            }
            else
            {
                var fadeOut = new DoubleAnimation(0, TimeSpan.FromMilliseconds(400));
                Chama.BeginAnimation(UIElement.OpacityProperty, fadeOut);
            }

            // Animar êmbolo (baseado no trabalho)
            double posicaoInicial = 60;
            double deslocamento = Math.Max(-30, Math.Min(30, w / 30.0));
            double novaPosicao = posicaoInicial - deslocamento;

            var moveEmbolo = new DoubleAnimation(
                Canvas.GetTop(Embolo),
                novaPosicao,
                TimeSpan.FromMilliseconds(1000))
            {
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseInOut }
            };
            Embolo.BeginAnimation(Canvas.TopProperty, moveEmbolo);

            // Animar partículas (velocidade baseada em ΔU)
            AnimarParticulas(deltaU);

            // Animar seta de trabalho
            if (Math.Abs(w) > 100)
            {
                var fadeIn = new DoubleAnimation(0, 0.95, TimeSpan.FromMilliseconds(600));
                SetaTrabalho.BeginAnimation(UIElement.OpacityProperty, fadeIn);

                // Inverter direção se compressão
                if (w < 0)
                {
                    SetaTrabalho.RenderTransform = new ScaleTransform(-1, 1, 25, 15);
                }
                else
                {
                    SetaTrabalho.RenderTransform = new ScaleTransform(1, 1);
                }
            }
            else
            {
                var fadeOut = new DoubleAnimation(0, TimeSpan.FromMilliseconds(400));
                SetaTrabalho.BeginAnimation(UIElement.OpacityProperty, fadeOut);
            }
        }

        // ANIMAR PARTÍCULAS
        private void AnimarParticulas(double deltaU)
        {
            var particulas = new[] { Particula1, Particula2, Particula3, Particula4, Particula5, Particula6 };

            foreach (var particula in particulas)
            {
                // Velocidade baseada em energia
                double amplitude = Math.Max(2, Math.Min(12, Math.Abs(deltaU) / 40));
                double duracao = Math.Max(300, 1200 - Math.Abs(deltaU));

                double posX = Canvas.GetLeft(particula);
                double posY = Canvas.GetTop(particula);

                var animX = new DoubleAnimation
                {
                    From = posX,
                    To = posX + random.Next(-4, 5) * amplitude / 2,
                    Duration = TimeSpan.FromMilliseconds(duracao),
                    AutoReverse = true,
                    RepeatBehavior = new RepeatBehavior(3)
                };

                var animY = new DoubleAnimation
                {
                    From = posY,
                    To = posY + random.Next(-4, 5) * amplitude / 2,
                    Duration = TimeSpan.FromMilliseconds(duracao + 100),
                    AutoReverse = true,
                    RepeatBehavior = new RepeatBehavior(3)
                };

                particula.BeginAnimation(Canvas.LeftProperty, animX);
                particula.BeginAnimation(Canvas.TopProperty, animY);

                // Cor baseada em temperatura
                Color cor = deltaU > 0
                    ? Color.FromRgb(231, (byte)Math.Max(76, 152 - Math.Min(deltaU / 5, 76)), 60)
                    : Color.FromRgb(52, 152, 219);

                var colorAnim = new ColorAnimation(cor, TimeSpan.FromMilliseconds(800));
                var brush = new SolidColorBrush();
                particula.Fill = brush;
                brush.BeginAnimation(SolidColorBrush.ColorProperty, colorAnim);
            }
        }

        // ANIMAR BARRAS
        private void AnimarBarras(double q, double w, double deltaU)
        {
            double larguraMax = 150;

            // Barra Q
            double larguraQ = Math.Min(larguraMax, Math.Abs(q) / 1000.0 * larguraMax);
            var animQ = new DoubleAnimation(0, larguraQ, TimeSpan.FromMilliseconds(800))
            {
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseOut }
            };
            BarraQ.BeginAnimation(FrameworkElement.WidthProperty, animQ);

            // Barra W
            double larguraW = Math.Min(larguraMax, Math.Abs(w) / 1000.0 * larguraMax);
            var animW = new DoubleAnimation(0, larguraW, TimeSpan.FromMilliseconds(800))
            {
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseOut }
            };
            BarraW.BeginAnimation(FrameworkElement.WidthProperty, animW);

            // Barra ΔU
            double larguraU = Math.Min(larguraMax, Math.Abs(deltaU) / 1000.0 * larguraMax);
            var animU = new DoubleAnimation(0, larguraU, TimeSpan.FromMilliseconds(800))
            {
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseOut }
            };
            BarraU.BeginAnimation(FrameworkElement.WidthProperty, animU);
        }

        // RESETAR SISTEMA
        private void BtnResetar_Click(object sender, RoutedEventArgs e)
        {
            SliderQ.Value = 500;
            SliderW.Value = 200;
            TxtResultado.Text = "ΔU = +300 J";
            TxtInterpretacao.Text = "Configure os parâmetros e clique em 'Simular Processo' para ver a análise física do sistema.";
            PainelAnalise.Visibility = Visibility.Collapsed;
            PainelDesafio.Visibility = Visibility.Collapsed;
            TxtEstadoSistema.Text = "⚡ Sistema em equilíbrio";

            // Resetar animações
            Chama.BeginAnimation(UIElement.OpacityProperty, new DoubleAnimation(0, TimeSpan.FromMilliseconds(300)));
            SetaTrabalho.BeginAnimation(UIElement.OpacityProperty, new DoubleAnimation(0, TimeSpan.FromMilliseconds(300)));
            Embolo.BeginAnimation(Canvas.TopProperty, new DoubleAnimation(60, TimeSpan.FromMilliseconds(500)));

            BarraQ.BeginAnimation(FrameworkElement.WidthProperty, new DoubleAnimation(0, TimeSpan.FromMilliseconds(300)));
            BarraW.BeginAnimation(FrameworkElement.WidthProperty, new DoubleAnimation(0, TimeSpan.FromMilliseconds(300)));
            BarraU.BeginAnimation(FrameworkElement.WidthProperty, new DoubleAnimation(0, TimeSpan.FromMilliseconds(300)));
        }

        // CENÁRIOS PREDEFINIDOS
        private void BtnCenario1_Click(object sender, RoutedEventArgs e)
        {
            // Aquecimento + Expansão
            SliderQ.Value = 800;
            SliderW.Value = 500;
            BtnCalcular_Click(sender, e);
        }

        private void BtnCenario2_Click(object sender, RoutedEventArgs e)
        {
            // Resfriamento + Compressão
            SliderQ.Value = -600;
            SliderW.Value = -400;
            BtnCalcular_Click(sender, e);
        }

        private void BtnCenario3_Click(object sender, RoutedEventArgs e)
        {
            // Processo Isotérmico (ΔU ≈ 0, Q = W)
            SliderQ.Value = 600;
            SliderW.Value = 600;
            BtnCalcular_Click(sender, e);
        }

        private void BtnCenario4_Click(object sender, RoutedEventArgs e)
        {
            // Processo Adiabático (Q = 0)
            SliderQ.Value = 0;
            SliderW.Value = 500;
            BtnCalcular_Click(sender, e);
        }

        private void BtnCenario5_Click(object sender, RoutedEventArgs e)
        {
            // Processo Isovolumétrico (W = 0)
            SliderQ.Value = 700;
            SliderW.Value = 0;
            BtnCalcular_Click(sender, e);
        }

        // MODO DESAFIO
        private void BtnDesafio_Click(object sender, RoutedEventArgs e)
        {
            // Gerar valores aleatórios
            desafioQCorreto = random.Next(-800, 901);
            desafioWCorreto = random.Next(-800, 901);
            desafioUCorreto = desafioQCorreto - desafioWCorreto;

            // Criar enunciado
            string tipoProcesso = "";
            string dica = "";

            if (Math.Abs(desafioUCorreto) < 50)
            {
                tipoProcesso = "isotérmico (temperatura constante)";
                dica = "Lembre-se: ΔU ≈ 0, então Q ≈ W";
            }
            else if (Math.Abs(desafioQCorreto) < 50)
            {
                tipoProcesso = "adiabático (sem troca de calor)";
                dica = "Lembre-se: Q = 0, então ΔU = -W";
            }
            else if (Math.Abs(desafioWCorreto) < 50)
            {
                tipoProcesso = "isovolumétrico (volume constante)";
                dica = "Lembre-se: W = 0, então ΔU = Q";
            }
            else
            {
                tipoProcesso = "isobárico (pressão constante)";
                dica = "Use a equação completa: ΔU = Q - W";
            }

            TxtEnunciadoDesafio.Text = $"🎯 DESAFIO:\n\n" +
                $"Um gás sofre um processo {tipoProcesso}.\n\n" +
                $"Dados:\n" +
                $"• Q = {(desafioQCorreto >= 0 ? "+" : "")}{desafioQCorreto:F0} J\n" +
                $"• W = {(desafioWCorreto >= 0 ? "+" : "")}{desafioWCorreto:F0} J\n\n" +
                $"Calcule a variação de energia interna (ΔU).\n\n" +
                $"💡 {dica}\n\n" +
                $"Configure os sliders com os valores dados e clique em 'Simular Processo' para verificar!";

            PainelDesafio.Visibility = Visibility.Visible;
            TxtRespostaDesafio.Visibility = Visibility.Collapsed;

            // Configurar sliders com valores do desafio
            SliderQ.Value = desafioQCorreto;
            SliderW.Value = desafioWCorreto;

            // Preparar resposta
            TxtRespostaDesafio.Text = $"✅ RESPOSTA CORRETA!\n\n" +
                $"ΔU = Q - W\n" +
                $"ΔU = {(desafioQCorreto >= 0 ? "+" : "")}{desafioQCorreto:F0} - ({(desafioWCorreto >= 0 ? "+" : "")}{desafioWCorreto:F0})\n" +
                $"ΔU = {(desafioUCorreto >= 0 ? "+" : "")}{desafioUCorreto:F0} J\n\n" +
                $"Agora clique em 'Simular Processo' para ver a análise completa!";

            // Mostrar resposta após 2 segundos
            var timer = new System.Windows.Threading.DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(2);
            timer.Tick += (s, args) =>
            {
                TxtRespostaDesafio.Visibility = Visibility.Visible;
                timer.Stop();
            };
            timer.Start();

            // Incrementar contador
            contadorDesafios++;
            TxtContadorDesafios.Text = contadorDesafios.ToString();
        }
    }
}