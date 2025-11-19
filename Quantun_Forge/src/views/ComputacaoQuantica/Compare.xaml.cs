using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Quantun_Forge.src.views.ComputacaoQuantica
{
    public partial class Compare : UserControl
    {
        public Compare()
        {
            InitializeComponent();
        }

        // ----- SIMULAÇÃO TSP -----
        private async void btnSimular_Click(object sender, RoutedEventArgs e)
        {
            btnSimular.IsEnabled = false;
            btnSimular.Background = new SolidColorBrush(Color.FromRgb(53, 53, 128));
            btnSimular.Content = "Simulando...";
            Mouse.OverrideCursor = Cursors.Wait;

            pbClassico.Value = 0;
            pbQuantico.Value = 0;
            txtTempoClassico.Text = "0 anos";
            txtTempoQuantico.Text = "0 min";
            txtTempoClassico.Foreground = new SolidColorBrush(Color.FromRgb(255, 111, 97));
            txtTempoQuantico.Foreground = new SolidColorBrush(Color.FromRgb(97, 223, 255));

            // Simulação Clássica
            long tempoTotalAnos = 280_000_000_000;
            for (int i = 0; i <= 100; i++)
            {
                pbClassico.Value = i;
                txtTempoClassico.Text = $"{tempoTotalAnos * i / 100:N0} anos";
                await Task.Delay(16);
            }
            txtTempoClassico.Text = "❌ 280.000.000.000 anos (Impraticável!) 🦕🕰️";
            txtTempoClassico.Foreground = Brushes.IndianRed;

            await Task.Delay(500);

            // Simulação Quântica
            for (int i = 0; i <= 100; i++)
            {
                pbQuantico.Value = i;
                int minutos = (int)(i * 60.0 / 100);
                txtTempoQuantico.Text = $"{minutos} min";
                await Task.Delay(5);
            }
            txtTempoQuantico.Text = "✅ 1 hora (Solucionável!) ⚡🧬";
            txtTempoQuantico.Foreground = Brushes.LightGreen;

            btnSimular.IsEnabled = true;
            btnSimular.Background = Brushes.LimeGreen;
            btnSimular.Content = "Simular Novamente";
            Mouse.OverrideCursor = null;
        }

        // ----- BIT E QUBIT -----
        private bool bitState = false; // false = 0, true = 1
        private bool qubitSuperposicao = false;
        private string ultimoQubitMedido = "|0⟩";
        private bool qubitHadamard = false; // para indicar se está em superposição por Hadamard
        private Random rnd = new();

        // Bit clássico
        private void btnBit_Click(object sender, RoutedEventArgs e)
        {
            bitState = !bitState;
            btnBit.Content = bitState ? "1" : "0";
            btnBit.Foreground = bitState ? Brushes.Orange : Brushes.DodgerBlue;
            FlashButtonColor(btnBit, bitState ? Brushes.OrangeRed : Brushes.DeepSkyBlue);
            txtPortaInfo.Text = "";
        }

        // Superposição quântica (botão "Superpor")
        private void btnSuperposicao_Click(object sender, RoutedEventArgs e)
        {
            qubitSuperposicao = true;
            qubitHadamard = false;
            txtQubit.Text = "|0⟩ + |1⟩";
            txtQubit.Foreground = Brushes.Gold;
            txtQubitEstado.Text = "Em superposição! (0 e 1 ao mesmo tempo)";
            txtPortaInfo.Text = "";
            FlashTextColor(txtQubit, Brushes.Gold);
        }

        // Medição do qubit (botão "Medir")
        private void btnMedirQubit_Click(object sender, RoutedEventArgs e)
        {
            if (qubitSuperposicao || qubitHadamard)
            {
                bool medido = rnd.Next(2) == 1;
                ultimoQubitMedido = medido ? "|1⟩" : "|0⟩";
                txtQubit.Text = ultimoQubitMedido;
                txtQubit.Foreground = medido ? Brushes.LimeGreen : Brushes.Aqua;
                txtQubitEstado.Text = "Colapsou para " + (medido ? "1" : "0");
                qubitSuperposicao = false;
                qubitHadamard = false;
                FlashTextColor(txtQubit, Brushes.White);
                txtPortaInfo.Text = "A medição colapsou o qubit para 0 ou 1 (probabilístico).";
            }
            else
            {
                txtQubit.Text = ultimoQubitMedido;
                txtQubit.Foreground = txtQubit.Text == "|1⟩" ? Brushes.LimeGreen : Brushes.Aqua;
                txtQubitEstado.Text = "Estado puro (não está em superposição)";
                txtPortaInfo.Text = "";
            }
        }

        // Flip Bit (porta NOT)
        private void btnNotBit_Click(object sender, RoutedEventArgs e)
        {
            bitState = !bitState;
            btnBit.Content = bitState ? "1" : "0";
            btnBit.Foreground = bitState ? Brushes.Orange : Brushes.DodgerBlue;
            txtPortaInfo.Text = "Flip/NOT: O bit foi invertido!";
            FlashButtonColor(btnBit, bitState ? Brushes.OrangeRed : Brushes.DeepSkyBlue);
        }

        // Hadamard Qubit (superposição quântica)
        private void btnHadamardQubit_Click(object sender, RoutedEventArgs e)
        {
            qubitSuperposicao = false;
            qubitHadamard = true;
            txtQubit.Text = "H|0⟩ = (|0⟩ + |1⟩)/√2";
            txtQubit.Foreground = Brushes.Gold;
            txtQubitEstado.Text = "Superposição: Hadamard aplicada";
            txtPortaInfo.Text = "Hadamard: Qubit em superposição (probabilidades iguais de 0 e 1)";
            FlashTextColor(txtQubit, Brushes.Gold);
        }

        // Medir Ambos
        private void btnMedirAmbos_Click(object sender, RoutedEventArgs e)
        {
            // Mede bit (mantém valor fixo)
            int valorBit = bitState ? 1 : 0;
            btnBit.Content = valorBit.ToString();
            btnBit.Foreground = valorBit == 1 ? Brushes.Orange : Brushes.DodgerBlue;

            // Mede qubit (aleatório se está em superposição ou Hadamard)
            bool medidoQubit = false;
            if (qubitSuperposicao || qubitHadamard)
            {
                medidoQubit = rnd.Next(2) == 1;
                ultimoQubitMedido = medidoQubit ? "|1⟩" : "|0⟩";
                txtQubit.Text = ultimoQubitMedido;
                txtQubit.Foreground = medidoQubit ? Brushes.LimeGreen : Brushes.Aqua;
                txtQubitEstado.Text = "Colapsou para " + (medidoQubit ? "1" : "0");
                qubitSuperposicao = false;
                qubitHadamard = false;
            }
            else
            {
                medidoQubit = ultimoQubitMedido == "|1⟩";
                txtQubit.Text = ultimoQubitMedido;
                txtQubit.Foreground = medidoQubit ? Brushes.LimeGreen : Brushes.Aqua;
                txtQubitEstado.Text = "Estado puro (não está em superposição)";
            }

            // Feedback comparativo
            if (valorBit == (medidoQubit ? 1 : 0))
                txtComparacao.Text = "🎯 Ambos iguais! (Na quântica, isso é por acaso)";
            else
                txtComparacao.Text = "🔀 Resultados diferentes: a incerteza quântica aparece!";
        }

        // EFEITO VISUAL FLASH EM BOTÃO
        private async void FlashButtonColor(Button btn, Brush highlight)
        {
            var original = btn.Background;
            btn.Background = highlight;
            await Task.Delay(180);
            btn.Background = original;
        }
        // EFEITO VISUAL FLASH EM TEXTBLOCK
        private async void FlashTextColor(TextBlock txt, Brush highlight)
        {
            var original = txt.Foreground;
            txt.Foreground = highlight;
            await Task.Delay(180);
            txt.Foreground = original;
        }
    }
}
