using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace Quantun_Forge.src.views.FisicaClassica
{
    public partial class Arquimedes : UserControl
    {
        private const double g = 9.8; // Gravidade (m/s²)
        private double densidadeFluido = 1000; // Densidade do fluido (kg/m³) - padrão: água

        public Arquimedes()
        {
            InitializeComponent();
            AtualizarVisualizacao();
        }

        private void Sliders_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            AtualizarVisualizacao();
        }

        private void AtualizarVisualizacao()
        {
            if (SliderMassaObjeto == null || SliderVolumeObjeto == null) return;

            double massa = SliderMassaObjeto.Value;
            double volume = SliderVolumeObjeto.Value;

            // Atualizar labels
            if (TxtMassaObjetoSlider != null)
                TxtMassaObjetoSlider.Text = $"{massa:F0} kg";

            if (TxtVolumeObjetoSlider != null)
                TxtVolumeObjetoSlider.Text = $"{volume:F2} m³";

            // Calcular e exibir densidade
            double densidadeObjeto = massa / volume;
            if (TxtDensidadeObjeto != null)
                TxtDensidadeObjeto.Text = $"{densidadeObjeto:F2} kg/m³";
        }

        private void BtnAnalisar_Click(object sender, RoutedEventArgs e)
        {
            double massa = SliderMassaObjeto.Value;
            double volume = SliderVolumeObjeto.Value;

            // Calcular peso
            double peso = massa * g;

            // Calcular empuxo
            double empuxo = densidadeFluido * volume * g;

            // Força resultante
            double forcaResultante = empuxo - peso;

            // Determinar situação
            string situacao;
            Color corResultado;
            string emoji;
            double novaPosicaoY;

            if (forcaResultante > 1)
            {
                situacao = "⬆️ FLUTUA";
                corResultado = Color.FromRgb(76, 175, 80);
                emoji = "⛵";
                novaPosicaoY = 120; // Sobe
            }
            else if (forcaResultante < -1)
            {
                situacao = "⬇️ AFUNDA";
                corResultado = Color.FromRgb(244, 67, 54);
                emoji = "⚓";
                novaPosicaoY = 280; // Desce
            }
            else
            {
                situacao = "⚖️ EQUILÍBRIO";
                corResultado = Color.FromRgb(255, 152, 0);
                emoji = "🐟";
                novaPosicaoY = 200; // Fica no meio
            }

            // Mostrar elementos visuais
            MostrarResultadoVisual(peso, empuxo, forcaResultante, situacao, corResultado, emoji);

            // Atualizar resultados
            AtualizarResultados(peso, empuxo, forcaResultante, situacao, massa, volume);

            // Animar objeto
            AnimarObjeto(novaPosicaoY);
        }

        private void MostrarResultadoVisual(double peso, double empuxo, double fResultante, string situacao, Color cor, string emoji)
        {
            // Setas de força
            if (SetaPeso != null) SetaPeso.Visibility = Visibility.Visible;
            if (PontaSetaPeso != null) PontaSetaPeso.Visibility = Visibility.Visible;
            if (LabelPesoBorder != null)
            {
                LabelPesoBorder.Visibility = Visibility.Visible;
                if (LabelPeso != null) LabelPeso.Text = $"P = {peso:F0} N";
            }

            if (SetaEmpuxo != null) SetaEmpuxo.Visibility = Visibility.Visible;
            if (PontaSetaEmpuxo != null) PontaSetaEmpuxo.Visibility = Visibility.Visible;
            if (LabelEmpuxoBorder != null)
            {
                LabelEmpuxoBorder.Visibility = Visibility.Visible;
                if (LabelEmpuxo != null) LabelEmpuxo.Text = $"E = {empuxo:F0} N";
            }

            // Volume deslocado
            if (VolumeDeslocado != null) VolumeDeslocado.Visibility = Visibility.Visible;
            if (LabelVolumeBorder != null) LabelVolumeBorder.Visibility = Visibility.Visible;

            // Resultado
            if (ResultadoBorder != null)
            {
                ResultadoBorder.Visibility = Visibility.Visible;
                ResultadoBorder.Background = new SolidColorBrush(cor);
                if (TxtResultadoVisual != null) TxtResultadoVisual.Text = situacao;
                if (TxtForcaResultante != null) TxtForcaResultante.Text = $"F_res = {fResultante:+0;-0} N";
            }

            // Info das forças
            if (InfoForcas != null)
            {
                InfoForcas.Visibility = Visibility.Visible;
                if (TxtPesoInfo != null) TxtPesoInfo.Text = $"Peso: {peso:F0} N ⬇️";
                if (TxtEmpuxoInfo != null) TxtEmpuxoInfo.Text = $"Empuxo: {empuxo:F0} N ⬆️";
                if (TxtResultanteInfo != null) TxtResultanteInfo.Text = $"Resultante: {fResultante:+0;-0} N";
            }

            // Emoji
            if (EmojiObjeto != null) EmojiObjeto.Text = emoji;
        }

        private void AtualizarResultados(double peso, double empuxo, double fRes, string situacao, double massa, double volume)
        {
            // Peso
            if (TxtPesoResultado != null) TxtPesoResultado.Text = $"P = {peso:F2} N";
            if (TxtComparacaoPeso != null) TxtComparacaoPeso.Text = $"Massa: {massa:F0} kg × g";

            // Empuxo
            if (TxtEmpuxoResultado != null) TxtEmpuxoResultado.Text = $"E = {empuxo:F2} N";
            if (TxtComparacaoEmpuxo != null) TxtComparacaoEmpuxo.Text = $"Fluido deslocado: {volume:F3} m³";

            // Conclusão
            if (TxtConclusao != null)
            {
                if (fRes > 1)
                    TxtConclusao.Text = $"{situacao} - O empuxo ({empuxo:F0} N) é maior que o peso ({peso:F0} N)!";
                else if (fRes < -1)
                    TxtConclusao.Text = $"{situacao} - O peso ({peso:F0} N) é maior que o empuxo ({empuxo:F0} N)!";
                else
                    TxtConclusao.Text = $"{situacao} - Empuxo e peso estão balanceados!";
            }

            // Passo a passo
            if (TxtPassoAPasso != null)
            {
                TxtPassoAPasso.Text = $"1️⃣ Dados:\n" +
                                     $"   Massa = {massa:F0} kg\n" +
                                     $"   Volume = {volume:F3} m³\n" +
                                     $"   Densidade fluido = {densidadeFluido:F0} kg/m³\n" +
                                     $"   g = {g} m/s²\n\n" +
                                     $"2️⃣ Calcular Peso:\n" +
                                     $"   P = m × g\n" +
                                     $"   P = {massa:F0} × {g}\n" +
                                     $"   P = {peso:F2} N\n\n" +
                                     $"3️⃣ Calcular Empuxo:\n" +
                                     $"   E = d_fluido × V × g\n" +
                                     $"   E = {densidadeFluido:F0} × {volume:F3} × {g}\n" +
                                     $"   E = {empuxo:F2} N\n\n" +
                                     $"4️⃣ Força Resultante:\n" +
                                     $"   F_res = E - P\n" +
                                     $"   F_res = {empuxo:F2} - {peso:F2}\n" +
                                     $"   F_res = {fRes:F2} N\n\n" +
                                     $"5️⃣ Conclusão:\n   {situacao}";
            }
        }

        private void AnimarObjeto(double novaPosY)
        {
            if (ObjetoSubmerso != null)
            {
                var anim = new DoubleAnimation
                {
                    To = novaPosY,
                    Duration = TimeSpan.FromSeconds(1),
                    EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseInOut }
                };
                ObjetoSubmerso.BeginAnimation(Canvas.TopProperty, anim);
            }
        }

        private void BtnResetar_Click(object sender, RoutedEventArgs e)
        {
            SliderMassaObjeto.Value = 60;
            SliderVolumeObjeto.Value = 0.06;
            densidadeFluido = 1000;
            AtualizarFluido("💧 Água", 1000);
            LimparResultados();
        }

        private void LimparResultados()
        {
            if (SetaPeso != null) SetaPeso.Visibility = Visibility.Collapsed;
            if (PontaSetaPeso != null) PontaSetaPeso.Visibility = Visibility.Collapsed;
            if (LabelPesoBorder != null) LabelPesoBorder.Visibility = Visibility.Collapsed;
            if (SetaEmpuxo != null) SetaEmpuxo.Visibility = Visibility.Collapsed;
            if (PontaSetaEmpuxo != null) PontaSetaEmpuxo.Visibility = Visibility.Collapsed;
            if (LabelEmpuxoBorder != null) LabelEmpuxoBorder.Visibility = Visibility.Collapsed;
            if (VolumeDeslocado != null) VolumeDeslocado.Visibility = Visibility.Collapsed;
            if (LabelVolumeBorder != null) LabelVolumeBorder.Visibility = Visibility.Collapsed;
            if (ResultadoBorder != null) ResultadoBorder.Visibility = Visibility.Collapsed;
            if (InfoForcas != null) InfoForcas.Visibility = Visibility.Collapsed;
            if (TxtPassoAPasso != null) TxtPassoAPasso.Text = "Configure os valores e clique em 'Analisar Flutuação' para ver o resultado";
            if (TxtPesoResultado != null) TxtPesoResultado.Text = "P = ?";
            if (TxtEmpuxoResultado != null) TxtEmpuxoResultado.Text = "E = ?";
            if (TxtConclusao != null) TxtConclusao.Text = "Aguardando análise...";
            if (EmojiObjeto != null) EmojiObjeto.Text = "📦";
        }

        private void AtualizarFluido(string nome, double densidade)
        {
            densidadeFluido = densidade;
            if (LabelFluido != null) LabelFluido.Text = nome;
            if (LabelDensidadeFluido != null) LabelDensidadeFluido.Text = $"ρ = {densidade:F1} kg/m³";
        }

        #region Botões de Fluido
        private void BtnAgua_Click(object sender, RoutedEventArgs e)
        {
            AtualizarFluido("💧 Água", 1000);
            LimparResultados();
        }

        private void BtnAguaMar_Click(object sender, RoutedEventArgs e)
        {
            AtualizarFluido("🌊 Água do Mar", 1025);
            LimparResultados();
        }

        private void BtnOleo_Click(object sender, RoutedEventArgs e)
        {
            AtualizarFluido("🛢️ Óleo", 920);
            LimparResultados();
        }

        private void BtnMercurio_Click(object sender, RoutedEventArgs e)
        {
            AtualizarFluido("⚗️ Mercúrio", 13600);
            LimparResultados();
        }

        private void BtnGasolina_Click(object sender, RoutedEventArgs e)
        {
            AtualizarFluido("⛽ Gasolina", 750);
            LimparResultados();
        }

        private void BtnAlcool_Click(object sender, RoutedEventArgs e)
        {
            AtualizarFluido("🍷 Álcool", 790);
            LimparResultados();
        }

        private void BtnGlicerina_Click(object sender, RoutedEventArgs e)
        {
            AtualizarFluido("🧴 Glicerina", 1260);
            LimparResultados();
        }

        private void BtnLeite_Click(object sender, RoutedEventArgs e)
        {
            AtualizarFluido("🥛 Leite", 1030);
            LimparResultados();
        }
        #endregion
    }
}