using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CnCMixNameFinder.Domain
{
    public interface NameFinderReporter
    {
        void ShowStatus(ProcessingStatus status, String currentStr, Int32 keyLength);
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
