namespace TreeSharp.Example
{
    using Annotations;

    [UsedImplicitly]
    internal class Program
    {
        private static void Main()
        {
            TreeExample.WalkTree(TreeExample.ExampleTree());
        }
    }
}