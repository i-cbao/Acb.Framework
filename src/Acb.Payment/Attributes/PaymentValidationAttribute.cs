using Acb.Payment.Enum;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Acb.Payment.Attributes
{
    /// <summary>
    /// 支付验证属性
    /// </summary>
    public class PaymentValidationAttribute : ValidationAttribute
    {
        private const TradeType AllTrades = (TradeType)999;
        private const ActionType AllActions = (ActionType)999;
        private readonly TradeType[] _trades = { AllTrades };
        private readonly ActionType[] _actions = { AllActions };

        #region 构造函数

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="tradeType">网关交易类型</param>
        public PaymentValidationAttribute(TradeType tradeType)
        {
            _trades = new TradeType[] { tradeType };
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="tradeTypes">网关交易类型</param>
        public PaymentValidationAttribute(params TradeType[] tradeTypes)
        {
            _trades = tradeTypes;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="actionType">网关辅助类型</param>
        public PaymentValidationAttribute(ActionType actionType)
        {
            _actions = new ActionType[] { actionType };
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="actionTypes">网关辅助类型</param>
        public PaymentValidationAttribute(params ActionType[] actionTypes)
        {
            _actions = actionTypes;
        }

        #endregion

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            validationContext.Items.TryGetValue(nameof(ActionType), out object obj);
            var actionType = (ActionType)obj;
            validationContext.Items.TryGetValue(nameof(TradeType), out obj);
            var tradeType = (TradeType)obj;

            if (actionType == 0)
            {
                if (_trades.Contains(tradeType) || _trades.Contains(AllTrades))
                {
                    if (value is null || string.IsNullOrEmpty(value.ToString()))
                    {
                        return new ValidationResult(ErrorMessage);
                    }
                }
            }
            else
            {
                if (_actions.Contains(actionType) || _actions.Contains(AllActions))
                {
                    if (value is null || string.IsNullOrEmpty(value.ToString()))
                    {
                        return new ValidationResult(ErrorMessage);
                    }
                }

            }
            return ValidationResult.Success;
        }
    }
}
