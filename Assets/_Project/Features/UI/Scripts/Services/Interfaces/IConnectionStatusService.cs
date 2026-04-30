using System;
using RicochetTanks.Features.UI.Core;

namespace RicochetTanks.Features.UI.Services.Interfaces
{
    public interface IConnectionStatusService
    {
        event Action<ConnectionStatusSnapshot> StatusChanged;

        ConnectionStatusSnapshot CurrentStatus { get; }

        void Connect();
        void Disconnect();
        void MarkConnectionLost();
    }
}
