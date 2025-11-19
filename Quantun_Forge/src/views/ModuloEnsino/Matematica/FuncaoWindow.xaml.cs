using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Quantun_Forge.src.views.ModuloEnsino.Matematica
{
    public partial class FuncaoWindow : Window
    {
        #region Constantes
        private const double ESCALA = 30;
        private const double ESPESSURA_EIXO = 2;
        private const double ESPESSURA_CURVA = 2.5;
        #endregion

        #region Construtor e Inicialização
        public FuncaoWindow()
        {
            InitializeComponent();
            Loaded += FuncaoWindow_Loaded;
        }

        private void FuncaoWindow_Loaded(object sender, RoutedEventArgs e)
        {
            AtualizarExemplos();
            AtualizarExplicacao();
            PlotarGrafico();
        }
        #endregion

        #region Eventos de Interface
        private void CmbTipoFuncao_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbTipoFuncao == null) return;

            bool isAfim = cmbTipoFuncao.SelectedIndex == 0;

            if (pnlFuncaoAfim != null)
                pnlFuncaoAfim.Visibility = isAfim ? Visibility.Visible : Visibility.Collapsed;

            if (pnlFuncaoQuadratica != null)
                pnlFuncaoQuadratica.Visibility = isAfim ? Visibility.Collapsed : Visibility.Visible;

            AtualizarExemplos();
            AtualizarExplicacao();
            PlotarGrafico();
        }

        private void Parametros_Changed(object sender, TextChangedEventArgs e)
        {
            if (IsLoaded)
            {
                PlotarGrafico();
            }
        }

        private void BtnPlotar_Click(object sender, RoutedEventArgs e)
        {
            PlotarGrafico();
        }

        private void BtnLimpar_Click(object sender, RoutedEventArgs e)
        {
            if (canvasGrafico == null) return;

            canvasGrafico.Children.Clear();

            if (txtAnalise != null)
                txtAnalise.Text = "Clique em 'Plotar Gráfico' para visualizar a função.";

            if (txtInfoGrafico != null)
                txtInfoGrafico.Text = "";
        }
        #endregion

        #region Plotagem Principal
        private void PlotarGrafico()
        {
            try
            {
                if (canvasGrafico == null || cmbTipoFuncao == null) return;

                canvasGrafico.Children.Clear();

                double width = canvasGrafico.ActualWidth > 0 ? canvasGrafico.ActualWidth : 800;
                double height = canvasGrafico.ActualHeight > 0 ? canvasGrafico.ActualHeight : 400;

                double centerX = width / 2;
                double centerY = height / 2;

                DesenharEixos(width, height, centerX, centerY);

                bool isAfim = cmbTipoFuncao.SelectedIndex == 0;

                if (isAfim)
                {
                    PlotarFuncaoAfim(centerX, centerY);
                }
                else
                {
                    PlotarFuncaoQuadratica(centerX, centerY);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao plotar gráfico: {ex.Message}", "Erro",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        #endregion

        #region Desenho de Eixos e Grade
        private void DesenharEixos(double width, double height, double centerX, double centerY)
        {
            if (canvasGrafico == null) return;

            // Eixo X
            Line eixoX = new Line
            {
                X1 = 0,
                Y1 = centerY,
                X2 = width,
                Y2 = centerY,
                Stroke = Brushes.Black,
                StrokeThickness = ESPESSURA_EIXO
            };
            canvasGrafico.Children.Add(eixoX);

            // Eixo Y
            Line eixoY = new Line
            {
                X1 = centerX,
                Y1 = 0,
                X2 = centerX,
                Y2 = height,
                Stroke = Brushes.Black,
                StrokeThickness = ESPESSURA_EIXO
            };
            canvasGrafico.Children.Add(eixoY);

            DesenharGrade(width, height, centerX, centerY);
            DesenharMarcacoes(width, height, centerX, centerY);
        }

        private void DesenharGrade(double width, double height, double centerX, double centerY)
        {
            if (canvasGrafico == null) return;

            // Linhas verticais
            for (double x = centerX % ESCALA; x < width; x += ESCALA)
            {
                Line linha = new Line
                {
                    X1 = x,
                    Y1 = 0,
                    X2 = x,
                    Y2 = height,
                    Stroke = new SolidColorBrush(Color.FromRgb(230, 230, 230)),
                    StrokeThickness = 0.5
                };
                canvasGrafico.Children.Add(linha);
            }

            // Linhas horizontais
            for (double y = centerY % ESCALA; y < height; y += ESCALA)
            {
                Line linha = new Line
                {
                    X1 = 0,
                    Y1 = y,
                    X2 = width,
                    Y2 = y,
                    Stroke = new SolidColorBrush(Color.FromRgb(230, 230, 230)),
                    StrokeThickness = 0.5
                };
                canvasGrafico.Children.Add(linha);
            }
        }

        private void DesenharMarcacoes(double width, double height, double centerX, double centerY)
        {
            if (canvasGrafico == null) return;

            // Marcações no eixo X (positivas)
            for (double x = centerX; x < width; x += ESCALA)
            {
                if (Math.Abs(x - centerX) < 1) continue;

                int valor = (int)Math.Round((x - centerX) / ESCALA);
                AdicionarMarcacao(x - 5, centerY + 5, valor.ToString());
            }

            // Marcações no eixo X (negativas)
            for (double x = centerX; x > 0; x -= ESCALA)
            {
                if (Math.Abs(x - centerX) < 1) continue;

                int valor = (int)Math.Round((x - centerX) / ESCALA);
                AdicionarMarcacao(x - 5, centerY + 5, valor.ToString());
            }

            // Marcações no eixo Y (positivas)
            for (double y = centerY; y > 0; y -= ESCALA)
            {
                if (Math.Abs(y - centerY) < 1) continue;

                int valor = (int)Math.Round((centerY - y) / ESCALA);
                AdicionarMarcacao(centerX + 5, y - 8, valor.ToString());
            }

            // Marcações no eixo Y (negativas)
            for (double y = centerY; y < height; y += ESCALA)
            {
                if (Math.Abs(y - centerY) < 1) continue;

                int valor = (int)Math.Round((centerY - y) / ESCALA);
                AdicionarMarcacao(centerX + 5, y - 8, valor.ToString());
            }
        }

        private void AdicionarMarcacao(double x, double y, string texto)
        {
            if (canvasGrafico == null) return;

            TextBlock txt = new TextBlock
            {
                Text = texto,
                FontSize = 10,
                Foreground = Brushes.Black
            };
            Canvas.SetLeft(txt, x);
            Canvas.SetTop(txt, y);
            canvasGrafico.Children.Add(txt);
        }
        #endregion

        #region Plotagem de Função Afim
        private void PlotarFuncaoAfim(double centerX, double centerY)
        {
            if (canvasGrafico == null || txtAfimA == null || txtAfimB == null) return;

            if (!double.TryParse(txtAfimA.Text, out double a) ||
                !double.TryParse(txtAfimB.Text, out double b))
            {
                MessageBox.Show("Por favor, insira valores numéricos válidos.", "Aviso");
                return;
            }

            double width = canvasGrafico.ActualWidth;
            double xMin = -centerX / ESCALA;
            double xMax = (width - centerX) / ESCALA;

            Polyline linha = new Polyline
            {
                Stroke = new SolidColorBrush(Color.FromRgb(41, 128, 185)),
                StrokeThickness = ESPESSURA_CURVA
            };

            for (double x = xMin; x <= xMax; x += 0.1)
            {
                double y = a * x + b;
                double canvasX = centerX + x * ESCALA;
                double canvasY = centerY - y * ESCALA;
                linha.Points.Add(new Point(canvasX, canvasY));
            }

            canvasGrafico.Children.Add(linha);

            // Marcar raiz se existir
            if (a != 0)
            {
                double raiz = -b / a;
                double raizX = centerX + raiz * ESCALA;
                double raizY = centerY;

                if (raizX >= 0 && raizX <= width)
                {
                    MarcarPonto(raizX, raizY, Brushes.Red, Brushes.DarkRed, 8);
                    AdicionarLabel(raizX + 10, raizY - 15, $"Raiz: ({raiz:F2}, 0)", Brushes.Red);
                }
            }

            AnalisarFuncaoAfim(a, b);
        }
        #endregion

        #region Plotagem de Função Quadrática
        private void PlotarFuncaoQuadratica(double centerX, double centerY)
        {
            if (canvasGrafico == null || txtQuadA == null || txtQuadB == null || txtQuadC == null) return;

            if (!double.TryParse(txtQuadA.Text, out double a) ||
                !double.TryParse(txtQuadB.Text, out double b) ||
                !double.TryParse(txtQuadC.Text, out double c))
            {
                MessageBox.Show("Por favor, insira valores numéricos válidos.", "Aviso");
                return;
            }

            if (a == 0)
            {
                MessageBox.Show("O coeficiente 'a' não pode ser zero em uma função quadrática.", "Aviso");
                return;
            }

            double width = canvasGrafico.ActualWidth;
            double height = canvasGrafico.ActualHeight;
            double xMin = -centerX / ESCALA;
            double xMax = (width - centerX) / ESCALA;

            Polyline parabola = new Polyline
            {
                Stroke = new SolidColorBrush(Color.FromRgb(142, 68, 173)),
                StrokeThickness = ESPESSURA_CURVA
            };

            for (double x = xMin; x <= xMax; x += 0.05)
            {
                double y = a * x * x + b * x + c;
                double canvasX = centerX + x * ESCALA;
                double canvasY = centerY - y * ESCALA;

                if (canvasY >= -50 && canvasY <= height + 50)
                {
                    parabola.Points.Add(new Point(canvasX, canvasY));
                }
            }

            canvasGrafico.Children.Add(parabola);

            // Calcular e marcar vértice
            double xv = -b / (2 * a);
            double yv = a * xv * xv + b * xv + c;
            double verticeX = centerX + xv * ESCALA;
            double verticeY = centerY - yv * ESCALA;

            MarcarPonto(verticeX, verticeY, Brushes.Orange, Brushes.DarkOrange, 10);
            AdicionarLabel(verticeX + 12, verticeY - 8, $"Vértice: ({xv:F2}, {yv:F2})", Brushes.Orange);

            // Calcular e marcar raízes
            double delta = b * b - 4 * a * c;
            if (delta >= 0)
            {
                double x1 = (-b + Math.Sqrt(delta)) / (2 * a);
                double x2 = (-b - Math.Sqrt(delta)) / (2 * a);

                MarcarRaiz(x1, centerX, centerY, width);
                if (delta > 0)
                {
                    MarcarRaiz(x2, centerX, centerY, width);
                }
            }

            AnalisarFuncaoQuadratica(a, b, c, delta, xv, yv);
        }

        private void MarcarRaiz(double raiz, double centerX, double centerY, double width)
        {
            if (canvasGrafico == null) return;

            double raizX = centerX + raiz * ESCALA;
            double raizY = centerY;

            if (raizX >= 0 && raizX <= width)
            {
                MarcarPonto(raizX, raizY, Brushes.Red, Brushes.DarkRed, 8);
                AdicionarLabel(raizX - 20, raizY + 10, $"x = {raiz:F2}", Brushes.Red);
            }
        }
        #endregion

        #region Métodos Auxiliares de Desenho
        private void MarcarPonto(double x, double y, Brush fill, Brush stroke, double tamanho)
        {
            if (canvasGrafico == null) return;

            Ellipse ponto = new Ellipse
            {
                Width = tamanho,
                Height = tamanho,
                Fill = fill,
                Stroke = stroke,
                StrokeThickness = 2
            };
            Canvas.SetLeft(ponto, x - tamanho / 2);
            Canvas.SetTop(ponto, y - tamanho / 2);
            canvasGrafico.Children.Add(ponto);
        }

        private void AdicionarLabel(double x, double y, string texto, Brush cor)
        {
            if (canvasGrafico == null) return;

            TextBlock lbl = new TextBlock
            {
                Text = texto,
                FontSize = 10,
                Foreground = cor,
                FontWeight = FontWeights.Bold
            };
            Canvas.SetLeft(lbl, x);
            Canvas.SetTop(lbl, y);
            canvasGrafico.Children.Add(lbl);
        }
        #endregion

        #region Análise de Funções
        private void AnalisarFuncaoAfim(double a, double b)
        {
            string analise = $"📊 ANÁLISE DA FUNÇÃO AFIM\n\n";
            analise += $"Equação: f(x) = {a}x + {b}\n\n";

            if (a > 0)
                analise += "📈 Função CRESCENTE (a > 0)\n";
            else if (a < 0)
                analise += "📉 Função DECRESCENTE (a < 0)\n";
            else
                analise += "➡️ Função CONSTANTE (a = 0)\n";

            analise += $"\n• Coef. Angular: {a}\n";
            analise += $"• Coef. Linear: {b}\n";

            if (a != 0)
            {
                double raiz = -b / a;
                analise += $"• Raiz: x = {raiz:F2}\n";
            }
            else
            {
                analise += "• Sem raiz (reta horizontal)\n";
            }

            if (txtAnalise != null)
                txtAnalise.Text = analise;

            if (txtInfoGrafico != null)
            {
                txtInfoGrafico.Text = $"f(x) = {a}x + {b}\n" +
                                      $"Inclinação: {Math.Abs(a):F2}\n" +
                                      $"Intercepto Y: {b}";
            }
        }

        private void AnalisarFuncaoQuadratica(double a, double b, double c, double delta, double xv, double yv)
        {
            string analise = $"📊 ANÁLISE DA FUNÇÃO QUADRÁTICA\n\n";
            analise += $"Equação: f(x) = {a}x² + {b}x + {c}\n\n";

            if (a > 0)
                analise += "⬆️ Concavidade PARA CIMA\n";
            else
                analise += "⬇️ Concavidade PARA BAIXO\n";

            analise += $"\n• Vértice: ({xv:F2}, {yv:F2})\n";
            analise += $"• Delta (Δ): {delta:F2}\n\n";

            if (delta > 0)
            {
                double x1 = (-b + Math.Sqrt(delta)) / (2 * a);
                double x2 = (-b - Math.Sqrt(delta)) / (2 * a);
                analise += $"✅ Duas raízes reais distintas:\n";
                analise += $"   x₁ = {x1:F2}\n";
                analise += $"   x₂ = {x2:F2}\n";
            }
            else if (delta == 0)
            {
                double x = -b / (2 * a);
                analise += $"✅ Uma raiz real (dupla):\n";
                analise += $"   x = {x:F2}\n";
            }
            else
            {
                analise += "❌ Não possui raízes reais\n";
                analise += "   (Δ < 0)\n";
            }

            if (a > 0)
                analise += $"\n📍 Ponto de MÍNIMO em y = {yv:F2}";
            else
                analise += $"\n📍 Ponto de MÁXIMO em y = {yv:F2}";

            if (txtAnalise != null)
                txtAnalise.Text = analise;

            if (txtInfoGrafico != null)
            {
                string infoGrafico = $"f(x) = {a}x² + {b}x + {c}\n";
                infoGrafico += $"Δ = {delta:F2}\n";
                infoGrafico += $"Vértice: ({xv:F2}, {yv:F2})";
                txtInfoGrafico.Text = infoGrafico;
            }
        }
        #endregion

        #region Exemplos e Explicações
        private void AtualizarExemplos()
        {
            if (pnlExemplos == null || cmbTipoFuncao == null) return;

            pnlExemplos.Children.Clear();

            bool isAfim = cmbTipoFuncao.SelectedIndex == 0;

            if (isAfim)
            {
                AdicionarExemplo("f(x) = 2x + 1", "2", "1", string.Empty);
                AdicionarExemplo("f(x) = -x + 3", "-1", "3", string.Empty);
                AdicionarExemplo("f(x) = 0.5x - 2", "0.5", "-2", string.Empty);
            }
            else
            {
                AdicionarExemplo("f(x) = x² - 4", "1", "0", "-4");
                AdicionarExemplo("f(x) = -x² + 2x + 3", "-1", "2", "3");
                AdicionarExemplo("f(x) = 2x² - 4x + 2", "2", "-4", "2");
            }
        }

        private void AdicionarExemplo(string nome, string a, string b, string c)
        {
            if (pnlExemplos == null) return;

            Button btnExemplo = new Button
            {
                Content = nome,
                Margin = new Thickness(0, 3, 0, 3),
                Padding = new Thickness(10, 5, 10, 5),
                HorizontalAlignment = HorizontalAlignment.Stretch,
                Background = new SolidColorBrush(Color.FromRgb(236, 240, 241)),
                Foreground = new SolidColorBrush(Color.FromRgb(52, 73, 94)),
                BorderBrush = new SolidColorBrush(Color.FromRgb(189, 195, 199)),
                BorderThickness = new Thickness(1),
                FontSize = 11,
                Cursor = System.Windows.Input.Cursors.Hand
            };

            btnExemplo.Click += (s, e) => CarregarExemplo(a, b, c);
            pnlExemplos.Children.Add(btnExemplo);
        }

        private void CarregarExemplo(string a, string b, string c)
        {
            if (cmbTipoFuncao == null) return;

            bool isAfim = cmbTipoFuncao.SelectedIndex == 0;

            if (isAfim)
            {
                if (txtAfimA != null) txtAfimA.Text = a;
                if (txtAfimB != null) txtAfimB.Text = b;
            }
            else
            {
                if (txtQuadA != null) txtQuadA.Text = a;
                if (txtQuadB != null) txtQuadB.Text = b;
                if (txtQuadC != null) txtQuadC.Text = c ?? "0";
            }

            PlotarGrafico();
        }

        private void AtualizarExplicacao()
        {
            if (txtExplicacao == null || cmbTipoFuncao == null) return;

            bool isAfim = cmbTipoFuncao.SelectedIndex == 0;

            if (isAfim)
            {
                txtExplicacao.Text =
                    "A função afim (f(x) = ax + b) representa uma relação linear entre x e y. " +
                    "O coeficiente 'a' determina a inclinação da reta: se a > 0, a função é crescente; " +
                    "se a < 0, é decrescente. O coeficiente 'b' indica onde a reta corta o eixo Y. " +
                    "A raiz da função é o valor de x quando f(x) = 0, calculada por x = -b/a.\n\n" +
                    "Aplicações práticas: conversão de temperaturas, cálculo de custos fixos e variáveis, " +
                    "velocidade constante, entre outros.";
            }
            else
            {
                txtExplicacao.Text =
                    "A função quadrática (f(x) = ax² + bx + c) forma uma parábola no gráfico. " +
                    "O coeficiente 'a' define a concavidade: a > 0 (côncava para cima) ou a < 0 (côncava para baixo). " +
                    "O vértice V(xᵥ, yᵥ) representa o ponto de máximo ou mínimo. " +
                    "O discriminante Δ = b² - 4ac determina o número de raízes reais.\n\n" +
                    "Aplicações práticas: trajetória de projéteis, otimização de lucros, " +
                    "modelagem de áreas, movimento uniformemente variado (física).";
            }
        }
        #endregion
    }
}