# Deploy dessa API .NET no Azure App Service

Abaixo o passo a passo de como fazer deploy desse aplicação .NET diretamente no Azure App Service:

## Pré-requisitos

1. **Visual Studio**: Instale a IDE de programação Visual Studio

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

5. **Abrir a Aplicação**: Navegue até a pasta do projeto onde contém o arquivo `.sln`. Clique com o direito sobre esse arquivo e seleciona Visual Studio (Não visual studio CODE)
   
## Base

6. **Criar Grupo de Recursos**: Crie um grupo de recursos no Azure:
   - Acesse o link: https://portal.azure.com/#create/Microsoft.ResourceGroup
   
   ![passo1](./docs/steps_readme/1.png)
   
7. **Criar uma Instância do Azure Cosmos DB**: Crie um recurso do serviço Cosmos DB para MongoDB:
   - Acesse o link: https://portal.azure.com/#view/Microsoft_Azure_DocumentDB/MongoDB_Type_Selection.ReactView
   
   ![passo2](./docs/steps_readme/2.png)
   
   - Selecione o grupo de recursos criado ou crie um novo, preencha o formulário e clique em criar
   
8. **Obter a Connection String**: Recupere a String de Conexão que será usada na aplicação para que ela possa se conectar ao banco de dados na nuvem:
   - Após a criação do recurso, entre nas configurações/Cadeia de Conexão:
   
   ![passo3](./docs/steps_readme/3.png)
   
   - Copie a ConnectionString primária ou segundária

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
- Após abrir o projeto, clique com o direito no projeto IAcademyAPI e depois em Publish

![passo4](./docs/steps_readme/4.png)

- Na janela que abrir, clique em "Add a publish  profile"

![passo5](./docs/steps_readme/5.png)

- Na janela que abrir, selecione Docker Container Registry e clique em next

![passo6](./docs/steps_readme/6.png)

- Selecione o serviço Azure Container Registry e e clique em next

![passo7](./docs/steps_readme/7.png)

- Agora clique em "+ Create new" para criar um novo  registro

![passo8](./docs/steps_readme/8.png)

- Preencha como na imagem colocando o nome desejado e seus recursos

![passo9](./docs/steps_readme/9.png)

- Após criado, selecione e clique em next

![passo10](./docs/steps_readme/10.png)

- Selecione Docker Desktop

![passo11](./docs/steps_readme/11.png)

- Ao aparecer a mensagem "Ready to publish", clique no botão "Publish"

![passo12](./docs/steps_readme/12.png)

- Aguarde concluir o processo

![passo13](./docs/steps_readme/13.png)

- Quando aparecer concluído significa que a imagem foi publicada

![passo14](./docs/steps_readme/14.png)

Agora precisamos criar o serviço e carregar essa imagem para usar a aplicação:

- Acesse o link: https://portal.azure.com/#create/Microsoft.WebSite

- Crie o serviço web utilizando a publicação por container

![passo15](./docs/steps_readme/15.png)

- Selecione o registro de container criado no Visual Studio:

![passo16](./docs/steps_readme/16.png)

- Se nessa página você receber um erro, entre no recurso de registro criado e ative usuário administrador como na imagem:

![passo17](./docs/steps_readme/17.png)

- Clique em Revisar e Criar e Crie

![passo18](./docs/steps_readme/18.png)

- Agora para concluir entre no recurso da aplicação e sete a variável de ambiente ASPNETCORE_ENVIRONMENT com valor Development:

![passo19](./docs/steps_readme/19.png)

- Salve e aguarde a aplicação reiniciar


## Verificação

10. **Acesse sua API**: Agora você deve ser capaz de acessar sua API através do URL apresentada após a publicação:
   
   ![passo20](./docs/steps_readme/20.png)
   
   Mas lembre-se de que é uma API, então visualmente você precisa adicionar /swagger na URL para ter uma interface
   
---

[Clique aqui](../README.md) para voltar para a documentação principal.
