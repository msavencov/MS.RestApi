using System;
using System.Text;

namespace MS.RestApi.SourceGenerator.Helpers;

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

    public IndentedWriter Write(string text)
    {
        for (var i = 0; i < _level; i++)
        {
            _builder.Append('\t');
        }

        _builder.Append(text);
        
        return this;
    }
    
    public void WriteLine(string line = "")
    {
        for (var i = 0; i < _level; i++)
        {
            _builder.Append('\t');
        }

        _builder.AppendLine(line);
    }

    public void WriteBlock(Action<IndentedWriter> inner) => WriteBlock("{", "}", inner);
    public void WriteBlock(string open, string close, Action<IndentedWriter> inner)
    {
        WriteLine(open);
            
        var block = new StringBuilder();
        var blockWriter = new IndentedWriter(block, _level + 1);
            
        inner(blockWriter);
            
        _builder.Append(block);
            
        WriteLine(close);
    }

    public void WriteHeaderLines()
    {
        WriteLine("// <auto-generated />");
        WriteLine("#pragma warning disable CS1591");
    }

    public override string ToString()
    {
        return _builder.ToString();
    }
}