using System;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace Lidya.Toolkit
{
    public static class StringExtentions
    {
        #region Similarity
        public static int Similarity(this string s, string t)
            {
                int n = s.Length;
                int m = t.Length;
                int[,] d = new int[n + 1, m + 1];

                // Step 1
                if (n == 0)
                {
                    return m;
                }

                if (m == 0)
                {
                    return n;
                }

                // Step 2
                for (int i = 0; i <= n; d[i, 0] = i++)
                {
                }

                for (int j = 0; j <= m; d[0, j] = j++)
                {
                }

                // Step 3
                for (int i = 1; i <= n; i++)
                {
                    //Step 4
                    for (int j = 1; j <= m; j++)
                    {
                        // Step 5
                        int cost = (t[j - 1] == s[i - 1]) ? 0 : 1;

                        // Step 6
                        d[i, j] = Math.Min(
                            Math.Min(d[i - 1, j] + 1, d[i, j - 1] + 1),
                            d[i - 1, j - 1] + cost);
                    }
                }
                // Step 7 fark
                return d[n, m];
            }

        public static double SimilarityRatio(this string FullString1, string FullString2/*, out double WordsRatio, out double RealWordsRatio*/)
        {
            double theResult = 0;
            String[] Splitted1 = FullString1.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            String[] Splitted2 = FullString2.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if (Splitted1.Length < Splitted2.Length)
            {
                String[] Temp = Splitted2;
                Splitted2 = Splitted1;
                Splitted1 = Temp;
            }
            int[,] theScores = new int[Splitted1.Length, Splitted2.Length];//Keep the best scores for each word.0 is the best, 1000 is the starting.
            int[] BestWord = new int[Splitted1.Length];//Index to the best word of Splitted2 for the Splitted1.

            for (int loop = 0; loop < Splitted1.Length; loop++)
            {
                for (int loop1 = 0; loop1 < Splitted2.Length; loop1++) theScores[loop, loop1] = 1000;
                BestWord[loop] = -1;
            }
            int WordsMatched = 0;
            for (int loop = 0; loop < Splitted1.Length; loop++)
            {
                String String1 = Splitted1[loop];
                for (int loop1 = 0; loop1 < Splitted2.Length; loop1++)
                {
                    String String2 = Splitted2[loop1];
                    int LevenshteinDistance = Similarity(String1, String2);
                    theScores[loop, loop1] = LevenshteinDistance;
                    if (BestWord[loop] == -1 || theScores[loop, BestWord[loop]] > LevenshteinDistance) BestWord[loop] = loop1;
                }
            }

            for (int loop = 0; loop < Splitted1.Length; loop++)
            {
                if (theScores[loop, BestWord[loop]] == 1000) continue;
                for (int loop1 = loop + 1; loop1 < Splitted1.Length; loop1++)
                {
                    if (theScores[loop1, BestWord[loop1]] == 1000) continue;//the worst score available, so there are no more words left
                    if (BestWord[loop] == BestWord[loop1])//2 words have the same best word
                    {
                        //The first in order has the advantage of keeping the word in equality
                        if (theScores[loop, BestWord[loop]] <= theScores[loop1, BestWord[loop1]])
                        {
                            theScores[loop1, BestWord[loop1]] = 1000;
                            int CurrentBest = -1;
                            int CurrentScore = 1000;
                            for (int loop2 = 0; loop2 < Splitted2.Length; loop2++)
                            {
                                //Find next bestword
                                if (CurrentBest == -1 || CurrentScore > theScores[loop1, loop2])
                                {
                                    CurrentBest = loop2;
                                    CurrentScore = theScores[loop1, loop2];
                                }
                            }
                            BestWord[loop1] = CurrentBest;
                        }
                        else//the latter has a better score
                        {
                            theScores[loop, BestWord[loop]] = 1000;
                            int CurrentBest = -1;
                            int CurrentScore = 1000;
                            for (int loop2 = 0; loop2 < Splitted2.Length; loop2++)
                            {
                                //Find next bestword
                                if (CurrentBest == -1 || CurrentScore > theScores[loop, loop2])
                                {
                                    CurrentBest = loop2;
                                    CurrentScore = theScores[loop, loop2];
                                }
                            }
                            BestWord[loop] = CurrentBest;
                        }

                        loop = -1;
                        break;//recalculate all
                    }
                }
            }
            for (int loop = 0; loop < Splitted1.Length; loop++)
            {
                if (theScores[loop, BestWord[loop]] == 1000) theResult += Splitted1[loop].Length;//All words without a score for best word are max failures
                else
                {
                    theResult += theScores[loop, BestWord[loop]];
                    if (theScores[loop, BestWord[loop]] == 0) WordsMatched++;
                }
            }
            int theLength = (FullString1.Replace(" ", "").Length > FullString2.Replace(" ", "").Length) ? FullString1.Replace(" ", "").Length : FullString2.Replace(" ", "").Length;
            if (theResult > theLength) theResult = theLength;
            theResult = (1 - (theResult / theLength)) * 100;
            //WordsRatio = ((double)WordsMatched / (double)Splitted2.Length) * 100;
            //RealWordsRatio = ((double)WordsMatched / (double)Splitted1.Length) * 100;
            return theResult;
        }

        #endregion

        #region Url Slugger
        // white space, em-dash, en-dash, underscore
        static readonly Regex WordDelimiters = new Regex(@"[\s—–_]", RegexOptions.Compiled);

        // characters that are not valid
        static readonly Regex InvalidChars = new Regex(@"[^a-z0-9\-]", RegexOptions.Compiled);

        static readonly Regex InvalidCharsTR = new Regex(@"[^a-z0-9\-çüöşğı]", RegexOptions.Compiled);

        // multiple hyphens
        static readonly Regex MultipleHyphens = new Regex(@"-{2,}", RegexOptions.Compiled);
        /// See: http://www.siao2.com/2007/05/14/2629747.aspx
        private static string RemoveDiacritics(string stIn)
        {
            string stFormD = stIn.Normalize(NormalizationForm.FormD);
            StringBuilder sb = new StringBuilder();

            for (int ich = 0; ich < stFormD.Length; ich++)
            {
                UnicodeCategory uc = CharUnicodeInfo.GetUnicodeCategory(stFormD[ich]);
                if (uc != UnicodeCategory.NonSpacingMark)
                {
                    sb.Append(stFormD[ich]);
                }
            }

            return (sb.ToString().Normalize(NormalizationForm.FormC));
        }

        public static string UrlSlugger(this string value)
        {
            Func<string, string> TRCharacter = x =>
            x.Replace("İ", "i")
           .Replace("ı", "i")
           .Replace("ş", "s")
           .Replace("ç", "c")
           .Replace("ö", "o")
           .Replace("ü", "u")
           .Replace("ğ", "g")
           .Replace("/", " ");

            // convert to lower case
            value = TRCharacter(value.ToLowerInvariant());

            // remove diacritics (accents)
            value = RemoveDiacritics(value);

            // ensure all word delimiters are hyphens
            value = WordDelimiters.Replace(value, "-");

            // strip out invalid characters
            value = InvalidChars.Replace(value, "");

            // replace multiple hyphens (-) with a single hyphen
            value = MultipleHyphens.Replace(value, "-");

            // trim hyphens (-) from ends
            return value.Trim('-');
        }
        #endregion

        public static string CamelCase(this string str)
        {
            if (string.IsNullOrEmpty(str))
                return str;

            var result = "";
            foreach (var item in str.Split(' '))
            {
                if (!string.IsNullOrEmpty(item) && item.Length > 1)
                {
                    var txt = Char.ToUpper(item[0], new CultureInfo("tr-TR", false)) + item.Substring(1).Replace("I", "ı").ToLower(new CultureInfo("tr-TR", false));
                    result = !string.IsNullOrEmpty(result) ? $"{result} {txt}" : txt;
                }
            }
            return result;
        }
    }
}

