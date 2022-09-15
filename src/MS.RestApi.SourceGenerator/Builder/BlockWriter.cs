using System;
using System.Text;
using MS.RestApi.SourceGenerator.Extensions;

namespace MS.RestApi.SourceGenerator.Builder;

internal class IndentedWriter
{
    public int IndentLevel => _level;
        
    private readonly StringBuilder _builder;
    private readonly int _level;

    public IndentedWriter() : this(new StringBuilder(), 0){}
    public IndentedWriter(int level) : this(new StringBuilder(), level){}
    public IndentedWriter(StringBuilder builder) : this(builder, 0){}
    public IndentedWriter(StringBuilder builder, int level)
    {
        _builder = builder;
        _level = level;
    }
        
    public void WriteLine(string line = "")
    {
        _builder.AppendLineIndented(_level, line);
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

    public override string ToString()
    {
        return _builder.ToString();
    }
}