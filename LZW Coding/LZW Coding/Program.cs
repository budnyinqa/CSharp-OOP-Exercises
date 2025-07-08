using System;
using System.Collections.Generic;

namespace LZW_Coding
{
    // Main program class
    class Program
    {
        static void Main(string[] args)
        {
            try { new Menu().Start(); } // Launch the application  
            catch (Exception ex) { Console.WriteLine("An error occurred: " + ex.ToString()); } // If an error occurs, notify with details  
        }
    }
    class Menu
    {
        public void Start()
        {
            string input = GetValidInput(); // Get a valid string from the user  
            new Runner().Run(input); // Pass the string  
        }
        private string GetValidInput()
        {
            string input = new UserInput().Read(); // Read the string  
            while (IsInvalid(input)) // Check if it is empty  
                input = RetryInput(); // Retry if the user provided an invalid string  
            return input; // Return the valid string  
        }
        private bool IsInvalid(string input) => string.IsNullOrWhiteSpace(input);
        private string RetryInput()
        {
            Console.WriteLine("Invalid string entered"); // Inform about the error  
            new UserOutput().PromptRetry(); // Ask to enter the string again  
            return new UserInput().Read(); // Read again  
        }
    }
    class Runner
    {
        public void Run(string input)
        {
            CompressionResult result = new CompressorLZW().Compress(input); // Compress using a separate class
            new UserOutput().DisplayCompression(result.Dictionary, result.Codes); // Display the dictionary and encoded characters
            new UserOutput().DisplayDecompression(new Decompressor().Decompress(result)); // Decompress and display the result
        }
    }
    class UserInput
    {
        public string Read()
        {
            Console.Write("Enter string to compress: ");  // Ask the user to input the string
            return Console.ReadLine();
        }
    }
    class UserOutput
    {
        public void ErrorMsg(string msg) => Console.WriteLine("Error: " + msg); // Display an error message if an error occurs
        public void PromptRetry() => Console.WriteLine("Please try again."); // Give another chance to enter the string
        public void DisplayCompression(Dictionary<int, string> dict, List<int> codes)
        {
            ShowDictionary(dict); // Display the dictionary
            ShowCodes(codes); // Display the encoded characters
        }
        private void ShowDictionary(Dictionary<int, string> dict)
        {
            Console.WriteLine("\nDictionary:");
            dict.ForEach(character => Console.WriteLine($"{character.Key}: {character.Value}")); // Display by iterating
        }
        private void ShowCodes(List<int> codes) => Console.WriteLine("\nEncoded string:\n" + string.Join(" ", codes));
        public void DisplayDecompression(string decompressionResult) => Console.WriteLine("\nDecompression:\n" + decompressionResult); // Display the decoded characters
    }
    static class DictionaryExtensions
    {
        public static void ForEach<TKey, TValue>(this Dictionary<TKey, TValue> dict, Action<KeyValuePair<TKey, TValue>> action)
        {
            foreach (var pair in dict)
                action(pair); // For each key-value pair execute the action
        }
    }
    class CompressorLZW
    {
        private DictionaryBuilder dictBuilder; // Build base and reverse dictionary responsible for processing input string into codes
        private Encoder encoder;
        public CompressorLZW() { dictBuilder = new DictionaryBuilder(); encoder = new Encoder(); } // Initialize modules
        public CompressionResult Compress(string input)
        {
            dictBuilder.Build(input); // Build base dictionary for all unique characters
            encoder.Encode(input, dictBuilder); // Process input string creating patterns and generating codes
            return new CompressionResult(dictBuilder.GetReverse(), encoder.GetCodes()); // Return result in object
        }
    }
    class DictionaryBuilder
    {
        private Dictionary<string, int> directDict = new Dictionary<string, int>(); // Map character strings to codes
        private Dictionary<int, string> reverseDict = new Dictionary<int, string>(); // And vice versa
        public void Build(string s)
        {
            int nextCode = 1;
            foreach (char character in s) // Iterate over each character
                if (!directDict.ContainsKey(character.ToString())) // If string not present in dictionary, add character to both dictionaries
                { directDict[character.ToString()] = nextCode; reverseDict[nextCode++] = character.ToString(); }
        }
        public Dictionary<int, string> GetReverse() => reverseDict; // Return reverse dictionary
        public bool Contains(string key) => directDict.ContainsKey(key); // Check if string exists in dictionary
        public int GetCode(string key) => directDict[key]; // Get code assigned to string
        public void Add(string key, int code) { directDict[key] = code; reverseDict[code] = key; } // Add new entry to dictionary
        public int Count() => directDict.Count; // Return number of elements in dictionary
    }
    class Encoder
    {
        private List<int> codes = new List<int>(); // List of resulting compression codes
        public void Encode(string s, DictionaryBuilder db)
        {
            string currentPattern = ""; // Current pattern
            int nextCode = db.Count() + 1; // New code is count of elements in dictionary plus one
            foreach (char character in s) ProcessCharacter(character, ref currentPattern, ref nextCode, db); // Process each character in string
            if (currentPattern != "") codes.Add(db.GetCode(currentPattern)); // Add code of last pattern if not empty
        }
        private void ProcessCharacter(char character, ref string currentPattern, ref int nextCode, DictionaryBuilder db)
        {
            string newPattern = currentPattern + character; // Combine current pattern with current character
            if (db.Contains(newPattern)) currentPattern = newPattern; // If new pattern exists, extend it
            else { codes.Add(db.GetCode(currentPattern)); db.Add(newPattern, nextCode++); currentPattern = character.ToString(); } // Add code, add new pattern and reset current
        }
        public List<int> GetCodes() => codes; // Expose generated code list
    }
    class Decompressor
    {
        public string Decompress(CompressionResult res)
        {
            // Retrieve dictionary and list of code words. Then read first symbol and set pointer to new code.
            var d = res.Dictionary; var k = res.Codes; string w = d[k[0]]; int i = d.Count + 1;
            return ProcessCodes(d, k, w, i); // Continue processing
        }
        private string ProcessCodes(Dictionary<int, string> d, List<int> k, string w, int i)
        {
            string result = w; // Initialize result
            for (int j = 1; j < k.Count; j++) result += Expand(d, ref i, ref w, k[j]); // Rebuild subsequent patterns
            return result;
        }
        private string Expand(Dictionary<int, string> dict, ref int nextCode, ref string current, int code)
        {
            string entry = dict.ContainsKey(code) ? dict[code] : current + current[0]; // If code doesn't exist, create pattern from current plus first character
            dict[nextCode++] = current + entry[0]; // Add new pattern to dictionary
            current = entry; // Update current pattern
            return entry;    // Return expanded string
        }
    }
    class CompressionResult // Class to store compression results
    {
        public Dictionary<int, string> Dictionary { get; private set; } // Dictionary
        public List<int> Codes { get; private set; } // List of codes
        public CompressionResult(Dictionary<int, string> dict, List<int> codes)
        { Dictionary = dict; Codes = codes; } // Compression results
    }
}
