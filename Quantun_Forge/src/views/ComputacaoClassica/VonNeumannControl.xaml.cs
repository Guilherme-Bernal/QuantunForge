// VonNeumannControl.xaml.cs
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace Quantun_Forge.src.views
{
    public partial class VonNeumannControl : UserControl
    {
        private ObservableCollection<CelulaMemoria> memoria = new ObservableCollection<CelulaMemoria>();
        private int programCounter = 0;
        private int instrucoesExecutadas = 0;
        private int acessosMemoria = 0;
        private int operacoesALU = 0;

        public VonNeumannControl()
        {
            InitializeComponent();
            InicializarSimulador();
        }

        private void InicializarSimulador()
        {
            // Inicializa memória com 8 posições
            memoria.Clear();
            for (int i = 0; i < 8; i++)
            {
                memoria.Add(new CelulaMemoria
                {
                    Endereco = $"M[{i}]",
                    Valor = 0
                });
            }

            // Define alguns valores iniciais
            memoria[1].Valor = 5;  // Valor para operações
            memoria[2].Valor = 3;  // Valor auxiliar

            lstMemoria.ItemsSource = memoria;

            // Reset registradores
            txtPC.Text = "0x00";
            txtMAR.Text = "--";
            txtMDR.Text = "--";

            // Reset estatísticas
            instrucoesExecutadas = 0;
            acessosMemoria = 0;
            operacoesALU = 0;
            AtualizarEstatisticas();

            // Reset componentes
            txtControle.Text = "Aguardando instrução";
            txtALU.Text = "Nenhuma";
            txtOutput.Text = "--";

            Log("✅ Simulador inicializado");
            Log("💾 Memória: 8 posições disponíveis");
            Log("📌 M[1] = 5 (valor inicial)");
            Log("📌 M[2] = 3 (valor auxiliar)");
        }

        // Operação ADD
        private void ExecutarADD_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidarEntrada(out int entrada)) return;

            Log("═══════════════════════════════");
            Log("▶️ Executando instrução: ADD");
            Log($"📥 INPUT = {entrada}");

            // Ciclo de instrução
            Fetch("ADD");
            Decode("Soma INPUT + M[1] → M[4]");

            // Lê da memória
            int valorMemoria = LerMemoria(1);

            // ALU executa soma
            int resultado = ExecutarOperacaoALU("ADD", entrada, valorMemoria);

            // Armazena resultado
            EscreverMemoria(4, resultado);

            // Incrementa PC
            programCounter++;
            txtPC.Text = $"0x{programCounter:X2}";

            instrucoesExecutadas++;
            AtualizarEstatisticas();

            Log($"✅ Instrução ADD concluída");
            Log("═══════════════════════════════");

            ResetarDestaques();
        }

        // Operação SUB
        private void ExecutarSUB_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidarEntrada(out int entrada)) return;

            Log("═══════════════════════════════");
            Log("▶️ Executando instrução: SUB");
            Log($"📥 INPUT = {entrada}");

            Fetch("SUB");
            Decode("Subtrai INPUT - M[1] → M[4]");

            int valorMemoria = LerMemoria(1);
            int resultado = ExecutarOperacaoALU("SUB", entrada, valorMemoria);

            EscreverMemoria(4, resultado);

            programCounter++;
            txtPC.Text = $"0x{programCounter:X2}";

            instrucoesExecutadas++;
            AtualizarEstatisticas();

            Log($"✅ Instrução SUB concluída");
            Log("═══════════════════════════════");

            ResetarDestaques();
        }

        // Operação MUL
        private void ExecutarMUL_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidarEntrada(out int entrada)) return;

            Log("═══════════════════════════════");
            Log("▶️ Executando instrução: MUL");
            Log($"📥 INPUT = {entrada}");

            Fetch("MUL");
            Decode("Multiplica INPUT × M[1] → M[4]");

            int valorMemoria = LerMemoria(1);
            int resultado = ExecutarOperacaoALU("MUL", entrada, valorMemoria);

            EscreverMemoria(4, resultado);

            programCounter++;
            txtPC.Text = $"0x{programCounter:X2}";

            instrucoesExecutadas++;
            AtualizarEstatisticas();

            Log($"✅ Instrução MUL concluída");
            Log("═══════════════════════════════");

            ResetarDestaques();
        }

        // Operação LOAD
        private void ExecutarLOAD_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidarEntrada(out int entrada)) return;

            Log("═══════════════════════════════");
            Log("▶️ Executando instrução: LOAD");
            Log($"📥 INPUT = {entrada}");

            Fetch("LOAD");
            Decode("Carrega INPUT → M[0]");

            DestacarComponente(borderIO);
            DestacarComponente(borderMemoria);

            EscreverMemoria(0, entrada);

            programCounter++;
            txtPC.Text = $"0x{programCounter:X2}";

            txtControle.Text = "Instrução LOAD concluída";
            txtALU.Text = "Nenhuma (transferência direta)";

            instrucoesExecutadas++;
            AtualizarEstatisticas();

            Log($"✅ Valor {entrada} carregado em M[0]");
            Log("═══════════════════════════════");

            ResetarDestaques();
        }

        // Operação STORE
        private void ExecutarSTORE_Click(object sender, RoutedEventArgs e)
        {
            Log("═══════════════════════════════");
            Log("▶️ Executando instrução: STORE");

            Fetch("STORE");
            Decode("Envia M[4] → OUTPUT");

            int valor = LerMemoria(4);

            DestacarComponente(borderMemoria);
            DestacarComponente(borderIO);

            txtOutput.Text = valor.ToString();
            AnimarElemento(txtOutput);

            programCounter++;
            txtPC.Text = $"0x{programCounter:X2}";

            txtControle.Text = "Instrução STORE concluída";
            txtALU.Text = "Nenhuma (transferência direta)";

            instrucoesExecutadas++;
            AtualizarEstatisticas();

            Log($"📤 OUTPUT = {valor}");
            Log($"✅ Instrução STORE concluída");
            Log("═══════════════════════════════");

            ResetarDestaques();
        }

        // Métodos auxiliares do ciclo de instrução
        private void Fetch(string instrucao)
        {
            DestacarComponente(borderControle);
            txtControle.Text = $"FETCH: Buscando instrução {instrucao}";
            txtMAR.Text = $"0x{programCounter:X2}";
            txtMDR.Text = instrucao;
            Log($"🔍 FETCH: Instrução {instrucao} buscada do PC=0x{programCounter:X2}");
        }

        private void Decode(string descricao)
        {
            DestacarComponente(borderControle);
            txtControle.Text = $"DECODE: {descricao}";
            Log($"🔧 DECODE: {descricao}");
        }

        private int LerMemoria(int endereco)
        {
            DestacarComponente(borderMemoria);

            int valor = memoria[endereco].Valor;
            txtMAR.Text = $"M[{endereco}]";
            txtMDR.Text = valor.ToString();

            acessosMemoria++;
            Log($"📖 Leitura: M[{endereco}] = {valor}");

            return valor;
        }

        private void EscreverMemoria(int endereco, int valor)
        {
            DestacarComponente(borderMemoria);

            memoria[endereco].Valor = valor;
            txtMAR.Text = $"M[{endereco}]";
            txtMDR.Text = valor.ToString();

            acessosMemoria++;
            Log($"✍️ Escrita: M[{endereco}] ← {valor}");
        }

        private int ExecutarOperacaoALU(string operacao, int a, int b)
        {
            DestacarComponente(borderALU);

            int resultado = operacao switch
            {
                "ADD" => a + b,
                "SUB" => a - b,
                "MUL" => a * b,
                _ => 0
            };

            string simbolo = operacao switch
            {
                "ADD" => "+",
                "SUB" => "-",
                "MUL" => "×",
                _ => "?"
            };

            txtALU.Text = $"{a} {simbolo} {b} = {resultado}";
            operacoesALU++;

            Log($"🔢 ALU: {a} {simbolo} {b} = {resultado}");

            return resultado;
        }

        private bool ValidarEntrada(out int valor)
        {
            if (!int.TryParse(txtInput.Text, out valor))
            {
                MessageBox.Show("Digite um valor numérico válido no campo INPUT!", "Entrada Inválida", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            return true;
        }

        private void DestacarComponente(Border componente)
        {
            componente.Style = (Style)FindResource("ActiveComponentStyle");
        }

        private void ResetarDestaques()
        {
            var timer = new System.Windows.Threading.DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(2)
            };

            timer.Tick += (s, e) =>
            {
                borderControle.Style = (Style)FindResource("ComponentBorderStyle");
                borderALU.Style = (Style)FindResource("ComponentBorderStyle");
                borderMemoria.Style = (Style)FindResource("ComponentBorderStyle");
                borderIO.Style = (Style)FindResource("ComponentBorderStyle");
                timer.Stop();
            };

            timer.Start();
        }

        private void AtualizarEstatisticas()
        {
            txtInstrucoesExecutadas.Text = instrucoesExecutadas.ToString();
            txtAcessosMemoria.Text = acessosMemoria.ToString();
            txtOperacoesALU.Text = operacoesALU.ToString();
        }

        private void Resetar_Click(object sender, RoutedEventArgs e)
        {
            var resultado = MessageBox.Show(
                "Deseja resetar o simulador?\nTodos os dados serão perdidos.",
                "Confirmar Reset",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question
            );

            if (resultado == MessageBoxResult.Yes)
            {
                programCounter = 0;
                txtInput.Text = "";
                lstLog.Items.Clear();
                InicializarSimulador();
                Log("🔄 Simulador resetado com sucesso");
            }
        }

        private void LimparLog_Click(object sender, RoutedEventArgs e)
        {
            lstLog.Items.Clear();
            Log("🗑️ Log limpo");
        }

        private void Log(string mensagem)
        {
            string timestamp = DateTime.Now.ToString("HH:mm:ss");
            lstLog.Items.Insert(0, $"[{timestamp}] {mensagem}");

            // Limita o log a 100 itens
            while (lstLog.Items.Count > 100)
            {
                lstLog.Items.RemoveAt(lstLog.Items.Count - 1);
            }
        }

        private void AnimarElemento(UIElement elemento)
        {
            var fade = new DoubleAnimation
            {
                From = 0.3,
                To = 1.0,
                Duration = TimeSpan.FromSeconds(0.5),
                AutoReverse = false
            };
            elemento.BeginAnimation(OpacityProperty, fade);

            var scale = new DoubleAnimation
            {
                From = 1.0,
                To = 1.2,
                Duration = TimeSpan.FromSeconds(0.2),
                AutoReverse = true
            };

            var scaleTransform = new ScaleTransform(1.0, 1.0);
            elemento.RenderTransform = scaleTransform;
            elemento.RenderTransformOrigin = new Point(0.5, 0.5);
            scaleTransform.BeginAnimation(ScaleTransform.ScaleXProperty, scale);
            scaleTransform.BeginAnimation(ScaleTransform.ScaleYProperty, scale);
        }

        // Classe auxiliar para células de memória
        public class CelulaMemoria
        {
            public string Endereco { get; set; } = "";
            public int Valor { get; set; }
        }
    }
}