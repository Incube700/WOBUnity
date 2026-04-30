using System;
using RicochetTanks.Features.UI.Core;

namespace RicochetTanks.Features.UI.Services.Interfaces
{
    public interface IRoomService
    {
        event Action<RoomSnapshot> RoomChanged;
        event Action<GameplayHudSnapshot> GameplayHudChanged;
        event Action<ResultSnapshot> ResultChanged;

        RoomSnapshot CurrentRoom { get; }
        GameplayHudSnapshot CurrentHud { get; }
        ResultSnapshot CurrentResult { get; }

        void OpenRoom(RoomSnapshot room);
        void SetReady(bool isReady);
        void StartMatch();
        void LeaveRoom();
        void FinishMatch(MatchResultType result);
    }
}
