using DanubeRS.CanServerUtils.Lib.DBC.Parser;
using DanubeRS.CanServerUtils.Lib.DBC.Tokenizer;
using Superpower;

namespace DanubeRS.CanServerUtils.Lib.DBC;

public class Database
{
    public Database()
    {
    }

    public void AddFile(TextReader reader)
    {
        var tokenizer = DbcTokenizer.Instance;
        var tokens = tokenizer.Tokenize(reader.ReadToEnd());
        var test = DbcParser.Database(tokens);
    }
}