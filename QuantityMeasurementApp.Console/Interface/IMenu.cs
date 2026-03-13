// ============================================================
// PROJECT : QuantityMeasurementApp.Console
// FILE    : Interface/IMenu.cs
//
// UC-15 : N-Tier Architecture
//
// Purpose : Contract for the Menu class.
//           Program.cs depends on this interface, not on the
//           concrete Menu class directly — follows DIP.
// ============================================================

namespace QuantityMeasurementApp.Console.Interface
{
    public interface IMenu
    {
        void Show();
    }
}