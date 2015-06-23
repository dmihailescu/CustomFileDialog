using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using SharpShell.Attributes;
using SharpShell.SharpContextMenu;
using System.Runtime.InteropServices;
using System.IO;
using System.Diagnostics;
using System.Security.Cryptography;

namespace FileCompareByHashExtension
{
    [ComVisible(true)]
    [COMServerAssociation(AssociationType.AllFiles)]
    public class MultiFileCompareByHash : SharpContextMenu
    {
        /// <summary>
        /// Determines whether this instance can a shell context show menu, given the specified selected file list.
        /// </summary>
        /// <returns>
        ///   <c>true</c> if this instance should show a shell context menu for the specified file list; otherwise, <c>false</c>.
        /// </returns>
        protected override bool CanShowMenu()
        {
            //  MessageBox.Show("menu == null is " + (_menu == null).ToString());
            //  We can show the item only for a single selection.
            int itemcnt = SelectedItemPaths.Count();
            if (itemcnt == 0)
                return false;
            return true;
        }
        ContextMenuStrip _menu;
        ToolStripMenuItem _compareItem;
        ToolStripMenuItem _md5Item;
        ToolStripMenuItem _sha1Item;
        ToolStripMenuItem _sha256Item;
        ToolStripMenuItem _rootItem;
        /// <summary>
        /// Creates the context menu. This can be a single menu item or a tree of them.
        /// </summary>
        /// <returns>
        /// The context menu for the shell context menu.
        /// </returns>
        protected override ContextMenuStrip CreateMenu()
        {

            //  Create the menu strip.
            _menu = new ContextMenuStrip();
            //  if (this.SelectedItemPaths.Count() > 1)
            {
                //  Create a menu item to hold all of the subitems.
                _rootItem = new ToolStripMenuItem
                {
                    Text = "Compare&&Hash",
                    //Image = Properties.Resources.Copy
                };

                //  Now add the child items.
                _compareItem = new ToolStripMenuItem
                {
                    Text = "Compare with...",
                    // Checked = File.GetAttributes(SelectedItemPaths.First()).HasFlag(FileAttributes.ReadOnly)
                };



                _compareItem.Click += (sender, args) => CompareAgainst();
                //  Add the touch item.
                _md5Item = new ToolStripMenuItem
                {
                    Text = "Calculate MD5",
                    Enabled = !File.GetAttributes(SelectedItemPaths.First()).HasFlag(FileAttributes.ReadOnly)
                };
                _md5Item.Click += (sender, args) => ShowHash<MD5CryptoServiceProvider>();//CountLines(); //DoTouch();

                _sha1Item = new ToolStripMenuItem
                {
                    Text = "Calculate SHA1",
                    //Image = Properties.Resources.Copy
                };

                _sha1Item.Click += (sender, args) => ShowHash<SHA1CryptoServiceProvider>();
                _sha256Item = new ToolStripMenuItem
                {
                    Text = "Calculate SHA256",
                    //Image = Properties.Resources.Copy
                };
                _sha256Item.Click += (sender, args) => ShowHash<SHA256CryptoServiceProvider>();

                //  Add the items.
                _rootItem.DropDownItems.Add(_compareItem);
                _rootItem.DropDownItems.Add(new ToolStripSeparator());
                _rootItem.DropDownItems.Add(_md5Item);
                _rootItem.DropDownItems.Add(_sha1Item);
                _rootItem.DropDownItems.Add(_sha256Item);
                //  Add the item to the context menu.
                _menu.Items.Add(_rootItem);
            }
            if (_menu != null)
            {
                UpdateMenu();
            }
            //  Return the menu.
            return _menu;
        }

        void CompareAgainst()
        {

            string file1 = SelectedItemPaths.FirstOrDefault();
            if (file1 == null || !File.Exists(file1))
                return;
            string ext = Path.GetExtension(file1);
            OpenFileDialog openFileDialog = new OpenFileDialog();

            openFileDialog.InitialDirectory = Path.GetDirectoryName(file1);
            openFileDialog.Filter = string.Format("specific files (*{0})|*{0}|All files (*.*)|*.*", ext);
            openFileDialog.FilterIndex = 1;
            openFileDialog.RestoreDirectory = true;
            openFileDialog.Title = "Comparing against " + file1;
            if (CurrentInvokeCommandInfo.WindowHandle == IntPtr.Zero)
            {
                // MessageBox.Show("Compare against file, Hwnd = " + CurrentInvokeCommandInfo.WindowHandle.ToString("X"));
                if (openFileDialog.ShowDialog() != DialogResult.OK)
                    return;

            }
            else
            {

                NativeWindow nw = new NativeWindow();
                try
                {

                    nw.AssignHandle(CurrentInvokeCommandInfo.WindowHandle);
                    //     MessageBox.Show(nw, "Compare against file, Hwnd = " + CurrentInvokeCommandInfo.WindowHandle.ToString("X"));
                    if (openFileDialog.ShowDialog(nw) != DialogResult.OK)
                        return;


                }
                finally
                {
                    nw.ReleaseHandle();
                }

            }
            try
            {
                string file2 = openFileDialog.FileName;

                FileInfo fi1 = new FileInfo(file1);
                FileInfo fi2 = new FileInfo(file2);
                if (FilesContentsAreEqual(fi1, fi2))
                    MessageBox.Show(string.Format("The files '{0}' and '{1}' are identical.",Path.GetFileName(file1),Path.GetFileName(file2)),"Binary comapriosn results");
                else
                    MessageBox.Show(string.Format("The files '{0}' and '{1}' are different.", Path.GetFileName(file1), Path.GetFileName(file2)), "Binary comapriosn results");

            }
            catch (Exception ex)
            {

                MessageBox.Show("Error: Could not read file from disk. Original error: " + ex.Message);
            }

        }
        private void UpdateMenu()
        {
            int itemcnt = SelectedItemPaths.Count();
            _compareItem.Enabled = itemcnt == 1;
            _md5Item.Enabled = _sha1Item.Enabled = _sha256Item.Enabled = itemcnt >= 1;
        }

        void ShowHash<T>() where T : HashAlgorithm, new()
        {
            //  Builder for the output string.
            var builder = new StringBuilder(200);

            //  Go through each file.
            foreach (var filePath in SelectedItemPaths)
            {
                //HashAlgorithm->MD5->MD5CryptoServiceProvider
                builder.AppendLine(string.Format("{0} is the {1} hash for '{2}'", CalcHash<T>(filePath), typeof(T).BaseType.Name, Path.GetFileName(filePath)));
            }

            //  Show the ouput.
            if (CurrentInvokeCommandInfo.WindowHandle == IntPtr.Zero)
                MessageBox.Show(builder.ToString());
            else
            {
                NativeWindow nw = new NativeWindow();
                try
                {

                    nw.AssignHandle(CurrentInvokeCommandInfo.WindowHandle);
                    MessageBox.Show(nw, builder.ToString());
                }
                catch
                {
                }
                finally
                {
                    nw.ReleaseHandle();
                }

            }

            builder.Clear();
        }

        public static string CalcHash<T>(string filename) where T : HashAlgorithm, new()
        {
            //HashAlgorithm->MD5->MD5CryptoServiceProvider
            using (HashAlgorithm csp = new T())//same as MD5.Create()
            {
                try
                {
                    using (var stream = File.OpenRead(filename))
                    {
                        string strHash = BitConverter.ToString(csp.ComputeHash(stream)).Replace("-", "").ToLower();
                        return strHash;
                       // return string.Format("{0} is {1} hash for '{2}'", strHash, csp.GetType().BaseType.Name, Path.GetFileName(filename));
                    }
                }
                catch (Exception ex)
                {

                    return string.Format("Can't calculate {0} for {1} because {2}", csp.GetType().BaseType.Name, filename, ex.Message);
                }
            }
        }

        public static bool FilesContentsAreEqual(FileInfo fileInfo1, FileInfo fileInfo2)
        {
            bool result;

            if (fileInfo1.Length != fileInfo2.Length)
            {
                result = false;
            }
            else
            {
                using (var file1 = fileInfo1.OpenRead())
                {
                    using (var file2 = fileInfo2.OpenRead())
                    {
                        result = StreamsContentsAreEqual(file1, file2);
                    }
                }
            }

            return result;
        }

        private static bool StreamsContentsAreEqual(Stream stream1, Stream stream2)
        {
            const int bufferSize = 2048 * 2;
            var buffer1 = new byte[bufferSize];
            var buffer2 = new byte[bufferSize];

            while (true)
            {
                int count1 = stream1.Read(buffer1, 0, bufferSize);
                int count2 = stream2.Read(buffer2, 0, bufferSize);

                if (count1 != count2)
                {
                    return false;
                }

                if (count1 == 0)
                {
                    return true;
                }

                int iterations = (int)Math.Ceiling((double)count1 / sizeof(Int64));
                for (int i = 0; i < iterations; i++)
                {
                    if (BitConverter.ToInt64(buffer1, i * sizeof(Int64)) != BitConverter.ToInt64(buffer2, i * sizeof(Int64)))
                    {
                        return false;
                    }
                }
            }
        }
        /*
        protected void DoToggleReadOnly()
        {
            //  Get the attributes.
            var path = SelectedItemPaths.First();
            var attributes = File.GetAttributes(path);

            //  Toggle the readonly flag.
            if ((attributes & FileAttributes.ReadOnly) != 0)
                attributes &= ~FileAttributes.ReadOnly;
            else
                attributes |= FileAttributes.ReadOnly;

            //  Set the attributes.
            File.SetAttributes(path, attributes);
        }

        protected void DoCopyPath()
        {
            Clipboard.SetText(SelectedItemPaths.First());
        }

        protected void DoTouch()
        {
            File.SetLastAccessTime(SelectedItemPaths.First(), DateTime.Now);
        }
        /// <summary>
        /// Counts the lines in the selected files.
        /// </summary>
        private void CountLines()
        {
            //  Builder for the output.
            var builder = new StringBuilder();

            //  Go through each file.
            foreach (var filePath in SelectedItemPaths)
            {
                //  Count the lines.
                builder.AppendLine(string.Format("{0} - {1} Lines", Path.GetFileName(filePath), File.ReadAllLines(filePath).Length));
            }

            //  Show the ouput.
            MessageBox.Show(builder.ToString());
        }
        */
    }
}
