# CSharp-OOP-Exercises




## Huffman Coding - Exercise 1
The purpose of the project is to correctly encode the data provided as a declared character string. Upon running the program, the following should be displayed on the screen:

- the value of the string before compression,
- a list in ascending order of the characters contained in the input string, along with their frequencies and the assigned codewords read from the Huffman tree, in the format: `"A, 17, 01101001";`
- the resulting encoded string, in which individual codewords are separated by the sequence `[ , ]` (comma followed by space).

The end of the compression code sequence should be indicated by displaying the word `[ , EOF]` (comma followed by `EOF`). The program should provide the user with the ability to encode a user-defined character string.


### Main Method
The entry point of the program, orchestrates the entire process of Huffman compression and decompression in a loop, allowing users to compress multiple strings without restarting the application. Within a while (true) loop, the program first performs compression by invoking `HuffmanCompress` method, passing the current input string, a list to collect encoded bits, and a list to hold character frequency information. 

After compression, it constructs a dictionary by projecting each `OccurringCharacter` into `HuffmanDictionaryEntry` objects that pair each character with its binary code. The compressed bit sequence is joined into a single string, which is then passed to HuffmanDecompression along with the dictionary and a flag indicating decompression success.

All operations are wrapped in a try-catch block to gracefully handle unexpected errors. If the user presses Enter at the prompt, the loop breaks and the application terminates.


### Compression
The `HuffmanCompress` method is responsible for analyzing the frequency of each character in the input string, constructing the Huffman tree, and generating the binary codes for each character. First, it scans the input:
```C#
// Filling the list of character occurrences with OccurringCharacter objects
// and counting their frequencies in the input string
do
{
    temp = remaining;
    nextChar = temp.Substring(0, 1);

    // Now we use the well-known Lambda function construct
    // to check whether the list contains the character taken from the input string
    listIndex = tempOccurringCharacter.FindIndex(IB_71578_f => IB_71578_f.Character == nextChar);

    // If the list does not contain it (i.e., the function returns -1),
    // then we add it to the list
    if (listIndex == -1)
    {
        OccurringCharacter newChar = new OccurringCharacter();
        newChar.Frequency = 1;
        newChar.Character = nextChar;
        tempOccurringCharacter.Add(newChar);
    }
    // If it is already in the list, then again using a Lambda function,
    // we find the appropriate index and increment
    // the frequency count for that character by 1
    else
    {
        tempOccurringCharacter.Where(w => w.Character == nextChar).ToList()
                              .ForEach(s => s.Frequency = s.Frequency + 1);
    }

    // Finally, from the input string, we remove the character that was just added to the list
    remaining = temp.Remove(0, 1);
}
// The above sequence repeats as long as the input string length is greater than 0
while (remaining.Length != 0);
```
This loop builds a list of `OccurringCharacter` objects, each tracking how often a character appears. After sorting by ascending frequency, the core Huffman tree is built in a do-while loop that repeatedly picks the two lowest-frequency nodes, merges them into a new parent node, and assigns 0 or 1 as the code bit:
```C#
for (int i = 0; i <= 1; i++)
{
    HuffmanTreeNode huffmanTreeItem = new HuffmanTreeNode();
    if (charListSorted.Count > 1)
        huffmanTreeItem.CodeBit = i;
    else
        huffmanTreeItem.CodeBit = 2;
    huffmanTreeItem.Character = charListSorted[i].Character;
    huffmanTreeItem.ParentNode = nodeNew + nodeNum.ToString();
    huffmanTreeItem.Frequency = charListSorted[i].Frequency;
    huffmanTreeNode.Add(huffmanTreeItem);
}

// The only thing left is to remove the first two objects
// from the sorted character list because they are now part of the tree
charListSorted.RemoveRange(0, 2);

// And then re-sort the remaining objects, because at the end
// we added a new root object with a frequency
// that may not be the greatest value
tempOccurringCharacter = charListSorted.OrderBy(IB_71578_o => IB_71578_o.Frequency).ToList();

// We also sort the temporary Huffman tree node list in each loop iteration
tempHuffmanTreeNode = huffmanTreeNode.OrderByDescending(IB_71578_o => IB_71578_o.Frequency).ToList();

// And assign the result back to the Huffman tree node list to keep
huffmanTreeNode = tempHuffmanTreeNode;

// Also assign the list of objects containing characters
charListSorted = tempOccurringCharacter;
```
Once the tree is complete and all intermediate nodes are created, the method traverses from each leaf back to the root to build the final binary code string for each character. The result is a list of binary strings corresponding to the original input characters.


### Decompression
The counterpart, takes the compressed bitstream and the dictionary of character–code mappings to reconstruct the original text. It reads the bitstream character by character, skipping any delimiters (such as commas) that may appear, and accumulates bits until a matching entry is found in the dictionary:
```C#
// Just in case, we set up a trap to catch errors
// which we won’t handle in this code,
// but using it in production code could be a good idea
try
{
    // Decompression, like compression, is performed in a loop
    // that defines the exit condition, i.e. a do-while loop

    do // "do-while" loop processing the entire input string
    {
        do // Loop skipping non-binary characters
        {
            if (source.Length > 0 &&
                source.Substring(0, 1) != "0" &&
                source.Substring(0, 1) != "1")
            {
                source = source.Remove(0, 1); // If the character is not 0 or 1, remove it
            }
        }
        while (source.Length > 0 &&
               source.Substring(0, 1) != "1" &&
               source.Substring(0, 1) != "0"); // Repeat until 0 or 1 is found

        if (source.Length > 0) // Then, if there are still characters in the compressed string
        {
            do // Build binary code, bit by bit
            {
                nextChar = nextChar + source.Substring(0, 1);
                source = source.Remove(0, 1); // Add the bit to variable and remove from input
            }
            while (source.Substring(0, 1) == "0" ||
                   source.Substring(0, 1) == "1"); // Repeat until all bits are processed

            consists = sourceDictionary.FindIndex(f => f.Code == nextChar); // Find in dictionary
            resultCode.Add(sourceDictionary[consists].Character); // If found, add the character
            nextChar = ""; // Reset variable to process next characters
        }
        else
        {
            dictionaryComplete = true; // If no characters to process, finish and mark as complete
            return;
        }
    }
    while (source.Length > 0); // Repeat until no more characters
}
catch (Exception e)
{
    dictionaryComplete = false; // Abort and mark as incomplete if error occurs
}
```
If at any point an exception occurs—such as a missing code in the dictionary - the method catches it and sets flag to false, signaling that decompression failed.


### Printing Results
After both compression and decompression are complete, the `Main` method prints comprehensive feedback to the console. If decompression succeeds, program lists each character alongside its occurrence count and binary code:
```C#
Console.WriteLine("\nCharacters (ascending frequency), counts, and codes:");
foreach (var item in characterList)
{
    Console.WriteLine($"{item.Character}, {item.Frequency}, {item.BinaryCode}");
}
```
Finally, the program outputs the encoded bitstream in order, marking the end with EOF. This structured output helps users verify both the integrity and efficiency of the Huffman encoding process.


## LZ77 Compression - Exercise 2
The purpose of the project is to correctly encode the data provided as a character string designated as the source data stream. Upon running the program, the following should be displayed on the screen:

- the original (decompressed) value of the string,
- the list of compression tokens in the format: (offset, length, nextCharacter),
- the resulting encoded string, in which tokens are separated by the sequence `[ , ]` (comma followed by space),
- the end of the compression code sequence should be indicated by displaying the word `[ , EOF]` (comma followed by EOF).


### Main Method
The program starts by defining a string to compress. The compression is performed by filling a list of compressed tokens. These tokens are formatted using `FormatCompressedOutput` for readability. Then, the tokens are joined and passed into the decompression method, which reconstructs the original string. Finally, both the decompressed string and the list of compression tokens is printed.

```C#
// Compression
List<string> compressedTokens = new List<string>(); // Create a list to store the results of the compression method
int dictionaryLength = 7; // Set the dictionary length
int bufferLength = 8; // Set the buffer length
CompressionLZ77(input, ref compressedTokens, dictionaryLength, bufferLength); // Perform compression using the initializing compression method
string displayOutput = FormatCompressedOutput(compressedTokens); // Format the output using a previously created method
```
After compression, the program prepares the compressed data for decompression:
```C#
// Decompression
string compressedString = string.Join("", compressedTokens); // Join the tokens into a single string
List<string> decompressedFragments = new List<string>(); // Create a list to store the results of the decompression method
DecompressionLZ77(compressedString, ref decompressedFragments, dictionaryLength); // Perform decompression using the decompression method, passing the compressed string and the list for decompressed fragments
string decompressedOutput = string.Concat(decompressedFragments); // Concatenate all decompressed fragments into a single string
```


### Compression
Compression is handled in the method `CompressionLZ77`, where a dictionary stores already processed text, and remaining holds the rest of the input. The program looks for the longest match of the upcoming characters in the dictionary window.

The main loop processes characters until the entire input is consumed. For each symbol, ProcessNextSymbol determines whether to handle it using an empty or non-empty dictionary.
```C#
while (remaining.Length > 0) // Loop processing the entire input string
{
    // Process the next symbols, update the dictionary, and generate a token
    ProcessNextSymbol(ref remaining, ref dictionary, ref matched, ref startIndex,
                      ref matchCount, ref matchOccurrences, ref resultCode, bufferLength);
    TrimDictionary(ref dictionary, dictionaryLength); // Trim the dictionary to the maximum length
}
```
If there’s a match remaining after the loop ends, it’s finalized with:
```C#
if (matched.Length > 0)
{
    // Finalize the last token – the last character of the matched fragment
    string extraChar = matched.Substring(matched.Length - 1, 1);
    string matchPart = matched.Substring(0, matched.Length - 1);
    // Add the last token
    FinalizeToken(ref dictionary, matchPart, extraChar, startIndex, ref resultCode);
}
```
The actual logic for handling a character when the dictionary is filled includes checking whether the extended string still exists in the dictionary and choosing the best match. If a repeated sequence is detected, the method selects the first match occurrence.
```C#
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
```


### Decompression
Decompression is done by reading back the tokens and rebuilding the string using a dynamic dictionary. The method removes extra characters like `EOF` and whitespace before starting.

The method reads token elements one by one. It first locates the opening bracket ( and then extracts the offset and match length:
```C#
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
```
If the length is non-zero, it appends the matched sequence from the dictionary and the next character:
```C#
// Add the decoded fragment to the result code
resultCode.Add(dictionary.Substring(matchIndex - 1, mathchLength) + character);

// And add the read matching index value to the dictionary
dictionary = dictionary +
             dictionary.Substring(matchIndex - 1, mathchLength) + character;
```
If there's no match (length is zero), it simply adds the new character:
```C#
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
```
Finally, the dictionary is trimmed if it exceeds the specified maximum length:
```C#
// Need to check if the dictionary length exceeded the assumed value
// and if it is greater, trim the dictionary to the initially declared size
if (dictionary.Length > dictionaryLength)
{
    dictionaryLength = dictionary.Length;
    dictionaryTrimSize = dictionaryLength - dictionaryLength;
    dictionary = dictionary.Remove(0, dictionaryTrimSize);
}
```



## LZW Coding - 




