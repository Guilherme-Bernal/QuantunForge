using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace Quantun_Forge.src.views
{
    public partial class VonNeumannControl : UserControl
    {
        private List<int> memoria = new List<int>();
        private Random rng = new Random();

        public VonNeumannControl()
        {
            InitializeComponent();
            InicializarMemoria();
        }

        private void InicializarMemoria()
        {
            memoria.Clear();
            lstMemoria.Items.Clear();

            // Inicializa 5 posições da memória com valores aleatórios
            for (int i = 0; i < 5; i++)
            {
                int valor = rng.Next(1, 10);
                memoria.Add(valor);
                lstMemoria.Items.Add($"[M{i}] = {valor}");
            }

            Log("🔄 Memória inicializada com 5 posições.");
            lblControle.Text = "Pronto para buscar instruções.";
            lblUla.Text = "Aguardando operação.";
            lblSaida.Text = "";
        }

        private void ExecutarInstrucao_Click(object sender, RoutedEventArgs e)
        {
            if (!int.TryParse(txtInput.Text, out int entrada))
            {
                MessageBox.Show("Digite um número inteiro válido na entrada.", "Entrada inválida", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            Log("📥 Entrada recebida: " + entrada);

            // Etapa 1: Unidade de Controle busca instrução da memória
            int instrucao = memoria[0];
            lblControle.Text = $"Instrução buscada: {instrucao}";
            Log("🧠 Unidade de Controle buscou instrução: " + instrucao);

            // Etapa 2: ULA executa soma com M[1]
            int valorMem = memoria[1];
            int resultado = entrada + valorMem;
            lblUla.Text = $"Soma: {entrada} + {valorMem} = {resultado}";
            Log($"➕ ULA irá somar entrada ({entrada}) com M[1] = {valorMem}");

            // Etapa 3: Armazena resultado na memória
            memoria[4] = resultado;
            AtualizarMemoriaVisual();

            // Etapa 4: Mostra saída
            lblSaida.Text = $"Resultado: {resultado}";
            AnimarElemento(lblSaida);

            Log($"💾 Resultado armazenado em M[4]: {resultado}");
            Log("📤 Resultado exibido com sucesso.");
        }

        private void AtualizarMemoriaVisual()
        {
            lstMemoria.Items.Clear();
            for (int i = 0; i < memoria.Count; i++)
            {
                lstMemoria.Items.Add($"[M{i}] = {memoria[i]}");
            }
        }

        private void Log(string mensagem)
        {
            lstLog.Items.Add(mensagem);
            lstLog.ScrollIntoView(mensagem);
        }

        private void AnimarElemento(UIElement elemento)
        {
            var fade = new DoubleAnimation
            {
                From = 0,
                To = 1,
                Duration = TimeSpan.FromSeconds(0.5)
            };
            elemento.BeginAnimation(OpacityProperty, fade);
        }
    }
}
