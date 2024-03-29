﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SupplyChainSystem.Server.ResponseWrapper
{
    public class QuantityItems
    {
        public string SupplierItemId { get; set; }

        //For BPA
        public int PromisedQuantity { get; set; }
        public int MinimumQuantity { get; set; }

        //For PPA
        public int Quantity { get; set; }

        public double Price { get; set; }
        public string Unit { get; set; }
    }
}