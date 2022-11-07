using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace DailyStatementEmas
{
    public class Model
    {
        public class app_VendorOutlet
        {
            [Key]
            public string OutletCode { get; set; }
            public string OutletCounterPartCode { get; set; }
            public string OutletName { get; set; }
            public int OutletState { get; set; }
           
        }
    }
}
