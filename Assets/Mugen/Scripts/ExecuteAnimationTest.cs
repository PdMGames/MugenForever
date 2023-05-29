using MugenForever.AIR;
using MugenForever.IO.AIR;
using MugenForever.IO.PAL;
using MugenForever.IO.SFF;
using MugenForever.Util;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UIElements;

namespace MugenForever
{
    [RequireComponent(typeof(SpriteRenderer))]
    internal class ExecuteAnimationTest : MonoBehaviour
    {

        public string ActionStateNumber;

        public string SffLocal;
        public string PaletteLocal;

        public string AirLocal;

        public HashSet<AIRState> animationStates; // A lista de estados de animação
        private AIRState currentState; // O estado de animação atual
        private int currentFrameIndex; // O índice do quadro atual no estado atual
        private int startLoopIndex; // O índice do quadro onde o loop deve começar
        private float framesToWait; // Quantos frames restantes para esperar

        private SpriteRenderer spriteRenderer; // O SpriteRenderer usado para exibir os sprites

        private Coroutine animationCoroutine; // A corrotina de animação atual

        public float speed = 1f;
        public int currentStateNumber = -1;

        ISFF sffSprite;

        private void Awake()
        {

            // Obtenha o SpriteRenderer deste objeto de jogo
            spriteRenderer = GetComponent<SpriteRenderer>();

            FileStream sffStream = File.OpenRead(SffLocal);
            FileStream palStream = File.OpenRead(PaletteLocal);
            FileStream airStream = File.OpenRead(AirLocal);

            sffSprite = new SFFImpl(sffStream, new PaletteImpl(palStream));

            AIRStateImpl air = new(new ReaderConfig(airStream));
            animationStates = air.States;

        }

        private void Update()
        {
            // Verifique as entradas do usuário e altere o estado da animação de acordo
            if (Input.GetKey(KeyCode.UpArrow) && Input.GetKey(KeyCode.RightArrow))
            {
                SetState(42);
            }
            else if (Input.GetKey(KeyCode.A))
            {
                SetState(210);
            }
            else if (Input.GetKey(KeyCode.LeftArrow))
            {
                SetState(21);
            }
            else if (Input.GetKey(KeyCode.RightArrow))
            {
                SetState(20);
            }
            else if (Input.GetKey(KeyCode.UpArrow))
            {
                //speed = 2f;
                SetState(41);
            }
            else
            {
                //speed = 3f;
                SetState(0);
            }
        }

        public void SetState(int stateNumber)
        {
            // Se o estado desejado for o mesmo que o estado atual, não faça nada
            if (currentStateNumber == stateNumber)
            {
                return;
            }

            // Encontre o estado com o número fornecido
            foreach (AIRState state in animationStates)
            {
                if (state.Action == stateNumber)
                {
                    // Se encontrarmos o estado, defina-o como o estado atual e reinicie o índice do quadro
                    currentState = state;
                    currentFrameIndex = 0;
                    startLoopIndex = 0; // Assumimos que o loop começa no primeiro quadro

                    // Se já temos uma corrotina de animação em execução, pare-a
                    if (animationCoroutine != null)
                    {
                        StopCoroutine(animationCoroutine);
                    }

                    // Inicie a nova animação
                    animationCoroutine = StartCoroutine(AnimateSprites());

                    // Atualize o estado atual
                    currentStateNumber = stateNumber;

                    // Não precisamos procurar mais, então saímos do loop
                    break;
                }
            }
        }

        private IEnumerator AnimateSprites()
        {
            while (true)
            {
                // Mude para o próximo sprite e ajuste a posição do objeto
                AIRFrame frame = currentState.Frames[currentFrameIndex];

                // Se este quadro deve iniciar um loop, salve o índice atual como o índice de início do loop
                if (frame.StartLoop)
                    startLoopIndex = currentFrameIndex;

                var sffSpriteGroup = sffSprite.Spriters[frame.Group];
                var sffSpriteFrame = sffSpriteGroup[frame.Index];
                spriteRenderer.sprite = sffSpriteFrame.Sprite;
                var axisX = ((frame.AxisX / 100f) + (sffSpriteFrame.AxisX / 100f)) * -1;
                var axisY = ((frame.AxisY / 100f) + (sffSpriteFrame.AxisY / 100f));

                GameObject gbPather = this.transform.parent.gameObject;

                spriteRenderer.transform.position = new Vector2(gbPather.transform.position.x +  axisX, gbPather.transform.position.y + axisY);

                // Configure quantos frames devemos esperar
                framesToWait = frame.Time - speed;

                while (framesToWait > 0)
                {
                    // Aguarde o próximo frame
                    yield return null;

                    // Reduza o número de frames restantes para esperar
                    framesToWait--;
                }
               
                // Vá para o próximo sprite na lista
                currentFrameIndex++;

                 // Se chegarmos ao fim da lista ou ao próximo loop, volte para o início do loop
                if (currentFrameIndex >= currentState.Frames.Count)
                    currentFrameIndex = startLoopIndex;

            }
        }

    }
}
