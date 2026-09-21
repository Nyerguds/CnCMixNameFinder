using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CnCMixNameFinder.Domain
{
    public interface NameFinderReporter
    {
        void ShowStatus(NameFinder origin, ProcessingStatus status, String currentKey, String currentStr, Int32 keyLength);
        void ShowStatus(NameFinder origin, ProcessingStatus status, String currentStr, Int32 keyLength);
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
