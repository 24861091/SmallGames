namespace GameLib.Main.Gameplay
{
    /// <summary>
    /// 一种特化的GameComponent
    /// 目的是让子节点能够最方便的找到当前逻辑中的根节点
    /// RootComponent本身也可以是其他的GameComponent的子节点
    /// RootComponent可以看作是一个逻辑模块的入口或者Manager，
    /// 子节点需要比较方便能访问到它的引用
    /// 继承RootComponent来实现自己的逻辑
    /// </summary>
    public abstract class RootComponent : GameComponent
    {
        protected override RootComponent GetRootForChild()
        {
            return this;
        }
    }
}