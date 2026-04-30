using System;
using RicochetTanks.Features.UI.Core;

namespace RicochetTanks.Features.UI.Services.Interfaces
{
    public interface IScreenService
    {
        event Action<UIFlowState> StateChanged;

        UIFlowState CurrentState { get; }

        void ShowMainMenu();
        void ShowLobby();
        void ShowRoom();
        void ShowGameplayHud();
        void ShowResult();
        void SetState(UIFlowState state);
    }
}
