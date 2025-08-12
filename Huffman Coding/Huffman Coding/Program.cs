using System;
using System.Collections.Generic;
using System.Linq;

namespace Huffman_Coding
{
    // Class for initial list entries of Huffman code
    public class OccurringCharacter
    {
        public int Frequency { get; set; }
        public string Character { get; set; }
        public string BinaryCode { get; set; }
    }

    // Class for nodes in the Huffman tree
    public class HuffmanTreeNode
    {
        public int CodeBit { get; set; }
        public string Character { get; set; }
        public string ParentNode { get; set; }
        public int Frequency { get; set; }
    }
    // Class for source dictionary entries for Huffman decompression
    public class HuffmanDictionaryEntry
    {
        public string Character { get; set; }
        public string Code { get; set; }
    }

    // Main Class of the Program
    class Program
    {
        static void Main(string[] args)
        {
            string input = "XADJSOSDAOUAZADXSXODJAOUAOADAOXAAJSAXADOAOADO"; // We declare a variable from the beginning of the exercises; this will be the first to be compressed

            while (true) // Performing the encoding in a loop allows multiple compressions without restarting the application
            {
                // Compression //
                // We perform the entire compression using a "try-catch" block to add error handling for potential issues
                try
                {
                    List<string> encodedBits = new List<string>(); // List to store the encoded binary representation of the input string
                    List<OccurringCharacter> characterList = new List<OccurringCharacter>(); // List of unique characters and their frequencies in the string

                    // We call the HuffmanCompress method. It analyzes the list of characters and assigns binary codes to them
                    HuffmanCompress(input, ref encodedBits, ref characterList);

                    // Decompression //
                    List<HuffmanDictionaryEntry> sourceDictionary = characterList
                        .Select(z => new HuffmanDictionaryEntry
                        {
                            Character = z.Character,
                            Code = z.BinaryCode
                        }).ToList(); // We create a new source dictionary for decompression using the character and its binary equivalent

                    // We join the list of encoded values into a single binary string
                    string kompressionResultStr = string.Join("", encodedBits);

                    List<string> decompressedResult = new List<string>(); // We create a list to store the results
                    bool decompressionSuccess = false; // We create a variable to track the success of the operation; initially it's "false" because decompression has not started yet

                    HuffmanDecompression(sourceDictionary, kompressionResultStr,
                                                 ref decompressedResult, ref decompressionSuccess); // We call the decompression method, passing in the prepared data, dictionary, and control variable

                    // Printing results //
                    if (decompressionSuccess) // We display the decompression result if it was successful
                    {
                        Console.WriteLine("\nSequence before compression:");
                        Console.WriteLine(string.Join("", decompressedResult));
                    }
                    else
                    {
                        Console.WriteLine("\nDecompression failed\n");
                    }

                    // We display information about the data being shown. Then, using a foreach loop, we print data from the characterList: character, frequency, binary code. Each piece of information is separated by a comma and a space.
                    Console.WriteLine("\nCharacters (ascending frequency), counts, and codes:");
                    foreach (var item in characterList)
                    {
                        Console.WriteLine($"{item.Character}, {item.Frequency}, {item.BinaryCode}");
                    }

                    // We display the encoded string, again using a foreach loop, this time from the encodedBits list. Here we don't need to add commas, only spaces.
                    Console.WriteLine("\nEncoded output:");
                    foreach (var res in encodedBits)
                    {
                        Console.Write(res + " ");
                    }
                    Console.WriteLine("EOF");  // Indicate end of compression
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"\nError: {ex.Message}"); // We inform about the error and its type
                }

                Console.WriteLine("\n\n\nEnter string to compress (or press Enter to exit):"); // After showing the compression of the example string, we ask the user to enter a string they want to compress
                string nextImput = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(nextImput))
                {
                    //Console.WriteLine("Program ended."); // We inform about program termination and exit the loop
                    break;
                }

                input = nextImput; // If the user entered something, we replace the input with the new value so the loop compre
            }
        }

        // Both main methods, compression and decompression, will be written
        // using a procedural technique, which will still utilize objects
        // for storing and manipulating data.
        #region -> Huffman Compression
        public static void HuffmanCompress(
                                            // some source is needed
                                            string source,
                                            // to output results, we need a container to hold them
                                            ref List<string> resultCode,
                                            // and of course, it's good to have information
                                            // about the number of occurrences of each character
                                            ref List<OccurringCharacter> occurrences)
        {
            // Since working with strings usually causes issues with copying
            // content, we create a few variables just in case
            // to store intermediate values
            string remaining = source;
            string temp = remaining;
            string nextChar = "";

            // We also need a bipolar variable to store
            // the answer to the question of whether the list contains the examined character.
            // This can be a bool variable or a simple int.
            // Since we'll be working with a list of objects which
            // returns -1 when the search pattern is not matched,
            // we’ll use an int variable
            int listIndex = 0;

            // Then we initialize all necessary lists
            List<HuffmanTreeNode> huffmanTreeNode = new List<HuffmanTreeNode>();
            List<OccurringCharacter> tempOccurringCharacter = new List<OccurringCharacter>();
            List<HuffmanTreeNode> tempHuffmanTreeNode = new List<HuffmanTreeNode>();

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

            // Now we need to sort the resulting list and temporarily copy it to another, empty one.
            // It can be the return list since it has already been declared,
            // and we can use it to create a new list of objects
            occurrences = tempOccurringCharacter.OrderBy(o => o.Frequency).ToList();
            List<OccurringCharacter> charListSorted = new List<OccurringCharacter>(occurrences);
            // * * * CONSTRUCTING THE HUFFMAN TREE * * *

            // We will definitely need a variable
            // that will store the current tree level value
            int nodeNum = 0;

            // Additionally, we need a variable to store
            // the sum of occurrences of characters in the leaves for the new root
            int nodeNewVal = 0;

            // And a prefix stored in a list,
            // indicating which element is the root and which is a leaf
            string nodeNew = "node";

            // Since the tree-building operations are performed sequentially
            // for each element of the sorted list of characters, and the same actions
            // must be performed for all elements of the sorted list of character occurrences,
            // the whole operation can again be performed with a do-while loop

            do
            {
                // As long as the sorted list has more than one element
                if (charListSorted.Count > 1)
                {
                    // If the built structure already has some root,
                    // then according to the Huffman algorithm logic,
                    // we should take the pair of objects on top and determine
                    // which of them will be the new root
                    if (huffmanTreeNode.Count > 0)
                    {
                        if (huffmanTreeNode[0].Frequency + charListSorted[0].Frequency >
                            charListSorted[0].Frequency + charListSorted[1].Frequency)
                            nodeNewVal = huffmanTreeNode[0].Frequency + charListSorted[0].Frequency;
                        else
                            nodeNewVal = charListSorted[0].Frequency + charListSorted[1].Frequency;
                    }
                    // However, if the tree is still empty, meaning it has no root yet,
                    // it means that the first root is the sum of frequencies of the first two
                    // elements of the sorted character list
                    else if (huffmanTreeNode.Count == 0)
                        nodeNewVal = charListSorted[0].Frequency + charListSorted[1].Frequency;

                    // Now we need to determine the tree level for the new element in the list,
                    // which is the newly created root
                    if (charListSorted.Count > 2)
                    {
                        if (nodeNewVal >= charListSorted[2].Frequency && charListSorted.Count >= 3)
                            nodeNum++;
                    }
                    else
                        nodeNum++;

                    // The new root simultaneously becomes a new element
                    // of the sorted list of objects, so we create a new object
                    // which takes the name in the form "node" + the determined root number,
                    // and add it to the end of the sorted character list
                    OccurringCharacter newChar = new OccurringCharacter
                    {
                        // adding the new root to the unprocessed list
                        Frequency = nodeNewVal,
                        Character = nodeNew + nodeNum
                    };
                    charListSorted.Add(newChar);

                    // We also need to assign binary values to the next tree elements in its structure,
                    // according to the rule that the object with the lower frequency
                    // is placed on the left side, as the left leaf and receives binary value "0",
                    // and the object on the right side is placed on the right,
                    // and receives binary value "1". Since this is just two loop iterations,
                    // we can set the iterator to range from 0 to 1 and use it
                    // as the binary flag of the given leaf
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
                }

                // As soon as it turns out that only one element remains in the sorted list,
                // it means this is simply the root with the greatest value,
                // so we add it to the tree at the very end


                else
                {
                    HuffmanTreeNode huffmanTreeItem = new HuffmanTreeNode
                    {
                        CodeBit = 2,
                        Character = nodeNew + (nodeNum + 1).ToString(),
                        ParentNode = "TOP"
                    };
                    huffmanTreeNode.Add(huffmanTreeItem);
                    charListSorted.Clear();
                }
            }

            // Repeat the whole operation as long as there is any object
            // left in the sorted character list
            while (charListSorted.Count != 0);

            // At this point, we have all the Huffman tree elements
            // so it makes sense to sort them again
            tempHuffmanTreeNode = huffmanTreeNode.OrderBy(o => o.Frequency).ToList();
            huffmanTreeNode = tempHuffmanTreeNode;

            // Prepare a list for printing. For this, we will also need
            // two variables to store partial information about the final element
            string tempBinaryCode = "";
            string actualNode = "";

            // First, temporarily move all NODES to the left side
            for (int i = 0; i < huffmanTreeNode.Count - 1; i++)
            {
                if (huffmanTreeNode[i].Frequency == huffmanTreeNode[i + 1].Frequency &&
                    huffmanTreeNode[i + 1].Character.Length > 1) // nodes have longer character names
                {
                    HuffmanTreeNode tempNode = huffmanTreeNode[i];
                    huffmanTreeNode[i] = huffmanTreeNode[i + 1];
                    huffmanTreeNode[i + 1] = tempNode;
                    huffmanTreeNode[i].CodeBit = 0;
                    huffmanTreeNode[i + 1].CodeBit = 1;
                }
            }

            // Now, create a list of characters with assigned binary values.
            // So far, elements in the list had only tags indicating
            // whether the element is on the left or right side, and since
            // the binary value of each leaf counts only after building the whole tree,
            // now we need to build the tree and correctly determine the binary values
            // for all elements of the Huffman tree.
            for (int i = 0; i < occurrences.Count; i++)
            {
                // Determine the index where the object containing
                // the searched character is located in the Huffman tree
                nextChar = occurrences[i].Character;
                listIndex = huffmanTreeNode.FindIndex(f => f.Character == nextChar);

                // Find which root node is the parent of this object
                actualNode = huffmanTreeNode[listIndex].ParentNode;

                // Assign the first binary value to a temporary variable
                // which will be used to build the complete binary code for the object
                tempBinaryCode = huffmanTreeNode[listIndex].CodeBit.ToString();

                // All operations of building the complete binary code this way
                // need to be performed again in a loop with a stopping condition,
                // so use a do-while loop
                do
                {
                    listIndex = huffmanTreeNode.FindIndex(f => f.Character == actualNode);
                    // This operation will only be done
                    // if the tree contains the searched element
                    if (listIndex != -1)
                    {
                        actualNode = huffmanTreeNode[listIndex].ParentNode;
                        tempBinaryCode = tempBinaryCode + huffmanTreeNode[listIndex].CodeBit.ToString();

                        // For neatness, remove leading zeros if any
                        if (tempBinaryCode.Length > 1 && tempBinaryCode.Substring(0, 1) == "0")
                            tempBinaryCode = tempBinaryCode.Remove(0, 1);
                    }
                }
                // The exit condition for the loop is an empty list (no parent node)
                while (listIndex != -1);

                // After determining the complete binary value for the examined object,
                // assign it to the corresponding field of the examined character in the list
                occurrences[i].BinaryCode = tempBinaryCode;
            }


            // Finally, we create an output string containing successive code words
            // ready to be printed anywhere (for example, in the console)
            for (int i = 0; i < source.Length; i++)
            {
                nextChar = source.Substring(i, 1);

                for (int j = 0; j <= occurrences.Count; j++)
                {
                    if (occurrences[j].Character == nextChar)
                    {
                        resultCode.Add(occurrences[j].BinaryCode + ",");
                        j = occurrences.Count;
                    }
                }
            }
        }
        #endregion

        #region -> Huffman Decompression
        public static void HuffmanDecompression(
        // According to assumptions, for decompressing a string
        // in the Huffman method, a predefined dictionary attached to the file is needed;
        List<HuffmanDictionaryEntry> sourceDictionary,
        // compressed string;
        string source,
        // container which will be used to return
        // the decoded content;
        ref List<string> resultCode,
        // and some variable indicating whether it was
        // possible to decompress the string without errors
        ref bool dictionaryComplete)
        {
            // As before, in the compression method, to examine
            // subsequent elements of the string
            // we need some temporary variable. We also need a variable
            // to catch the result of list searching with a Lambda function
            string nextChar = "";
            int consists = 0;
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
        }
        #endregion

        //indexList = tempCharacterList.FindIndex(f => f.Character == nextChar);
        private int GiveIndex(List<OccurringCharacter> list, string character)
        {
            int index = 0;
            try
            {
                index = list.FindIndex(f => f.Character == character);
            }
            catch (Exception ex)
            {
                throw;
            }
            return index;
        }
        //tempCharacterList.Where(w => w.Character == nextChar).ToList().ForEach(s => s.Frequency = s.Frequency + 1);
        private bool Increment(List<OccurringCharacter> list, string character)
        {
            try
            {
                list.Where(w => w.Character == character).ToList().ForEach(s => s.Frequency = s.Frequency + 1);
            }
            catch (ArgumentNullException)
            {
                Console.WriteLine("\nAttempted to reference a character not present in the list.\n");
            }
            return true;
        }
    }
}
