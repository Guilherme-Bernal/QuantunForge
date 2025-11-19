using System.Windows;
using System.Windows.Controls;
using Quantun_Forge.src;
using Quantun_Forge.src.views;
using Quantun_Forge.src.views.ComputacaoQuantica;
using Quantun_Forge.src.views.FisicaClassica;
using Quantun_Forge.src.views.FisicaQuantica;
using Quantun_Forge.src.views.ModuloEnsino;

namespace Quantun_Forge
{
    public partial class MainWindow : Window
    {
        private Button? _botaoAtualmenteSelecionado;

        public MainWindow()
        {
            InitializeComponent();
            CarregarPaginaInicial();
        }

        #region Métodos Auxiliares

        /// <summary>
        /// Carrega a página inicial ao abrir o aplicativo
        /// </summary>
        private void CarregarPaginaInicial()
        {
            MainFrame.Content = new Introducao();
        }
        

        /// <summary>
        /// Marca visualmente o botão selecionado
        /// </summary>
        private void MarcarBotaoSelecionado(Button botao)
        {
            // Remove a seleção do botão anterior
            if (_botaoAtualmenteSelecionado != null)
            {
                _botaoAtualmenteSelecionado.Tag = null;
            }

            // Marca o novo botão como selecionado
            botao.Tag = "Selected";
            _botaoAtualmenteSelecionado = botao;
        }

        /// <summary>
        /// Exibe mensagem de funcionalidade em desenvolvimento
        /// </summary>
        private void ExibirEmDesenvolvimento(string nomeTela)
        {
            MessageBox.Show(
                $"Tela de {nomeTela} em desenvolvimento",
                "Em Desenvolvimento",
                MessageBoxButton.OK,
                MessageBoxImage.Information
            );
        }

        /// <summary>
        /// Navega para um conteúdo específico no frame principal
        /// </summary>
        private void NavegarPara(UIElement conteudo)
        {
            MainFrame.Content = conteudo;
        }

        #endregion

        #region Computação Clássica

        private void BtnLogicaBooleana_Click(object sender, RoutedEventArgs e)
        {
            MarcarBotaoSelecionado((Button)sender);
            MainFrame.Content = new LogicaBooleanaControl();
        }

        private void roadmap_Click(object sender, RoutedEventArgs e)
        {
            MarcarBotaoSelecionado((Button)sender);
            MainFrame.Content = new roadmap();
        }

        private void BtnVonNeumann_Click(object sender, RoutedEventArgs e)
        {
            MarcarBotaoSelecionado((Button)sender);
            MainFrame.Content = new VonNeumannControl();
        }

        private void BtnPortasLogicas_Click(object sender, RoutedEventArgs e)
        {
            MarcarBotaoSelecionado((Button)sender);
            MainFrame.Content = new PortasLogicasControl();
        }

        private void BtnSistemaBinario_Click(object sender, RoutedEventArgs e)
        {
            MarcarBotaoSelecionado((Button)sender);
            MainFrame.Content = new SistemaBinarioControl();
        }

        private void BtnAlu_Click(object sender, RoutedEventArgs e)
        {
            MarcarBotaoSelecionado((Button)sender);
            MainFrame.Content = new AluControl();
        }

        private void BtnCicloInstrucao_Click(object sender, RoutedEventArgs e)
        {
            MarcarBotaoSelecionado((Button)sender);
            MainFrame.Content = new CicloInstrucaoControl();
        }

        #endregion

        #region Física Clássica - Mecânica

        private void Btn_LeiNewton1_Click(object sender, RoutedEventArgs e)
        {
            MarcarBotaoSelecionado((Button)sender);
            MainFrame.Content = new LeiNewton1Control();
        }

        private void Btn_LeiNewton2_Click(object sender, RoutedEventArgs e)
        {
            MarcarBotaoSelecionado((Button)sender);
            MainFrame.Content = new LeiNewton2Control();
        }

        private void Btn_LeiNewton3_Click(object sender, RoutedEventArgs e)
        {
            MarcarBotaoSelecionado((Button)sender);
            MainFrame.Content = new LeiNewton3Control();
        }

        private void Btn_Gravitacao_Click(object sender, RoutedEventArgs e)
        {
            MarcarBotaoSelecionado((Button)sender);
            MainFrame.Content = new GravidadeUniversalControl();
        }

        private void Btn_MomentoAngular_Click(object sender, RoutedEventArgs e)
        {
            MarcarBotaoSelecionado((Button)sender);
            ExibirEmDesenvolvimento("Momento Angular");
        }

        #endregion

        #region Física Clássica - Termodinâmica

        private void Btn_Termo1_Click(object sender, RoutedEventArgs e)
        {
            MarcarBotaoSelecionado((Button)sender);
            MainFrame.Content = new Termo1Control();
        }

        private void Btn_Termo2_Click(object sender, RoutedEventArgs e)
        {
            MarcarBotaoSelecionado((Button)sender);
            MainFrame.Content = new Termo2Control();
        }

        private void Btn_Termo3_Click(object sender, RoutedEventArgs e)
        {
            MarcarBotaoSelecionado((Button)sender);
            MainFrame.Content = new Termo3Control();
        }

        #endregion

        #region Física Clássica - Eletromagnetismo

        private void Btn_Coulomb_Click(object sender, RoutedEventArgs e)
        {
            MarcarBotaoSelecionado((Button)sender);
            MainFrame.Content = new CoulombControl();
        }

        private void Btn_Maxwell_Click(object sender, RoutedEventArgs e)
        {
            MarcarBotaoSelecionado((Button)sender);
            MainFrame.Content = new MaxwellControl();
        }

        private void Btn_CampoEM_Click(object sender, RoutedEventArgs e)
        {
            MarcarBotaoSelecionado((Button)sender);
            ExibirEmDesenvolvimento("Campo Eletromagnético");
        }

        #endregion

        #region Física Clássica - Hidrodinâmica

        private void Btn_Pascal_Click(object sender, RoutedEventArgs e)
        {
            MarcarBotaoSelecionado((Button)sender);
            MainFrame.Content = new PascalControl();
        }

        private void Btn_Arquimedes_Click(object sender, RoutedEventArgs e)
        {
            MarcarBotaoSelecionado((Button)sender);
            MainFrame.Content = new Arquimedes();
        }

        private void Btn_Torricelli_Click(object sender, RoutedEventArgs e)
        {
            MarcarBotaoSelecionado((Button)sender);
            MainFrame.Content = new TorricelliControl();
        }

        #endregion

        #region Física Clássica - Oscilações e Ondas

        private void Btn_Oscilador_Click(object sender, RoutedEventArgs e)
        {
            MarcarBotaoSelecionado((Button)sender);
            MainFrame.Content = new OsciladorHarmonicoControl();
        }

        private void Btn_OndasMecanicas_Click(object sender, RoutedEventArgs e)
        {
            MarcarBotaoSelecionado((Button)sender);
            ExibirEmDesenvolvimento("Ondas Mecânicas");
        }

        private void Btn_Interferencia_Click(object sender, RoutedEventArgs e)
        {
            MarcarBotaoSelecionado((Button)sender);
            ExibirEmDesenvolvimento("Interferência");
        }

        private void Btn_Reflexao_Click(object sender, RoutedEventArgs e)
        {
            MarcarBotaoSelecionado((Button)sender);
            ExibirEmDesenvolvimento("Reflexão");
        }

        private void Btn_Difracao_Click(object sender, RoutedEventArgs e)
        {
            MarcarBotaoSelecionado((Button)sender);
            ExibirEmDesenvolvimento("Difração");
        }

        private void Btn_DuplaFenda_Click(object sender, RoutedEventArgs e)
        {
            MarcarBotaoSelecionado((Button)sender);
            ExibirEmDesenvolvimento("Experimento da Dupla Fenda");
        }

        #endregion

        #region Física Quântica - Fundamentos

        private void Btn_QuantizacaoEnergia_Click(object sender, RoutedEventArgs e)
        {
            MarcarBotaoSelecionado((Button)sender);
            MainFrame.Content = new QuantizaçãoEnergia();
        }

        private void Btn_Dualidade_Click(object sender, RoutedEventArgs e)
        {
            MarcarBotaoSelecionado((Button)sender);
            MainFrame.Content = new DualidadeOndaParticula();
        }

        private void Btn_EspectroAto_Click(object sender, RoutedEventArgs e)
        {
            MarcarBotaoSelecionado((Button)sender);
            MainFrame.Content = new EspectroAto();
        }

        private void Btn_CorpoNegro_Click(object sender, RoutedEventArgs e)
        {
            MarcarBotaoSelecionado((Button)sender);
            MainFrame.Content = new CorpoNegro();
        }

        #endregion

        #region Física Quântica - Princípios Fundamentais

        private void Btn_Heisenberg_Click(object sender, RoutedEventArgs e)
        {
            MarcarBotaoSelecionado((Button)sender);
            MainFrame.Content = new Heisenberg();
        }

        private void Btn_Schrodinger1D_Click(object sender, RoutedEventArgs e)
        {
            MarcarBotaoSelecionado((Button)sender);
            MainFrame.Content = new Schrodinger1D();
        }

        #endregion

        #region Computação Quântica

        private void BtnSuperposicao_Click(object sender, RoutedEventArgs e)
        {
            MarcarBotaoSelecionado((Button)sender);
            MainFrame.Content = new SuperposicaoControl();
        }

        private void BtnEmaranhamento_Click(object sender, RoutedEventArgs e)
        {
            MarcarBotaoSelecionado((Button)sender);
            MainFrame.Content = new EmaranhamentoControl();
        }

        private void Btncls_Click(object sender, RoutedEventArgs e)
        {
            MarcarBotaoSelecionado((Button)sender);
            MainFrame.Content = new Compare();
        }

        private void Btn01Quantun(object sender, RoutedEventArgs e)
        {
            MarcarBotaoSelecionado((Button)sender);
            MainFrame.Content = new _01quantun();
        }

        #endregion

        #region Módulo de Exercícios

        private void BtnExerciciosClassica_Click(object sender, RoutedEventArgs e)
        {
            MarcarBotaoSelecionado((Button)sender);
            MainFrame.Content = new ExerciciosClassicaControl();
        }

        private void BtnExerciciosQuantica_Click(object sender, RoutedEventArgs e)
        {
            MarcarBotaoSelecionado((Button)sender);
            MainFrame.Content = new ExerciciosQuanticaControl();
        }

        private void BtnExerciciosFisica_Click(object sender, RoutedEventArgs e)
        {
            MarcarBotaoSelecionado((Button)sender);
            MainFrame.Content = new ExerciciosFisicaControl();
        }

        private void BtnExerciciosFisicaQuantica_Click(object sender, RoutedEventArgs e)
        {
            MarcarBotaoSelecionado((Button)sender);
            MainFrame.Content = new ExerciciosFisicaQuanticaControl();
        }

        private void BtnExerciciosMatematica_Click(object sender, RoutedEventArgs e)
        {
            MarcarBotaoSelecionado((Button)sender);
            MainFrame.Content = new ExerciciosMatematicaControl();
        }

        #endregion

        #region Módulo Híbrido - Transição Física Clássica → Quântica

        private void Btn_LimitesFisicaClassica_Click(object sender, RoutedEventArgs e)
        {
            MarcarBotaoSelecionado((Button)sender);
            MainFrame.Content = new LimitesClassicos();
        }

        private void Btn_DualidadeComparativo_Click(object sender, RoutedEventArgs e)
        {
            MarcarBotaoSelecionado((Button)sender);
            MainFrame.Content = new DualidadeOndaFisicaCQ();
        }

        private void Btn_QuantizacaoTransicao_Click(object sender, RoutedEventArgs e)
        {
            MarcarBotaoSelecionado((Button)sender);
            MainFrame.Content = new Quantizacao();
        }

        private void Btn_DeterminismoProbabilismo_Click(object sender, RoutedEventArgs e)
        {
            MarcarBotaoSelecionado((Button)sender);
            MainFrame.Content = new DeterminismoaoProbabilismo();
        }

        private void Btn_CampoQuantizado_Click(object sender, RoutedEventArgs e)
        {
            MarcarBotaoSelecionado((Button)sender);
            MainFrame.Content = new ContinuoQuantizacao();
        }

        #endregion

        #region Módulo Híbrido - Computação Clássica → Quântica

        private void Btn_BitVsQubit_Click(object sender, RoutedEventArgs e)
        {
            MarcarBotaoSelecionado((Button)sender);
            MainFrame.Content = new bitquibit();
        }

        private void Btn_PortasHibridas_Click(object sender, RoutedEventArgs e)
        {
            MarcarBotaoSelecionado((Button)sender);
            MainFrame.Content = new PortasLogicas();
        }

        private void Btn_ParalelismoHibrido_Click(object sender, RoutedEventArgs e)
        {
            MarcarBotaoSelecionado((Button)sender);
            MainFrame.Content = new PortasLogicas();
        }

        private void Btn_ErroDecoerencia_Click(object sender, RoutedEventArgs e)
        {
            MarcarBotaoSelecionado((Button)sender);
            MainFrame.Content = new EMD();
        }

        private void Btn_EntropiaComparativo_Click(object sender, RoutedEventArgs e)
        {
            MarcarBotaoSelecionado((Button)sender);
            MainFrame.Content = new Entropia();
        }

        #endregion

        #region Navegação Principal

        private void BtnIntroducao_Click(object sender, RoutedEventArgs e)
        {
            MarcarBotaoSelecionado((Button)sender);
            MainFrame.Content = new Introducao();
        }

        private void BtnSobre_Click(object sender, RoutedEventArgs e)
        {
            MarcarBotaoSelecionado((Button)sender);

            MessageBox.Show(
                "Quantum Forge v2.0.0\n\n" +
                "Plataforma educacional para ensino de Computação Quântica.\n\n" +
                "© 2025 Quantum Forge - Todos os direitos reservados",
                "Sobre o Quantum Forge",
                MessageBoxButton.OK,
                MessageBoxImage.Information
            );
        }

        #endregion
    }
}