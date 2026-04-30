using System;
using System.Collections.Generic;
using RicochetTanks.Features.UI.Core;
using RicochetTanks.Features.UI.Services.Interfaces;

namespace RicochetTanks.Features.UI.Services.Mocks
{
    public sealed class MockRoomService : IRoomService
    {
        private const string LocalPlayerId = "local-player";

        public event Action<RoomSnapshot> RoomChanged;
        public event Action<GameplayHudSnapshot> GameplayHudChanged;
        public event Action<ResultSnapshot> ResultChanged;

        public RoomSnapshot CurrentRoom { get; private set; }
        public GameplayHudSnapshot CurrentHud { get; private set; }
        public ResultSnapshot CurrentResult { get; private set; }

        public MockRoomService()
        {
            CurrentHud = new GameplayHudSnapshot(100f, 100f, 100f, 100f, 1f, "No hits yet", 0);
            CurrentResult = new ResultSnapshot(MatchResultType.None, "No result", "Match has not finished.");
        }

        public void OpenRoom(RoomSnapshot room)
        {
            CurrentRoom = room;
            RoomChanged?.Invoke(CurrentRoom);
        }

        public void SetReady(bool isReady)
        {
            if (CurrentRoom == null)
            {
                return;
            }

            CurrentRoom = CopyRoomWithReadyState(isReady, "Waiting");
            RoomChanged?.Invoke(CurrentRoom);
        }

        public void StartMatch()
        {
            if (CurrentRoom == null)
            {
                return;
            }

            CurrentRoom = CopyRoomWithState("LoadingMatch");
            RoomChanged?.Invoke(CurrentRoom);
            CurrentHud = new GameplayHudSnapshot(100f, 100f, 100f, 100f, 1f, "Ready", 0);
            GameplayHudChanged?.Invoke(CurrentHud);
        }

        public void LeaveRoom()
        {
            CurrentRoom = null;
            RoomChanged?.Invoke(CurrentRoom);
        }

        public void FinishMatch(MatchResultType result)
        {
            CurrentResult = CreateResult(result);
            ResultChanged?.Invoke(CurrentResult);
        }

        private RoomSnapshot CopyRoomWithReadyState(bool isReady, string stateText)
        {
            var players = new List<RoomPlayerSlot>();
            for (var i = 0; i < CurrentRoom.Players.Count; i++)
            {
                var player = CurrentRoom.Players[i];
                if (player.PlayerId == LocalPlayerId)
                {
                    players.Add(new RoomPlayerSlot(player.PlayerId, player.DisplayName, isReady, player.IsHost));
                }
                else
                {
                    players.Add(player);
                }
            }

            return new RoomSnapshot(
                CurrentRoom.RoomCode,
                CurrentRoom.RoomName,
                CurrentRoom.Region,
                CurrentRoom.MaxPlayers,
                players.AsReadOnly(),
                CurrentRoom.IsHost,
                isReady,
                stateText);
        }

        private RoomSnapshot CopyRoomWithState(string stateText)
        {
            return new RoomSnapshot(
                CurrentRoom.RoomCode,
                CurrentRoom.RoomName,
                CurrentRoom.Region,
                CurrentRoom.MaxPlayers,
                CurrentRoom.Players,
                CurrentRoom.IsHost,
                CurrentRoom.IsLocalReady,
                stateText);
        }

        private static ResultSnapshot CreateResult(MatchResultType result)
        {
            switch (result)
            {
                case MatchResultType.Victory:
                    return new ResultSnapshot(result, "Victory", "Sandbox result placeholder.");
                case MatchResultType.Defeat:
                    return new ResultSnapshot(result, "Defeat", "Sandbox result placeholder.");
                case MatchResultType.Draw:
                    return new ResultSnapshot(result, "Draw", "Sandbox result placeholder.");
                default:
                    return new ResultSnapshot(result, "Finished", "Sandbox result placeholder.");
            }
        }
    }
}
