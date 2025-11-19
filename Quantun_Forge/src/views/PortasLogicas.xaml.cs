using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace Quantun_Forge.src.views
{
    public partial class PortasLogicas : UserControl
    {
        // Estado do simulador AND
        private int inputA = 0;
        private int inputB = 0;
        private int outputAND = 0;
        private bool andExecuted = false;

        // Estado do simulador X
        private string quantumState = "|0⟩";
        private string quantumOutput = "|0⟩";
        private bool xExecuted = false;

        // Contador de perdas de informação
        private int count0 = 0;
        private int count1 = 0;

        public PortasLogicas()
        {
            InitializeComponent();
            Loaded += PortasLogicas_Loaded;
        }

        private void PortasLogicas_Loaded(object sender, RoutedEventArgs e)
        {
            // Inicialização
            UpdateANDVisual();
            UpdateQuantumVisual();
        }

        #region Simulador 1: Porta AND (Clássica)

        private void BtnAND_A0_Click(object sender, RoutedEventArgs e)
        {
            inputA = 0;
            UpdateANDVisual();
            andExecuted = false;
            BtnRevertAND.IsEnabled = false;
            TxtAND_RevertMsg.Text = "";
        }

        private void BtnAND_A1_Click(object sender, RoutedEventArgs e)
        {
            inputA = 1;
            UpdateANDVisual();
            andExecuted = false;
            BtnRevertAND.IsEnabled = false;
            TxtAND_RevertMsg.Text = "";
        }

        private void BtnAND_B0_Click(object sender, RoutedEventArgs e)
        {
            inputB = 0;
            UpdateANDVisual();
            andExecuted = false;
            BtnRevertAND.IsEnabled = false;
            TxtAND_RevertMsg.Text = "";
        }

        private void BtnAND_B1_Click(object sender, RoutedEventArgs e)
        {
            inputB = 1;
            UpdateANDVisual();
            andExecuted = false;
            BtnRevertAND.IsEnabled = false;
            TxtAND_RevertMsg.Text = "";
        }

        private void BtnExecuteAND_Click(object sender, RoutedEventArgs e)
        {
            // Calcular AND
            outputAND = (inputA == 1 && inputB == 1) ? 1 : 0;

            // Animar o fluxo
            AnimateANDFlow();

            andExecuted = true;
            BtnRevertAND.IsEnabled = true;
        }

        private void AnimateANDFlow()
        {
            if (FlowParticle == null) return;

            var storyboard = new Storyboard();

            // Fade in da partícula
            var fadeIn = new DoubleAnimation
            {
                From = 0,
                To = 1,
                Duration = TimeSpan.FromSeconds(0.3)
            };
            Storyboard.SetTarget(fadeIn, FlowParticle);
            Storyboard.SetTargetProperty(fadeIn, new PropertyPath(OpacityProperty));
            storyboard.Children.Add(fadeIn);

            // Mover partícula da entrada para a saída
            var moveX = new DoubleAnimation
            {
                From = 50,
                To = 280,
                Duration = TimeSpan.FromSeconds(1),
                BeginTime = TimeSpan.FromSeconds(0.3)
            };
            Storyboard.SetTarget(moveX, FlowParticle);
            Storyboard.SetTargetProperty(moveX, new PropertyPath("(Canvas.Left)"));
            storyboard.Children.Add(moveX);

            // Fade out
            var fadeOut = new DoubleAnimation
            {
                From = 1,
                To = 0,
                Duration = TimeSpan.FromSeconds(0.3),
                BeginTime = TimeSpan.FromSeconds(1.3)
            };
            Storyboard.SetTarget(fadeOut, FlowParticle);
            Storyboard.SetTargetProperty(fadeOut, new PropertyPath(OpacityProperty));
            storyboard.Children.Add(fadeOut);

            storyboard.Completed += (s, e) =>
            {
                UpdateANDOutput();
            };

            storyboard.Begin();
        }

        private void UpdateANDVisual()
        {
            if (TxtInputA == null || TxtInputB == null || InputA_Classical == null || InputB_Classical == null)
                return;

            TxtInputA.Text = inputA.ToString();
            TxtInputB.Text = inputB.ToString();

            // Atualizar cores das entradas
            InputA_Classical.Fill = inputA == 1 ?
                new SolidColorBrush(Color.FromRgb(231, 76, 60)) :
                new SolidColorBrush(Color.FromRgb(189, 195, 199));

            InputB_Classical.Fill = inputB == 1 ?
                new SolidColorBrush(Color.FromRgb(231, 76, 60)) :
                new SolidColorBrush(Color.FromRgb(189, 195, 199));
        }

        private void UpdateANDOutput()
        {
            if (TxtOutputClassical == null || Output_Classical == null || TxtAND_Status == null)
                return;

            TxtOutputClassical.Text = outputAND.ToString();

            Output_Classical.Fill = outputAND == 1 ?
                new SolidColorBrush(Color.FromRgb(39, 174, 96)) :
                new SolidColorBrush(Color.FromRgb(189, 195, 199));

            // Atualizar status
            string statusMsg = "";
            if (outputAND == 0)
            {
                statusMsg = "3 entradas diferentes → OUT = 0\n(0,0), (0,1), (1,0) todos geram 0!";
            }
            else
            {
                statusMsg = "Apenas (1,1) → OUT = 1";
            }
            TxtAND_Status.Text = statusMsg;
        }

        private void BtnRevertAND_Click(object sender, RoutedEventArgs e)
        {
            if (TxtAND_RevertMsg == null) return;

            if (!andExecuted)
            {
                TxtAND_RevertMsg.Text = "Execute a porta AND primeiro!";
                return;
            }

            // Tentar reverter
            if (outputAND == 0)
            {
                TxtAND_RevertMsg.Text = "❌ IMPOSSÍVEL! A saída '0' pode vir de (0,0), (0,1) ou (1,0).\nQual era a entrada original? Não há como saber!";
            }
            else
            {
                TxtAND_RevertMsg.Text = "✓ Possível reverter: se OUT=1, então entrada era (1,1).\nMas isso só funciona para este caso específico!";
            }

            // Animar mensagem
            var animation = new DoubleAnimation
            {
                From = 0,
                To = 1,
                Duration = TimeSpan.FromSeconds(0.5)
            };
            TxtAND_RevertMsg.BeginAnimation(OpacityProperty, animation);
        }

        #endregion

        #region Simulador 1: Porta X (Quântica)

        private void BtnX_State0_Click(object sender, RoutedEventArgs e)
        {
            quantumState = "|0⟩";
            UpdateQuantumVisual();
            xExecuted = false;
            BtnRevertX.IsEnabled = false;
            TxtX_RevertMsg.Text = "";
        }

        private void BtnX_State1_Click(object sender, RoutedEventArgs e)
        {
            quantumState = "|1⟩";
            UpdateQuantumVisual();
            xExecuted = false;
            BtnRevertX.IsEnabled = false;
            TxtX_RevertMsg.Text = "";
        }

        private void BtnX_StatePlus_Click(object sender, RoutedEventArgs e)
        {
            quantumState = "|+⟩";
            UpdateQuantumVisual();
            xExecuted = false;
            BtnRevertX.IsEnabled = false;
            TxtX_RevertMsg.Text = "";
        }

        private void BtnExecuteX_Click(object sender, RoutedEventArgs e)
        {
            // Aplicar porta X
            if (quantumState == "|0⟩")
                quantumOutput = "|1⟩";
            else if (quantumState == "|1⟩")
                quantumOutput = "|0⟩";
            else if (quantumState == "|+⟩")
                quantumOutput = "|−⟩";

            // Animar transformação quântica
            AnimateQuantumTransform();

            xExecuted = true;
            BtnRevertX.IsEnabled = true;
        }

        private void AnimateQuantumTransform()
        {
            if (QuantumParticle == null) return;

            var storyboard = new Storyboard();

            // Fade in da partícula
            var fadeIn = new DoubleAnimation
            {
                From = 0,
                To = 1,
                Duration = TimeSpan.FromSeconds(0.3)
            };
            Storyboard.SetTarget(fadeIn, QuantumParticle);
            Storyboard.SetTargetProperty(fadeIn, new PropertyPath(OpacityProperty));
            storyboard.Children.Add(fadeIn);

            // Mover partícula
            var moveX = new DoubleAnimation
            {
                From = 44,
                To = 284,
                Duration = TimeSpan.FromSeconds(1.2),
                BeginTime = TimeSpan.FromSeconds(0.3),
                EasingFunction = new CubicEase { EasingMode = EasingMode.EaseInOut }
            };
            Storyboard.SetTarget(moveX, QuantumParticle);
            Storyboard.SetTargetProperty(moveX, new PropertyPath("(Canvas.Left)"));
            storyboard.Children.Add(moveX);

            // Garantir que RenderTransform existe
            if (QuantumParticle.RenderTransform == null || !(QuantumParticle.RenderTransform is ScaleTransform))
            {
                QuantumParticle.RenderTransform = new ScaleTransform(1, 1);
                QuantumParticle.RenderTransformOrigin = new Point(0.5, 0.5);
            }

            // Pulsar durante a transformação
            var pulse = new DoubleAnimation
            {
                From = 1,
                To = 1.5,
                Duration = TimeSpan.FromSeconds(0.3),
                AutoReverse = true,
                RepeatBehavior = new RepeatBehavior(2),
                BeginTime = TimeSpan.FromSeconds(0.8)
            };
            Storyboard.SetTarget(pulse, QuantumParticle);
            Storyboard.SetTargetProperty(pulse, new PropertyPath("(UIElement.RenderTransform).(ScaleTransform.ScaleX)"));
            storyboard.Children.Add(pulse);

            var pulseY = new DoubleAnimation
            {
                From = 1,
                To = 1.5,
                Duration = TimeSpan.FromSeconds(0.3),
                AutoReverse = true,
                RepeatBehavior = new RepeatBehavior(2),
                BeginTime = TimeSpan.FromSeconds(0.8)
            };
            Storyboard.SetTarget(pulseY, QuantumParticle);
            Storyboard.SetTargetProperty(pulseY, new PropertyPath("(UIElement.RenderTransform).(ScaleTransform.ScaleY)"));
            storyboard.Children.Add(pulseY);

            // Fade out
            var fadeOut = new DoubleAnimation
            {
                From = 1,
                To = 0,
                Duration = TimeSpan.FromSeconds(0.3),
                BeginTime = TimeSpan.FromSeconds(1.5)
            };
            Storyboard.SetTarget(fadeOut, QuantumParticle);
            Storyboard.SetTargetProperty(fadeOut, new PropertyPath(OpacityProperty));
            storyboard.Children.Add(fadeOut);

            storyboard.Completed += (s, e) =>
            {
                UpdateQuantumOutput();
            };

            storyboard.Begin();
        }

        private void UpdateQuantumVisual()
        {
            if (TxtQuantumInput == null || TxtQuantumOutput == null) return;

            TxtQuantumInput.Text = quantumState;
            TxtQuantumOutput.Text = quantumState; // Ainda não transformou
        }

        private void UpdateQuantumOutput()
        {
            if (TxtQuantumOutput == null || TxtX_Status == null) return;

            TxtQuantumOutput.Text = quantumOutput;

            // Atualizar status
            TxtX_Status.Text = $"X|{quantumState.Trim('|', '⟩')}⟩ = {quantumOutput}\nX†X = I ✓ (inversa existe)";
        }

        private void BtnRevertX_Click(object sender, RoutedEventArgs e)
        {
            if (TxtX_RevertMsg == null) return;

            if (!xExecuted)
            {
                TxtX_RevertMsg.Text = "Execute a porta X primeiro!";
                return;
            }

            // Aplicar X† (que é igual a X para a porta X)
            string revertedState = "";
            if (quantumOutput == "|0⟩")
                revertedState = "|1⟩";
            else if (quantumOutput == "|1⟩")
                revertedState = "|0⟩";
            else if (quantumOutput == "|−⟩")
                revertedState = "|+⟩";

            // Verificar se voltou ao original
            if (revertedState == quantumState)
            {
                TxtX_RevertMsg.Text = $"✅ SUCESSO! X†({quantumOutput}) = {revertedState}\nVoltou exatamente ao estado original!";

                // Animar o retorno
                AnimateSuccessfulRevert();
            }
            else
            {
                TxtX_RevertMsg.Text = "Erro inesperado na reversão.";
            }

            // Animar mensagem
            var animation = new DoubleAnimation
            {
                From = 0,
                To = 1,
                Duration = TimeSpan.FromSeconds(0.5)
            };
            TxtX_RevertMsg.BeginAnimation(OpacityProperty, animation);
        }

        private void AnimateSuccessfulRevert()
        {
            if (QuantumParticle == null || TxtQuantumOutput == null) return;

            // Animar partícula voltando
            var storyboard = new Storyboard();

            var fadeIn = new DoubleAnimation
            {
                From = 0,
                To = 1,
                Duration = TimeSpan.FromSeconds(0.3)
            };
            Storyboard.SetTarget(fadeIn, QuantumParticle);
            Storyboard.SetTargetProperty(fadeIn, new PropertyPath(OpacityProperty));
            storyboard.Children.Add(fadeIn);

            var moveBack = new DoubleAnimation
            {
                From = 284,
                To = 44,
                Duration = TimeSpan.FromSeconds(1),
                BeginTime = TimeSpan.FromSeconds(0.3),
                EasingFunction = new CubicEase { EasingMode = EasingMode.EaseInOut }
            };
            Storyboard.SetTarget(moveBack, QuantumParticle);
            Storyboard.SetTargetProperty(moveBack, new PropertyPath("(Canvas.Left)"));
            storyboard.Children.Add(moveBack);

            var fadeOut = new DoubleAnimation
            {
                From = 1,
                To = 0,
                Duration = TimeSpan.FromSeconds(0.3),
                BeginTime = TimeSpan.FromSeconds(1.3)
            };
            Storyboard.SetTarget(fadeOut, QuantumParticle);
            Storyboard.SetTargetProperty(fadeOut, new PropertyPath(OpacityProperty));
            storyboard.Children.Add(fadeOut);

            storyboard.Completed += (s, e) =>
            {
                TxtQuantumOutput.Text = quantumState;
            };

            storyboard.Begin();
        }

        #endregion

        #region Simulador 2: Perda de Informação

        private void BtnTestInput_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not Button btn) return;
            if (btn.Tag == null) return;

            string input = btn.Tag.ToString() ?? "";
            if (input.Length != 2) return;

            // Parse input
            int a = int.Parse(input[0].ToString());
            int b = int.Parse(input[1].ToString());

            // Calcular AND
            int result = (a == 1 && b == 1) ? 1 : 0;

            // Atualizar contadores
            if (result == 0)
                count0++;
            else
                count1++;

            // Atualizar UI
            UpdateLossVisualization();

            // Destacar o botão clicado
            HighlightButton(btn);

            // Animar a saída correspondente
            AnimateOutputBox(result);
        }

        private void UpdateLossVisualization()
        {
            if (TxtCount0 == null || TxtCount1 == null || TxtLossAnalysis == null) return;

            TxtCount0.Text = $"{count0} entrada{(count0 != 1 ? "s" : "")} →";
            TxtCount1.Text = $"{count1} entrada{(count1 != 1 ? "s" : "")} →";

            // Atualizar análise
            string analysis = "📊 Análise de Perda:\n\n";

            if (count0 == 0 && count1 == 0)
            {
                analysis += "Clique nas entradas acima para ver como múltiplas combinações levam ao mesmo resultado.\n\n";
                analysis += "Quando você vê \"0\" na saída, não consegue saber qual foi a entrada original!";
            }
            else
            {
                analysis += $"Até agora: {count0} entrada(s) geraram 0 e {count1} entrada(s) geraram 1.\n\n";

                if (count0 >= 2)
                {
                    analysis += "⚠️ PERDA DE INFORMAÇÃO DETECTADA!\n";
                    analysis += "Múltiplas entradas diferentes produziram a mesma saída (0).\n";
                    analysis += "Isso torna a operação IRREVERSÍVEL - não dá para saber qual foi a entrada original apenas olhando a saída!";
                }
                else if (count0 == 1 && count1 == 1)
                {
                    analysis += "Continue testando as outras entradas para ver a perda de informação...";
                }
                else if (count0 == 3)
                {
                    analysis += "🔴 MÁXIMA PERDA DE INFORMAÇÃO!\n";
                    analysis += "3 entradas diferentes (0,0), (0,1) e (1,0) TODAS geraram 0.\n";
                    analysis += "Apenas (1,1) gera 1. A porta AND é claramente IRREVERSÍVEL!";
                }
            }

            TxtLossAnalysis.Text = analysis;
        }

        private void HighlightButton(Button btn)
        {
            if (BtnTest00 == null || BtnTest01 == null || BtnTest10 == null || BtnTest11 == null) return;

            // Reset outros botões
            BtnTest00.Opacity = 0.7;
            BtnTest01.Opacity = 0.7;
            BtnTest10.Opacity = 0.7;
            BtnTest11.Opacity = 0.7;

            // Destacar botão clicado
            btn.Opacity = 1;

            var animation = new DoubleAnimation
            {
                From = 1,
                To = 1.1,
                Duration = TimeSpan.FromSeconds(0.2),
                AutoReverse = true
            };

            btn.RenderTransform = new ScaleTransform(1, 1);
            btn.RenderTransformOrigin = new Point(0.5, 0.5);
            btn.RenderTransform.BeginAnimation(ScaleTransform.ScaleXProperty, animation);
            btn.RenderTransform.BeginAnimation(ScaleTransform.ScaleYProperty, animation);
        }

        private void AnimateOutputBox(int output)
        {
            if (Output0Box == null || Output1Box == null) return;

            Border box = output == 0 ? Output0Box : Output1Box;

            // Pulsar
            var storyboard = new Storyboard();

            var scaleX = new DoubleAnimation
            {
                From = 1,
                To = 1.15,
                Duration = TimeSpan.FromSeconds(0.3),
                AutoReverse = true
            };

            var scaleY = new DoubleAnimation
            {
                From = 1,
                To = 1.15,
                Duration = TimeSpan.FromSeconds(0.3),
                AutoReverse = true
            };

            box.RenderTransform = new ScaleTransform(1, 1);
            box.RenderTransformOrigin = new Point(0.5, 0.5);

            Storyboard.SetTarget(scaleX, box);
            Storyboard.SetTargetProperty(scaleX, new PropertyPath("(UIElement.RenderTransform).(ScaleTransform.ScaleX)"));
            storyboard.Children.Add(scaleX);

            Storyboard.SetTarget(scaleY, box);
            Storyboard.SetTargetProperty(scaleY, new PropertyPath("(UIElement.RenderTransform).(ScaleTransform.ScaleY)"));
            storyboard.Children.Add(scaleY);

            storyboard.Begin();

            // Efeito de brilho temporário
            var originalBrush = box.BorderBrush;
            box.BorderBrush = new SolidColorBrush(Color.FromRgb(255, 215, 0)); // Dourado

            var timer = new System.Windows.Threading.DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(0.6)
            };
            timer.Tick += (s, e) =>
            {
                box.BorderBrush = originalBrush;
                timer.Stop();
            };
            timer.Start();
        }

        #endregion
    }
}