// CicloInstrucaoControl.xaml.cs
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace Quantun_Forge.src.views
{
    public partial class CicloInstrucaoControl : UserControl
    {
        // Estados do simulador
        private int etapaAtual = -1; // -1 = não iniciado
        private int programCounter = 0;
        private int ciclosExecutados = 0;
        private int instrucoesProcessadas = 0;
        private int acessosMemoria = 0;
        private bool simulacaoAtiva = false;

        private DispatcherTimer timer = null!;

        // Instruções na memória (simplificado para demonstração)
        private readonly List<Instrucao> memoria = new()
        {
            new Instrucao(0x0000, "LOAD", "A", "Carrega valor de A no ACC"),
            new Instrucao(0x0001, "ADD", "B", "Soma valor de B ao ACC"),
            new Instrucao(0x0002, "STORE", "C", "Armazena ACC em C"),
            new Instrucao(0x0003, "SUB", "D", "Subtrai valor de D do ACC"),
            new Instrucao(0x0004, "JUMP", "0x0000", "Retorna ao início"),
        };

        private readonly Dictionary<string, int> variaveis = new()
        {
            { "A", 10 },
            { "B", 5 },
            { "C", 0 },
            { "D", 3 }
        };

        public CicloInstrucaoControl()
        {
            InitializeComponent();
            InicializarTimer();
            InicializarMemoria();
            ResetarSimulacao();
        }

        private void InicializarTimer()
        {
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(1000);
            timer.Tick += Timer_Tick;

            // Atualiza velocidade quando o slider muda
            sliderVelocidade.ValueChanged += (s, e) =>
            {
                timer.Interval = TimeSpan.FromMilliseconds(e.NewValue);
                txtVelocidade.Text = $"{(int)e.NewValue} ms";
            };
        }

        private void Timer_Tick(object? sender, EventArgs e)
        {
            if (simulacaoAtiva)
            {
                ProximoPasso();
            }
        }

        private void InicializarMemoria()
        {
            pnlMemoria.Children.Clear();

            foreach (var instr in memoria)
            {
                var cell = new Border
                {
                    Style = (Style)FindResource("MemoryCellStyle")
                };

                var stackPanel = new StackPanel();

                stackPanel.Children.Add(new TextBlock
                {
                    Text = $"0x{instr.Endereco:X4}",
                    FontSize = 10,
                    Foreground = new SolidColorBrush(Color.FromRgb(149, 165, 166)),
                    FontFamily = new FontFamily("Consolas")
                });

                stackPanel.Children.Add(new TextBlock
                {
                    Text = $"{instr.Operacao} {instr.Operando}",
                    FontWeight = FontWeights.Bold,
                    FontSize = 11,
                    Foreground = new SolidColorBrush(Color.FromRgb(44, 62, 80)),
                    FontFamily = new FontFamily("Consolas"),
                    Margin = new Thickness(0, 3, 0, 0)
                });

                cell.Child = stackPanel;
                pnlMemoria.Children.Add(cell);
            }
        }

        private void IniciarCiclo_Click(object sender, RoutedEventArgs e)
        {
            if (!simulacaoAtiva)
            {
                simulacaoAtiva = true;
                btnIniciar.IsEnabled = false;
                btnProximoPasso.IsEnabled = true;
                btnExecutarCompleto.IsEnabled = true;

                AdicionarLog("✅ Simulação iniciada");
                ProximoPasso();
            }
        }

        private void ProximoPasso_Click(object sender, RoutedEventArgs e)
        {
            ProximoPasso();
        }

        private void ExecutarCompleto_Click(object sender, RoutedEventArgs e)
        {
            if (!timer.IsEnabled)
            {
                timer.Start();
                btnExecutarCompleto.Content = "⏸️ Pausar";
                btnProximoPasso.IsEnabled = false;
                AdicionarLog("⚡ Execução automática iniciada");
            }
            else
            {
                timer.Stop();
                btnExecutarCompleto.Content = "⚡ Executar Tudo";
                btnProximoPasso.IsEnabled = true;
                AdicionarLog("⏸️ Execução automática pausada");
            }
        }

        private void Reset_Click(object sender, RoutedEventArgs e)
        {
            timer.Stop();
            ResetarSimulacao();
            AdicionarLog("🔄 Simulação resetada");
        }

        private void LimparLog_Click(object sender, RoutedEventArgs e)
        {
            lstLog.Items.Clear();
            AdicionarLog("🗑️ Log limpo");
        }

        private void ProximoPasso()
        {
            etapaAtual = (etapaAtual + 1) % 4;

            switch (etapaAtual)
            {
                case 0: // FETCH
                    ExecutarFetch();
                    break;
                case 1: // DECODE
                    ExecutarDecode();
                    break;
                case 2: // EXECUTE
                    ExecutarExecute();
                    break;
                case 3: // STORE
                    ExecutarStore();
                    ciclosExecutados++;
                    txtCiclosExecutados.Text = ciclosExecutados.ToString();
                    break;
            }

            AtualizarVisualizacaoEtapa();
        }

        private void ExecutarFetch()
        {
            if (programCounter >= memoria.Count)
            {
                programCounter = 0; // Loop infinito para demonstração
            }

            var instrucao = memoria[programCounter];

            txtMAR.Text = $"0x{instrucao.Endereco:X4}";
            txtMDR.Text = $"{instrucao.Operacao} {instrucao.Operando}";
            txtIR.Text = $"{instrucao.Operacao} {instrucao.Operando}";
            txtALU.Text = "IDLE";

            acessosMemoria++;
            txtAcessosMemoria.Text = acessosMemoria.ToString();

            txtEtapaAtual.Text = "FETCH: Buscando instrução da memória";
            txtDescricaoEtapa.Text = $"Lendo instrução do endereço 0x{instrucao.Endereco:X4}: {instrucao.Operacao} {instrucao.Operando}";

            AdicionarLog($"[FETCH] PC=0x{programCounter:X4} → MAR → Memória[0x{instrucao.Endereco:X4}] → IR");

            DestacaCelula(programCounter);
        }

        private void ExecutarDecode()
        {
            var instrucao = memoria[programCounter];

            txtEtapaAtual.Text = "DECODE: Decodificando instrução";
            txtDescricaoEtapa.Text = $"Interpretando: {instrucao.Operacao} {instrucao.Operando} - {instrucao.Descricao}";
            txtALU.Text = "PREPARING";

            AdicionarLog($"[DECODE] Operação: {instrucao.Operacao}, Operando: {instrucao.Operando}");
        }

        private void ExecutarExecute()
        {
            var instrucao = memoria[programCounter];
            txtALU.Text = "ACTIVE";

            int acumulador = int.Parse(txtACC.Text);
            int resultado = acumulador;

            switch (instrucao.Operacao)
            {
                case "LOAD":
                    if (variaveis.ContainsKey(instrucao.Operando))
                    {
                        resultado = variaveis[instrucao.Operando];
                        AdicionarLog($"[EXECUTE] LOAD: ACC ← {instrucao.Operando} ({resultado})");
                    }
                    break;

                case "ADD":
                    if (variaveis.ContainsKey(instrucao.Operando))
                    {
                        int valor = variaveis[instrucao.Operando];
                        resultado = acumulador + valor;
                        AdicionarLog($"[EXECUTE] ADD: ACC ({acumulador}) + {instrucao.Operando} ({valor}) = {resultado}");
                    }
                    break;

                case "SUB":
                    if (variaveis.ContainsKey(instrucao.Operando))
                    {
                        int valor = variaveis[instrucao.Operando];
                        resultado = acumulador - valor;
                        AdicionarLog($"[EXECUTE] SUB: ACC ({acumulador}) - {instrucao.Operando} ({valor}) = {resultado}");
                    }
                    break;

                case "STORE":
                    AdicionarLog($"[EXECUTE] STORE: Preparando para armazenar ACC ({acumulador}) em {instrucao.Operando}");
                    break;

                case "JUMP":
                    AdicionarLog($"[EXECUTE] JUMP: Preparando salto para {instrucao.Operando}");
                    break;
            }

            txtACC.Text = resultado.ToString();
            txtEtapaAtual.Text = "EXECUTE: Executando operação na ALU";
            txtDescricaoEtapa.Text = $"ALU processando: {instrucao.Operacao}";
        }

        private void ExecutarStore()
        {
            var instrucao = memoria[programCounter];
            txtALU.Text = "IDLE";

            int acumulador = int.Parse(txtACC.Text);

            switch (instrucao.Operacao)
            {
                case "STORE":
                    if (variaveis.ContainsKey(instrucao.Operando))
                    {
                        variaveis[instrucao.Operando] = acumulador;
                        acessosMemoria++;
                        txtAcessosMemoria.Text = acessosMemoria.ToString();
                        AdicionarLog($"[STORE] ACC ({acumulador}) → {instrucao.Operando}");
                    }
                    break;

                case "JUMP":
                    programCounter = -1; // Será incrementado para 0 no final
                    AdicionarLog($"[STORE] PC ← {instrucao.Operando}");
                    break;

                default:
                    AdicionarLog($"[STORE] Ciclo concluído");
                    break;
            }

            programCounter++;
            txtPC.Text = $"0x{programCounter:X4}";

            instrucoesProcessadas++;
            txtInstrucoesProcessadas.Text = instrucoesProcessadas.ToString();

            txtEtapaAtual.Text = "STORE: Armazenando resultado";
            txtDescricaoEtapa.Text = "Ciclo de instrução completo. PC incrementado.";

            txtMAR.Text = "----";
            txtMDR.Text = "----";
        }

        private void AtualizarVisualizacaoEtapa()
        {
            // Reset todas as bordas
            bordaFetch.Style = (Style)FindResource("RegisterStyle");
            bordaDecode.Style = (Style)FindResource("RegisterStyle");
            bordaExecute.Style = (Style)FindResource("RegisterStyle");
            bordaStore.Style = (Style)FindResource("RegisterStyle");

            // Destaca etapa atual
            switch (etapaAtual)
            {
                case 0:
                    bordaFetch.Style = (Style)FindResource("ActiveRegisterStyle");
                    break;
                case 1:
                    bordaDecode.Style = (Style)FindResource("ActiveRegisterStyle");
                    break;
                case 2:
                    bordaExecute.Style = (Style)FindResource("ActiveRegisterStyle");
                    break;
                case 3:
                    bordaStore.Style = (Style)FindResource("ActiveRegisterStyle");
                    break;
            }
        }

        private void DestacaCelula(int indice)
        {
            // Reset todas as células
            foreach (var child in pnlMemoria.Children)
            {
                if (child is Border border)
                {
                    border.Style = (Style)FindResource("MemoryCellStyle");
                }
            }

            // Destaca célula atual
            if (indice < pnlMemoria.Children.Count)
            {
                var celula = pnlMemoria.Children[indice] as Border;
                if (celula != null)
                {
                    celula.Background = new SolidColorBrush(Color.FromRgb(232, 248, 245));
                    celula.BorderBrush = new SolidColorBrush(Color.FromRgb(26, 188, 156));
                    celula.BorderThickness = new Thickness(2);
                }
            }
        }

        private void ResetarSimulacao()
        {
            etapaAtual = -1;
            programCounter = 0;
            ciclosExecutados = 0;
            instrucoesProcessadas = 0;
            acessosMemoria = 0;
            simulacaoAtiva = false;

            txtPC.Text = "0x0000";
            txtIR.Text = "----";
            txtACC.Text = "0";
            txtMAR.Text = "----";
            txtMDR.Text = "----";
            txtALU.Text = "IDLE";

            txtEtapaAtual.Text = "Ciclo não iniciado";
            txtDescricaoEtapa.Text = "Clique em 'Iniciar Ciclo' para começar a simulação";

            txtCiclosExecutados.Text = "0";
            txtInstrucoesProcessadas.Text = "0";
            txtAcessosMemoria.Text = "0";

            btnIniciar.IsEnabled = true;
            btnProximoPasso.IsEnabled = false;
            btnExecutarCompleto.IsEnabled = false;
            btnExecutarCompleto.Content = "⚡ Executar Tudo";

            // Reset variáveis
            variaveis["A"] = 10;
            variaveis["B"] = 5;
            variaveis["C"] = 0;
            variaveis["D"] = 3;

            AtualizarVisualizacaoEtapa();
            InicializarMemoria();
        }

        private void AdicionarLog(string mensagem)
        {
            string timestamp = DateTime.Now.ToString("HH:mm:ss.fff");
            lstLog.Items.Insert(0, $"[{timestamp}] {mensagem}");

            // Limita o log a 100 itens
            while (lstLog.Items.Count > 100)
            {
                lstLog.Items.RemoveAt(lstLog.Items.Count - 1);
            }
        }

        // Classe auxiliar para representar instruções
        private class Instrucao
        {
            public int Endereco { get; set; }
            public string Operacao { get; set; }
            public string Operando { get; set; }
            public string Descricao { get; set; }

            public Instrucao(int endereco, string operacao, string operando, string descricao)
            {
                Endereco = endereco;
                Operacao = operacao;
                Operando = operando;
                Descricao = descricao;
            }
        }
    }
}