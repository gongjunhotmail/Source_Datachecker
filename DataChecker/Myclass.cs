using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DataChecker
{

  public  class UniqID
    {
        private int SN=0;   //used as unique ID for each node or drawing

        public UniqID() { }

        public int GetUniqID()  {  return SN++;   }
    }
 
}
