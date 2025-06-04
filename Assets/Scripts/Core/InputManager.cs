using UnityEngine;
using System.Collections.Generic;

namespace MugenForever.Core
{
    public enum MugenInputKey
    {
        Up, Down, Left, Right,
        A, B, C, X, Y, Z,
        Start
    }

    public static class InputManager
    {
        private static Dictionary<MugenInputKey, KeyCode> keyMappings;

        private static Dictionary<MugenInputKey, bool> heldKeys;
        private static Dictionary<MugenInputKey, bool> pressedKeys;
        private static Dictionary<MugenInputKey, bool> releasedKeys;

        private static bool isInitialized = false;

        public static void Initialize()
        {
            if (isInitialized) return;

            keyMappings = new Dictionary<MugenInputKey, KeyCode>
            {
                { MugenInputKey.Up, KeyCode.UpArrow },
                { MugenInputKey.Down, KeyCode.DownArrow },
                { MugenInputKey.Left, KeyCode.LeftArrow },
                { MugenInputKey.Right, KeyCode.RightArrow },
                { MugenInputKey.A, KeyCode.A },
                { MugenInputKey.B, KeyCode.S },
                { MugenInputKey.C, KeyCode.D },
                { MugenInputKey.X, KeyCode.Z },
                { MugenInputKey.Y, KeyCode.X },
                { MugenInputKey.Z, KeyCode.C },
                { MugenInputKey.Start, KeyCode.Return } // Enter key
            };

            heldKeys = new Dictionary<MugenInputKey, bool>();
            pressedKeys = new Dictionary<MugenInputKey, bool>();
            releasedKeys = new Dictionary<MugenInputKey, bool>();

            foreach (MugenInputKey key in System.Enum.GetValues(typeof(MugenInputKey)))
            {
                heldKeys[key] = false;
                pressedKeys[key] = false;
                releasedKeys[key] = false;
            }
            isInitialized = true;
            Debug.Log("InputManager Initialized with default mappings.");
        }

        public static void Update()
        {
            if (!isInitialized)
            {
                Initialize();
            }

            foreach (MugenInputKey mKey in System.Enum.GetValues(typeof(MugenInputKey)))
            {
                if (keyMappings.TryGetValue(mKey, out KeyCode keyCode))
                {
                    pressedKeys[mKey] = Input.GetKeyDown(keyCode);
                    releasedKeys[mKey] = Input.GetKeyUp(keyCode);
                    heldKeys[mKey] = Input.GetKey(keyCode);
                }
                else
                {
                    // Should not happen if Initialize correctly maps all MugenInputKeys
                    pressedKeys[mKey] = false;
                    releasedKeys[mKey] = false;
                    heldKeys[mKey] = false;
                }
            }
        }

        public static bool IsHeld(MugenInputKey key)
        {
            // Ensure initialized before trying to access, though Update should handle this.
            if (!isInitialized) Initialize();
            if (heldKeys.TryGetValue(key, out bool value)) return value;
            return false;
        }

        public static bool IsPressed(MugenInputKey key)
        {
            if (!isInitialized) Initialize();
            if (pressedKeys.TryGetValue(key, out bool value)) return value;
            return false;
        }

        public static bool IsReleased(MugenInputKey key)
        {
            if (!isInitialized) Initialize();
            if (releasedKeys.TryGetValue(key, out bool value)) return value;
            return false;
        }
    }
}
