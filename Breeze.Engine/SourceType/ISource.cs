using System;
using System.IO;

namespace Breeze.Engine.SourceType
{
    public interface ISource
    {
        FileInfo Info();
        String Content();
        string Gen();
        bool OverrideFile { get; }
    }
}
