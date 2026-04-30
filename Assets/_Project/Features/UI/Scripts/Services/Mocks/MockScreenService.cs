using System;
using RicochetTanks.Features.UI.Core;
using RicochetTanks.Features.UI.Services.Interfaces;

namespace RicochetTanks.Features.UI.Services.Mocks
{
    public sealed class MockScreenService : IScreenService
    {
        public event Action<UIFlowState> StateChanged;

        public UIFlowState CurrentState { get; private set; } = UIFlowState.Disconnected;

        public void ShowMainMenu()
        {
            SetState(UIFlowState.Disconnected);
        }

        public void ShowLobby()
        {
            SetState(UIFlowState.InLobby);
        }

        public void ShowRoom()
        {
            SetState(UIFlowState.InRoom);
        }

        public void ShowGameplayHud()
        {
            SetState(UIFlowState.InMatch);
        }

        public void ShowResult()
        {
            SetState(UIFlowState.MatchFinished);
        }

        public void SetState(UIFlowState state)
        {
            if (CurrentState == state)
            {
                StateChanged?.Invoke(CurrentState);
                return;
            }

            CurrentState = state;
            StateChanged?.Invoke(CurrentState);
        }
    }
}
