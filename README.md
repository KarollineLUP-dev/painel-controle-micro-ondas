# Painel de Controle - Micro-ondas Digital

Sistema de simulação de um micro-ondas digital com funcionalidades avançadas, desenvolvido com .NET e Avalonia UI.

### 🚀 Tecnologias

* **C#** (com .NET 9.0)
* **Avalonia UI 11.x** (Framework de UI Multiplataforma)
* **System.Text.Json** (Para persistência de dados)
* **.NET SDK 9.x**

### ⚡ Recursos da Aplicação

* **Aquecimento Manual:** Controle preciso de tempo e potência.
* **Programas Pré-definidos:** 5 programas base carregados via JSON.
* **Programas Customizáveis (CRUD):**
    * **Criação:** Adicione novos programas através de um formulário dedicado.
    * **Leitura:** Programas customizados são exibidos dinamicamente na tela principal.
    * **Exclusão:** Remova programas customizados através de um menu de seleção.
* **Persistência:** Programas criados pelo usuário são salvos em um arquivo `custom_programs.json` para não serem perdidos.
* **UI Reativa:** A interface se atualiza automaticamente ao adicionar ou remover programas.

### 📁 Estrutura do Projeto

O código foi organizado seguindo princípios de separação de responsabilidades para facilitar a manutenção e a escalabilidade:

```
/
├── data/                 # Armazena os arquivos JSON com os dados da aplicação.
│   ├── presets.json      # Contém os 5 programas de aquecimento pré-definidos.
│   └── custom_programs.json # Armazena os programas criados pelo usuário.
│
├── enums/                # Define os diferentes estados que a aplicação pode ter.
│   ├── InputState.cs     # Controla se o usuário está digitando Tempo ou Potência.
│   └── StatusMicroOndas.cs # Controla o estado do micro-ondas (Ocioso, Aquecendo, Pausado).
│
├── model/                # Contém as classes de modelo e o código C# das janelas (code-behind).
│   ├── HeatingProgram.cs # Classe que representa um programa de aquecimento.
│   ├── MainWindow.axaml.cs
│   ├── CreateProgramWindow.axaml.cs
│   └── DeleteProgramWindow.axaml.cs
│
├── service/              # Lógica de negócio para manipulação dos dados (leitura e escrita).
│   └── ProgramService.cs
│
├── controller/           # Contém a "máquina de estados" e a lógica de funcionamento do micro-ondas.
│   └── MicroOndasController.cs
│
├── view/                 # Contém os arquivos .axaml, que definem a parte visual da interface.
│   ├── MainWindow.axaml
│   ├── CreateProgramWindow.axaml
│   └── DeleteProgramWindow.axaml
│
├── App.axaml             # Definição visual global da aplicação.
├── App.axaml.cs          # Ponto de entrada C# da aplicação Avalonia.
├── Program.cs            # Ponto de entrada do executável (método Main).
└── *.csproj              # Arquivo de projeto .NET que gerencia as dependências.
```

### 🏃‍♂️ Como executar

* **Pré-requisitos:** .NET SDK 9.0 ou superior instalado.
* Clone o repositório ou descompacte os arquivos do projeto.
* Abra o terminal na pasta raiz do projeto.
* Restaure as dependências:
 ```bash
 dotnet restore
 ```
4.  Compile o projeto para verificar se há erros:
```bash
dotnet build
```
5.  Execute a aplicação com o comando:
```bash
dotnet run
```

> **Nota para usuários Linux:** Se a acentuação não funcionar nos campos de texto, adicione `Environment.SetEnvironmentVariable("AVALONIA_ENABLE_XIM", "1");` no início do método `Main` em `Program.cs`.

### 🔧 Configuração

* **Programas Pré-definidos:** `data/presets.json` (Somente leitura)
* **Programas Customizados:** `data/custom_programs.json` (Leitura e Escrita)

### 👨‍💻 Desenvolvedor

* Karolline Lopes Urtado Pereira
* **Screenshots:** [Veja os screenshots da aplicação aqui.](prints-tela-rodando/)

> Desenvolvido com C# 12, .NET 9.0 e Avalonia UI 11.x
