namespace Interfaces
{
    public interface IPlayFabService
    {
        void SaveNickname(string nickname);
        
        void SubmitScore(int score);
        
        void RetrieveLeaderboard();
    }
}