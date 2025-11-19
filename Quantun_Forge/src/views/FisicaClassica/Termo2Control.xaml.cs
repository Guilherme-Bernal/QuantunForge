using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace Quantun_Forge.src.views.FisicaClassica
{
    public partial class Termo2Control : UserControl
    {
        private Random random = new Random();
        private int contadorSimulacoes = 0;
        private int contadorDesafios = 0;

        public Termo2Control()
        {
            InitializeComponent();
            AtualizarValoresDisplay();
        }

        // EVENTOS DOS SLIDERS
        private void SliderTq_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (TxtValorTq != null)
            {
                double tq = SliderTq.Value;
                double celsius = tq - 273;
                TxtValorTq.Text = $"Tq = {tq:F0} K ({celsius:F0}°C)";
                LblTq.Text = $"Tq = {tq:F0} K";
                AtualizarValoresDisplay();
            }
        }

        private void SliderTf_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (TxtValorTf != null)
            {
                double tf = SliderTf.Value;
                double celsius = tf - 273;
                TxtValorTf.Text = $"Tf = {tf:F0} K ({celsius:F0}°C)";
                LblTf.Text = $"Tf = {tf:F0} K";
                AtualizarValoresDisplay();
            }
        }

        private void SliderQh_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (TxtValorQh != null)
            {
                TxtValorQh.Text = $"Qh = {SliderQh.Value:F0} J";
                AtualizarValoresDisplay();
            }
        }

        // ATUALIZAR VALORES DISPLAY
        private void AtualizarValoresDisplay()
        {
            if (TxtValoresCalculados == null) return;

            double tq = SliderTq.Value;
            double tf = SliderTf.Value;
            double qh = SliderQh.Value;

            // Validar temperaturas
            if (tf >= tq)
            {
                return;
            }

            // Calcular eficiência de Carnot
            double eficiencia = 1 - (tf / tq);

            // Calcular trabalho e calor rejeitado
            double w = qh * eficiencia;
            double qc = qh - w;

            TxtValoresCalculados.Inlines.Clear();
            TxtValoresCalculados.Inlines.Add(new System.Windows.Documents.Run($"Qh = {qh:F0} J")
            {
                Foreground = new SolidColorBrush(Color.FromRgb(231, 76, 60)),
                FontWeight = FontWeights.Bold
            });
            TxtValoresCalculados.Inlines.Add(new System.Windows.Documents.LineBreak());
            TxtValoresCalculados.Inlines.Add(new System.Windows.Documents.Run($"W = {w:F0} J")
            {
                Foreground = new SolidColorBrush(Color.FromRgb(39, 174, 96)),
                FontWeight = FontWeights.Bold
            });
            TxtValoresCalculados.Inlines.Add(new System.Windows.Documents.LineBreak());
            TxtValoresCalculados.Inlines.Add(new System.Windows.Documents.Run($"Qc = {qc:F0} J")
            {
                Foreground = new SolidColorBrush(Color.FromRgb(52, 152, 219)),
                FontWeight = FontWeights.Bold
            });
        }

        // SIMULAR MÁQUINA
        private void BtnSimular_Click(object sender, RoutedEventArgs e)
        {
            double tq = SliderTq.Value;
            double tf = SliderTf.Value;
            double qh = SliderQh.Value;

            // Validar temperaturas
            if (tf >= tq)
            {
                MessageBox.Show("A temperatura da fonte fria (Tf) deve ser menor que a da fonte quente (Tq)!",
                    "Erro de Configuração", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Calcular eficiência de Carnot
            double eficiencia = 1 - (tf / tq);
            double eficienciaPercent = eficiencia * 100;

            // Calcular trabalho e calor rejeitado
            double w = qh * eficiencia;
            double qc = qh - w;

            // Calcular variação de entropia
            double deltaS_quente = -qh / tq;
            double deltaS_fria = qc / tf;
            double deltaS_total = deltaS_quente + deltaS_fria;

            // Atualizar displays
            TxtEficiencia.Text = $"η = {eficienciaPercent:F1}%";
            TxtEficienciaMotor.Text = $"η = {eficienciaPercent:F1}%";

            TxtQhValor.Text = $"Qh = {qh:F0} J";
            TxtWValor.Text = $"W = {w:F0} J";
            TxtQcValor.Text = $"Qc = {qc:F0} J";
            TxtEntropiaTotal.Text = $"ΔS_total = {deltaS_total:F3} J/K";

            // Interpretação
            string interpretacao = GerarInterpretacao(eficienciaPercent, tq, tf, qh, w, qc);
            TxtInterpretacao.Text = interpretacao;

            // Análise detalhada
            string analise = GerarAnaliseDetalhada(eficienciaPercent, tq, tf, qh, w, qc, deltaS_total);
            TxtAnaliseDetalhada.Text = analise;
            PainelAnalise.Visibility = Visibility.Visible;

            // Comparação
            AtualizarComparacao(eficienciaPercent);

            // Atualizar dica
            AtualizarDica(eficienciaPercent);

            // Animações
            AnimarBarras(qh, w, qc);
            AnimarSetas();

            // Incrementar contador
            contadorSimulacoes++;
            TxtContadorSimulacoes.Text = contadorSimulacoes.ToString();
        }

        // GERAR INTERPRETAÇÃO
        private string GerarInterpretacao(double eficiencia, double tq, double tf, double qh, double w, double qc)
        {
            string texto = "";

            texto += $"⚙️ A máquina de Carnot operando entre {tq:F0} K e {tf:F0} K tem eficiência máxima de {eficiencia:F1}%.\n\n";

            texto += $"📊 FLUXO DE ENERGIA:\n";
            texto += $"• Absorve {qh:F0} J da fonte quente\n";
            texto += $"• Converte {w:F0} J em trabalho útil\n";
            texto += $"• Rejeita {qc:F0} J para a fonte fria\n\n";

            if (eficiencia > 60)
                texto += "✅ Eficiência muito alta! Isso requer grande diferença de temperatura.";
            else if (eficiencia > 40)
                texto += "✅ Boa eficiência, típica de usinas termelétricas modernas.";
            else if (eficiencia > 25)
                texto += "⚖️ Eficiência moderada, similar a motores a combustão.";
            else
                texto += "⚠️ Eficiência baixa. Maior parte da energia é desperdiçada como calor.";

            return texto;
        }

        // ANÁLISE DETALHADA
        private string GerarAnaliseDetalhada(double eficiencia, double tq, double tf, double qh, double w, double qc, double deltaS)
        {
            string texto = "";

            texto += "🔬 ANÁLISE TERMODINÂMICA COMPLETA\n\n";

            texto += "📐 CÁLCULOS:\n";
            texto += $"η = 1 - (Tf/Tq) = 1 - ({tf:F0}/{tq:F0}) = {eficiencia:F3}%\n";
            texto += $"W = Qh × η = {qh:F0} × {eficiencia / 100:F3} = {w:F0} J\n";
            texto += $"Qc = Qh - W = {qh:F0} - {w:F0} = {qc:F0} J\n\n";

            texto += "📈 ANÁLISE DE ENTROPIA:\n";
            texto += $"ΔS_quente = -Qh/Tq = -{qh:F0}/{tq:F0} = {-qh / tq:F3} J/K\n";
            texto += $"ΔS_fria = Qc/Tf = {qc:F0}/{tf:F0} = {qc / tf:F3} J/K\n";
            texto += $"ΔS_total = {deltaS:F3} J/K";

            if (Math.Abs(deltaS) < 0.001)
                texto += " ≈ 0 (processo reversível ideal)\n\n";
            else
                texto += " > 0 (processo real, irreversível)\n\n";

            texto += "⚡ BALANÇO ENERGÉTICO:\n";
            double percentW = (w / qh) * 100;
            double percentQc = (qc / qh) * 100;
            texto += $"• {percentW:F1}% convertido em trabalho\n";
            texto += $"• {percentQc:F1}% rejeitado como calor residual\n\n";

            texto += "🎯 SIGNIFICADO FÍSICO:\n";
            if (eficiencia > 50)
                texto += "A grande diferença de temperatura permite alta conversão de calor em trabalho. ";
            else
                texto += "A pequena diferença de temperatura limita a conversão de energia. ";

            texto += "A 2ª Lei garante que sempre há perda de energia útil, tornando impossível η = 100%.";

            return texto;
        }

        // ATUALIZAR COMPARAÇÃO
        private void AtualizarComparacao(double eficiencia)
        {
            if (eficiencia > 45)
                TxtComparacao.Text = "🏆 Eficiência excelente! Acima de todas as máquinas reais!";
            else if (eficiencia > 35)
                TxtComparacao.Text = "✅ Ótima eficiência! Comparável às melhores usinas.";
            else if (eficiencia > 25)
                TxtComparacao.Text = "⚖️ Eficiência boa, similar a motores modernos.";
            else
                TxtComparacao.Text = "⚠️ Eficiência baixa. Melhorias são necessárias.";

            TxtComparacao.Foreground = eficiencia > 35
                ? new SolidColorBrush(Color.FromRgb(39, 174, 96))
                : eficiencia > 25
                ? new SolidColorBrush(Color.FromRgb(243, 156, 18))
                : new SolidColorBrush(Color.FromRgb(231, 76, 60));
        }

        // ATUALIZAR DICA
        private void AtualizarDica(double eficiencia)
        {
            string[] dicas = new string[]
            {
                "Quanto maior a diferença entre Tq e Tf, maior a eficiência!",
                "Nenhuma máquina real alcança a eficiência de Carnot devido a atritos e perdas.",
                "A entropia do universo sempre aumenta em processos reais!",
                "Refrigeradores são máquinas térmicas operando no ciclo reverso!",
                "O limite de Carnot é fundamental para o design de motores eficientes!",
                "Temperatura em Kelvin é essencial para os cálculos corretos!",
                "Mesmo a máquina de Carnot rejeita parte da energia como calor!",
                "A 2ª Lei explica por que não podemos ter movimento perpétuo!"
            };

            TxtDica.Text = dicas[random.Next(dicas.Length)];
        }

        // ANIMAÇÕES
        private void AnimarBarras(double qh, double w, double qc)
        {
            double larguraMax = 180;

            // Normalizar valores
            double maxValor = Math.Max(qh, Math.Max(w, qc));

            double larguraQh = (qh / maxValor) * larguraMax;
            double larguraW = (w / maxValor) * larguraMax;
            double larguraQc = (qc / maxValor) * larguraMax;

            var animQh = new DoubleAnimation(0, larguraQh, TimeSpan.FromMilliseconds(1000))
            {
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseOut }
            };
            BarraQh.BeginAnimation(FrameworkElement.WidthProperty, animQh);

            var animW = new DoubleAnimation(0, larguraW, TimeSpan.FromMilliseconds(1000))
            {
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseOut }
            };
            BarraW.BeginAnimation(FrameworkElement.WidthProperty, animW);

            var animQc = new DoubleAnimation(0, larguraQc, TimeSpan.FromMilliseconds(1000))
            {
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseOut }
            };
            BarraQc.BeginAnimation(FrameworkElement.WidthProperty, animQc);
        }

        private void AnimarSetas()
        {
            // Animar setas com efeito pulsante
            var pulseAnimation = new DoubleAnimation
            {
                From = 1.0,
                To = 1.2,
                Duration = TimeSpan.FromMilliseconds(800),
                AutoReverse = true,
                RepeatBehavior = new RepeatBehavior(3)
            };

            var scaleTransformQh = new ScaleTransform(1, 1, 0, 35);
            SetaQh.RenderTransform = scaleTransformQh;
            scaleTransformQh.BeginAnimation(ScaleTransform.ScaleXProperty, pulseAnimation);
            scaleTransformQh.BeginAnimation(ScaleTransform.ScaleYProperty, pulseAnimation);

            var scaleTransformW = new ScaleTransform(1, 1, 55, 0);
            SetaW.RenderTransform = scaleTransformW;
            scaleTransformW.BeginAnimation(ScaleTransform.ScaleXProperty, pulseAnimation);
            scaleTransformW.BeginAnimation(ScaleTransform.ScaleYProperty, pulseAnimation);

            var scaleTransformQc = new ScaleTransform(1, 1, 0, 35);
            SetaQc.RenderTransform = scaleTransformQc;
            scaleTransformQc.BeginAnimation(ScaleTransform.ScaleXProperty, pulseAnimation);
            scaleTransformQc.BeginAnimation(ScaleTransform.ScaleYProperty, pulseAnimation);
        }

        // RESETAR SISTEMA
        private void BtnResetar_Click(object sender, RoutedEventArgs e)
        {
            SliderTq.Value = 500;
            SliderTf.Value = 300;
            SliderQh.Value = 1000;

            TxtEficiencia.Text = "η = 40.0%";
            TxtInterpretacao.Text = "Configure as temperaturas e clique em 'Simular Máquina' para ver a análise.";
            PainelAnalise.Visibility = Visibility.Collapsed;
            PainelDesafio.Visibility = Visibility.Collapsed;

            // Resetar barras
            BarraQh.BeginAnimation(FrameworkElement.WidthProperty, new DoubleAnimation(0, TimeSpan.FromMilliseconds(300)));
            BarraW.BeginAnimation(FrameworkElement.WidthProperty, new DoubleAnimation(0, TimeSpan.FromMilliseconds(300)));
            BarraQc.BeginAnimation(FrameworkElement.WidthProperty, new DoubleAnimation(0, TimeSpan.FromMilliseconds(300)));
        }

        // CENÁRIOS PREDEFINIDOS
        private void BtnCenario1_Click(object sender, RoutedEventArgs e)
        {
            // Motor a Combustão (temperatura moderada)
            SliderTq.Value = 600;  // ~327°C
            SliderTf.Value = 350;  // ~77°C
            SliderQh.Value = 2000;
            BtnSimular_Click(sender, e);
        }

        private void BtnCenario2_Click(object sender, RoutedEventArgs e)
        {
            // Usina Termelétrica (alta temperatura)
            SliderTq.Value = 900;  // ~627°C
            SliderTf.Value = 300;  // ~27°C
            SliderQh.Value = 5000;
            BtnSimular_Click(sender, e);
        }

        private void BtnCenario3_Click(object sender, RoutedEventArgs e)
        {
            // Refrigerador Doméstico (temperaturas próximas)
            SliderTq.Value = 310;  // ~37°C (condensador)
            SliderTf.Value = 250;  // ~-23°C (evaporador)
            SliderQh.Value = 800;
            BtnSimular_Click(sender, e);
        }

        private void BtnCenario4_Click(object sender, RoutedEventArgs e)
        {
            // Motor de Alta Temperatura (muito quente)
            SliderTq.Value = 1200; // ~927°C
            SliderTf.Value = 300;  // ~27°C
            SliderQh.Value = 3000;
            BtnSimular_Click(sender, e);
        }

        private void BtnCenario5_Click(object sender, RoutedEventArgs e)
        {
            // Máquina Ideal (grande diferença)
            SliderTq.Value = 1000; // ~727°C
            SliderTf.Value = 200;  // ~-73°C
            SliderQh.Value = 2500;
            BtnSimular_Click(sender, e);
        }

        // MODO DESAFIO
        private void BtnDesafio_Click(object sender, RoutedEventArgs e)
        {
            // Gerar cenário aleatório
            double tqDesafio = random.Next(500, 1201);
            double tfDesafio = random.Next(250, (int)tqDesafio - 100);
            double qhDesafio = random.Next(1000, 5001);

            double eficienciaCorreta = (1 - (tfDesafio / tqDesafio)) * 100;
            double wCorreto = qhDesafio * (eficienciaCorreta / 100);
            double qcCorreto = qhDesafio - wCorreto;

            string tipoMaquina = "";
            if (eficienciaCorreta > 60)
                tipoMaquina = "uma máquina de altíssima eficiência";
            else if (eficienciaCorreta > 40)
                tipoMaquina = "uma usina termelétrica";
            else if (eficienciaCorreta > 25)
                tipoMaquina = "um motor a combustão";
            else
                tipoMaquina = "uma máquina de baixa eficiência";

            TxtEnunciadoDesafio.Text = $"🎯 DESAFIO:\n\n" +
                $"Você está projetando {tipoMaquina}.\n\n" +
                $"Dados:\n" +
                $"• Fonte Quente: Tq = {tqDesafio:F0} K ({tqDesafio - 273:F0}°C)\n" +
                $"• Fonte Fria: Tf = {tfDesafio:F0} K ({tfDesafio - 273:F0}°C)\n" +
                $"• Calor Absorvido: Qh = {qhDesafio:F0} J\n\n" +
                $"Calcule:\n" +
                $"1. A eficiência máxima (η)\n" +
                $"2. O trabalho realizado (W)\n" +
                $"3. O calor rejeitado (Qc)\n\n" +
                $"💡 Use a fórmula: η = 1 - (Tf/Tq)\n\n" +
                $"Configure os valores nos sliders e simule para verificar!";

            PainelDesafio.Visibility = Visibility.Visible;
            TxtRespostaDesafio.Visibility = Visibility.Collapsed;

            // Configurar sliders
            SliderTq.Value = tqDesafio;
            SliderTf.Value = tfDesafio;
            SliderQh.Value = qhDesafio;

            // Preparar resposta
            TxtRespostaDesafio.Text = $"✅ RESPOSTA CORRETA!\n\n" +
                $"η = 1 - (Tf/Tq) = 1 - ({tfDesafio:F0}/{tqDesafio:F0})\n" +
                $"η = {eficienciaCorreta:F2}%\n\n" +
                $"W = Qh × η = {qhDesafio:F0} × {eficienciaCorreta / 100:F3}\n" +
                $"W = {wCorreto:F0} J\n\n" +
                $"Qc = Qh - W = {qhDesafio:F0} - {wCorreto:F0}\n" +
                $"Qc = {qcCorreto:F0} J\n\n" +
                $"Clique em 'Simular Máquina' para ver a análise completa!";

            // Mostrar resposta após 3 segundos
            var timer = new System.Windows.Threading.DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(3);
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