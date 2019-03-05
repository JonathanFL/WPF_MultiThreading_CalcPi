using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalcPi
{
    class CalculateDigitsOfPiInput
    {
        // USE A CLASS LIKE THIS, IF YOU WANT TO PASS MORE THAN ONE ARGUMENT THROUGH EVENT - fx from btnClick to DoWork
        public int Digits { get; set; }

        public CalculateDigitsOfPiInput(int digits)
        {
            Digits = digits;
        }
    }
}
