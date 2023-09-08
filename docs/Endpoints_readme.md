## Endpoints
- AI
  - [Criar Resumo](#criar-resumo)
  - [Criar conteúdos do Tópico](#criar-conteudos-do-tópico)
  - [Criar conteúdo alternativo](#criar-conteudo-alternativo)
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
  - [Recupear conteúdo](#recuperar-conteudo)
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
{
    [
		"07cd1867-05c2-431f-a2a7-eea9e935378c",
		"4e520ee6-8f81-49fe-8dd7-d972e58bac4e",
		"1ba702b0-6781-4ef7-b6a4-c3612c1d0f7c"
	]
}
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
{
    "id": "07cd1867-05c2-431f-a2a7-eea9e935378c"
}
```

**Códigos de Resposta**
| Código | Descrição
|:-:|-
| 201 | Conteúdo criado com sucesso
| 400 | Erro na requisição

---

## Criar exercício
`POST` /api/ai/content/{contentId}/request-exercise-creation

**Exemplo de corpo de response**
```js
{
    "id": "07cd1867-05c2-431f-a2a7-eea9e935378c"
}
```

**Códigos de Resposta**
| Código | Descrição
|:-:|-
| 201 | Conteúdos criados com sucesso
| 400 | Erro na requisição

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

---