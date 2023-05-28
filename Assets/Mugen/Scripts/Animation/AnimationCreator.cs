using Assets.Scripts.IO.SFF;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;
using static MugenForever.AIR.AIRImpl;

namespace MugenForever.Mugen.Animation
{
    internal class AnimationCreator : MonoBehaviour
    {
        [MenuItem("Custom/Create Animation")]
        public static void CreateAnimation()
        {
            // Cria uma instância de AnimationClip
            AnimationClip clip = new AnimationClip();
            clip.frameRate = 60;  // Taxa de quadros da animação.

            // Dados de animação temporários (substitua pelo seus próprios dados)
            List<AIRFrame> frames = new List<AIRFrame>();
            Dictionary<int, Dictionary<int, SFFSprite>> spriters = new Dictionary<int, Dictionary<int, SFFSprite>>();
            // ...adicionar valores a frames e spriters

            // Cria as curvas de animação para cada propriedade que você deseja animar.
            AnimationCurve curveX = new AnimationCurve();
            AnimationCurve curveY = new AnimationCurve();

            for (int i = 0; i < frames.Count; i++)
            {
                AIRFrame frame = frames[i];
                if (spriters.TryGetValue(frame.Group, out Dictionary<int, SFFSprite> group))
                {
                    if (group.TryGetValue(frame.Index, out SFFSprite sprite))
                    {
                        // Adiciona um keyframe para cada propriedade que você deseja animar.
                        curveX.AddKey(new Keyframe(i, sprite.AxisX));
                        curveY.AddKey(new Keyframe(i, sprite.AxisY));
                    }
                }
            }

            // Adiciona as curvas ao clip.
            clip.SetCurve("", typeof(Transform), "localPosition.x", curveX);
            clip.SetCurve("", typeof(Transform), "localPosition.y", curveY);

            // Salva o clip
            AssetDatabase.CreateAsset(clip, "Assets/MyAnimation.anim");

            // Cria o controlador
            AnimatorController controller = AnimatorController.CreateAnimatorControllerAtPath("Assets/MyController.controller");

            // Adiciona o clip ao controlador
            AnimatorState state = controller.layers[0].stateMachine.AddState("MyAnimation");
            state.motion = clip;
        }
    }
}
