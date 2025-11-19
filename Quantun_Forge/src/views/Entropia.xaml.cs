using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace Quantun_Forge.src.views
{
    public partial class Entropia : UserControl
    {
        private bool isUpdating = false;
        private bool isInitialized = false;

        public Entropia()
        {
            InitializeComponent();
            isInitialized = true;
            InitializeSimulator();
        }

        #region Inicialização

        private void InitializeSimulator()
        {
            // Configuração inicial com probabilidades iguais (0.5, 0.5)
            SliderP1.Value = 0.5;
            SliderP2.Value = 0.5;

            UpdateSimulation();
        }

        #endregion

        #region Eventos dos Sliders

        private void SliderProbabilities_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            // Verificar se os controles estão inicializados
            if (!isInitialized || isUpdating)
                return;

            if (SliderP1 == null || SliderP2 == null)
                return;

            isUpdating = true;

            try
            {
                // Normalizar para que p1 + p2 = 1
                if (sender == SliderP1)
                {
                    double p1 = SliderP1.Value;
                    double p2 = 1.0 - p1;

                    // Garantir que p2 esteja no range válido
                    if (p2 < 0) p2 = 0;
                    if (p2 > 1) p2 = 1;

                    SliderP2.Value = p2;
                }
                else if (sender == SliderP2)
                {
                    double p2 = SliderP2.Value;
                    double p1 = 1.0 - p2;

                    // Garantir que p1 esteja no range válido
                    if (p1 < 0) p1 = 0;
                    if (p1 > 1) p1 = 1;

                    SliderP1.Value = p1;
                }

                UpdateSimulation();
            }
            finally
            {
                isUpdating = false;
            }
        }

        private void BtnResetEntropy_Click(object sender, RoutedEventArgs e)
        {
            if (!isInitialized)
                return;

            isUpdating = true;

            SliderP1.Value = 0.5;
            SliderP2.Value = 0.5;

            isUpdating = false;

            UpdateSimulation();
        }

        #endregion

        #region Atualização da Simulação

        private void UpdateSimulation()
        {
            if (!isInitialized)
                return;

            // Verificar se todos os controles necessários existem
            if (SliderP1 == null || SliderP2 == null ||
                TxtP1 == null || TxtP2 == null ||
                TxtShannonEntropy == null || TxtVonNeumannEntropy == null ||
                TxtEntropyComparison == null ||
                Bar1 == null || Bar2 == null ||
                ValueBar1 == null || ValueBar2 == null)
            {
                return;
            }

            // Obter probabilidades
            double p1 = SliderP1.Value;
            double p2 = SliderP2.Value;

            // Normalizar (garantir que soma = 1)
            double sum = p1 + p2;
            if (sum > 0)
            {
                p1 /= sum;
                p2 /= sum;
            }

            // Atualizar textos das probabilidades
            TxtP1.Text = $"p₁ = {p1:F2}";
            TxtP2.Text = $"p₂ = {p2:F2}";

            // Calcular entropias
            double shannonEntropy = CalculateShannonEntropy(p1, p2);
            double vonNeumannEntropy = CalculateVonNeumannEntropy(p1, p2);

            // Atualizar textos das entropias
            TxtShannonEntropy.Text = $"H(X) = {shannonEntropy:F4} bits";
            TxtVonNeumannEntropy.Text = $"S(ρ) = {vonNeumannEntropy:F4} bits";

            // Atualizar texto de comparação
            if (Math.Abs(shannonEntropy - vonNeumannEntropy) < 0.0001)
            {
                TxtEntropyComparison.Text = "(Estado diagonal: S = H) ✓";
            }
            else
            {
                TxtEntropyComparison.Text = $"(Diferença: {Math.Abs(shannonEntropy - vonNeumannEntropy):F4} bits)";
            }

            // Atualizar visualização
            UpdateVisualization(p1, p2);
        }

        #endregion

        #region Cálculo de Entropias

        /// <summary>
        /// Calcula a entropia de Shannon: H(X) = -Σ pᵢ log₂(pᵢ)
        /// </summary>
        private double CalculateShannonEntropy(double p1, double p2)
        {
            double entropy = 0.0;

            if (p1 > 0)
            {
                entropy -= p1 * Math.Log(p1, 2);
            }

            if (p2 > 0)
            {
                entropy -= p2 * Math.Log(p2, 2);
            }

            return entropy;
        }

        /// <summary>
        /// Calcula a entropia de von Neumann para estado diagonal: S(ρ) = -Σ λᵢ log₂(λᵢ)
        /// Para um estado diagonal, os autovalores são as probabilidades diagonais
        /// </summary>
        private double CalculateVonNeumannEntropy(double lambda1, double lambda2)
        {
            // Para matriz densidade diagonal, os autovalores são os elementos diagonais
            // S(ρ) = -Tr(ρ log₂(ρ)) = -Σ λᵢ log₂(λᵢ)
            // Que é exatamente a entropia de Shannon dos autovalores!

            double entropy = 0.0;

            if (lambda1 > 0)
            {
                entropy -= lambda1 * Math.Log(lambda1, 2);
            }

            if (lambda2 > 0)
            {
                entropy -= lambda2 * Math.Log(lambda2, 2);
            }

            return entropy;
        }

        #endregion

        #region Visualização

        private void UpdateVisualization(double p1, double p2)
        {
            if (Bar1 == null || Bar2 == null || ValueBar1 == null || ValueBar2 == null)
                return;

            // Atualizar altura das barras (escala de 0 a 220 pixels)
            // Altura máxima = 220 pixels quando probabilidade = 1.0
            double maxHeight = 220;

            double height1 = p1 * maxHeight;
            double height2 = p2 * maxHeight;

            // Posição Y (de cima para baixo)
            // Base está em Y=250, então Y = 250 - altura
            double y1 = 250 - height1;
            double y2 = 250 - height2;

            // Animar altura das barras
            AnimateBarHeight(Bar1, height1, y1);
            AnimateBarHeight(Bar2, height2, y2);

            // Atualizar valores exibidos nas barras
            ValueBar1.Text = p1.ToString("F2");
            ValueBar2.Text = p2.ToString("F2");

            // Posicionar os valores acima das barras
            double labelY1 = y1 - 25;
            double labelY2 = y2 - 25;

            if (labelY1 < 0) labelY1 = 0;
            if (labelY2 < 0) labelY2 = 0;

            AnimateLabelPosition(ValueBar1, labelY1);
            AnimateLabelPosition(ValueBar2, labelY2);

            // Atualizar cores baseado na probabilidade
            UpdateBarColors(p1, p2);
        }

        private void AnimateBarHeight(System.Windows.Shapes.Rectangle bar, double newHeight, double newY)
        {
            if (bar == null)
                return;

            try
            {
                // Animar altura
                var heightAnimation = new DoubleAnimation
                {
                    To = newHeight,
                    Duration = TimeSpan.FromMilliseconds(300),
                    EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut }
                };

                // Animar posição Y
                var yAnimation = new DoubleAnimation
                {
                    To = newY,
                    Duration = TimeSpan.FromMilliseconds(300),
                    EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut }
                };

                bar.BeginAnimation(System.Windows.Shapes.Rectangle.HeightProperty, heightAnimation);
                bar.BeginAnimation(Canvas.TopProperty, yAnimation);
            }
            catch
            {
                // Fallback: atualizar sem animação
                bar.Height = newHeight;
                Canvas.SetTop(bar, newY);
            }
        }

        private void AnimateLabelPosition(TextBlock label, double newY)
        {
            if (label == null)
                return;

            try
            {
                var yAnimation = new DoubleAnimation
                {
                    To = newY,
                    Duration = TimeSpan.FromMilliseconds(300),
                    EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut }
                };

                label.BeginAnimation(Canvas.TopProperty, yAnimation);
            }
            catch
            {
                // Fallback: atualizar sem animação
                Canvas.SetTop(label, newY);
            }
        }

        private void UpdateBarColors(double p1, double p2)
        {
            if (Bar1 == null || Bar2 == null)
                return;

            try
            {
                // Cores baseadas na magnitude da probabilidade
                // Mais intenso = maior probabilidade

                // Bar1 (azul)
                byte blue1Intensity = (byte)(100 + (p1 * 155)); // 100-255
                Bar1.Fill = new System.Windows.Media.SolidColorBrush(
                    System.Windows.Media.Color.FromRgb(52, 152, blue1Intensity));

                // Bar2 (roxo)
                byte purple2Intensity = (byte)(100 + (p2 * 155)); // 100-255
                Bar2.Fill = new System.Windows.Media.SolidColorBrush(
                    System.Windows.Media.Color.FromRgb(155, 89, purple2Intensity));
            }
            catch
            {
                // Ignorar erros de cor
            }
        }

        #endregion

        #region Métodos Auxiliares

        /// <summary>
        /// Verifica se um valor está muito próximo de zero (para evitar log(0))
        /// </summary>
        private bool IsEffectivelyZero(double value)
        {
            return Math.Abs(value) < 1e-10;
        }

        /// <summary>
        /// Calcula -p * log₂(p) com tratamento para p=0
        /// </summary>
        private double EntropyTerm(double p)
        {
            if (IsEffectivelyZero(p) || p <= 0)
                return 0.0;

            return -p * Math.Log(p, 2);
        }

        /// <summary>
        /// Interpreta o valor da entropia
        /// </summary>
        private string InterpretEntropy(double entropy)
        {
            if (entropy < 0.1)
                return "Muito baixa - Sistema quase determinístico";
            else if (entropy < 0.5)
                return "Baixa - Pouca incerteza";
            else if (entropy < 0.9)
                return "Moderada - Incerteza intermediária";
            else if (entropy < 0.99)
                return "Alta - Grande incerteza";
            else
                return "Máxima - Equiprovável";
        }

        #endregion

        #region Exemplos Pré-definidos (opcional para futuras expansões)

        /// <summary>
        /// Define um estado determinístico (p1=1, p2=0)
        /// </summary>
        public void SetDeterministicState()
        {
            if (!isInitialized)
                return;

            isUpdating = true;
            SliderP1.Value = 1.0;
            SliderP2.Value = 0.0;
            isUpdating = false;
            UpdateSimulation();
        }

        /// <summary>
        /// Define um estado maximalmente misto (p1=0.5, p2=0.5)
        /// </summary>
        public void SetMaximallyMixedState()
        {
            if (!isInitialized)
                return;

            isUpdating = true;
            SliderP1.Value = 0.5;
            SliderP2.Value = 0.5;
            isUpdating = false;
            UpdateSimulation();
        }

        /// <summary>
        /// Define um estado com probabilidades customizadas
        /// </summary>
        public void SetCustomState(double p1, double p2)
        {
            if (!isInitialized)
                return;

            // Normalizar
            double sum = p1 + p2;
            if (sum > 0)
            {
                p1 /= sum;
                p2 /= sum;
            }
            else
            {
                p1 = 0.5;
                p2 = 0.5;
            }

            isUpdating = true;
            SliderP1.Value = p1;
            SliderP2.Value = p2;
            isUpdating = false;
            UpdateSimulation();
        }

        #endregion

        #region Cálculos Avançados (para referência educacional)

        /// <summary>
        /// Calcula a matriz densidade diagonal para o estado dado
        /// </summary>
        private double[,] GetDensityMatrix(double p1, double p2)
        {
            return new double[,]
            {
                { p1, 0 },
                { 0, p2 }
            };
        }

        /// <summary>
        /// Calcula os autovalores de uma matriz 2x2 diagonal
        /// Para matriz diagonal, os autovalores são os elementos diagonais
        /// </summary>
        private (double lambda1, double lambda2) GetEigenvalues(double[,] densityMatrix)
        {
            // Para matriz diagonal, autovalores são os elementos diagonais
            return (densityMatrix[0, 0], densityMatrix[1, 1]);
        }

        /// <summary>
        /// Calcula a pureza do estado: Tr(ρ²)
        /// Pureza = 1 para estado puro, Pureza = 1/d para estado maximalmente misto
        /// </summary>
        private double CalculatePurity(double p1, double p2)
        {
            return p1 * p1 + p2 * p2;
        }

        /// <summary>
        /// Verifica se o estado é puro
        /// Estado é puro se Tr(ρ²) = 1
        /// </summary>
        private bool IsPureState(double p1, double p2)
        {
            double purity = CalculatePurity(p1, p2);
            return Math.Abs(purity - 1.0) < 0.0001;
        }

        /// <summary>
        /// Calcula a entropia de Rényi de ordem α
        /// Para α → 1, converge para entropia de von Neumann
        /// </summary>
        private double CalculateRenyiEntropy(double p1, double p2, double alpha)
        {
            if (Math.Abs(alpha - 1.0) < 0.0001)
            {
                // Limite α → 1 é a entropia de von Neumann
                return CalculateVonNeumannEntropy(p1, p2);
            }

            if (alpha <= 0)
            {
                throw new ArgumentException("Alpha deve ser positivo");
            }

            double sum = Math.Pow(p1, alpha) + Math.Pow(p2, alpha);

            if (sum <= 0)
                return 0;

            return (1.0 / (1.0 - alpha)) * Math.Log(sum, 2);
        }

        /// <summary>
        /// Calcula a entropia de Tsallis
        /// Generalização não-aditiva da entropia
        /// </summary>
        private double CalculateTsallisEntropy(double p1, double p2, double q)
        {
            if (Math.Abs(q - 1.0) < 0.0001)
            {
                // Para q = 1, converge para Shannon
                return CalculateShannonEntropy(p1, p2);
            }

            double sum = Math.Pow(p1, q) + Math.Pow(p2, q);
            return (1.0 / (q - 1.0)) * (1.0 - sum);
        }

        /// <summary>
        /// Calcula a informação mútua entre dois sistemas
        /// I(A:B) = S(A) + S(B) - S(AB)
        /// </summary>
        private double CalculateMutualInformation(double pA1, double pA2, double pB1, double pB2)
        {
            double SA = CalculateVonNeumannEntropy(pA1, pA2);
            double SB = CalculateVonNeumannEntropy(pB1, pB2);

            // Para estados independentes: S(AB) = S(A) + S(B)
            // Logo I(A:B) = 0

            // Para estados correlacionados, precisaríamos da matriz densidade conjunta
            // Aqui retornamos 0 assumindo independência
            return 0.0;
        }

        #endregion
    }
}