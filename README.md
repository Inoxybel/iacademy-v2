# Descrição

Sabe quando você está consumindo um treinamento e não entende aquela parte do conteúdo ? O que você faz ? Abre um tópico no fórum da plataforma e aguarda algum instrutor ou colaborador da comunidade lhe responder enquanto continua consumindo o treinamento com dúvidas ? Navega na internet em busca de respostas e fica mais confuso ? E se a plataforma fosse capaz de lhe fornecer outro exemplo ou explicação em instantes ?
O projeto IAcademy integra a inteligência artificial da OpenAI para resolver esses problemas e proporcionar uma experiência de aprendizado mais produtiva. Utilizando o maior potêncial de uma inteligência artificial generativa treinada, tornou-se possível oferecer suporte 24/7 para suplir essas necessidades que o usuário possa vir a ter, de forma automatizada. Além de oferecer grande suporte para a rápida criação de novos treinamentos e gerenciamento de treinamentos já existentes a um custo operacional incrivelmente menor!

# Arquitetura

![arquitetura](./docs/architecture.png)

# Deploy dessa API .NET no Azure App Service

Abaixo o passo a passo de como fazer deploy desse aplicação .NET diretamente no Azure App Service:

## Pré-requisitos

1. **Instale o Azure CLI**: Verifique a instalação com o comando:0
   ```sh
   az --version
   ```

2. **Instale o .NET 7.0 SDK**: Verifique a instalação com o comando:
   ```sh
   dotnet --version
   ```

3. **Login no Azure**: Faça login na sua conta Azure com o comando:
   ```sh
   az login
   ```

## Preparação

4. **Clone o Repositório**: Abra o terminal e navegue até a pasta que deseja clonar o projeto. Clone o projeto contido nesse repositório utilizando o comando abaixo:
	```sh
	git clone https://github.com/Inoxybel/iacademy.git
	```

5. **Construir a Aplicação**: Navegue até a pasta do projeto onde contém o arquivo `.sln` do projeto .NET e construa a aplicação com o comando:
   ```sh
   dotnet publish -c Release
   ```
   
## Base

6. **Criar Grupo de Recursos**: Crie um grupo de recursos no Azure com o comando:
   ```sh
   az group create --name nome-do-seu-grupo-de-recursos --location sua-regiao
   ```
   
   Ex:
   ```sh
   az group create --name gr-exemplo --location eastus
   ```
   
7. **Criar uma Instância do Azure Cosmos DB**: Crie um recurso do serviço Cosmos DB para MongoDB:
   ```sh
   az cosmosdb create --name nomeDoSeuCosmosDB --resource-group nome-do-seu-grupo-de-recursos --kind MongoDB --locations regionName=sua-regiao failoverPriority=0 isZoneRedundant=False --public-network-access Enabled
   ```
   
   Ex:
   ```sh
   az cosmosdb create --name cosmosdb --resource-group gr-exemplo --kind MongoDB --locations regionName=eastus failoverPriority=0 isZoneRedundant=False --public-network-access Enabled
   ```
   
8. **Obter a Connection String**: Recupere a String de Conexão que será usada na aplicação para que ela possa se conectar ao banco de dados na nuvem:
   ```sh
   az cosmosdb keys list --name nomeDoSeuCosmosDB --resource-group nome-do-seu-grupo-de-recursos --type connection-strings
   ```
   
   Ex:
   ```sh
   az cosmosdb keys list --name cosmosdb --resource-group gr-exemplo --type connection-strings
   ```

9. **Configurar Connection String no Projeto**: 
   - Abra o arquivo appsettings.Development.json localizado em Application/IAcademyAPI/
   - Na linha:
   ```sh
   "IAcademy:Mongo:ConnectionString": "mongodb://localhost:27017",
   ```

   Troque o valor "mongodb://localhost:27017" pela ConnectionString copiada no passo 8 e salve o arquivo.

   Na linha:
   ```sh
   "IAcademy:ExternalServices:OpenAI:SecretKey": "SECRET_FROM_OPENAI"
   ```

   Você deve colocar sua API Key obtida direto no site da OpenAI após se cadastrar, no link: https://platform.openai.com/account/api-keys

   Caso contrário não conseguirá utilizar os endpoints da controller AI, apenas conseguirá utilizar os endpoints de CRUD direto com o banco de dados.

## Deploy

10. **Criar Plano de Serviço de Aplicativo**: Crie um plano de serviço de aplicativo com o comando:
   ```sh
   az appservice plan create --name nomeDoSeuPlanoDeServico --resource-group nome-do-seu-grupo-de-recursos --sku FREE
   ```
   
   Ex:
   ```sh
   az appservice plan create --name ps-exemplo --resource-group gr-exemplo --sku FREE
   ```

11. **Criar Aplicativo Web**: Crie um aplicativo web com o comando:
   ```sh
   az webapp create --resource-group nome-do-seu-grupo-de-recursos --plan nomeDoSeuPlanoDeServico --name nomeDoSeuAplicativoWeb
   ```
   
   Ex:
   ```sh
   az webapp create --resource-group gr-exemplo --plan ps-exemplo --name apresentacaosprint3-iacademy
   ```

12. **Deploy da Aplicação**: Faça o deploy da sua aplicação usando o comando:
   ```sh
   az webapp up --name nomeDoSeuAplicativoWeb --sku F1 --resource-group nome-do-seu-grupo-de-recursos --location sua-regiao --plan nomeDoSeuPlanoDeServico
   ```
   
   Ex:
   ```sh
   az webapp up --name apresentacaosprint3-iacademy --sku F1 --resource-group gr-exemplo --location eastus --plan ps-exemplo
   ```

## Verificação

13. **Acesse sua API**: Agora você deve ser capaz de acessar sua API através do URL do aplicativo web:
   ```
   https://nomeDoSeuAplicativoWeb.azurewebsites.net
   ```
   
   Ex:
   ```
   https://apresentacaosprint3-iacademy.azurewebsites.net/swagger
   ```

# Uso da aplicação:

## Endpoints

[Clique aqui](./docs/Endpoints_readme.md) para acessar a documentação dos endpoints.

