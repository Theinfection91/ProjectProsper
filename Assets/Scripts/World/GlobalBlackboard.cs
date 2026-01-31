using System;
using System.Collections.Generic;
using System.Text;

namespace Assets.Scripts.World
{
    public static class GlobalBlackboard
    {
        private static readonly Dictionary<WorldFlagSO, bool> _worldFlags = new();

        public static void SetFlag(WorldFlagSO flag, bool value)
        {
            if (_worldFlags.ContainsKey(flag))
            {
                _worldFlags[flag] = value;
            }
            else
            {
                _worldFlags.Add(flag, value);
            }
        }

        public static bool GetFlag(WorldFlagSO flag)
        {
            return _worldFlags.TryGetValue(flag, out var value) && value;
        }

        public static bool HasKey(WorldFlagSO flag)
        {
            return _worldFlags.ContainsKey(flag);
        }
    }
}
