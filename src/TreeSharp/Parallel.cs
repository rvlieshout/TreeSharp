namespace TreeSharp
{
    using System.Collections.Generic;

    /// <summary>
    ///   This class is not yet implemented!
    /// </summary>
    public class Parallel : GroupComposite
    {
        public Parallel(params Composite[] children)
            : base(children)
        {
        }

        public Parallel(ContextChangeHandler contextChange, params Composite[] children)
            : this(children)
        {
            ContextChanger = contextChange;
        }

        public override IEnumerable<RunStatus> Execute(object context)
        {
            if (ContextChanger != null)
            {
                context = ContextChanger(context);
            }

            Composite markforremoval = null;

            foreach (Composite node in Children)
            {
                node.Start(context);
                node.Tick(context);
                /*
                while (node.Tick(context) == RunStatus.Running)
                {
                    Selection = node;
                 //   yield return RunStatus.Running;
                }*/

                Selection = null;
                node.Stop(context);

                if (node.LastStatus == RunStatus.Failure)
                {
                    markforremoval = node;
                    //   Children.Remove(node);
                    //yield return RunStatus.Running;
                    // yield break;
                }
            }

            if (markforremoval != null)
            {
                Children.Remove(markforremoval);
            }
            yield return RunStatus.Running;
            yield break;
        }

        /*
        public override IEnumerable<RunStatus> Execute(object context)
        {
            throw new NotImplementedException();
        }*/
    }
}