# Configurações do Projeto

## Banco de Dados
- **Tipo**: SQLite
- **Arquivo**: `teamclassification.db` (criado automaticamente)
- **Localização**: Raiz do projeto

## Portas
- **HTTP**: 5000
- **HTTPS**: 7000

## URLs Importantes
- **API**: http://localhost:5000/api/classification
- **Swagger**: http://localhost:5000/swagger
- **Interface Web**: http://localhost:5000/index.html
- **SignalR Hub**: http://localhost:5000/classificationHub

## Dados Iniciais
O sistema já vem com dados de exemplo:
- **Equipe 1**: Pontuação 00:00:00
- **Equipe 2**: Pontuação 00:00:00  
- **Equipe 3**: Pontuação 00:00:00
- **João** (Equipe 2): Pontuação 00:00:00
- **Emily** (Equipe 2): Pontuação 00:00:00
- **Gustavo** (Equipe 3): Pontuação 00:00:00

## Comandos Úteis

### Executar o Projeto
```bash
dotnet run
```

### Compilar
```bash
dotnet build
```

### Restaurar Pacotes
```bash
dotnet restore
```

### Limpar e Recompilar
```bash
dotnet clean
dotnet build
```

## Estrutura do Projeto
```
TeamClassification/
├── Controllers/          # Controllers da API
├── Data/                 # DbContext e configurações do banco
├── DTOs/                 # Data Transfer Objects
├── Hubs/                 # SignalR Hubs
├── Models/               # Modelos de dados
├── Services/             # Serviços de negócio
├── wwwroot/              # Arquivos estáticos (HTML, CSS, JS)
├── Properties/           # Configurações de execução
├── Program.cs            # Ponto de entrada da aplicação
├── appsettings.json      # Configurações da aplicação
└── TeamClassification.csproj  # Arquivo do projeto
```

## Funcionalidades Implementadas

✅ **CRUD de Equipes**
- Criar equipe
- Listar equipes
- Remover equipe
- Histórico da equipe

✅ **CRUD de Participantes**
- Criar participante
- Listar participantes
- Remover participante
- Histórico do participante

✅ **Sistema de Pontuação**
- Adicionar pontuação (tempo)
- Remover pontuação
- Aplicar penalidades
- Aplicar bônus

✅ **Classificação em Tempo Real**
- SignalR para atualizações instantâneas
- Classificação ordenada por pontuação
- Posições atualizadas automaticamente

✅ **Histórico Completo**
- Todas as mudanças de pontuação são registradas
- Timestamp de cada alteração
- Descrição da ação
- Tipo de ação (AddScore, RemoveScore, Penalty, Bonus)

✅ **Interface Web**
- Visualização em tempo real
- Formulários para adicionar pontuação
- Histórico interativo
- Conexão automática com SignalR
