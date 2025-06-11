# CSharp-OOP-Exercises




## Huffman Coding - Exercise 1

### Main Method
The entry point of the program, orchestrates the entire process of Huffman compression and decompression in a loop, allowing users to compress multiple strings without restarting the application. Within a while (true) loop, the program first performs compression by invoking HuffmanCompress method, passing the current input string, a list to collect encoded bits, and a list to hold character frequency information. 

After compression, it constructs a dictionary by projecting each OccurringCharacter into HuffmanDictionaryEntry objects that pair each character with its binary code. The compressed bit sequence is joined into a single string, which is then passed to HuffmanDecompression along with the dictionary and a flag indicating decompression success.

All operations are wrapped in a try-catch block to gracefully handle unexpected errors. If the user presses Enter at the prompt, the loop breaks and the application terminates.


### Compression
The HuffmanCompress method is responsible for analyzing the frequency of each character in the input string, constructing the Huffman tree, and generating the binary codes for each character. First, it scans the input:
```C#
do {
    nextChar = remaining.Substring(0, 1);
    listIndex = tempOccurringCharacter.FindIndex(f => f.Character == nextChar);
    if (listIndex == -1) {
        tempOccurringCharacter.Add(new OccurringCharacter { Character = nextChar, Frequency = 1 });
    } else {
        tempOccurringCharacter.Where(w => w.Character == nextChar)
                               .ToList()
                               .ForEach(s => s.Frequency++);
    }
    remaining = remaining.Remove(0, 1);
} while (remaining.Length != 0);
```
This loop builds a list of OccurringCharacter objects, each tracking how often a character appears. After sorting by ascending frequency, the core Huffman tree is built in a do-while loop that repeatedly picks the two lowest-frequency nodes, merges them into a new parent node, and assigns 0 or 1 as the code bit:
```C#
for (int i = 0; i <= 1; i++) {
    var node = new HuffmanTreeNode {
        Character = charListSorted[i].Character,
        ParentNode = nodeNew + nodeNum,
        CodeBit = i
    };
    huffmanTreeNode.Add(node);
}
charListSorted.RemoveRange(0, 2);
charListSorted = charListSorted.OrderBy(o => o.Frequency).ToList();
```
Once the tree is complete and all intermediate nodes are created, the method traverses from each leaf back to the root to build the final binary code string for each character. The result is a list of binary strings corresponding to the original input characters.


### Decompression
The counterpart, takes the compressed bitstream and the dictionary of character–code mappings to reconstruct the original text. It reads the bitstream character by character, skipping any delimiters (such as commas) that may appear, and accumulates bits until a matching entry is found in the dictionary:
```C#
do {
    // Skip non-binary characters
    while (source.Length > 0 && source[0] != '0' && source[0] != '1') {
        source = source.Remove(0, 1);
    }
    // Build the next code fragment
    while (source.Length > 0 && (source[0] == '0' || source[0] == '1')) {
        nextChar += source[0];
        source = source.Remove(0, 1);
    }
    int index = sourceDictionary.FindIndex(f => f.Code == nextChar);
    resultCode.Add(sourceDictionary[index].Character);
    nextChar = "";
} while (source.Length > 0);
dictionaryComplete = true;
```
If at any point an exception occurs—such as a missing code in the dictionary—the method catches it and sets flag to false, signaling that decompression failed.


### Printing Results
After both compression and decompression are complete, the Main method prints comprehensive feedback to the console. If decompression succeeds, program lists each character alongside its occurrence count and binary code:
```C#
Console.WriteLine("\nCharacters (ascending frequency), counts, and codes:");
foreach (var item in characterList)
{
    Console.WriteLine($"{item.Character}, {item.Frequency}, {item.BinaryCode}");
}
```
Finally, the program outputs the encoded bitstream in order, marking the end with EOF. This structured output helps users verify both the integrity and efficiency of the Huffman encoding process.






















