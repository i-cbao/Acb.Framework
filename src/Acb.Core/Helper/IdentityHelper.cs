﻿using System;
using System.Linq;
using Acb.Core.Dependency;

namespace Acb.Core.Helper
{
    /// <summary> 主键辅助类 </summary>
    public static class IdentityHelper
    {
        /// <summary> This algorithm generates secuential GUIDs across system boundaries, ideal for databases  </summary>
        /// <returns></returns>
        public static Guid NewSequentialGuid()
        {
            var uid = Guid.NewGuid().ToByteArray();
            var binDate = BitConverter.GetBytes(DateTime.Now.Ticks);

            var secuentialGuid = new byte[uid.Length];

            secuentialGuid[0] = uid[0];
            secuentialGuid[1] = uid[1];
            secuentialGuid[2] = uid[2];
            secuentialGuid[3] = uid[3];
            secuentialGuid[4] = uid[4];
            secuentialGuid[5] = uid[5];
            secuentialGuid[6] = uid[6];
            // set the first part of the 8th byte to '1100' so     
            // later we'll be able to validate it was generated by us   

            secuentialGuid[7] = (byte)(0xc0 | (0xf & uid[7]));

            // the last 8 bytes are sequential,    
            // it minimizes index fragmentation   
            // to a degree as long as there are not a large    
            // number of Secuential-Guids generated per millisecond  

            secuentialGuid[9] = binDate[0];
            secuentialGuid[8] = binDate[1];
            secuentialGuid[15] = binDate[2];
            secuentialGuid[14] = binDate[3];
            secuentialGuid[13] = binDate[4];
            secuentialGuid[12] = binDate[5];
            secuentialGuid[11] = binDate[6];
            secuentialGuid[10] = binDate[7];

            return new Guid(secuentialGuid);
        }

        /// <summary> Guid32位 </summary>
        public static string Guid32 => NewSequentialGuid().ToString("N");

        /// <summary> Guid16位 </summary>
        public static string Guid16
        {
            get
            {
                var i = Guid.NewGuid().ToByteArray().Aggregate<byte, long>(1, (current, b) => current * (b + 1));
                return $"{i - DateTimeOffset.Now.Ticks:x}";
            }
        }

        /// <summary> 长整型ID </summary>
        public static long LongId => CurrentIocManager.Resolve<IdWorker>().NextId();
    }
}
