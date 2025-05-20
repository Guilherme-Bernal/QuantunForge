using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.Defaults;
using Quantum_Forge.Services;

namespace Quantun_Forge.src.views.ComputacaoQuantica
{
    public partial class EmaranhamentoControl : UserControl
    {
        private ObservableValue valor00 = new ObservableValue(0);
        private ObservableValue valor11 = new ObservableValue(0);

        public ISeries[] Series { get; set; }

        public EmaranhamentoControl()
        {
            InitializeComponent();
            ConfigurarGrafico();
        }

        private void ConfigurarGrafico()
        {
            Series = new ISeries[]
            {
                new ColumnSeries<ObservableValue>
                {
                    Name = "|00⟩",
                    Values = new ObservableValue[] { valor00 }
                },
                new ColumnSeries<ObservableValue>
                {
                    Name = "|11⟩",
                    Values = new ObservableValue[] { valor11 }
                }
            };

            graficoEmaranhamento.Series = Series;
            graficoEmaranhamento.XAxes = new[] { new Axis { Labels = new[] { "" } } };
            graficoEmaranhamento.YAxes = new[] { new Axis { MinLimit = 0 } };
        }

        private async void ExecutarSimulacao_Click(object sender, RoutedEventArgs e)
        {
            string resultado = await QuantumSimulatorService.ExecutarEmaranhamentoAsync();

            if (resultado == "00")
            {
                valor00.Value++;
                txtResultado.Text = "Resultado: |00⟩ (qubits correlacionados)";
            }
            else if (resultado == "11")
            {
                valor11.Value++;
                txtResultado.Text = "Resultado: |11⟩ (qubits correlacionados)";
            }
            else
            {
                txtResultado.Text = $"Resultado inesperado: |{resultado}⟩";
            }
        }

        private async void ExecutarMultiplasSimulacoes_Click(object sender, RoutedEventArgs e)
        {
            valor00.Value = 0;
            valor11.Value = 0;

            for (int i = 0; i < 100; i++)
            {
                string resultado = await QuantumSimulatorService.ExecutarEmaranhamentoAsync();
                if (resultado == "00") valor00.Value++;
                else if (resultado == "11") valor11.Value++;
            }

            txtResultado.Text = $"Após 100 execuções:\n|00⟩ = {valor00.Value} vezes\n|11⟩ = {valor11.Value} vezes";
        }
    }
}
