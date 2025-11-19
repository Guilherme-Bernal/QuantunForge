using System.Threading.Tasks;
using Microsoft.Quantum.Simulation.Core;
using Microsoft.Quantum.Simulation.Simulators;
using Quantun_Forge.OSharp;

namespace Quantum_Forge.Services
{
    /// <summary>
    /// Serviço para executar operações quânticas usando o simulador Q#
    /// </summary>
    public static class QuantumSimulatorService
    {
        /// <summary>
        /// Executa uma simulação de superposição e retorna o resultado como string
        /// </summary>
        /// <returns>String representando o estado medido: |0⟩ ou |1⟩</returns>
        public static async Task<string> ExecutarSuperposicaoAsync()
        {
            using var sim = new QuantumSimulator();
            var result = await SuperpositionTest.Run(sim);
            return result == Result.One ? "|1⟩" : "|0⟩";
        }

        /// <summary>
        /// Executa uma simulação de superposição e retorna o resultado como inteiro
        /// </summary>
        /// <returns>0 para |0⟩ ou 1 para |1⟩</returns>
        public static async Task<int> SimularQubitAsync()
        {
            using var sim = new QuantumSimulator();
            var result = await SuperpositionTest.Run(sim);
            return result == Result.One ? 1 : 0;
        }

        /// <summary>
        /// Executa uma simulação de emaranhamento quântico
        /// </summary>
        /// <returns>String representando o estado emaranhado</returns>
        public static async Task<string> ExecutarEmaranhamentoAsync()
        {
            using var sim = new QuantumSimulator();
            return await EntangleTest.Run(sim);
        }
    }
}