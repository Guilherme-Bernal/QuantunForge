// CicloInstrucaoControl.xaml.cs
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace Quantun_Forge.src.views
{
    public partial class CicloInstrucaoControl : UserControl
    {
        private int etapaAtual = 0;
        private DispatcherTimer timer;
        private bool modoAutomatico = false;

        private readonly List<(string Nome, string Descricao)> etapas = new()
        {
            ("Fetch (Busca)", "O processador localiza e carrega a próxima instrução da memória principal para o registrador de instrução. (PC → MAR → MBR → IR)"),
            ("Decode (Decodificação)", "A unidade de controle interpreta a instrução e prepara os sinais necessários para sua execução."),
            ("Execute (Execução)", "A ALU realiza a operação solicitada, como somar valores ou comparar dados."),
            ("Store (Armazenamento)", "O resultado da instrução é gravado de volta na memória ou em registradores.")
        };

        public CicloInstrucaoControl()
        {
            InitializeComponent();
            AtualizarExibicao();
            InicializarTimer();
        }

        private void InicializarTimer()
        {
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(2);
            timer.Tick += (s, e) => AvancarEtapa();
        }

        private void AvancarEtapa_Click(object sender, RoutedEventArgs e)
        {
            if (!modoAutomatico)
                AvancarEtapa();
            else
                ToggleModoAutomatico();
        }

        private void AvancarEtapa()
        {
            etapaAtual = (etapaAtual + 1) % etapas.Count;
            AtualizarExibicao();
        }

        private void AtualizarExibicao()
        {
            var etapa = etapas[etapaAtual];
            lblEtapa.Text = etapa.Nome;
            lblDescricao.Text = etapa.Descricao;
            lstHistorico.Items.Insert(0, $"🌀 {DateTime.Now:HH:mm:ss} - {etapa.Nome}");

            // Atualiza visualização dos registradores simulados
            txtPC.Text = etapaAtual == 0 ? "002" : txtPC.Text;
            txtIR.Text = etapaAtual == 0 ? "ADD 02" : etapaAtual == 1 ? "Decodificando" : etapaAtual == 2 ? "Executando" : "Pronto";
            txtACC.Text = etapaAtual == 2 ? "15" : txtACC.Text;
        }

        private void ToggleModoAutomatico()
        {
            modoAutomatico = !modoAutomatico;
            if (modoAutomatico)
                timer.Start();
            else
                timer.Stop();
        }
    }
}
