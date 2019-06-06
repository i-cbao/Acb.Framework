using System;

namespace Acb.FileServer.ViewModels
{
    public class VPolicyPdfInput
    {
        public string OderId { get; set; }
        public string PolicyNumber { get; set; }
        public string Batch { get; set; }
        public string Name { get; set; }
        public string CardName { get; set; }
        public string CardNumber { get; set; }
        public string PlateNumber { get; set; }
        /// <summary> 厂牌型号 </summary>
        public string FactoryVersion { get; set; }
        /// <summary> 登记日期 </summary>
        public DateTime RegisterTime { get; set; }
        public string EngineNumber { get; set; }
        public string VinNumber { get; set; }
        public DateTime ServerStartTime { get; set; }
        public DateTime ServerEndTime { get; set; }
        public decimal BuyAmount { get; set; }
        public decimal InsuredAmount { get; set; }
        public decimal PaidAmount { get; set; }
        public string Text1 { get; set; }
        public string Text2 { get; set; }
        public string Text3 { get; set; }
        public DateTime CreateTime { get; set; }
        public int Year { get; set; }

        public bool IsElectronPolicy { get; set; }
        public string ElectronPolicySupplierId { get; set; }

        public string FinanceId { get; set; }
    }
}
