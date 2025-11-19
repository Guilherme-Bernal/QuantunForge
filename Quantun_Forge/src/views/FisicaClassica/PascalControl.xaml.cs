using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace Quantun_Forge.src.views.FisicaClassica
{
    public partial class PascalControl : UserControl
    {
        public PascalControl()
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
            if (SliderForca1 == null || SliderArea1 == null || SliderArea2 == null) return;

            double F1 = SliderForca1.Value;
            double A1_cm2 = SliderArea1.Value;
            double A2_cm2 = SliderArea2.Value;

            // Converter cm² para m²
            double A1 = A1_cm2 / 10000.0;
            double A2 = A2_cm2 / 10000.0;

            // Atualizar labels dos sliders
            if (TxtForca1Slider != null)
            {
                TxtForca1Slider.Text = $"{F1:F0} N";
            }

            if (TxtArea1Slider != null)
            {
                TxtArea1Slider.Text = $"{A1_cm2:F0} cm² ({A1:F4} m²)";
            }

            if (TxtArea2Slider != null)
            {
                TxtArea2Slider.Text = $"{A2_cm2:F0} cm² ({A2:F4} m²)";
            }

            // Atualizar labels no canvas
            if (LabelA1 != null)
            {
                LabelA1.Text = $"A₁ = {A1:F4} m²";
            }

            if (LabelA2 != null)
            {
                LabelA2.Text = $"A₂ = {A2:F4} m²";
            }

            // Ajustar tamanhos visuais dos êmbolos proporcionalmente
            AjustarTamanhosEmbolos(A1_cm2, A2_cm2);
        }

        private void AjustarTamanhosEmbolos(double A1_cm2, double A2_cm2)
        {
            if (EmboloEsquerdo == null || EmboloDireito == null) return;

            // Calcular larguras proporcionais (base: 74 para A1=100, 154 para A2=400)
            double larguraBase1 = 74;
            double larguraBase2 = 154;

            double novaLargura1 = Math.Max(40, Math.Min(120, larguraBase1 * (A1_cm2 / 100.0)));
            double novaLargura2 = Math.Max(80, Math.Min(200, larguraBase2 * (A2_cm2 / 400.0)));

            // Atualizar larguras
            EmboloEsquerdo.Width = novaLargura1;
            FluidoEsquerdo.Width = novaLargura1;
            TuboEsquerdo.Width = novaLargura1 + 6;

            EmboloDireito.Width = novaLargura2;
            FluidoDireito.Width = novaLargura2;
            TuboDireito.Width = novaLargura2 + 6;

            // Ajustar posições para centralizar
            Canvas.SetLeft(EmboloEsquerdo, 140 - novaLargura1 / 2);
            Canvas.SetLeft(FluidoEsquerdo, 140 - novaLargura1 / 2 + 3);
            Canvas.SetLeft(TuboEsquerdo, 140 - (novaLargura1 + 6) / 2);

            Canvas.SetLeft(EmboloDireito, 600 - novaLargura2 / 2);
            Canvas.SetLeft(FluidoDireito, 600 - novaLargura2 / 2 + 3);
            Canvas.SetLeft(TuboDireito, 600 - (novaLargura2 + 6) / 2);
        }

        private void BtnCalcular_Click(object sender, RoutedEventArgs e)
        {
            double F1 = SliderForca1.Value;
            double A1_cm2 = SliderArea1.Value;
            double A2_cm2 = SliderArea2.Value;

            // Converter cm² para m²
            double A1 = A1_cm2 / 10000.0;
            double A2 = A2_cm2 / 10000.0;

            // Calcular pressão
            double P = F1 / A1;

            // Calcular força resultante
            double F2 = P * A2;

            // Calcular ganho mecânico
            double ganhoMecanico = A2 / A1;

            // Mostrar elementos visuais
            MostrarElementosVisuais(F1, F2, P, ganhoMecanico);

            // Atualizar resultados
            if (TxtForcaResultado != null)
            {
                TxtForcaResultado.Text = $"F₂ = {F2:F2} N";
            }

            if (TxtComparacaoForca != null)
            {
                double massaEquivalente = F2 / 9.8;
                if (massaEquivalente >= 1000)
                {
                    TxtComparacaoForca.Text = $"≈ peso de {massaEquivalente / 1000:F2} toneladas";
                }
                else
                {
                    TxtComparacaoForca.Text = $"≈ peso de {massaEquivalente:F2} kg";
                }
            }

            if (TxtGanhoResultado != null)
            {
                TxtGanhoResultado.Text = $"GM = {ganhoMecanico:F2}x";
            }

            if (TxtExplicacaoGanho != null)
            {
                TxtExplicacaoGanho.Text = $"Cada 1 N aplicado gera {ganhoMecanico:F2} N de força";
            }

            // Passo a passo
            if (TxtPassoAPasso != null)
            {
                TxtPassoAPasso.Text = $"1️⃣ Dados fornecidos:\n" +
                                     $"   F₁ = {F1:F2} N (força aplicada)\n" +
                                     $"   A₁ = {A1:F4} m² (área menor)\n" +
                                     $"   A₂ = {A2:F4} m² (área maior)\n\n" +
                                     $"2️⃣ Calcular pressão no êmbolo menor:\n" +
                                     $"   P = F₁/A₁\n" +
                                     $"   P = {F1:F2}/{A1:F4}\n" +
                                     $"   P = {P:F2} Pa\n\n" +
                                     $"3️⃣ Princípio de Pascal:\n" +
                                     $"   A pressão é igual em todo o fluido!\n" +
                                     $"   P₁ = P₂ = {P:F2} Pa\n\n" +
                                     $"4️⃣ Calcular força no êmbolo maior:\n" +
                                     $"   F₂ = P × A₂\n" +
                                     $"   F₂ = {P:F2} × {A2:F4}\n" +
                                     $"   F₂ = {F2:F2} N\n\n" +
                                     $"5️⃣ Ganho mecânico:\n" +
                                     $"   GM = A₂/A₁ = {A2:F4}/{A1:F4}\n" +
                                     $"   GM = {ganhoMecanico:F2}x\n\n" +
                                     $"✅ Com {F1:F2} N conseguimos levantar {F2:F2} N!";
            }

            // Animar êmbolos
            AnimarSistema();
        }

        private void MostrarElementosVisuais(double F1, double F2, double P, double ganho)
        {
            // Mostrar setas de força
            if (SetaForca1 != null) SetaForca1.Visibility = Visibility.Visible;
            if (PontaSetaForca1 != null) PontaSetaForca1.Visibility = Visibility.Visible;
            if (LabelF1Border != null) LabelF1Border.Visibility = Visibility.Visible;
            if (LabelF1 != null) LabelF1.Text = $"F₁ = {F1:F0} N";

            if (SetaForca2 != null) SetaForca2.Visibility = Visibility.Visible;
            if (PontaSetaForca2 != null) PontaSetaForca2.Visibility = Visibility.Visible;
            if (LabelF2Border != null) LabelF2Border.Visibility = Visibility.Visible;
            if (LabelF2 != null) LabelF2.Text = $"F₂ = {F2:F0} N";

            // Mostrar ganho mecânico
            if (GanhoMecanico != null)
            {
                GanhoMecanico.Visibility = Visibility.Visible;
                if (TxtGanhoMecanico != null)
                {
                    TxtGanhoMecanico.Text = $"{ganho:F1}x";
                }
            }

            // Mostrar info da pressão
            if (InfoPressao != null)
            {
                InfoPressao.Visibility = Visibility.Visible;
                if (TxtPressao != null)
                {
                    if (P >= 1e6)
                    {
                        TxtPressao.Text = $"P = {P / 1e6:F2} MPa";
                    }
                    else if (P >= 1e3)
                    {
                        TxtPressao.Text = $"P = {P / 1e3:F2} kPa";
                    }
                    else
                    {
                        TxtPressao.Text = $"P = {P:F2} Pa";
                    }
                }
            }
        }

        private void AnimarSistema()
        {
            // Animar êmbolo esquerdo descendo
            if (EmboloEsquerdo != null)
            {
                var animDesc = new DoubleAnimation
                {
                    From = Canvas.GetTop(EmboloEsquerdo),
                    To = Canvas.GetTop(EmboloEsquerdo) + 15,
                    Duration = TimeSpan.FromSeconds(0.5),
                    AutoReverse = true
                };
                EmboloEsquerdo.BeginAnimation(Canvas.TopProperty, animDesc);
            }

            // Animar êmbolo direito subindo
            if (EmboloDireito != null)
            {
                var animSub = new DoubleAnimation
                {
                    From = Canvas.GetTop(EmboloDireito),
                    To = Canvas.GetTop(EmboloDireito) - 20,
                    Duration = TimeSpan.FromSeconds(0.5),
                    AutoReverse = true
                };
                EmboloDireito.BeginAnimation(Canvas.TopProperty, animSub);
            }

            // Animar carga subindo
            if (CargaLevantada != null)
            {
                var animCarga = new DoubleAnimation
                {
                    From = Canvas.GetTop(CargaLevantada),
                    To = Canvas.GetTop(CargaLevantada) - 20,
                    Duration = TimeSpan.FromSeconds(0.5),
                    AutoReverse = true
                };
                CargaLevantada.BeginAnimation(Canvas.TopProperty, animCarga);
            }

            // Pulsar fluido
            if (FluidoEsquerdo != null && FluidoDireito != null)
            {
                var animOpacity = new DoubleAnimation
                {
                    From = 0.8,
                    To = 1.0,
                    Duration = TimeSpan.FromSeconds(0.3),
                    AutoReverse = true,
                    RepeatBehavior = new RepeatBehavior(2)
                };
                FluidoEsquerdo.BeginAnimation(UIElement.OpacityProperty, animOpacity);
                FluidoDireito.BeginAnimation(UIElement.OpacityProperty, animOpacity);
            }
        }

        private void BtnResetar_Click(object sender, RoutedEventArgs e)
        {
            // Resetar valores padrão
            SliderForca1.Value = 100;
            SliderArea1.Value = 100;
            SliderArea2.Value = 400;

            // Limpar resultados
            LimparResultados();
        }

        private void LimparResultados()
        {
            if (TxtForcaResultado != null)
                TxtForcaResultado.Text = "F₂ = ?";

            if (TxtComparacaoForca != null)
                TxtComparacaoForca.Text = "Aguardando cálculo...";

            if (TxtGanhoResultado != null)
                TxtGanhoResultado.Text = "GM = ?";

            if (TxtExplicacaoGanho != null)
                TxtExplicacaoGanho.Text = "Quantas vezes a força é multiplicada";

            if (TxtPassoAPasso != null)
                TxtPassoAPasso.Text = "Configure os valores e clique em 'Calcular Sistema' para ver o resultado";

            // Esconder elementos visuais
            if (SetaForca1 != null) SetaForca1.Visibility = Visibility.Collapsed;
            if (PontaSetaForca1 != null) PontaSetaForca1.Visibility = Visibility.Collapsed;
            if (LabelF1Border != null) LabelF1Border.Visibility = Visibility.Collapsed;
            if (SetaForca2 != null) SetaForca2.Visibility = Visibility.Collapsed;
            if (PontaSetaForca2 != null) PontaSetaForca2.Visibility = Visibility.Collapsed;
            if (LabelF2Border != null) LabelF2Border.Visibility = Visibility.Collapsed;
            if (GanhoMecanico != null) GanhoMecanico.Visibility = Visibility.Collapsed;
            if (InfoPressao != null) InfoPressao.Visibility = Visibility.Collapsed;
        }

        #region Cenários Prontos
        private void BtnElevadorCarro_Click(object sender, RoutedEventArgs e)
        {
            // Elevador de carro: pequena força, grande resultado
            SliderForca1.Value = 200; // 200 N de força humana
            SliderArea1.Value = 50; // Área pequena
            SliderArea2.Value = 800; // Área bem maior
            LimparResultados();
        }

        private void BtnSistemaFreio_Click(object sender, RoutedEventArgs e)
        {
            // Sistema de freio: médio ganho
            SliderForca1.Value = 150;
            SliderArea1.Value = 80;
            SliderArea2.Value = 300;
            LimparResultados();
        }

        private void BtnMacacoHidraulico_Click(object sender, RoutedEventArgs e)
        {
            // Macaco hidráulico típico
            SliderForca1.Value = 100;
            SliderArea1.Value = 60;
            SliderArea2.Value = 600;
            LimparResultados();
        }

        private void BtnGrandeMultiplicacao_Click(object sender, RoutedEventArgs e)
        {
            // Máxima multiplicação de força
            SliderForca1.Value = 50;
            SliderArea1.Value = 10;
            SliderArea2.Value = 1000;
            LimparResultados();
        }
        #endregion
    }
}