using System;
using System.Linq;
using System.Text;

namespace MS.RestApi.SourceGenerator.Extensions;

internal class IndentedWriter
{
    private readonly StringBuilder _builder;
    private readonly int _level;

    public IndentedWriter(StringBuilder builder) : this(builder, 0){}
    public IndentedWriter(StringBuilder builder, int level)
    {
        _builder = builder;
        _level = level;
    }
        
    public void WriteLine(string line = "")
    {
        _builder.AppendLine(string.Join("", Enumerable.Repeat("\t", _level)) + line);
    }

    public void WriteBlock(Action<IndentedWriter> inner, string open = "{", string close = "}")
    {
        WriteLine(open);
            
        var block = new StringBuilder();
        var blockWriter = new IndentedWriter(block, _level + 1);
            
        inner(blockWriter);
            
        _builder.Append(block);
            
        WriteLine(close);
    }
}