using System;
using System.Threading;

namespace CapsCollection.Desktop.Tests
{
    /// <summary>
    /// Provides a synchronization context for unit tests.
    /// </summary>
    public sealed class TestSynchronizationContext : SynchronizationContext
    {
        /// <summary>
        /// A reference to the <c>TestSynchronizationContext</c> that has posted or sent a delegate to the current
        /// thread.
        /// </summary>
        [ThreadStatic]
        private static SynchronizationContext ThreadContext;



        /// <summary>
        /// Gets a value indicating whether the current thread is executing a delegate that was posted or sent to the 
        /// current context.
        /// </summary>
        public bool ExecutingInContext
        {
            get
            {
                return ThreadContext == this;
            }
        }

        /// <summary>
        /// <c>True</c> if posted delegates should be executed synchronously, or <c>False</c> to use the thread pool.
        /// </summary>
        private readonly bool Synchronous;



        /// <summary>
        /// Occurs when the context has finished executing a delegate.
        /// </summary>
        public event EventHandler ExecutionComplete;




        /// <summary>
        /// Initializes a new instance of the <see cref="TestSynchronizationContext"/> class.
        /// </summary>
        public TestSynchronizationContext()
            : this(false)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TestSynchronizationContext"/> class.
        /// </summary>
        /// <param name="synchronous"><c>True</c> if posted delegates should be executed synchronously, or <c>False</c>
        /// to use the thread pool.</param>
        public TestSynchronizationContext(bool synchronous)
        {
            Synchronous = synchronous;
        }




        /// <summary>
        /// Raises the <see cref="ExecutionComplete"/> event.
        /// </summary>
        private void OnExecutionComplete()
        {
            if (ExecutionComplete != null)
            {
                ExecutionComplete(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Dispatches an asynchronous message to a synchronization context.
        /// </summary>
        /// <param name="d">The <see cref="SendOrPostCallback"/> delegate to call.</param>
        /// <param name="state">The object passed to the delegate.</param>
        public override void Post(SendOrPostCallback d, object state)
        {
            if (Synchronous)
            {
                Send(d, state);
            }
            else
            {
                ThreadPool.QueueUserWorkItem(o => Send(d, o), state);
            }
        }

        /// <summary>
        /// Dispatches a synchronous message to a synchronization context.
        /// </summary>
        /// <param name="d">The <see cref="SendOrPostCallback"/> delegate to call.</param>
        /// <param name="state">The object passed to the delegate.</param>
        public override void Send(SendOrPostCallback d, object state)
        {
            var previousContext = Current;
            try
            {
                SetSynchronizationContext(this);
                ThreadContext = this;
                d(state);
            }
            finally
            {
                SetSynchronizationContext(previousContext);
                ThreadContext = previousContext is TestSynchronizationContext ? previousContext : null;
                OnExecutionComplete();
            }
        }
    }
}