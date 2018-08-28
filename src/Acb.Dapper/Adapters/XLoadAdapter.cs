﻿using System.Data;
using System.Data.Common;

namespace Acb.Dapper.Adapters
{
    public class XLoadAdapter : DataAdapter
    {
        public int FillFromReader(DataSet ds, IDataReader dataReader, int startRecord, int maxRecords)
        {
            return Fill(ds, "Table", dataReader, startRecord, maxRecords);
        }
    }
}
