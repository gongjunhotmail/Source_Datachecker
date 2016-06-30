using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using BWNSDataset;
using BWNSProtocol;
using BWNSUtility;
using BWNSLWDData;
using NSXYPlot;
using NSTreeViewEx;

namespace DataChecker
{

  public  class UniqID
  {
      private int SN=0;   //A dataset plot and its corresponding treeview node share the same ID.
      public UniqID() { }
      public int GetUniqID()  {  return SN++;   }
  }
  public class DataError
  {
      public string ErrLable;   //The lable displayed for each error      
      public double ErrTime;    //The time of error
      public int UniqId;        
  }

  public class DataNode    //used for 
  {
      public int UniqId;
      public BWDataset Dataset;
      public List<DataError> ErrList;

      public DataNode(int n) { UniqId = n; Dataset = new BWDataset(); ErrList = new List<DataError>(); }
  }

  public class DataPlot
  {
      public int UniqId;
      public NSXYPlot.XYPlotCtrl Plot;
      public Label Title;
      public DataPlot(int sn) { UniqId = sn; }
      public DataPlot(int sn, NSXYPlot.XYPlotCtrl plot) { UniqId = sn; Plot = plot; }
  }

}
