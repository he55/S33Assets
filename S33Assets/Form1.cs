using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace S33Assets
{
    public partial class Form1 : Form
    {
        private const string PROJECT_NAME = "S33Assets";
        private const string BMP0_BIN = "BMP0.BIN";
        private string _binPath;
        private RKRS_H _rkrs;
        private BID_H _bid;
        private List<BID_H> _bids;
        private Bitmap _bitmap;

        public Form1()
        {
            InitializeComponent();
            pictureBox1.Size = new Size(0, 0);
#if !DEBUG
            listView1.Columns.Clear();
            listView1.Columns.AddRange(new ColumnHeader[] { columnHeader1, columnHeader2, columnHeader3 });
#endif
        }

        private int fatMet(string filePath)
        {
            ProcessStartInfo psi = new ProcessStartInfo();
            psi.FileName = "7z2107-x64\\7z.exe";
            psi.Arguments = $"e -y \"{filePath}\" RESOURCE\\BID\\BMP0.BIN";
            psi.UseShellExecute = true;
            psi.WindowStyle = ProcessWindowStyle.Hidden;

            Process process = Process.Start(psi);
            process.WaitForExit();
            return process.ExitCode;
        }

        private void LoadFile(string filePath)
        {
            _binPath = null;
            _rkrs = null;
            _bid = null;
            _bids = null;
            _bitmap = null;

            listView1.Items.Clear();
            pictureBox1.Image = null;
            searchButton.Enabled = false;
            saveButton.Enabled = false;
            saveItemsButton.Enabled = false;
            toolStripStatusLabel1.Text = "图片数量: 0";
            textBox1.Text = filePath;

            if (filePath.EndsWith(".img", StringComparison.OrdinalIgnoreCase))
            {
                toolStripStatusLabel3.Text = "正在提取资源...";
                Application.DoEvents();

                int result = fatMet(filePath);
                if (result != 0)
                {
                    MessageBox.Show("资源提取失败", PROJECT_NAME);
                    toolStripStatusLabel3.Text = "资源提取失败";
                    return;
                }

                _binPath = BMP0_BIN;
            }
            else
            {
                _binPath = filePath;
            }

            toolStripStatusLabel3.Text = "正在加载数据...";
            Application.DoEvents();

            try
            {
                _rkrs = RKRSFile.ReadFile(_binPath);
            }
            catch
            {
                MessageBox.Show("数据加载失败", PROJECT_NAME);
                toolStripStatusLabel3.Text = "数据加载失败";
                return;
            }

            ReloadBidData();
            searchButton.Enabled = true;
            toolStripStatusLabel3.Text = "完成";
        }

        private void ReloadBidData()
        {
            if (_rkrs != null)
            {
                _bids = _rkrs._bids.Where(x =>
                    (checkBox1.Checked && (x._bidd.width * x._bidd.height < 320 * 240)) ||
                    (checkBox2.Checked && (x._bidd.width * x._bidd.height > 320 * 240)) ||
                    (checkBox3.Checked && (x._bidd.width * x._bidd.height == 320 * 240)) ||
                    (checkBox4.Checked && (x._bidd.width * x._bidd.height == 480 * 272)) ||
                    (checkBox5.Checked && (x._bidd.width * x._bidd.height == 400 * 240)) ||
                    (checkBox6.Checked && (x._bidd.width * x._bidd.height == 800 * 480))).ToList();
                ReloadListView();
            }
        }

        private void ReloadListView()
        {
            listView1.BeginUpdate();
            listView1.Items.Clear();
            foreach (BID_H item in _bids)
            {
#if DEBUG
                ListViewItem listViewItem = new ListViewItem(new string[] { item.Id, item.Name, item.Size, item.Offset, item.Length, item.D3, item.D4, item.D5 });
#else
                ListViewItem listViewItem = new ListViewItem(new string[] { item.Id, item.Name, item.Size });
#endif
                listView1.Items.Add(listViewItem);
            }
            listView1.EndUpdate();

            if (_bids.Count == 0)
            {
                pictureBox1.Image = null;
            }
            else
            {
                listView1.SelectedIndices.Add(0);
            }

            saveButton.Enabled = _bids.Count != 0;
            saveItemsButton.Enabled = _bids.Count != 0;
            toolStripStatusLabel1.Text = $"图片数量: {_bids.Count}";
        }


        #region 窗体事件

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (File.Exists(BMP0_BIN))
            {
                try
                {
                    File.Delete(BMP0_BIN);
                }
                catch { }
            }
        }

        private void Form1_DragEnter(object sender, DragEventArgs e)
        {
            StringCollection files = ((DataObject)e.Data).GetFileDropList();
            if (files.Count > 0 &&
                (files[0].EndsWith(".img", StringComparison.OrdinalIgnoreCase) || files[0].EndsWith(".bin", StringComparison.OrdinalIgnoreCase)))
            {
                e.Effect = DragDropEffects.All;
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }
        }

        private void Form1_DragDrop(object sender, DragEventArgs e)
        {
            StringCollection files = ((DataObject)e.Data).GetFileDropList();
            LoadFile(files[0]);
        }

        #endregion


        #region 控件事件

        private void openButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image Files|*.img|BMP0 Files|*.bin";
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                LoadFile(openFileDialog.FileName);
            }
        }

        private void searchButton_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(bidTextBox.Text))
            {
                ReloadBidData();
                return;
            }

            if (int.TryParse(bidTextBox.Text, NumberStyles.HexNumber, null, out int id))
            {
                _bids = _rkrs._bids.Where(x => x._bid == id).ToList();
                ReloadListView();
            }
        }

        private void checkBox_Click(object sender, EventArgs e)
        {
            if (sender == checkBox2 && checkBox2.Checked)
            {
                checkBox4.Checked = true;
                checkBox5.Checked = true;
                checkBox6.Checked = true;
            }
            ReloadBidData();
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listView1.SelectedIndices.Count > 0)
            {
                _bid = _bids[listView1.SelectedIndices[0]];
                Bitmap bitmap = RKRSFile.ReadBitmap(_binPath, _bid._bindex);
                pictureBox1.Image = bitmap;

                _bitmap?.Dispose();
                _bitmap = bitmap;
            }
        }

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            string pngfile = Path.Combine(Path.GetTempPath(), $"{_bid.Id}.png");
            _bitmap.Save(pngfile, ImageFormat.Png);

            DataObject dataObject = new DataObject();
            dataObject.SetFileDropList(new StringCollection { pngfile });

            DoDragDrop(dataObject, DragDropEffects.Move);
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.FileName = $"{_bid.Id}.png";
            saveFileDialog.Filter = "PNG Files|*.png";

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                _bitmap.Save(saveFileDialog.FileName, ImageFormat.Png);
            }
        }

        private void saveItemsButton_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
            if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                string folder = Path.Combine(folderBrowserDialog.SelectedPath, "Images");
                if (!Directory.Exists(folder))
                {
                    Directory.CreateDirectory(folder);
                }

                toolStripStatusLabel3.Text = "正在保存图片...";
                Application.DoEvents();

                using (FileStream fileStream = File.OpenRead(_binPath))
                {
                    foreach (BID_H bid in _bids)
                    {
                        using (Bitmap bitmap = RKRSFile.ReadBitmap(fileStream, bid._bindex))
                        {
                            string pngfile = Path.Combine(folder, $"{bid.Id}.png");
                            bitmap.Save(pngfile, ImageFormat.Png);
                        }
                    }
                }
                toolStripStatusLabel3.Text = "完成";
            }
        }

        #endregion
    }
}
