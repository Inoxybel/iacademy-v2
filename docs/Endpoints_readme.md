## Endpoints
- AI
  - [Criar Resumo](#criar-resumo)
  - [Criar conteúdo do Subtópico](#criar-conteúdo-do-subtópico)
  - [Criar conteúdo alternativo](#criar-conteúdo-alternativo)
  - [Criar exercício](#criar-exercício)
  - [Criar correção](#criar-correção)
- Configuração
  - [Criar configuração](#criar-configuração)
  - [Atualizar configuração](#atualizar-configuração)
  - [Recuperar configuração](#recuperar-configuração)
- Sumário
  - [Criar sumário](#criar-sumário)
  - [Criar matrícula](#criar-matrícula)
  - [Atualizar sumário](#atualizar-sumário)
  - [Recuperar sumário](#recuperar-sumário)
  - [Recuperar sumário por categoria](#recuperar-sumário-por-categoria)
  - [Recuperar sumário por subcategoria](#recuperar-sumário-por-subcategoria)
  - [Recuperar sumário por categoria e subcategoria](#recuperar-sumário-por-categoria-e-subcategoria)
  - [Recuperar sumário por identificação do dono](#recuperar-sumário-por-identificação-do-dono)
- Conteúdo
  - [Recupear conteúdo](#recuperar-conteúdo)
  - [Recuperar todos conteúdos](#recuperar-todos-conteúdos)
  - [Atualizar conteúdo](#atualizar-conteúdo)
  - [Atualizar todos conteúdos](#atualizar-todos-conteúdos)
  - [Criar um conteúdo](#criar-conteúdo)
  - [Criar vários conteúdos](#criar-vários-conteúdos)
- Exercício
  - [Recuperar exercício](#recuperar-exercício)
  - [Recuperar vários exercícios](#recuperar-vários-exercícios)
  - [Atualizar exercício](#atualizar-exercício)
  - [Criar exercício](#criar-exercício)
- Correção
  - [Recuperar correção](#recuperar-correção)
  - [Atualizar correção](#atualizar-correção)
  
---

#AI

## Criar Resumo
`POST` /api/ai/summary/create

| Campo | Tipo | Obrigatório | Descrição
|:-------:|:------:|:-------------:|--
| theme | string | sim | é o tema do treinamento
| category | string | sim | é a categoria  do treinamento
| subcategory | string | sim | é a subcategoria dentro da categoria do treinamento
| configurationId | string | sim | é o GUID da configuração no banco de dados
| ownerId | string | sim | é o GUID que identifica o "dono" desse objeto no banco de dados


**Exemplo de corpo do request**
```js
{
  "theme": "C# Básico",
  "category": "Tecnologia",
  "subcategory": "Programação",
  "configurationId": "1ba702b0-6781-4ef7-b6a4-c3612c1d0f7c",
  "ownerId": "4e520ee6-8f81-49fe-8dd7-d972e58bac4e"
}
```

**Exemplo de corpo de response**
```js
{
    "id": "07cd1867-05c2-431f-a2a7-eea9e935378c"
}
```

**Códigos de Resposta**
| Código | Descrição
|:-:|-
| 201 | Resumo criado com sucesso
| 400 | Erro na requisição

[Voltar ao início](#endpoints)

---

## Criar conteúdos do Tópico
`POST` /api/ai/summary/{summaryId}/create-content-by-topic

| Campo | Tipo | Obrigatório | Descrição
|:-------:|:------:|:-------------:|--
| topicIndex | string | sim | índice do tópico desejado

**Exemplo de corpo do request**
```js
{
  "topicIndex": "1"
}
```

**Exemplo de corpo de response**
```js
[
	"07cd1867-05c2-431f-a2a7-eea9e935378c",
	"4e520ee6-8f81-49fe-8dd7-d972e58bac4e",
	"1ba702b0-6781-4ef7-b6a4-c3612c1d0f7c"
]
```

**Códigos de Resposta**
| Código | Descrição
|:-:|-
| 201 | Conteúdos criados com sucesso
| 400 | Erro na requisição

---

## Criar conteúdo alternativo
`POST` /api/ai/content/{contentId}/new-content

**Exemplo de corpo de response**
```js
"07cd1867-05c2-431f-a2a7-eea9e935378c"
```

**Códigos de Resposta**
| Código | Descrição
|:-:|-
| 201 | Conteúdo criado com sucesso
| 400 | Erro na requisição

[Voltar ao início](#endpoints)

---

## Criar exercício
`POST` /api/ai/content/{contentId}/request-exercise-creation

**Exemplo de corpo de response**
```js
"07cd1867-05c2-431f-a2a7-eea9e935378c"
```

**Códigos de Resposta**
| Código | Descrição
|:-:|-
| 201 | Conteúdos criados com sucesso
| 400 | Erro na requisição

[Voltar ao início](#endpoints)

---

## Criar correção
`POST` /api/ai/exercise/{exerciseId}/request-correction

| Campo | Tipo | Obrigatório | Descrição
|:-------:|:------:|:-------------:|--
| exercises | list | sim | lista de objetos
| identification | int | sim | identificação do exercício respondido
| answer | string | sim | resposta enviada pelo usuário

**Exemplo de corpo do request**
```js
{
  "exercises": [
    {
      "identification": "1",
      "answer": "Console.WriteLine('Hello World')"
    },
    {
      "identification": "2",
      "answer": "e - Todas alternativas são verdadeiras"
    }
  ]
}
```

**Estrutura do objeto de response**

| Campo | Tipo | Obrigatório | Descrição
|:-------:|:------:|:-------------:|--
Id | string | Identificador da correção.
ExerciseId | string | Identificador do exercício corrigido.
OwnerId | string | Identificador do usuário que enviou as respostas.
CreatedDate | DateTime | Data e hora da criação da correção.
UpdatedDate | DateTime | Data e hora da última atualização da correção.
Corrections | list | Lista de objetos que representam cada item de correção.

**Estrutura do objeto da lista**
| Campo | Tipo | Obrigatório | Descrição
|:-------:|:------:|:-------------:|--
Identification | int | Identificação do item de correção.
Question | string | Pergunta associada ao exercício corrigido.
Complementation | list | Lista de complementações associadas ao exercício corrigido
Answer | string | Resposta enviada associada ao exercício corrigido.
IsCorrect | bool | Indica se a resposta está correta.
Feedback | string | Feedback associado a correção.

**Exemplo de corpo de response**
```js
{
    "Id": "07cd1867-05c2-431f-a2a7-eea9e935378c",
    "ExerciseId": "1ba702b0-6781-4ef7-b6a4-c3612c1d0f7c",
    "OwnerId": "4e520ee6-8f81-49fe-8dd7-d972e58bac4e",
    "CreatedDate": "2023-09-08T14:30:00Z",
    "UpdatedDate": "2023-09-08T14:32:00Z",
    "Corrections": [
        {
            "Identification": 1,
            "Question": "Escreva um código em C# para imprimir 'Hello World' no console.",
            "Complementation": [],
            "Answer": "Console.WriteLine('Hello World')",
            "IsCorrect": true,
            "Feedback": "Bom trabalho! Você respondeu corretamente."
        },
        {
            "Identification": 2,
            "Question": "Qual das seguintes alternativas é verdadeira?",
            "Complementation": [
                "a - 1x1 = 1",
				"b - 2x2 = 4",
				"c - 3x3 = 9",
				"d - 4x4 = 16",
				"e - Todas alternativas são verdadeiras"
            ],
            "Answer": "e - Todas alternativas são verdadeiras",
            "IsCorrect": true,
            "Feedback": "Correto! Você selecionou a resposta certa."
        }
    ]
}

```

**Códigos de Resposta**
| Código | Descrição
|:-:|-
| 201 | Conteúdos criados com sucesso
| 400 | Erro na requisição

[Voltar ao início](#endpoints)

---

# Configuração

## Criar configuração

`POST` /api/configurations

| Campo | Tipo | Obrigatório | Descrição
|:-------------:|:-----:|:--------:|------------------------------|
| summary      | object | sim        |Conteudo do resumo para ser configurado              |
| firstContent | object | sim        |Primeiro conteudo que será configurado             |
| newContent   | object | sim        |Novo conteudo que será configurado               |
| exercise     | object | sim        |Exercicio do conteudo que será configura              |
| correction   | object | sim        |Correção do exercicio do conteudo que será configura                |
| pendency     | object | sim        |Pendencia do conteudo que será configura              |
| initialInput | string | sim        |Parâmetro inicial |
| finalInput   | string | sim        |Parâmetro final       |

**Example of request body**
```js
{
  "summary": {
    "initialInput": "Crie um plano de estudos sobre o tema:",
    "finalInput": "Organizado em forma de sumário numerado"
  },
  "firstContent": {
    "initialInput": "Crie o conteúdo didático do seguinte assunto:",
    "finalInput": "Explicado aos detalhes com exemplos e analogias"
  },
  "newContent": {
    "initialInput": "Baseado no seguinte conteúdo:",
    "finalInput": "Crie outro exemplo, explicação, analogias para o mesmo assunto de uma maneira diferente"
  },
  "exercise": {
    "initialInput": "Baseado no seguinte conteúdo",
    "finalInput": "Crie um exercício de multipla escolha para validar o conhecimento sobre ele"
  },
  "correction": {
    "initialInput": "Baseado nas seguintes perguntas e respostas:",
    "finalInput": "Corrija cada um deles fornecendo um feedback"
  },
  "pendency": {
    "initialInput": "Para os seguintes exercícios respondidos errado:",
    "finalInput": "Crie outro diferente mas que valide o mesmo conhecimento"
  }
}
```

**Examplo de response**

**Response Codes**
| Code | Description
|:-:|-
| 201 | Contents created successfully
| 400 | Error in the request

[Voltar ao início](#endpoints)

---

## Atualizar configuração

`PUT` /api/configurations/{configurationId}

| Campo | Tipo | Obrigatório | Descrição
|:-------------:|:-----:|:--------:|------------------------------|
| summary      | object | sim        |Conteudo do resumo para ser configurado              |
| firstContent | object | sim        |Primeiro conteudo que será configurado             |
| newContent   | object | sim        |Novo conteudo que será configurado               |
| exercise     | object | sim        |Exercicio do conteudo que será configurado              |
| correction   | object | sim        |Correção do exercicio do conteudo que será configura                |
| pendency     | object | sim        |Pendencia do conteudo que será configura              |
| initialInput | string | sim        |Parâmetro inicial |
| finalInput   | string | sim        |Parâmetro final       |

**Example of request body**
```js
{
  "summary": {
    "initialInput": "Crie um plano de estudos sobre o tema:",
    "finalInput": "Organizado em forma de sumário numerado"
  },
  "firstContent": {
    "initialInput": "Crie o conteúdo didático do seguinte assunto:",
    "finalInput": "Explicado aos detalhes com exemplos e analogias"
  },
  "newContent": {
    "initialInput": "Baseado no seguinte conteúdo:",
    "finalInput": "Crie outro exemplo, explicação, analogias para o mesmo assunto de uma maneira diferente"
  },
  "exercise": {
    "initialInput": "Baseado no seguinte conteúdo",
    "finalInput": "Crie um exercício de multipla escolha para validar o conhecimento sobre ele"
  },
  "correction": {
    "initialInput": "Baseado nas seguintes perguntas e respostas:",
    "finalInput": "Corrija cada um deles fornecendo um feedback"
  },
  "pendency": {
    "initialInput": "Para os seguintes exercícios respondidos errado:",
    "finalInput": "Crie outro diferente mas que valide o mesmo conhecimento"
  }
}
```

**Response Codes**
| Code | Description
|:-:|-
| 204 | Updated with successful
| 400 | Error in the request

[Voltar ao início](#endpoints)

---

## Recuperar configuração

`GET` /api/configurations/{configurationId}

**Examplo de response**
```js
{
  "id": "078df11e-b8ec-420e-8924-d46e1c2c1c32",
  "summary": {
    "initialInput": "Crie um plano de estudos sobre o tema:",
    "finalInput": "Organizado em forma de sumário numerado"
  },
  "firstContent": {
    "initialInput": "Crie o conteúdo didático do seguinte assunto:",
    "finalInput": "Explicado aos detalhes com exemplos e analogias"
  },
  "newContent": {
    "initialInput": "Baseado no seguinte conteúdo:",
    "finalInput": "Crie outro exemplo, explicação, analogias para o mesmo assunto de uma maneira diferente"
  },
  "exercise": {
    "initialInput": "Baseado no seguinte conteúdo",
    "finalInput": "Crie um exercício de multipla escolha para validar o conhecimento sobre ele"
  },
  "correction": {
    "initialInput": "Baseado nas seguintes perguntas e respostas:",
    "finalInput": "Corrija cada um deles fornecendo um feedback"
  },
  "pendency": {
    "initialInput": "Para os seguintes exercícios respondidos errado:",
    "finalInput": "Crie outro diferente mas que valide o mesmo conhecimento"
  }
}
```

**Response Codes**
| Código | Descrição
|:-:|-
| 200 | Contents created successfully
| 400 | Error in the request

[Voltar ao início](#endpoints)

---

# Sumário

## Criar sumário

`POST` /api/summary
| Campo            | Tipo     | Obrigatório | Descrição                                 |
|:---------------:|:--------:|:----------:|-----------------------------------------|
| originId         | string   | sim        | Identificador de 36 caracteres do sumário de origem |
| ownerId          | string   | sim        | Identificador de 36 caracteres do proprietário |
| configurationId  | string   | sim        | Identificador de 36 caracteres da configuração |
| isAvailable      | boolean  | sim        | Retorna se o resumo está disponível |
| category         | string   | sim        | Conteúdo da categoria do resumo |
| subcategory      | string   | sim        | Conteúdo da subcategoria do resumo |
| theme            | string   | sim        | Tema do resumo |
| topics           | lista    | sim        | Tópicos do resumo |
| index            | string   | sim        | Índice do tópico |
| title            | int      | sim        | Título do tópico |
| description      | string   | sim        | Descrição do tópico |
| subtopics        | lista    | sim        | Subtópicos do tópico |
| index            | string   | sim        | Índice dos subtópicos |
| title            | string   | sim        | Título dos subtópicos |
| contentId        | string   | sim        | Identificador de 36 caracteres do conteúdo do usuário |

**Examplo de request**
```js
{
  "originId": "",
  "ownerId": "iacademy",
  "configurationId": "078df11e-b8ec-420e-8924-d46e1c2c1c32",
  "chatId": "chatcmpl-7wgTA8DEKzV1YJROS1dgFpx36Keg1",
  "createdDate": "2023-09-08T20:47:26.157Z",
  "updatedDate": "2023-09-11T21:07:09.195Z",
  "isAvaliable": false,
  "category": "Marketing",
  "subcategory": "Content Marketing",
  "theme": "Storytelling",
  "topics": [
    {
      "index": "1",
      "title": "Introdução ao Storytelling",
      "description": "Compreensão básica sobre o que é storytelling e sua importância",
      "subtopics": [
        {
          "index": "1.1",
          "title": "Definição de Storytelling",
          "contentId": ""
        },
        {
          "index": "1.2",
          "title": "A Importância do Storytelling",
          "contentId": ""
        },
        {
          "index": "1.3",
          "title": "Aplicações do Storytelling",
          "contentId": ""
        }
      ]
    }
  ]
}
```

**Response Codes**
| Code | Description
|:-:|-
| 201 | Contents created successfully
| 400 | Error in the request

[Voltar ao início](#endpoints)

---

## Criar matrícula

`POST` /api/summary/enroll

| Campo      | Tipo   | Obrigatório | Descrição                                           |
|:----------:|:------:|:----------:|---------------------------------------------------|
| summaryId  | string | sim        | Identificador de 36 caracteres do sumário        |
| ownerId    | string | sim        | Identificador de 36 caracteres do proprietário   |

**Examplo de request**
```js
{
  "summaryId": "078df11e-b8ec-420e-8924-d46e1c2c1c32",
  "ownerId": "string"
}
```


**Examplo de response**
```js
a448ba24-2df3-447b-a455-4a33e08ab2e9
```

**Response Codes**
| Code | Description
|:-:|-
| 201 | Contents created successfully
| 400 | Error in the request

[Voltar ao início](#endpoints)

---

## Atualizar sumário

`POST` /api/summary

| Campo            | Tipo     | Obrigatório | Descrição                                 |
|:---------------:|:--------:|:----------:|-----------------------------------------|
| originId         | string   | sim        | Identificador de 36 caracteres do sumário de origem |
| ownerId          | string   | sim        | Identificador de 36 caracteres do proprietário |
| configurationId  | string   | sim        | Identificador de 36 caracteres da configuração |
| isAvailable      | boolean  | sim        | Retorna se o resumo está disponível |
| category         | string   | sim        | Conteúdo da categoria do resumo |
| subcategory      | string   | sim        | Conteúdo da subcategoria do resumo |
| theme            | string   | sim        | Tema do resumo |
| topics           | lista    | sim        | Tópicos do resumo |
| index            | string   | sim        | Índice do tópico |
| title            | int      | sim        | Título do tópico |
| description      | string   | sim        | Descrição do tópico |
| subtopics        | lista    | sim        | Subtópicos do tópico |
| index            | string   | sim        | Índice dos subtópicos |
| title            | string   | sim        | Título dos subtópicos |
| contentId        | string   | sim        | Identificador de 36 caracteres do conteúdo do usuário |

**Example of request body**
```js
{
  "originId": "",
  "ownerId": "0c847ea2-4b8e-469c-83a4-41418fe02cf7",
  "configurationId": "078df11e-b8ec-420e-8924-d46e1c2c1c32",
  "chatId": "chatcmpl-7wgTA8DEKzV1YJROS1dgFpx36Keg1",
  "createdDate": "2023-09-08T20:47:26.157Z",
  "updatedDate": "2023-09-11T21:07:09.195Z",
  "isAvaliable": false,
  "category": "Marketing",
  "subcategory": "Content Marketing",
  "theme": "Storytelling",
  "topics": [
    {
      "index": "1",
      "title": "Introdução ao Storytelling",
      "description": "Compreensão básica sobre o que é storytelling e sua importância",
      "subtopics": [
        {
          "index": "1.1",
          "title": "Definição de Storytelling",
          "contentId": ""
        },
        {
          "index": "1.2",
          "title": "A Importância do Storytelling",
          "contentId": ""
        },
        {
          "index": "1.3",
          "title": "Aplicações do Storytelling",
          "contentId": ""
        }
      ]
    }
  ]
}
```

**Response Codes**
| Code | Description
|:-:|-
| 204 | Updated with successful
| 400 | Error in the request

[Voltar ao início](#endpoints)

---

## Recuperar sumário

`GET` /api/summary/{id}

**Examplo de response**
```js
{
  "id": "a448ba24-2df3-447b-a455-4a33e08ab2e9",
  "ownerId": "0c847ea2-4b8e-469c-83a4-41418fe02cf7",
  "configurationId": "078df11e-b8ec-420e-8924-79674248e3c0",
  "createdDate": "2023-09-08T20:47:26.157Z",
  "updatedDate": "2023-09-11T21:07:09.195Z",
  "isAvaliable": false,
  "category": "Tecnologia",
  "subcategory": "Programação",
  "theme": "C# básico",
  "topics": [
    {
      "index": "1",
      "title": "Introdução ao C#",
      "description": "Entenda o que é C#, quais suas características e onde é aplicado.",
      "subtopics": [
        {
          "index": "1.1",
          "title": "História e características do C#",
          "contentId": "5d218b82-e9ea-4b70-baf6-0ee8b1075155"
        },
        {
          "index": "1.2",
          "title": "Aplicações do C#",
          "contentId": "a8a6d08c-7b39-497b-9b57-aa6635ec1386"
        },
        {
          "index": "1.3",
          "title": "Ambiente de desenvolvimento",
          "contentId": "8f7f5470-487b-46de-939e-79674248e3c0"
        }
      ]
    },
    {
      "index": "2",
      "title": "Sintaxe Básica",
      "description": "Aprenda as regras de sintaxe, palavras-chave e como estruturar seu código em C#.",
      "subtopics": [
        {
          "index": "2.1",
          "title": "Sintaxe básica do C#",
          "contentId": "0c847ea2-4b8e-469c-83a4-41418fe02cf7"
        },
        {
          "index": "2.2",
          "title": "Palavras-chave do C#",
          "contentId": "88e47311-82b4-494a-9512-ea5e549d92ca"
        },
        {
          "index": "2.3",
          "title": "Estrutura de um programa C#",
          "contentId": "e84f472e-22f4-4636-bd58-bd7c22ed920e"
        }
      ]
    }
  ]
}
```

**Response Codes**
| Code | Description
|:-:|-
| 200 | Contents created successfully
| 400 | Error in the request

[Voltar ao início](#endpoints)

---

## Recuperar sumário por categoria

`GET` /api/summary/category/{category}

**Examplo de response**
```js
[
{
  "_id": "14f595ff-04e9-4cb4-913c-1f2917252113",
  "originId": "",
  "ownerId": "iacademy",
  "configurationId": "078df11e-b8ec-420e-8924-d46e1c2c1c32",
  "createdDate": "2023-09-08T20:56:03.499Z",
  "updatedDate": "0001-01-01T00:00:01.001Z",
  "isAvaliable": false,
  "category": "Tecnologia",
  "subcategory": "Container",
  "theme": "Docker",
  "topics": [
    {
      "index": "1",
      "title": "Introdução ao Docker",
      "description": "Compreensão básica do Docker e sua instalação.",
      "subtopics": [
        {
          "index": "1.1",
          "title": "O que é Docker",
          "contentId": "c83329b9-8439-4e66-8094-536d6ae678e2"
        },
        {
          "index": "1.2",
          "title": "Instalação do Docker",
          "contentId": "f61885f8-9d01-47a5-84ea-5f37263985bc"
        }
      ]
    }
  ]
}
]
```

**Response Codes**
| Code | Description
|:-:|-
| 200 | Contents created successfully
| 400 | Error in the request

[Voltar ao início](#endpoints)

---

## Recuperar sumário por categoria e subcategoria

`GET` /api/summary/category/{category}/subcategory/{subcategory}

**Examplo de response**
```js
[
{
  "_id": "14f595ff-04e9-4cb4-913c-1f2917252113",
  "originId": "",
  "ownerId": "iacademy",
  "configurationId": "078df11e-b8ec-420e-8924-d46e1c2c1c32",
  "createdDate": "2023-09-08T20:56:03.499Z",
  "updatedDate": "0001-01-01T00:00:01.001Z",
  "isAvaliable": false,
  "category": "Tecnologia",
  "subcategory": "Container",
  "theme": "Docker",
  "topics": [
    {
      "index": "1",
      "title": "Introdução ao Docker",
      "description": "Compreensão básica do Docker e sua instalação.",
      "subtopics": [
        {
          "index": "1.1",
          "title": "O que é Docker",
          "contentId": "c83329b9-8439-4e66-8094-536d6ae678e2"
        },
        {
          "index": "1.2",
          "title": "Instalação do Docker",
          "contentId": "f61885f8-9d01-47a5-84ea-5f37263985bc"
        }
      ]
    }
  ]
}
]
```

**Response Codes**
| Code | Description
|:-:|-
| 200 | Contents created successfully
| 400 | Error in the request

[Voltar ao início](#endpoints)

---

## Recuperar sumário por identificação do dono

`GET` /api/summary/owner/{ownerId}

**Examplo de response**
```js
[
{
  "_id": "14f595ff-04e9-4cb4-913c-1f2917252113",
  "originId": "",
  "ownerId": "iacademy",
  "configurationId": "078df11e-b8ec-420e-8924-d46e1c2c1c32",
  "createdDate": "2023-09-08T20:56:03.499Z",
  "updatedDate": "0001-01-01T00:00:01.001Z",
  "isAvaliable": false,
  "category": "Tecnologia",
  "subcategory": "Container",
  "theme": "Docker",
  "topics": [
    {
      "index": "1",
      "title": "Introdução ao Docker",
      "description": "Compreensão básica do Docker e sua instalação.",
      "subtopics": [
        {
          "index": "1.1",
          "title": "O que é Docker",
          "contentId": "c83329b9-8439-4e66-8094-536d6ae678e2"
        },
        {
          "index": "1.2",
          "title": "Instalação do Docker",
          "contentId": "f61885f8-9d01-47a5-84ea-5f37263985bc"
        }
      ]
    }
  ]
}
]
```

**Response Codes**
| Code | Description
|:-:|-
| 200 | Contents created successfully
| 400 | Error in the request

[Voltar ao início](#endpoints)

---

## Recuperar sumário por subcategoria

`GET` /api/summary/subcategory/{subcategory}

**Examplo de response**
```js
[
{
  "_id": "14f595ff-04e9-4cb4-913c-1f2917252113",
  "originId": "",
  "ownerId": "iacademy",
  "configurationId": "078df11e-b8ec-420e-8924-d46e1c2c1c32",
  "createdDate": "2023-09-08T20:56:03.499Z",
  "updatedDate": "0001-01-01T00:00:01.001Z",
  "isAvaliable": false,
  "category": "Tecnologia",
  "subcategory": "Container",
  "theme": "Docker",
  "topics": [
    {
      "index": "1",
      "title": "Introdução ao Docker",
      "description": "Compreensão básica do Docker e sua instalação.",
      "subtopics": [
        {
          "index": "1.1",
          "title": "O que é Docker",
          "contentId": "c83329b9-8439-4e66-8094-536d6ae678e2"
        },
        {
          "index": "1.2",
          "title": "Instalação do Docker",
          "contentId": "f61885f8-9d01-47a5-84ea-5f37263985bc"
        }
      ]
    }
  ]
}
]
```

**Response Codes**
| Code | Description
|:-:|-
| 200 | Contents created successfully
| 400 | Error in the request

[Voltar ao início](#endpoints)

---

# Conteúdo

## Recuperar conteúdo

`GET` /api/content/{contentId}

**Examplo de response**
```js
{
  "id": "a8a6d08c-7b39-497b-9b57-aa6635ec1386",
  "ownerId": "iacademy",
  "summaryId": "a448ba24-2df3-447b-a455-4a33e08ab2e9",
  "configurationId": "078df11e-b8ec-420e-8924-d46e1c2c1c32",
  "exerciseId": "e77ab4de-afca-4ed6-a323-2ec9975f062b"
  "theme": "C# básico",
  "subtopicIndex": "1.2",
  "title": "Aplicações do C#",
  "body": [
    {
      "content": "C# é amplamente usado para desenvolver aplicativos de desktop, jogos, aplicativos web e serviços web. Ele é usado no desenvolvimento de jogos com o Unity game engine. Além disso, C# é a linguagem recomendada para o desenvolvimento de aplicativos para dispositivos que executam o sistema operacional Windows.",
      "createdDate": "2023-09-08T20:56:03.499Z",
      "disabledDate": "0001-01-01T00:00:01.001Z"
    }
  ],
  "createdDate": "2023-09-08T20:47:26.157Z",
  "updatedDate": "2023-09-11T21:07:09.195Z"
}
```

**Response Codes**
| Code | Description
|:-:|-
| 200 | Contents created successfully
| 400 | Error in the request

[Voltar ao início](#endpoints)

---

## Recuperar todos conteúdos

`GET` /api/content/summary/{summaryId}

**Examplo de response**
```js
{
  "id": "a8a6d08c-7b39-497b-9b57-aa6635ec1386",
  "ownerId": "iacademy",
  "summaryId": "a448ba24-2df3-447b-a455-4a33e08ab2e9",
  "configurationId": "078df11e-b8ec-420e-8924-d46e1c2c1c32",
  "exerciseId": "e77ab4de-afca-4ed6-a323-2ec9975f062b"
  "theme": "C# básico",
  "subtopicIndex": "1.2",
  "title": "Aplicações do C#",
  "body": [
    {
      "content": "C# é amplamente usado para desenvolver aplicativos de desktop, jogos, aplicativos web e serviços web. Ele é usado no desenvolvimento de jogos com o Unity game engine. Além disso, C# é a linguagem recomendada para o desenvolvimento de aplicativos para dispositivos que executam o sistema operacional Windows.",
      "createdDate": "2023-09-08T20:56:03.499Z",
      "disabledDate": "0001-01-01T00:00:01.001Z"
    }
  ],
  "createdDate": "2023-09-08T20:47:26.157Z",
  "updatedDate": "2023-09-11T21:07:09.195Z"
}
]
```

**Response Codes**
| Code | Description
|:-:|-
| 200 | Contents created successfully
| 400 | Error in the request

[Voltar ao início](#endpoints)

---

## Atualizar conteúdo

`PUT` /api/content/{contentId}

| Campo           | Tipo   | Obrigatório | Descrição                  |
|:--------------:|:------:|:----------:|--------------------------|
| ownerId         | string | sim        | Identificador de 36 caracteres do proprietário |
| summaryId       | string | sim        | Identificador de 36 caracteres do sumário |
| configurationId | string | sim        | Identificador de 36 caracteres da configuração |
| theme           | string | sim        | Tema do Conteúdo |
| subtopicIndex   | string | sim        | Índice do subtema do tema |
| title           | string | sim        | Título do Tema |
| body            | lista  | sim        | Lista de datas |
| content         | string | sim        | Texto do usuário referente ao conteúdo |
| createdDate     | string | sim        | Data em que o conteúdo foi criado |
| disabledDate    | string | sim        | Data em que o conteúdo foi desabilitado |
| updatedDate     | string | sim        | Data em que o conteúdo foi atualizado |

**Example of request body**
```js
{
  "_id": "6cdbf47a-e5ce-46e4-a2e2-955da7bbf175",
  "ownerId": "iacademy",
  "summaryId": "f0515d85-308c-49ec-b815-1d10c23e113e",
  "configurationId": "078df11e-b8ec-420e-8924-d46e1c2c1c32",
  "exerciceId": "",
  "theme": "Python",
  "subtopicIndex": "1.1",
  "title": "Introdução ao Python",
  "body": [
    {
      "content": "Python é uma linguagem de programação de alto nível, interpretada, de script, imperativa, orientada a objetos, funcional, de tipagem dinâmica e forte. Foi criada por Guido van Rossum em 1991. Python é uma linguagem de propósito geral com foco na legibilidade do código, tendo uma sintaxe que permite aos programadores expressar conceitos em menos linhas de código do que seria possível em linguagens como C++ ou Java.",
      "createdDate": "2023-09-04T21:32:32.900Z",
      "disabledDate": "0001-01-01T00:00:01.001Z"
    }
  ],
  "createdDate": "2023-09-04T21:32:32.900Z",
  "updatedDate": "2023-09-04T21:32:32.900Z",
}
```

**Response Codes**
| Code | Description
|:-:|-
| 204 | Updated with successful
| 400 | Error in the request

[Voltar ao início](#endpoints)

---

## Atualizar todos conteúdos

`PUT` /api/content/update-all/summary/{summaryId}

| Campo           | Tipo   | Obrigatório | Descrição                  |
|:--------------:|:------:|:----------:|--------------------------|
| ownerId         | string | sim        | Identificador de 36 caracteres do proprietário |
| summaryId       | string | sim        | Identificador de 36 caracteres do sumário |
| configurationId | string | sim        | Identificador de 36 caracteres da configuração |
| theme           | string | sim        | Tema do Conteúdo |
| subtopicIndex   | string | sim        | Índice do subtema do tema |
| title           | string | sim        | Título do Tema |
| body            | lista  | sim        | Lista de datas |
| content         | string | sim        | Texto do usuário referente ao conteúdo |
| createdDate     | string | sim        | Data em que o conteúdo foi criado |
| disabledDate    | string | sim        | Data em que o conteúdo foi desabilitado |
| updatedDate     | string | sim        | Data em que o conteúdo foi atualizado |


**Example of request body**
```js
[
{
  "_id": "6cdbf47a-e5ce-46e4-a2e2-955da7bbf175",
  "ownerId": "iacademy",
  "summaryId": "f0515d85-308c-49ec-b815-1d10c23e113e",
  "configurationId": "078df11e-b8ec-420e-8924-d46e1c2c1c32",
  "exerciceId": "",
  "theme": "Python",
  "subtopicIndex": "1.1",
  "title": "Introdução ao Python",
  "body": [
    {
      "content": "Python é uma linguagem de programação de alto nível, interpretada, de script, imperativa, orientada a objetos, funcional, de tipagem dinâmica e forte. Foi criada por Guido van Rossum em 1991. Python é uma linguagem de propósito geral com foco na legibilidade do código, tendo uma sintaxe que permite aos programadores expressar conceitos em menos linhas de código do que seria possível em linguagens como C++ ou Java.",
      "createdDate": "2023-09-04T21:32:32.900Z",
      "disabledDate": "0001-01-01T00:00:01.001Z"
    }
  ],
  "createdDate": "2023-09-04T21:32:32.900Z",
  "updatedDate": "2023-09-04T21:32:32.900Z",
}
]
```

**Response Codes**
| Code | Description
|:-:|-
| 204 | Updated with successful
| 400 | Error in the request

[Voltar ao início](#endpoints)

---

## Criar um conteúdo

`POST` /api/content

| Campo | Tipo | Obrigatório | Descrição
|:-------------:|:-----:|:--------:|------------------------------|
| ownerId      | string | sim       |Identificador de 36 caracteres do usuário |
| summaryId | string | sim       |Identificador de 36 caracteres do sumário  |
| configurationId   | string | sim       |Identificador de 36 caracteres da configuração    |
| theme     | string | sim       |Tema do Conteúdo  |
| subtopicIndex   | string | sim       |Índice do subtema do tema  |
| title     | string | sim       |Título do Tema |
| body | list   | sim     | Lista de datas e o conteúdo do tema |
| content | string   | sim     | Texto do usuário referente ao conteúdo |
| createdDate        | object | sim     | Data em que o conteúdo foi criado |
| disabledDate        | object | sim     | Data em que o conteúdo foi desabilitado |


**Example of request body**
```js
{
  "ownerId": "1ba702b0-6781-4ef7-b6a4-c3612c1d0f7c",
  "summaryId": "f0515d85-308c-49ec-b815-1d10c23e113e",
  "configurationId": "078df11e-b8ec-420e-8924-d46e1c2c1c32",
  "exerciceId": "",
  "theme": "Python",
  "subtopicIndex": "1.1",
  "title": "Introdução ao Python",
  "body": [
    {
      "content": "Python é uma linguagem de programação de alto nível, interpretada, de script, imperativa, orientada a objetos, funcional, de tipagem dinâmica e forte. Foi lançada por Guido van Rossum em 1991. É uma linguagem de fácil aprendizado e possui uma sintaxe clara e legível. É muito utilizada em análise de dados, inteligência artificial, aprendizado de máquina e desenvolvimento web.",
      "createdDate": "2023-09-04T21:32:32.900Z",
      "disabledDate": "0001-01-01T00:00:01.001Z"
    }
}
```

**Examplo de response**
```js
078df11e-b8ec-420e-8924-d46e1c2c1c32
```

**Response Codes**
| Code | Description
|:-:|-
| 201 | Contents created successfully
| 400 | Error in the request

[Voltar ao início](#endpoints)

---

## Criar vários conteúdos

`POST` /api/content/save-all

| Campo | Tipo | Obrigatório | Descrição
|:-------------:|:-----:|:--------:|------------------------------|
| ownerId      | string | sim       |Identificador de 36 caracteres do usuário |
| summaryId | string | sim       |Identificador de 36 caracteres do sumário  |
| configurationId   | string | sim       |Identificador de 36 caracteres da configuração    |
| theme     | string | sim       |Tema do Conteúdo  |
| subtopicIndex   | string | sim       |Índice do subtema do tema  |
| title     | string | sim       |Título do Tema |
| body | list   | sim     | Lista de datas e o conteúdo do tema |
| content | string   | sim     | Texto do usuário referente ao conteúdo |
| createdDate        | object | sim     | Data em que o conteúdo foi criado |
| disabledDate        | object | sim     | Data em que o conteúdo foi desabilitado |

**Example of request body**
```js
[
{
  "ownerId": "iacademy",
  "summaryId": "f0515d85-308c-49ec-b815-1d10c23e113e",
  "configurationId": "078df11e-b8ec-420e-8924-d46e1c2c1c32",
  "exerciceId": "",
  "theme": "Python",
  "subtopicIndex": "1.1",
  "title": "Introdução ao Python",
  "body": [
    {
      "content": "Python é uma linguagem de programação de alto nível, interpretada, de script, imperativa, orientada a objetos, funcional, de tipagem dinâmica e forte. Foi criada por Guido van Rossum durante 1985- 1990. Python é projetado para ser fácil de ler enquanto ainda fornece um grande controle ao programador. Uma característica notável de Python é o seu indentação de blocos de código, que é feita para aumentar a legibilidade do código.",
      "createdDate": "2023-09-08T20:56:03.499Z",
      "disabledDate": "0001-01-01T00:00:01.001Z"
    }
  ]
}
]
```

**Examplo de response**
```js
[
"078df11e-b8ec-420e-8924-d46e1c2c1c32"
]
```

**Response Codes**
| Code | Description
|:-:|-
| 201 | Contents created successfully
| 400 | Error in the request

[Voltar ao início](#endpoints)

---

# Exercício

## Recuperar exercício

`GET` /api/exercise/{exerciseId}

**Examplo de response**
```js
{
  "id": "ccda969b-5124-4116-baa1-3002b4b177d0",
  "ownerId": "iacademy",
  "contentId": "728301ba-8d48-4c90-8bc3-4863fff54562",
  "correctionId": "",
  "configurationId": "078df11e-b8ec-420e-8924-d46e1c2c1c32",
  "status": "WaitingToDo",
  "type": "Default",
  "sendedAt": "0001-01-01T00:00:01.001Z",
  "topicIndex": "2.1",
  "title": "Listas, Tuplas e Dicionários",
  "exercises": [
    {
      "identification": 1,
      "type": "Code",
      "question": "Crie uma lista em Python com os seguintes elementos: 'Python', 'é', 'interessante'. Em seguida, altere o terceiro elemento para 'incrível'.",
      "complementation": [],
      "answer": "lista = [\"Python\", \"é\", \"interessante\"] \n lista[2] = \"incrível\""
    },
    {
      "identification": 2,
      "type": "SingleChoice",
      "question": "O que acontecerá se você tentar alterar um elemento de uma tupla em Python?",
      "complementation": [
        "a - O elemento será alterado com sucesso.",
        "b - O Python lançará um erro.",
        "c - O Python irá ignorar a tentativa de alteração e manterá a tupla original.",
        "d - A tupla será convertida em uma lista.",
        "e - Nenhuma das alternativas."
      ],
      "answer": "e - Nenhuma das alternativas."
    },
    {
      "identification": 3,
      "type": "Code",
      "question": "Crie um dicionário em Python que armazene os seguintes pares chave-valor: 'nome' - 'Ana', 'idade' - 25, 'cidade' - 'São Paulo'.",
      "complementation": [],
      "answer": "banana"
    }
  ]
}
```

**Response Codes**
| Code | Description
|:-:|-
| 200| Contents created successfully
| 400 | Error in the request

[Voltar ao início](#endpoints)

---

## Recuperar vários exercícios

`GET` /api/exercise/owner/{ownerId}/type/{type}

**Examplo de response**
```js
[
{
  "id": "ccda969b-5124-4116-baa1-3002b4b177d0",
  "ownerId": "iacademy",
  "contentId": "728301ba-8d48-4c90-8bc3-4863fff54562",
  "correctionId": "",
  "configurationId": "078df11e-b8ec-420e-8924-d46e1c2c1c32",
  "status": "WaitingToDo",
  "type": "Default",
  "sendedAt": "0001-01-01T00:00:01.001Z",
  "topicIndex": "2.1",
  "title": "Listas, Tuplas e Dicionários",
  "exercises": [
    {
      "identification": 1,
      "type": "Code",
      "question": "Crie uma lista em Python com os seguintes elementos: 'Python', 'é', 'interessante'. Em seguida, altere o terceiro elemento para 'incrível'.",
      "complementation": [],
      "answer": "lista = [\"Python\", \"é\", \"interessante\"] \n lista[2] = \"incrível\""
    },
    {
      "identification": 2,
      "type": "SingleChoice",
      "question": "O que acontecerá se você tentar alterar um elemento de uma tupla em Python?",
      "complementation": [
        "a - O elemento será alterado com sucesso.",
        "b - O Python lançará um erro.",
        "c - O Python irá ignorar a tentativa de alteração e manterá a tupla original.",
        "d - A tupla será convertida em uma lista.",
        "e - Nenhuma das alternativas."
      ],
      "answer": "e - Nenhuma das alternativas."
    },
    {
      "identification": 3,
      "type": "Code",
      "question": "Crie um dicionário em Python que armazene os seguintes pares chave-valor: 'nome' - 'Ana', 'idade' - 25, 'cidade' - 'São Paulo'.",
      "complementation": [],
      "answer": "banana"
    }
  ]
}
]
```

**Response Codes**
| Code | Description
|:-:|-
| 200 | Contents created successfully
| 400 | Error in the request

[Voltar ao início](#endpoints)

---

## Atualizar exercício

`PUT` /api/exercise/{exerciseId} 

| Campo           | Tipo     | Obrigatório | Descrição                             |
|:--------------:|:--------:|:----------:|-------------------------------------|
| ownerId         | string   | sim        | Identificador de 36 caracteres do proprietário |
| correctionId    | string   | sim        | Identificador de 36 caracteres da correção |
| configurationId | string   | sim        | Identificador de 36 caracteres da configuração |
| status          | int      | sim        | Número de status do exercício |
| type            | int      | sim        | Número do tipo de exercício |
| sendedAt        | objeto   | sim        | Data de atualização do exercício |
| topicIndex      | string   | sim        | Índice do tópico do exercício |
| title           | string   | sim        | Título do exercício |
| exercises       | lista    | sim        | Conteúdo do exercício |
| identification  | int      | sim        | Identificador de atualização do patch |
| question        | string   | sim        | Conteúdo da pergunta |
| complementation | lista    | sim        | Complemento para a pergunta |
| answer          | string   | não        | Conteúdo da resposta (opcional) |

**Example of request body**
```js
{
  "ownerId": "1ba702b0-6781-4ef7-b6a4-c3612c1d0f7c",
  "contentId": "ba017389-6e79-470e-8412-a86aa93e910d",
  "correctionId": "",
  "configurationId": "078df11e-b8ec-420e-8924-d46e1c2c1c32",
  "status": "WaitingToDo",
  "type": "Default",
  "sendedAt": "0001-01-01T00:00:01.001Z",
  "topicIndex": "2.2",
  "title": "Funções e Módulos",
  "exercises": [
    {
      "identification": 2,
      "type": "SingleChoice",
      "question": "O que é um módulo em Python?",
      "complementation": [
        "a - Um bloco de código que é executado automaticamente ao iniciar o programa.",
        "b - Um arquivo contendo definições e instruções Python que podem ser importadas em outros scripts.",
        "c - Uma função que pode ser chamada várias vezes durante a execução do programa.",
        "d - Um tipo especial de variável que pode armazenar múltiplos valores.",
        "e - Um recurso usado para manipular e processar strings."
      ],
      "answer": ""
    }
  ]
}
```

**Response Codes**
| Code | Description
|:-:|-
| 204 | Updated with successful
| 400 | Error in the request

[Voltar ao início](#endpoints)

---

## Criar exercício

`POST` /api/exercise

**Exemplo de request**
```js
{
  "ownerId": "c4e4646d-15c6-4703-8717-2008dfc8917b",
  "contentId": "0488f15c-2eb9-4f9b-bbc3-acbc62bec362",
  "correctionId": "",
  "configurationId": "078df11e-b8ec-420e-8924-d46e1c2c1c32",
  "status": "WaitingToDo",
  "type": "Default",
  "sendedAt": "0001-01-01T00:00:01.001Z",
  "topicIndex": "1.1",
  "title": "História e características do C#",
  "exercises": [
    {
      "identification": 1,
      "type": "Code",
      "question": "Escreva um código em C# que declare uma variável do tipo inteiro, atribua um valor a ela e, em seguida, imprima esse valor na saída padrão.",
      "complementation": [],
      "answer": ""
    },
    {
      "identification": 2,
      "type": "SingleChoice",
      "question": "Qual das seguintes afirmações é verdadeira sobre a linguagem de programação C#?",
      "complementation": [
        "a - C# não é uma linguagem orientada a objetos.",
        "b - C# foi desenvolvida pela Apple.",
        "c - C# não permite o desenvolvimento de aplicações seguras e robustas.",
        "d - C# foi desenvolvida pela Microsoft como parte de sua plataforma .NET.",
        "e - C# não é compatível com o .NET Framework."
      ],
      "answer": ""
    },
    {
      "identification": 3,
      "type": "MultipleChoice",
      "question": "Marque as opções que são características da linguagem de programação C#.",
      "complementation": [
        "a - C# é orientada a objetos.",
        "b - C# combina os melhores recursos de linguagens como C++ e Java.",
        "c - C# não permite a criação de aplicações seguras e robustas.",
        "d - C# é uma linguagem de programação moderna desenvolvida pela Microsoft como parte de sua plataforma .NET.",
        "e - C# não é compatível com o .NET Framework."
      ],
      "answer": ""
    }
  ]
}
```

**Examplo de response**
```js
078df11e-b8ec-420e-8924-d46e1c2c1c32
```

**Response Codes**
| Code | Description
|:-:|-
| 201 | Contents created successfully
| 400 | Error in the request

[Voltar ao início](#endpoints)

---

# Correção

## Recuperar correção

`GET` /api/correction/{correctionId}

**Examplo de response**
```js
{
  "_id": "0488f15c-2eb9-4f9b-bbc3-acbc62bec362",
  "exerciseId": "440c45bb-6b28-48b4-8784-2ab22e9e6e69",
  "ownerId": "c4e4646d-15c6-4703-8717-2008dfc8917b",
  "createdDate": "2023-09-11T23:29:47.436Z",
  "updatedDate": "2023-09-11T23:29:47.436Z",
  "corrections": [
    {
      "identification": 1,
      "question": "Qual tipo de dado primitivo em C# é usado para representar um único caractere?",
      "complementation": [
        "a - int",
        "b - bool",
        "c - char",
        "d - byte",
        "e - double"
      ],
      "answer": "c - char",
      "isCorrect": true,
      "feedback": "A resposta está correta. O tipo de dado primitivo 'char' em C# é usado para representar um único caractere."
    },
    {
      "identification": 2,
      "question": "Verdadeiro ou Falso: O tipo de dados 'long' em C# é usado para representar números inteiros grandes e o 'short' para representar números inteiros pequenos.",
      "complementation": [
        "a - Verdadeiro",
        "b - Falso"
      ],
      "answer": "a - Verdadeiro-",
      "isCorrect": true,
      "feedback": "A resposta está correta. O tipo de dados 'long' em C# é usado para representar números inteiros grandes e o 'short' para representar números inteiros pequenos."
    },
    {
      "identification": 3,
      "question": "Escreva um código em C# que declare variáveis para todos os tipos de dados primitivos mencionados e os atribua com valores.",
      "complementation": [],
      "answer": "123",
      "isCorrect": false,
      "feedback": "A resposta está incorreta. Foi solicitado um código em C# que declare variáveis para todos os tipos de dados primitivos mencionados e os atribua com valores, portanto, a resposta '123' não é valida."
    }
  ]
}
```

**Response Codes**
| Code | Description
|:-:|-
| 200 | Contents created successfully
| 400 | Error in the request

[Voltar ao início](#endpoints)

---

## Atualizar correção

`PUT`/api/correction/{correctionId}

| Campo           | Tipo     | Obrigatório | Descrição                             |
|:--------------:|:--------:|:----------:|-------------------------------------|
| ownerId         | string   | sim        | Identificador de 36 caracteres do proprietário |
| exerciseId      | string   | sim        | Identificador de 36 caracteres do exercício |
| corrections     | lista    | sim        | Lista com informações de correção   |
| identification  | int      | sim        | Identificador de atualização do patch |
| question        | string   | sim        | Conteúdo da pergunta                 |
| complementation | lista    | sim        | Complemento para a pergunta          |
| answer          | string   | sim        | Conteúdo da resposta                 |
| isCorrect       | boolean  | sim        | Se a resposta estiver correta, retorna verdadeiro, caso contrário, falso |
| feedback        | string   | sim        | Conteúdo do feedback do questionário  |


**Example of request body**
```js
{
  "ownerId": "iacademye362a376-58d2-451e-af75-38caeb1e9966",
  "exerciseId": "stbf41c41e-3e31-40a2-917e-11222102b298ring",
  "corrections": [
    {
      "identification": 1,
      "question": "O que é uma classe em Python na programação orientada a objetos?",
      "complementation": [
        "a - Uma função que realiza uma tarefa específica",
        "b - Um tipo de variável",
        "c - Um modelo que define os atributos e métodos que seus objetos terão",
        "d - Um tipo de loop",
        "e - Uma biblioteca Python"
      ],
      "answer": "",
      "isCorrect": false,
      "feedback": "A resposta correta é a opção 'c'. Uma classe em Python na programação orientada a objetos é um modelo que define os atributos e métodos que seus objetos terão."
    }
  ]
}
```

**Response Codes**
| Code | Description
|:-:|-
| 204 | Updated with successful
| 400 | Error in the request

[Voltar ao início](#endpoints)

---

[Clique aqui](../README.md) para voltar para a documentação principal.