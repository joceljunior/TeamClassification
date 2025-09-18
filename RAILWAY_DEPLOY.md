# Deploy no Railway - Guia de Configuração

## Pré-requisitos
- Conta no Railway (https://railway.app)
- Projeto conectado ao GitHub

## Passos para Deploy

### 1. Preparação do Projeto
O projeto já está configurado com:
- ✅ `railway.json` - Configuração do Railway
- ✅ Porta dinâmica configurada
- ✅ SQLite local para banco de dados
- ✅ Configuração simplificada para testes

### 2. Deploy no Railway

1. **Conecte seu repositório GitHub ao Railway:**
   - Acesse https://railway.app
   - Clique em "New Project"
   - Selecione "Deploy from GitHub repo"
   - Escolha este repositório

2. **Configure as variáveis de ambiente:**
   - No dashboard do projeto, vá em "Variables"
   - Adicione: `ASPNETCORE_ENVIRONMENT=Production`

3. **Aguarde o deploy:**
   - Railway fará o build e deploy automaticamente
   - O healthcheck agora usa o endpoint `/health`
   - Timeout aumentado para 300 segundos

### 3. Banco de Dados SQLite

O banco SQLite será criado automaticamente quando a aplicação iniciar. Não é necessário configuração adicional.

### 4. Acessar a Aplicação

Após o deploy, Railway fornecerá uma URL pública para sua aplicação.

## Configurações Importantes

### Variáveis de Ambiente Automáticas do Railway:
- `PORT` - Porta da aplicação

### Configurações Manuais:
- `ASPNETCORE_ENVIRONMENT=Production`

## Estrutura de Arquivos para Deploy

```
├── railway.json              # Configuração do Railway
├── appsettings.json          # Configurações de desenvolvimento
├── appsettings.Production.json # Configurações de produção
├── Program.cs                # Configurado para Railway
├── TeamClassification.csproj # Com SQLite
└── ... (outros arquivos do projeto)
```

## Troubleshooting

### Problemas Comuns:

1. **Aplicação não inicia:**
   - Verifique os logs no dashboard do Railway
   - Confirme se `PORT` está sendo usada corretamente

2. **Banco não é criado:**
   - O `Program.cs` já está configurado para criar o banco automaticamente
   - Verifique os logs para erros de criação do banco

3. **Dados não persistem:**
   - SQLite é local ao container, dados podem ser perdidos em reinicializações
   - Para persistência, considere usar PostgreSQL em produção

## Comandos Úteis

```bash
# Instalar Railway CLI
npm install -g @railway/cli

# Login no Railway
railway login

# Conectar ao projeto
railway link

# Ver logs
railway logs
```
