namespace TreeSharp
{
    using System.Collections.Generic;
    using Example.Annotations;

    public abstract class GroupComposite : Composite
    {
        protected GroupComposite([NotNull] params Composite[] children)
        {
            Children = new List<Composite>(children);
            foreach (Composite composite in Children)
            {
                if (composite != null)
                {
                    composite.Parent = this;
                }
            }
        }

        [NotNull]
        public List<Composite> Children { get; set; }

        public Composite Selection { get; protected set; }

        public override void Start(object context)
        {
            CleanupHandlers.Push(new ChildrenCleanupHandler(this, context));
            base.Start(context);
        }

        public void AddChild(Composite child)
        {
            if (child != null)
            {
                child.Parent = this;
                Children.Add(child);
            }
        }

        public void InsertChild(int index, Composite child)
        {
            if (child != null)
            {
                child.Parent = this;
                Children.Insert(index, child);
            }
        }

        #region Nested type: ChildrenCleanupHandler

        protected class ChildrenCleanupHandler : CleanupHandler
        {
            public ChildrenCleanupHandler(GroupComposite owner, object context)
                : base(owner, context)
            {
            }

            protected override void DoCleanup(object context)
            {
                foreach (Composite composite in (Owner as GroupComposite).Children)
                {
                    composite.Stop(context);
                }
            }
        }

        #endregion
    }
}