# Solução para Problema de Healthcheck no Railway

## Problema
O Railway está falhando no healthcheck mesmo sem configuração explícita.

## Soluções Aplicadas

### 1. Configuração Mínima (railway.json)
```json
{
  "build": {
    "builder": "NIXPACKS"
  }
}
```

### 2. Endpoints de Healthcheck
- `/health` - Retorna "OK"
- `/` - Retorna "TeamClassification API is running!"

### 3. Configuração Robusta de Porta
- Usa variável `PORT` do Railway
- Fallback para porta 5000
- Bind em `0.0.0.0` para aceitar conexões externas

## Passos para Resolver

### Opção 1: Deploy com Nixpacks (Recomendado)
1. **Configure as variáveis no Railway:**
   - `ASPNETCORE_ENVIRONMENT=Production`
   - `ASPNETCORE_URLS=http://0.0.0.0:5000`

2. **Faça commit e push:**
   ```bash
   git add .
   git commit -m "Fix Railway healthcheck with minimal config"
   git push
   ```

### Opção 2: Deploy com Docker (Alternativa)
Se o problema persistir, use o Dockerfile:

1. **No Railway, vá em Settings:**
   - Mude o builder para "Dockerfile"
   - Railway usará o Dockerfile em vez do Nixpacks

2. **Configure as mesmas variáveis:**
   - `ASPNETCORE_ENVIRONMENT=Production`
   - `ASPNETCORE_URLS=http://0.0.0.0:5000`

## Verificação
Após o deploy, teste os endpoints:
- `https://seu-app.railway.app/` - Deve retornar "TeamClassification API is running!"
- `https://seu-app.railway.app/health` - Deve retornar "OK"

## Logs
Se ainda falhar, verifique os logs no Railway:
- Vá em "Deployments" → "View logs"
- Procure por erros de porta ou binding
- Verifique se a aplicação está iniciando corretamente
