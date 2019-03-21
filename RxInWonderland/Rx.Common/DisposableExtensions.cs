using System;

namespace Rx.Common
{
    public static class DisposableExtensions
    {
        /// <summary>
        /// Dispose that will (probably) not throw an exception.
        /// </summary>
        public static void DisposeSafe(this IDisposable resouce)
        {
            if (resouce != null)
            {
                try
                {
                    resouce.Dispose();
                }
                catch
                {
                    //nomnom
                }
            }
        }
    }
}
