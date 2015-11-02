using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PW_Lab1
{
    class ThreadManager
    {

        public int[,] getIntervalsTresholds(byte[] _tab, int _case)
        {
            int[,] ret = new int[_case, 2];
            int tresholdLength, tresholdLeft =0, tresholdRight =0;
            int counter =0;
            int caseNum =0;
            bool startCountLeftTreshold =false;

            tresholdLength = _tab.Length / _case; // length od single interval

            for(int i=0; i<_tab.Length; i++)
            {
                counter++;
                tresholdRight++;

                if(counter == tresholdLength)
                {
                    counter=0; 
                    caseNum++;
                    startCountLeftTreshold = true;

                    ret[caseNum - 1, 1 - 1] = tresholdLeft;
                    ret[caseNum - 1, 2 - 1] = (i == _tab.Length) ? tresholdRight : tresholdRight - 1;
                }

                if (startCountLeftTreshold)
                    tresholdLeft++;
            }

            return ret;

        }

    }
}
