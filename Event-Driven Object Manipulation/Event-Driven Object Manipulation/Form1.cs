using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Event_Driven_Object_Manipulation
{
    #region Form
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

    #endregion

    #region Interfaces
    public interface InterfaceManager
    {
        void InitializeControls(); // Method for initializing controls
        void ProcessKeyDown(Keys keyCode);
        void ProcessKeyUp(Keys keyCode);
        void SimulateKeyPress(string keyName); // Simulate key press
    }
    public interface Handler // Interface for keyboard handling
    {
        void OnKeyDown(object sender, KeyEventArgs e);
        void OnKeyUp(object sender, KeyEventArgs e);
    }
    #endregion

    #region Interface Implementation
    public class Interface : InterfaceManager
    {
        private readonly Form hostForm; // Reference to the main form
        private readonly LayoutConfig layoutConfig; // Layout configuration
        private readonly ControlCreator controlCreator; // Control creation
        private DraggableLabel draggableLabel; // Label
        private LabelController labelController; // Logic for moving and scaling the label
        private Label activeKeysLabel; // Label for active keys
        private readonly HashSet<string> activeKeys = new HashSet<string>(); // Storage for active keys
        private readonly Dictionary<Keys, string> keyMappings = new Dictionary<Keys, string> // Dictionary for key mapping
        {
            { Keys.Left, "<<<" },
            { Keys.Right, ">>>" },
            { Keys.Up, "UP" },
            { Keys.Down, "DOWN" },
            { Keys.Oemplus, "INCREASE" },
            { Keys.Add, "INCREASE" },
            { Keys.OemMinus, "DECREASE" },
            { Keys.Subtract, "DECREASE" }
        };

        public Interface(Form form)
        {
            hostForm = form;
            layoutConfig = new LayoutConfig(); // Creating layout configuration
            controlCreator = new ControlCreator();
        }

        public async void SimulateKeyPress(string keyName) // We simulate it to get consistent effect (keyboard button - form button)
        {
            activeKeys.Add(keyName); // Key name
            UpdateActiveKeysLabel(); // Refresh the label
            await Task.Delay(70); // Show it for a short time
            activeKeys.Remove(keyName); // Remove it
            UpdateActiveKeysLabel(); // Refresh again
        }

        public void ProcessKeyDown(Keys keyCode) // Update the set of active keys and take action on the label
        {
            // Applies only to the keyboard!
            // Check whether the key has a name assigned in the mapping dictionary. If not, use the default representation (e.g. "Left", "A", etc.)
            string keyText = keyMappings.ContainsKey(keyCode) ? keyMappings[keyCode] : keyCode.ToString();
            activeKeys.Add(keyText); // Add to the list
            UpdateActiveKeysLabel(); // Refresh the label

            // Label control - perform the appropriate action depending on the key
            switch (keyCode)
            {
                case Keys.Left: labelController.MoveLeft(); break;
                case Keys.Right: labelController.MoveRight(); break;
                case Keys.Up: labelController.MoveUp(); break;
                case Keys.Down: labelController.MoveDown(); break;
                case Keys.Oemplus:
                case Keys.Add: labelController.IncreaseSize(); break;
                case Keys.OemMinus:
                case Keys.Subtract: labelController.DecreaseSize(); break;
            }
        }

        public void ProcessKeyUp(Keys keyCode)
        {
            string keyText = keyMappings.ContainsKey(keyCode) ? keyMappings[keyCode] : keyCode.ToString();
            activeKeys.Remove(keyText); // when the key is released, remove it from active keys
            UpdateActiveKeysLabel(); // Refresh
        }

        public void InitializeControls()
        {
            draggableLabel = controlCreator.CreateDraggableLabel(layoutConfig, hostForm.ClientSize); // Create label
            LabelMover mover = new LabelMover(); // Mover for mouse movement handling
            draggableLabel.EnableDragging(mover); // Enable dragging
            hostForm.Controls.Add(draggableLabel); // Add to form

            labelController = new LabelController(draggableLabel); // Assign controller for label control

            foreach (var btnCfg in layoutConfig.Buttons) // Iterate over buttons
            {
                Button btn = controlCreator.CreateButton(btnCfg, labelController, this); // Create button
                hostForm.Controls.Add(btn); // Add to form
            }

            activeKeysLabel = new Label
            {
                Location = new Point(10, 10),
                AutoSize = true,
                Font = new Font("Arial", 10, FontStyle.Bold),
                ForeColor = Color.Black
            };
            hostForm.Controls.Add(activeKeysLabel);
            UpdateActiveKeysLabel(); // Set initial text
        }

        private void UpdateActiveKeysLabel() // Refresh the view of active keys and display the information
        {
            activeKeysLabel.Text = "Active keys: " + string.Join(", ", activeKeys);
        }
    }
    #endregion

    #region Keyboard Handling
    public class KeyboardHandler : Handler
    {
        private readonly InterfaceManager interfaceManager; // Reference

        public KeyboardHandler(InterfaceManager interfaceManager)
        {
            this.interfaceManager = interfaceManager;
        }

        public void OnKeyDown(object sender, KeyEventArgs e)
        {
            interfaceManager.ProcessKeyDown(e.KeyCode);
            ((Form)sender).ActiveControl = null; // Remove focus
            e.Handled = true; // Block further processing
        }

        public void OnKeyUp(object sender, KeyEventArgs e)
        {
            interfaceManager.ProcessKeyUp(e.KeyCode); // Delegate key release handling
            ((Form)sender).ActiveControl = null;
        }
    }
    #endregion

    #region Layout and Button Configuration
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
    public class ButtonConfig // Class configuring a single button
    {
        public string Text { get; }
        public int X { get; }
        public int Y { get; }
        public Action<LabelController> Action { get; }

        public ButtonConfig(string text, int x, int y, Action<LabelController> action)
        {
            Text = text; // Button label
            X = x; // X-axis position
            Y = y; // Y-axis position
            Action = action; // Assign action
        }
    }
    #endregion

    #region Control Creation
    public class ControlCreator
    {
        public DraggableLabel CreateDraggableLabel(LayoutConfig config, Size clientSize)
        {
            return new DraggableLabel(config.LabelPosition, config.LabelSize); // Create and return a label with a set position and size
        }

        // Create a button and attach click action along with key simulation
        public Button CreateButton(ButtonConfig btnCfg, LabelController controller, InterfaceManager interfaceManager)
        {
            ButtonCreator creator = new ButtonCreator();
            return creator.CreateButton(btnCfg.Text, btnCfg.X, btnCfg.Y, () =>
            {
                btnCfg.Action(controller); // Execute the action defined for the button
                interfaceManager.SimulateKeyPress(btnCfg.Text); // Simulate key press
            });
        }
    }

    public abstract class ButtonCreatorBase // Abstract button creator – allows for extensibility
    {
        public abstract Button CreateButton(string text, int x, int y, Action onClickAction);
    }

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

    // Add a special button that doesn't take focus
    // Very important – allows the form to receive events
    public class NonFocusableButton : Button
    {
        protected override bool ShowFocusCues => false; // Hide focus indicator

        public NonFocusableButton()
        {
            this.TabStop = false;
            this.SetStyle(ControlStyles.Selectable, false); // Remove "selectable" capability!!!
        }

        protected override void OnGotFocus(EventArgs e) // Just in case – if the button still gains focus
        {
            this.Parent?.SelectNextControl(this, true, true, true, true); // Move focus to the next control
            base.OnGotFocus(e);
        }
    }
    #endregion

    #region Mouse Event Handling
    public class DraggableLabel : Label
    {
        public DraggableLabel(Point location, Size size) // Label constructor
        {
            this.AutoSize = false;
            this.BackColor = Color.Black;
            this.Location = location;
            this.Size = size;
        }

        public void EnableDragging(LabelMover mover)
        {
            this.MouseDown += mover.HandleMouseDown; // React to mouse button press – store position
            this.MouseMove += mover.HandleMouseMove; // React to mouse movement – move the label
        }
    }

    public class LabelMover // Class handling mouse events for the label
    {
        public void HandleMouseDown(object sender, MouseEventArgs e) // Triggered on mouse button press
        {
            if (sender is Label label) // If the source of the event is a label
            {
                if (e.Button == MouseButtons.Left)
                    label.Tag = e.Location; // Left button – store the click point as "Tag" (temporary storage)
                else if (e.Button == MouseButtons.Right)
                    label.BringToFront(); // Right button – bring the label to the front
            }
        }

        public void HandleMouseMove(object sender, MouseEventArgs e) // Triggered when left mouse button is held down
        {
            // If the label has stored the click point and is being dragged
            if (sender is Label label && label.Tag is Point clickPoint && e.Button == MouseButtons.Left)
            {
                // Calculate cursor movement relative to the click point and move the label by that distance
                label.Left += e.X - clickPoint.X;
                label.Top += e.Y - clickPoint.Y;
            }
        }
    }
    #endregion

    #region Label Control Logic
    public class LabelController // Logic for moving and scaling
    {
        private readonly DraggableLabel label; // Reference to the controlled label
        private readonly int moveStep; // Number of pixels to move the label
        private readonly float scaleFactor; // Scale factor – for resizing
        private readonly float minScale; // Minimum allowed size
        private readonly float maxScale; // Maximum allowed size
        private float currentScale; // Default scale
        private readonly int initialWidth; // Original width
        private readonly int initialHeight; // Original height

        // Constructor sets all parameters and stores the label’s original size
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

        public LabelController MoveRight()
        {
            label.Left += moveStep;
            return this;
        }

        public LabelController MoveUp()
        {
            label.Top -= moveStep;
            return this;
        }

        public LabelController MoveDown()
        {
            label.Top += moveStep;
            return this;
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

        public void DecreaseSize() // Reduce label size
        {
            float newScale = currentScale / scaleFactor; // New scale – divide current scale by factor
            if (newScale >= minScale)
            {
                currentScale = newScale;
                UpdateSize();
            }
        }

        private void UpdateSize() // Private helper method to:
        {
            int newWidth = (int)(initialWidth * currentScale); // proportionally scale width
            int newHeight = (int)(initialHeight * currentScale); // and height
            label.Size = new Size(newWidth, newHeight); // Set final size
        }
    }
    #endregion
}
