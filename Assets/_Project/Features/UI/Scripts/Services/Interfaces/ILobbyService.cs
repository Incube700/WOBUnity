using System;
using RicochetTanks.Features.UI.Core;

namespace RicochetTanks.Features.UI.Services.Interfaces
{
    public interface ILobbyService
    {
        event Action<LobbySnapshot> LobbyChanged;

        LobbySnapshot CurrentLobby { get; }

        RoomSnapshot QuickMatch();
        RoomSnapshot CreateRoom(string roomName);
        RoomSnapshot JoinRoom(string roomCode);
        void RefreshRooms();
        void LeaveLobby();
    }
}
