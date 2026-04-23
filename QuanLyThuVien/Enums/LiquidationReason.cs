using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLyThuVien.Enums
{
    public enum LiquidationReason
    {
        Lost,
        Damaged,
        LostByUser
    }

    public static class LiquidationReasonExtensions
    {
        public static string GetDisplayName(this LiquidationReason reason)
        {
            switch (reason)
            {
                case LiquidationReason.Lost:
                    return "Lost";
                case LiquidationReason.Damaged:
                    return "Damaged";
                case LiquidationReason.LostByUser:
                    return "LostByUser";
                default:
                    return reason.ToString();
            }
        }
    }
}
