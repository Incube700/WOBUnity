namespace RicochetTanks.Features.UI.Core
{
    public enum UIFlowState
    {
        Disconnected,
        Connecting,
        InLobby,
        InRoom,
        LoadingMatch,
        InMatch,
        MatchFinished,
        ConnectionLost
    }
}
