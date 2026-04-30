using System;
using System.Collections.Generic;

namespace RicochetTanks.Features.UI.Core
{
    public enum MatchResultType
    {
        None,
        Victory,
        Defeat,
        Draw
    }

    [Serializable]
    public sealed class ConnectionStatusSnapshot
    {
        public ConnectionStatusSnapshot(UIFlowState state, string statusText, int pingMs)
        {
            State = state;
            StatusText = statusText;
            PingMs = pingMs;
        }

        public UIFlowState State { get; }
        public string StatusText { get; }
        public int PingMs { get; }
        public string PingText
        {
            get { return PingMs >= 0 ? PingMs + " ms" : "-- ms"; }
        }
    }

    [Serializable]
    public sealed class RoomPlayerSlot
    {
        public RoomPlayerSlot(string playerId, string displayName, bool isReady, bool isHost)
        {
            PlayerId = playerId;
            DisplayName = displayName;
            IsReady = isReady;
            IsHost = isHost;
        }

        public string PlayerId { get; }
        public string DisplayName { get; }
        public bool IsReady { get; }
        public bool IsHost { get; }
    }

    [Serializable]
    public sealed class RoomSummary
    {
        public RoomSummary(string roomCode, string roomName, int playerCount, int maxPlayers, string region)
        {
            RoomCode = roomCode;
            RoomName = roomName;
            PlayerCount = playerCount;
            MaxPlayers = maxPlayers;
            Region = region;
        }

        public string RoomCode { get; }
        public string RoomName { get; }
        public int PlayerCount { get; }
        public int MaxPlayers { get; }
        public string Region { get; }
    }

    [Serializable]
    public sealed class LobbySnapshot
    {
        public LobbySnapshot(IReadOnlyList<RoomSummary> rooms, string statusText)
        {
            Rooms = rooms;
            StatusText = statusText;
        }

        public IReadOnlyList<RoomSummary> Rooms { get; }
        public string StatusText { get; }
    }

    [Serializable]
    public sealed class RoomSnapshot
    {
        public RoomSnapshot(
            string roomCode,
            string roomName,
            string region,
            int maxPlayers,
            IReadOnlyList<RoomPlayerSlot> players,
            bool isHost,
            bool isLocalReady,
            string stateText)
        {
            RoomCode = roomCode;
            RoomName = roomName;
            Region = region;
            MaxPlayers = maxPlayers;
            Players = players;
            IsHost = isHost;
            IsLocalReady = isLocalReady;
            StateText = stateText;
        }

        public string RoomCode { get; }
        public string RoomName { get; }
        public string Region { get; }
        public int MaxPlayers { get; }
        public IReadOnlyList<RoomPlayerSlot> Players { get; }
        public bool IsHost { get; }
        public bool IsLocalReady { get; }
        public string StateText { get; }
        public int PlayerCount
        {
            get { return Players != null ? Players.Count : 0; }
        }

        public bool CanStart
        {
            get { return IsHost && PlayerCount == MaxPlayers && ArePlayersReady(); }
        }

        private bool ArePlayersReady()
        {
            if (Players == null || Players.Count == 0)
            {
                return false;
            }

            for (var i = 0; i < Players.Count; i++)
            {
                if (!Players[i].IsReady)
                {
                    return false;
                }
            }

            return true;
        }
    }

    [Serializable]
    public sealed class GameplayHudSnapshot
    {
        public GameplayHudSnapshot(
            float playerHp,
            float playerMaxHp,
            float enemyHp,
            float enemyMaxHp,
            float reloadProgress,
            string lastHitResult,
            int ricochetCount)
        {
            PlayerHp = playerHp;
            PlayerMaxHp = playerMaxHp;
            EnemyHp = enemyHp;
            EnemyMaxHp = enemyMaxHp;
            ReloadProgress = reloadProgress;
            LastHitResult = lastHitResult;
            RicochetCount = ricochetCount;
        }

        public float PlayerHp { get; }
        public float PlayerMaxHp { get; }
        public float EnemyHp { get; }
        public float EnemyMaxHp { get; }
        public float ReloadProgress { get; }
        public string LastHitResult { get; }
        public int RicochetCount { get; }
    }

    [Serializable]
    public sealed class ResultSnapshot
    {
        public ResultSnapshot(MatchResultType result, string headline, string details)
        {
            Result = result;
            Headline = headline;
            Details = details;
        }

        public MatchResultType Result { get; }
        public string Headline { get; }
        public string Details { get; }
    }
}
