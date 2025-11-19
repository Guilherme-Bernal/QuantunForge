# Quantum Forge

<div align="center">

[![pt-br](https://img.shields.io/badge/lang-pt--br-green.svg)](README.md)
[![en](https://img.shields.io/badge/lang-en-red.svg)](README.en.md)

</div>

Quantum Forge is an interactive educational platform for teaching **Classical Physics**, **Quantum Physics**, **Classical Computing**, and **Quantum Computing**, built with **C#/.NET (WPF)** with simulation modules in **Q#**.

The project was developed as a **Final Course Project (TCC)** in Computer Engineering, focusing on supporting the learning of theoretical concepts through visual simulators and guided experimentation.

---

## 🎯 Project Objectives

- Provide **meaningful learning** (inspired by Ausubel's theory), connecting students' prior knowledge to new classical and quantum concepts
- Bring students from **Computing** and **Engineering** closer to the fundamentals of **Quantum Computing** in an accessible way
- Compare, visually and practically, **classical vs quantum** behavior (bit vs qubit, logic gates, information, etc.)
- Provide **free and open source software**, allowing other educators and developers to expand and adapt the simulators

---

## 📚 Main Modules

### 1. Classical Computing

Modules implemented in the `src/views/ComputacaoClassica` folder:

- **ALU (AluControl)**  
  Simulates basic arithmetic and logical operations, showing inputs, operations, and results.

- **Instruction Cycle (CicloInstrucaoControl)**  
  Step-by-step representation of the fetch, decode, and execute cycle in a classical architecture.

- **Boolean Logic and Logic Gates (LogicaBooleana / PortasLogicasClC)**  
  - AND, OR, NOT, NAND, NOR, XOR gates, etc.  
  - Truth table visualization and logical expression composition.

- **Binary System (SistemaBinarioControl)**  
  Conversion between decimal and binary and visualization of how information is encoded.

- **Von Neumann Architecture (VonNeumannControl)**  
  Illustration of classical memory organization, CPU, buses, and data flow.

### 2. Quantum Computing

Modules implemented in the `src/views/ComputacaoQuantica` folder and Q# files:

- **Bit vs Qubit**  
  Demonstrates the difference between classical states (0 or 1) and quantum states (superposition of |0⟩ and |1⟩).

- **Quantum Logic Gates**  
  Q# implementation of gates such as Hadamard, Pauli-X/Y/Z, among others, connected to the WPF interface.

- **Superposition and Measurement**  
  Visualization of probabilities, state collapse, and direct comparison with classical logic.

- **Classical vs Quantum Comparisons**  
  Comparison screens that show, side by side, classical and quantum behaviors for the same scenarios.

---

## 🛠️ Technologies Used

- **C# / .NET** – Application logic and module integration
- **WPF (Windows Presentation Foundation)** – Graphical interface, animations, and interactive controls
- **Q# (Quantum Development Kit)** – Definition of quantum circuits and operations
- **GitHub** – Version control and open source distribution

---

## 🏗️ Application Architecture

Quantum Forge's architecture follows three main layers:

1. **Interface (UI – WPF)**  
   - Windows and UserControls for each simulator  
   - Interactive controls (buttons, sliders, graphics, animations)

2. **Application Logic (C#)**  
   - Orchestration of learning flows  
   - Connection between interface and simulation engines  
   - Handling of states, events, and data displayed to the user

3. **Simulation Engines**  
   - **Classical:** simulations of energy, logic, architecture, and binary systems  
   - **Quantum:** operations implemented in Q# (qubits, gates, superposition)

This separation facilitates maintenance, project evolution, and the inclusion of new simulators in the future.

---

## 🚀 Installation and Execution

### Prerequisites

- **Operating system:** Windows with WPF support
- **.NET SDK:** version 6 or higher
- **Visual Studio 2022** (or newer) with workloads:
  - ".NET desktop development"
  - **Quantum Development Kit (QDK) / Q#** support

### Steps

1. Clone the repository:
   ```bash
   git clone https://github.com/Guilherme-Bernal/QuantunForge.git
   cd QuantunForge
   ```

2. Open the solution in Visual Studio:
   ```
   Quantun_Forge.sln
   ```

3. Restore NuGet packages (if Visual Studio doesn't do it automatically)

4. Set the main WPF project (`Quantun_Forge`) as **Startup Project**

5. Compile and run:
   - Build → Build Solution
   - Start (F5)

---

## 💡 How to Use

When opening the application, navigate through the **Classical Computing** and **Quantum Computing** modules. Each module has:

- Interactive elements (buttons, sliders, checkboxes)
- Graphical visualization of the concept (gates, circuits, flows)
- Explanatory texts to support learning

The software was designed to be used:

- In **lectures** (teacher demonstrating the simulators)
- In **laboratory**, with students interacting directly
- As support material for individual studies

---

## 📊 Educational Results (TCC Summary)

During the project evaluation, the following were observed:

- **Greater understanding** of classical and quantum concepts
- **High engagement** with the use of interactive simulators
- Evidence of **meaningful learning**, with students relating new concepts to prior knowledge
- **Improvement in scientific and investigative reasoning**, from experimentation and classical vs quantum comparison
- Positive feedback regarding the tool's **clarity**, **accessibility**, and **pedagogical usefulness**

---

## 📄 License

Quantum Forge is free software, licensed under the terms of the **GNU General Public License v3.0 (GPL-3.0)**.

You can use, study, modify, and redistribute this project, including for commercial purposes, as long as any distributed version remains under the same license and has the source code available.

See the [`LICENSE`](./LICENSE) file for the full license text.

---

## 📖 Academic Reference

If you use Quantum Forge in academic work, reports, or research, it is recommended to cite:

**Author:** Guilherme Savassa Bernal  
**Title:** *Quantum Forge – Educational platform for learning classical and quantum concepts in Physics and Computing*  
**Course:** Computer Engineering  
**Institution:** Facens  
**Year:** 2025

---

## 📧 Contact

For questions, suggestions, or contributions:

**LinkedIn:** [Guilherme Savassa Bernal](https://www.linkedin.com/in/guilherme-savassa-bernal/)  
**GitHub:** [@Guilherme-Bernal](https://github.com/Guilherme-Bernal)

---

## 🤝 Contributing

Contributions are welcome! Feel free to:

- Report bugs or issues
- Suggest new modules or features
- Improve documentation
- Submit pull requests

---

**Developed with 💙 to democratize Quantum Computing education**
