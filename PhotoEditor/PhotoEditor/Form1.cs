using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PhotoEditor
{
	public partial class Form1 : Form
	{

		List<FileInfo> files = new List<FileInfo>();
		public static Bitmap EditedPhoto;

		private FolderBrowserDialog folderBrowserDialog1 = new FolderBrowserDialog();
		private string ScanPath;
		public Form1()
		{
			InitializeComponent();

			listView1.View = View.Details;
			imageList2.ImageSize = new Size(64, 64);

			listView1.Columns.Add("Name", -2, HorizontalAlignment.Left);
			listView1.Columns.Add("Last Changed", -2, HorizontalAlignment.Left);
			listView1.Columns.Add("Size", -2, HorizontalAlignment.Left);

			populateTreeView(Environment.GetFolderPath(Environment.SpecialFolder.MyPictures));

			setTreeViewIcons();
			
			//PopulateImageList(new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.MyPictures)));

		}

		private void populateTreeView(string path)
		{
			treeView1.Nodes.Clear();
			int intNodeIndex;
			DirectoryInfo rootDirectoryInfo;
			rootDirectoryInfo = new DirectoryInfo(path);
			intNodeIndex = treeView1.Nodes.Add(CreateDirectoryNode(rootDirectoryInfo));
			treeView1.Nodes[intNodeIndex].Tag = rootDirectoryInfo.FullName;
			ScanPath = path;
			backgroundWorker1.RunWorkerAsync(); //https://stackoverflow.com/a/6481328/13966072
		}

		private static TreeNode CreateDirectoryNode(DirectoryInfo directoryInfo)
		{
			var directoryNode = new TreeNode(directoryInfo.Name);
			int intNodeIndex;

			foreach (var directory in directoryInfo.GetDirectories())
            {
				intNodeIndex = directoryNode.Nodes.Add(CreateDirectoryNode(directory));
				directoryNode.Nodes[intNodeIndex].Tag = directory.FullName;
            }
				 

			//foreach (var file in directoryInfo.GetFiles())
			//	directoryNode.Nodes.Add(new TreeNode(file.Name));

			return directoryNode;
		}

		private void setTreeViewIcons()
		{
			imageList1.Images.Add(new Bitmap(GetType(), "folder-closed.png"));
			imageList1.Images.Add(new Bitmap(GetType(), "folder-open.png"));
			imageList1.Images.Add(new Bitmap(GetType(), "image-icon.png"));
			treeView1.ImageList = imageList1;
		}

		async private void ScanDirectory(DirectoryInfo dir)
		{
			listView1.Items.Clear();


			//setProgressBar(dir.GetFiles("*jpeg").Length);

			int intI = -1;
			foreach (FileInfo file in dir.GetFiles("*.jpeg"))
			{
				try
				{ 
					intI += 1;
				
					files.Add(file);
					byte[] bytes = File.ReadAllBytes(file.FullName);
					MemoryStream ms = new MemoryStream(bytes);
					Image img = Image.FromStream(ms); // Don’t use Image.FromFile() !!!
					imageList2.Images.Add(img);
					listView1.Items.Add(new ListViewItem(new[] { file.Name, file.LastWriteTime.ToString(), (file.Length / 1024).ToString() + " KB" }, intI)); //https://stackoverflow.com/a/22387272/13966072
					//Thread.Sleep(100);
					progressBar1.PerformStep();
				}
				catch
				{
					Console.WriteLine("This is not an image file");
				}
			}
			//setProgressBar();
		}

		private void setProgressBar(int max = 0)
		{
			progressBar1.Value = 0;
			progressBar1.Minimum = 0;
			progressBar1.Maximum = max;
			progressBar1.Step = 1;
		}

		private void PopulateImageList(DirectoryInfo dir)
		{
			ImageList imageList = new ImageList();
			imageList.ImageSize = new Size(32, 32);

			String[] paths = { };
			paths = Directory.GetFiles(dir.FullName);

			try
			{
				foreach (String path in paths)
				{
					byte[] bytes = File.ReadAllBytes(path);
					MemoryStream ms = new MemoryStream(bytes);
					imageList.Images.Add(Image.FromStream(ms));
				}
			} catch (Exception e)
			{
				MessageBox.Show(e.Message);
			}

			listView1.SmallImageList = imageList;

			var j = 0;
			foreach (var img in imageList.Images)
			{
				listView1.Items.Add(Name, j++);
			}
		}

		private void listView1_SelectedIndexChanged(object sender, EventArgs e)
		{
			ListView.SelectedIndexCollection indexes = this.listView1.SelectedIndices;
			foreach (int index in indexes) {
				EditPhoto EP = new EditPhoto(files[index]);
			}
			//if (DialogResult.OK == EP.ShowDialog())
			//{

			//}
		}

		private void Form1_Load(object sender, EventArgs e)
		{
			
		}

		//private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
		//{
		//    EditPhoto EP = new EditPhoto(files[listBox1.SelectedIndex]);
		//    if(DialogResult.OK == EP.ShowDialog())
		//    {

		//    }

		//}

		private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
		{
			
		}

		private void menuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
		{

		}

		private void locateOnDiskToolStripMenuItem_Click(object sender, EventArgs e)
		{
			
			var folderPath = treeView1.SelectedNode.Tag.ToString();
			if (Directory.Exists(folderPath))
			{
				ProcessStartInfo startInfo = new ProcessStartInfo
				{
					Arguments = folderPath,
					FileName = "explorer.exe"
				};

				Process.Start(startInfo);
            }
            else
            {
				MessageBox.Show(string.Format("{0} Directory does not exist!", folderPath));
            }
		}

		private void selectRootFolderToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
			{
				populateTreeView(folderBrowserDialog1.SelectedPath);
			}
		}

		private void exitToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Close();
		}

		private void detailToolStripMenuItem_Click(object sender, EventArgs e)
		{
			listView1.View = View.Details;
		}

		private void smallToolStripMenuItem_Click(object sender, EventArgs e)
		{
			listView1.View = View.SmallIcon;
			imageList2.ImageSize = new Size(64, 64);
		}

		private void largeToolStripMenuItem_Click(object sender, EventArgs e)
		{
			listView1.View = View.LargeIcon;
			//imageList2.ImageSize = new Size(128, 128);
		}

		private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
		{
			var aboutDialog = new AboutDialog();
			aboutDialog.ShowDialog();
		}

		private void listView1_SelectedIndexChanged_1(object sender, EventArgs e)
		{
			ListView.SelectedIndexCollection indexes = this.listView1.SelectedIndices;
			foreach (int index in indexes)
			{
				EditPhoto EP = new EditPhoto(files[index]);
				Console.WriteLine("Here");
				DialogResult EditResult = EP.ShowDialog();
			}
		}

        private void viewToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void treeView1_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
			ScanDirectory(new DirectoryInfo(e.Node.Tag.ToString()));
		}

        private void progressBar1_SizeChanged(object sender, EventArgs e)
        {

        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
			progressBar1.Visible = true;
			ScanDirectory(new DirectoryInfo(ScanPath));  //https://stackoverflow.com/questions/816566/how-do-you-get-the-current-project-directory-from-c-sharp-code-when-creating-a-c
		}

		private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
			//backgroundWorker1.ReportProgress(
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
			progressBar1.Visible = false;
        }
    }
}