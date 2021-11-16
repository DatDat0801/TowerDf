using System;

namespace EW2.Tools
{
    public static class Extensions 
    {
        /// <summary>
        /// Get a substring between two anchor strings, maximal span
        /// </summary>
        /// <param name="s">source string</param>
        /// <param name="from">search from end of this string</param>
        /// <param name="to">to beginning of this string, searching backwards, from end to start of s</param>
        /// <returns>a substring between from and to, maximal span</returns>
        public static string GetFirstStringBetweenStringsCleanup(this string s, string from, string to)
        {
            if (string.IsNullOrEmpty(s) || string.IsNullOrEmpty(from) || string.IsNullOrEmpty(to)) return string.Empty;
        
            int idxFrom = s.IndexOf(from, StringComparison.Ordinal);
            int idxStart = idxFrom + from.Length; //we filter "not found" -1, never get neg number here

            if (idxFrom == -1 || idxStart >= s.Length - 1)
			    return string.Empty;
            
            int idxEnd = s.LastIndexOf(to, StringComparison.Ordinal);
            
            if (idxEnd == -1 || idxEnd <= idxStart) 
                return string.Empty;
            
            return s.Substring(idxStart, idxEnd - idxStart);

        }

		//4:00AM coding journey, testing conditions you may or may not need, buiding from scatch
        /// <summary>
        /// Get a substring between two anchor strings, maximal span
        /// </summary>
        /// <param name="s">source string</param>
        /// <param name="from">search from end of this string</param>
        /// <param name="to">to beginning of this string, searching backwards, from end to start of s</param>
        /// <returns>a substring between from and to, maximal span</returns>
        public static string GetFirstStringBetweenStrings(this string s, string from, string to)
        {
            //edge case
            if (string.IsNullOrEmpty(s) || string.IsNullOrEmpty(from) || string.IsNullOrEmpty(to)) return string.Empty;
            
            int idxFrom = s.IndexOf(from, StringComparison.Ordinal);
            int idxStart = idxFrom + from.Length; //we filter "not found" -1, never race condtn

            if (idxFrom == -1)       
                return string.Empty;
            
            if (idxStart >= s.Length - 1) //for testing combine to 1 line, we combining a idx with a length, and w/ lengths we normally subtract 1 
            {
                Console.WriteLine("r1. idxStart={0} >= (s.Length - 1)={1}", idxStart, s.Length - 1);
                return string.Empty; 
            }

            int idxEnd = s.LastIndexOf(to, StringComparison.Ordinal); 
      
            if (idxEnd == -1 )
                return string.Empty;

            if (idxEnd <= idxStart)
            {
                return string.Empty;
            }

            return s.Substring(idxStart, idxEnd - idxStart);
            
        }


    }
}
