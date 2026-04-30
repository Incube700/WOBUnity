using UnityEngine;

namespace RicochetTanks.Statistics
{
    public sealed class PlayerStatisticsRepository
    {
        public const string PlayerPrefsKey = "RicochetTanks.PlayerStatistics.v1";

        public PlayerStatisticsData Load()
        {
            var json = PlayerPrefs.GetString(PlayerPrefsKey, string.Empty);
            if (string.IsNullOrWhiteSpace(json))
            {
                return new PlayerStatisticsData();
            }

            var data = JsonUtility.FromJson<PlayerStatisticsData>(json);
            return data ?? new PlayerStatisticsData();
        }

        public void Save(PlayerStatisticsData data)
        {
            if (data == null)
            {
                return;
            }

            PlayerPrefs.SetString(PlayerPrefsKey, JsonUtility.ToJson(data));
            PlayerPrefs.Save();
        }

        public void Reset()
        {
            PlayerPrefs.DeleteKey(PlayerPrefsKey);
            PlayerPrefs.Save();
        }
    }
}
