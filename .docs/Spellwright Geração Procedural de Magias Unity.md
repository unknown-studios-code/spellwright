# **Spellwright: Um Documento de Arquitetura para a Geração Procedural de Magias com IA e Unity ECS**

## **Introdução: A Confluência da Linguagem Natural e do Design Orientado a Dados**

### **Visão do Projeto: Definindo o Spellwright**

O projeto Spellwright representa uma ambição fundamental: capacitar designers de jogos e, potencialmente, até mesmo jogadores a criar habilidades mágicas únicas através de linguagem simples e descritiva. Esta iniciativa transcende a mera novidade tecnológica; ela propõe um fluxo de trabalho de próxima geração que une o poder criativo e intuitivo da linguagem natural com a execução determinística e de alto desempenho de um motor de jogo orientado a dados. O objetivo é criar um sistema onde a frase "uma bola de fogo que persegue inimigos e explode em uma chuva de brasas" não seja apenas uma anotação de design, mas uma entrada direta que o motor pode compilar e executar. Este documento serve como o projeto arquitetônico definitivo para realizar essa visão.

### **O Desafio Central: Unindo a Intenção Humana e a Execução da Máquina**

O problema técnico central que o Spellwright visa resolver é a tradução da natureza ambígua e rica em contexto da linguagem humana para o formato rígido e puramente de dados exigido pelo Entity Component System (ECS) da Unity. A linguagem humana é flexível, contextual e muitas vezes imprecisa. Em contraste, um sistema de alto desempenho como o ECS exige dados perfeitamente estruturados, contíguos na memória e desprovidos de lógica para permitir o processamento massivamente paralelo.1 A falha em preencher essa lacuna resultaria em um sistema que é imprevisível, propenso a erros e incapaz de aproveitar os benefícios de desempenho que o ECS promete. Este relatório apresenta uma solução arquitetônica robusta para enfrentar esse desafio de frente.

### **Pilares Arquitetônicos**

A arquitetura do Spellwright se apoia em três pilares fundamentais, cada um abordando uma faceta distinta do desafio central. Este documento detalhará cada um deles em profundidade:

1. **Um Runtime Orientado a Dados:** Construído sobre o Data-Oriented Technology Stack (DOTS) da Unity, este pilar garante que a execução de cada magia, desde o seu movimento até o seu impacto, seja processada com desempenho e escalabilidade incomparáveis. A escolha pelo DOTS não é uma preferência, mas um requisito estratégico para lidar com a complexidade e a escala de um sistema verdadeiramente procedural.3
2. **Uma Camada de Interpretação por IA:** Utilizando um Large Language Model (LLM) com restrições, esta camada atua como um "compilador" para a linguagem natural. Sua função é analisar a intenção do usuário e traduzi-la em uma representação de dados estruturada e inequívoca, definida por um esquema JSON rigoroso.
3. **Um Sistema de Montagem Procedural:** Este pilar serve como a ponte entre o mundo abstrato da IA e o mundo concreto do motor de jogo. Ele recebe a saída estruturada da IA e a utiliza para "montar" dinamicamente as entidades e componentes ECS que constituem a magia, preparando-a para execução em tempo real.

Juntos, esses pilares formam uma arquitetura coesa projetada para transformar a criatividade humana em experiências de jogo performáticas e dinâmicas.

## **Seção 1: O Motor da Magia \- Uma Fundação Orientada a Dados com Unity ECS**

A base sobre a qual o Spellwright é construído é fundamental para o seu sucesso. A escolha do Data-Oriented Technology Stack (DOTS) da Unity não é uma mera decisão de implementação, mas uma escolha arquitetônica estratégica. É a única abordagem que pode fornecer o desempenho, a escalabilidade e o determinismo necessários para um sistema que pode gerar e simular um número virtualmente ilimitado de efeitos mágicos complexos simultaneamente.

### **1.1. Justificativa para uma Abordagem Orientada a Dados (O "Porquê")**

A mudança do paradigma de programação tradicional orientado a objetos (OOP) para o design orientado a dados (DOD) é a decisão mais impactante na arquitetura do Spellwright. Esta seção detalha por que essa mudança é essencial.

#### **Além das Limitações da Orientação a Objetos**

A abordagem tradicional da Unity, baseada em GameObjects e MonoBehaviours, é intuitiva e tem servido bem aos desenvolvedores por anos. No entanto, sua natureza baseada em classes e pesada em referências leva a um problema fundamental de desempenho em grande escala: a alocação de memória dispersa. Cada GameObject e seus componentes são objetos alocados individualmente no heap, o que significa que seus dados podem estar espalhados por toda a memória RAM.1

Para uma CPU moderna, isso é um desastre de desempenho. As CPUs dependem de caches—pequenas e rápidas áreas de memória—para operar eficientemente. Quando a CPU precisa de dados, ela os busca na RAM e carrega não apenas os dados solicitados, mas também um bloco contíguo de memória ao redor deles no cache. Em um layout de memória orientado a objetos, os dados adjacentes são frequentemente irrelevantes, levando a "cache misses" constantes. Cada cache miss força a CPU a esperar ociosamente enquanto busca novos dados da RAM, um processo ordens de magnitude mais lento.2 Em um sistema como o Spellwright, com potencialmente milhares de projéteis, partículas e efeitos de status ativos simultaneamente, o custo acumulado de cache misses limitaria severamente a complexidade e a escala das magias possíveis.

#### **Desempenho por Padrão**

O DOTS é uma filosofia que visa o "desempenho por padrão", alinhando a arquitetura do software com o funcionamento do hardware moderno.2 Ele alcança isso através de três tecnologias principais que trabalham em sinergia:

- **Localidade de Dados e Eficiência de Cache (ECS):** O Entity Component System (ECS) resolve o problema da memória dispersa. Em vez de objetos, os dados são organizados em componentes, que são structs simples. Entidades com a mesma combinação de componentes (o mesmo "arquétipo") têm seus dados de componentes armazenados juntos em blocos contíguos de memória chamados "chunks".1 Quando um sistema itera sobre os componentes de um chunk, os dados para a próxima entidade já estão no cache da CPU, eliminando quase que completamente os cache misses e permitindo que a CPU processe os dados em velocidade máxima.5
- **Paralelização através do C\# Job System:** A clara separação entre dados (componentes) e lógica (sistemas) no ECS é ideal para a execução segura e multithreaded. O C\# Job System permite que os desenvolvedores escrevam lógica que opera em dados de componentes em paralelo em múltiplos núcleos de CPU. Como os sistemas são geralmente sem estado e operam em blocos de dados bem definidos, o risco de condições de corrida e outros problemas de concorrência é drasticamente reduzido, permitindo uma utilização quase linear do poder de processamento de CPUs multi-core.4
- **Código Otimizado com o Burst Compiler:** O Burst é um compilador que traduz o código C\# (especificamente, um subconjunto dele) em código de máquina nativo altamente otimizado, utilizando a infraestrutura LLVM. O código compilado com Burst pode atingir um desempenho comparável ao C++ escrito à mão. No entanto, o Burst impõe uma restrição crítica: ele só pode operar em código que não utiliza memória gerenciada (classes, strings, etc.) e, portanto, não está sujeito à coleta de lixo (Garbage Collection \- GC).4 Esta restrição é um fator determinante para as decisões de design em toda a arquitetura do Spellwright.

### **1.2. Os Primitivos Fundamentais da Criação de Magias (O "O Quê")**

Para construir o Spellwright sobre esta fundação, é essencial entender os blocos de construção fundamentais do ECS.

#### **Entidades: Os Recipientes da Magia**

No ECS, uma entidade não é um objeto. É simplesmente um identificador inteiro único—um "ponteiro" que associa um conjunto de componentes.1 Uma única magia de "Bola de Fogo" não é uma única entidade. Ela é uma composição. Pode haver uma entidade para o projétil, outra para seu rastro de efeitos visuais, e entidades subsequentes para a explosão e os efeitos de queimadura persistentes. Esta granularidade é a chave para a flexibilidade e modularidade do sistema. Permite que sistemas distintos operem em diferentes aspectos da magia de forma independente e performática.

#### **Componentes (IComponentData): Os Dados Atômicos das Magias**

Os componentes são a alma do ECS. Eles são structs C\# que implementam a interface IComponentData (ou uma de suas variantes). A regra de ouro é que eles contêm _apenas dados_, sem lógica ou métodos.1 Exemplos para o Spellwright incluiriam Velocity, Lifetime, DamagePayload, HomingParameters.

A distinção mais crítica na arquitetura do Spellwright é entre componentes não gerenciados e gerenciados:

- **Componentes Não Gerenciados (Unmanaged):** São structs que contêm apenas tipos "blittable" (tipos primitivos como int, float, bool, e structs compostas por eles). Eles não contêm referências a objetos gerenciados (como classes ou strings) e podem ser copiados bit a bit. Esta é a única categoria de componentes que pode ser usada com o C\# Job System e o Burst Compiler. A arquitetura do Spellwright _exige_ o uso de componentes não gerenciados para toda a lógica de simulação de magias em tempo real para garantir o máximo desempenho.2
- **Componentes Gerenciados (Managed):** São classes que implementam IComponentData. Eles existem para facilitar a integração com sistemas Unity tradicionais e para prototipagem. No entanto, eles vêm com severas penalidades de desempenho: não podem ser usados em Jobs ou com o Burst, seus dados não são armazenados em chunks contíguos e eles estão sujeitos à coleta de lixo.7 No Spellwright, o uso de componentes gerenciados será explicitamente proibido para qualquer entidade de magia em tempo de execução. Seu uso será restrito apenas a entidades "singleton" de alto nível que servem como pontes para a camada de comunicação com a API do LLM.

#### **Sistemas (ISystem vs. SystemBase): A Lógica da Magia**

Os sistemas contêm toda a lógica do jogo. Eles são classes ou structs sem estado que consultam por entidades que possuem uma combinação específica de componentes e, em seguida, operam sobre os dados desses componentes.1 Por exemplo, um MovementSystem consultaria todas as entidades com os componentes Position e Velocity e atualizaria a posição de cada uma a cada frame.

A escolha entre ISystem e SystemBase é uma decisão arquitetônica primária que reflete diretamente a filosofia de desempenho do DOTS.

- **ISystem:** É uma interface que define um sistema não gerenciado, implementado como uma struct. Sistemas ISystem são totalmente compatíveis com o Burst Compiler, não alocam memória gerenciada (zero GC) e oferecem o desempenho mais alto possível. Eles são a escolha padrão e _obrigatória_ para todos os sistemas de simulação de alto desempenho no Spellwright, como movimento, colisão, processamento de vida útil e aplicação de efeitos.9
- **SystemBase:** É uma classe base abstrata para sistemas gerenciados. É mais fácil de usar, oferece sintaxe conveniente como Entities.ForEach e Job.WithCode, e pode interagir facilmente com dados gerenciados e APIs tradicionais da Unity. No entanto, ele não pode ser compilado com Burst e pode gerar lixo de memória, tornando-o mais lento.8

A diretriz arquitetônica para o Spellwright é clara: adotar uma abordagem híbrida. **ISystem será usado para 95% da lógica de magia em tempo de execução.** SystemBase será usado _apenas_ para tarefas não críticas de desempenho e que ocorrem uma única vez, como o sistema inicial que analisa o JSON recebido do LLM e "banca" (cria) o prefab da entidade da magia. Esta partição estratégica maximiza o desempenho onde ele é mais crítico—a simulação frame a frame—enquanto aproveita a conveniência do SystemBase para tarefas de configuração menos frequentes.

A tabela a seguir resume esta decisão arquitetônica crucial.

**Tabela 1.1: Trade-offs Arquitetônicos: ISystem vs. SystemBase**

| Característica                | ISystem                                  | SystemBase                                            | Diretriz Arquitetônica para o Spellwright                                                     |
| :---------------------------- | :--------------------------------------- | :---------------------------------------------------- | :-------------------------------------------------------------------------------------------- |
| **Compatibilidade com Burst** | Sim                                      | Não                                                   | **Obrigatório** para todos os sistemas de simulação em tempo real (movimento, colisão, etc.). |
| **Alocação de Memória (GC)**  | Nenhuma (Zero GC)                        | Sim (Potencial)                                       | Minimiza a pressão do GC, crucial para um desempenho suave e sem picos.                       |
| **Armazenamento de Dados**    | Apenas dados não gerenciados             | Pode conter dados gerenciados                         | Mantém a simulação principal livre de referências a objetos.                                  |
| **Sintaxe/Conveniência**      | Mais verboso (SystemAPI.Query)           | Mais simples (Entities.ForEach)                       | A clareza é secundária ao desempenho para a lógica central.                                   |
| **Caso de Uso**               | Lógica de alto desempenho, frame a frame | Lógica de configuração, integração com MonoBehaviours | Usar **apenas** para tarefas de "baking" de JSON e comunicação com a camada de UI/rede.       |

## **Seção 2: O Oráculo de IA \- Traduzindo Intenção em Estrutura com LLMs**

Esta seção detalha a camada de inteligência do Spellwright, o componente responsável por interpretar a linguagem criativa e transformá-la em um projeto estruturado que o motor de jogo possa entender. Este processo não é mágico; é um pipeline de dados rigorosamente projetado que utiliza as capacidades dos LLMs enquanto mitiga suas fraquezas inerentes.

### **2.1. O Pipeline de Dados do LLM para o Motor**

A comunicação entre o cliente Unity e o serviço de LLM deve ser robusta, assíncrona e bem definida. O fluxo de dados de alto nível é o seguinte:

1. **Entrada do Usuário (Linguagem Natural):** O processo começa com um designer ou jogador inserindo uma descrição de texto, como "Crie uma lança de gelo que perfura o primeiro inimigo e explode no segundo".
2. **Cliente Unity:** A aplicação Unity recebe essa string.
3. **Requisição HTTPS:** O cliente constrói uma requisição para a API do LLM. Esta requisição não contém apenas a entrada do usuário, mas também um "prompt de sistema" cuidadosamente elaborado que inclui o esquema de saída desejado e exemplos de traduções corretas.
4. **Serviço de API do LLM:** Um serviço de LLM hospedado (como os oferecidos pela OpenAI, Anthropic ou Google) processa a requisição.
5. **Resposta HTTPS (JSON):** O serviço de API retorna uma resposta. Graças às restrições impostas no prompt, esta resposta deve ser uma string JSON formatada.
6. **Cliente Unity:** O cliente recebe a string JSON.
7. **Validador JSON:** Antes de qualquer processamento adicional, o cliente valida a string recebida em relação ao esquema predefinido para garantir tanto a validade sintática quanto a conformidade estrutural.
8. **Sistema de Baking ECS:** Se a validação for bem-sucedida, o JSON validado é passado para um sistema ECS dedicado para ser transformado em uma entidade de magia.

É crucial que toda a comunicação de rede (passos 3-6) seja tratada de forma assíncrona. O cliente Unity não deve congelar enquanto espera por uma resposta da API. Isso será implementado usando os padrões async/await do C\# em conjunto com a classe UnityWebRequest da Unity ou uma biblioteca HTTP de terceiros mais robusta, garantindo que a thread principal do jogo permaneça responsiva e a experiência do usuário seja fluida.

### **2.2. Definindo a "Lingua Magica": Um Esquema JSON Robusto**

O contrato de dados entre o LLM e o motor de jogo é a pedra angular de todo o sistema. Sem um formato de dados rigorosamente definido e aplicado, a saída do LLM seria inutilizável. Este contrato é formalizado através de um Esquema JSON.12 O esquema a seguir, inspirado nos conceitos de design de magias encontrados em vários jogos 13, define a estrutura que o LLM deve gerar.

```json
{
    "type": "object",
    "properties": {
        "spellName": {
            "type": "string",
            "description": "O nome descritivo da magia."
        },
        "generator": {
            "type": "object",
            "description": "Define a forma fundamental e o método de entrega da magia.",
            "properties": {
                "type": {
                    "type": "string",
                    "enum": [
                        "projectile",
                        "cone",
                        "self_cast",
                        "area_of_effect"
                    ]
                },
                "speed": {
                    "type": "number",
                    "description": "Velocidade inicial para projéteis."
                },
                "lifetime": {
                    "type": "number",
                    "description": "Duração em segundos para projéteis ou áreas de efeito."
                },
                "count": {
                    "type": "integer",
                    "description": "Número de projéteis a serem disparados."
                },
                "radius": {
                    "type": "number",
                    "description": "Raio para áreas de efeito ou cones."
                },
                "angle": {
                    "type": "number",
                    "description": "Ângulo para geradores do tipo cone."
                }
            },
            "required": ["type"]
        },
        "modifiers": {
            "type": "array",
            "description": "Uma lista de comportamentos que alteram a funcionalidade básica do gerador.",
            "items": {
                "type": "object",
                "properties": {
                    "type": {
                        "type": "string",
                        "enum": [
                            "homing",
                            "chain",
                            "pierce",
                            "multi_shot",
                            "on_impact_trigger",
                            "fork"
                        ]
                    },
                    "turn_speed": {
                        "type": "number",
                        "description": "Velocidade de curva para o modificador 'homing'."
                    },
                    "max_bounces": {
                        "type": "integer",
                        "description": "Número máximo de saltos para o modificador 'chain'."
                    },
                    "pierce_count": {
                        "type": "integer",
                        "description": "Número de alvos que podem ser perfurados."
                    }
                },
                "required": ["type"]
            }
        },
        "payloads": {
            "type": "array",
            "description": "Uma lista de efeitos que são aplicados no impacto ou ativação.",
            "items": {
                "type": "object",
                "properties": {
                    "type": {
                        "type": "string",
                        "enum": [
                            "damage",
                            "heal",
                            "apply_status_effect",
                            "spawn_entity"
                        ]
                    },
                    "amount": {
                        "type": "number",
                        "description": "Quantidade de dano ou cura."
                    },
                    "elemental_type": {
                        "type": "string",
                        "enum": ["fire", "frost", "shock", "physical", "holy"]
                    },
                    "effect_name": {
                        "type": "string",
                        "enum": ["slow", "burn", "stun", "poison"]
                    },
                    "duration": {
                        "type": "number",
                        "description": "Duração para efeitos de status."
                    }
                },
                "required": ["type"]
            }
        }
    },
    "required": ["spellName", "generator", "payloads"]
}
```

Este esquema define claramente os três componentes principais de uma magia:

- **generator:** Como a magia se origina e se move. É um projétil, um cone, um efeito em área?
- **modifiers:** Como o comportamento do gerador é alterado. Ele persegue alvos, ricocheteia entre eles, perfura-os?
- **payloads:** O que a magia faz ao atingir seu objetivo. Causa dano, cura, aplica um debuff?

### **2.3. Garantindo a Integridade Sintática e Semântica (O "Como")**

O principal desafio ao trabalhar com LLMs é sua natureza probabilística. Eles são excelentes geradores de texto, mas não são compiladores determinísticos. Sem um controle rigoroso, eles podem produzir JSON inválido, adicionar texto conversacional ou "alucinar" valores que não se encaixam no esquema.16 Uma única técnica de prompting é insuficiente para garantir a robustez necessária para a produção. Portanto, o Spellwright empregará uma estratégia de defesa em camadas.

1. **Camada 1: Aplicação em Nível de API (Function Calling / JSON Mode):** As APIs de LLM mais recentes (como as da OpenAI, Google e Anthropic) oferecem recursos projetados especificamente para forçar a saída estruturada. O recurso de "Uso de Ferramentas" (Tool Use), anteriormente conhecido como "Function Calling", é o mais poderoso. Ao definir nosso esquema JSON como a assinatura de uma "ferramenta" que o modelo pode "chamar", instruímos o LLM a gerar um objeto JSON que corresponde precisamente aos argumentos dessa ferramenta.16 Alternativamente, o "Modo JSON" garante que a saída seja um JSON sintaticamente válido, embora com menos controle sobre o esquema. Para o Spellwright, o Uso de Ferramentas é a abordagem preferida, pois resolve o problema da _validade sintática_ no nível mais fundamental.16
2. **Camada 2: Orientação Semântica (Few-Shot Prompting):** Garantir que o JSON seja sintaticamente válido é apenas metade da batalha. Também precisamos garantir que ele represente corretamente a intenção do usuário—o problema da _correção semântica_. Por exemplo, o modelo precisa aprender que a frase "persegue inimigos" deve ser mapeada para modifiers: \[{type: "homing"}\]. A técnica mais eficaz para isso é o "few-shot prompting". O prompt de sistema enviado ao LLM incluirá não apenas a instrução e o esquema da ferramenta, mas também vários exemplos completos de entradas do usuário e a saída JSON correta correspondente.20 Este aprendizado em contexto (in-context learning) treina o modelo no momento da inferência, guiando seu processo de raciocínio e melhorando drasticamente a precisão da tradução semântica.20
3. **Camada 3: Validação no Lado do Cliente:** Como uma última linha de defesa, o cliente Unity validará a resposta JSON recebida antes de tentar processá-la. Usando uma biblioteca C\# como Newtonsoft.Json ou System.Text.Json, o cliente irá analisar a string e validá-la em relação a uma representação local do esquema. Se a validação falhar devido a um erro de rede, uma mudança na API do modelo ou uma falha rara do modelo, o sistema pode descartar o resultado e notificar o usuário. Uma abordagem mais avançada, inspirada em bibliotecas como a Instructor 16, poderia até mesmo enviar a mensagem de erro de validação de volta ao LLM em uma chamada de repetição, pedindo-lhe para corrigir sua própria saída.

Esta abordagem em camadas cria um pipeline de dados resiliente que aproveita os pontos fortes dos LLMs enquanto se protege contra suas fraquezas, garantindo que apenas dados bem formados e semanticamente plausíveis cheguem ao motor do jogo. A tabela a seguir compara as técnicas disponíveis e justifica a abordagem híbrida escolhida.

**Tabela 2.1: Análise Comparativa de Técnicas de Geração de JSON por LLM**

| Técnica                                | Descrição                                                                                               | Prós                                                         | Contras                                                                       | Papel no Spellwright                                                                                            |
| :------------------------------------- | :------------------------------------------------------------------------------------------------------ | :----------------------------------------------------------- | :---------------------------------------------------------------------------- | :-------------------------------------------------------------------------------------------------------------- |
| **Prompting Básico**                   | Simplesmente pedir ao LLM para gerar JSON no prompt.                                                    | Fácil de implementar.                                        | Altamente não confiável; propenso a erros de sintaxe e texto extra.           | **Inadequado** para produção.                                                                                   |
| **Esquema JSON no Prompt**             | Incluir a definição do esquema como texto no prompt.                                                    | Melhora a conformidade da estrutura.                         | Ainda não garante a validade sintática; o modelo pode ignorar as instruções.  | Usado como parte do prompt de sistema, mas insuficiente por si só.                                              |
| **Modo JSON da API**                   | Um parâmetro de API que força a saída a ser um JSON sintaticamente válido.                              | Garante a validade sintática.                                | Menos controle sobre o esquema; o modelo pode gerar uma estrutura inesperada. | Uma boa alternativa, mas menos controlável que o Function Calling.                                              |
| **Function Calling da API**            | Definir o esquema como uma "ferramenta" que o modelo "chama".                                           | Garante a validade sintática e a conformidade com o esquema. | Específico para APIs que suportam o recurso.                                  | **Camada 1:** A principal técnica para garantir a integridade estrutural e sintática.                           |
| **Few-Shot Prompting**                 | Fornecer exemplos de entrada/saída no prompt.                                                           | Melhora drasticamente a precisão semântica e a consistência. | Aumenta o tamanho do prompt e o custo do token.                               | **Camada 2:** Essencial para ensinar ao modelo a mapear a linguagem natural para o esquema correto.             |
| **Amostradores Baseados em Gramática** | Bibliotecas (ex: Jsonformer) que restringem a geração de tokens do LLM para se ajustar a uma gramática. | Garante 100% de validade.                                    | Requer mais integração, pode ser incompatível com APIs hospedadas.            | Uma alternativa poderosa para modelos de código aberto locais, mas o Function Calling é mais simples para APIs. |

## **Seção 3: A Linha de Montagem \- Construção Procedural de Magias em ECS**

Esta seção detalha o processo de transformação do projeto JSON abstrato, gerado pela IA, em uma realidade concreta e executável dentro do mundo ECS. Este é o coração do sistema de montagem procedural, onde os dados são convertidos em comportamento. A elegância desta arquitetura reside na sinergia quase perfeita entre o modelo conceitual Generator/Modifier/Payload e os padrões de design de composição inerentes ao ECS.

### **3.1. Do Projeto à Realidade: O Processo de "Baking" do JSON**

O termo "baking" (ou "assar") no contexto do ECS refere-se ao processo de converter dados de um formato de autoria (neste caso, nosso JSON) para o formato de tempo de execução otimizado do ECS.6 No Spellwright, este processo será gerenciado por um sistema dedicado.

- **O SpellBakingSystem:** Este será um sistema que herda de SystemBase. A escolha do SystemBase é deliberada aqui, pois esta é uma tarefa de configuração que ocorre uma única vez por nova magia, não é uma operação de alto desempenho que precisa ser executada a cada frame. O SystemBase oferece a conveniência de trabalhar com dados gerenciados (a string JSON e objetos C\# desserializados) que seriam proibitivos em um ISystem.8
- **O Processo de Baking:**
    1. O SpellBakingSystem é ativado quando um novo objeto JSON validado é recebido da camada de IA.
    2. Ele desserializa a string JSON em uma estrutura de classes C\# que espelha o esquema.
    3. Ele utiliza um EntityCommandBuffer (ECB). Um ECB é um mecanismo essencial para enfileirar "mudanças estruturais" (como a criação de entidades ou a adição/remoção de componentes). Essas mudanças são dispendiosas e, ao enfileirá-las em um ECB, elas podem ser processadas em lote em um ponto de sincronização controlado pelo motor, o que é muito mais performático do que executá-las imediatamente.6
    4. O sistema percorre a estrutura de dados desserializada e, para cada parte da magia (gerador, modificadores, payloads), ele adiciona comandos ao ECB para adicionar os componentes ECS apropriados à nova entidade.
    5. Após o ECB ser executado, o resultado é uma nova entidade que existe no mundo ECS. Esta entidade é configurada como um "Prefab". No ECS, um prefab não é um asset como no mundo dos GameObjects; é simplesmente uma entidade com um componente Prefab que pode ser instanciada (clonada) de forma extremamente eficiente milhares de vezes por frame. Esta entidade prefab é o projeto final e executável da magia.

### **3.2. Anatomia de uma Magia em ECS**

Esta subseção fornece o mapeamento direto e detalhado do esquema JSON para os componentes ECS, ilustrando a abordagem composicional.

#### **Geradores (ex: Projétil, Cone, Próprio)**

Os geradores definem a forma fundamental da magia e são implementados como uma combinação de "componentes de tag" (tags) e componentes de dados.

- **JSON:** "generator": {"type": "projectile", "speed": 50, "lifetime": 3.0}
- **Mapeamento ECS:** Este JSON instrui o SpellBakingSystem a criar uma entidade com os seguintes componentes:
    - ProjectileTag : IComponentData
        - Um struct vazio que serve como um marcador. Permite que sistemas consultem facilmente por "todas as entidades que são projéteis" sem a sobrecarga de dados adicionais.
    - Velocity : IComponentData { public Unity.Mathematics.float3 Value; }
        - Armazena a velocidade e a direção do projétil.
    - Lifetime : IComponentData { public float Value; }
        - Armazena o tempo restante de vida do projétil em segundos.

Essas entidades são então processadas por sistemas genéricos e reutilizáveis, como um MovementSystem (que atualiza a posição com base na velocidade) e um LifetimeSystem (que decrementa a vida útil e destrói a entidade quando ela chega a zero).

#### **Modificadores (ex: Teleguiado, Em Cadeia, Perfurante)**

Os modificadores são quase sempre implementados como componentes adicionais que "ativam" sistemas especializados. Esta é a essência da modularidade no ECS.

- **JSON:** "modifiers": \[{"type": "homing", "turn_speed": 5.0}\]
- **Mapeamento ECS:** Este JSON adiciona o seguinte componente à entidade do projétil:
    - HomingModifier : IComponentData { public float TurnSpeed; public Entity Target; }
        - Contém os parâmetros para o comportamento de perseguição. O campo Target seria preenchido em tempo de execução por um sistema de aquisição de alvos.

A beleza desta abordagem é o desacoplamento. O MovementSystem continua a fazer seu trabalho simples de mover a entidade com base em sua Velocity. Um HomingSystem separado e especializado consultará por entidades que tenham Velocity _e_ HomingModifier. Este sistema então executará sua lógica para encontrar um alvo e ajustar o componente Velocity para guiar o projétil em direção a ele. O MovementSystem não precisa saber nada sobre o comportamento de perseguição; ele apenas age sobre os dados que lhe são fornecidos.

#### **Payloads (ex: Dano de Fogo, Lentidão de Gelo, Cura)**

Os payloads, que representam os efeitos da magia, são implementados de forma ideal usando DynamicBuffer\<T\>. Um buffer dinâmico é uma lista de tamanho variável de elementos de dados (IBufferElementData) associada a uma entidade. É perfeito para magias que podem ter múltiplos efeitos.6

- **JSON:** "payloads": \[{"type": "damage", "amount": 10, "elemental_type": "fire"}, {"type": "apply_status_effect", "effect_name": "burn", "duration": 5.0}\]
- **Mapeamento ECS:** Este JSON adiciona um buffer dinâmico à entidade:
    - DynamicBuffer\<PayloadElement\>
    - Onde PayloadElement é um struct que implementa IBufferElementData:  
      C\#  
      public struct PayloadElement : IBufferElementData  
      {  
       public PayloadType Type; // Enum: Damage, ApplyStatus  
       public ElementalType Element; // Enum: Fire, Frost  
       public StatusEffectType Effect; // Enum: Burn, Slow  
       public float Amount;  
       public float Duration;  
      }

Quando um CollisionSystem detecta que o projétil atingiu um alvo, ele não precisa saber como aplicar dano ou status. Ele simplesmente lê o DynamicBuffer\<PayloadElement\> na entidade do projétil e, para cada elemento no buffer, passa os dados para um PayloadResolutionSystem genérico. Este sistema de resolução é o único que contém a lógica para aplicar dano à saúde de um alvo, adicionar um componente de status Burning a ele, etc. Isso mantém os sistemas altamente focados e reutilizáveis.

A tabela a seguir serve como um dicionário central, traduzindo os conceitos do JSON para a implementação concreta no ECS.

**Tabela 3.1: Biblioteca de Componentes de Magia Principal**

| Categoria       | Valor type do JSON  | Componente(s) ECS                                         | Sistema(s) Responsável(is)                   |
| :-------------- | :------------------ | :-------------------------------------------------------- | :------------------------------------------- |
| **Gerador**     | projectile          | ProjectileTag, Velocity, Lifetime, Position               | MovementSystem, LifetimeSystem               |
| **Gerador**     | area_of_effect      | AreaOfEffectTag, Radius, Lifetime, Position               | AreaQuerySystem, LifetimeSystem              |
| **Modificador** | homing              | HomingModifier { float TurnSpeed; Entity Target; }        | TargetAcquisitionSystem, HomingSystem        |
| **Modificador** | chain               | ChainModifier { int MaxBounces; int BouncesLeft; }        | ChainingSystem                               |
| **Modificador** | pierce              | PierceModifier { int PierceCount; int PiercesLeft; }      | CollisionSystem (lógica de perfuração)       |
| **Payload**     | damage              | DynamicBuffer\<PayloadElement\> (com Type \= Damage)      | CollisionSystem \-\> PayloadResolutionSystem |
| **Payload**     | heal                | DynamicBuffer\<PayloadElement\> (com Type \= Heal)        | CollisionSystem \-\> PayloadResolutionSystem |
| **Payload**     | apply_status_effect | DynamicBuffer\<PayloadElement\> (com Type \= ApplyStatus) | CollisionSystem \-\> PayloadResolutionSystem |

Esta arquitetura demonstra que o modelo conceitual Generator/Modifier/Payload não é apenas uma abstração útil para a interface do LLM; é um reflexo direto do padrão de design mais poderoso e performático do ECS: a composição. Essa sinergia reduz o atrito arquitetônico e torna todo o sistema mais coerente, extensível e de fácil manutenção. Adicionar um novo modificador, por exemplo, "fork" (que divide um projétil em vários), simplesmente requer a definição de um novo componente ForkModifier e a criação de um ForkSystem que o processe, sem a necessidade de modificar nenhum sistema existente.

## **Seção 4: Integração, Fluxo de Trabalho e Melhores Práticas**

Esta seção final consolida os conceitos discutidos anteriormente em um fluxo de trabalho prático e aborda detalhes de implementação cruciais para a integração, comunicação e depuração do sistema Spellwright.

### **4.1. O Fluxo de Criação de Magia de Ponta a Ponta: Um Exemplo**

Para ilustrar como todos os componentes da arquitetura funcionam em conjunto, vamos percorrer um exemplo completo, desde a intenção do usuário até a execução no jogo.

1. **Entrada do Usuário:** Um designer digita no editor do Spellwright: "Crie um raio em cadeia que salta entre três inimigos, causando dano de choque."
2. **Prompt para o LLM:** O cliente Unity encapsula esta entrada em um prompt de sistema abrangente. Este prompt contém:
    - A instrução principal para traduzir o texto em JSON.
    - A definição completa do esquema JSON, formatada como uma "ferramenta" que o modelo pode usar (conforme a Camada 1 da nossa estratégia de defesa).
    - Vários exemplos de "few-shot" para guiar a semântica (Camada 2).
3. **Resposta do LLM (JSON):** A API do LLM processa o prompt e retorna um objeto JSON estruturado:

    ```json
    {
        "spellName": "Raio em Cadeia",
        "generator": {
            "type": "projectile",
            "speed": 100.0,
            "lifetime": 2.0
        },
        "modifiers": [
            {
                "type": "chain",
                "max_bounces": 3
            }
        ],
        "payloads": [
            {
                "type": "damage",
                "amount": 15.0,
                "elemental_type": "shock"
            }
        ]
    }
    ```

4. **Validação:** O cliente Unity recebe a string JSON e a valida em relação ao seu esquema local (Camada 3). A validação é bem-sucedida.
5. **Baking:** O SpellBakingSystem (SystemBase) é acionado. Ele lê o objeto JSON e usa um EntityCommandBuffer para enfileirar a criação de uma nova entidade prefab. Ele adiciona os seguintes componentes: Prefab, ProjectileTag, Velocity (com valor baseado na velocidade e direção inicial), Lifetime, ChainModifier (com max_bounces \= 3), e um DynamicBuffer\<PayloadElement\> contendo os dados do dano de choque.
6. **Instanciação:** No jogo, quando o jogador lança a magia "Raio em Cadeia", um PlayerCastSystem (ISystem) executa um único comando: instanciar a entidade prefab. Esta é uma operação extremamente rápida e eficiente no ECS.
7. **Execução:** A nova entidade de projétil agora existe no mundo e os sistemas ISystem começam a processá-la a cada frame:
    - O MovementSystem atualiza a posição da entidade com base em sua Velocity.
    - Um CollisionSystem detecta uma colisão com uma entidade inimiga.
    - Ao detectar a colisão, o CollisionSystem lê o DynamicBuffer do projétil e passa os dados para o PayloadResolutionSystem, que aplica os 15 de dano de choque ao inimigo.
    - O CollisionSystem também notifica o ChainingSystem sobre a colisão.
    - O ChainingSystem consulta por entidades com ChainModifier. Ele encontra um novo alvo próximo, atualiza os componentes Position e Velocity do projétil para mirar no novo alvo e decrementa a contagem de saltos restantes no componente ChainModifier.
    - Este ciclo se repete até que os saltos se esgotem ou nenhum novo alvo seja encontrado, momento em que o ChainingSystem pode destruir o projétil.

Este fluxo demonstra a clara separação de responsabilidades: o SystemBase lida com a configuração única e complexa, enquanto os ISystems, otimizados com Burst, lidam com a simulação repetitiva e de alto desempenho.

### **4.2. Comunicação do Motor com a Nuvem**

A implementação técnica da chamada assíncrona para o serviço de LLM é crítica para a experiência do usuário.

- **Endpoint da API:** A arquitetura pressupõe um endpoint de API RESTful simples. O cliente Unity serializará a solicitação do usuário (a string de entrada, mais quaisquer metadados) em um payload JSON e a enviará via UnityWebRequest (ou uma biblioteca similar) usando um método POST.
- **Gerenciamento Assíncrono:** A chamada de rede _deve_ ser não bloqueante. O uso de async/await em C\# é a abordagem moderna e recomendada. Uma classe de serviço dedicada (por exemplo, LLMSpellCreationService) encapsulará essa lógica. O método de chamada (por exemplo, public async Task\<string\> CreateSpellJsonAsync(string prompt)) retornará uma Task, permitindo que o código de chamada (por exemplo, na UI do editor) aguarde o resultado sem bloquear a thread principal da Unity. Isso evita que o editor ou o jogo congele, o que seria inaceitável.
- **Tratamento de Erros:** O serviço de comunicação deve implementar um tratamento de erros robusto. Isso inclui o tratamento de timeouts de rede, códigos de status de erro HTTP (por exemplo, 4xx para erros de cliente, 5xx para erros de servidor) e respostas malformadas da API. Os erros devem ser registrados de forma clara e apresentados ao usuário de maneira informativa (por exemplo, "Não foi possível conectar ao serviço de IA" ou "A descrição da magia era muito ambígua").

### **4.3. Depuração e Ferramentas**

Depurar um sistema orientado a dados e multithreaded como o DOTS pode ser desafiador sem as ferramentas certas. Felizmente, a Unity fornece um conjunto robusto de janelas de depuração específicas para o DOTS, que serão essenciais para o desenvolvimento do Spellwright.6

- **Janela de Hierarquia de Entidades (Entities Hierarchy):** Esta é a ferramenta mais fundamental. Ela permite inspecionar as entidades ativas no mundo em tempo de execução, ver quais componentes cada uma possui e examinar os valores de dados desses componentes em tempo real. É indispensável para verificar se o processo de baking está adicionando os componentes corretos.
- **Janela de Sistemas (Systems Window):** Esta janela mostra a ordem de atualização de todos os sistemas no mundo. É crucial para depurar problemas de lógica que dependem da ordem em que os sistemas são executados (por exemplo, garantir que o HomingSystem seja executado antes do MovementSystem). Também exibe o tempo que cada sistema leva para ser executado, ajudando a identificar gargalos de desempenho.
- **Profiler da Unity:** O profiler integrado da Unity possui módulos específicos para o DOTS. O "Entities Structural Changes Profiler" é particularmente útil para identificar onde e com que frequência as mudanças estruturais estão ocorrendo, ajudando a otimizar o uso de EntityCommandBuffers. O "Entities Memory Profiler" ajuda a rastrear o uso de memória por arquétipos e chunks.
- **Logging da IA:** Para a parte do LLM, um logging robusto é não negociável. O sistema deve registrar:
    1. A entrada de linguagem natural bruta do usuário.
    2. O prompt completo enviado para a API (incluindo o prompt de sistema e os exemplos de few-shot).
    3. A resposta JSON exata recebida da API.

Este registro detalhado é a única maneira de depurar eficazmente por que o LLM pode estar interpretando mal uma instrução ou gerando uma estrutura incorreta.

## **Conclusão: O Futuro do Spellwright**

### **Resumo da Arquitetura**

A arquitetura proposta para o Spellwright representa uma fusão robusta e performática de duas tecnologias de ponta: o design orientado a dados e a inteligência artificial generativa. A base do sistema é o Data-Oriented Technology Stack (DOTS) da Unity, que garante que a simulação de um número potencialmente massivo de magias complexas possa ser executada com eficiência máxima, aproveitando o hardware moderno através da localidade de dados e do paralelismo. Sobre esta fundação, uma camada de interpretação de IA, alimentada por um Large Language Model, atua como uma interface intuitiva, traduzindo a intenção criativa da linguagem natural para um formato de dados estruturado e inequívoco.

A chave para a elegância e eficácia desta arquitetura é a sinergia entre o modelo conceitual de Generator/Modifier/Payload, exigido pela interface de IA, e a natureza inerentemente composicional do Entity Component System. Esta correspondência direta minimiza a complexidade da tradução e resulta em um sistema que não é apenas performático, mas também extraordinariamente modular, extensível e de fácil manutenção. As diretrizes arquitetônicas rigorosas—como a preferência mandatória por ISystem sobre SystemBase para a lógica de simulação e a estratégia de defesa em camadas para a geração de JSON—garantem que o sistema seja construído sobre princípios sólidos de engenharia de software de alto desempenho.

### **Caminhos para Expansão**

A arquitetura fundamental descrita neste documento não é um ponto final, mas uma plataforma poderosa para inovações futuras. Várias avenidas de expansão podem ser construídas sobre este núcleo:

- **Balanceamento Dinâmico por IA:** Um LLM poderia ser encarregado não apenas de criar, mas também de analisar magias. Ao receber o JSON de uma magia gerada, ele poderia ser solicitado a avaliar seu poder e sugerir ajustes de balanceamento. Por exemplo, com o prompt: "Analise esta magia. Ela tem alto dano e um modificador de perseguição. Sugira um custo de mana apropriado e um tempo de recarga.", o sistema poderia automatizar parte do tedioso processo de balanceamento do jogo.
- **Geração de Efeitos Visuais e Sonoros (VFX/SFX):** O escopo do JSON gerado pode ser expandido para incluir parâmetros para outros sistemas do jogo. O LLM poderia adicionar um objeto "vfx" ao JSON, contendo tags descritivas ou valores diretos, como {"primary_color": "\#FF8C00", "particle_shape": "ember", "impact_sound": "fiery_explosion"}. Esses dados poderiam então ser lidos por sistemas de VFX e áudio para montar proceduralmente os efeitos correspondentes, estendendo o poder da criação por linguagem natural para além da mecânica de jogo.
- **Criação de Magias pelo Jogador:** A extensão final e mais ambiciosa do Spellwright seria expor esta interface diretamente aos jogadores dentro do jogo. A arquitetura baseada em DOTS é projetada desde o início para ser escalável o suficiente para suportar tal recurso, permitindo que os jogadores criem, nomeiem e compartilhem suas próprias magias usando linguagem natural. Isso transformaria a magia de um conjunto estático de habilidades em um sistema de criação verdadeiramente dinâmico e emergente, representando uma nova fronteira no design de jogos de RPG.

Em suma, o Spellwright não é apenas um sistema para criar magias; é um projeto para um novo paradigma de desenvolvimento de conteúdo, onde a barreira entre a ideia criativa e a implementação técnica é significativamente reduzida, abrindo um vasto potencial para a criatividade tanto de desenvolvedores quanto de jogadores.

#### **Referências citadas**

1. ELI5: Isn't Unity ALREADY an entity component system? : r/Unity3D \- Reddit, acessado em outubro 21, 2025, [https://www.reddit.com/r/Unity3D/comments/1c00tdr/eli5_isnt_unity_already_an_entity_component_system/](https://www.reddit.com/r/Unity3D/comments/1c00tdr/eli5_isnt_unity_already_an_entity_component_system/)
2. Unity\`s "Performance by Default" under the hood \- \#InnoBlog, acessado em outubro 21, 2025, [https://blog.innogames.com/unitys-performance-by-default-under-the-hood/](https://blog.innogames.com/unitys-performance-by-default-under-the-hood/)
3. Start learning data-oriented design in Unity with these resources | Unity Blog, acessado em outubro 21, 2025, [https://unity.com/blog/engine-platform/dots-bootcamp-resources](https://unity.com/blog/engine-platform/dots-bootcamp-resources)
4. DOTS \- Unity's Data-Oriented Technology Stack, acessado em outubro 21, 2025, [https://unity.com/dots](https://unity.com/dots)
5. What is the entity component system (ECS) and Why should I use it? : r/Unity3D \- Reddit, acessado em outubro 21, 2025, [https://www.reddit.com/r/Unity3D/comments/cocrz6/what_is_the_entity_component_system_ecs_and_why/](https://www.reddit.com/r/Unity3D/comments/cocrz6/what_is_the_entity_component_system_ecs_and_why/)
6. Entities overview | Entities | 1.0.16 \- Unity \- Manual, acessado em outubro 21, 2025, [https://docs.unity3d.com/Packages/com.unity.entities@1.0/](https://docs.unity3d.com/Packages/com.unity.entities@1.0/)
7. General purpose components | Entities | 0.7.0-preview.19, acessado em outubro 21, 2025, [https://docs.unity3d.com/Packages/com.unity.entities@0.8/manual/component_data.html](https://docs.unity3d.com/Packages/com.unity.entities@0.8/manual/component_data.html)
8. SystemBase overview | Entities | 1.2.2-pre.1 \- Unity \- Manual, acessado em outubro 21, 2025, [https://docs.unity3d.com/Packages/com.unity.entities@1.2/manual/systems-systembase.html](https://docs.unity3d.com/Packages/com.unity.entities@1.2/manual/systems-systembase.html)
9. docs.unity3d.com, acessado em outubro 21, 2025, [https://docs.unity3d.com/Packages/com.unity.entities@1.2/manual/systems-comparison.html\#:\~:text=Differences%20between%20systems,to%20get%20better%20performance%20benefits.](https://docs.unity3d.com/Packages/com.unity.entities@1.2/manual/systems-comparison.html#:~:text=Differences%20between%20systems,to%20get%20better%20performance%20benefits.)
10. ISystem vs SystemBase differences in concepts \- ECS \- YouTube, acessado em outubro 21, 2025, [https://www.youtube.com/watch?v=gAqmcMZHIcw](https://www.youtube.com/watch?v=gAqmcMZHIcw)
11. Systems comparison | Entities | 1.2.2-pre.1 \- Unity \- Manual, acessado em outubro 21, 2025, [https://docs.unity3d.com/Packages/com.unity.entities@1.2/manual/systems-comparison.html](https://docs.unity3d.com/Packages/com.unity.entities@1.2/manual/systems-comparison.html)
12. How JSON Schema Works for LLM Tools & Structured Outputs, acessado em outubro 21, 2025, [https://blog.promptlayer.com/how-json-schema-works-for-structured-outputs-and-tool-integration/](https://blog.promptlayer.com/how-json-schema-works-for-structured-outputs-and-tool-integration/)
13. Character class \- Wikipedia, acessado em outubro 21, 2025, [https://en.wikipedia.org/wiki/Character_class](https://en.wikipedia.org/wiki/Character_class)
14. Spells \- The Noita Wiki, acessado em outubro 21, 2025, [https://noita.wiki.gg/wiki/Spells](https://noita.wiki.gg/wiki/Spells)
15. Spell Effect Generator \- Feed The Beast Wiki, acessado em outubro 21, 2025, [https://m.ftbwiki.org/Spell_Effect_Generator](https://m.ftbwiki.org/Spell_Effect_Generator)
16. Enhancing JSON Output with Large Language Models: A ... \- Medium, acessado em outubro 21, 2025, [https://medium.com/@dinber19/enhancing-json-output-with-large-language-models-a-comprehensive-guide-f1935aa724fb](https://medium.com/@dinber19/enhancing-json-output-with-large-language-models-a-comprehensive-guide-f1935aa724fb)
17. Practical Techniques to constraint LLM output in JSON format | by Minyang Chen \- Medium, acessado em outubro 21, 2025, [https://mychen76.medium.com/practical-techniques-to-constraint-llm-output-in-json-format-e3e72396c670](https://mychen76.medium.com/practical-techniques-to-constraint-llm-output-in-json-format-e3e72396c670)
18. Whats the best open source LLM for returning only JSON? : r/LocalLLaMA \- Reddit, acessado em outubro 21, 2025, [https://www.reddit.com/r/LocalLLaMA/comments/197mnt5/whats_the_best_open_source_llm_for_returning_only/](https://www.reddit.com/r/LocalLLaMA/comments/197mnt5/whats_the_best_open_source_llm_for_returning_only/)
19. imaurer/awesome-llm-json: Resource list for generating JSON using LLMs via function calling, tools, CFG. Libraries, Models, Notebooks, etc. \- GitHub, acessado em outubro 21, 2025, [https://github.com/imaurer/awesome-llm-json](https://github.com/imaurer/awesome-llm-json)
20. What is few shot prompting? | IBM, acessado em outubro 21, 2025, [https://www.ibm.com/think/topics/few-shot-prompting](https://www.ibm.com/think/topics/few-shot-prompting)
21. Zero-Shot, One-Shot, and Few-Shot Prompting, acessado em outubro 21, 2025, [https://learnprompting.org/docs/basics/few_shot](https://learnprompting.org/docs/basics/few_shot)
22. Few-Shot Prompting \- Prompt Engineering Guide, acessado em outubro 21, 2025, [https://www.promptingguide.ai/techniques/fewshot](https://www.promptingguide.ai/techniques/fewshot)
23. Entities package | Entities | 1.4.2 \- Unity \- Manual, acessado em outubro 21, 2025, [https://docs.unity3d.com/Packages/com.unity.entities@latest/](https://docs.unity3d.com/Packages/com.unity.entities@latest/)
24. Entities overview | Entities | 1.1.0-pre.3 \- Unity \- Manual, acessado em outubro 21, 2025, [https://docs.unity3d.com/Packages/com.unity.entities@1.1/manual/index.html](https://docs.unity3d.com/Packages/com.unity.entities@1.1/manual/index.html)
25. The different lists allowed in IComponentData \- COFFEE BRAIN GAMES, acessado em outubro 21, 2025, [https://coffeebraingames.wordpress.com/2020/12/20/the-different-lists-allowed-in-icomponentdata/](https://coffeebraingames.wordpress.com/2020/12/20/the-different-lists-allowed-in-icomponentdata/)
26. Unity-Technologies/EntityComponentSystemSamples \- GitHub, acessado em outubro 21, 2025, [https://github.com/Unity-Technologies/EntityComponentSystemSamples](https://github.com/Unity-Technologies/EntityComponentSystemSamples)
