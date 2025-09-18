# Exemplos de Teste da API

## 1. Obter Classificação Atual
```bash
curl -X GET "http://localhost:5000/api/classification"
```

## 2. Criar uma Nova Equipe
```bash
curl -X POST "http://localhost:5000/api/classification/teams" \
  -H "Content-Type: application/json" \
  -d '{"name": "Equipe Alpha", "initialTime": "02:00:00"}'
```

## 3. Criar um Participante
```bash
curl -X POST "http://localhost:5000/api/classification/participants" \
  -H "Content-Type: application/json" \
  -d '{"name": "João Silva", "teamId": 1, "initialTime": "01:30:00"}'
```

## 4. Adicionar Pontuação ao Participante
```bash
curl -X PUT "http://localhost:5000/api/classification/participants/1/score" \
  -H "Content-Type: application/json" \
  -d '{
    "scoreChange": "02:30:00",
    "description": "Trabalho realizado",
    "actionType": "AddScore"
  }'
```

## 5. Adicionar Penalidade
```bash
curl -X PUT "http://localhost:5000/api/classification/participants/1/score" \
  -H "Content-Type: application/json" \
  -d '{
    "scoreChange": "00:30:00",
    "description": "Atraso na entrega",
    "actionType": "Penalty"
  }'
```

## 6. Obter Histórico do Participante
```bash
curl -X GET "http://localhost:5000/api/classification/participants/1/history"
```

## 7. Obter Histórico da Equipe
```bash
curl -X GET "http://localhost:5000/api/classification/teams/1/history"
```

## 8. Remover Participante
```bash
curl -X DELETE "http://localhost:5000/api/classification/participants/1"
```

## 9. Remover Equipe
```bash
curl -X DELETE "http://localhost:5000/api/classification/teams/1"
```

## 10. Iniciar Timer (Equipe)
```bash
curl -X POST "http://localhost:5000/api/classification/timer/start" \
  -H "Content-Type: application/json" \
  -d '{"teamId": 1}'
```

## 11. Iniciar Timer (Participante)
```bash
curl -X POST "http://localhost:5000/api/classification/timer/start" \
  -H "Content-Type: application/json" \
  -d '{"participantId": 1}'
```

## 12. Parar Timer (Equipe)
```bash
curl -X POST "http://localhost:5000/api/classification/timer/stop" \
  -H "Content-Type: application/json" \
  -d '{"teamId": 1}'
```

## 13. Parar Timer (Participante)
```bash
curl -X POST "http://localhost:5000/api/classification/timer/stop" \
  -H "Content-Type: application/json" \
  -d '{"participantId": 1}'
```

## 14. Obter Tempo Restante (Equipe)
```bash
curl -X GET "http://localhost:5000/api/classification/timer/remaining?teamId=1"
```

## 15. Obter Tempo Restante (Participante)
```bash
curl -X GET "http://localhost:5000/api/classification/timer/remaining?participantId=1"
```

## Testando com PowerShell

### Obter Classificação
```powershell
Invoke-RestMethod -Uri "http://localhost:5000/api/classification" -Method GET
```

### Criar Equipe
```powershell
$body = @{
    name = "Equipe Beta"
    initialTime = "02:00:00"
} | ConvertTo-Json

Invoke-RestMethod -Uri "http://localhost:5000/api/classification/teams" -Method POST -Body $body -ContentType "application/json"
```

### Adicionar Pontuação
```powershell
$body = @{
    scoreChange = "01:45:30"
    description = "Projeto concluído"
    actionType = "AddScore"
} | ConvertTo-Json

Invoke-RestMethod -Uri "http://localhost:5000/api/classification/participants/1/score" -Method PUT -Body $body -ContentType "application/json"
```

### Iniciar Timer (Equipe)
```powershell
$body = @{
    teamId = 1
} | ConvertTo-Json

Invoke-RestMethod -Uri "http://localhost:5000/api/classification/timer/start" -Method POST -Body $body -ContentType "application/json"
```

### Iniciar Timer (Participante)
```powershell
$body = @{
    participantId = 1
} | ConvertTo-Json

Invoke-RestMethod -Uri "http://localhost:5000/api/classification/timer/start" -Method POST -Body $body -ContentType "application/json"
```

### Parar Timer (Equipe)
```powershell
$body = @{
    teamId = 1
} | ConvertTo-Json

Invoke-RestMethod -Uri "http://localhost:5000/api/classification/timer/stop" -Method POST -Body $body -ContentType "application/json"
```

### Obter Tempo Restante
```powershell
Invoke-RestMethod -Uri "http://localhost:5000/api/classification/timer/remaining?teamId=1" -Method GET
```

## Interface Web

Acesse: http://localhost:5000/index.html

A interface web permite:
- Visualizar classificação em tempo real com tempo regressivo
- Criar e gerenciar equipes e participantes
- Controlar timers individuais (iniciar/parar)
- Ver tempo restante em tempo real
- Adicionar/remover pontuação
- Ver histórico de mudanças
- Interface moderna com abas organizadas
- Testar todas as funcionalidades via interface gráfica
