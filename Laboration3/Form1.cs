using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace Laboration3
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        //variables to keep track of couple of things during runtime, such as filename, path and etc.
        string path = "";
        string filename = "";
        string temp_text = "";
        int rad = 0;
        int ord = 0;
        int char_count = 0;
        int char2_count = 0;
        DialogResult res;
        int flag = 0;
        //function that makes sure to ask user to save file if file is changed otherwise remove all variables.
        private void newToolStripMenuItem_click(object sender, EventArgs e)
        {
            if (this.Text == "*" + filename + "-Anteckningar" || this.Text == "*Namnlös-Anteckningar")
            {
                if (this.Text == "*Namnlös-Anteckningar")
                {
                    res = MessageBox.Show("Do you want to save the changes for Namnlös?", "Anteckningar", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                }
                else
                {
                    res = MessageBox.Show("Do you want to save the changes for " + Path.Combine(path, filename) + "?", "Anteckningar", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                }
                if (res == DialogResult.Yes)
                {
                    saveToolStripMenuItem_click(sender, e);
                }
                if (res == DialogResult.No)
                {
                    richTextBox1.Clear();
                    this.Text = "Namnlös-Anteckningar";
                    filename = "";
                    temp_text = "";
                    path = "";
                    rad = 0;
                    ord = 0;
                    char_count = 0;
                    char2_count = 0;
                }
            }
            else if (flag != 1)
            {
                richTextBox1.Clear();
                this.Text = "Namnlös-Anteckningar";
                filename = "";
                temp_text = "";
                path = "";
                rad = 0;
                ord = 0;
                char_count = 0;
                char2_count = 0;
            }
        }
        //function for exit button
        private void exitToolStripMenuItem_click(object sender, EventArgs e)
        {
            newToolStripMenuItem_click(sender, e);
            if (res == DialogResult.Cancel)
            {
                res = DialogResult.None;
                return;
            }
            else
            {
                Application.Exit();
            }
        }
        //function for open file button
        private void openToolStripMenuItem_click(object sender, EventArgs e)
        {
            flag = 1; //make sure to not remove variables when open, they will be replaced later in function
            newToolStripMenuItem_click(sender, e);
            flag = 0;
            if (res == DialogResult.Cancel)
            {
                res = DialogResult.None;
                return;
            }
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Text Files (.txt)|*.txt";
            openFileDialog.Title = "Öppna";
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                System.IO.StreamReader sr = new System.IO.StreamReader(openFileDialog.FileName);
                path = System.IO.Path.GetDirectoryName(openFileDialog.FileName);
                filename = System.IO.Path.GetFileName(openFileDialog.FileName);
                this.Text = filename + "-Anteckningar";
                richTextBox1.Text = sr.ReadToEnd();
                temp_text = richTextBox1.Text;
                sr.Close();
            }
            else
            {
                return;
            }
        }
        //function for save as button
        private void saveAsToolStripMenuItem_click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Text Files (.txt)|*.txt";
            saveFileDialog.Title = "Spara som";
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    System.IO.StreamWriter sw = new System.IO.StreamWriter(saveFileDialog.FileName);
                    path = System.IO.Path.GetDirectoryName(saveFileDialog.FileName);
                    filename = System.IO.Path.GetFileName(saveFileDialog.FileName);
                    this.Text = filename + "-Anteckningar";
                    temp_text = richTextBox1.Text;
                    sw.Write(richTextBox1.Text);
                    sw.Close();
                }
                catch (UnauthorizedAccessException ioe)
                {
                    MessageBox.Show("Error saving file: " + ioe.Message);
                }
            }
            else
            {
                res = DialogResult.Cancel;
                return;
            }
        }
        //function for save button
        private void saveToolStripMenuItem_click(object sender, EventArgs e)
        {
            if (path == string.Empty) saveAsToolStripMenuItem_click(sender, e);
            else
            {
                try
                {
                    string total_path = Path.Combine(path, filename);
                    System.IO.StreamWriter sw = new System.IO.StreamWriter(total_path);
                    sw.Write(richTextBox1.Text);
                    sw.Close();
                    temp_text = richTextBox1.Text;
                    this.Text = filename + "-Anteckningar";
                }
                catch (UnauthorizedAccessException ioe)
                {
                    MessageBox.Show("Error saving file: " + ioe.Message);
                }
            }
        }
        //function for key event in rich textbox, this function changes title name and status bar depending on if specific conditons are fulfilled
        private void textbox_change(object sender, KeyEventArgs e)
        {
            char_count = 0;
            char2_count = 0;
            rad = 0;
            ord = 0;
            if (path == string.Empty)
            {
                if (!string.IsNullOrEmpty(richTextBox1.Text)) this.Text = "*Namnlös-Anteckningar";
                else this.Text = "Namnlös-Anteckningar";
            }
            else if (e.KeyCode == Keys.S && e.Control)
            {
                this.Text = filename + "-Anteckningar";
                return;
            }
            else if (richTextBox1.Text != temp_text) this.Text = "*" + filename + "-Anteckningar";
       
            rad = richTextBox1.Text.Split('\n').Length;
            ord = richTextBox1.Text.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).Length;
            foreach(char c in richTextBox1.Text)
            {
                char_count++;
            }
            char2_count = char_count - richTextBox1.Text.Count(Char.IsWhiteSpace);
            toolStripStatusLabel4.Text = "Rad " + rad + " ";
            toolStripStatusLabel3.Text = "Ord " + ord + ", ";
            toolStripStatusLabel2.Text = "Tecken (utan blanksteg) " + char2_count + ", ";
            toolStripStatusLabel1.Text = "Tecken (med blanksteg) " + char_count + ", ";
            
        }
        //function for mananing  drag and drop files, keystate 8 is ctrl and 4 is shift.
        private void RichBox1_DragDrop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop, false);
            foreach(string file in files)
            {
                if (e.KeyState != 8 && e.KeyState != 4)
                {
                    newToolStripMenuItem_click(sender, e);
                    if (res == DialogResult.Cancel)
                    {
                        res = DialogResult.None;
                        return;
                    }
                    richTextBox1.Text = File.ReadAllText(file);
                    path = System.IO.Path.GetDirectoryName(file);
                    filename = System.IO.Path.GetFileName(file);
                    this.Text = filename + "-Anteckningar";
                    temp_text = richTextBox1.Text;
                }

                if (e.KeyState == 8)
                {
                    richTextBox1.Text = richTextBox1.Text + File.ReadAllText(file);
                    temp_text = richTextBox1.Text;
                    this.Text = "*"+filename + "-Anteckningar";
                }
                if(e.KeyState == 4)
                {
                    richTextBox1.SelectedText = File.ReadAllText(file);
                    temp_text = richTextBox1.Text;
                    this.Text = "*"+filename + "-Anteckningar";
                }
            }

        }
        // raised when user first drags the mouse over the rich textbox
        private void RichBox1_DragEnter(object sender, DragEventArgs e)
        {
            if(e.Data.GetDataPresent(DataFormats.FileDrop, false) == true)
            {
                e.Effect = DragDropEffects.All;
            }
        }
        //ctrl+z
        private void undoToolStripMenuItem_click(object sender, EventArgs e)
        {
            richTextBox1.Undo();
        }
        //ctrl+y
        private void redoToolStripMenuItem_click(object sender, EventArgs e)
        {
            richTextBox1.Redo();
        }
        //ctrl+x
        private void cutToolStripMenuItem_click(object sender, EventArgs e)
        {
            richTextBox1.Cut();
        }
        //ctrl+c
        private void copyToolStripMenuItem_click(object sender, EventArgs e)
        {
            richTextBox1.Copy();
        }
        //ctrl+v
        private void pasteToolStripMenuItem_click(object sender, EventArgs e)
        {
            richTextBox1.Paste();
        }
        //select all text
        private void selectAllToolStripMenuItem_click(object sender, EventArgs e)
        {
            richTextBox1.SelectAll();
        }
        // exit with X button
        private void Exit_Click(object sender, FormClosingEventArgs e)
        {
            newToolStripMenuItem_click(sender, e);
            if (res == DialogResult.Cancel)
            {
                res = DialogResult.None;
                e.Cancel = true;
                return;
            }
            Application.Exit();
        }
    }
}
