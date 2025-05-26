# Mugen Forever (Unity)

## Objetivo do Projeto

Este projeto tem como objetivo recriar a funcionalidade da engine de jogos de luta 2D M.U.G.E.N, utilizando a engine Unity3D. A meta é permitir que personagens, cenários e outros assets do MUGEN possam ser carregados e utilizados neste novo motor.

## Estado Atual do Projeto (Outubro de 2023)

Atualmente, o projeto alcançou os seguintes marcos:

*   **Leitura de Arquivos SFF (Sprites):**
    *   Implementada a leitura de arquivos SFF (versões 1 e 2).
    *   Decodificação de imagens PCX (incluindo compressão RLE e extração de paleta) contidas nos arquivos SFF.
    *   Geração de texturas e sprites do Unity a partir dos dados SFF.
*   **Leitura de Arquivos AIR (Animações):**
    *   Implementado o parsing de arquivos `.AIR` para extrair definições de animação (Actions).
    *   Cada ação contém uma sequência de frames, com informações sobre qual sprite SFF usar, duração e flags de espelhamento.
*   **Leitura de Arquivos CNS (Estados):**
    *   Implementado um parser básico para arquivos `.CNS`.
    *   O parser consegue identificar `[Statedef]` (definições de estado) e os `StateControllers` (SCTRLs) dentro deles.
    *   Triggers e parâmetros dos SCTRLs são armazenados como strings para processamento futuro.
*   **Leitura de Arquivos CMD (Comandos):**
    *   Implementado um parser básico para arquivos `.CMD`.
    *   O parser consegue identificar `[Command]` e extrair o nome do comando, a sequência de inputs (com tratamento para "hold" e "release"), tempos e o número do estado a ser ativado.
*   **Exibição e Animação Básica:**
    *   Um personagem de exemplo (KFM - Kung Fu Man) pode ser carregado.
    *   O script `StartGame.cs` carrega o arquivo `.DEF` do personagem, que por sua vez aponta para os arquivos SFF, AIR, CNS e CMD.
    *   O personagem é exibido na cena e executa sua animação de "parado" (Action 0) lida do arquivo AIR.

## Próximos Passos

Os próximos passos planejados para o desenvolvimento incluem:

1.  **Sistema de Input do Jogador:** Implementar um sistema para capturar inputs do teclado/controle.
2.  **Processamento de Comandos:** Utilizar os dados parseados do `.CMD` para detectar quando um jogador executa um comando específico.
3.  **Máquina de Estados (State Machine):**
    *   Começar a implementar a lógica da máquina de estados baseada nos dados do `.CNS`.
    *   Permitir que o personagem mude de estado (ex: de parado para andando, de andando para um ataque) com base em inputs (comandos) ou triggers internos.
    *   Executar `StateControllers` básicos (como `ChangeAnim`, `ChangeState`, `VelSet`).
4.  **Física e Movimentação Básica:** Implementar a aplicação de velocidade e a movimentação básica do personagem conforme definido nos estados do CNS.
5.  **Renderização de Caixas de Colisão (Debug):** Adicionar a capacidade de visualizar as caixas de colisão (Clsn2) definidas nos arquivos AIR/CNS para auxiliar no desenvolvimento do sistema de combate.

## Como Executar

1.  Clone este repositório.
2.  Abra o projeto na versão adequada do Unity Editor (verificar `ProjectVersion.txt`).
3.  Abra a cena principal (provavelmente localizada em `Assets/Scenes/`).
4.  Execute a cena. O personagem KFM deve carregar e iniciar sua animação de "parado".

**Nota:** Certifique-se de que a pasta `mugen_2010` (contendo os assets do MUGEN como personagens e cenários) esteja presente na raiz do projeto Unity (ao lado da pasta `Assets`). O script `StartGame.cs` atualmente espera essa estrutura para localizar os arquivos do personagem KFM.
