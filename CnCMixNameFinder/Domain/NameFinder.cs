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
        private List<String> m_results;

        private Boolean m_isMatched = false;
        private Double m_timePassed = 0;
        /* The length of the charactersToTest Array is stored in a
            * additional variable to increase performance  */
        private Int32 m_charactersToTestLength = 0;
        private Int64 m_computedKeys = 0;
        
        private Char[] m_charactersToTest;
        private Boolean m_getAllMatches;

        private String m_StartString;
        private String m_EndString;
        private UInt32 m_NameId;
        private Int32 m_MinLength;
        private Int32 m_MaxLength;
        private NameFinderReporter m_Reporter;
        private Boolean m_CancelRun;
        private String m_CancelString;
        private HashMethod m_NameGenerator;
        #endregion

        #region Public properties
        public Boolean IsMatched { get { return m_isMatched; } }
        public List<String> Results { get { return m_results; } }
        public Double TimePassed { get { return m_timePassed; } }
        public Int64 ComputedKeys { get { return m_computedKeys; } }
        public Boolean RunWasCancelled { get { return m_CancelRun; } }
        public Boolean RunPaused { get; set; }
        #endregion

        #region Public functions
        public NameFinder(HashMethod method, String startStr, String endStr, Char[] characters, UInt32 nameId, Int32 minLength, Int32 maxLength, NameFinderReporter reporter)
        {
            this.m_NameGenerator = method;
            Boolean cs = !this.m_NameGenerator.NeedsUpperCase;
            if (startStr == null)
                startStr = String.Empty;
            if (endStr == null)
                endStr = String.Empty;
            this.m_results = new List<String>();

            this.m_StartString = cs ? startStr : startStr.ToUpperInvariant();
            this.m_EndString = cs ? endStr : endStr.ToUpperInvariant();
            List<Char> charactersList = new List<Char>();
            for (int i = 0; i < characters.Length; i++)
            {
                Char ch = characters[i];
                if (!cs)
                    ch = ch.ToString().ToUpperInvariant()[0];
                if (!charactersList.Contains(ch))
                    charactersList.Add(ch);
            }
            this.m_charactersToTest = charactersList.ToArray();
            this.m_NameId = nameId;
            this.m_MinLength = Math.Max(1, Math.Min(maxLength, minLength));
            this.m_MaxLength = Math.Max(1, Math.Max(maxLength, minLength));
            this.m_Reporter = reporter;
        }

        public void FindName(Boolean getAllMatches)
        {
            m_CancelRun = false;
            m_CancelString = null;
            m_getAllMatches = getAllMatches;
            Int32 maxGenLength = m_MaxLength;
            DateTime timeStarted = DateTime.Now;
            // check if the string isn't fully filled
            if (maxGenLength == 0 && StringMatches(new Char[0], 0))
            {
                if (!m_isMatched)
                {
                    m_isMatched = true;
                    m_results.Add(this.getFullString(new Char[0]));
                }
                return;
            }

            // The length of the array is stored permanently during runtime
            m_charactersToTestLength = m_charactersToTest.Length;
            Int32 currentGenLength = m_MinLength;
            while ((!m_isMatched || m_getAllMatches) && currentGenLength < maxGenLength)
            {
                // The estimated length of the password will be increased and every possible key
                // for this key length will be created and compared against the password
                this.StartBruteForce(currentGenLength);
                if (m_CancelRun && m_CancelString != null)
                {
                    if (m_Reporter != null)
                        m_Reporter.ShowStatus(ProcessingStatus.ABORTED, m_CancelString, currentGenLength);
                    break;
                }
                currentGenLength++;
            }
            m_timePassed = DateTime.Now.Subtract(timeStarted).TotalSeconds;
            if (!m_CancelRun && m_Reporter != null)
            {
                if (m_results !=null && m_results.Count > 0)
                    m_Reporter.ShowStatus(ProcessingStatus.ENDED, m_results[m_results.Count - 1], 0);
                else
                    m_Reporter.ShowStatus(ProcessingStatus.ENDED, String.Empty, 0);
            }
        }

        public void CancelRun()
        {
            this.m_CancelRun = true;
        }
        #endregion

        #region Private methods
        /// <summary>
        /// Starts the recursive method which will create the keys via brute force
        /// </summary>
        /// <param name="keyLength">The length of the key</param>
        private void StartBruteForce(Int32 keyLength)
        {
            Char[] keyChars = this.CreateCharArray(keyLength, m_charactersToTest[0]);
            // The index of the last character will be stored for slight perfomance improvement
            Int32 indexOfLastChar = keyLength - 1;
            this.CreateNewKey(0, keyChars, keyLength, indexOfLastChar);
        }

        /// <summary>
        /// Creates a new char array of a specific length filled with the defaultChar
        /// </summary>
        /// <param name="length">The length of the array</param>
        /// <param name="defaultChar">The char with whom the array will be filled</param>
        /// <returns></returns>
        private Char[] CreateCharArray(Int32 length, Char defaultChar)
        {
            return (from c in new Char[length] select defaultChar).ToArray();
        }

        /// <summary>
        /// This is the main workhorse, it creates new keys and compares them to the password until the password
        /// is matched or all keys of the current key length have been checked
        /// </summary>
        /// <param name="currentCharPosition">The position of the char which is replaced by new characters currently</param>
        /// <param name="keyChars">The current key represented as char array</param>
        /// <param name="keyLength">The length of the key</param>
        /// <param name="indexOfLastChar">The index of the last character of the key</param>
        private void CreateNewKey(Int32 currentCharPosition, Char[] keyChars, Int32 keyLength, Int32 indexOfLastChar)
        {
            Int32 nextCharPosition = currentCharPosition + 1;
            // We are looping trough the full length of our charactersToTest array
            for (Int32 i = 0; i < m_charactersToTestLength; i++)
            {
                if (m_isMatched && !m_getAllMatches)
                    return;
                if (m_CancelRun && m_CancelString != null)
                    return;
                /* The character at the currentCharPosition will be replaced by a
                    * new character from the charactersToTest array => a new key combination will be created */
                keyChars[currentCharPosition] = m_charactersToTest[i];

                // The method calls itself recursively until all positions of the key char array have been replaced
                if (currentCharPosition < indexOfLastChar)
                {
                    this.CreateNewKey(nextCharPosition, keyChars, keyLength, indexOfLastChar);
                }
                else
                {
                    // A new key has been created, remove this counter to improve performance
                    m_computedKeys++;
                    // check if run is paused
                    if (RunPaused)
                    {
                        String currentString = this.getFullString(keyChars);
                        if (m_Reporter != null)
                            m_Reporter.ShowStatus(ProcessingStatus.PAUSED, currentString, keyLength);
                        while (RunPaused)
                        {
                            Thread.Sleep(500);
                            if (m_CancelRun)
                                break;
                        }
                    }
                    // check if run should be cancelled
                    if (m_CancelRun)
                    {
                        m_CancelString = this.getFullString(keyChars);
                        return;
                    }
                    /* The char array will be converted to a string and compared to the password. If the password
                        * is matched the loop breaks and the password is stored as result. */
                    if (StringMatches(keyChars, keyLength))
                    {
                        if (!m_isMatched)
                        {
                            m_isMatched = true;
                            m_results.Add(this.getFullString(keyChars));
                        }
                        if (!m_getAllMatches)
                            return;
                    }
                }
            }
        }

        private Boolean StringMatches(Char[] newStringChars, Int32 keyLength)
        {
            String testString = this.getFullString(newStringChars);
            // Only show one in every million keys. This is a lot less than it would seem; should show about a key per second (depending on algo and PC speed of course).
            if (m_Reporter != null && (m_computedKeys & 0xFFFFF) == 1)
            {
                m_Reporter.ShowStatus(ProcessingStatus.RUNNING, testString, keyLength);
            }
            if (this.m_NameId == m_NameGenerator.GetNameId(testString, true))
            {
                if (m_Reporter != null)
                    m_Reporter.ShowStatus(ProcessingStatus.FOUND, testString, keyLength);
                return true;
            }
            return false;
        }

        private String getFullString(Char[] newStringChars)
        {
            return String.Format("{0}{1}{2}", m_StartString, new String(newStringChars), m_EndString);
        }
        #endregion
    }
}
