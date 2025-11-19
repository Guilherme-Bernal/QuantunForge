using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Quantun_Forge.src.views
{
    public partial class AluControl : UserControl
    {
        // Elementos do XAML que precisam ser acessados (adicione ao XAML se não existirem)
        private TextBlock? txtAluStatus;
        private Border? borderAluCore;

        public AluControl()
        {
            InitializeComponent();
            InicializarElementos();
        }

        /// <summary>
        /// Inicializa referências aos elementos visuais do XAML
        /// </summary>
        private void InicializarElementos()
        {
            // Tenta encontrar os elementos no XAML
            // Se não existirem, o código funcionará normalmente sem os efeitos visuais extras
            txtAluStatus = FindName("txtAluStatus") as TextBlock;
            borderAluCore = FindName("borderAluCore") as Border;
        }

        private async void Calcular_Click(object sender, RoutedEventArgs e)
        {
            // Validação de entradas
            if (!double.TryParse(txtOperando1.Text, out double op1) ||
                !double.TryParse(txtOperando2.Text, out double op2))
            {
                await ExibirErro("❌ ERRO: Entradas inválidas! Insira números válidos.");
                return;
            }

            // Obtém a operação selecionada
            string? operacaoCompleta = (cmbOperacao.SelectedItem as ComboBoxItem)?.Content.ToString();
            if (string.IsNullOrEmpty(operacaoCompleta))
            {
                await ExibirErro("❌ ERRO: Selecione uma operação!");
                return;
            }

            // Remove emojis e identifica a operação
            string operacao = RemoverEmojis(operacaoCompleta!);

            double resultado = 0;
            string simbolo = "";
            string tipoOperacao = "";

            // Atualiza registradores de entrada
            lblR1.Text = op1.ToString();
            lblR2.Text = op2.ToString();

            // Verifica se é operação lógica
            bool isOperacaoLogica = operacao == "AND" || operacao == "OR" || operacao == "XOR";

            if (isOperacaoLogica && (op1 % 1 != 0 || op2 % 1 != 0))
            {
                await ExibirErro("⚠️ ATENÇÃO: Operações lógicas exigem números inteiros!");
                return;
            }

            // FEEDBACK VISUAL: Inicia processamento
            await IniciarProcessamento(operacao);

            // Executa a operação
            switch (operacao)
            {
                case "Soma":
                    resultado = op1 + op2;
                    simbolo = "+";
                    tipoOperacao = "ADD";
                    break;
                case "Subtração":
                    resultado = op1 - op2;
                    simbolo = "-";
                    tipoOperacao = "SUB";
                    break;
                case "Multiplicação":
                    resultado = op1 * op2;
                    simbolo = "×";
                    tipoOperacao = "MUL";
                    break;
                case "Divisão":
                    if (op2 == 0)
                    {
                        await ExibirErro("❌ ERRO: Divisão por zero não permitida!");
                        return;
                    }
                    resultado = op1 / op2;
                    simbolo = "÷";
                    tipoOperacao = "DIV";
                    break;
                case "AND":
                    resultado = (int)op1 & (int)op2;
                    simbolo = "&";
                    tipoOperacao = "AND";
                    break;
                case "OR":
                    resultado = (int)op1 | (int)op2;
                    simbolo = "|";
                    tipoOperacao = "OR";
                    break;
                case "XOR":
                    resultado = (int)op1 ^ (int)op2;
                    simbolo = "⊕";
                    tipoOperacao = "XOR";
                    break;
                default:
                    await ExibirErro("❌ ERRO: Operação inválida!");
                    return;
            }

            // Simula tempo de processamento (opcional, para efeito visual)
            await Task.Delay(300);

            // Valida resultado
            if (double.IsNaN(resultado) || double.IsInfinity(resultado))
            {
                await ExibirErro("❌ ERRO: Resultado inválido!");
                return;
            }

            // Formata o resultado
            string resultadoFormatado = resultado % 1 == 0 ?
                ((int)resultado).ToString() :
                resultado.ToString("F2");

            // FEEDBACK VISUAL: Operação concluída
            await FinalizarProcessamento(tipoOperacao);

            // Exibe resultado com sucesso
            ExibirSucesso(resultadoFormatado, tipoOperacao);

            // Adiciona ao histórico com formatação melhorada
            AdicionarAoHistorico(op1, op2, resultado, simbolo, tipoOperacao, resultadoFormatado);
        }

        /// <summary>
        /// Adiciona operação ao histórico com formatação visual aprimorada
        /// </summary>
        private void AdicionarAoHistorico(double op1, double op2, double resultado, string simbolo, string tipoOperacao, string resultadoFormatado)
        {
            string timestamp = DateTime.Now.ToString("HH:mm:ss");

            // Cabeçalho da operação com destaque
            string cabecalho = $"╔══════════════════════════════════════════════════════════╗";
            lstHistorico.Items.Insert(0, cabecalho);

            // Linha de operação
            string operacaoStr = $"║ [{timestamp}] {tipoOperacao}: {op1} {simbolo} {op2} = {resultadoFormatado}";
            // Preenche com espaços até 58 caracteres + ║
            operacaoStr = operacaoStr.PadRight(59) + "║";
            lstHistorico.Items.Insert(1, operacaoStr);

            // Mostra binário apenas para inteiros
            if (op1 % 1 == 0 && op2 % 1 == 0 && resultado % 1 == 0)
            {
                int valorOp1 = (int)op1;
                int valorOp2 = (int)op2;
                int valorResultado = (int)resultado;

                // Determina o número de bits necessários
                int maxBits = Math.Max(8, Math.Max(
                    GetBitsNecessarios(valorOp1),
                    Math.Max(GetBitsNecessarios(valorOp2), GetBitsNecessarios(valorResultado))
                ));

                string bin1 = Convert.ToString(valorOp1, 2).PadLeft(maxBits, '0');
                string bin2 = Convert.ToString(valorOp2, 2).PadLeft(maxBits, '0');
                string binR = Convert.ToString(valorResultado, 2).PadLeft(maxBits, '0');

                lblBinario.Text = $"Binário ({maxBits} bits): {binR}";

                // Separador
                lstHistorico.Items.Insert(2, "║──────────────────────────────────────────────────────────║");

                // Linha binária
                string binarioStr = $"║ BIN ({maxBits} bits): {bin1} {simbolo} {bin2} = {binR}";
                binarioStr = binarioStr.PadRight(59) + "║";
                lstHistorico.Items.Insert(3, binarioStr);

                // Rodapé
                lstHistorico.Items.Insert(4, "╚══════════════════════════════════════════════════════════╝");
                lstHistorico.Items.Insert(5, ""); // Linha em branco
            }
            else
            {
                lblBinario.Text = "Representação binária: N/A (apenas para inteiros)";

                // Rodapé
                lstHistorico.Items.Insert(2, "╚══════════════════════════════════════════════════════════╝");
                lstHistorico.Items.Insert(3, ""); // Linha em branco
            }

            // Limita o histórico a 200 itens
            while (lstHistorico.Items.Count > 200)
            {
                lstHistorico.Items.RemoveAt(lstHistorico.Items.Count - 1);
            }
        }

        /// <summary>
        /// Inicia feedback visual de processamento
        /// </summary>
        private async Task IniciarProcessamento(string operacao)
        {
            // Atualiza texto da ALU
            if (txtAluStatus != null)
            {
                string operacaoUpper = operacao.ToUpper();
                switch (operacao)
                {
                    case "Soma": operacaoUpper = "ADD"; break;
                    case "Subtração": operacaoUpper = "SUB"; break;
                    case "Multiplicação": operacaoUpper = "MUL"; break;
                    case "Divisão": operacaoUpper = "DIV"; break;
                }

                txtAluStatus.Text = $"⚡ Processando {operacaoUpper}...";
                txtAluStatus.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#F39C12"));
            }

            // Destaca a borda do processador
            if (borderAluCore != null)
            {
                borderAluCore.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#F39C12"));
                borderAluCore.BorderThickness = new Thickness(4);
            }

            // Força a atualização da interface
            Application.Current?.Dispatcher?.Invoke(() => { }, System.Windows.Threading.DispatcherPriority.Render);

            await Task.Delay(100); // Pequeno delay para feedback visual
        }

        /// <summary>
        /// Finaliza feedback visual de processamento
        /// </summary>
        private async Task FinalizarProcessamento(string tipoOperacao)
        {
            // Atualiza texto da ALU
            if (txtAluStatus != null)
            {
                txtAluStatus.Text = $"✓ Operação {tipoOperacao} concluída!";
                txtAluStatus.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#27AE60"));
            }

            // Restaura borda do processador (verde temporariamente)
            if (borderAluCore != null)
            {
                borderAluCore.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#27AE60"));
                borderAluCore.BorderThickness = new Thickness(4);
            }

            // Força a atualização da interface
            Application.Current?.Dispatcher?.Invoke(() => { }, System.Windows.Threading.DispatcherPriority.Render);

            await Task.Delay(1500); // Mantém verde por 1.5 segundos

            // Volta ao estado padrão
            if (txtAluStatus != null)
            {
                txtAluStatus.Text = "Aguardando execução...";
                txtAluStatus.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#7F8C8D"));
            }

            if (borderAluCore != null)
            {
                borderAluCore.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#5C6BC0"));
                borderAluCore.BorderThickness = new Thickness(3);
            }
        }

        /// <summary>
        /// Exibe mensagem de erro com feedback visual
        /// </summary>
        private async Task ExibirErro(string mensagem)
        {
            lblResultado.Text = mensagem;
            lblResultado.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#E74C3C"));
            lblBinario.Text = "";
            lblR1.Text = "0";
            lblR2.Text = "0";

            // Atualiza status da ALU
            if (txtAluStatus != null)
            {
                txtAluStatus.Text = "❌ Erro na operação";
                txtAluStatus.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#E74C3C"));
            }

            // Destaca erro na borda
            if (borderAluCore != null)
            {
                borderAluCore.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#E74C3C"));
                borderAluCore.BorderThickness = new Thickness(4);
            }

            await Task.Delay(2000); // Mantém erro visível por 2 segundos

            // Restaura estado
            if (borderAluCore != null)
            {
                borderAluCore.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#5C6BC0"));
                borderAluCore.BorderThickness = new Thickness(3);
            }

            if (txtAluStatus != null)
            {
                txtAluStatus.Text = "Aguardando execução...";
                txtAluStatus.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#7F8C8D"));
            }
        }

        /// <summary>
        /// Exibe resultado de sucesso
        /// </summary>
        private void ExibirSucesso(string resultado, string operacao)
        {
            lblResultado.Text = $"✓ {operacao}: {resultado}";
            lblResultado.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#16A085"));
        }

        /// <summary>
        /// Remove emojis e caracteres especiais do texto da operação
        /// </summary>
        private string RemoverEmojis(string texto)
        {
            // Remove espaços extras e converte para uppercase para comparação
            texto = texto.Trim().ToUpper();

            if (texto.Contains("SOMA") || texto.Contains("ADD")) return "Soma";
            if (texto.Contains("SUBTRAÇÃO") || texto.Contains("SUB")) return "Subtração";
            if (texto.Contains("MULTIPLICAÇÃO") || texto.Contains("MUL")) return "Multiplicação";
            if (texto.Contains("DIVISÃO") || texto.Contains("DIV")) return "Divisão";
            if (texto.Contains("AND")) return "AND";
            if (texto.Contains("OR") && !texto.Contains("XOR")) return "OR";
            if (texto.Contains("XOR")) return "XOR";

            return texto.Trim();
        }

        /// <summary>
        /// Calcula o número de bits necessários para representar um número
        /// </summary>
        private int GetBitsNecessarios(int valor)
        {
            if (valor == 0) return 1;

            // Considera números negativos
            if (valor < 0)
            {
                // Para negativos, usamos complemento de 2
                return GetBitsNecessarios(Math.Abs(valor)) + 1;
            }

            int bits = 0;
            while (valor > 0)
            {
                bits++;
                valor >>= 1;
            }

            return bits;
        }
    }
}