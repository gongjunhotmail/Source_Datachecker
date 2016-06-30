using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

using BWNSDataset;
using BWNSProtocol;
using BWNSUtility;
using BWNSLWDData;

using NSXYPlot;
using NSTreeViewEx;

namespace DataChecker
{
    public partial class Form1 : Form
    {
        UniqID Uid;
        TreeViewEx mytreeview1;
        List<DataPlot> DataPlots;
        int PlotBase_Top, PlotBase_Left, PlotBase_Height, PlotBase_Width;
        int PlotInterval = 30;    // the interval between 2 plots
        public Form1()
        {
            InitializeComponent();
            Uid = new UniqID();

            mytreeview1 = new TreeViewEx();
            mytreeview1.Location = treeView1.Location;
            mytreeview1.Size = treeView1.Size;
            this.Controls.Add(mytreeview1);
            treeView1.Visible = false;     //hide the original treeview1
            mytreeview1.Show();            //use mytreeview1 to replace original treeview1 to keep user interface
            mytreeview1.CheckBoxes = true;

            PlotBase_Top  = 0;
            PlotBase_Left  = 0;
            PlotBase_Height  = xyPlotCtrl0.Height ;
            PlotBase_Width  = xyPlotCtrl0.Width ;
            xyPlotCtrl0.Visible = false;
            PlotInterval = 10;

            DataPlots = new List<DataPlot>();

        }

        private void button1_Click(object sender, EventArgs e)
        {
            DialogResult result = openFileDialog1.ShowDialog();
            if (result == DialogResult.OK) // Test result.
            {
                Console.WriteLine (openFileDialog1.FileName);
            }
            AppendDatadump(mytreeview1, openFileDialog1.FileName, openFileDialog1.SafeFileName);
            //FillPrototypeTreeview(treeView1, openFileDialog1.SafeFileName);
            //dataPacketList = BWProcessMemoryData.DataPacketList;

            //foreach (BWDataset dataset in datasetList)
            //{
            //    xyPlotCtrl1.XYPlot1.AddDataset(dataset);
            //    xyPlotCtrl1.XYPlot1.FullView();
            //    //xyPlotViewCtrl1.XYPlots[0].AddDataset(dataset);
            //    //xyPlotViewCtrl1.XYPlots[1].AddDataset(dataset);
                
            //}
            //xyPlotViewCtrl1.XYPlots[0]=xyPlotCtrl1.XYPlot1;
            //xyPlotViewCtrl1.FullView();
            //MyTreeView myTreeview = new MyTreeView(treeView1,treeViewSignalContextMenu1);
            //myTreeview.CreateTreeViewSignalContextMenuStrip();
            
        }
        private void AppendDatadump(TreeView treeView1, String DatadumpFileName,String TreeViewNodeName)
        {                        

            byte[] dumpedData = loadDumpedDataOneRun(DatadumpFileName);
            if (dumpedData == null || dumpedData.Length == 0)
            {
                MessageBox.Show("Error: Data file error!");
                return;
            }

            List<BWDataset> datasetList;
            datasetList = BWProcessMemoryData.ProcessDumpedData(dumpedData);
            //dataPacketList = BWProcessMemoryData.DataPacketList;
            //AppendDatapacketListToTreeView(dataPacketList, treeView1, "Haaallaa");
            
            TreeNode node = treeView1.Nodes.Add(TreeViewNodeName);
            foreach (BWDataset dataset in datasetList)
            {
                TreeNode t = new TreeNode();
                int sn = Uid.GetUniqID();
                DataNode dd = new DataNode(sn);
                dd.Dataset = dataset;
                
                t.Tag = dd;
                //var value = nd.Tag as DataNode;
                //value.GetUniqID();

                t.Text = dataset.DataType.ToString() + " : " + dataset.Name.ToString();
                t.ToolTipText = t.Tag.ToString();
                node.Nodes.Add(t);               
                
                //create a plot for each of the dataset
                DataPlot dp = new DataPlot(sn);
                dp.UniqId = sn;
                dp.Plot = new XYPlotCtrl();

                int EstimateHeight = (DataPlots.Count()+1) * PlotBase_Height + PlotBase_Top ;
                if (EstimateHeight > 32767)                    
                    break;
                this.panel1.Controls.Add(dp.Plot);
                Application.DoEvents();
                dp.Plot.Top = DataPlots.Count() * PlotBase_Height + PlotBase_Top + PlotInterval;
                dp.Plot.Height = PlotBase_Height;
                dp.Plot.Left = PlotBase_Left;
                dp.Plot.Width = PlotBase_Width;
                dp.Plot.XYPlot1.AddDataset(dataset);
                DataPlots.Add(dp);
                Application.DoEvents();
            }
        }

        private void AppendDatapacketListToTreeView(List<BWMemoryDataPacket> dataPacketList, TreeView treeView1, string nodeName)
        {
            TreeNode node = mytreeview1.Nodes.Add(nodeName);

            foreach (BWMemoryDataPacket pkt in dataPacketList)
            {
                node.Nodes.Add(pkt.Type.ToString () );
            }
        }

        private void loadRawData(String fileName)
        {
            byte[] dumpedData = loadDumpedDataOneRun(fileName);
            if (dumpedData == null || dumpedData.Length == 0) return;

            List<BWDataset> datasetList = BWProcessMemoryData.ProcessDumpedData(dumpedData);
            List<BWMemoryDataPacket> dataPacketList = BWProcessMemoryData.DataPacketList;

            //if (dataPacketList != null && dataPacketList.Count > 0)
            //{
            //    startTime = dataPacketList[0].Time;
            //    endTime = dataPacketList[dataPacketList.Count - 1].Time;
            //}
        }

        private byte[] loadDumpedDataOneRun(String fileName)
        {
            byte[] dumpedData = null;
            FileStream fs = new FileStream(fileName, FileMode.Open);
            try
            {
                dumpedData = new byte[fs.Length];
                BinaryReader br = new BinaryReader(fs);
                br.Read(dumpedData, 0, (int)fs.Length);
                br.Close();
                fs.Close();
            }
            catch (Exception ex)
            {
                String debugMsg = "Info: BWMemoryDumpDlg::loadDumpedDataOneRun(): " + "Fail to read dumped data!" + " " + ex.Message;
                BWUtility.DebugLog(debugMsg);
                if (fs != null) fs.Close();
            }

            return dumpedData;
        }

        private int CalcTopPosition(int PlotNumber)
        {
            //it calculates the vertical distance between Top positions of nth plot to the 0th plot in the DataPlots .

            if (PlotNumber <= 0) return 0;
            int n;
            int TopPosition=0;

            if (PlotNumber == 0) return 0;

            for (n = 1; n < DataPlots.Count; n++)
            {
                TopPosition = TopPosition + DataPlots[n].Plot.Height + PlotInterval;
            }
            return TopPosition;
        }
        private void JumpToPlot(int PlotNumber)
        {
            if (PlotNumber > DataPlots.Count-1)
                return;
            panel1.VerticalScroll.Value = 0;
            panel1.VerticalScroll.Value = CalcTopPosition(PlotNumber);
        }
        private void btnTest_Click(object sender, EventArgs e)
        {

            if(DataPlots.Count > 3){
                //panel1.ScrollControlIntoView(DataPlots[DataPlots.Count -1].Plot);
                //JumpToPlot(2);
                //Application.DoEvents();
            }
            
        }
        private void btnShowChecked_Click(object sender, EventArgs e)
        {
            // Disable redrawing of treeView1 to prevent flickering 
            // while changes are made.
            mytreeview1.BeginUpdate();

            // Collapse all nodes of mytreeview1.
            mytreeview1.CollapseAll();

            // Add the checkForCheckedChildren event handler to the BeforeExpand event.
            //mytreeview1.BeforeExpand += checkForCheckedChildren;

            // Expand all nodes of mytreeview1. Nodes without checked children are 
            // prevented from expanding by the checkForCheckedChildren event handler.
            mytreeview1.ExpandAll();

            // Remove the checkForCheckedChildren event handler from the BeforeExpand 
            // event so manual node expansion will work correctly.
            //mytreeview1.BeforeExpand -= checkForCheckedChildren;

            // Enable redrawing of mytreeview1.
            mytreeview1.EndUpdate();
        }
        private void btnDeleteFile_Click(object sender, EventArgs e)
        {

        }

        private void treeView1_AfterCheck(object sender, TreeViewEventArgs e)
        {
            //CheckTreeViewNode(e.Node, e.Node.Checked);
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void fileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DataCheckerConfigForm dataCheckerConfigForm = new DataCheckerConfigForm();
            dataCheckerConfigForm.Show();
        }

        private void treeView1_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {

        }

        private void treeView1_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            checkSignalForm csf = new checkSignalForm();
            csf.Show();
            
            // or Select the clicked node
            mytreeview1.SelectedNode = mytreeview1.GetNodeAt(e.X, e.Y);

            if (mytreeview1.SelectedNode != null)
            {
                //myContextMenuStrip.Show(treeView1, e.Location);
            }

        }

        private void menuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

    }



    public class BWLWDTools
    {
        public static BWResistivityTool ResTool = new BWResistivityTool();
        public static BWGammaTool GaTool = new BWGammaTool();
        public static BWMwdTool MwdTool = new BWMwdTool();
        public static BWNearBitTool NearBitTool = new BWNearBitTool();
        public static BWDirectionalResistivityTool DirResTool = new BWDirectionalResistivityTool();
        public static BWPwdTool PwdTool = new BWPwdTool();
        public static BWNuclearTool NuclearTool = new BWNuclearTool();
    }

    public struct BWMemoryDataPacket
    {
        // 1  2  3  4  5  6  7  8  9  10 11 12
        //       time        d  s  len   o  pr data   crc
        // 5A 5A XX XX XX XX XX XX XX XX XX XX ...... XX XX
        public byte[] DataBytes;

        public bool IsValid;
        public String Time;
        public BWEnumToolAddress Type;
        public BWEnumBoardAddress SubType;
        public double[] RawData;
        public double[] CalcData;
        public String ErrMsg;

        public BWMemoryDataPacket(byte[] data)
        {
            if (data == null || data.Length == 0) DataBytes = null;
            else
            {
                DataBytes = new byte[data.Length];
                Array.Copy(data, DataBytes, data.Length);
            }

            BytesToData(DataBytes, out IsValid, out Time, out Type, out SubType, out RawData, out CalcData, out ErrMsg);
        }

        public BWMemoryDataPacket(byte[] data, int start, int length)
        {
            if (data == null || data.Length == 0 ||
                start < 0 || length <= 0 || start + length >= data.Length) DataBytes = null;
            else
            {
                DataBytes = new byte[length];
                Array.Copy(data, start, DataBytes, 0, length);
            }

            BytesToData(DataBytes, out IsValid, out Time, out Type, out SubType, out RawData, out CalcData, out ErrMsg);
        }

        private static void BytesToData(byte[] dataBytes, out bool isValid, out String time, out BWEnumToolAddress type, 
            out BWEnumBoardAddress subType, out double[] rawData, out double[] calcData, out String errMsg)
        {
            isValid = false;
            time = String.Empty;
            type = BWEnumToolAddress.All;
            subType = BWEnumBoardAddress.AllBoards;
            rawData = null;
            calcData = null;
            errMsg = String.Empty;

            if (dataBytes == null || dataBytes.Length == 0) return;
            if (dataBytes.Length < 6 + 8) return;

            //       1  2  3  4  5  6  7  8  9  10
            // 5A 5A XX XX XX XX XX XX XX XX XX XX data XX XX
            //       time        d  s  len   o  pr      crc
            // check src tool address
            byte toolAddr = dataBytes[7];   // 0x5A 0x5A + 4 byte time + 1 byte dst addr (memory 1 or 2)
            byte len = dataBytes[8];        // 0x34, 0x48, 0x0C
            byte obj = dataBytes[10];       // 0x87
            int dataLength = 0;
            if (toolAddr == BWToolBoardAddress.GetToolBoardAddress(BWEnumToolAddress.MWD, BWEnumBoardAddress.MWD))
            {
                if (len != (BWLWDTools.MwdTool.NumOfBytesInMem + 8) || obj != 0x87)
                {
                    isValid = false;
                    errMsg = "Length doesn't match! (Mwd, 0x34)";
                    return;
                }
                dataLength = BWLWDTools.MwdTool.NumOfBytesInMem + 12;
                type = BWEnumToolAddress.MWD;
            }
            else if (toolAddr == BWToolBoardAddress.GetToolBoardAddress(BWEnumToolAddress.Resisitivity, BWEnumBoardAddress.DataProcess))
            {
                if (len != (BWLWDTools.ResTool.NumOfBytesInMem + 8) || obj != 0x87)
                {
                    isValid = false;
                    errMsg = "Length doesn't match! (Res, 0x48)";
                    return;
                }
                dataLength = BWLWDTools.ResTool.NumOfBytesInMem + 12;
                type = BWEnumToolAddress.Resisitivity;
            }
            else if (toolAddr == BWToolBoardAddress.GetToolBoardAddress(BWEnumToolAddress.Gamma, BWEnumBoardAddress.Gamma))
            {
                if (len != (BWLWDTools.GaTool.NumOfBytesInMem + 8) || obj != 0x87)
                {
                    isValid = false;
                    errMsg = "Length doesn't match! (Gam, 0x0C)";
                    return;
                }
                dataLength = BWLWDTools.GaTool.NumOfBytesInMem + 12;
                type = BWEnumToolAddress.Gamma;
            }
            else if (toolAddr == BWToolBoardAddress.GetToolBoardAddress(BWEnumToolAddress.BWNB, BWEnumBoardAddress.NBBatteryControl))
            {
                if (len != (BWLWDTools.NearBitTool.Status.NumOfBytesInMem + 8) || obj != 0x87)
                {
                    isValid = false;
                    errMsg = "Length doesn't match! (NBBC, 0x2B)";
                    return;
                }
                dataLength = BWLWDTools.NearBitTool.Status.NumOfBytesInMem + 12;
                type = BWEnumToolAddress.BWNB;
                subType = BWEnumBoardAddress.NBBatteryControl;
            }
            else if (toolAddr == BWToolBoardAddress.GetToolBoardAddress(BWEnumToolAddress.BWNB, BWEnumBoardAddress.DataProcess))
            {
                if (len != (BWLWDTools.NearBitTool.ResData.NumOfBytesInMem + 8) || obj != 0x87)
                {
                    isValid = false;
                    errMsg = "Length doesn't match! (NBRes, 0x22)";
                    return;
                }
                dataLength = BWLWDTools.NearBitTool.ResData.NumOfBytesInMem + 12;
                type = BWEnumToolAddress.BWNB;
                subType = BWEnumBoardAddress.DataProcess;
            }
            else if (toolAddr == BWToolBoardAddress.GetToolBoardAddress(BWEnumToolAddress.BWNB, BWEnumBoardAddress.NBAzimuthalGamma))
            {
                if (len != (BWLWDTools.NearBitTool.GaData.NumOfBytesInMem + 8) || obj != 0x87)
                {
                    isValid = false;
                    errMsg = "Length doesn't match! (NBGam, 0x1C)";
                    return;
                }
                dataLength = BWLWDTools.NearBitTool.GaData.NumOfBytesInMem + 12;
                type = BWEnumToolAddress.BWNB;
                subType = BWEnumBoardAddress.NBAzimuthalGamma;
            }
            else if (toolAddr == BWToolBoardAddress.GetToolBoardAddress(BWEnumToolAddress.BWNB, BWEnumBoardAddress.NBShorthopRX))
            {
                if (len != (BWLWDTools.NearBitTool.ShortHopData.NumOfBytesInMem + 8) || obj != 0x87)
                {
                    isValid = false;
                    errMsg = "Length doesn't match! (NBShortHop, 0x60)";
                    return;
                }
                dataLength = BWLWDTools.NearBitTool.ShortHopData.NumOfBytesInMem + 12;
                type = BWEnumToolAddress.BWNB;
                subType = BWEnumBoardAddress.NBShorthopRX;
            }
            else if (toolAddr == BWToolBoardAddress.GetToolBoardAddress(BWEnumToolAddress.BWPV, BWEnumBoardAddress.PressVib))
            {
                if (len != (BWNSLWDData.BWPwdData.NumOfBytesInMem + 8) || obj != 0x87)
                {
                    isValid = false;
                    errMsg = "Length doesn't match! (Pwd, 0x20)";
                    return;
                }
                dataLength = BWNSLWDData.BWPwdData.NumOfBytesInMem + 12;
                type = BWEnumToolAddress.BWPV;
            }
            else
            {   // tool address incorrect
                isValid = false;
                errMsg = "Source address incorrect! (Mwd 0x13, Res 0x19, Gam 0x21)";
                return;
            }

            if (dataLength > dataBytes.Length - 2)
            {   // reaches end of data
                isValid = false;
                errMsg = "Data length incorrect! (Mwd 58, Res 78, Gam 18)";
                return;
            }

            int seconds = BWConverter.BytesToInt(dataBytes, 2);
            time = BWConverter.SecondsToLongTime(seconds, false, true);
            byte[] dataPacketBytes = new byte[dataLength - 4];
            Array.Copy(dataBytes, 6, dataPacketBytes, 0, dataLength - 4);
            BWDataPacket dataPacket = new BWDataPacket(dataPacketBytes);

            if (dataPacket.IsValid() == true)
            {
                isValid = true;

                if (type == BWEnumToolAddress.MWD)
                {
                    // 44 bytes to 11 floating numbers
                    float[] data11Floating = BWLoggingObject.GetMwdData(dataPacket.Command.Data);
                    rawData = new double[11];
                    for (int i = 0; i < data11Floating.Length; i++) rawData[i] = (double)data11Floating[i];
                    calcData = new BWMwdTool().ProcessMemoryData(seconds, dataPacket.Command.Data, dataPacket.Command.Header.Return);
                }
                else if (type == BWEnumToolAddress.Resisitivity)
                {
                    rawData = BWLWDTools.ResTool.ProcessMemoryRawData(dataPacket.Command.Data);
                    calcData = BWLWDTools.ResTool.ProcessMemoryData(seconds, dataPacket.Command.Data, dataPacket.Command.Header.Return);
                }
                else if (type == BWEnumToolAddress.Gamma)
                {
                    rawData = BWLWDTools.GaTool.ProcessMemoryData(seconds, dataPacket.Command.Data, dataPacket.Command.Header.Return);
                    if (rawData != null && rawData.Length > 0)
                    {
                        calcData = new double[rawData.Length];
                        Array.Copy(rawData, calcData, rawData.Length);
                    }
                }
                else if (type == BWEnumToolAddress.BWNB)
                {
                    rawData = BWLWDTools.NearBitTool.ProcessMemoryData(subType, seconds, dataPacket.Command.Data, dataPacket.Command.Header.Return);
                    if (rawData != null && rawData.Length > 0)
                    {
                        calcData = new double[rawData.Length];
                        Array.Copy(rawData, calcData, rawData.Length);
                    }
                }
                else if (type == BWEnumToolAddress.BWPV)
                {
                    rawData = BWLWDTools.PwdTool.ProcessMemoryData(seconds, dataPacket.Command.Data, dataPacket.Command.Header.Return);
                    if (rawData != null && rawData.Length > 0)
                    {
                        calcData = new double[rawData.Length];
                        Array.Copy(rawData, calcData, rawData.Length);
                    }
                }
            }
        }
    }

    public static class BWProcessMemoryData
    {
        public static bool ShowMessageBox = true;

        public static List<BWMemoryDataPacket> DataPacketList;

        /// <summary>
        /// The input should be dumped from a single run memory data.
        /// It will create and append the memory data in a datasetList,
        /// which includes 8 resistivity, 7 mwd and 1 gamma curves.
        /// 
        /// ===============================================================
        ///                 DataLength  ActualLength    #ofRawData  #ofData
        /// Mwd             56          44              11          7
        /// Res             76          64              32          8
        /// Gamma           16          4               1           1
        /// NearBit
        ///     Status      47          35              16          16
        ///     Res         38          26              11          3
        ///     DirGA       32          20              10          3
        /// NearBit RX      100         88              22          22
        /// Pwd             36          24              6           6
        /// =================================================================
        /// 
        /// </summary>
        /// <param name="dumpedData"></param>
        /// <returns>all the curves in a single run</returns>
        public static List<BWDataset> ProcessDumpedData(byte[] dumpedData)
        {
            if (dumpedData == null || dumpedData.Length == 0) return null;

            List<BWDataset> datasetList = new List<BWDataset>();
            DataPacketList = new List<BWMemoryDataPacket>();
            int curPos = 0;
            while (curPos < dumpedData.Length - 2)
            {
                int startPos = findSeparators(dumpedData, curPos);
                startPos += 2;

                if (startPos + 10 >= dumpedData.Length)
                {   // reaches end of data
                    break;
                }

                //       1  2  3  4  5  6  7  8  9  10
                // 5A 5A XX XX XX XX XX XX XX XX XX XX data XX XX
                //       time        d  s  len   o  pr      crc
                // check src tool address
                byte toolAddr = dumpedData[startPos + 5];   // 4 byte time + 1 byte dst addr (memory 1 or 2)
                byte len = dumpedData[startPos + 6];        // 0x34, 0x48, 0x0C
                byte obj = dumpedData[startPos + 8];        // 0x87
                int dataLength = 0;
                if (toolAddr == BWToolBoardAddress.GetToolBoardAddress(BWEnumToolAddress.MWD, BWEnumBoardAddress.MWD))
                {
                    if (len != (BWLWDTools.MwdTool.NumOfBytesInMem + 8) || obj != 0x87)
                    {
                        curPos++;
                        continue;
                    }
                    dataLength = BWLWDTools.MwdTool.NumOfBytesInMem + 12;
                }
                else if (toolAddr == BWToolBoardAddress.GetToolBoardAddress(BWEnumToolAddress.Resisitivity, BWEnumBoardAddress.DataProcess))
                {
                    if (len != (BWLWDTools.ResTool.NumOfBytesInMem + 8) || obj != 0x87)
                    {
                        curPos++;
                        continue;
                    }
                    dataLength = BWLWDTools.ResTool.NumOfBytesInMem + 12;
                }
                else if (toolAddr == BWToolBoardAddress.GetToolBoardAddress(BWEnumToolAddress.Gamma, BWEnumBoardAddress.Gamma))
                {
                    if (len != (BWLWDTools.GaTool.NumOfBytesInMem + 8) || obj != 0x87)
                    {
                        curPos++;
                        continue;
                    }
                    dataLength = BWLWDTools.GaTool.NumOfBytesInMem + 12;
                }
                else if (toolAddr == BWToolBoardAddress.GetToolBoardAddress(BWEnumToolAddress.BWNB, BWEnumBoardAddress.NBBatteryControl))
                {
                    if (len != (BWLWDTools.NearBitTool.Status.NumOfBytesInMem + 8) || obj != 0x87)
                    {
                        curPos++;
                        continue;
                    }
                    dataLength = BWLWDTools.NearBitTool.Status.NumOfBytesInMem + 12;
                }
                else if (toolAddr == BWToolBoardAddress.GetToolBoardAddress(BWEnumToolAddress.BWNB, BWEnumBoardAddress.DataProcess))
                {
                    if (len != (BWLWDTools.NearBitTool.ResData.NumOfBytesInMem + 8) || obj != 0x87)
                    {
                        curPos++;
                        continue;
                    }
                    dataLength = BWLWDTools.NearBitTool.ResData.NumOfBytesInMem + 12;
                }
                else if (toolAddr == BWToolBoardAddress.GetToolBoardAddress(BWEnumToolAddress.BWNB, BWEnumBoardAddress.NBAzimuthalGamma))
                {
                    if (len != (BWLWDTools.NearBitTool.GaData.NumOfBytesInMem + 8) || obj != 0x87)
                    {
                        curPos++;
                        continue;
                    }
                    dataLength = BWLWDTools.NearBitTool.GaData.NumOfBytesInMem + 12;
                }
                else if (toolAddr == BWToolBoardAddress.GetToolBoardAddress(BWEnumToolAddress.BWNB, BWEnumBoardAddress.NBShorthopRX))
                {
                    if (len != (BWLWDTools.NearBitTool.ShortHopData.NumOfBytesInMem + 8) || obj != 0x87)
                    {
                        curPos++;
                        continue;
                    }
                    dataLength = BWLWDTools.NearBitTool.ShortHopData.NumOfBytesInMem + 12;
                }
                else if (toolAddr == BWToolBoardAddress.GetToolBoardAddress(BWEnumToolAddress.BWPV, BWEnumBoardAddress.PressVib))
                {
                    if (len != (BWPwdData.NumOfBytesInMem + 8) || obj != 0x87)
                    {
                        curPos++;
                        continue;
                    }
                    dataLength = BWPwdData.NumOfBytesInMem + 12;
                }
                else
                {   // tool address incorrect, move curPos, continue
                    curPos++;
                    continue;
                }

                if (startPos + dataLength >= dumpedData.Length)
                {   // reaches end of data
                    break;
                }
                int endPos = startPos + dataLength;

                byte[] temp = new byte[endPos - startPos];
                Array.Copy(dumpedData, startPos, temp, 0, endPos - startPos);

                int time = BWConverter.BytesToInt(temp, 0);
                byte[] dataPacketBytes = new byte[temp.Length - 4];
                Array.Copy(temp, 4, dataPacketBytes, 0, temp.Length - 4);

                processSingleDataPacket(dataPacketBytes, time, datasetList);
                curPos = endPos;

                //String msg = String.Format("curPos = {0}, {1}, {2}, {3}",
                //    curPos, datasetList[0].TotalRecords, datasetList[1].TotalRecords, datasetList[10].TotalRecords);
                //System.Diagnostics.Debug.WriteLine(msg);

                if (startPos - 2 >= 0)
                {   // create raw data list
                    //byte[] temp2 = new byte[dataLength + 2];        // 5A 5A + temp 
                    //Array.Copy(dumpedData, startPos - 2, temp2, 0, dataLength + 2);
                    BWMemoryDataPacket packet = new BWMemoryDataPacket(dumpedData, startPos - 2, dataLength + 2);
                    DataPacketList.Add(packet);
                }
            }

            //String line = BWConverter.BytesToHexString(dumpedData);
            //line = BWConverter.AddSpacesToHexString(line);
            //System.Diagnostics.Debug.WriteLine(line);

            return datasetList;
        }

        /// <summary>
        /// find the index of the separator 0x5A0x5A in a byte array, starting at startPos
        /// </summary>
        /// <param name="data"></param>
        /// <param name="startPos"></param>
        /// <returns>if none found, the returned index == startPos</returns>
        public static int findSeparators(byte[] data, int startPos)
        {
            int curPos = startPos;
            while (curPos < data.Length - 2)
            {
                if (data[curPos] != 0x5A || data[curPos + 1] != 0x5A)
                {
                    curPos++;
                    continue;
                }

                break;
            }

            return curPos;
        }

        /// <summary>
        /// It will append one packet data to corresponding dataset
        /// Resistivity and Mwd data are logged in a file, GA data is not logged
        /// </summary>
        /// <param name="dataPacketBytes">data in a single dataPacket, without time</param>
        /// <param name="time"></param>
        /// <param name="datasetList"></param>
        private static void processSingleDataPacket(byte[] dataPacketBytes, int time, List<BWDataset> datasetList)
        {
            if (dataPacketBytes == null || dataPacketBytes.Length < 8) return;
            if (datasetList == null) return;

            BWDataPacket dataPacket = new BWDataPacket(dataPacketBytes);
            String line = dataPacket.ToHexString();
            if (dataPacket.IsValid() == false)
            {
                System.Diagnostics.Debug.WriteLine("Invalid " + line);

                //String msg = MyStrings.String_Invalid_memroy_data_packet + " " + dataPacket.ToHexString();
                //if (ShowMessageBox == true)
                //{
                //    MessageBox.Show(msg, MyStrings.String_Invalid_memroy_data, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                //    ShowMessageBox = false;
                //}

                //String debugMsg = "Info: BWProcessMemoryData::processSingleDataPacket(): " + msg;
                //BWUtility.DebugLog(debugMsg); 
                
                return;
            }
            else
            {
                //System.Diagnostics.Debug.WriteLine("Valid datapacket: " + line);
            }

            byte srcAddr = dataPacket.SrcAddr;
            BWToolBoardAddress address = new BWToolBoardAddress(srcAddr);
            BWToolCommand cmd = dataPacket.Command;
            BWCommandHeader header = cmd.Header;
            int ret = header.Return;
            int len = dataPacket.PayLoad;
            byte[] data = cmd.Data;

            //System.Diagnostics.Debug.Assert((data != null) && (data.Length == len - 8));
            if (address.ToolAddress == BWEnumToolAddress.Resisitivity)
            {
                double[] comp = BWLWDTools.ResTool.ProcessMemoryData(time, data, ret);
                System.Diagnostics.Debug.Assert(comp != null && comp.Length == 8);
                if (comp == null || comp.Length != 8) return;
                appendResistivityData(time, comp, ret, datasetList);

                // raw data
                double[] rawData = BWLWDTools.ResTool.ProcessMemoryRawData(data);
                System.Diagnostics.Debug.Assert(rawData != null && rawData.Length == 32);
                if (rawData == null || rawData.Length != 32) return;
                appendRawResistivityData(time, rawData, ret, datasetList);
            }
            else if (address.ToolAddress == BWEnumToolAddress.MWD)
            {
                double[] mwdData = BWLWDTools.MwdTool.ProcessMemoryData(time, data, ret);
                System.Diagnostics.Debug.Assert(mwdData != null && mwdData.Length == 7);
                if (mwdData == null || mwdData.Length != 7) return;
                appendMwdData(time, mwdData, ret, datasetList);
            }
            else if (address.ToolAddress == BWEnumToolAddress.Gamma)
            {
                double[] gaData = BWLWDTools.GaTool.ProcessMemoryData(time, data, ret);
                System.Diagnostics.Debug.Assert(gaData != null && gaData.Length == BWLWDTools.GaTool.NumOfDataInMem);
                if (gaData == null || gaData.Length != BWLWDTools.GaTool.NumOfDataInMem) return;
                appendGammaData(time, gaData, ret, datasetList);
            }
            else if (address.ToolAddress == BWEnumToolAddress.BWNB)
            {
                double[] nbData = BWLWDTools.NearBitTool.ProcessMemoryData(address.BoardAddress, time, data, ret);
                if (address.BoardAddress == BWEnumBoardAddress.NBBatteryControl)
                {
                    System.Diagnostics.Debug.Assert(nbData != null && nbData.Length == BWLWDTools.NearBitTool.Status.NumOfDataInMem);
                    if (nbData == null || nbData.Length != BWLWDTools.NearBitTool.Status.NumOfDataInMem) return;
                    appendNBBatteryStatusData(time, nbData, ret, datasetList);
                }
                else if (address.BoardAddress == BWEnumBoardAddress.DataProcess)
                {
                    System.Diagnostics.Debug.Assert(nbData != null && nbData.Length == BWLWDTools.NearBitTool.ResData.NumOfDataInMem);
                    if (nbData == null || nbData.Length != BWLWDTools.NearBitTool.ResData.NumOfDataInMem) return;
                    appendNBResistivityData(time, nbData, ret, datasetList);
                }
                else if (address.BoardAddress == BWEnumBoardAddress.NBAzimuthalGamma)
                {
                    System.Diagnostics.Debug.Assert(nbData != null && nbData.Length == BWLWDTools.NearBitTool.GaData.NumOfDataInMem);
                    if (nbData == null || nbData.Length != BWLWDTools.NearBitTool.GaData.NumOfDataInMem) return;
                    appendNBGammaData(time, nbData, ret, datasetList);
                }
                else if (address.BoardAddress == BWEnumBoardAddress.NBShorthopRX)
                {
                    System.Diagnostics.Debug.Assert(nbData != null && nbData.Length == BWLWDTools.NearBitTool.ShortHopData.NumOfDataInMem);
                    if (nbData == null || nbData.Length != BWLWDTools.NearBitTool.ShortHopData.NumOfDataInMem) return;
                    appendNBShorthopData(time, nbData, ret, datasetList);
                }
            }
            else if (address.ToolAddress == BWEnumToolAddress.BWPV)
            {
                double[] pwdData = BWLWDTools.PwdTool.ProcessMemoryData(time, data, ret);
                System.Diagnostics.Debug.Assert(pwdData != null && pwdData.Length == BWPwdData.NumOfDataInMem);
                if (pwdData == null || pwdData.Length != BWPwdData.NumOfDataInMem)
                    return;
                appendPwdData(time, pwdData, ret, datasetList);
            }
            else
            {
                //System.Diagnostics.Debug.WriteLine("Invalid dataPacket: " + line);
            }
        }

        private static void appendResistivityData(int time, double[] data8Res, int errCode, List<BWDataset> datasetList)
        {
            for (int i = 0; i < 8; i++)
            {
                String name = ((BWEnumResCurves)i).ToString();
                BWDataset dataset = BWDatasetUtility.FindDatasetInList(name, datasetList);
                if (dataset == null)
                {
                    dataset = new BWDataset();
                    dataset.Name = name;
                    dataset.TotalColumns = 3;
                    dataset.DepthType = BWDepthType.typeTime;
                    dataset.DepthUnit = EnumUnit.SECONDS.ToString();
                    dataset.DataType = BWDataType.typeDataResistivity;
                    dataset.ValueUnit = "Ohmm";

                    datasetList.Add(dataset);
                }

                double[] temp = new double[] { time, data8Res[i], errCode };
                changeNaNtoNoReading(temp);
                dataset.AppendData(new BWDataPoint(temp));
            }
        }
        private static void appendRawResistivityData(int time, double[] data32Res, int errCode, List<BWDataset> datasetList)
        {
            for (int i = 0; i < 32; i++)
            {
                String name = ((BWEnumRawResCurves)i).ToString();
                BWDataset dataset = BWDatasetUtility.FindDatasetInList(name, datasetList);
                if (dataset == null)
                {
                    dataset = new BWDataset();
                    dataset.Name = name;
                    dataset.TotalColumns = 3;
                    dataset.DepthType = BWDepthType.typeTime;
                    dataset.DepthUnit = EnumUnit.SECONDS.ToString();
                    dataset.DataType = BWDataType.typeDataResistivity;
                    dataset.ValueUnit = "";

                    datasetList.Add(dataset);
                }

                double[] temp = new double[] { time, data32Res[i], errCode };
                changeNaNtoNoReading(temp);
                dataset.AppendData(new BWDataPoint(temp));
            }
        }
        private static void appendMwdData(int time, double[] data7Mwd, int errCode, List<BWDataset> datasetList)
        {
            for (int i = 0; i < 7; i++)
            {
                String name = ((BWEnumMwdCurves)i).ToString();
                BWDataset dataset = BWDatasetUtility.FindDatasetInList(name, datasetList);
                if (dataset == null)
                {
                    dataset = new BWDataset();
                    dataset.Name = name;
                    dataset.TotalColumns = 3;
                    dataset.DepthType = BWDepthType.typeTime;
                    dataset.DepthUnit = EnumUnit.SECONDS.ToString();
                    dataset.DataType = BWDataType.typeDataMwd;
                    if (name == "RPM") dataset.ValueUnit = "RPM";
                    else if (name == "TEM") dataset.ValueUnit = "C";
                    else dataset.ValueUnit = "Degree";

                    datasetList.Add(dataset);
                }
                double[] temp = new double[] { time, data7Mwd[i], errCode };
                changeNaNtoNoReading(temp);
                dataset.AppendData(new BWDataPoint(temp));
            }
        }
        private static void appendGammaData(int time, double[] data1Gamma, int errCode, List<BWDataset> datasetList)
        {
            for (int i = 0; i < 1; i++)
            {
                String name = ((BWEnumGammaCurves)i).ToString();
                BWDataset dataset = BWDatasetUtility.FindDatasetInList(name, datasetList);
                if (dataset == null)
                {
                    dataset = new BWDataset();
                    dataset.Name = name;
                    dataset.TotalColumns = 3;
                    dataset.DepthType = BWDepthType.typeTime;
                    dataset.DepthUnit = EnumUnit.SECONDS.ToString();
                    dataset.DataType = BWDataType.typeDataGamma;
                    dataset.ValueUnit = "CPS";

                    datasetList.Add(dataset);
                }
                double[] temp = new double[] { time, data1Gamma[i], errCode };
                changeNaNtoNoReading(temp);
                dataset.AppendData(new BWDataPoint(temp));
            }
        }
        private static void appendNBBatteryStatusData(int time, double[] dataNB, int errCode, List<BWDataset> datasetList)
        {
            for (int i = 0; i < 16; i++)
            {
                String name = ((BWEnumNBBatteryStatusCurves)i).ToString();
                BWDataset dataset = BWDatasetUtility.FindDatasetInList(name, datasetList);
                if (dataset == null)
                {
                    dataset = new BWDataset();
                    dataset.Name = name;
                    dataset.TotalColumns = 3;
                    dataset.DepthType = BWDepthType.typeTime;
                    dataset.DepthUnit = EnumUnit.SECONDS.ToString();
                    dataset.DataType = BWDataType.typeDataNBBatteryStatus;
                    dataset.ValueUnit = "";

                    datasetList.Add(dataset);
                }
                double[] temp = new double[] { time, dataNB[i], errCode };
                changeNaNtoNoReading(temp);
                dataset.AppendData(new BWDataPoint(temp));
            }
        }
        private static void appendNBResistivityData(int time, double[] dataNB, int errCode, List<BWDataset> datasetList)
        {
            // 8 raw + 2 comp + 1 temp
            for (int i = 0; i < 11; i++)
            {
                String name = ((BWEnumNBRawResCurves)i).ToString();
                BWDataset dataset = BWDatasetUtility.FindDatasetInList(name, datasetList);
                if (dataset == null)
                {
                    dataset = new BWDataset();
                    dataset.Name = name;
                    dataset.TotalColumns = 3;
                    dataset.DepthType = BWDepthType.typeTime;
                    dataset.DepthUnit = EnumUnit.SECONDS.ToString();
                    dataset.DataType = BWDataType.typeDataNBResistivity;
                    dataset.ValueUnit = "";

                    datasetList.Add(dataset);
                }
                double[] temp = new double[] { time, dataNB[i], errCode };
                changeNaNtoNoReading(temp);
                dataset.AppendData(new BWDataPoint(temp));
            }
        }
        private static void appendNBGammaData(int time, double[] dataNB, int errCode, List<BWDataset> datasetList)
        {
            if (dataNB == null || dataNB.Length != BWLWDTools.NearBitTool.GaData.NumOfDataInMem) return;

            // NBINC, NBRPM, NBGATotal
            for (int i = 0; i < 3; i++)
            {
                String name = ((BWEnumNBRawGammaCurves)i).ToString();
                BWDataset dataset = BWDatasetUtility.FindDatasetInList(name, datasetList);
                if (dataset == null)
                {
                    dataset = new BWDataset();
                    dataset.Name = name;
                    dataset.TotalColumns = 3;
                    dataset.DepthType = BWDepthType.typeTime;
                    dataset.DepthUnit = EnumUnit.SECONDS.ToString();
                    dataset.DataType = BWDataType.typeDataNBGamma;
                    dataset.ValueUnit = "";

                    datasetList.Add(dataset);
                }

                double[] temp = new double[] { time, dataNB[i], errCode };
                changeNaNtoNoReading(temp);
                dataset.AppendData(new BWDataPoint(temp));
            }

            // NBGA0-7 merge into INBDirGA
            String datasetName = "INBDirGA";
            BWDataset dirGaDataset = BWDatasetUtility.FindDatasetInList(datasetName, datasetList);
            if (dirGaDataset == null)
            {
                dirGaDataset = new BWDataset();
                dirGaDataset.Name = datasetName;
                dirGaDataset.TotalColumns = 10;
                dirGaDataset.DepthType = BWDepthType.typeTime;
                dirGaDataset.DepthUnit = EnumUnit.SECONDS.ToString();
                dirGaDataset.DataType = BWDataType.typeDataImageNBGamma;
                dirGaDataset.ValueUnit = "";
                dirGaDataset.ColumnNames = new string[] { "Time", "NBGA0", "NBGA1", "NBGA2", "NBGA3", "NBGA4", "NBGA5", "NBGA6", "NBGA7", "ErrCode" };

                datasetList.Add(dirGaDataset);
            }

            double[] newEntry = new double[] { time, dataNB[3], dataNB[4], dataNB[5], dataNB[6], dataNB[7], dataNB[8], dataNB[9], dataNB[10], errCode };
            changeNaNtoNoReading(newEntry);
            dirGaDataset.AppendData(new BWDataPoint(newEntry));
        }
        private static void appendNBShorthopData(int time, double[] dataNB, int errCode, List<BWDataset> datasetList)
        {
            if (dataNB == null || dataNB.Length != BWLWDTools.NearBitTool.ShortHopData.NumOfDataInMem) return;

            // battery status
            for (int i = 0; i < 9; i++)
            {
                String name = ((BWEnumNBShortHopCurves)i).ToString();
                BWDataset dataset = BWDatasetUtility.FindDatasetInList(name, datasetList);
                if (dataset == null)
                {
                    dataset = new BWDataset();
                    dataset.Name = name;
                    dataset.TotalColumns = 3;
                    dataset.DepthType = BWDepthType.typeTime;
                    dataset.DepthUnit = EnumUnit.SECONDS.ToString();
                    dataset.DataType = BWDataType.typeDataNBBatteryStatus;
                    dataset.ValueUnit = "";

                    datasetList.Add(dataset);
                }
                double[] temp = new double[] { time, dataNB[i], errCode };
                changeNaNtoNoReading(temp);
                dataset.AppendData(new BWDataPoint(temp));
            }

            // res
            for (int i = 9; i < 12; i++)
            {
                String name = ((BWEnumNBShortHopCurves)i).ToString();
                BWDataset dataset = BWDatasetUtility.FindDatasetInList(name, datasetList);
                if (dataset == null)
                {
                    dataset = new BWDataset();
                    dataset.Name = name;
                    dataset.TotalColumns = 3;
                    dataset.DepthType = BWDepthType.typeTime;
                    dataset.DepthUnit = EnumUnit.SECONDS.ToString();
                    dataset.DataType = BWDataType.typeDataNBResistivity;
                    dataset.ValueUnit = "";

                    datasetList.Add(dataset);
                }
                double[] temp = new double[] { time, dataNB[i], errCode };
                changeNaNtoNoReading(temp);
                dataset.AppendData(new BWDataPoint(temp));
            }

            // gam: NBINC, NBRPM, NBGATotal
            for (int i = 12; i < 15; i++)
            {
                String name = ((BWEnumNBShortHopCurves)i).ToString();
                BWDataset dataset = BWDatasetUtility.FindDatasetInList(name, datasetList);
                if (dataset == null)
                {
                    dataset = new BWDataset();
                    dataset.Name = name;
                    dataset.TotalColumns = 3;
                    dataset.DepthType = BWDepthType.typeTime;
                    dataset.DepthUnit = EnumUnit.SECONDS.ToString();
                    dataset.DataType = BWDataType.typeDataNBGamma;
                    dataset.ValueUnit = "";

                    datasetList.Add(dataset);
                }
                double[] temp = new double[] { time, dataNB[i], errCode };
                changeNaNtoNoReading(temp);
                dataset.AppendData(new BWDataPoint(temp));
            }

            // NBGA0-7 merge into INBDirGA
            String datasetName = "INBDirGA";
            BWDataset dirGaDataset = BWDatasetUtility.FindDatasetInList(datasetName, datasetList);
            if (dirGaDataset == null)
            {
                dirGaDataset = new BWDataset();
                dirGaDataset.Name = datasetName;
                dirGaDataset.TotalColumns = 10;
                dirGaDataset.DepthType = BWDepthType.typeTime;
                dirGaDataset.DepthUnit = EnumUnit.SECONDS.ToString();
                dirGaDataset.DataType = BWDataType.typeDataImageNBGamma;
                dirGaDataset.ValueUnit = "";
                dirGaDataset.ColumnNames = new string[] { "Time", "NBGA0", "NBGA1", "NBGA2", "NBGA3", "NBGA4", "NBGA5", "NBGA6", "NBGA7", "ErrCode" };

                datasetList.Add(dirGaDataset);
            }

            double[] newEntry = new double[] { time, dataNB[15], dataNB[16], dataNB[17], dataNB[18], dataNB[19], dataNB[20], dataNB[21], dataNB[22], errCode };
            changeNaNtoNoReading(newEntry);
            dirGaDataset.AppendData(new BWDataPoint(newEntry));
        }
        private static void appendPwdData(int time, double[] dataPwd, int errCode, List<BWDataset> datasetList)
        {
            for (int i = 0; i < 6; i++)
            {
                String name = ((BWEnumPwdCurves)i).ToString();
                BWDataset dataset = BWDatasetUtility.FindDatasetInList(name, datasetList);
                if (dataset == null)
                {
                    dataset = new BWDataset();
                    dataset.Name = name;
                    dataset.TotalColumns = 3;
                    dataset.DepthType = BWDepthType.typeTime;
                    dataset.DepthUnit = EnumUnit.SECONDS.ToString();
                    dataset.DataType = BWDataType.typeDataPwd;
                    dataset.ValueUnit = "";

                    datasetList.Add(dataset);
                }
                double[] temp = new double[] { time, dataPwd[i], errCode };
                changeNaNtoNoReading(temp);
                dataset.AppendData(new BWDataPoint(temp));
            }
        }
        private static void appenDirResData(int time, double[] dataDirRes, int errCode, List<BWDataset> datasetList)
        {

        }

        public static void changeNaNtoNoReading(double[] data)
        {
            if (data == null || data.Length == 0) return;

            for (int i = 0; i < data.Length; i++)
            {
                if (double.IsNaN(data[i]) == true || double.IsInfinity(data[i]) == true) data[i] = BWDataset.NO_READING;
            }
        }
    }

}
