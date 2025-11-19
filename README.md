# Quantum Forge

<div align="center">

[![pt-br](https://img.shields.io/badge/lang-pt--br-green.svg)](README.md)
[![en](https://img.shields.io/badge/lang-en-red.svg)](README.en.md)

</div>

Quantum Forge é uma plataforma educacional interativa para o ensino de **Física Clássica**, **Física Quântica**, **Computação Clássica** e **Computação Quântica**, construída em **C#/.NET (WPF)** com módulos de simulação em **Q#**.

O projeto foi desenvolvido como **Trabalho de Conclusão de Curso (TCC)** em Engenharia da Computação, com foco em apoiar o aprendizado de conceitos teóricos por meio de simuladores visuais e experimentação guiada.

---

## 🎯 Objetivos do Projeto

- Proporcionar uma **aprendizagem significativa** (inspirada na teoria de Ausubel), conectando conhecimentos prévios dos estudantes a novos conceitos clássicos e quânticos
- Aproximar estudantes de **Computação** e **Engenharia** dos fundamentos da **Computação Quântica** de forma acessível
- Comparar, de forma visual e prática, o comportamento **clássico vs quântico** (bit x qubit, portas lógicas, informação, etc.)
- Disponibilizar um **software livre e open source**, permitindo que outros docentes e desenvolvedores ampliem e adaptem os simuladores

---

## 📚 Principais Módulos

### 1. Computação Clássica

Módulos implementados na pasta `src/views/ComputacaoClassica`:

- **ALU (AluControl)**  
  Simula operações aritméticas e lógicas básicas, mostrando entradas, operações e resultados.

- **Ciclo de Instrução (CicloInstrucaoControl)**  
  Representação passo a passo do ciclo de busca, decodificação e execução em uma arquitetura clássica.

- **Lógica Booleana e Portas Lógicas (LogicaBooleana / PortasLogicasClC)**  
  - Portas AND, OR, NOT, NAND, NOR, XOR etc.  
  - Visualização da tabela verdade e composição de expressões lógicas.

- **Sistema Binário (SistemaBinarioControl)**  
  Conversão entre decimal e binário e visualização de como a informação é codificada.

- **Arquitetura de Von Neumann (VonNeumannControl)**  
  Ilustração da organização clássica de memória, CPU, barramentos e fluxo de dados.

### 2. Computação Quântica

Módulos implementados na pasta `src/views/ComputacaoQuantica` e em arquivos Q#:

- **Bit vs Qubit**  
  Demonstra a diferença entre estados clássicos (0 ou 1) e quânticos (superposição de |0⟩ e |1⟩).

- **Portas Lógicas Quânticas**  
  Implementação em Q# de portas como Hadamard, Pauli-X/Y/Z, entre outras, ligadas à interface WPF.

- **Superposição e Medida**  
  Visualização de probabilidades, colapso de estado e comparação direta com lógica clássica.

- **Comparações Clássico vs Quântico**  
  Telas de comparação que mostram, lado a lado, comportamentos clássicos e quânticos para os mesmos cenários.

---

## 🛠️ Tecnologias Utilizadas

- **C# / .NET** – Lógica de aplicação e integração entre módulos
- **WPF (Windows Presentation Foundation)** – Interface gráfica, animações e controles interativos
- **Q# (Quantum Development Kit)** – Definição dos circuitos e operações quânticas
- **GitHub** – Controle de versão e disponibilização open source

---

## 🏗️ Arquitetura da Aplicação

A arquitetura do Quantum Forge segue três camadas principais:

1. **Interface (UI – WPF)**  
   - Janelas e UserControls para cada simulador  
   - Controles interativos (botões, sliders, gráficos, animações)

2. **Lógica de Aplicação (C#)**  
   - Orquestração dos fluxos de aprendizado  
   - Ligação entre a interface e os motores de simulação  
   - Tratamento de estados, eventos e dados exibidos ao usuário

3. **Motores de Simulação**  
   - **Clássico:** simulações de energia, lógica, arquitetura e sistemas binários  
   - **Quântico:** operações implementadas em Q# (qubits, portas, superposição)

Essa separação facilita a manutenção, a evolução do projeto e a inclusão de novos simuladores no futuro.

---

## 🚀 Instalação e Execução

### Pré-requisitos

- **Sistema operacional:** Windows com suporte a WPF
- **.NET SDK:** versão 6 ou superior
- **Visual Studio 2022** (ou mais recente) com os workloads:
  - ".NET desktop development"
  - Suporte ao **Quantum Development Kit (QDK) / Q#**

### Passos

1. Clonar o repositório:
   ```bash
   git clone https://github.com/Guilherme-Bernal/QuantunForge.git
   cd QuantunForge
   ```

2. Abrir a solução no Visual Studio:
   ```
   Quantun_Forge.sln
   ```

3. Restaurar os pacotes NuGet (se o Visual Studio não fizer automaticamente)

4. Definir o projeto WPF principal (`Quantun_Forge`) como **Startup Project**

5. Compilar e executar:
   - Build → Build Solution
   - Start (F5)

---

## 💡 Como Utilizar

Ao abrir o aplicativo, navegue pelos módulos de **Computação Clássica** e **Computação Quântica**. Cada módulo possui:

- Elementos interativos (botões, sliders, checkboxes)
- Visualização gráfica do conceito (portas, circuitos, fluxos)
- Textos explicativos para apoiar o estudo

O software foi pensado para ser usado:

- Em **aulas expositivas** (professor demonstrando os simuladores)
- Em **laboratório**, com estudantes interagindo diretamente
- Como material de apoio para estudos individuais

---

## 📊 Resultados Educacionais (Resumo do TCC)

Durante a avaliação do projeto, foram observados:

- **Maior compreensão** dos conceitos clássicos e quânticos
- **Engajamento elevado** com o uso de simuladores interativos
- Indícios de **aprendizagem significativa**, com estudantes relacionando novos conceitos aos conhecimentos prévios
- **Melhoria no raciocínio científico e investigativo**, a partir de experimentação e comparação clássico vs quântico
- Feedback positivo quanto à **clareza**, **acessibilidade** e **utilidade pedagógica** da ferramenta

---

## 📄 Licença

Quantum Forge é software livre, licenciado sob os termos da **GNU General Public License v3.0 (GPL-3.0)**.

Você pode usar, estudar, modificar e redistribuir este projeto, inclusive para fins comerciais, desde que qualquer versão distribuída permaneça sob a mesma licença e tenha o código-fonte disponibilizado.

Consulte o arquivo [`LICENSE`](./LICENSE) para o texto completo da licença.

---

## 📖 Referência Acadêmica

Se você utilizar o Quantum Forge em trabalhos acadêmicos, relatórios ou pesquisas, recomenda-se citar:

**Autor:** Guilherme Savassa Bernal  
**Título:** *Quantum Forge – Plataforma educacional para o aprendizado de conceitos clássicos e quânticos em Física e Computação*  
**Curso:** Engenharia da Computação  
**Instituição:** Facens  
**Ano:** 2025

---

## 📧 Contato

Para dúvidas, sugestões ou contribuições:

**Linkedin:** [@Guilherme-Bernal](https://www.linkedin.com/in/guilherme-savassa-bernal/)

---

## 🤝 Contribuindo

Contribuições são bem-vindas! Sinta-se à vontade para:

- Reportar bugs ou problemas
- Sugerir novos módulos ou funcionalidades
- Melhorar a documentação
- Enviar pull requests

---

**Desenvolvido com 💙 para democratizar o ensino de Computação Quântica**
