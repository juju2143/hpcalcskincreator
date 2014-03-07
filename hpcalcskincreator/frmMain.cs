using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace hpcalcskincreator
{
    public partial class frmMain : Form
    {
        string filename;
        Skin calc;
        public frmMain()
        {
            InitializeComponent();
            string[] args = Environment.GetCommandLineArgs();
            if (args.Length > 1)
            {
                filename = args[1];
                loadFile();
            }
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openSkinDialog.InitialDirectory = (Environment.GetEnvironmentVariable("PROGRAMFILES(X86)") ?? Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles)) + "\\Hewlett-Packard\\HP Prime Virtual Calculator";
            DialogResult result = openSkinDialog.ShowDialog(this);
            if (result == DialogResult.OK)
            {
                filename = openSkinDialog.FileName;
                loadFile();
            }
        }

        private void loadFile()
        {
            lblStatus.Text = "Loading...";
            progressBar.Style = ProgressBarStyle.Marquee;
            progressBar.Value = 0;
            progressBar.Visible = true;
            calc = new Skin();
            Environment.CurrentDirectory = Path.GetDirectoryName(filename);
            this.Text = "HP Skin Creator (" + Path.GetFileName(filename) + ")";
            string[] lines = File.ReadAllLines(filename);
            progressBar.Maximum = lines.Length;
            progressBar.Style = ProgressBarStyle.Blocks;
            string[] args;
            int i = 0;
            try
            {
                for (i = 0; i < lines.Length; i++)
                {
                    progressBar.PerformStep();
                    string[] split = lines[i].Split("#".ToCharArray(), 2, StringSplitOptions.None);
                    split = split[0].Split("=".ToCharArray(), 2, StringSplitOptions.None);
                    switch (split[0])
                    {
                        case "picture":
                            picSkin.ImageLocation = split[1];
                            txtSkin.Text = split[1];
                            break;
                        case "size":
                            args = split[1].Split(",".ToCharArray());
                            calc.Size = new Rectangle(0, 0, Int32.Parse(args[0]), Int32.Parse(args[1]));
                            nudX.Maximum = nudWidth.Maximum = nudSkinWidth.Value = picSkin.Width = Int32.Parse(args[0]);
                            nudY.Maximum = nudHeight.Maximum = nudSkinHeight.Value = picSkin.Height = Int32.Parse(args[1]);
                            break;
                        case "screen":
                            args = split[1].Split(",".ToCharArray());
                            calc.Screen.X = Int32.Parse(args[0]);
                            calc.Screen.Y = Int32.Parse(args[1]);
                            calc.Screen.Width = Int32.Parse(args[2]);
                            calc.Screen.Height = Int32.Parse(args[3]);
                            break;
                        case "key":
                            args = split[1].Split(",".ToCharArray(), 7, StringSplitOptions.None);
                            int j, id;
                            j = args[0].StartsWith("\"") ? 1 : 0;
                            id = Int32.Parse(args[j]);
                            calc.Keys[id] = new Key();
                            calc.Keys[id].ID = id;
                            if (args[0].StartsWith("\"")) calc.Keys[id].KeyName = args[0][1];
                            calc.Keys[id].Position.X = Int32.Parse(args[j + 1]);
                            calc.Keys[id].Position.Y = Int32.Parse(args[j + 2]);
                            calc.Keys[id].Position.Width = Int32.Parse(args[j + 3]) - calc.Keys[id].Position.X;
                            calc.Keys[id].Position.Height = Int32.Parse(args[j + 4]) - calc.Keys[id].Position.Y;
                            break;
                        case "border":
                            args = split[1].Split(",".ToCharArray(), StringSplitOptions.None);
                            calc.border = new Point[args.Length / 2];
                            for (int k = 0; k < args.Length; k += 2)
                            {
                                calc.border[(int)Math.Floor((double)k / 2)] = new Point(Int32.Parse(args[k]), Int32.Parse(args[k + 1]));
                            }
                            break;
                        case "MAXIMIZED":
                            args = split[1].Split(",".ToCharArray(), StringSplitOptions.None);
                            calc.Maximized.X = Int32.Parse(args[0]);
                            calc.Maximized.Y = Int32.Parse(args[1]);
                            calc.Maximized.Width = Int32.Parse(args[2]);
                            calc.Maximized.Height = Int32.Parse(args[3]);
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(String.Format("Parse error at line {0}: {1}\n\n{2}", i + 1, ex.Message, lines[i]), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error, 0, 0, ex.HelpLink, HelpNavigator.Topic);
                filename = "";
                calc = null;
                picSkin.ImageLocation = "";
                txtSkin.Text = "";
                picSkin.Width = 0;
                picSkin.Height = 0;
                this.Text = "HP Skin Creator";
            }
            picSkin.Invalidate();
            progressBar.Visible = false;
            lblStatus.Text = "Ready";
        }

        private void quitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void picSkin_Paint(object sender, PaintEventArgs e)
        {
            if (calc != null)
            {
                Graphics g = e.Graphics;
                g.DrawPolygon(new Pen(Brushes.Purple, 3), calc.border);
                g.DrawRectangle(new Pen(Brushes.Purple, 3), calc.Maximized);
                g.FillRectangle(new SolidBrush(Color.FromArgb(127, Color.Red)), calc.Screen);
                g.DrawString("Screen", new Font(FontFamily.GenericSansSerif, 8), Brushes.Black, calc.Screen.Location);
                foreach (Key k in calc.Keys)
                {
                    if (k != null)
                    {
                        g.FillRectangle(new SolidBrush(Color.FromArgb(127, Color.Green)), k.Position);
                        g.DrawString(k.ID.ToString(), new Font(FontFamily.GenericSansSerif, 8), Brushes.Black, k.Position.Location);
                    }
                }
            }
        }

        private void picSkin_MouseMove(object sender, MouseEventArgs e)
        {
            lblPosition.Text = "(" + e.X + ", " + e.Y + ")";
        }

        private void picSkin_MouseLeave(object sender, EventArgs e)
        {
            lblPosition.Text = "";
        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            filename = "";
            calc = null;
            picSkin.ImageLocation = "";
            txtSkin.Text = "";
            picSkin.Width = 0;
            picSkin.Height = 0;
            this.Text = "HP Skin Creator";
        }

        private void picSkin_MouseClick(object sender, MouseEventArgs e)
        {
            if (calc.Screen.Contains(e.Location))
            {
                cbKey.SelectedIndex = 0;
                nudX.Value = calc.Screen.X;
                nudY.Value = calc.Screen.Y;
                nudWidth.Value = calc.Screen.Width;
                nudHeight.Value = calc.Screen.Height;
            }
            else
            {
                foreach(Key k in calc.Keys)
                    if (k != null && k.Position.Contains(e.Location))
                    {
                        cbKey.SelectedIndex = k.ID + 1;
                        nudX.Value = k.Position.X;
                        nudY.Value = k.Position.Y;
                        nudWidth.Value = k.Position.Width;
                        nudHeight.Value = k.Position.Height;
                    }
            }
        }

        private void nudX_ValueChanged(object sender, EventArgs e)
        {
            nudWidth.Maximum = picSkin.Width - nudX.Value;
        }

        private void nudY_ValueChanged(object sender, EventArgs e)
        {
            nudHeight.Maximum = picSkin.Height - nudY.Value;
        }

        private void txtSkin_TextChanged(object sender, EventArgs e)
        {
            picSkin.ImageLocation = txtSkin.Text;
            //picSkin.Width = picSkin.Image.Width;
            //picSkin.Height = picSkin.Image.Height;
        }

        private void btnOpenSkin_Click(object sender, EventArgs e)
        {
            openBMPDialog.InitialDirectory = (Environment.GetEnvironmentVariable("PROGRAMFILES(X86)") ?? Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles)) + "\\Hewlett-Packard\\HP Prime Virtual Calculator";
            DialogResult result = openBMPDialog.ShowDialog(this);
            if (result == DialogResult.OK)
            {
                txtSkin.Text = openBMPDialog.FileName;
            }
        }

        private void cbKey_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbKey.SelectedIndex == 0)
            {
                nudX.Value = calc.Screen.X;
                nudY.Value = calc.Screen.Y;
                nudWidth.Value = calc.Screen.Width;
                nudHeight.Value = calc.Screen.Height;
            }
            else
            {
                nudX.Value = calc.Keys[cbKey.SelectedIndex - 1].Position.X;
                nudY.Value = calc.Keys[cbKey.SelectedIndex - 1].Position.Y;
                nudWidth.Value = calc.Keys[cbKey.SelectedIndex - 1].Position.Width;
                nudHeight.Value = calc.Keys[cbKey.SelectedIndex - 1].Position.Height;
            }
        }
    }
}