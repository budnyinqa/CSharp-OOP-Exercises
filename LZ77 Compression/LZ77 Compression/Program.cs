using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace LZ77_Compression
{
    class Program
    {
        static void Main(string[] args)
        {
            // Create a variable with the string to compress
            string input = "DCDCACCABADAAACBABBDADBCBCABBCAAACCDCABABBCDCABAAAACCADACCBBCDADCBDBDDDABCCACCDAABDADBCBAAADCBAAABCCCAABDDDCAADCBCCCBDCBAABADDCCDDDDDDCCBCDCDCAACBCCDCADBDACCBBCBAACBCDDBCDDCADBBBCADACCDBABDADBDDDACCACABACADDAACDBCAACADBCDADDACA";

            // Compression
            List<string> compressedTokens = new List<string>(); // Create a list to store the results of the compression method
            int dictionaryLength = 7; // Set the dictionary length
            int bufferLength = 8; // Set the buffer length
            CompressionLZ77(input, ref compressedTokens, dictionaryLength, bufferLength); // Perform compression using the initializing compression method

            string displayOutput = FormatCompressedOutput(compressedTokens); // Format the output using a previously created method

            // Decompression
            string compressedString = string.Join("", compressedTokens); // Join the tokens into a single string
            List<string> decompressedFragments = new List<string>(); // Create a list to store the results of the decompression method
            DecompressionLZ77(compressedString, ref decompressedFragments, dictionaryLength); // Perform decompression using the decompression method, passing the compressed string and the list for decompressed fragments
            string decompressedOutput = string.Concat(decompressedFragments); // Concatenate all decompressed fragments into a single string

            // Displaying results
            Console.WriteLine("Value of the string before compression:\n" + decompressedOutput);
            Console.WriteLine("\nTokens after compression:\n" + displayOutput);
        }

        #region LZ77 Compression
        public static void CompressionLZ77(string source, ref List<string> resultCode, int dictionaryLength, int bufferLength)
        {
            string dictionary = ""; // Dictionary storing the processed part of the string
            string remaining = source; // Input string that has not yet been compressed
                                       // Helper variables for building tokens.
            string matched = ""; // The longest possible match in the dictionary
            int startIndex = 0; // Starting index of the match in the dictionary
            int matchCount = 0; // Number of matches found
            List<int> matchOccurrences = new List<int>(); // List of occurrences of the given pattern

            while (remaining.Length > 0) // Loop processing the entire input string
            {
                // Process the next symbols, update the dictionary, and generate a token
                ProcessNextSymbol(ref remaining, ref dictionary, ref matched, ref startIndex,
                                  ref matchCount, ref matchOccurrences, ref resultCode, bufferLength);
                TrimDictionary(ref dictionary, dictionaryLength); // Trim the dictionary to the maximum length
            }

            // If there is an unprocessed fragment left at the end
            if (matched.Length > 0)
            {
                // Finalize the last token – the last character of the matched fragment
                string extraChar = matched.Substring(matched.Length - 1, 1);
                string matchPart = matched.Substring(0, matched.Length - 1);
                // Add the last token
                FinalizeToken(ref dictionary, matchPart, extraChar, startIndex, ref resultCode);
            }
        }
        // Method processing a single symbol
        private static void ProcessNextSymbol(ref string remaining, ref string dictionary, ref string matched,
                                              ref int startIndex, ref int matchCount, ref List<int> matchOccurrences,
                                              ref List<string> resultCode, int bufferLength)
        {
            // Get the first character from the input string.
            string temp = remaining;
            string character = "";
            character = temp.Substring(0, 1);
            // Remove that character
            remaining = temp.Remove(0, 1);
            // Check if the dictionary already contains any data
            if (dictionary.Length > 0)
                IB_71578_ProcessWithDictionary(ref dictionary, ref matched, ref startIndex, ref matchCount,
                                      ref matchOccurrences, character, bufferLength, ref resultCode);
            else
                ProcessWithoutDictionary(ref dictionary, character, ref resultCode, ref matched);
        }
        // Method handling the situation when the dictionary is still empty
        private static void ProcessWithoutDictionary(ref string dictionary, string character, ref List<string> resultCode, ref string matched)
        {
            dictionary = dictionary + matched + character; // Add the character to the dictionary and save as a new token
            resultCode.Add($"(0, 0, {character})"); // When no match is found
            matched = "";
        }

        // Method handling the situation when the dictionary already contains data
        private static void IB_71578_ProcessWithDictionary(ref string dictionary, ref string matched, ref int startIndex,
                                                  ref int matchCount, ref List<int> matchOccurrences,
                                                  string character, int bufferLength, ref List<string> resultCode)
        {
            // We need to check if the dictionary contains the character and if it fits in the buffer
            if (dictionary.Contains(character) && matched.Length <= bufferLength)
            {
                if (matched.Length == 0)
                {
                    startIndex = dictionary.IndexOf(character) + 1;
                    matched = matched + character;
                }
                else if (dictionary.Contains(matched + character))
                {
                    matchCount = Regex.Matches(dictionary, matched + character).Count;
                    matched = matched + character;
                    startIndex = dictionary.IndexOf(matched) + 1;
                    character = "";
                }
                else
                {
                    if (matchCount > 1)
                    {
                        ResolveMultipleOccurrences(dictionary, matched, ref startIndex, ref matchOccurrences);
                    }
                    FinalizeToken(ref dictionary, matched, character, startIndex, ref resultCode);
                    matched = "";
                }
            }
            else
            {
                FinalizeToken(ref dictionary, matched, character, startIndex, ref resultCode);
                matched = "";
                startIndex = 0;
            }
        }

        // Method called in case of multiple occurrences of a fragment in the dictionary
        private static void ResolveMultipleOccurrences(string dictionary, string matched, ref int startIndex, ref List<int> matchOccurrences)
        {
            // Set the initial offset to the length of the matched fragment
            int offset = matched.Length;
            int index = 0;
            // Search for all occurrences of the matched fragment
            do
            {
                // Find the next occurrence
                index = dictionary.IndexOf(matched, offset) + 1;
                // If it exists, add it to the list
                if (index > 0)
                    matchOccurrences.Add(index);
                // Move the offset
                offset = offset + index - 1;
            }
            while (offset <= dictionary.Length && index > 0);
            // If multiple occurrences are found, take the first one and then clear the list
            if (matchOccurrences.Count > 0)
            {
                startIndex = matchOccurrences[0];
                matchOccurrences.Clear();
            }
        }
        // After everything, add the token to the result list
        private static void FinalizeToken(ref string dictionary, string matched, string character, int startIndex, ref List<string> resultCode)
        {
            int matchLength = matched.Length; // Calculate the length of the match
            resultCode.Add($"({startIndex}, {matchLength}, {character})"); // Create the token and add it
            dictionary = dictionary + matched + character; // Update the dictionary by adding the fragment and new character
        }

        // Method limits the length of the dictionary to a specified limit by removing the oldest characters
        private static void TrimDictionary(ref string dictionary, int dictionaryLength)
        {
            // When the dictionary exceeds the allowed length, remove from the beginning
            if (dictionary.Length > dictionaryLength)
            {
                int trimSize = dictionary.Length - dictionaryLength;
                dictionary = dictionary.Remove(0, trimSize);
            }
        }

        private static string FormatCompressedOutput(List<string> tokens)
        {
            // Separate each token with a comma and space, and add ", EOF" at the end
            return string.Join(", ", tokens) + ", EOF";
        }
        #endregion

        #region -> LZ77 Decompression 
        // This method is constructed similarly to the compression method, but it reverses its operation.
        // We will decompose the source string using the LZ77 algorithm and build the decompressed string.
        // The operation will again use a dynamically rebuilt dictionary.
        public static void DecompressionLZ77(/* some data source should be provided */
        string source,
        /* the output needs a container
        * that will hold it */
        ref List<string> resultCode,
        /* need to provide the dictionary part length */
        int dictionaryLengthPart)
        {
            // First, declare local variables, thus ensuring the possibility
            // to work on a copy of the input string and not on its original.
            string character;
            string temp;
            string remaining;
            // Additionally, declare variables to hold:
            // the value of the next word read according to the code word pattern;
            string dictionary = "";
            // target string for the read characters;
            string readCharacters = "";
            // variable storing the pointer/index of the match;
            int matchIndex = 0;
            // number of matched characters;
            int mathchLength = 0;
            // current length of the dictionary, which cannot exceed initial parameters;
            // Of course, this can be retrieved at any time by calling Length ☺
            int dictionaryLength = 0;
            // values specifying how much to trim the new version of the dictionary;
            int dictionaryTrimSize = 0;
            // and an additional helper list of occurrences
            List<int> occurrenceList = new List<int>();
            // First, remove ordering characters from the input stream, basically garbage.
            remaining = source.Replace(" ", "").Replace("EOF|)", "");

            // Then write the LZ77 decompression code for code words constructed according to the pattern:
            // (0, 0, B) (0, 0, C) (0, 0, A) (3, 1, B) (1, 2, C) (1, 2, D) (9, 1, C) (3, 2, C) 
            do
            {
                // find the beginning of the code word
                do
                {
                    temp = remaining;
                    character = temp.Substring(0, 1);
                    remaining = temp.Remove(0, 1);
                }
                // loop repeats until the character "(" is found in the string
                while (character != "(");
                character = "";

                // Then, according to the order of the code word elements,
                // read the dictionary index value;
                do
                {
                    readCharacters = readCharacters + character;
                    temp = remaining;
                    character = temp.Substring(0, 1);
                    remaining = temp.Remove(0, 1);
                }
                while (character != ",");
                // Let's set the match index value from the code word;
                matchIndex = Convert.ToInt32(readCharacters);
                readCharacters = "";
                character = "";
                // Read the match length value from the code word;
                do
                {
                    readCharacters = readCharacters + character;
                    temp = remaining;
                    character = temp.Substring(0, 1);
                    remaining = temp.Remove(0, 1);
                }
                while (character != ",");
                // Convert the value and clear variables.
                mathchLength = Convert.ToInt32(readCharacters);
                readCharacters = "";
                character = "";
                // Then read the first character following the matched sequence
                temp = remaining;
                character = temp.Substring(0, 1);
                remaining = temp.Remove(0, 1);
                // If the length of the sequence matched in the dictionary is greater than 0
                if (mathchLength > 0)
                {
                    // If the read character is part of the ending sequence, clear the variable value
                    if (character == "|")
                        character = "";

                    // Add the decoded fragment to the result code
                    resultCode.Add(dictionary.Substring(matchIndex - 1, mathchLength) + character);

                    // And add the read matching index value to the dictionary
                    dictionary = dictionary +
                                 dictionary.Substring(matchIndex - 1, mathchLength) + character;
                }

                // If no match was found in the dictionary
                else
                {
                    // Add the read character to the result code
                    resultCode.Add(character);

                    // And update the dictionary content
                    if (dictionary.Length > 0)
                        dictionary = dictionary + character;
                    else
                        dictionary = character;
                }
                // Need to check if the dictionary length exceeded the assumed value
                // and if it is greater, trim the dictionary to the initially declared size
                if (dictionary.Length > dictionaryLength)
                {
                    dictionaryLength = dictionary.Length;
                    dictionaryTrimSize = dictionaryLength - dictionaryLength;
                    dictionary = dictionary.Remove(0, dictionaryTrimSize);
                }
                // Finally, clear variables for tidiness
                matchIndex = 0;
                mathchLength = 0;
                character = "";
            }
            // The exit condition is reading an end character that can be found in the string
            while (remaining != "|" && remaining != "" && remaining != ")");
            #endregion
        }
    }
}
