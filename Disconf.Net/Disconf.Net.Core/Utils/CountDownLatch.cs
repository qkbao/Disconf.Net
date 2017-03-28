using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Disconf.Net.Core.Utils
{
    /// <summary>
    /// 仿Java的CountDownLatch
    /// </summary>
    public class CountDownLatch
    {
        int _count;

        public CountDownLatch(int count)
        {
            this._count = count;
        }
        public void CountDown()
        {
            if (this._count > 0)
            {
                Interlocked.Decrement(ref _count);
            }
        }
        public void Await(long millisecond = 0)
        {
            var intervalMs = 10;
            if (millisecond <= 0)
            {
                millisecond = long.MaxValue;
            }
            var count = Math.Ceiling(millisecond * 1.0 / intervalMs);
            for (var i = 0; i < count; i++)
            {
                if (this._count <= 0)
                {
                    return;
                }
                Thread.Sleep(intervalMs);
            }
            throw new TimeoutException("CountDownLatch is timeout");
        }
    }
}
