/************************************************
 created : 2018-09-05
 author : wanglijun
************************************************/
using System;

namespace EchoEngine
{
    public interface ITickable : IDisposable
    {
        void Tick(float deltaTime);
    }
}