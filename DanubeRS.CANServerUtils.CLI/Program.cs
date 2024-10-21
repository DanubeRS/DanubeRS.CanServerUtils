// See https://aka.ms/new-console-template for more information

using DanubeRS.CanServerUtils.Lib.BinaryLogs;
using DanubeRS.CanServerUtils.Lib.DBC;
using DanubeRS.CanServerUtils.Lib.DBC.Parser;
using Microsoft.Extensions.Logging;

var loggerFactory = LoggerFactory.Create(b => { b.AddConsole(); });

var dbc = new Database();
var testString = @"VERSION """"

NS_ : 
	NS_DESC_
	CM_
	BA_DEF_
	BA_
	VAL_
	CAT_DEF_
	CAT_
	FILTER
	BA_DEF_DEF_
	EV_DATA_
	ENVVAR_DATA_
	SGTYPE_
	SGTYPE_VAL_
	BA_DEF_SGTYPE_
	BA_SGTYPE_
	SIG_TYPE_REF_
	VAL_TABLE_
	SIG_GROUP_
	SIG_VALTYPE_
	SIGTYPE_VALTYPE_
	BO_TX_BU_
	BA_DEF_REL_
	BA_RE_
	BA_DEF_DEF_REL_
	BU_SG_REL_
	BU_EV_REL_
	BU_BO_REL_
	SG_MUL_VAL_
";
await using var dbcStream = File.OpenRead("./database.dbc");
using var dbcReader = new StringReader(testString);
dbc.AddFile(dbcReader);

var logger = loggerFactory.CreateLogger<Program>();
var dir = new DirectoryInfo(Directory.GetCurrentDirectory());
var files = dir.EnumerateFiles("*.log");
foreach (var file in files)
{
    await using var fs = file.OpenRead();
    var reader = await Reader.Open(fs, loggerFactory.CreateLogger<Reader>());
    var frames = 0;
    while (true)
    {
        var frame = reader.GetNextFrame();
        frames++;
        if (frames % 1000 == 0)
            logger.LogInformation("Processed {frames} frames", frames);
        if (frame == null)
            return;
    }
}