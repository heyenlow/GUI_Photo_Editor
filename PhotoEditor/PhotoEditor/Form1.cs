using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PhotoEditor
{
	public partial class Form1 : Form
	{

		List<FileInfo> files = new List<FileInfo>();

		private FolderBrowserDialog folderBrowserDialog1 = new FolderBrowserDialog();

		public Form1()
		{
			InitializeComponent();

			listView1.View = View.LargeIcon;
			imageList2.ImageSize = new Size(64, 64);

			populateTreeView(Environment.GetFolderPath(Environment.SpecialFolder.MyPictures));

			setTreeViewIcons();
			
			//PopulateImageList(new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.MyPictures)));

		}

		private void populateTreeView(string path)
		{
			treeView1.Nodes.Clear();

			var rootDirectoryInfo = new DirectoryInfo(path);

			treeView1.Nodes.Add(CreateDirectoryNode(rootDirectoryInfo));

			ScanDirectory(new DirectoryInfo(path));  //https://stackoverflow.com/questions/816566/how-do-you-get-the-current-project-directory-from-c-sharp-code-when-creating-a-c
		}

		private static TreeNode CreateDirectoryNode(DirectoryInfo directoryInfo)
		{
			var directoryNode = new TreeNode(directoryInfo.Name);

			foreach (var directory in directoryInfo.GetDirectories())
				directoryNode.Nodes.Add(CreateDirectoryNode(directory));

			foreach (var file in directoryInfo.GetFiles())
				directoryNode.Nodes.Add(new TreeNode(file.Name));

			return directoryNode;
		}

		private void setTreeViewIcons()
		{
			imageList1.Images.Add(new Bitmap(GetType(), "folder-closed.png"));
			imageList1.Images.Add(new Bitmap(GetType(), "folder-open.png"));
			imageList1.Images.Add(new Bitmap(GetType(), "image-icon.png"));
			treeView1.ImageList = imageList1;
		}

		private void ScanDirectory(DirectoryInfo dir)
		{
			listView1.Items.Clear();
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
					listView1.Items.Add(file.Name,intI);
					//Console.WriteLine("Filename: " + file.Name);
					//Console.WriteLine("Last mod: " + file.LastWriteTime.ToString());
					//Console.WriteLine("File size: " + file.Length);
				}
				catch
				{
					Console.WriteLine("This is not an image file");
				}
			}
			listView1.LargeImageList = imageList2;
			listView1.SmallImageList = imageList2;
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
			
			var folderPath = treeView1.SelectedNode.FullPath;
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

		}

		private void smallToolStripMenuItem_Click(object sender, EventArgs e)
		{
			imageList2.ImageSize = new Size(16, 16);
			listView1.View = View.SmallIcon;
		}

		private void largeToolStripMenuItem_Click(object sender, EventArgs e)
		{
			imageList2.ImageSize = new Size(64, 64);
			listView1.View = View.LargeIcon;
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
			ScanDirectory(new DirectoryInfo(treeView1.SelectedNode.FullPath));
		}
    }
}