using System.Windows;
using System.Windows.Controls;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.Defaults;
using LiveChartsCore.Measure;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;
using Quantum_Forge.Services;

namespace Quantun_Forge.src.views.ComputacaoQuantica
{
    public partial class EmaranhamentoControl : UserControl
    {
        // Esses valores serão vinculados às barras do gráfico
        private ObservableValue valor00 = new ObservableValue(0);
        private ObservableValue valor11 = new ObservableValue(0);

        // Série de dados para o chart
        public ISeries[] Series { get; set; }

        // Rótulos do eixo X (binding no XAML: Labels = "{Binding XLabels}")
        public string[] XLabels { get; set; } = new[] { "|00⟩", "|11⟩" };

        public EmaranhamentoControl()
        {
            InitializeComponent();

            // Define este UserControl como DataContext para que o XLabels funcione
            DataContext = this;

            // Configura série e eixos do gráfico
            ConfigurarGrafico();
        }

        private void ConfigurarGrafico()
        {
            // Cada ColumnSeries ligado a um ObservableValue
            Series = new ISeries[]
            {
                new ColumnSeries<ObservableValue>
                {
                    Name            = "|00⟩",
                    Values          = new ObservableValue[] { valor00 },
                    DataLabelsPaint = new SolidColorPaint(SKColors.White), // cor dos data labels
                    DataLabelsSize  = 16,                                   // fonte maior para melhor leitura
                    Stroke          = null                                  // sem contorno nas barras
                },
                new ColumnSeries<ObservableValue>
                {
                    Name            = "|11⟩",
                    Values          = new ObservableValue[] { valor11 },
                    DataLabelsPaint = new SolidColorPaint(SKColors.White),
                    DataLabelsSize  = 16,
                    Stroke          = null
                }
            };

            // Atribui a série ao gráfico
            graficoEmaranhamento.Series = Series;

            // Eixo X: será preenchido via binding em XAML (Labels="{Binding XLabels}")
            graficoEmaranhamento.XAxes = new[]
            {
                new Axis
                {
                    Name            = "",    // Sem título extra no eixo X
                    Labels          = XLabels,             // {"|00⟩","|11⟩"}
                    LabelsPaint     = new SolidColorPaint(SKColors.White),
                    TextSize        = 14,
                    LabelsRotation  = 0
                    // Não definimos Padding aqui; usaremos DrawMargin abaixo
                }
            };

            // Eixo Y: frequência, com título “Freq.”
            graficoEmaranhamento.YAxes = new[]
            {
                new Axis
                {
                    Name           = "Freq.",
                    NamePaint      = new SolidColorPaint(SKColors.White),
                    NameTextSize   = 16,
                    LabelsPaint    = new SolidColorPaint(SKColors.White),
                    TextSize       = 14,
                    MinLimit       = 0
                    // MaxLimit pode ser ajustado no code‐behind, se desejar
                }
            };

            // Ajusta o espaço interno onde as colunas são desenhadas usando Margin (não Padding)
            graficoEmaranhamento.DrawMargin = new LiveChartsCore.Measure.Margin(10, 10, 10, 10);
        }

        private async void ExecutarSimulacao_Click(object sender, RoutedEventArgs e)
        {
            // Executa um único experimento de emaranhamento (assíncrono)
            string resultado = await QuantumSimulatorService.ExecutarEmaranhamentoAsync();

            if (resultado == "00")
            {
                valor00.Value++; // incrementa a barra |00⟩
                txtResultado.Text = "Resultado: |00⟩ (qubits correlacionados)";
            }
            else if (resultado == "11")
            {
                valor11.Value++; // incrementa a barra |11⟩
                txtResultado.Text = "Resultado: |11⟩ (qubits correlacionados)";
            }
            else
            {
                txtResultado.Text = $"Resultado inesperado: |{resultado}⟩";
            }
        }

        private async void ExecutarMultiplasSimulacoes_Click(object sender, RoutedEventArgs e)
        {
            // Zera ambos os contadores antes das 100 simulações
            valor00.Value = 0;
            valor11.Value = 0;

            // Executa 100 vezes de forma assíncrona em sequência
            for (int i = 0; i < 100; i++)
            {
                string resultado = await QuantumSimulatorService.ExecutarEmaranhamentoAsync();
                if (resultado == "00") valor00.Value++;
                else if (resultado == "11") valor11.Value++;
            }

            // Exibe texto resumindo quantas vezes cada estado apareceu
            txtResultado.Text = $"Após 100 execuções:\n|00⟩ = {valor00.Value} vezes\n|11⟩ = {valor11.Value} vezes";
        }
    }
}
