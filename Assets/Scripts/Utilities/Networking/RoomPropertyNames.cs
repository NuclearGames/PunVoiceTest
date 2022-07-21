namespace Utilities.Networking {
    internal static class RoomPropertyNames {
        internal const string TEAMS_COUNT = "TC";
        internal const string TEAM_MAX_SIZE = "TS";
        
        internal static string GetPlayerTeamIndexName(int actorNumber) {
            return $"P{actorNumber}_T";
        }

        internal static string GetPlayerInterestGroupName(int actorNumber) {
            return $"P{actorNumber}_IG";
        }
    }
}