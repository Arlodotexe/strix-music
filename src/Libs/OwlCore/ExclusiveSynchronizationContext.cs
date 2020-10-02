using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace OwlCore
{
    internal class ExclusiveSynchronizationContext : SynchronizationContext
    {
        private bool done;

        readonly AutoResetEvent workItemsWaiting = new AutoResetEvent(false);

        readonly Queue<Tuple<SendOrPostCallback, object>> items =
            new Queue<Tuple<SendOrPostCallback, object>>();

        public Exception? InnerException { get; set; }

        public override void Send(SendOrPostCallback d, object state)
        {
            throw new NotSupportedException("We cannot send to our same thread");
        }

        public override void Post(SendOrPostCallback d, object? state)
        {
            lock (items)
            {
#pragma warning disable CS8620 // Argument cannot be used for parameter due to differences in the nullability of reference types.
                items.Enqueue(Tuple.Create(d, state));
#pragma warning restore CS8620 // Argument cannot be used for parameter due to differences in the nullability of reference types.
            }
            workItemsWaiting.Set();
        }

        public void EndMessageLoop()
        {
            Post(_ => done = true, null);
        }

        public void BeginMessageLoop()
        {
            while (!done)
            {
                Tuple<SendOrPostCallback, object>? task = null;
                lock (items)
                {
                    if (items.Count > 0)
                    {
                        task = items.Dequeue();
                    }
                }
                if (task != null)
                {
                    task.Item1(task.Item2);
                    if (InnerException != null) // the method threw an exeption
                    {
                        throw new AggregateException("AsyncHelpers.Run method threw an exception.", InnerException);
                    }
                }
                else
                {
                    workItemsWaiting.WaitOne();
                }
            }
        }

        public override SynchronizationContext CreateCopy()
        {
            return this;
        }
    }
}
