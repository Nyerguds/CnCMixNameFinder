using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace CnCMixNameFinder.Domain
{

    /// <summary>
    /// Brute force generation class for C&amp;C mix filename encoding algorithm.
    /// Brute force generator based on http://janosch.woschitz.org/a-simple-brute-force-algorithm-in-c-sharp/
    /// </summary>
    public class NameFinder
    {
        #region Private constants
        #endregion

        #region Private variables
        protected List<string> m_results;

        protected bool m_isRunning = false;
        protected bool m_isMatched = false;
        protected DateTime m_timeStarted = DateTime.MinValue;
        protected double m_timeTaken = 0;
        // The length of the m_charactersToTest / m_dictionaryToTest array
        // is stored in an additional variable to increase performance
        protected int m_entriesToTestLength = 0;
        protected long m_computedKeys = 0;

        protected bool m_isDictionary;
        protected char[] m_charactersToTest;
        protected string[] m_dictionaryToTest;
        protected bool m_getAllMatches;
        protected bool m_skipSpaceTrim;
        protected int m_spaceIndex;
        protected bool m_addSeparator;
        protected string m_separator;

        protected uint[] m_NameIdsInput;
        protected HashSet<uint> m_NameIds;
        protected string m_StartString;
        protected string m_EndString;
        protected int m_MinLength;
        protected int m_MaxLength;
        protected NameFinderReporter m_Reporter;
        protected object m_RunPausedLock = new object();
        protected bool m_RunPaused;
        protected object m_RunCancelledLock = new object();
        protected bool m_RunCancelled;
        protected bool m_RunCancelledForClose;
        protected string m_CancelString;
        protected string m_CancelState;
        protected HashMethod m_HashGenerator;
        #endregion

        #region Public properties
        public virtual string HashMethodDisplayName => m_HashGenerator?.DisplayName;
        public virtual string HashMethodSimpleName => m_HashGenerator?.SimpleName;
        public virtual bool IsMatched => m_isMatched;
        public virtual List<string> Results => m_results;
        public virtual DateTime TimeStarted => m_timeStarted;
        public virtual double TimeTaken => m_timeTaken;
        public virtual long ComputedKeys => m_computedKeys;
        public virtual uint[] NameIds => m_NameIdsInput.ToArray();
        public virtual bool IsDictionary => m_isDictionary;
        public virtual int MinLength => m_MinLength;
        public virtual int MaxLength => m_MaxLength;
        public virtual string StartString => m_StartString;
        public virtual string EndString => m_EndString;

        public virtual bool RunWasCancelled
        {
            get
            {
                bool val;
                lock (m_RunCancelledLock)
                {
                    val = m_RunCancelled;
                }
                return val;
            }
            protected set
            {
                lock (m_RunCancelledLock)
                {
                    m_RunCancelled = value;
                }
            }
        }

        public virtual bool RunWasCancelledForClose
        {
            get
            {
                bool val;
                lock (m_RunCancelledLock)
                {
                    val = m_RunCancelledForClose;
                }
                return val;
            }
            protected set
            {
                lock (m_RunCancelledLock)
                {
                    m_RunCancelledForClose = value;
                    if (value)
                    {
                        // If run is aborted, immediately disconnect from the reporter, since it is now probably disposed.
                        m_Reporter = null;
                    }
                }
            }
        }

        public virtual bool RunPaused
        {
            get
            {
                bool val;
                lock (m_RunPausedLock)
                {
                    val = m_RunPaused;
                }
                return val;
            }
            set
            {
                lock (m_RunPausedLock)
                {
                    m_RunPaused = value;
                }
            }
        }

        #endregion

        #region Public functions
        public NameFinder(HashMethod method, string startStr, string endStr, char[] characters, uint[] nameIds, int minLength, int maxLength, bool skipSpaceTrimmed, NameFinderReporter reporter)
            : this(method, startStr, endStr, characters, null, nameIds, minLength, maxLength, skipSpaceTrimmed, null, reporter)
        {
        }

        public NameFinder(HashMethod method, string startStr, string endStr, string[] dictionary, uint[] nameIds, int minLength, int maxLength, string separator, NameFinderReporter reporter)
            : this(method, startStr, endStr, null, dictionary, nameIds, minLength, maxLength, false, separator, reporter)
        {
        }

        protected NameFinder(HashMethod method, string startStr, string endStr, char[] characters, string[] dictionary, uint[] nameIds, int minLength, int maxLength, bool skipSpaceTrimmed, string separator, NameFinderReporter reporter)
        {
            m_HashGenerator = method;
            bool caseSensitive = !m_HashGenerator.NeedsUpperCase;
            if (startStr == null)
                startStr = String.Empty;
            if (endStr == null)
                endStr = String.Empty;
            m_results = new List<string>();
            m_StartString = caseSensitive ? startStr : startStr.ToUpperInvariant();
            m_EndString = caseSensitive ? endStr : endStr.ToUpperInvariant();
            m_isDictionary = dictionary != null;
            m_charactersToTest = null;
            m_dictionaryToTest = null;
            if (!m_isDictionary)
            {
                HashSet<char> charactersList = new HashSet<char>();
                for (int i = 0; i < characters.Length; i++)
                {
                    char ch = characters[i];
                    if (!caseSensitive)
                        ch = ch.ToString().ToUpperInvariant()[0];
                    if (!charactersList.Contains(ch))
                        charactersList.Add(ch);
                }
                m_charactersToTest = charactersList.ToArray();
                //Array.Sort(m_charactersToTest);

                // in dictionary, items should not have spaces. So this only applies to char generating.
                m_skipSpaceTrim = skipSpaceTrimmed && Array.IndexOf(m_charactersToTest, ' ') != -1;
                m_addSeparator = false;
            }
            else
            {
                HashSet<string> stringsSet = new HashSet<string>();
                List<string> stringsList = new List<string>();
                for (int i = 0; i < dictionary.Length; i++)
                {
                    string str = dictionary[i];
                    if (!caseSensitive)
                        str = str.ToUpperInvariant();
                    if (!stringsSet.Contains(str))
                    {
                        stringsSet.Add(str);
                        stringsList.Add(str);
                    }
                }
                stringsSet.Clear();
                m_dictionaryToTest = stringsList.ToArray();
                // Don't sort: it makes the initial state impossible to manipulate
                //Array.Sort(m_dictionaryToTest);
                m_skipSpaceTrim = false;
                m_addSeparator = !String.IsNullOrEmpty(separator);
                m_separator = separator;

            }
            m_NameIdsInput = nameIds.ToArray();
            m_NameIds = new HashSet<uint>(nameIds);
            m_MinLength = Math.Max(1, Math.Min(maxLength, minLength));
            m_MaxLength = Math.Max(1, Math.Max(maxLength, minLength));
            m_Reporter = reporter;
        }

        public virtual void FindName(bool getAllMatches, params int[] initialValues)
        {
            if (m_isRunning)
            {
                throw new InvalidOperationException("This instance is already running a search.");
            }
            try
            {
                m_isRunning = true;
                m_timeStarted = DateTime.Now;
                RunWasCancelled = false;
                RunWasCancelledForClose = false;
                m_CancelString = null;
                m_CancelState = null;
                m_getAllMatches = getAllMatches;
                int currentGenLength = m_MinLength;
                int maxGenLength = m_MaxLength;
                // check if the string isn't fully filled
                string empty = GetFullString(new int[0], null);
                if (currentGenLength == 0 && StringMatches(empty, 0))
                {
                    m_isMatched = true;
                    m_results.Add(empty);
                    currentGenLength++;
                }
                if (maxGenLength > 0)
                {
                    if (initialValues != null)
                    {
                        int maxVal = (m_isDictionary ? m_dictionaryToTest.Length : m_charactersToTest.Length) - 1;
                        for (int i = 0; i < initialValues.Length; ++i)
                        {
                            initialValues[i] = Math.Max(0, Math.Min(initialValues[i], maxVal));
                        }
                    }
                    // The length of the array is stored permanently during runtime
                    m_entriesToTestLength = m_charactersToTest == null ? m_dictionaryToTest.Length : m_charactersToTest.Length;
                    while ((m_getAllMatches || !m_isMatched) && currentGenLength <= maxGenLength)
                    {
                        // The estimated length of the password will be increased and every possible key
                        // for this key length will be created and compared against the password
                        StartBruteForce(currentGenLength, initialValues);
                        // Only use initial values once.
                        initialValues = null;
                        if (RunWasCancelled && m_CancelString != null)
                        {
                            if (m_Reporter != null)
                                m_Reporter.ShowStatus(this, ProcessingStatus.ABORTED, m_CancelState, m_CancelString, currentGenLength);
                            break;
                        }
                        currentGenLength++;
                    }
                }
                m_timeTaken = DateTime.Now.Subtract(m_timeStarted).TotalSeconds;
                if (!RunWasCancelled && m_Reporter != null)
                {
                    if (m_results != null && m_results.Count > 0)
                        m_Reporter.ShowStatus(this, ProcessingStatus.ENDED, m_results[m_results.Count - 1], 0);
                    else
                        m_Reporter.ShowStatus(this, ProcessingStatus.ENDED, String.Empty, 0);
                }
            } finally
            {
                m_isRunning = false;
            }
        }

        public virtual void CancelRun(bool forClose)
        {
            if (forClose)
            {
                RunWasCancelledForClose = true;
            }
            RunWasCancelled = true;
        }
        #endregion

        #region Private methods
        /// <summary>
        /// Starts the recursive method which will create the keys via brute force
        /// </summary>
        /// <param name="keyLength">The length of the key</param>
        protected virtual void StartBruteForce(int keyLength, int[] initialValues)
        {
            int[] keyChars = new int[keyLength]; //this.CreateCharArray(keyLength, m_charactersToTest[0]);
            if (initialValues != null)
            {
                int length = Math.Min(keyLength, initialValues.Length);
                for (int i = 0; i < length; i++)
                {
                    keyChars[i] = initialValues[i];
                }
            }
            // The index of the last character will be stored for slight perfomance improvement
            CreateNewKey(0, keyChars, keyLength, keyLength - 1, initialValues != null);
        }

        /// <summary>
        /// This is the main workhorse, it creates new keys and compares them to the password until the password
        /// is matched or all keys of the current key length have been checked
        /// </summary>
        /// <param name="currentEntryPosition">The position of the entry which is replaced by new items currently.</param>
        /// <param name="keyEntries">The current key represented as int array, to be filled ith the array of items to iterate.</param>
        /// <param name="keyLength">The length of the full key, to know when to end.</param>
        protected virtual void CreateNewKey(int currentCharPosition, int[] keyEntries, int keyLength, int indexOfLastChar, bool fromInit)
        {
            int nextCharPosition = currentCharPosition + 1;
            int start = fromInit ? keyEntries[currentCharPosition] : 0;
            // We are looping through the full length of our entries-to-test array
            for (int i = start; i < m_entriesToTestLength; i++)
            {
                if (m_isMatched && !m_getAllMatches)
                    return;
                if (RunWasCancelled && m_CancelString != null)
                    return;
                // The character at the currentCharPosition will be replaced by a new character
                // from the charactersToTest array => a new key combination will be created
                keyEntries[currentCharPosition] = i;

                // The method calls itself recursively until all positions of the key char array have been replaced
                if (currentCharPosition < indexOfLastChar)
                {
                    CreateNewKey(nextCharPosition, keyEntries, keyLength, indexOfLastChar, fromInit);
                    fromInit = false;
                    continue;
                }
                fromInit = false;
                // check if run is paused
                if (RunPaused)
                {
                    string currentString = GetFullString(keyEntries, null);
                    if (m_Reporter != null)
                        m_Reporter.ShowStatus(this, ProcessingStatus.PAUSED, currentString, keyLength);
                    while (RunPaused)
                    {
                        Thread.Sleep(500);
                        if (RunWasCancelled)
                            break;
                    }
                }
                // check if run should be cancelled
                if (RunWasCancelled)
                {
                    m_CancelState = String.Join(",", keyEntries.Select(k => k.ToString()).ToArray());
                    m_CancelString = GetFullString(keyEntries, null);
                    return;
                }
                // If space-ended entries need to be skipped, check if generated part starts or ends with space.
                if (m_skipSpaceTrim && keyEntries[0] == m_spaceIndex || keyEntries[keyEntries.Length - 1] == m_spaceIndex)
                {
                    continue;
                }
                // The char array will be converted to a string and compared to the password.
                // If the password is matched, the loop breaks and the password is stored as result.
                if (StringMatches(keyEntries, keyLength, null, out string res))
                {
                    if (!m_isMatched)
                    {
                        m_isMatched = true;
                    }
                    m_results.Add(res);
                    if (!m_getAllMatches)
                        return;
                }
                // Dictionary only: if 'with spaces' is enabled, also generate the same match with spaces between the words.
                if (m_addSeparator && keyLength > 1 && StringMatches(keyEntries, keyLength, m_separator, out string res2))
                {
                    if (!m_isMatched)
                    {
                        m_isMatched = true;
                    }
                    m_results.Add(res2);
                    if (!m_getAllMatches)
                        return;
                }
            }
        }

        protected virtual bool StringMatches(int[] newStringChars, int keyLength, string separator, out string generated)
        {
            generated = GetFullString(newStringChars, separator);
            return StringMatches(generated, keyLength);
        }

        protected virtual bool StringMatches(string testString, int keyLength)
        {
            // Only show one in every million keys. This is a lot less than it would seem; should show about a key per second (depending on algo and PC speed of course).
            if (m_Reporter != null && (m_computedKeys & 0xFFFFF) == 0)
            {
                m_Reporter.ShowStatus(this, ProcessingStatus.RUNNING, testString, keyLength);
            }
            m_computedKeys++;
            uint computedKey = m_HashGenerator.GetNameIdCorrectCase(testString);
            if (m_NameIds.Contains(computedKey))
            {
                if (m_Reporter != null)
                    m_Reporter.ShowStatus(this, ProcessingStatus.FOUND, computedKey.ToString("X8"), testString, keyLength);
                return true;
            }
            return false;
        }

        protected virtual string GetFullString(int[] newStringChars, string separator)
        {
            int len = newStringChars.Length;
            if (m_isDictionary)
            {
                string[] entries = new string[len];
                for (int i = 0; i < len; i++)
                {
                    entries[i] = m_dictionaryToTest[newStringChars[i]];
                }
                return String.Concat(m_StartString, String.Join(separator ?? String.Empty, entries), m_EndString);
            }
            else
            {
                char[] entries = new char[len];
                for (int i = 0; i < len; i++)
                {
                    entries[i] = m_charactersToTest[newStringChars[i]];
                }
                return String.Concat(m_StartString, new string(entries), m_EndString);
            }
        }
        #endregion
    }
}
