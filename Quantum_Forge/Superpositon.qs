namespace Quantun_Forge.OSharp 
{
    open Microsoft.Quantum.Intrinsic;
    open Microsoft.Quantum.Measurement;

    /// <summary>
    /// Testa a superposição quântica aplicando uma porta Hadamard
    /// em um qubit e medindo o resultado
    /// </summary>
    operation SuperpositionTest() : Result 
    {
        // Aloca um qubit no estado |0⟩
        use q = Qubit();
        
        // Aplica a porta Hadamard para criar superposição
        // |0⟩ → (|0⟩ + |1⟩) / √2
        H(q);
        
        // Mede o qubit (colapsa a superposição)
        let result = M(q);
        
        // Reseta o qubit para |0⟩ (obrigatório)
        Reset(q);
        
        return result;
    }
}