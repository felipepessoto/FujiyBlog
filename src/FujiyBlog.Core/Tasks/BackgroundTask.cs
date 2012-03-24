using System;
using System.Threading.Tasks;
using NLog;

namespace FujiyBlog.Core.Tasks
{
    public abstract class BackgroundTask
    {
        private readonly Logger logger = LogManager.GetCurrentClassLogger();

        protected virtual void OnError(Exception ex)
        {
            logger.ErrorException("Could not execute task " + GetType().Name, ex);
        }

        public void ExcuteLater()
        {
            try
            {
                Task.Factory.StartNew(Execute, TaskCreationOptions.LongRunning)
                    .ContinueWith(task => OnError(task.Exception), TaskContinuationOptions.OnlyOnFaulted);
            }
            catch (Exception e)
            {
                OnError(e);
            }
        }

        protected abstract void Execute();
    }
}
