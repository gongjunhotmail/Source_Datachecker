using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NSTreeViewEx
{
    public class TreeViewEx : TreeView
    {
        public ContextMenuStrip treeViewSignalContextMenu1;

        public TreeViewEx()
        {
            CreateTreeViewSignalContextMenuStrip();
            this.ContextMenuStrip = treeViewSignalContextMenu1;
           // FillPrototypeTreeview(this, "Test");
        }

        public TreeViewEx(ContextMenuStrip cm1)
        {            
            treeViewSignalContextMenu1 = cm1;
            this.ContextMenuStrip = treeViewSignalContextMenu1;
        }
        public TreeViewEx(TreeView tr1, ContextMenuStrip cm1)
        {            
            CopyTreeview(this, tr1);
            treeViewSignalContextMenu1 = cm1;
            this.ContextMenuStrip = treeViewSignalContextMenu1;
        }
        // Prevent expansion of a node that does not have any checked child nodes.

        // Returns a value indicating whether the specified 
        // TreeNode has checked child nodes.
        public bool HasCheckedChildNodes(TreeNode node)
        {
            if (node.Nodes.Count == 0) return false;
            foreach (TreeNode childNode in node.Nodes)
            {
                if (childNode.Checked) return true;
                // Recursively check the children of the current child node.
                if (HasCheckedChildNodes(childNode)) return true;
            }
            return false;
        }
        public void CheckAllNodes(TreeNodeCollection nodes)
        {
            foreach (TreeNode node in nodes)
            {
                node.Checked = true;
                CheckChildren(node, true);
            }
        }

        public void UncheckAllNodes(TreeNodeCollection nodes)
        {
            foreach (TreeNode node in nodes)
            {
                node.Checked = false;
                CheckChildren(node, false);
            }
        }

        private void CheckChildren(TreeNode rootNode, bool isChecked)
        {
            foreach (TreeNode node in rootNode.Nodes)
            {
                CheckChildren(node, isChecked);
                node.Checked = isChecked;
            }
        }
        private void CheckTreeViewNode(TreeNode node, Boolean isChecked)
        {
            foreach (TreeNode item in node.Nodes)
            {
                item.Checked = isChecked;

                if (item.Nodes.Count > 0)
                {
                    this.CheckTreeViewNode(item, isChecked);
                }
            }
        }
        public void CreateTreeViewSignalContextMenuStrip()
        {
            // Create the ContextMenuStrip.
            treeViewSignalContextMenu1 = new ContextMenuStrip();
            //Create some menu items.
            ToolStripMenuItem checkSignalLimit = new ToolStripMenuItem();
            checkSignalLimit.Text = "Check Signal Limit";
            ToolStripMenuItem checkSignalTiming = new ToolStripMenuItem();
            checkSignalTiming.Text = "Check signal timing";
            ToolStripMenuItem hideSignalWave = new ToolStripMenuItem();
            hideSignalWave.Text = "Hide signal wave";
            ToolStripMenuItem dispSignalWave = new ToolStripMenuItem();
            dispSignalWave.Text = "Display signal wave";

            //Add the menu items to the menu.
            treeViewSignalContextMenu1.Items.AddRange(new ToolStripMenuItem[]{checkSignalLimit, 
                checkSignalTiming, hideSignalWave, dispSignalWave});
            treeViewSignalContextMenu1.ItemClicked += new ToolStripItemClickedEventHandler(treeViewSignalContextMenu_ItemClicked);
        }
        void treeViewSignalContextMenu_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            ToolStripItem item = e.ClickedItem;
            // your code here
            MessageBox.Show(item.Text + " @ " + this.SelectedNode.FullPath + " => " + this.SelectedNode.Text + " Index=" + Convert.ToString(this.SelectedNode.Index));
        }
        private void FillPrototypeTreeview(TreeView treeView1, string nodeName)
        {
            CreateTreeViewSignalContextMenuStrip();
            TreeNode node_file = treeView1.Nodes.Add(nodeName);
            TreeNode node_type = node_file.Nodes.Add("Gamma");
            node_type.ContextMenuStrip = treeViewSignalContextMenu1;

            TreeNode node_wave = node_type.Nodes.Add("Gamma Signal");
            node_wave.ContextMenuStrip = treeViewSignalContextMenu1;
            node_wave.Nodes.Add("Err 1 @ 12:34:56");

            node_type = node_file.Nodes.Add("Resistivity");
            node_type.ContextMenuStrip = treeViewSignalContextMenu1;
            TreeNode node_wave1 = node_type.Nodes.Add("Resistivity 1");
            TreeNode node_wave2 = node_type.Nodes.Add("Resistivity 2");
            TreeNode node_wave3 = node_type.Nodes.Add("Resistivity 3");
            node_wave1.ContextMenuStrip = treeViewSignalContextMenu1;
            node_wave2.ContextMenuStrip = treeViewSignalContextMenu1;
            node_wave3.ContextMenuStrip = treeViewSignalContextMenu1;

            node_wave1.Nodes.Add("Err 1 @ 12:34:56");
            node_wave1.Nodes.Add("Err 2 @ 12:37:12");


        }
        public void CopyTreeview(TreeView treeview_src, TreeView treeview_dest)
        {
            TreeNode newTn;
            foreach (TreeNode tn in treeview_src.Nodes)
            {
                newTn = new TreeNode(tn.Text);
                newTn.Tag = tn.Tag;
                CopyChilds(newTn, tn);
                treeview_dest.Nodes.Add(newTn);
            }
        }

        private void CopyChilds(TreeNode parent, TreeNode willCopied)
        {
            TreeNode newTn;
            foreach (TreeNode tn in willCopied.Nodes)
            {
                newTn = new TreeNode(tn.Text);
                newTn.Tag = tn.Tag;
                parent.Nodes.Add(newTn);
            }
        }

    }
}
