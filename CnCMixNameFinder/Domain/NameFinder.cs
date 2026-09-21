using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
        protected List<String> m_results;

        protected Boolean m_isRunning = false;
        protected Boolean m_isMatched = false;
        protected DateTime m_timeStarted = DateTime.MinValue;
        protected Double m_timeTaken = 0;
        // The length of the m_charactersToTest / m_dictionaryToTest array
        // is stored in an additional variable to increase performance
        protected Int32 m_entriesToTestLength = 0;
        protected Int64 m_computedKeys = 0;

        protected Boolean m_isDictionary;
        protected Char[] m_charactersToTest;
        protected String[] m_dictionaryToTest;
        protected Boolean m_getAllMatches;
        protected Boolean m_skipSpaceTrim;
        protected int m_spaceIndex;
        protected Boolean m_addSeparator;
        protected String m_separator;

        protected UInt32[] m_NameIdsInput;
        protected HashSet<UInt32> m_NameIds;
        protected String m_StartString;
        protected String m_EndString;
        protected Int32 m_MinLength;
        protected Int32 m_MaxLength;
        protected NameFinderReporter m_Reporter;
        protected Object m_RunPausedLock = new Object();
        protected Boolean m_RunPaused;
        protected Object m_RunCancelledLock = new Object();
        protected Boolean m_RunCancelled;
        protected Boolean m_RunCancelledForClose;
        protected String m_CancelString;
        protected String m_CancelState;
        protected HashMethod m_HashGenerator;
        #endregion

        #region Public properties
        public virtual String HashMethodDisplayName { get { return m_HashGenerator == null ? null : m_HashGenerator.GetDisplayName(); } }
        public virtual String HashMethodSimpleName { get { return m_HashGenerator == null ? null : m_HashGenerator.GetSimpleName(); } }
        public virtual Boolean IsMatched { get { return m_isMatched; } }
        public virtual List<String> Results { get { return m_results; } }
        public virtual DateTime TimeStarted { get { return m_timeStarted; } }
        public virtual Double TimeTaken { get { return m_timeTaken; } }
        public virtual Int64 ComputedKeys { get { return m_computedKeys; } }
        public virtual UInt32[] NameIds { get { return this.m_NameIdsInput.ToArray(); } }
        public virtual Int32 MinLength { get { return this.m_MinLength; } }
        public virtual Int32 MaxLength { get { return this.m_MaxLength; } }
        public virtual String StartString { get { return this.m_StartString; } }
        public virtual String EndString { get { return this.m_EndString; } }

        public virtual Boolean RunWasCancelled
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

        public virtual Boolean RunWasCancelledForClose
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

        public virtual Boolean RunPaused
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
        public NameFinder(HashMethod method, String startStr, String endStr, Char[] characters, UInt32[] nameIds, Int32 minLength, Int32 maxLength, Boolean skipSpaceTrimmed, NameFinderReporter reporter)
            : this(method, startStr, endStr, characters, null, nameIds, minLength, maxLength, skipSpaceTrimmed, null, reporter)
        {
        }

        public NameFinder(HashMethod method, String startStr, String endStr, String[] dictionary, UInt32[] nameIds, Int32 minLength, Int32 maxLength, String separator, NameFinderReporter reporter)
            : this(method, startStr, endStr, null, dictionary, nameIds, minLength, maxLength, false, separator, reporter)
        {
        }

        protected NameFinder(HashMethod method, String startStr, String endStr, Char[] characters, String[] dictionary, UInt32[] nameIds, Int32 minLength, Int32 maxLength, Boolean skipSpaceTrimmed, String separator, NameFinderReporter reporter)
        {
            this.m_HashGenerator = method;
            Boolean caseSensitive = !this.m_HashGenerator.NeedsUpperCase;
            if (startStr == null)
                startStr = String.Empty;
            if (endStr == null)
                endStr = String.Empty;
            this.m_results = new List<String>();
            this.m_StartString = caseSensitive ? startStr : startStr.ToUpperInvariant();
            this.m_EndString = caseSensitive ? endStr : endStr.ToUpperInvariant();
            this.m_isDictionary = dictionary != null;
            this.m_charactersToTest = null;
            this.m_dictionaryToTest = null;
            if (!m_isDictionary)
            {
                HashSet<Char> charactersList = new HashSet<Char>();
                for (int i = 0; i < characters.Length; i++)
                {
                    Char ch = characters[i];
                    if (!caseSensitive)
                        ch = ch.ToString().ToUpperInvariant()[0];
                    if (!charactersList.Contains(ch))
                        charactersList.Add(ch);
                }
                this.m_charactersToTest = charactersList.ToArray();
                //Array.Sort(m_charactersToTest);

                // in dictionary, items should not have spaces. So this only applies to char generating.
                m_skipSpaceTrim = skipSpaceTrimmed && Array.IndexOf(m_charactersToTest, ' ') != -1;
                m_addSeparator = false;
            }
            else
            {
                HashSet<String> stringsSet = new HashSet<String>();
                List<String> stringsList = new List<String>();
                for (int i = 0; i < dictionary.Length; i++)
                {
                    String str = dictionary[i];
                    if (!caseSensitive)
                        str = str.ToUpperInvariant();
                    if (!stringsSet.Contains(str))
                    {
                        stringsSet.Add(str);
                        stringsList.Add(str);
                    }
                }
                stringsSet.Clear();
                this.m_dictionaryToTest = stringsList.ToArray();
                // Don't sort: it makes the initial state impossible to manipulate
                //Array.Sort(m_dictionaryToTest);
                m_skipSpaceTrim = false;
                this.m_addSeparator = !String.IsNullOrEmpty(separator);
                this.m_separator = separator;

            }
            this.m_NameIdsInput = nameIds.ToArray();
            this.m_NameIds = new HashSet<uint>(nameIds);
            this.m_MinLength = Math.Max(1, Math.Min(maxLength, minLength));
            this.m_MaxLength = Math.Max(1, Math.Max(maxLength, minLength));
            this.m_Reporter = reporter;
        }

        public virtual void FindName(Boolean getAllMatches, params int[] initialValues)
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
                Int32 currentGenLength = m_MinLength;
                Int32 maxGenLength = m_MaxLength;
                // check if the string isn't fully filled
                String empty = this.GetFullString(new int[0], null);
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
                        int maxVal = (this.m_isDictionary ? m_dictionaryToTest.Length : m_charactersToTest.Length) - 1;
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
                        this.StartBruteForce(currentGenLength, initialValues);
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

        public virtual void CancelRun(Boolean forClose)
        {
            if (forClose)
            {
                this.RunWasCancelledForClose = true;
            }
            this.RunWasCancelled = true;
        }
        #endregion

        #region Private methods
        /// <summary>
        /// Starts the recursive method which will create the keys via brute force
        /// </summary>
        /// <param name="keyLength">The length of the key</param>
        protected virtual void StartBruteForce(Int32 keyLength, int[] initialValues)
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
            this.CreateNewKey(0, keyChars, keyLength, keyLength - 1, initialValues != null);
        }

        /// <summary>
        /// This is the main workhorse, it creates new keys and compares them to the password until the password
        /// is matched or all keys of the current key length have been checked
        /// </summary>
        /// <param name="currentEntryPosition">The position of the entry which is replaced by new items currently.</param>
        /// <param name="keyEntries">The current key represented as int array, to be filled ith the array of items to iterate.</param>
        /// <param name="keyLength">The length of the full key, to know when to end.</param>
        protected virtual void CreateNewKey(Int32 currentCharPosition, int[] keyEntries, Int32 keyLength, Int32 indexOfLastChar, bool fromInit)
        {
            Int32 nextCharPosition = currentCharPosition + 1;
            int start = fromInit ? keyEntries[currentCharPosition] : 0;
            // We are looping through the full length of our entries-to-test array
            for (Int32 i = start; i < m_entriesToTestLength; i++)
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
                    this.CreateNewKey(nextCharPosition, keyEntries, keyLength, indexOfLastChar, fromInit);
                    fromInit = false;
                    continue;
                }
                fromInit = false;
                // check if run is paused
                if (RunPaused)
                {
                    String currentString = this.GetFullString(keyEntries, null);
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
                    m_CancelString = this.GetFullString(keyEntries, null);
                    return;
                }
                // If space-ended entries need to be skipped, check if generated part starts or ends with space.
                if (m_skipSpaceTrim && keyEntries[0] == m_spaceIndex || keyEntries[keyEntries.Length - 1] == m_spaceIndex)
                {
                    continue;
                }
                // The char array will be converted to a string and compared to the password.
                // If the password is matched, the loop breaks and the password is stored as result.
                if (StringMatches(keyEntries, keyLength, null, out String res))
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
                if (m_addSeparator && keyLength > 1 && StringMatches(keyEntries, keyLength, m_separator, out String res2))
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

        protected virtual Boolean StringMatches(int[] newStringChars, Int32 keyLength, string separator, out String generated)
        {
            generated = this.GetFullString(newStringChars, separator);
            return StringMatches(generated, keyLength);
        }

        protected virtual Boolean StringMatches(String testString, Int32 keyLength)
        {
            // Only show one in every million keys. This is a lot less than it would seem; should show about a key per second (depending on algo and PC speed of course).
            if (m_Reporter != null && (m_computedKeys & 0xFFFFF) == 0)
            {
                m_Reporter.ShowStatus(this, ProcessingStatus.RUNNING, testString, keyLength);
            }
            m_computedKeys++;
            UInt32 computedKey = m_HashGenerator.GetNameIdCorrectCase(testString);
            if (this.m_NameIds.Contains(computedKey))
            {
                if (m_Reporter != null)
                    m_Reporter.ShowStatus(this, ProcessingStatus.FOUND, computedKey.ToString("X8"), testString, keyLength);
                return true;
            }
            return false;
        }

        protected virtual String GetFullString(int[] newStringChars, string separator)
        {
            int len = newStringChars.Length;
            if (m_isDictionary)
            {
                String[] entries = new String[len];
                for (int i = 0; i < len; i++)
                {
                    entries[i] = m_dictionaryToTest[newStringChars[i]];
                }
                return String.Concat(m_StartString, String.Join(separator ?? String.Empty, entries), m_EndString);
            }
            else
            {
                Char[] entries = new Char[len];
                for (int i = 0; i < len; i++)
                {
                    entries[i] = m_charactersToTest[newStringChars[i]];
                }
                return String.Concat(m_StartString, new String(entries), m_EndString);
            }
        }
        #endregion
    }
}
