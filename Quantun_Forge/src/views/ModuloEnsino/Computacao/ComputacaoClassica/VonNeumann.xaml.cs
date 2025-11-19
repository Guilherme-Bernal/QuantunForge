using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using Quantun_Forge.src.views.ModuloEnsino.Computacao.ComputacaoClassica.Testes;

namespace Quantun_Forge.src.views.ModuloEnsino.Computacao.ComputacaoClassica
{
    /// <summary>
    /// Lógica interna para VonNeumann.xaml
    /// </summary>
    public partial class VonNeumann : Window
    {
        // Estado do simulador
        private int currentStep = 0;
        private int cycleCount = 0;
        private List<SimulationStep> steps;

        // Classe para representar um passo da simulação
        private class SimulationStep
        {
            public string StepName { get; set; }
            public string StepDescription { get; set; }
            public Color StepColor { get; set; }
            public string Instruction { get; set; }
        }

        public VonNeumann()
        {
            InitializeComponent();
            InitializeSimulation();
        }

        /// <summary>
        /// Inicializa os passos da simulação
        /// </summary>
        private void InitializeSimulation()
        {
            steps = new List<SimulationStep>
            {
                new SimulationStep
                {
                    StepName = "1. FETCH",
                    StepDescription = "Buscando instrução da memória usando o Program Counter (PC)",
                    StepColor = Color.FromRgb(39, 174, 96), // Verde
                    Instruction = "LOAD R1, [100]"
                },
                new SimulationStep
                {
                    StepName = "2. DECODE",
                    StepDescription = "Decodificando a instrução: carregar valor da posição 100 para R1",
                    StepColor = Color.FromRgb(243, 156, 18), // Laranja
                    Instruction = "LOAD R1, [100]"
                },
                new SimulationStep
                {
                    StepName = "3. EXECUTE",
                    StepDescription = "Executando: acessando memória na posição 100",
                    StepColor = Color.FromRgb(52, 152, 219), // Azul
                    Instruction = "LOAD R1, [100]"
                },
                new SimulationStep
                {
                    StepName = "4. STORE",
                    StepDescription = "Armazenando o valor lido no registrador R1",
                    StepColor = Color.FromRgb(233, 30, 99), // Rosa
                    Instruction = "LOAD R1, [100]"
                },
                // Segunda instrução
                new SimulationStep
                {
                    StepName = "1. FETCH",
                    StepDescription = "Buscando próxima instrução da memória",
                    StepColor = Color.FromRgb(39, 174, 96),
                    Instruction = "LOAD R2, [101]"
                },
                new SimulationStep
                {
                    StepName = "2. DECODE",
                    StepDescription = "Decodificando: carregar valor da posição 101 para R2",
                    StepColor = Color.FromRgb(243, 156, 18),
                    Instruction = "LOAD R2, [101]"
                },
                new SimulationStep
                {
                    StepName = "3. EXECUTE",
                    StepDescription = "Executando: acessando memória na posição 101",
                    StepColor = Color.FromRgb(52, 152, 219),
                    Instruction = "LOAD R2, [101]"
                },
                new SimulationStep
                {
                    StepName = "4. STORE",
                    StepDescription = "Armazenando o valor lido no registrador R2",
                    StepColor = Color.FromRgb(233, 30, 99),
                    Instruction = "LOAD R2, [101]"
                },
                // Terceira instrução
                new SimulationStep
                {
                    StepName = "1. FETCH",
                    StepDescription = "Buscando instrução de soma da memória",
                    StepColor = Color.FromRgb(39, 174, 96),
                    Instruction = "ADD R3, R1, R2"
                },
                new SimulationStep
                {
                    StepName = "2. DECODE",
                    StepDescription = "Decodificando: somar R1 e R2, resultado em R3",
                    StepColor = Color.FromRgb(243, 156, 18),
                    Instruction = "ADD R3, R1, R2"
                },
                new SimulationStep
                {
                    StepName = "3. EXECUTE",
                    StepDescription = "Executando na ULA: R3 = R1 + R2",
                    StepColor = Color.FromRgb(52, 152, 219),
                    Instruction = "ADD R3, R1, R2"
                },
                new SimulationStep
                {
                    StepName = "4. STORE",
                    StepDescription = "Armazenando resultado da soma no registrador R3",
                    StepColor = Color.FromRgb(233, 30, 99),
                    Instruction = "ADD R3, R1, R2"
                },
                // Quarta instrução
                new SimulationStep
                {
                    StepName = "1. FETCH",
                    StepDescription = "Buscando instrução de armazenamento",
                    StepColor = Color.FromRgb(39, 174, 96),
                    Instruction = "STORE [102], R3"
                },
                new SimulationStep
                {
                    StepName = "2. DECODE",
                    StepDescription = "Decodificando: armazenar R3 na posição 102",
                    StepColor = Color.FromRgb(243, 156, 18),
                    Instruction = "STORE [102], R3"
                },
                new SimulationStep
                {
                    StepName = "3. EXECUTE",
                    StepDescription = "Executando: gravando valor de R3 na memória",
                    StepColor = Color.FromRgb(52, 152, 219),
                    Instruction = "STORE [102], R3"
                },
                new SimulationStep
                {
                    StepName = "4. STORE",
                    StepDescription = "Operação completa! Resultado salvo na posição 102",
                    StepColor = Color.FromRgb(233, 30, 99),
                    Instruction = "STORE [102], R3"
                }
            };

            UpdateSimulationDisplay();
        }

        /// <summary>
        /// Atualiza a exibição do simulador
        /// </summary>
        private void UpdateSimulationDisplay()
        {
            if (currentStep >= steps.Count)
            {
                // Simulação completa
                StepTitle.Text = "✅ PROGRAMA COMPLETO";
                StepDescription.Text = "Todas as instruções foram executadas com sucesso!";
                StepPanel.Background = new SolidColorBrush(Color.FromRgb(212, 239, 223));
                StepPanel.BorderBrush = new SolidColorBrush(Color.FromRgb(39, 174, 96));
                StepTitle.Foreground = new SolidColorBrush(Color.FromRgb(39, 174, 96));
                StepButton.Content = "🔄 REINICIAR";
                return;
            }

            var step = steps[currentStep];

            CurrentInstruction.Text = step.Instruction;
            StepTitle.Text = step.StepName;
            StepDescription.Text = step.StepDescription;

            // Define cores do painel de passo
            var lightColor = LightenColor(step.StepColor, 0.9f);
            StepPanel.Background = new SolidColorBrush(lightColor);
            StepPanel.BorderBrush = new SolidColorBrush(step.StepColor);
            StepTitle.Foreground = new SolidColorBrush(step.StepColor);

            // Anima os componentes baseado no passo
            AnimateComponents(step.StepName);

            // Atualiza contador de ciclos (a cada 4 passos)
            if (currentStep > 0 && currentStep % 4 == 0)
            {
                cycleCount++;
                CycleCounter.Text = cycleCount.ToString();
                AnimateCounterUpdate();
            }
        }

        /// <summary>
        /// Clareia uma cor
        /// </summary>
        private Color LightenColor(Color color, float factor)
        {
            return Color.FromRgb(
                (byte)(color.R + (255 - color.R) * factor),
                (byte)(color.G + (255 - color.G) * factor),
                (byte)(color.B + (255 - color.B) * factor)
            );
        }

        /// <summary>
        /// Anima componentes baseado no passo atual
        /// </summary>
        private void AnimateComponents(string stepName)
        {
            // Reset de todos os componentes
            ResetComponentAnimation(CPUBox);
            ResetComponentAnimation(MemoryBox);
            ResetComponentAnimation(InputBox);
            ResetComponentAnimation(OutputBox);

            // Anima baseado no passo
            if (stepName.Contains("FETCH") || stepName.Contains("DECODE") || stepName.Contains("EXECUTE"))
            {
                HighlightComponent(CPUBox);

                if (stepName.Contains("FETCH"))
                {
                    HighlightComponent(MemoryBox);
                }
            }
            else if (stepName.Contains("STORE"))
            {
                HighlightComponent(MemoryBox);
            }
        }

        /// <summary>
        /// Destaca um componente
        /// </summary>
        private void HighlightComponent(Border component)
        {
            var animation = new DoubleAnimation
            {
                From = 1.0,
                To = 1.1,
                Duration = TimeSpan.FromMilliseconds(300),
                AutoReverse = true,
                RepeatBehavior = new RepeatBehavior(2)
            };

            var scaleTransform = new ScaleTransform(1.0, 1.0);
            component.RenderTransform = scaleTransform;
            component.RenderTransformOrigin = new Point(0.5, 0.5);

            scaleTransform.BeginAnimation(ScaleTransform.ScaleXProperty, animation);
            scaleTransform.BeginAnimation(ScaleTransform.ScaleYProperty, animation);

            // Muda a borda temporariamente
            var originalBrush = component.BorderBrush;
            component.BorderBrush = new SolidColorBrush(Color.FromRgb(155, 89, 182)); // Roxo

            var timer = new System.Windows.Threading.DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(1200)
            };
            timer.Tick += (s, e) =>
            {
                component.BorderBrush = originalBrush;
                timer.Stop();
            };
            timer.Start();
        }

        /// <summary>
        /// Reseta animação do componente
        /// </summary>
        private void ResetComponentAnimation(Border component)
        {
            component.RenderTransform = new ScaleTransform(1.0, 1.0);
        }

        /// <summary>
        /// Anima atualização do contador
        /// </summary>
        private void AnimateCounterUpdate()
        {
            var animation = new DoubleAnimation
            {
                From = 1.0,
                To = 1.3,
                Duration = TimeSpan.FromMilliseconds(200),
                AutoReverse = true
            };

            var scaleTransform = new ScaleTransform(1.0, 1.0);
            CycleCounter.RenderTransform = scaleTransform;
            CycleCounter.RenderTransformOrigin = new Point(0.5, 0.5);

            scaleTransform.BeginAnimation(ScaleTransform.ScaleXProperty, animation);
            scaleTransform.BeginAnimation(ScaleTransform.ScaleYProperty, animation);
        }

        /// <summary>
        /// Próximo passo da simulação
        /// </summary>
        private void NextStep_Click(object sender, RoutedEventArgs e)
        {
            if (currentStep >= steps.Count)
            {
                // Reinicia se já terminou
                ResetSimulation_Click(sender, e);
                return;
            }

            currentStep++;
            UpdateSimulationDisplay();
        }

        /// <summary>
        /// Reinicia a simulação
        /// </summary>
        private void ResetSimulation_Click(object sender, RoutedEventArgs e)
        {
            currentStep = 0;
            cycleCount = 0;
            CycleCounter.Text = "0";
            StepButton.Content = "▶ PRÓXIMO PASSO";
            UpdateSimulationDisplay();

            // Reset visual dos componentes
            ResetComponentAnimation(CPUBox);
            ResetComponentAnimation(MemoryBox);
            ResetComponentAnimation(InputBox);
            ResetComponentAnimation(OutputBox);
        }

        /// <summary>
        /// Mostra informações do componente ao passar o mouse
        /// </summary>
        private void Component_MouseEnter(object sender, MouseEventArgs e)
        {
            var border = sender as Border;
            if (border == null) return;

            string tag = border.Tag?.ToString();

            switch (tag)
            {
                case "CPU":
                    InfoTitle.Text = "CPU - Unidade Central de Processamento";
                    InfoDescription.Text = "O cérebro do computador. Contém a UC (Unidade de Controle) que coordena operações e a ULA (Unidade Lógica Aritmética) que realiza cálculos matemáticos e operações lógicas.";
                    InfoPanel.Background = new SolidColorBrush(Color.FromRgb(244, 236, 247));
                    InfoPanel.BorderBrush = new SolidColorBrush(Color.FromRgb(155, 89, 182));
                    InfoTitle.Foreground = new SolidColorBrush(Color.FromRgb(155, 89, 182));
                    break;

                case "Memory":
                    InfoTitle.Text = "MEMÓRIA - RAM (Random Access Memory)";
                    InfoDescription.Text = "Armazena temporariamente dados e instruções dos programas em execução. É volátil: perde todo o conteúdo quando o computador é desligado. Permite acesso rápido e aleatório a qualquer posição.";
                    InfoPanel.Background = new SolidColorBrush(Color.FromRgb(235, 245, 251));
                    InfoPanel.BorderBrush = new SolidColorBrush(Color.FromRgb(52, 152, 219));
                    InfoTitle.Foreground = new SolidColorBrush(Color.FromRgb(52, 152, 219));
                    break;

                case "Input":
                    InfoTitle.Text = "ENTRADA - Dispositivos de Input";
                    InfoDescription.Text = "Dispositivos que permitem ao usuário enviar dados para o computador: teclado, mouse, scanner, microfone, câmera, sensores, touchscreen, etc.";
                    InfoPanel.Background = new SolidColorBrush(Color.FromRgb(232, 245, 233));
                    InfoPanel.BorderBrush = new SolidColorBrush(Color.FromRgb(39, 174, 96));
                    InfoTitle.Foreground = new SolidColorBrush(Color.FromRgb(39, 174, 96));
                    break;

                case "Output":
                    InfoTitle.Text = "SAÍDA - Dispositivos de Output";
                    InfoDescription.Text = "Dispositivos que mostram resultados processados ao usuário: monitor, impressora, alto-falantes, LEDs, projetores, etc. Transformam dados digitais em formas perceptíveis aos humanos.";
                    InfoPanel.Background = new SolidColorBrush(Color.FromRgb(254, 245, 231));
                    InfoPanel.BorderBrush = new SolidColorBrush(Color.FromRgb(230, 126, 34));
                    InfoTitle.Foreground = new SolidColorBrush(Color.FromRgb(230, 126, 34));
                    break;
            }

            InfoPanel.Visibility = Visibility.Visible;

            // Anima entrada do painel
            var fadeIn = new DoubleAnimation
            {
                From = 0.0,
                To = 1.0,
                Duration = TimeSpan.FromMilliseconds(200)
            };
            InfoPanel.BeginAnimation(OpacityProperty, fadeIn);
        }

        /// <summary>
        /// Esconde informações ao sair com o mouse
        /// </summary>
        private void Component_MouseLeave(object sender, MouseEventArgs e)
        {
            var fadeOut = new DoubleAnimation
            {
                From = 1.0,
                To = 0.0,
                Duration = TimeSpan.FromMilliseconds(200)
            };

            fadeOut.Completed += (s, args) =>
            {
                InfoPanel.Visibility = Visibility.Collapsed;
            };

            InfoPanel.BeginAnimation(OpacityProperty, fadeOut);
        }

        /// <summary>
        /// Vai para exercícios práticos
        /// </summary>
        private void btnExePratica_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var testeWindow = new VonNeumannTeste();
                testeWindow.Show();
               
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Erro ao abrir exercícios: {ex.Message}",
                    "Erro",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );
            }
        }
    }
}
