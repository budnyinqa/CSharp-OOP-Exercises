using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Configurable_Compression_Display_Tool
{
    public partial class Form1 : Form
    {
        public static Panel workPanel; // Static panel that is the main container for objects
        private Controlls crl; // Object of the class that defines the controls
        private ControlsManager controlsManager; // Object that manages the controls
        public Form1()
        {
            InitializeComponent();
            MessageService.Initialize(this); // Initialize message handling and operation of individual classes
            InitializeWorkPanel();

            crl = new Controlls();
            controlsManager = new ControlsManager(workPanel, crl, this);

            this.Size = new Size(1030, 620);
        }
        private void form1_Load(object sender, EventArgs e)
        {
            controlsManager.LoadControls();
        }
        private void InitializeWorkPanel()
        {
            workPanel = new Panel // We create panel
            {
                Name = "workPanel",
                Location = new Point(10, 10),
                Size = new Size(990, 560),
                BackColor = Color.LightGray,
                BorderStyle = BorderStyle.FixedSingle
            };
            Controls.Add(workPanel);
        }

        private void BtAutoGenerate_Click(object sender, EventArgs e)
        {
            AutoArrangeControls(); // We arrange controlls automaticly
        }

        private void BtManualGenerate_Click(object sender, EventArgs e)
        {
            ClearWorkPanelControls(); // Clears the work panel
            CustomizationControls(); // Adds new controls
        }

        // Handling clicks of the CLEAR and EXIT buttons
        private void BtClear_Click(object sender, EventArgs e)
        {
            ClearGroupBoxes(0); // Remove controls from both panels
        }

        private void BtExit(object sender, EventArgs e)
        {
            this.Close(); // Close the application
        }

        public void ClearWorkPanelControls()
        {
            // Find the GroupBox container named "GbWorkPanel"
            // inside the WorkPanel container
            GroupBox gb = (GroupBox)workPanel.Controls.Find("GbWorkPanel", true)[0];

            // Check if the container contains any controls
            if (gb.Controls.Count > 0)
                gb.Controls.Clear();
        }

        public GroupBox ClearGroupBoxes(int function)
        {
            // Find the GroupBox container named "GbWorkPanel"
            // inside the WorkPanel container
            GroupBox gb1 = (GroupBox)this.Controls.Find("GbWorkPanel", true)[0];

            // Check if the container contains any controls
            if (gb1.Controls.Count > 0)
                gb1.Controls.Clear();

            GroupBox gb2 = (GroupBox)this.Controls.Find("GbParameters", true)[0];

            // Check if the container contains any controls
            if (gb2.Controls.Count > 0)
                gb2.Controls.Clear();
            switch (function)
            {
                case 0:
                    return null;
                    break;
                case 1:
                    return gb1;
                    break;
                case 2:
                    return gb2;
                    break;
                default:
                    return null;
                    break;
            }
        }

        public void AutoArrangeControls()
        {
            // Clear controls from GbParameters
            GroupBox gbParameters = (GroupBox)workPanel.Controls.Find("GbParameters", true)[0];
            if (gbParameters.Controls.Count > 0)
                gbParameters.Controls.Clear();

            ClearWorkPanelControls();

            GroupBox gb = (GroupBox)workPanel.Controls.Find("GbWorkPanel", true)[0];

            // Declaration of local variables
            int topMargin = 20;
            int leftMargin = 15;
            int labelWidth = 40;
            int labelHeight = 25;
            int nbOfControlls = 110;
            string controlName = "";
            Point controllPoint = new Point(leftMargin, topMargin);
            Font labelFont = new Font("Tahoma", 12, FontStyle.Bold);
            Font textboxFont = new Font("Tahoma", 8, FontStyle.Bold);
            Color labelColor = gb.BackColor;
            Color textboxColor = Color.Bisque;
            Color foreColor = Color.Red;

            // Iterative arrangement of Label and TextBox objects
            for (int i = 1; i <= nbOfControlls; i++)
            {
                // Add a Label object
                controlName = "Lb" + i.ToString();
                Label newLabel = crl.Create_Label(controlName, controllPoint,
                                labelFont, labelColor, foreColor, labelWidth,
                                labelHeight, i.ToString());
                gb.Controls.Add(newLabel);
                controllPoint.X = controllPoint.X + 40;
                controllPoint.Y = controllPoint.Y + 0;

                // Add a TextBox object
                controlName = "Tb" + i.ToString();
                TextBox newTextBox = crl.Create_TextBox(controlName,
                controllPoint, 60, 15, textboxFont,
                textboxColor, foreColor);
                gb.Controls.Add(newTextBox);
                controllPoint.X = leftMargin;

                if (controllPoint.Y + 35 > gb.Height)
                {
                    leftMargin = leftMargin + 130;
                    controllPoint.X = leftMargin;
                    controllPoint.Y = topMargin;
                }
                else
                    controllPoint.Y = controllPoint.Y + 25;

                if (controllPoint.X + 125 > gb.Width)
                    i = nbOfControlls;
            }
        }
        public void CustomizationControls()
        {
            var builder = new ParametersPanelBuilder(workPanel, crl, this); // Create a builder for the configuration interface
            builder.Build(); // Build the configuration interface
        }

        public void BackColorTB_Click(object sender, EventArgs e)
        {
            ColorDialog colorDialog = new ColorDialog(); // Create a ColorBox dialog
            if (colorDialog.ShowDialog() == DialogResult.OK) // Check if the user confirmed the color selection
            {
                TextBox tb = sender as TextBox;
                tb.BackColor = colorDialog.Color; // Set the background color of the text box
                tb.Text = colorDialog.Color.Name; // And its name
                MessageService.ShowTemporaryMessage($"Selected background color: {colorDialog.Color.Name}"); // Show a message
            }
        }

        public void ForeColorTB_Click(object sender, EventArgs e)
        {
            ColorDialog colorDialog = new ColorDialog();
            if (colorDialog.ShowDialog() == DialogResult.OK)
            {
                TextBox tb = sender as TextBox; // Similarly
                tb.ForeColor = colorDialog.Color;
                tb.Text = colorDialog.Color.Name;
                MessageService.ShowTemporaryMessage($"Wybrano kolor tekstu: {colorDialog.Color.Name}");
            }
        }
        public void cb2_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox cb = sender as CheckBox;
            if (cb != null && cb.Checked) // If any of the checkboxes is selected
            {
                foreach (Control control in cb.Parent.Controls) // Iterate through the remaining checkboxes
                {
                    if (control is CheckBox otherCb && otherCb != cb) // Uncheck all others
                    {
                        otherCb.Checked = false;
                    }
                }
                MessageService.ShowTemporaryMessage($"Selected: {cb.Text}");
            }
            else
            {
                MessageService.ShowTemporaryMessage($"Deselected: {cb.Text}");
                return; // Send appropriate notifications
            }
        }
        public void BtGenerate_Click(object sender, EventArgs e)
        {
            MessageService.ShowTemporaryMessage("GENERATE button pressed"); // Display a notification when the user presses GENERATE
        }

        public void OnlyNumeric_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true; // Block the character if it's not a digit
                MessageBox.Show("Only digits are allowed!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning); // Show an error notification
            }
        }
    }
    class ParametersPanelBuilder
    {
        private readonly Panel workPanel; // References
        private readonly Controlls crl;
        private readonly Form1 form;

        // Build the constructor
        public ParametersPanelBuilder(Panel workPanel, Controlls crl, Form1 form)
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

            // Sizes
            var widthL = crl.Create_Label("widthLabel", new Point(115, 21), font, backColor, foreColor, 60, 25, "Width:");
            var heightL = crl.Create_Label("heightLabel", new Point(115, 46), font, backColor, foreColor, 60, 25, "Height:");
            var widthTB = crl.Create_TextBox("TbParamWidth", new Point(180, 19), 70, 200, font, Color.BlanchedAlmond, Color.Black);
            var heightTB = crl.Create_TextBox("TbParamHeight", new Point(180, 44), 70, 200, font, Color.BlanchedAlmond, Color.Black);

            // Colors
            var backColorL = crl.Create_Label("backColorL", new Point(260, 21), font, backColor, foreColor, 95, 25, "Back Color:");
            var foreColorL = crl.Create_Label("foreColorL", new Point(260, 46), font, backColor, foreColor, 95, 25, "Fore Color:");
            var backColorTB = crl.Create_TextBox("TbParamBack", new Point(355, 19), 60, 25, font, Color.DarkGreen, Color.Black);
            var foreColorTB = crl.Create_TextBox("TbParamFore", new Point(355, 44), 60, 25, font, Color.Black, Color.Black);
            backColorTB.Multiline = true; backColorTB.BorderStyle = BorderStyle.None;
            foreColorTB.Multiline = true; foreColorTB.BorderStyle = BorderStyle.None;
            backColorTB.Click += form.BackColorTB_Click;
            foreColorTB.Click += form.ForeColorTB_Click;

            // Margins
            var topMarginL = crl.Create_Label("topMarginL", new Point(430, 21), font, backColor, foreColor, 95, 25, "Top margin:");
            var leftMarginL = crl.Create_Label("leftMarginL", new Point(430, 46), font, backColor, foreColor, 95, 25, "Left margin:");
            var topMarginTB = crl.Create_TextBox("TbParamTop", new Point(530, 19), 70, 200, font, Color.BlanchedAlmond, Color.Black);
            var leftMarginTB = crl.Create_TextBox("TbParamLeft", new Point(530, 44), 70, 200, font, Color.BlanchedAlmond, Color.Black);

            // Shift
            var columnsShiftL = crl.Create_Label("columnsShiftL", new Point(615, 21), font, backColor, foreColor, 110, 25, "Columns shift:");
            var rowsShiftL = crl.Create_Label("rowsShiftL", new Point(615, 46), font, backColor, foreColor, 100, 25, "Rows shift:");
            var columnsShiftTB = crl.Create_TextBox("TbParamCols", new Point(730, 19), 70, 200, font, Color.BlanchedAlmond, Color.Black);
            var rowsShiftTB = crl.Create_TextBox("TbParamRows", new Point(730, 44), 70, 200, font, Color.BlanchedAlmond, Color.Black);

            // GENERATE
            var btGenerate = crl.Create_Button("BtGenerate", 820, 17, 120, 50, font, foreColor, backColor, "GENERATE");
            btGenerate.Click += form.BtGenerate_Click;

            // Add all controls to the panel:
            gbParameters.Controls.AddRange(new Control[] {
            labelCB, textBoxCB,
            widthL, heightL, widthTB, heightTB,
            backColorL, foreColorL, backColorTB, foreColorTB,
            topMarginL, leftMarginL, topMarginTB, leftMarginTB,
            columnsShiftL, rowsShiftL, columnsShiftTB, rowsShiftTB,
            btGenerate
        });


            // Attach KeyPress event to all TextBoxes whose names start with "TbParam"
            foreach (Control ctrl in gbParameters.Controls)
                if (ctrl is TextBox tb && tb.Name.StartsWith("TbParam"))
                    tb.KeyPress += form.OnlyNumeric_KeyPress;
        }
    }
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
            btAutoGenerate.Click += (s, e) => ((Form1)parentForm).AutoArrangeControls();
            btAutoGenerate.MouseHover += (s, e) => ButtonHover(btAutoGenerate);
            btAutoGenerate.MouseLeave += (s, e) => ButtonLeave(btAutoGenerate);
            Gb002.Controls.Add(btAutoGenerate);

            // Button MANUAL
            Button btManualGenerate = crl.Create_Button("BtManualGenerate", 12, 85, 95, 50,
                font, foreColor, backColor, "MANUAL");
            btManualGenerate.Click += (s, e) =>
            {
                ((Form1)parentForm).ClearWorkPanelControls();
                ((Form1)parentForm).CustomizationControls();
            };
            btManualGenerate.MouseHover += (s, e) => ButtonHover(btManualGenerate);
            btManualGenerate.MouseLeave += (s, e) => ButtonLeave(btManualGenerate);
            Gb002.Controls.Add(btManualGenerate);

            // Button CLEAR
            Button btClear = crl.Create_Button("BtClear", 12, 330, 95, 50,
                font, foreColor, backColor, "CLEAR");
            btClear.Click += (s, e) => ((Form1)parentForm).ClearGroupBoxes(0);
            btClear.MouseHover += (s, e) => ButtonHover(btClear);
            btClear.MouseLeave += (s, e) => ButtonLeave(btClear);
            Gb002.Controls.Add(btClear);

            // Button EXIT
            Button btExit = crl.Create_Button("BtExit", 12, 390, 95, 50,
                font, foreColor, backColor, "EXIT");
            btExit.Click += (s, e) => parentForm.Close();
            btExit.MouseHover += (s, e) => ButtonHover(btExit);
            btExit.MouseLeave += (s, e) => ButtonLeave(btExit);
            Gb002.Controls.Add(btExit);
        }
        private void ButtonHover(Button button) // When the mouse pointer enters the button
        {
            if (button != null)
                button.BackColor = Color.Orange; // Change its color
        }

        private void ButtonLeave(Button button) // When the mouse pointer leaves the button
        {
            if (button != null)
                button.BackColor = button.Parent.BackColor; // Restore the color
        }
    }
    class MessageService
    {
        private static Label messageLabel; // Label that displays the message
        private static Timer messageTimer; // Timer to count down 3 seconds

        public static void Initialize(Form form)
        {
            messageLabel = new Label()
            {
                Size = new Size(250, 30),
                ForeColor = Color.Black,
                Location = new Point(15, 10),
                Visible = false,
                Font = new Font("Microsoft Sans Serif", 10, FontStyle.Bold)
            };
            form.Controls.Add(messageLabel); // Add the label to the form

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
}
