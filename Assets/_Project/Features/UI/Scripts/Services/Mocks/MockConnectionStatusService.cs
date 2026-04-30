using System;
using RicochetTanks.Features.UI.Core;
using RicochetTanks.Features.UI.Services.Interfaces;

namespace RicochetTanks.Features.UI.Services.Mocks
{
    public sealed class MockConnectionStatusService : IConnectionStatusService
    {
        public event Action<ConnectionStatusSnapshot> StatusChanged;

        public ConnectionStatusSnapshot CurrentStatus { get; private set; }

        public MockConnectionStatusService()
        {
            CurrentStatus = new ConnectionStatusSnapshot(UIFlowState.Disconnected, "Disconnected", -1);
        }

        public void Connect()
        {
            SetStatus(new ConnectionStatusSnapshot(UIFlowState.Connecting, "Connecting to Auto/Asia", -1));
            SetStatus(new ConnectionStatusSnapshot(UIFlowState.InLobby, "Connected: Auto/Asia", 42));
        }

        public void Disconnect()
        {
            SetStatus(new ConnectionStatusSnapshot(UIFlowState.Disconnected, "Disconnected", -1));
        }

        public void MarkConnectionLost()
        {
            SetStatus(new ConnectionStatusSnapshot(UIFlowState.ConnectionLost, "Connection lost", -1));
        }

        private void SetStatus(ConnectionStatusSnapshot status)
        {
            CurrentStatus = status;
            StatusChanged?.Invoke(CurrentStatus);
        }
    }
}
