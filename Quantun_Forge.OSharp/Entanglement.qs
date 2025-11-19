namespace Quantun_Forge.OSharp {
    open Microsoft.Quantum.Intrinsic;
    open Microsoft.Quantum.Measurement;

    operation EntangleTest() : String {
        use q1 = Qubit();
        use q2 = Qubit();

        H(q1);
        CNOT(q1, q2);

        let m1 = M(q1);
        let m2 = M(q2);

        Reset(q1);
        Reset(q2);

        if (m1 == Zero and m2 == Zero) {
            return "00";
        } elif (m1 == One and m2 == One) {
            return "11";
        } else {
            return "invalido";
        }
    }
}