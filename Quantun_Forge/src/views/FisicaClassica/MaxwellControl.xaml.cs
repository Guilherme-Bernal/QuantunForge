using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;
using System.Windows.Media;
using System.Windows.Threading;

namespace Quantun_Forge.src.views.FisicaClassica
{
    public partial class MaxwellControl : UserControl
    {
        private DispatcherTimer timer;
        private double fase = 0;
        private List<Line> linhasE = new(); // Campo elétrico
        private List<Line> linhasB = new(); // Campo magnético

        public MaxwellControl()
        {
            InitializeComponent();
            Loaded += MaxwellControl_Loaded;
        }

        private void MaxwellControl_Loaded(object sender, RoutedEventArgs e)
        {
            CriarLinhas();
            timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(30)
            };
            timer.Tick += OscilarCampos;
        }

        private void CriarLinhas()
        {
            CanvasOnda.Children.Clear();
            linhasE.Clear();
            linhasB.Clear();

            int espaco = 20;
            double altura = CanvasOnda.ActualHeight;

            for (int x = 0; x < CanvasOnda.ActualWidth; x += espaco)
            {
                // Campo elétrico (vertical)
                var linhaE = new Line
                {
                    X1 = x,
                    X2 = x,
                    Y1 = altura / 2 - 30,
                    Y2 = altura / 2 + 30,
                    Stroke = Brushes.DeepSkyBlue,
                    StrokeThickness = 2
                };
                CanvasOnda.Children.Add(linhaE);
                linhasE.Add(linhaE);

                // Campo magnético (horizontal)
                var linhaB = new Line
                {
                    Y1 = altura / 2,
                    Y2 = altura / 2,
                    X1 = x - 30,
                    X2 = x + 30,
                    Stroke = Brushes.Orange,
                    StrokeThickness = 2
                };
                CanvasOnda.Children.Add(linhaB);
                linhasB.Add(linhaB);
            }
        }

        private void OscilarCampos(object? sender, EventArgs e)
        {
            fase += 0.2;
            double altura = CanvasOnda.ActualHeight;

            for (int i = 0; i < linhasE.Count; i++)
            {
                double offset = Math.Sin(fase + i * 0.5) * 30;

                // Elétrico (vertical)
                linhasE[i].Y1 = altura / 2 - offset;
                linhasE[i].Y2 = altura / 2 + offset;

                // Magnético (horizontal)
                linhasB[i].X1 = linhasE[i].X1 - offset;
                linhasB[i].X2 = linhasE[i].X1 + offset;
            }
        }

        private void BtnIniciar_Click(object sender, RoutedEventArgs e)
        {
            if (!timer.IsEnabled)
            {
                CriarLinhas();
                timer.Start();
            }
        }
    }
}
