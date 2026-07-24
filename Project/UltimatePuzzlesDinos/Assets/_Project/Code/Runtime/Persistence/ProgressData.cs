using System;
using System.Collections.Generic;

namespace VRMGames.UltimatePuzzlesDinos.Persistence
{
    [Serializable]
    public sealed class LevelStarsData
    {
        public int levelId;
        public int stars;
    }

    [Serializable]
    public sealed class ModeProgressData
    {
        public List<int> unlockedLevelIds = new();
        public List<LevelStarsData> levelStars = new();
    }

    [Serializable]
    public sealed class ProgressData
    {
        public int version = 2;
        public ModeProgressData standard = new();
        public ModeProgressData logic = new();
        public ModeProgressData mosaic = new();

        // Campos heredados del Incremento 013. Se conservan únicamente para migrar
        // partidas de prueba existentes y se ignorarán después de la migración.
        public int standardHighestUnlocked = 1;
        public int logicHighestUnlocked = 1;
        public int mosaicHighestUnlocked = 1;
        public List<string> completedLevels = new();
    }
}
