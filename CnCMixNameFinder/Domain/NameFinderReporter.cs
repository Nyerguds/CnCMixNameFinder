namespace CnCMixNameFinder.Domain
{
    public interface NameFinderReporter
    {
        void ShowStatus(NameFinder origin, ProcessingStatus status, string currentKey, string currentStr, int keyLength);
        void ShowStatus(NameFinder origin, ProcessingStatus status, string currentStr, int keyLength);
    }

    public enum ProcessingStatus
    {
        RUNNING,
        FOUND,
        ABORTED,
        ENDED,
        PAUSED
    }
}
