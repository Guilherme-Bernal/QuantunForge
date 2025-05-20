using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Quantun_Forge.src.views.FisicaClassica
{
    public partial class LeiNewton3Control : UserControl
    {
        private DispatcherTimer timer;
        private double forca;
        private double massaA;
        private double massaB;
        private double aceleracaoA;
        private double aceleracaoB;
        private double velocidadeA = 0;
        private double velocidadeB = 0;

        public LeiNewton3Control()
        {
            InitializeComponent();

            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(16); // ~60 FPS
            timer.Tick += Timer_Tick;
        }

        private void BtnSimular_Click(object sender, RoutedEventArgs e)
        {
            // Captura valores
            forca = SliderForca.Value;
            massaA = SliderMassaA.Value;
            massaB = SliderMassaB.Value;

            // Acelerações opostas
            aceleracaoA = forca / massaA;
            aceleracaoB = forca / massaB;

            velocidadeA = aceleracaoA * 0.5; // deslocamento proporcional
            velocidadeB = aceleracaoB * 0.5;

            // Atualiza explicações
            TxtExplicacao.Text = $"Corpo A aplica {forca:F0} N em B → B aplica {-forca:F0} N em A (ação e reação).";
            TxtAceleracoes.Text = $"Aceleração A: {aceleracaoA:F2} m/s² | Aceleração B: {aceleracaoB:F2} m/s²";

            // Mostra setas
            AtualizarSetas();

            timer.Start();
        }

        private void BtnResetar_Click(object sender, RoutedEventArgs e)
        {
            timer.Stop();

            // Reset posições
            Canvas.SetLeft(BlocoA, 50);
            Canvas.SetLeft(BlocoB, 700);
            velocidadeA = 0;
            velocidadeB = 0;

            TxtExplicacao.Text = "";
            TxtAceleracoes.Text = "";

            LinhaSetaA.Visibility = Visibility.Collapsed;
            PontaSetaA.Visibility = Visibility.Collapsed;
            LinhaSetaB.Visibility = Visibility.Collapsed;
            PontaSetaB.Visibility = Visibility.Collapsed;
        }

        private void Timer_Tick(object? sender, EventArgs e)
        {
            double posA = Canvas.GetLeft(BlocoA);
            double posB = Canvas.GetLeft(BlocoB);

            double novoA = posA + velocidadeA;
            double novoB = posB - velocidadeB;

            // Detecção de limite
            if (novoA + BlocoA.Width < CanvasSimulacao.ActualWidth / 2 &&
                novoB > CanvasSimulacao.ActualWidth / 2)
            {
                Canvas.SetLeft(BlocoA, novoA);
                Canvas.SetLeft(BlocoB, novoB);
                AtualizarSetas();
            }
            else
            {
                timer.Stop();
                TxtExplicacao.Text += "\n\nOs corpos atingiram o limite da simulação.";
            }
        }

        private void AtualizarSetas()
        {
            // Atualiza a posição das setas de força

            // Corpo A → B
            double xA = Canvas.GetLeft(BlocoA) + BlocoA.Width;
            double yA = Canvas.GetTop(BlocoA) + BlocoA.Height / 2;

            LinhaSetaA.X1 = xA;
            LinhaSetaA.Y1 = yA;
            LinhaSetaA.X2 = xA + 30;
            LinhaSetaA.Y2 = yA;

            Canvas.SetLeft(PontaSetaA, xA + 30);
            Canvas.SetTop(PontaSetaA, yA - 3);

            LinhaSetaA.Visibility = Visibility.Visible;
            PontaSetaA.Visibility = Visibility.Visible;

            // Corpo B → A
            double xB = Canvas.GetLeft(BlocoB);
            double yB = Canvas.GetTop(BlocoB) + BlocoB.Height / 2;

            LinhaSetaB.X1 = xB;
            LinhaSetaB.Y1 = yB;
            LinhaSetaB.X2 = xB - 30;
            LinhaSetaB.Y2 = yB;

            Canvas.SetLeft(PontaSetaB, xB - 30);
            Canvas.SetTop(PontaSetaB, yB - 3);

            LinhaSetaB.Visibility = Visibility.Visible;
            PontaSetaB.Visibility = Visibility.Visible;
        }
    }
}
