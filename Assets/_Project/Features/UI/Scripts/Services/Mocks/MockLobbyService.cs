using System;
using System.Collections.Generic;
using RicochetTanks.Features.UI.Core;
using RicochetTanks.Features.UI.Services.Interfaces;

namespace RicochetTanks.Features.UI.Services.Mocks
{
    public sealed class MockLobbyService : ILobbyService
    {
        private const int MaxPlayers = 2;
        private const string Region = "Auto/Asia";

        private readonly List<RoomSummary> _rooms = new List<RoomSummary>();
        private int _roomCounter = 1000;

        public event Action<LobbySnapshot> LobbyChanged;

        public LobbySnapshot CurrentLobby { get; private set; }

        public MockLobbyService()
        {
            _rooms.Add(new RoomSummary("A7K2", "Training Duel", 1, MaxPlayers, Region));
            _rooms.Add(new RoomSummary("B4R9", "Greybox Arena", 2, MaxPlayers, Region));
            CurrentLobby = new LobbySnapshot(_rooms.AsReadOnly(), "Room List placeholder");
        }

        public RoomSnapshot QuickMatch()
        {
            RefreshRooms();
            return CreateJoinedRoom("Quick Match", "QM" + _roomCounter);
        }

        public RoomSnapshot CreateRoom(string roomName)
        {
            var displayName = string.IsNullOrWhiteSpace(roomName) ? "Player Room" : roomName.Trim();
            var roomCode = "RT" + _roomCounter;
            _roomCounter++;
            _rooms.Insert(0, new RoomSummary(roomCode, displayName, 1, MaxPlayers, Region));
            SetLobby("Created room " + roomCode);
            return CreateHostRoom(displayName, roomCode);
        }

        public RoomSnapshot JoinRoom(string roomCode)
        {
            if (string.IsNullOrWhiteSpace(roomCode))
            {
                SetLobby("Enter a room code");
                return null;
            }

            var normalizedCode = roomCode.Trim().ToUpperInvariant();
            SetLobby("Joined room " + normalizedCode);
            return CreateJoinedRoom("Joined Duel", normalizedCode);
        }

        public void RefreshRooms()
        {
            SetLobby("Room List placeholder");
        }

        public void LeaveLobby()
        {
            SetLobby("Left lobby");
        }

        private RoomSnapshot CreateHostRoom(string roomName, string roomCode)
        {
            var players = new List<RoomPlayerSlot>
            {
                new RoomPlayerSlot("local-player", "Player", false, true)
            };

            return new RoomSnapshot(roomCode, roomName, Region, MaxPlayers, players.AsReadOnly(), true, false, "Waiting");
        }

        private RoomSnapshot CreateJoinedRoom(string roomName, string roomCode)
        {
            var players = new List<RoomPlayerSlot>
            {
                new RoomPlayerSlot("host-player", "Host", true, true),
                new RoomPlayerSlot("local-player", "Player", false, false)
            };

            return new RoomSnapshot(roomCode, roomName, Region, MaxPlayers, players.AsReadOnly(), false, false, "Waiting");
        }

        private void SetLobby(string statusText)
        {
            CurrentLobby = new LobbySnapshot(_rooms.AsReadOnly(), statusText);
            LobbyChanged?.Invoke(CurrentLobby);
        }
    }
}
