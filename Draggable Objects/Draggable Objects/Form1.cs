using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Draggable_Objects
{
    public partial class Form1 : Form
    {
        public Form1()
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
    }
}
