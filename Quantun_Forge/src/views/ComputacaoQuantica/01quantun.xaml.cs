using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Quantun_Forge.src.views.ComputacaoQuantica
{
    public partial class _01quantun : UserControl
    {
        // Estados simplificados para o ensino visual
        private enum EstadoQubit
        {
            Zero,              // |0⟩
            Um,                // |1⟩
            Superposicao,      // (|0⟩ + |1⟩)/√2
            MinusSuperposicao  // (|0⟩ - |1⟩)/√2
        }

        private EstadoQubit estadoAtual = EstadoQubit.Zero;
        private EstadoQubit estadoQubit2 = EstadoQubit.Zero;
        private Random rnd = new();

        public _01quantun()
        {
            InitializeComponent();
            AtualizaQubit("Qubit inicializado em |0⟩");
        }

        private void BtnReset_Click(object sender, RoutedEventArgs e)
        {
            estadoAtual = EstadoQubit.Zero;
            estadoQubit2 = EstadoQubit.Zero;
            AtualizaQubit("Resetado para |0⟩");
        }

        private void BtnH_Click(object sender, RoutedEventArgs e)
        {
            if (estadoAtual == EstadoQubit.Zero)
            {
                estadoAtual = EstadoQubit.Superposicao;
                AtualizaQubit("Hadamard: |0⟩ → (|0⟩ + |1⟩)/√2 (superposição)");
            }
            else if (estadoAtual == EstadoQubit.Um)
            {
                estadoAtual = EstadoQubit.MinusSuperposicao;
                AtualizaQubit("Hadamard: |1⟩ → (|0⟩ - |1⟩)/√2 (superposição com sinal -)");
            }
            else if (estadoAtual == EstadoQubit.Superposicao || estadoAtual == EstadoQubit.MinusSuperposicao)
            {
                AtualizaQubit("Hadamard: já está em superposição.");
            }
        }

        private void BtnX_Click(object sender, RoutedEventArgs e)
        {
            if (estadoAtual == EstadoQubit.Zero)
                estadoAtual = EstadoQubit.Um;
            else if (estadoAtual == EstadoQubit.Um)
                estadoAtual = EstadoQubit.Zero;
            else if (estadoAtual == EstadoQubit.Superposicao)
                estadoAtual = EstadoQubit.Superposicao; // X mantém superposição simétrica
            else if (estadoAtual == EstadoQubit.MinusSuperposicao)
                estadoAtual = EstadoQubit.MinusSuperposicao;
            AtualizaQubit("Pauli-X (NOT): troca |0⟩ ↔ |1⟩");
        }

        private void BtnY_Click(object sender, RoutedEventArgs e)
        {
            // Didático: Inverte e alterna superposições (não simula parte imaginária real)
            if (estadoAtual == EstadoQubit.Zero)
                estadoAtual = EstadoQubit.Um;
            else if (estadoAtual == EstadoQubit.Um)
                estadoAtual = EstadoQubit.Zero;
            else if (estadoAtual == EstadoQubit.Superposicao)
                estadoAtual = EstadoQubit.MinusSuperposicao;
            else if (estadoAtual == EstadoQubit.MinusSuperposicao)
                estadoAtual = EstadoQubit.Superposicao;
            AtualizaQubit("Pauli-Y: inverte estado e alterna a fase (didático)");
        }

        private void BtnZ_Click(object sender, RoutedEventArgs e)
        {
            // Didático: Troca o sinal da superposição ou do estado |1⟩
            if (estadoAtual == EstadoQubit.Zero)
                estadoAtual = EstadoQubit.Zero;
            else if (estadoAtual == EstadoQubit.Um)
                estadoAtual = EstadoQubit.MinusSuperposicao;
            else if (estadoAtual == EstadoQubit.Superposicao)
                estadoAtual = EstadoQubit.MinusSuperposicao;
            else if (estadoAtual == EstadoQubit.MinusSuperposicao)
                estadoAtual = EstadoQubit.Superposicao;
            AtualizaQubit("Pauli-Z: muda o sinal de |1⟩ (fase quântica)");
        }

        private void BtnMedir_Click(object sender, RoutedEventArgs e)
        {
            if (estadoAtual == EstadoQubit.Superposicao || estadoAtual == EstadoQubit.MinusSuperposicao)
            {
                var val = rnd.Next(2);
                estadoAtual = val == 0 ? EstadoQubit.Zero : EstadoQubit.Um;
                AtualizaQubit($"Mediu: colapsou para {(val == 0 ? "|0⟩" : "|1⟩")} (aleatório)");
            }
            else
            {
                AtualizaQubit($"Mediu: permaneceu em {(estadoAtual == EstadoQubit.Zero ? "|0⟩" : "|1⟩")}");
            }
        }

        private void BtnCNOT_Click(object sender, RoutedEventArgs e)
        {
            // Se controle for |1⟩, inverte o segundo qubit (|0⟩ <-> |1⟩)
            if (estadoAtual == EstadoQubit.Um)
            {
                estadoQubit2 = estadoQubit2 == EstadoQubit.Zero ? EstadoQubit.Um : EstadoQubit.Zero;
                AtualizaQubit("CNOT: Segundo qubit invertido porque controle = |1⟩");
            }
            else
            {
                AtualizaQubit("CNOT: Controle não é |1⟩, nada muda no segundo qubit.");
            }
        }

        // Atualiza feedback visual e textual do(s) qubit(s)
        private void AtualizaQubit(string mensagem = "")
        {
            // Estado principal
            switch (estadoAtual)
            {
                case EstadoQubit.Zero:
                    txtQubitVisual.Text = "|0⟩";
                    txtQubitVisual.Foreground = new SolidColorBrush(Color.FromRgb(59, 154, 225));
                    txtQubitDesc.Text = "Estado puro: 0 (igual ao bit clássico)";
                    break;
                case EstadoQubit.Um:
                    txtQubitVisual.Text = "|1⟩";
                    txtQubitVisual.Foreground = new SolidColorBrush(Color.FromRgb(255, 126, 90));
                    txtQubitDesc.Text = "Estado puro: 1 (igual ao bit clássico)";
                    break;
                case EstadoQubit.Superposicao:
                    txtQubitVisual.Text = "|ψ⟩ = (|0⟩ + |1⟩)/√2";
                    txtQubitVisual.Foreground = new SolidColorBrush(Color.FromRgb(255, 215, 0));
                    txtQubitDesc.Text = "Superposição simétrica: 0 e 1 ao mesmo tempo";
                    break;
                case EstadoQubit.MinusSuperposicao:
                    txtQubitVisual.Text = "|ψ⟩ = (|0⟩ - |1⟩)/√2";
                    txtQubitVisual.Foreground = new SolidColorBrush(Color.FromRgb(185, 142, 255));
                    txtQubitDesc.Text = "Superposição com sinal negativo: (|0⟩ - |1⟩)/√2";
                    break;
            }
            txtPortaResultado.Text = mensagem;

            // Segundo qubit para CNOT (exibe sempre |0⟩ ou |1⟩)
            if (txtQubit2Visual != null)
            {
                if (estadoQubit2 == EstadoQubit.Zero)
                    txtQubit2Visual.Text = "|0⟩";
                else if (estadoQubit2 == EstadoQubit.Um)
                    txtQubit2Visual.Text = "|1⟩";
                else
                    txtQubit2Visual.Text = "";
            }
        }
    }
}
