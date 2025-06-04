using UnityEngine;
using MugenForever.Core; // Para InputManager

namespace MugenForever.Core
{
    public class GameManager : MonoBehaviour
    {
        void Awake()
        {
            // Garante que o InputManager seja inicializado ao iniciar o jogo.
            InputManager.Initialize();
        }

        void Update()
        {
            InputManager.Update();
        }
    }
}
