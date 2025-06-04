using UnityEngine;
using MugenForever.Sff;
using MugenForever.AIR;
using System.Collections.Generic; // Para List

namespace MugenForever.Character
{
    public class CharacterAnimator : MonoBehaviour
    {
        private SffInfo sffData;
        private AirData airData;
        private SpriteRenderer spriteRenderer;

        private AirAnimation currentAnimation;
        private int currentFrameIndex;
        private float frameTimer;
        private SffSprite currentSffSprite;

        void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            if (spriteRenderer == null)
            {
                // Adiciona se não existir, embora o StartGame deva garantir isso.
                spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
                Debug.LogWarning("CharacterAnimator: SpriteRenderer não encontrado, adicionando um novo.");
            }
        }

        public void Initialize(SffInfo sff, AirData air)
        {
            sffData = sff;
            airData = air;
            // Debug.Log("CharacterAnimator Initialized.");
        }

        public bool PlayAnimation(int actionNumber)
        {
            if (airData == null || !airData.Animations.TryGetValue(actionNumber, out currentAnimation))
            {
                Debug.LogError($"ANIM_PLAY: Action {actionNumber} não encontrada no AirData.");
                return false;
            }

            if (currentAnimation.Frames == null || currentAnimation.Frames.Count == 0)
            {
                Debug.LogError($"ANIM_PLAY: Action {actionNumber} não contém frames.");
                currentAnimation = null; // Para não tentar processar uma animação vazia
                return false;
            }

            currentFrameIndex = 0;
            frameTimer = 0; // Será setado pelo primeiro frame
            UpdateSpriteForCurrentFrame();
            // Debug.Log($"ANIM_PLAY: Iniciando Action {actionNumber} com {currentAnimation.Frames.Count} frames.");
            return true;
        }

        void Update()
        {
            if (currentAnimation == null || sffData == null || currentAnimation.Frames == null || currentAnimation.Frames.Count == 0)
            {
                return; // Nenhuma animação tocando ou dados não carregados ou animação sem frames
            }

            AirFrame currentAirFrame = currentAnimation.Frames[currentFrameIndex];

            // Se a duração for -1 (loop infinito no frame), não avançar o timer nem o frame.
            if (currentAirFrame.Duration == -1)
            {
                // Debug.Log($"ANIM_UPDATE: Frame {currentFrameIndex} da Action {currentAnimation.ActionNumber} tem duração -1 (infinito).");
                return;
            }

            frameTimer += Time.deltaTime * 60.0f; // Assumindo 60 ticks por segundo (padrão MUGEN)

            if (frameTimer >= currentAirFrame.Duration)
            {
                currentFrameIndex++;
                if (currentFrameIndex >= currentAnimation.Frames.Count)
                {
                    // Animação terminou. Por enquanto, vamos parar no último quadro.
                    // Poderíamos implementar LoopStart aqui ou repetir.
                    // Debug.Log($"ANIM_UPDATE: Animação {currentAnimation.ActionNumber} terminou. Parando no último frame (índice {currentFrameIndex-1}).");
                    currentFrameIndex = currentAnimation.Frames.Count - 1;
                    currentAnimation = null; // Para a animação
                    return;
                }
                frameTimer = 0; // Reseta o timer para o novo frame
                UpdateSpriteForCurrentFrame();
            }
        }

        private void UpdateSpriteForCurrentFrame()
        {
            if (currentAnimation == null || currentFrameIndex < 0 || currentFrameIndex >= currentAnimation.Frames.Count || sffData == null)
            {
                return;
            }

            AirFrame frameToShow = currentAnimation.Frames[currentFrameIndex];

            // Encontra o SffSprite correspondente no SffData
            currentSffSprite = sffData.sprites.Find(s => s.groupNumber == frameToShow.GroupNumber && s.imageNumber == frameToShow.ImageNumber);

            if (currentSffSprite != null && currentSffSprite.sprite != null)
            {
                spriteRenderer.sprite = currentSffSprite.sprite;
                spriteRenderer.flipX = frameToShow.HorizontalFlip;
                spriteRenderer.flipY = frameToShow.VerticalFlip;

                // Garantir FilterMode.Point para a textura do sprite atual
                if (currentSffSprite.texture != null && currentSffSprite.texture.filterMode != FilterMode.Point)
                {
                    currentSffSprite.texture.filterMode = FilterMode.Point;
                }
                // Debug.Log($"ANIM_FRAME: Action {currentAnimation.ActionNumber}, Frame {currentFrameIndex}, Sprite G{frameToShow.GroupNumber},I{frameToShow.ImageNumber}, Dura {frameToShow.Duration}, FlipH: {frameToShow.HorizontalFlip}");
            }
            else
            {
                string spriteDetails = currentSffSprite == null ? "não encontrado no SFF" : "encontrado mas UnityEngine.Sprite é null";
                Debug.LogError($"ANIM_FRAME_ERROR: Sprite G{frameToShow.GroupNumber},I{frameToShow.ImageNumber} {spriteDetails}.");
                spriteRenderer.sprite = null;
            }
        }
    }
}
