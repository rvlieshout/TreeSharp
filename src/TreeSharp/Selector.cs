namespace TreeSharp
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    ///   The base selector class. This will attempt to execute all branches of logic, until one succeeds. 
    ///   This composite will fail only if all branches fail as well.
    /// </summary>
    public abstract class Selector : GroupComposite
    {
        public Selector(params Composite[] children) : base(children)
        {
        }

        public abstract override IEnumerable<RunStatus> Execute(object context);
    }

    public class ProbabilitySelection
    {
        public Composite Branch;

        public double ChanceToExecute;

        public ProbabilitySelection(Composite branch, double chanceToExecute)
        {
            Branch = branch;
            ChanceToExecute = chanceToExecute;
        }
    }

    /// <summary>
    ///   Will execute random branches of logic, until one succeeds. This composite
    ///   will fail only if all branches fail as well.
    /// </summary>
    public class ProbabilitySelector : Selector
    {
        public ProbabilitySelector(params ProbabilitySelection[] children)
            : base(children.Select(c => c.Branch).ToArray())
        {
            PossibleBranches = children.OrderBy(c => c.ChanceToExecute).ToArray();
            Randomizer = new Random();
        }

        private ProbabilitySelection[] PossibleBranches { get; set; }

        protected Random Randomizer { get; private set; }

        public override IEnumerable<RunStatus> Execute(object context)
        {
            throw new NotImplementedException();
        }
    }
}