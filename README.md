# Sistema de Classificação de Equipes e Participantes

Este projeto é uma API .NET que gerencia a classificação de equipes e participantes em tempo real, onde a pontuação é baseada em horas trabalhadas com **sistema de tempo regressivo**.

## Funcionalidades

- ✅ Criação e gerenciamento de equipes
- ✅ Criação e gerenciamento de participantes
- ✅ **Sistema de tempo regressivo (countdown)**
- ✅ **Controle de timer individual para equipes e participantes**
- ✅ Sistema de pontuação baseado em tempo (horas:minutos:segundos)
- ✅ Classificação em tempo real usando SignalR
- ✅ Histórico de pontuações e punições
- ✅ Adição e remoção de pontuação
- ✅ Banco de dados SQLite local
- ✅ **Interface web moderna com abas organizadas**

## Como Executar

1. Certifique-se de ter o .NET 8.0 instalado
2. Execute o projeto:
```bash
dotnet run
```

3. Acesse a documentação da API em: `https://localhost:7000/swagger`

## Endpoints da API

### Classificação
- `GET /api/classification` - Obter classificação atual

### Equipes
- `POST /api/classification/teams` - Criar nova equipe
- `PUT /api/classification/teams/{teamId}/score` - Atualizar pontuação da equipe
- `GET /api/classification/teams/{teamId}/history` - Obter histórico da equipe
- `DELETE /api/classification/teams/{teamId}` - Remover equipe

### Participantes
- `POST /api/classification/participants` - Criar novo participante
- `PUT /api/classification/participants/{participantId}/score` - Atualizar pontuação do participante
- `GET /api/classification/participants/{participantId}/history` - Obter histórico do participante
- `DELETE /api/classification/participants/{participantId}` - Remover participante

### Timer (Tempo Regressivo)
- `POST /api/classification/timer/start` - Iniciar timer (equipe ou participante)
- `POST /api/classification/timer/stop` - Parar timer (equipe ou participante)
- `GET /api/classification/timer/remaining` - Obter tempo restante

## Exemplos de Uso

### Criar uma equipe
```json
POST /api/classification/teams
{
  "name": "Equipe Alpha",
  "initialTime": "02:00:00"
}
```

### Criar um participante
```json
POST /api/classification/participants
{
  "name": "João Silva",
  "teamId": 1,
  "initialTime": "01:30:00"
}
```

### Adicionar pontuação (tempo)
```json
PUT /api/classification/participants/1/score
{
  "scoreChange": "02:30:00",
  "description": "Trabalho realizado",
  "actionType": "AddScore"
}
```

### Remover pontuação (penalidade)
```json
PUT /api/classification/participants/1/score
{
  "scoreChange": "00:30:00",
  "description": "Atraso na entrega",
  "actionType": "Penalty"
}
```

### Tipos de Ação
- `AddScore` - Adicionar pontuação
- `RemoveScore` - Remover pontuação
- `Penalty` - Penalidade (remove pontuação)
- `Bonus` - Bônus (adiciona pontuação)

### Iniciar Timer (Equipe)
```json
POST /api/classification/timer/start
{
  "teamId": 1
}
```

### Iniciar Timer (Participante)
```json
POST /api/classification/timer/start
{
  "participantId": 1
}
```

### Parar Timer (Equipe)
```json
POST /api/classification/timer/stop
{
  "teamId": 1
}
```

### Obter Tempo Restante
```json
GET /api/classification/timer/remaining?teamId=1
```

## SignalR - Atualizações em Tempo Real

O sistema usa SignalR para notificar todos os clientes conectados sobre mudanças na classificação.

### Conectar ao Hub
```javascript
const connection = new signalR.HubConnectionBuilder()
    .withUrl("/classificationHub")
    .build();

connection.start().then(() => {
    console.log("Conectado ao hub");
});

// Escutar atualizações da classificação
connection.on("ReceiveClassification", function (classification) {
    console.log("Nova classificação:", classification);
    // Atualizar UI com os novos dados
});
```

## Estrutura do Banco de Dados

- **Teams**: Equipes com pontuação total
- **Participants**: Participantes vinculados a equipes
- **ScoreHistories**: Histórico de todas as mudanças de pontuação

## Exemplo de Resposta da Classificação

```json
{
  "teams": [
    {
      "id": 2,
      "name": "Equipe 2",
      "totalScore": "100:00:00",
      "remainingTime": "01:30:45",
      "position": 1,
      "isActive": true
    },
    {
      "id": 3,
      "name": "Equipe 3",
      "totalScore": "08:42:10",
      "remainingTime": "00:00:00",
      "position": 2,
      "isActive": false
    }
  ],
  "participants": [
    {
      "id": 1,
      "name": "João",
      "totalScore": "50:00:00",
      "remainingTime": "00:45:30",
      "teamName": "Equipe 2",
      "teamId": 2,
      "position": 1,
      "isActive": true
    },
    {
      "id": 2,
      "name": "Emily",
      "totalScore": "50:00:00",
      "remainingTime": "01:15:20",
      "teamName": "Equipe 2",
      "teamId": 2,
      "position": 2,
      "isActive": true
    },
    {
      "id": 3,
      "name": "Gustavo",
      "totalScore": "08:42:10",
      "remainingTime": "00:00:00",
      "teamName": "Equipe 3",
      "teamId": 3,
      "position": 3,
      "isActive": false
    }
  ],
  "lastUpdated": "2024-01-15T10:30:00Z"
}
```

## Tecnologias Utilizadas

- .NET 8.0
- Entity Framework Core
- SQLite
- SignalR
- ASP.NET Core Web API
- Swagger/OpenAPI
