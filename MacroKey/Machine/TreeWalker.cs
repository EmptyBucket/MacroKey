namespace MacroKey.Machine
{
    class TreeWalker<KeyTypeTransition> : StateWalker<KeyTypeTransition>
    {
        public TreeWalker(Tree<KeyTypeTransition> tree) :base(tree.StartStateTree) { }
    }
}
