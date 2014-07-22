namespace TreeSharp
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    ///   Implements a 'wait' composite. This composite will return Running until some condition is met, or it has
    ///   exceeded its alloted wait time.
    /// </summary>
    /// <remarks>
    ///   Created 1/13/2011.
    /// </remarks>
    public class Wait : Decorator
    {
        private DateTime _end;

        /// <summary>
        ///   Creates a new Wait decorator using the specified timeout, run delegate, and child composite.
        /// </summary>
        /// <param name = "timeoutSeconds"></param>
        /// <param name = "runFunc"></param>
        /// <param name = "child"></param>
        public Wait(int timeoutSeconds, CanRunDecoratorDelegate runFunc, Composite child)
            : base(runFunc, child)
        {
            Timeout =  TimeSpan.FromSeconds(timeoutSeconds);
        }

        /// <summary>
        ///   Creates a new Wait decorator with an 'infinite' timeout, the specified run delegate, and a child composite.
        /// </summary>
        /// <param name = "runFunc"></param>
        /// <param name = "child"></param>
        public Wait(CanRunDecoratorDelegate runFunc, Composite child) : this(int.MaxValue, runFunc, child)
        {
        }

        /// <summary>
        ///   Creates a new Wait decorator with the specified timeout, and child composite.
        /// </summary>
        /// <param name = "timeoutSeconds"></param>
        /// <param name = "child"></param>
        public Wait(int timeoutSeconds, Composite child)
            : base(child)
        {
            Timeout = TimeSpan.FromSeconds(timeoutSeconds);
        }

        /// <summary>
        ///   The current timeout for this Wait, as expressed in total seconds.
        /// </summary>
        public TimeSpan Timeout { get; set; }

        public override void Start(object context)
        {
            _end = DateTime.Now + Timeout;
            base.Start(context);
        }

        public override void Stop(object context)
        {
            _end = DateTime.MinValue;
            base.Stop(context);
        }

        public override IEnumerable<RunStatus> Execute(object context)
        {
            while (DateTime.Now < _end)
            {
                if (Runner != null)
                {
                    if (Runner(context))
                    {
                        break;
                    }
                }
                else
                {
                    if (CanRun(context))
                    {
                        break;
                    }
                }

                yield return RunStatus.Running;
            }

            if (DateTime.Now < _end)
            {
                yield return RunStatus.Failure;
                yield break;
            }

            DecoratedChild.Start(context);
            while (DecoratedChild.Tick(context) == RunStatus.Running)
            {
                yield return RunStatus.Running;
            }

            DecoratedChild.Stop(context);
            if (DecoratedChild.LastStatus == RunStatus.Failure)
            {
                yield return RunStatus.Failure;
                yield break;
            }

            yield return RunStatus.Success;
            yield break;
        }
    }
}