using Microsoft.Quantum.Simulation.Core;
using Microsoft.Quantum.Simulation.Simulators;
using Quantun_Forge.OSharp;

namespace Quantum_Forge.Services
{
    public static class QuantumSimulatorService
    {
        public static async Task<string> ExecutarSuperposicaoAsync()
        {
            using var sim = new QuantumSimulator();
            var result = await SuperpositionTest.Run(sim);
            return result == Result.One ? "|1⟩" : "|0⟩";
        }

        public static async Task<int> SimularQubitAsync()
        {
            using var sim = new QuantumSimulator();
            var result = await SuperpositionTest.Run(sim);
            return result == Result.One ? 1 : 0;
        }

        public static async Task<string> ExecutarEmaranhamentoAsync()
        {
            using var sim = new QuantumSimulator();
            return await EntangleTest.Run(sim);
        }
    }
}
