using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Configurable_Compression_Display_Tool
{
    internal class Controlls
    {
        public Button Create_Button(string name, int x, int y, int width, int height, Font font, Color foreColor, Color backColor, string text)
        {
            Button button = new Button
            {
                Name = name,
                Location = new Point(x, y),
                Size = new Size(width, height),
                Font = font,
                ForeColor = foreColor,
                BackColor = backColor,
                Text = text,
                TextAlign = ContentAlignment.MiddleCenter
            };
            return button;
        }
        public CheckBox Create_CheckBox(string name, Point location, int width, Font font,
                                Color foreColor, bool selected, string text)
        {
            CheckBox checkBox = new CheckBox // create a new CheckBox object
            {
                Location = location,
                Width = width,
                Font = font,
                ForeColor = foreColor,
                Checked = selected,   // set the initial checked state
                Name = name,          // set the control's name
                Text = text
            };
            return checkBox;
        }
        public GroupBox Create_GoupBox(int x, int y, int width, int height, string text,
                               string name)
        {
            GroupBox groupBox = new GroupBox  // create a new GroupBox object
            {
                Location = new Point(x, y),
                Width = width,
                Height = height,
                Text = text,
                Name = name
            };
            return groupBox;
        }

        public Label Create_Label(string name, Point location, Font font, Color backColor,
                                  Color foreColor, int width, int height, string text)
        {
            Label label = new Label   // create a new Label object
            {
                Name = name,
                Location = location,
                //AutoSize = false,   // disable default size  
                Width = width,
                Height = height,
                Font = font,
                BackColor = backColor,
                ForeColor = foreColor,
                Text = text
            };
            return label;
        }

        public TextBox Create_TextBox(string name, Point location, int width, int height,
                                      Font font, Color backColor, Color foreColor)
        {
            TextBox textBox = new TextBox
            // create a new TextBox object
            {
                Location = location,
                Width = width,
                Height = height,
                Name = name,
                Font = font,
                BackColor = backColor,
                ForeColor = foreColor,
                BorderStyle = BorderStyle.FixedSingle, // set border style
            };
            return textBox;
        }
    }
}
