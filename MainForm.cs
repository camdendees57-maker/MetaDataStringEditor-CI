using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MetaDataStringEditor
{
    public class MainForm : Form
    {
        private ListBox listBox;
        private TextBox searchBox;
        private TextBox editBox;
        private Button loadBtn, saveBtn, replaceBtn;
        private List<string> strings = new List<string>();
        private string filePath;

        public MainForm()
        {
            Text = "MetaData String Editor";
            Width = 800;
            Height = 600;

            searchBox = new TextBox { Dock = DockStyle.Top };
            listBox = new ListBox { Dock = DockStyle.Fill };
            editBox = new TextBox { Dock = DockStyle.Bottom, Height = 80, Multiline = true };

            var panel = new Panel { Dock = DockStyle.Bottom, Height = 40 };
            loadBtn = new Button { Text = "Load", Dock = DockStyle.Left };
            saveBtn = new Button { Text = "Save", Dock = DockStyle.Left };
            replaceBtn = new Button { Text = "Replace Selected", Dock = DockStyle.Right };

            panel.Controls.Add(loadBtn);
            panel.Controls.Add(saveBtn);
            panel.Controls.Add(replaceBtn);

            Controls.Add(listBox);
            Controls.Add(editBox);
            Controls.Add(searchBox);
            Controls.Add(panel);

            loadBtn.Click += Load_Click;
            saveBtn.Click += Save_Click;
            replaceBtn.Click += Replace_Click;
            searchBox.TextChanged += Search_Changed;
            listBox.SelectedIndexChanged += List_Selected;
        }

        private void Load_Click(object sender, EventArgs e)
        {
            using var ofd = new OpenFileDialog { Filter = "Metadata|*.dat" };
            if (ofd.ShowDialog() != DialogResult.OK) return;
            filePath = ofd.FileName;
            try
            {
                var data = File.ReadAllBytes(filePath);
                // Basic extract of printable strings (works on unencrypted metadata)
                strings.Clear();
                var sb = new StringBuilder();
                for (int i = 0; i < data.Length; i++)
                {
                    if (data[i] >= 32 && data[i] < 127)
                        sb.Append((char)data[i]);
                    else if (sb.Length > 4)
                    {
                        strings.Add(sb.ToString());
                        sb.Clear();
                    }
                    else sb.Clear();
                }
                if (sb.Length > 4) strings.Add(sb.ToString());
                RefreshList();
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        private void RefreshList()
        {
            listBox.Items.Clear();
            var q = searchBox.Text.ToLower();
            foreach (var s in strings)
                if (string.IsNullOrEmpty(q) || s.ToLower().Contains(q))
                    listBox.Items.Add(s);
        }

        private void Search_Changed(object sender, EventArgs e) => RefreshList();

        private void List_Selected(object sender, EventArgs e)
        {
            if (listBox.SelectedItem != null)
                editBox.Text = listBox.SelectedItem.ToString();
        }

        private void Replace_Click(object sender, EventArgs e)
        {
            if (listBox.SelectedIndex < 0) return;
            var old = listBox.SelectedItem.ToString();
            var neu = editBox.Text;
            var idx = strings.IndexOf(old);
            if (idx >= 0) strings[idx] = neu;
            RefreshList();
        }

        private void Save_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(filePath)) return;
            MessageBox.Show("This basic version only extracts strings.\nFull length-safe rewrite needs the original tool logic.\nFor real patches use the official MetaDataStringEditor on unencrypted files.");
        }
    }
}
