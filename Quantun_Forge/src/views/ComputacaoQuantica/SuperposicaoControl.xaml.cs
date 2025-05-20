using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.WPF;
using LiveChartsCore.Kernel.Sketches;
using Quantum_Forge.Services;
using LiveChartsCore.Defaults;

namespace Quantun_Forge.src.views.ComputacaoQuantica
{
    public partial class SuperposicaoControl : UserControl
    {
        private ObservableValue valorZero = new ObservableValue(0);
        private ObservableValue valorUm = new ObservableValue(0);

        public ISeries[] Series { get; set; }

        public SuperposicaoControl()
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
                    Name = "|0⟩",
                    Values = new ObservableValue[] { valorZero }
                },
                new ColumnSeries<ObservableValue>
                {
                    Name = "|1⟩",
                    Values = new ObservableValue[] { valorUm }
                }
            };

            graficoResultado.Series = Series;
            graficoResultado.XAxes = new Axis[] { new Axis { Labels = new[] { "" } } };
            graficoResultado.YAxes = new Axis[] { new Axis { MinLimit = 0 } };
        }

        private async void ExecutarSimulacao_Click(object sender, RoutedEventArgs e)
        {
            int resultado = await QuantumSimulatorService.SimularQubitAsync();

            if (resultado == 0)
            {
                valorZero.Value++;
                txtResultado.Text = "Resultado da medição: |0⟩";
                txtEstado.Text = "Estado colapsado: |0⟩";
            }
            else
            {
                valorUm.Value++;
                txtResultado.Text = "Resultado da medição: |1⟩";
                txtEstado.Text = "Estado colapsado: |1⟩";
            }
        }

        private async void ExecutarMultiplasSimulacoes_Click(object sender, RoutedEventArgs e)
        {
            valorZero.Value = 0;
            valorUm.Value = 0;

            txtEstado.Text = "Estado reiniciado: superposição";

            for (int i = 0; i < 100; i++)
            {
                int resultado = await QuantumSimulatorService.SimularQubitAsync();
                if (resultado == 0) valorZero.Value++;
                else valorUm.Value++;
            }

            txtResultado.Text = $"Após 100 simulações:\n|0⟩ = {valorZero.Value} vezes\n|1⟩ = {valorUm.Value} vezes";
        }
    }
}
