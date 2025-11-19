// LeiNewton3Control.xaml.cs
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;

namespace Quantun_Forge.src.views.FisicaClassica
{
    public partial class LeiNewton3Control : UserControl
    {
        // Timer de animação
        private DispatcherTimer timer = null!;

        // Variáveis da física
        private double massaA = 5;              // kg
        private double massaB = 5;              // kg
        private double forcaColisao = 50;       // N
        private double velocidadeA = 0;         // m/s
        private double velocidadeB = 0;         // m/s
        private double aceleracaoA = 0;         // m/s²
        private double aceleracaoB = 0;         // m/s²
        private bool simulacaoAtiva = false;
        private bool colidiu = false;

        // Constantes
        private const double DT = 0.05;         // 20 FPS
        private const double ESCALA_PIXELS = 2;
        private const double VELOCIDADE_INICIAL = 5; // m/s

        public LeiNewton3Control()
        {
            InitializeComponent();
            InicializarTimer();
            InicializarEventos();
            AtualizarLabels();
        }

        private void InicializarTimer()
        {
            timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(DT)
            };
            timer.Tick += Timer_Tick;
        }

        private void InicializarEventos()
        {
            // Atualiza labels dos sliders
            SliderMassaA.ValueChanged += (s, e) =>
            {
                TxtMassaASlider.Text = $"{e.NewValue:F0} kg";
                AjustarTamanhoObjeto(ShapeA, e.NewValue);
                MassLabelA.Text = $"{e.NewValue:F0} kg";
            };

            SliderMassaB.ValueChanged += (s, e) =>
            {
                TxtMassaBSlider.Text = $"{e.NewValue:F0} kg";
                AjustarTamanhoObjeto(ShapeB, e.NewValue);
                MassLabelB.Text = $"{e.NewValue:F0} kg";
            };

            SliderForca.ValueChanged += (s, e) =>
            {
                TxtForcaSlider.Text = $"{e.NewValue:F0} N";
            };
        }

        private void AjustarTamanhoObjeto(System.Windows.Shapes.Rectangle shape, double massa)
        {
            // Tamanho proporcional à massa: 40px (min) a 100px (max)
            double tamanho = 40 + (massa / 50.0) * 60;
            shape.Width = tamanho;
            shape.Height = tamanho;
        }

        private void Sliders_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (!IsLoaded) return;
            AtualizarLabels();
        }

        private void AtualizarLabels()
        {
            massaA = SliderMassaA.Value;
            massaB = SliderMassaB.Value;
            forcaColisao = SliderForca.Value;

            TxtMassaA.Text = $"{massaA:F0} kg";
            TxtMassaB.Text = $"{massaB:F0} kg";
        }

        private void SimularColisao_Click(object sender, RoutedEventArgs e)
        {
            if (simulacaoAtiva)
            {
                MessageBox.Show("Simulação já está ativa! Clique em Resetar primeiro.", "Aviso", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            // Pega valores atuais
            massaA = SliderMassaA.Value;
            massaB = SliderMassaB.Value;
            forcaColisao = SliderForca.Value;

            // Velocidades iniciais (objetos se movendo um em direção ao outro)
            velocidadeA = VELOCIDADE_INICIAL;
            velocidadeB = -VELOCIDADE_INICIAL;

            // Posições iniciais
            Canvas.SetLeft(ObjectA, 100);
            Canvas.SetLeft(ObjectB, SimulationCanvas.ActualWidth - 160);

            // Reset variáveis
            colidiu = false;
            aceleracaoA = 0;
            aceleracaoB = 0;

            // Esconde vetores inicialmente
            EsconderVetores();

            // Inicia simulação
            simulacaoAtiva = true;
            timer.Start();

            AnimarElemento(ObjectA);
            AnimarElemento(ObjectB);
        }

        private void Timer_Tick(object? sender, EventArgs e)
        {
            if (!simulacaoAtiva) return;

            double posA = Canvas.GetLeft(ObjectA);
            double posB = Canvas.GetLeft(ObjectB);

            // Verifica colisão (quando objetos se encontram)
            double centroA = posA + ShapeA.Width / 2;
            double centroB = posB + ShapeB.Width / 2;
            double distancia = Math.Abs(centroB - centroA);

            if (!colidiu && distancia < (ShapeA.Width + ShapeB.Width) / 2 + 10)
            {
                // COLISÃO DETECTADA!
                colidiu = true;
                ExecutarColisao();
            }

            if (!colidiu)
            {
                // Movimento antes da colisão
                Canvas.SetLeft(ObjectA, posA + velocidadeA * ESCALA_PIXELS);
                Canvas.SetLeft(ObjectB, posB + velocidadeB * ESCALA_PIXELS);
            }
            else
            {
                // Movimento após a colisão (com acelerações opostas)
                velocidadeA += aceleracaoA * DT;
                velocidadeB += aceleracaoB * DT;

                double novaPosiçãoA = posA + velocidadeA * ESCALA_PIXELS;
                double novaPosicaoB = posB + velocidadeB * ESCALA_PIXELS;

                // Verifica limites
                if (novaPosiçãoA < 0 || novaPosicaoB > SimulationCanvas.ActualWidth - ShapeB.Width)
                {
                    PararSimulacao();
                    MessageBox.Show("Os objetos saíram da área de simulação após a colisão!", "Simulação Concluída", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                Canvas.SetLeft(ObjectA, novaPosiçãoA);
                Canvas.SetLeft(ObjectB, novaPosicaoB);

                AtualizarVetores();
            }

            // Atualiza displays
            TxtVelocidadeA.Text = $"{velocidadeA:F2} m/s";
            TxtVelocidadeB.Text = $"{velocidadeB:F2} m/s";
            TxtAceleracaoA.Text = $"{aceleracaoA:F2} m/s²";
            TxtAceleracaoB.Text = $"{aceleracaoB:F2} m/s²";
        }

        private void ExecutarColisao()
        {
            // APLICAÇÃO DA 3ª LEI DE NEWTON
            // Forças iguais e opostas: F_A = -F_B

            // Força em A (para a esquerda, negativa)
            double forcaEmA = -forcaColisao;

            // Força em B (para a direita, positiva) - REAÇÃO
            double forcaEmB = forcaColisao;

            // Calcula acelerações usando F = m × a
            aceleracaoA = forcaEmA / massaA;  // Será negativa (para esquerda)
            aceleracaoB = forcaEmB / massaB;  // Será positiva (para direita)

            // Mostra vetores de força
            MostrarVetores();

            // Mostra ponto de colisão
            CollisionPoint.Visibility = Visibility.Visible;
            double centroCanvas = SimulationCanvas.ActualWidth / 2;
            Canvas.SetLeft(CollisionPoint, centroCanvas - 10);
            AnimarColisao();

            // Análise detalhada
            string analise = $"💥 COLISÃO DETECTADA!\n\n" +
                           $"Forças (3ª Lei de Newton):\n" +
                           $"• Força em A: {forcaEmA:F1} N (←)\n" +
                           $"• Força em B: {forcaEmB:F1} N (→)\n" +
                           $"• |F_A| = |F_B| = {forcaColisao:F1} N ✓\n\n" +
                           $"Acelerações (F = m × a):\n" +
                           $"• a_A = F/m_A = {forcaEmA:F1}/{massaA:F0} = {aceleracaoA:F2} m/s²\n" +
                           $"• a_B = F/m_B = {forcaEmB:F1}/{massaB:F0} = {aceleracaoB:F2} m/s²\n\n" +
                           $"Relação: a_A/a_B = m_B/m_A = {massaB / massaA:F2}\n\n" +
                           $"📌 Conclusão: Forças iguais causam acelerações diferentes se as massas forem diferentes!";

            TxtAnaliseColisao.Text = analise;
        }

        private void MostrarVetores()
        {
            double posA = Canvas.GetLeft(ObjectA);
            double posB = Canvas.GetLeft(ObjectB);

            // Vetor de força em A (AÇÃO - para esquerda, então inverte)
            ForceLineA.X1 = posA + ShapeA.Width / 2;
            ForceLineA.Y1 = 150;
            ForceLineA.X2 = posA + ShapeA.Width / 2 + 80;
            ForceLineA.Y2 = 150;

            ForceArrowA.Points = new PointCollection
            {
                new Point(ForceLineA.X2, 150),
                new Point(ForceLineA.X2 - 10, 145),
                new Point(ForceLineA.X2 - 10, 155)
            };

            ForceLineA.Visibility = Visibility.Visible;
            ForceArrowA.Visibility = Visibility.Visible;
            ForceLabelA.Visibility = Visibility.Visible;
            Canvas.SetLeft(ForceLabelA, ForceLineA.X1 + 30);

            // Vetor de força em B (REAÇÃO - para direita, então inverte)
            ForceLineB.X1 = posB + ShapeB.Width / 2;
            ForceLineB.Y1 = 150;
            ForceLineB.X2 = posB + ShapeB.Width / 2 - 80;
            ForceLineB.Y2 = 150;

            ForceArrowB.Points = new PointCollection
            {
                new Point(ForceLineB.X2, 150),
                new Point(ForceLineB.X2 + 10, 145),
                new Point(ForceLineB.X2 + 10, 155)
            };

            ForceLineB.Visibility = Visibility.Visible;
            ForceArrowB.Visibility = Visibility.Visible;
            ForceLabelB.Visibility = Visibility.Visible;
            Canvas.SetLeft(ForceLabelB, ForceLineB.X2 + 10);
        }

        private void AtualizarVetores()
        {
            double posA = Canvas.GetLeft(ObjectA);
            double posB = Canvas.GetLeft(ObjectB);

            // Atualiza posições dos vetores conforme os objetos se movem
            ForceLineA.X1 = posA + ShapeA.Width / 2;
            ForceLineA.X2 = posA + ShapeA.Width / 2 + 80;

            ForceArrowA.Points = new PointCollection
            {
                new Point(ForceLineA.X2, 150),
                new Point(ForceLineA.X2 - 10, 145),
                new Point(ForceLineA.X2 - 10, 155)
            };

            Canvas.SetLeft(ForceLabelA, ForceLineA.X1 + 30);

            ForceLineB.X1 = posB + ShapeB.Width / 2;
            ForceLineB.X2 = posB + ShapeB.Width / 2 - 80;

            ForceArrowB.Points = new PointCollection
            {
                new Point(ForceLineB.X2, 150),
                new Point(ForceLineB.X2 + 10, 145),
                new Point(ForceLineB.X2 + 10, 155)
            };

            Canvas.SetLeft(ForceLabelB, ForceLineB.X2 + 10);
        }

        private void EsconderVetores()
        {
            ForceLineA.Visibility = Visibility.Collapsed;
            ForceArrowA.Visibility = Visibility.Collapsed;
            ForceLabelA.Visibility = Visibility.Collapsed;

            ForceLineB.Visibility = Visibility.Collapsed;
            ForceArrowB.Visibility = Visibility.Collapsed;
            ForceLabelB.Visibility = Visibility.Collapsed;

            CollisionPoint.Visibility = Visibility.Collapsed;
        }

        private void PararSimulacao()
        {
            timer.Stop();
            simulacaoAtiva = false;
        }

        private void Resetar_Click(object sender, RoutedEventArgs e)
        {
            PararSimulacao();

            // Reset posições
            Canvas.SetLeft(ObjectA, 100);
            Canvas.SetLeft(ObjectB, SimulationCanvas.ActualWidth - 160);

            // Reset variáveis
            velocidadeA = 0;
            velocidadeB = 0;
            aceleracaoA = 0;
            aceleracaoB = 0;
            colidiu = false;

            // Reset displays
            TxtVelocidadeA.Text = "0 m/s";
            TxtVelocidadeB.Text = "0 m/s";
            TxtAceleracaoA.Text = "0 m/s²";
            TxtAceleracaoB.Text = "0 m/s²";
            TxtAnaliseColisao.Text = "Configure as massas e clique em 'Simular Colisão' para ver a análise";

            EsconderVetores();
        }

        // Cenários pré-definidos
        private void CenarioIguais_Click(object sender, RoutedEventArgs e)
        {
            SliderMassaA.Value = 10;
            SliderMassaB.Value = 10;
            SliderForca.Value = 50;
            MessageBox.Show("Cenário: Massas Iguais\n\n" +
                          "m_A = m_B = 10 kg\n" +
                          "As acelerações serão iguais em módulo!",
                          "Cenário Configurado", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void CenarioAMaior_Click(object sender, RoutedEventArgs e)
        {
            SliderMassaA.Value = 30;  // Caminhão
            SliderMassaB.Value = 5;   // Carro
            SliderForca.Value = 100;
            MessageBox.Show("Cenário: Caminhão vs Carro\n\n" +
                          "m_A = 30 kg (caminhão)\n" +
                          "m_B = 5 kg (carro)\n\n" +
                          "O carro terá aceleração muito maior!",
                          "Cenário Configurado", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void CenarioAMenor_Click(object sender, RoutedEventArgs e)
        {
            SliderMassaA.Value = 1;   // Bola
            SliderMassaB.Value = 50;  // Parede
            SliderForca.Value = 50;
            MessageBox.Show("Cenário: Bola vs Parede\n\n" +
                          "m_A = 1 kg (bola)\n" +
                          "m_B = 50 kg (parede)\n\n" +
                          "A bola sofre aceleração muito maior!",
                          "Cenário Configurado", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        // Animações
        private void AnimarElemento(UIElement elemento)
        {
            var fade = new DoubleAnimation
            {
                From = 0.5,
                To = 1.0,
                Duration = TimeSpan.FromSeconds(0.3)
            };
            elemento.BeginAnimation(OpacityProperty, fade);
        }

        private void AnimarColisao()
        {
            var scaleTransform = new ScaleTransform(1, 1);
            CollisionPoint.RenderTransform = scaleTransform;
            CollisionPoint.RenderTransformOrigin = new Point(0.5, 0.5);

            var scaleAnimation = new DoubleAnimation
            {
                From = 0.5,
                To = 2.0,
                Duration = TimeSpan.FromSeconds(0.5),
                AutoReverse = true
            };

            scaleTransform.BeginAnimation(ScaleTransform.ScaleXProperty, scaleAnimation);
            scaleTransform.BeginAnimation(ScaleTransform.ScaleYProperty, scaleAnimation);
        }
    }
}