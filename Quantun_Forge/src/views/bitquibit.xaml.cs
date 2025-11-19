using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace Quantun_Forge.src.views
{
    /// <summary>
    /// Módulo interativo Bit vs Qubit
    /// Demonstra a diferença fundamental entre computação clássica e quântica
    /// 
    /// CONCEITOS-CHAVE:
    /// - BIT: Estado definido (0 OU 1) - Probabilidade epistêmica (sua ignorância)
    /// - QUBIT: Superposição (0 E 1) - Probabilidade ontológica (parte do estado)
    /// - ESFERA DE BLOCH: Representação visual - cada ponto = um par de probabilidades único
    /// </summary>
    public partial class bitquibit : UserControl
    {
        /// <summary>
        /// Ângulo theta (0° a 180°) - CONTROLA AS PROBABILIDADES
        /// θ = 0°   → P(0)=100%, P(1)=0%   (estado |0⟩)
        /// θ = 90°  → P(0)=50%,  P(1)=50%  (superposição máxima)
        /// θ = 180° → P(0)=0%,   P(1)=100% (estado |1⟩)
        /// </summary>
        private double _theta = 0;

        /// <summary>
        /// Ângulo phi (0° a 360°) - CONTROLA A FASE
        /// Não muda probabilidades sozinho, mas permite interferência
        /// Essencial para algoritmos quânticos funcionarem
        /// </summary>
        private double _phi = 0;

        public bitquibit()
        {
            InitializeComponent();
            InicializarAnimacoes();
            AtualizarEsferaBloch();
        }

        /// <summary>
        /// Inicializa as animações de entrada
        /// </summary>
        private void InicializarAnimacoes()
        {
            Loaded += (sender, e) =>
            {
                var fadeIn = FindResource("FadeInAnimation") as Storyboard;
                fadeIn?.Begin(this);
            };
        }

        #region Simulador 1: Bit vs Qubit

        /// <summary>
        /// Define o bit para 0
        /// IMPORTANTE: Bit clássico não está em superposição - ele JÁ É 0 ou 1
        /// Probabilidade = 100% no estado escolhido
        /// </summary>
        private void BtnBit0_Click(object sender, RoutedEventArgs e)
        {
            AnimarBit(50, "Bit = 0 (100%)");
        }

        /// <summary>
        /// Define o bit para 1
        /// Bit clássico: estado definido, sem superposição
        /// Probabilidade = 100% no estado escolhido
        /// </summary>
        private void BtnBit1_Click(object sender, RoutedEventArgs e)
        {
            AnimarBit(320, "Bit = 1 (100%)");
        }

        /// <summary>
        /// Anima o bit para uma posição na linha binária
        /// Representa o movimento entre dois estados EXCLUSIVOS
        /// </summary>
        /// <param name="posX">Posição X no canvas (50 para 0, 320 para 1)</param>
        /// <param name="estado">Texto descritivo do estado</param>
        private void AnimarBit(double posX, string estado)
        {
            if (BitAtual == null || TxtEstadoBit == null) return;

            var animacao = new DoubleAnimation
            {
                To = posX,
                Duration = TimeSpan.FromSeconds(0.5),
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseInOut }
            };

            BitAtual.BeginAnimation(Canvas.LeftProperty, animacao);
            TxtEstadoBit.Text = $"Estado: {estado}";
        }

        /// <summary>
        /// Define o qubit para |0⟩ (polo norte da esfera)
        /// P(0) = 100%, P(1) = 0%
        /// Apesar de 100%, ainda é um ESTADO QUÂNTICO (diferente do bit)
        /// </summary>
        private void BtnQubit0_Click(object sender, RoutedEventArgs e)
        {
            AnimarQubit(190, 50, "|ψ⟩ = |0⟩ (100%/0%)");
        }

        /// <summary>
        /// Define o qubit para |+⟩ (equador da esfera)
        /// SUPERPOSIÇÃO MÁXIMA: |+⟩ = (|0⟩ + |1⟩)/√2
        /// P(0) = 50%, P(1) = 50%
        /// O qubit está LITERALMENTE em ambos os estados simultaneamente
        /// </summary>
        private void BtnQubitPlus_Click(object sender, RoutedEventArgs e)
        {
            AnimarQubit(190, 140, "|ψ⟩ = (|0⟩ + |1⟩)/√2 (50%/50%)");
        }

        /// <summary>
        /// Define o qubit para |1⟩ (polo sul da esfera)
        /// P(0) = 0%, P(1) = 100%
        /// </summary>
        private void BtnQubit1_Click(object sender, RoutedEventArgs e)
        {
            AnimarQubit(190, 230, "|ψ⟩ = |1⟩ (0%/100%)");
        }

        /// <summary>
        /// Anima o qubit e o vetor de estado na representação simplificada
        /// </summary>
        private void AnimarQubit(double posX, double posY, string estado)
        {
            if (QubitAtual == null || VetorEstado == null || TxtEstadoQubit == null) return;

            // Animar posição do qubit
            var animacaoX = new DoubleAnimation
            {
                To = posX,
                Duration = TimeSpan.FromSeconds(0.5),
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseInOut }
            };

            var animacaoY = new DoubleAnimation
            {
                To = posY,
                Duration = TimeSpan.FromSeconds(0.5),
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseInOut }
            };

            QubitAtual.BeginAnimation(Canvas.LeftProperty, animacaoX);
            QubitAtual.BeginAnimation(Canvas.TopProperty, animacaoY);

            // Animar vetor
            var animacaoVetorY = new DoubleAnimation
            {
                To = posY + 10,
                Duration = TimeSpan.FromSeconds(0.5),
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseInOut }
            };

            VetorEstado.BeginAnimation(Line.Y2Property, animacaoVetorY);

            TxtEstadoQubit.Text = $"Estado: {estado}";
        }

        #endregion

        #region Simulador 2: Esfera de Bloch

        /// <summary>
        /// Evento de mudança do ângulo theta
        /// THETA É A VARIÁVEL QUE CONTROLA AS PROBABILIDADES!
        /// 
        /// Fórmula das probabilidades:
        /// P(0) = cos²(θ/2)
        /// P(1) = sin²(θ/2)
        /// 
        /// Exemplos:
        /// θ=0°   → cos²(0)=1,   sin²(0)=0   → P(0)=100%, P(1)=0%
        /// θ=90°  → cos²(45)≈0.5, sin²(45)≈0.5 → P(0)=50%,  P(1)=50%
        /// θ=180° → cos²(90)=0,  sin²(90)=1  → P(0)=0%,   P(1)=100%
        /// </summary>
        private void SliderTheta_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            _theta = e.NewValue;

            if (TxtTheta != null)
            {
                // Calcular as probabilidades baseadas em theta
                double thetaRad = _theta * Math.PI / 180.0;
                double prob0 = Math.Cos(thetaRad / 2.0);
                prob0 = prob0 * prob0; // |α|² = cos²(θ/2)

                double prob1 = Math.Sin(thetaRad / 2.0);
                prob1 = prob1 * prob1; // |β|² = sin²(θ/2)

                string descricao = ObterDescricaoTheta(_theta);
                TxtTheta.Text = $"θ = {_theta:F0}° {descricao} → P(0)={prob0 * 100:F0}%, P(1)={prob1 * 100:F0}%";
            }

            AtualizarEsferaBloch();
        }

        /// <summary>
        /// Evento de mudança do ângulo phi
        /// PHI CONTROLA A FASE - não muda probabilidades sozinho
        /// 
        /// A fase é crucial para:
        /// - Interferência construtiva e destrutiva
        /// - Algoritmos quânticos (Shor, Grover, etc.)
        /// - Criar correlações entre qubits
        /// 
        /// Matematicamente: β = sin(θ/2) × e^(iφ)
        /// A fase e^(iφ) não afeta |β|², mas muda como o qubit interage
        /// </summary>
        private void SliderPhi_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            _phi = e.NewValue;

            if (TxtPhi != null)
            {
                TxtPhi.Text = $"φ = {_phi:F0}° (fase - não muda probabilidades sozinho)";
            }

            AtualizarEsferaBloch();
        }

        /// <summary>
        /// Obtém descrição do estado baseado em theta
        /// </summary>
        private string ObterDescricaoTheta(double theta)
        {
            if (theta == 0) return "(Estado |0⟩)";
            if (theta == 90) return "(Superposição máxima)";
            if (theta == 180) return "(Estado |1⟩)";
            if (theta < 90) return "(Mais |0⟩ que |1⟩)";
            return "(Mais |1⟩ que |0⟩)";
        }

        /// <summary>
        /// Atualiza a visualização da Esfera de Bloch
        /// 
        /// CONCEITOS IMPLEMENTADOS:
        /// 1. Coordenadas esféricas → Cartesianas
        /// 2. Cálculo de amplitudes (α, β)
        /// 3. Cálculo de probabilidades (|α|², |β|²)
        /// 4. Visualização da fase (projeção no plano XY)
        /// </summary>
        private void AtualizarEsferaBloch()
        {
            if (VetorBloch == null || PontoBloch == null || ProjecaoFase == null) return;
            if (TxtFormula == null || TxtProbabilidades == null) return;

            // Converter ângulos para radianos
            double thetaRad = _theta * Math.PI / 180.0;
            double phiRad = _phi * Math.PI / 180.0;

            // Calcular coordenadas na esfera (raio = 150 pixels)
            double raio = 150;
            double centroX = 175;
            double centroY = 175;

            // Coordenadas esféricas → Cartesianas
            // x = r·sin(θ)·cos(φ)
            // y = r·sin(θ)·sin(φ)  
            // z = r·cos(θ)
            double x = raio * Math.Sin(thetaRad) * Math.Cos(phiRad);
            double y = raio * Math.Sin(thetaRad) * Math.Sin(phiRad);
            double z = raio * Math.Cos(thetaRad);

            // Posição final no canvas (projeção 2D)
            double posX = centroX + x;
            double posY = centroY - z; // Invertido porque Y cresce para baixo

            // Animar vetor
            var animacaoVetorX = new DoubleAnimation
            {
                To = posX,
                Duration = TimeSpan.FromSeconds(0.3)
            };

            var animacaoVetorY = new DoubleAnimation
            {
                To = posY,
                Duration = TimeSpan.FromSeconds(0.3)
            };

            VetorBloch.BeginAnimation(Line.X2Property, animacaoVetorX);
            VetorBloch.BeginAnimation(Line.Y2Property, animacaoVetorY);

            // Animar ponto
            var animacaoPontoX = new DoubleAnimation
            {
                To = posX - 10,
                Duration = TimeSpan.FromSeconds(0.3)
            };

            var animacaoPontoY = new DoubleAnimation
            {
                To = posY - 10,
                Duration = TimeSpan.FromSeconds(0.3)
            };

            PontoBloch.BeginAnimation(Canvas.LeftProperty, animacaoPontoX);
            PontoBloch.BeginAnimation(Canvas.TopProperty, animacaoPontoY);

            // Projeção da fase no plano XY (mostra a componente da fase)
            double projecaoX = centroX + (raio * Math.Sin(thetaRad) * Math.Cos(phiRad));

            var animacaoProjecao = new DoubleAnimation
            {
                To = projecaoX,
                Duration = TimeSpan.FromSeconds(0.3)
            };

            ProjecaoFase.BeginAnimation(Line.X2Property, animacaoProjecao);

            // ===== CÁLCULO DAS AMPLITUDES E PROBABILIDADES =====
            // 
            // Estado geral do qubit:
            // |ψ⟩ = α|0⟩ + β|1⟩
            // 
            // Onde:
            // α = cos(θ/2)           → amplitude do estado |0⟩
            // β = sin(θ/2)·e^(iφ)    → amplitude do estado |1⟩ (com fase)
            // 
            // Probabilidades (Regra de Born):
            // P(0) = |α|² = cos²(θ/2)
            // P(1) = |β|² = sin²(θ/2)
            // 
            // Nota: |e^(iφ)|² = 1, então a fase não afeta |β|²

            double alpha = Math.Cos(thetaRad / 2.0);
            double beta = Math.Sin(thetaRad / 2.0);

            // Considerar a fase para beta complexo
            // β = |β|·e^(iφ) = |β|·(cos(φ) + i·sin(φ))
            double betaReal = beta * Math.Cos(phiRad);
            double betaImag = beta * Math.Sin(phiRad);

            // PROBABILIDADES (independentes da fase!)
            double prob0 = alpha * alpha;  // P(0) = |α|²
            double prob1 = beta * beta;    // P(1) = |β|²

            // Atualizar textos com as amplitudes
            string alphaStr = $"α = {alpha:F2}";
            string betaStr;

            if (Math.Abs(betaImag) < 0.01)
            {
                // Fase desprezível, mostrar só a parte real
                betaStr = $"β = {betaReal:F2}";
            }
            else
            {
                // Mostrar número complexo completo
                string sinal = betaImag >= 0 ? "+" : "-";
                betaStr = $"β = {betaReal:F2} {sinal} {Math.Abs(betaImag):F2}i";
            }

            TxtFormula.Text = $"|ψ⟩ = α|0⟩ + β|1⟩\n\n{alphaStr}\n{betaStr}";

            // ATUALIZAR PROBABILIDADES - A PARTE MAIS IMPORTANTE!
            TxtProbabilidades.Text =
                $"📊 PROBABILIDADES:\n\n" +
                $"P(0) = |α|² = {prob0 * 100:F1}%\n" +
                $"P(1) = |β|² = {prob1 * 100:F1}%\n\n" +
                $"(Ao medir, o qubit colapsa)";
        }

        /// <summary>
        /// Define o estado para |0⟩ (polo norte)
        /// θ=0°, φ=0° → P(0)=100%, P(1)=0%
        /// </summary>
        private void BtnEstado0_Click(object sender, RoutedEventArgs e)
        {
            if (SliderTheta != null && SliderPhi != null)
            {
                AnimarSliders(0, 0);
            }
        }

        /// <summary>
        /// Define o estado para |1⟩ (polo sul)
        /// θ=180°, φ=0° → P(0)=0%, P(1)=100%
        /// </summary>
        private void BtnEstado1_Click(object sender, RoutedEventArgs e)
        {
            if (SliderTheta != null && SliderPhi != null)
            {
                AnimarSliders(180, 0);
            }
        }

        /// <summary>
        /// Define o estado para |+⟩ (equador - superposição máxima)
        /// θ=90°, φ=0° → P(0)=50%, P(1)=50%
        /// Estado: |+⟩ = (|0⟩ + |1⟩)/√2
        /// 
        /// Este é o estado de SUPERPOSIÇÃO PERFEITA!
        /// O qubit está literalmente 50% em |0⟩ e 50% em |1⟩
        /// </summary>
        private void BtnEstadoPlus_Click(object sender, RoutedEventArgs e)
        {
            if (SliderTheta != null && SliderPhi != null)
            {
                AnimarSliders(90, 0);
            }
        }

        /// <summary>
        /// Anima os sliders para valores específicos
        /// </summary>
        /// 
        private void AnimarSliders(double theta, double phi)
        {
            var animacaoTheta = new DoubleAnimation
            {
                To = theta,
                Duration = TimeSpan.FromSeconds(0.6),
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseInOut }
            };

            var animacaoPhi = new DoubleAnimation
            {
                To = phi,
                Duration = TimeSpan.FromSeconds(0.6),
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseInOut }
            };

            SliderTheta.BeginAnimation(RangeBase.ValueProperty, animacaoTheta);
            SliderPhi.BeginAnimation(RangeBase.ValueProperty, animacaoPhi);
        }

        #endregion
    }
}