using System;
using System.Collections.Generic;
using UnityEngine;

public static class Prefs
{
    public enum KEY_TYPES
    {
        MUSIC,
        SOUND,
        NEW_SCORE,
        FIRST_GAME,
        GRAPHICS,
        LEVEL,
        XP
    }

    public enum KEY_DATA_TYPES
    {
        STRING,
        INT,
        FLOAT,
        BOOL
    }
    
    public static class KeyTypeExtensions
    {
        private static readonly Dictionary<KEY_TYPES, (string key, KEY_DATA_TYPES type)> keyMap = new()
        {
            { KEY_TYPES.MUSIC, ("musicVolume", KEY_DATA_TYPES.FLOAT) },
            { KEY_TYPES.SOUND, ("soundVolume", KEY_DATA_TYPES.FLOAT) },
            { KEY_TYPES.NEW_SCORE, ("newScore", KEY_DATA_TYPES.BOOL) },
            { KEY_TYPES.FIRST_GAME, ("firstGame", KEY_DATA_TYPES.BOOL) },
            { KEY_TYPES.GRAPHICS, ("graphicsIndex", KEY_DATA_TYPES.INT) },
            { KEY_TYPES.LEVEL, ("level", KEY_DATA_TYPES.INT) },
            { KEY_TYPES.XP, ("xp", KEY_DATA_TYPES.INT) }
        };

        public static string GetKey(KEY_TYPES keyType) => keyMap[keyType].key;
        public static KEY_DATA_TYPES GetDataType(KEY_TYPES keyType) => keyMap[keyType].type;
    }

    public static T GetKey<T>(KEY_TYPES keyType, T defaultValue = default)
    {
        string key = KeyTypeExtensions.GetKey(keyType);
        
        switch (KeyTypeExtensions.GetDataType(keyType))
        {
            case KEY_DATA_TYPES.INT: return (T)(object)PlayerPrefs.GetInt(key, Convert.ToInt32(defaultValue));
            case KEY_DATA_TYPES.FLOAT: return (T)(object)PlayerPrefs.GetFloat(key, Convert.ToSingle(defaultValue));
            case KEY_DATA_TYPES.STRING: return (T)(object)PlayerPrefs.GetString(key, defaultValue?.ToString() ?? "");
            case KEY_DATA_TYPES.BOOL: 
                bool def = defaultValue is bool b && b;
                return (T)(object)(PlayerPrefs.GetInt(key, def ? 1 : 0) != 0);
            default: throw new NotSupportedException();
        }
    }

    public static void SetKey<T>(KEY_TYPES keyType, T value)
    {
        string key = KeyTypeExtensions.GetKey(keyType);
        
        switch (KeyTypeExtensions.GetDataType(keyType))
        {
            case KEY_DATA_TYPES.INT: PlayerPrefs.SetInt(KeyTypeExtensions.GetKey(keyType), Convert.ToInt32(value)); break;
            case KEY_DATA_TYPES.FLOAT: PlayerPrefs.SetFloat(KeyTypeExtensions.GetKey(keyType), Convert.ToSingle(value)); break;
            case KEY_DATA_TYPES.STRING: PlayerPrefs.SetString(KeyTypeExtensions.GetKey(keyType), value?.ToString() ?? ""); break;
            case KEY_DATA_TYPES.BOOL:
                bool b = value is bool bv && bv;
                PlayerPrefs.SetInt(key, b ? 1 : 0);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        
        PlayerPrefs.Save();
    }

    public static bool HasKey(KEY_TYPES keyType)
    {
        string key = KeyTypeExtensions.GetKey(keyType);
        
        return PlayerPrefs.HasKey(key);
    }

    public static bool ExistsOrCreateKey(KEY_TYPES keyType, object value = null)
    {
        string key = KeyTypeExtensions.GetKey(keyType);
        KEY_DATA_TYPES dataType = KeyTypeExtensions.GetDataType(keyType);
        
        if (PlayerPrefs.HasKey(key))
        {
            return true;
        }

        if (value == null)
        {
            switch (dataType)
            {
                case KEY_DATA_TYPES.STRING: value = ""; break;
                case KEY_DATA_TYPES.INT: value = 0; break;
                case KEY_DATA_TYPES.FLOAT: value = 0f; break;
            }
        }

        switch (dataType)
        {
            case KEY_DATA_TYPES.STRING:
                PlayerPrefs.SetString(key, value.ToString());
                break;
            case KEY_DATA_TYPES.INT:
                PlayerPrefs.SetInt(key, Convert.ToInt32(value));
                break;
            case KEY_DATA_TYPES.FLOAT:
                PlayerPrefs.SetFloat(key, Convert.ToSingle(value));
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(dataType), dataType, null);
        }
        
        PlayerPrefs.Save();
        return false;
    }

    public static class ScoreSystem
    {
        public static List<int> BestScores
        {
            get
            {
                List<int> scores = new();
                for (int i = 1; i <= 10; i++)
                {
                    if (PlayerPrefs.HasKey("highScore" + i)) scores.Add(PlayerPrefs.GetInt("highScore" + i));
                }

                if (scores.Count <= 0) return null;

                scores.Sort();
                scores.Reverse();

                return scores;
            }
            set
            {
                if (value == null) return;
            
                for (int i = 1; i < 10; i++)
                {
                    int score = i < value.Count ? value[i] : 0;
                    PlayerPrefs.SetInt("highScore" + (i + 1), score);
                }
            
                PlayerPrefs.Save();
            }
        }

        public static int GetBestScore()
        {
            List<int> scores = BestScores;
            return (scores == null || scores.Count == 0) ? -1 : scores[0];
        }
    }
}