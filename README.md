# CSharp-OOP-Exercises




## Huffman Coding - Exercise 1
The purpose of the project is to correctly encode the data provided as a declared character string. Upon running the program, the following should be displayed on the screen:

- the value of the string before compression,
- a list in ascending order of the characters contained in the input string, along with their frequencies and the assigned codewords read from the Huffman tree, in the format: `A, 17, 01101001`
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


### Output
![Image](https://github.com/user-attachments/assets/0d812142-90d9-4ce4-99bd-b02656e6bf9f)



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


### Output
![Image](https://github.com/user-attachments/assets/b28d9f95-d0c8-4d8f-9ccb-0db487e4ad3c)



## LZW Coding - Exercise 3
The goal is to rewrite both the compression and decompression algorithms from scratch using object-oriented programming principles. The code should be structured into multiple well-defined classes, each following the Single Responsibility Principle (SRP). Every class must encapsulate a logically minimal and focused purpose, ensuring its existence is justified and its role is clear.

The solution must include a console-based user interface module, allowing users to manually input any text for compression. Specifically, the program must be able to handle the following sample input: 

`DBCDDCBBBCDBADBADACBCABACBACDCBCACDACADDBAAADBDCBDDDABACBCCAAACBCDBCBDADDBBBBCCBDDDBBAADDCDCCDADBDCDCCACADCDCAADDCDBAAABBACCDBDABBDCDBCCBCADDDDACCCCCBCBADDCDDCDBBCDCCBDCDBDABDBBDAABBAACACABDACAAADACAABDBCAABADCCADDBCACACBAACA`

The user interface must also be robust against intentional misuse or error generation, such as unexpected inputs or attempts to crash the program. All exceptions must be gracefully handled. Implementation of the LZW algorithm should be fully original and designed to enforce the following constraints:
- each method must be no longer than 4 lines of code, promoting clarity and maintainability,
- the program must consist of at least five core classes that collaborate by passing strongly encapsulated objects,
- every class must expose only the strictly necessary interface (full data encapsulation) to ensure maximum modularity and minimum coupling,
- the solution should demonstrate a clear understanding of object-oriented design patterns such as encapsulation, composition, responsibility separation, and controlled communication between components.


### Main Method
This LZW compressor is structured around distinct classes that reflect core object‑oriented principles. The `Program` class exclusively handles application startup and global error management:
```C#
static void Main(string[] args)
{
    try { new Menu().Start(); } // Launch the application  
    catch (Exception ex) { Console.WriteLine("An error occurred: " + ex.ToString()); } // If an error occurs, notify with details  
}
```
By delegating the actual work to `Menu`, the entry point remains minimal and focused on bootstrapping. This separation makes it straightforward to modify error‑handling behavior (for example, logging to a file) without touching business logic.



### User Interaction and Validation
Interaction with the console is funneled through two collaborators, ensuring that nothing outside these classes performs. In `Menu`, input is validated in a loop:
```C#
private string GetValidInput()
{
    string input = new UserInput().Read(); // Read the string  
    while (IsInvalid(input)) // Check if it is empty  
        input = RetryInput(); // Retry if the user provided an invalid string  
    return input; // Return the valid string  
}
```
Here `Menu` relies on `UserInput.Read()` to acquire raw text and on `UserOutput.PromptRetry()` to inform the user, but it contains the control‑flow logic for validation. Maintaining I/O classes as simple wrappers allows you to introduce alternative interfaces by replacing these two classes alone.


### Compression
The most important class in the program is `CompressorLZW`, which uses composition to assemble a `DictionaryBuilde`r and an `Encoder`. The builder first registers every unique character, assigning incremental codes:
```C#
public void Build(string s)
{
    int nextCode = 1;
    foreach (char character in s) // Iterate over each character
        if (!directDict.ContainsKey(character.ToString())) // If string not present in dictionary, add character to both dictionaries
        { directDict[character.ToString()] = nextCode; reverseDict[nextCode++] = character.ToString(); }
}
```
The Encoder then implements the LZW loop, keeps track of the longest known pattern:
```C#
public void Encode(string s, DictionaryBuilder db)
{
    string currentPattern = ""; // Current pattern
    int nextCode = db.Count() + 1; // New code is count of elements in dictionary plus one
    foreach (char character in s) ProcessCharacter(character, ref currentPattern, ref nextCode, db); // Process each character in string
    if (currentPattern != "") codes.Add(db.GetCode(currentPattern)); // Add code of last pattern if not empty
}
```
Within `ProcessCharacter`, the decision to extend a pattern or emit a code and add a new dictionary entry exemplifies encapsulation of algorithmic state. Neither `Program` nor `Menu` need to know how codes are generated—only that they will receive a list of integers representing the compressed data.


### Decompression
Decompression mirrors compression but in reverse. Decompressor starts by reading the first code to seed both the output and the dictionary pointer:
```C#
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
```
As each subsequent code arrives, `Expand` handles two scenarios: a known code or the special case where the code refers to a sequence not yet in the dictionary. The method then updates the dictionary and returns the decoded string fragment:
```C#
private string Expand(Dictionary<int, string> dict, ref int nextCode, ref string current, int code)
{
    string entry = dict.ContainsKey(code) ? dict[code] : current + current[0]; // If code doesn't exist, create pattern from current plus first character
    dict[nextCode++] = current + entry[0]; // Add new pattern to dictionary
    current = entry; // Update current pattern
    return entry;    // Return expanded string
}
```
By returning a single object instead of disparate values, the design guarantees that consumers cannot accidentally mix up which list belongs to which dictionary, reinforcing type safety and clarity.


### Output
![Image](https://github.com/user-attachments/assets/29fd05d6-b702-47ff-a340-436af8502533)



## Draggable Objects - Exercise 4
The purpose of the exercise is to create a `Windows Forms` application with 3 labels, added using the `Designer`, that can be moved with the mouse. The code must follow the Single Responsibility Principle (SRP) – separate label movement logic from UI layout and other concerns.


### Labels Inicialization
During the form's construction, all `Label` controls added by the `Windows Forms Designer` are automatically assigned two mouse event handlers: `MouseDown` and `MouseMove`. This ensures that each label can respond to mouse actions individually without hardcoding their names.
```C#
public form1()
{
    InitializeComponent();
    foreach (Control control in Controls)
    {
        if (control is Label)
        {
            // Initialize methods responsible for handling object movement 
            // or their overlapping
            control.MouseMove += new MouseEventHandler(ObjectMove);
            control.MouseDown += new MouseEventHandler(MouseAction);
        }
    }
}
```
This loop detects every Label on the form and attaches event handlers that will manage its movement behavior.


### Mouse Interaction Logic
Two methods control how labels react to mouse input:
- `ObjectMove`: Calculates and updates the label's position as the mouse moves,
- `MouseAction`: Captures the initial click location (on left click) and brings the label to the front (on right click).
```C#
// Method that changes the position of the object
private void ObjectMove(object sender, MouseEventArgs e)
{
    if (e.Button == MouseButtons.Left)
    {
        Label box = sender as Label;
        // When the user holds the left mouse button and moves the cursor,
        // the object also moves based on the control’s position saved in its tag
        box.Left += e.X - ((Point)box.Tag).X;
        box.Top += e.Y - ((Point)box.Tag).Y;
    }
}
// Method handles the mouse down event, reacting to mouse clicks
private void MouseAction(object sender, MouseEventArgs e)
{
    if (e.Button == MouseButtons.Left)
    {
        Label box = sender as Label;
        box.Tag = e.Location; // When the user clicks the left mouse button, the cursor position is saved
    }
    if (e.Button == MouseButtons.Right)
    {
        Label box = sender as Label;
        box.BringToFront();
    }
}
```
`Tag` is used to store the original mouse position relative to the label. On `MouseMove`, the difference between the current and original position determines how far the label should be moved.

This minimal and modular approach aligns with the Single Responsibility Principle, keeping logic and UI concerns separate and manageable.


### Output
![Image](https://github.com/user-attachments/assets/6e9fcd3b-731d-4d9e-b5b4-4c6b168ceda6)



## Event-Driven Object Manipulation - Exercise 5
Purpose of the project is to create a `Windows Forms` application that dynamically generates all of its UI elements at runtime—without using the `Designer` and provides both mouse‐driven and game-style keyboard controls for manipulating a single black object on the form.

The interface will consist of ten buttons `UP, U-P, >>>, D-P, DOWN, D-L, <<<, U-L, INCREASE, DECREASE` arranged next to the black label. Clicking any button or pressing its corresponding keyboard shortcut moves or resizes the shape accordingly. A Label on the form displays, in real time, the list of all currently held down keys `Active keys`, updating instantly as keys are pressed and released.


### Core Architecture and Object‑Oriented Design
The `Form1` class initializes the window, configures global settings, and delegates control creation and input handling to other components.
```C#
public partial class Form1 : Form // Main form class
{
    private readonly InterfaceManager interfaceManager; // Variable for managing the interface
    private readonly Handler keyboardHandler; // Variable for handling the keyboard

    public Form1()
    {
        InitializeComponent();
        this.Size = new Size(816, 489);
        this.StartPosition = FormStartPosition.CenterScreen;
        this.KeyPreview = true; // Allow reading keystrokes from the keyboard

        interfaceManager = new Interface(this); // Initialize
        keyboardHandler = new KeyboardHandler(interfaceManager);

        this.KeyDown += keyboardHandler.OnKeyDown; // Handle/add action when a key is pressed
        this.KeyUp += keyboardHandler.OnKeyUp; // Handle/add action when a key is released

        interfaceManager.InitializeControls(); // Initialization of controls
    }
}
```
The `Interface` class implements `InterfaceManager`, responsible for instantiating controls (buttons, draggable label, active‑keys label) and wiring event handlers. It maintains a set of active keys and updates the display whenever keys are pressed or released.

Keyboard events are routed through the `KeyboardHandler`, which implements the `Handler` interface. By calling `ProcessKeyDown` and `ProcessKeyUp` on the `InterfaceManager`, it ensures that the UI logic remains decoupled from raw event data.


### Component Implementation
The LayoutConfig class encapsulates button positions, sizes, and associated actions:
```C#
public class LayoutConfig
{
    public Point LabelPosition { get; private set; } // Initial position
    public Size LabelSize { get; private set; } // Label size
    public List<ButtonConfig> Buttons { get; private set; } // List of buttons

    public LayoutConfig()
    {
        LabelSize = new Size(90, 40);
        LabelPosition = new Point(500, 167);
        Buttons = new List<ButtonConfig>
        {
            new ButtonConfig("U-L", 21, 67, lc => lc.MoveUp().MoveLeft()),
            new ButtonConfig("<<<", 21, 107, lc => lc.MoveLeft()),
            new ButtonConfig("D-L", 21, 147, lc => lc.MoveDown().MoveLeft()),
            new ButtonConfig("UP", 120, 47, lc => lc.MoveUp()),
            new ButtonConfig("DOWN", 120, 167, lc => lc.MoveDown()),
            new ButtonConfig("U-P", 219, 67, lc => lc.MoveUp().MoveRight()),
            new ButtonConfig(">>>", 219, 107, lc => lc.MoveRight()),
            new ButtonConfig("D-P", 219, 147, lc => lc.MoveDown().MoveRight()),
            new ButtonConfig("INCREASE", 120, 240, lc => lc.IncreaseSize()),
            new ButtonConfig("DECREASE", 120, 280, lc => lc.DecreaseSize())
        };
    }
}
```
`ButtonConfig` pairs button text and location with an `Action<LabelController>`, allowing each button’s click handler to invoke the appropriate method on the `LabelController`.

All controls are generated by `ControlCreator` and its abstract base ButtonCreatorBase. The concrete `ButtonCreator` produces non‑focusable buttons to prevent interference with keyboard events:
```C#
public class ButtonCreator : ButtonCreatorBase // Button creator
{
    public override Button CreateButton(string text, int x, int y, Action onClickAction)
    {
        // Create a button without focus, using an inheriting class
        NonFocusableButton button = new NonFocusableButton
        {
            Text = text,
            Size = new Size(93, 35),
            Location = new Point(x, y),
            Font = new Font("Arial", 9, FontStyle.Bold)
        };
        button.Click += (sender, args) => onClickAction(); // Add action on click
        return button;
    }
}
```
The `DraggableLabel` is a subclass of `Label` with event hooks for mouse down and move, relying on `LabelMover` to implement drag‑and‑drop behavior.


###  Behavior Control
The `LabelController` encapsulates all operations that modify the label’s position and size. It records the original dimensions, applies a scaleFactor within defined bounds, and exposes chainable move methods:
```C#
public LabelController(DraggableLabel label, int moveStep = 5, float scaleFactor = 1.1f, float minScale = 0.2f, float maxScale = 3.0f)
{
    this.label = label;
    this.moveStep = moveStep;
    this.scaleFactor = scaleFactor;
    this.minScale = minScale;
    this.maxScale = maxScale;
    currentScale = 1.0f;
    initialWidth = label.Width;
    initialHeight = label.Height;
}

public LabelController MoveLeft() // Move label left by moveStep value
{
    label.Left -= moveStep;
    return this; // Return object reference
}
public void IncreaseSize() // Enlarge label
{
    float newScale = currentScale * scaleFactor; // New scale – product of current scale and factor
    if (newScale <= maxScale) // Add upper limit
    {
        currentScale = newScale;
        UpdateSize();
    }
}
```
To provide immediate feedback on user input, the `Interface` class maintains a `HashSet<string>` of active keys, updating the `activeKeysLabel` whenever keys are pressed or released. This mechanism reuses the `SimulateKeyPress` method to display temporary key‑press animations triggered by button clicks.
```C#
private void UpdateActiveKeysLabel() // Refresh the view of active keys and display the information
{
    activeKeysLabel.Text = "Active keys: " + string.Join(", ", activeKeys);
}
```
Thanks to constructor injection, interface abstractions, and single‑responsibility classes, new features—such as snapping to grid, undo/redo, or support for additional control types—can be added without modifying existing code significantly. New button types simply require additional ButtonConfig entries, and behaviors can be encapsulated in new methods on `LabelController` or separate strategy classes.



## Configurable Compression Display Tool - Exercise 6
The purpose of the project is to build a program that, in the future, could be used for encoding by compression algorithms. The application consists of three panels: 
- the `Work Panel`, which, upon clicking the `AUTO` button, will display 102 labels and read-only textboxes that will represent the output of the compression algorithm,
- the `Parameters`, which will add a pair of labels and textboxes that (in the future) will allow the user to customize the entire `Work Panel`,
- the `Arrangement`,  which consists of four buttons: one to display objects in the `Work Panel` in a default way, one to clear the `Work Panel`, one to open the `Parameters` panel, and one to exit the application.

All controls that require event handling—such as `CheckBoxes`, `Buttons`, and fields that open a color selection dialog must have appropriately declared and implemented event handler methods. `TextBoxes` must include input validation to ensure that only numeric values are allowed, letters and other invalid characters are not permitted. The entire interface should be built dynamically, with objects passed appropriately, and the architecture must adhere to the `Single Responsibility Principle (SRP)`. Additionally, every user action must be reflected with a message displayed on a `Label` to inform the user of the current operation.


### Overview and Project Structure
The code follows object-oriented principles to separate concerns and make each component responsible for a single part of the UI or behavior. The entry point is the form1 class, which initializes the main window, sets up shared services, and delegates the creation and management of all UI elements to helper classes.
```C#
public partial class form1 : Form
{
    public static Panel workPanel; // Static panel that is the main container for objects
    private Controlls crl; // Object of the class that defines the controls
    private ControlsManager controlsManager; // Object that manages the controls
    public form1()
    {
        InitializeComponent();
        MessageService.Initialize(this); // Initialize message handling and operation of individual classes
        InitializeWorkPanel();

        crl = new Controlls(); 
        controlsManager = new ControlsManager(workPanel, crl, this);

        this.Size = new Size(1030, 620);
    }
```
`workPanel` is declared static so that it can be referenced by any manager or builder without repeatedly passing the same instance. `ControlsManager` orchestrates the construction of the three main panels and wires up their event handlers.


### ControlsManager
`ControlsManager` encapsulates all logic related to creating and laying out the three `GroupBox` panels inside `workPanel`. By isolating panel construction in its own class, `form1` remains focused on overall application setup, while `ControlsManager` only manages controls.
```C#
class ControlsManager
{
    private Panel workPanel;
    private Controlls crl;
    private Form parentForm;

    public ControlsManager(Panel panel, Controlls controls, Form form)
    {
        workPanel = panel;
        crl = controls;
        parentForm = form;
    }

    public void LoadControls()
    {
        // We create 3 GroupBox-es
        GroupBox Gb001 = crl.Create_GoupBox(15, 10, 825, 450, "Work Panel", "GbWorkPanel");
        workPanel.Controls.Add(Gb001);

        GroupBox Gb002 = crl.Create_GoupBox(850, 10, 120, 450, "Arrangement", "GbArrangement");
        workPanel.Controls.Add(Gb002);

        GroupBox Gb003 = crl.Create_GoupBox(15, 465, 955, 80, "Parameters", "GbParameters");
        workPanel.Controls.Add(Gb003);

        Font font = new Font("Microsoft Sans Serif", 8, FontStyle.Bold);
        Color foreColor = Color.DarkBlue;
        Color backColor = Gb002.BackColor;

        // Button AUTO
        Button btAutoGenerate = crl.Create_Button("BtAutoGenerate", 12, 25, 95, 50,
            font, foreColor, backColor, "AUTO");
        btAutoGenerate.Click += (s, e) => ((form1)parentForm).AutoArrangeControls();
        btAutoGenerate.MouseHover += (s, e) => ButtonHover(btAutoGenerate);
        btAutoGenerate.MouseLeave += (s, e) => ButtonLeave(btAutoGenerate);
        Gb002.Controls.Add(btAutoGenerate);

        // Similarly for the rest of the buttons
```
The constructor receives `workPanel`, the `Controlls` factory, and a reference to `parentForm` for callback methods which is a use of dependency injection rule.

Inline lambda expressions delegate clicks back to `form1` methods such as `AutoArrangeControls`, preserving encapsulation.


### Builder Pattern
For the more complex `Parameters` panel, we apply a builder-style approach. The `ParametersPanelBuilder` class is responsible for all aspects of the `Parameters` UI: checkboxes, textboxes, color-pickers, and the GENERATE button.
```C#
class ParametersPanelBuilder
{
    private readonly Panel workPanel; // References
    private readonly Controlls crl;
    private readonly form1 form;

    // Build the constructor
    public ParametersPanelBuilder(Panel workPanel, Controlls crl, form1 form)
    {
        this.workPanel = workPanel;
        this.crl = crl;
        this.form = form;
    }

    public void Build()
    {
        // Search for the GroupBox named "GbParameters" inside workPanel — assume it already exists
        var gbParameters = (GroupBox)workPanel.Controls.Find("GbParameters", true)[0];
        // Set general parameters for the created controls
        Font font = new Font("Microsoft Sans Serif", 10, FontStyle.Bold);
        Color foreColor = Color.DarkBlue;
        Color backColor = Color.Transparent;

        // CheckBoxes
        var labelCB = crl.Create_CheckBox("labelCB", new Point(15, 20), 90, font, foreColor, false, "Label");
        var textBoxCB = crl.Create_CheckBox("textBoxCB", new Point(15, 45), 90, font, foreColor, false, "TextBox");
        labelCB.CheckedChanged += form.cb2_CheckedChanged;
        textBoxCB.CheckedChanged += form.cb2_CheckedChanged;

        // Similarly for the rest of the controlls...

        // Attach KeyPress event to all TextBoxes whose names start with "TbParam"
        foreach (Control ctrl in gbParameters.Controls)
            if (ctrl is TextBox tb && tb.Name.StartsWith("TbParam"))
                tb.KeyPress += form.OnlyNumeric_KeyPress;
    }
}
```
Thanks to this class the way that objects are created is reusable.


### Event-Driven Notifications
To provide feedback, `MessageService` centralizes the display of transient status messages on a dedicated `Label` with a `Timer`. This separates notification logic from UI-construction or business logic.
```C#
class MessageService
{
    private static Label messageLabel; // Label that displays the message
    private static Timer messageTimer; // Timer to count down 3 seconds

    public static void Initialize(Form IB_71578_form)
    {
        messageLabel = new Label()
        {
            Size = new Size(250, 30),
            ForeColor = Color.Black,
            Location = new Point(15, 10),
            Visible = false,
            Font = new Font("Microsoft Sans Serif", 10, FontStyle.Bold)
        };
        IB_71578_form.Controls.Add(messageLabel); // Add the label to the form

        messageTimer = new Timer();
        messageTimer.Interval = 3000; // 3 seconds
        messageTimer.Tick += (s, e) =>
        {
            messageLabel.Visible = false; // Hide the message
            messageTimer.Stop(); // Stop the timer
        };
    }

    public static void ShowTemporaryMessage(string msg)
    {
        messageLabel.Text = msg; // Set the text
        messageLabel.Visible = true; // Show the message
        messageTimer.Start(); // Start the timer
    }
}
```
By using static methods and fields, any class can invoke `MessageService.ShowTemporaryMessage("…")` without needing an instance. The `Timer.Tick` event hides the message automatically after three seconds, freeing callers from cleanup responsibilities.

Through these four components the application follows SOLID and object-oriented design principles, ensuring clarity, maintainability, and separation of concerns.









