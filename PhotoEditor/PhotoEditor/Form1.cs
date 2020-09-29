using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
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
        List<FileInfo> files;

        public Form1()
        {
            InitializeComponent();
            files = new List<FileInfo>();
            // Create three items with sets of subitems for each item

            ListViewItem item1 = new ListViewItem("item1", 0);   // Text and image index
            item1.SubItems.Add("1");   // Column 2
            item1.SubItems.Add("2");   // Column 3
            item1.SubItems.Add("3");   // Column 4

            ListViewItem item2 = new ListViewItem("item2", 1);
            item2.SubItems.Add("4");
            item2.SubItems.Add("5");
            item2.SubItems.Add("6");

            ListViewItem item3 = new ListViewItem("item3", 2);
            item3.SubItems.Add("7");
            item3.SubItems.Add("8");
            item3.SubItems.Add("9");

            // Create columns (Width of -2 indicates auto-size)
            listView1.Columns.Add("Column 1", -2, HorizontalAlignment.Left);
            listView1.Columns.Add("Column 2", -2, HorizontalAlignment.Left);
            listView1.Columns.Add("Column 3", 40, HorizontalAlignment.Right);
            listView1.Columns.Add("Column 4", 40, HorizontalAlignment.Center);

            // Add the items to the list view
            listView1.Items.AddRange(new ListViewItem[] { item1, item2, item3 });

            // Show default view
            //listView1.View = View.Details;
            listView1.View = View.LargeIcon;

            //all files info will be added into here
            

            ScanDirectory();
        }

        private void ScanDirectory()
        {
            // ! this must change !
                    DirectoryInfo computerHomeDir = new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.MyPictures));    //https://stackoverflow.com/questions/816566/how-do-you-get-the-current-project-directory-from-c-sharp-code-when-creating-a-c
                    Console.WriteLine(computerHomeDir);
            DirectoryInfo homeDir = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory);
            DirectoryInfo resDir = new DirectoryInfo(Path.Combine(homeDir.FullName, "Resources"));
            
            
            const string filePath = "C:\\Users\\konradheyen\\Documents\\GUI_Photo_Editor\\PhotoEditor\\PhotoEditor\\Resources";
            resDir = new DirectoryInfo(filePath);
            Console.WriteLine("Resource Directory: " + resDir);
            
            //ListBox myImages = new ListBox();
            foreach (FileInfo file in resDir.GetFiles("*.jpeg"))
            {
                try
                {
                    files.Add(file);
                    byte[] bytes = System.IO.File.ReadAllBytes(file.FullName);
                    MemoryStream ms = new MemoryStream(bytes);
                    Image img = Image.FromStream(ms); // Don’t use Image.FromFile() !!!
                    listBox1.Items.Add(file.Name);
                    Console.WriteLine("Filename: " + file.Name);
                    Console.WriteLine("Last mod: " + file.LastWriteTime.ToString());
                    Console.WriteLine("File size: " + file.Length);
                }
                catch
                {
                    Console.WriteLine("This is not an image file");
                }
            }
            //listBox1.Items = myImages.Items;
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            EditPhoto EP = new EditPhoto(files[listBox1.SelectedIndex]);
            EP.Show();
        }
    }
}
