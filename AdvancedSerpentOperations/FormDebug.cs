using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

/*
Author: Halil Kemal TASKIN
Web: http://hkt.me
*/

namespace AdvancedSerpentOperations
{
    public partial class FormDebug : Form
    {
        public FormDebug()
        {
            InitializeComponent();
        }

        private void FormDebug_Load(object sender, EventArgs e)
        {

        }

        public void WriteLine(string s)
        {
            textBox1.AppendText(s + Environment.NewLine);
        }

        public void Write(string s)
        {
            textBox1.AppendText(s);
        }

        public string ReadAllDebug()
        {
            return textBox1.Text;
        }

        public void ClearDebug()
        {
            textBox1.Clear();
        }

        private void FormDebug_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
        }
    }
}
