// See https://aka.ms/new-console-template for more information

using DanubeRS.CANServer.CLI.VideoJoiner;

var dir = new DirectoryInfo("/Volumes/Public/Dashcam");

var results = await Scanner.Scan(new ScanRequest(dir), new Progress<ScanProgress>(), CancellationToken.None);

const string temp = "/Volumes/T7/Dashcam";
const string final = "/Volumes/Public/Dashcam-test";
var vb = new VideoBuilder(final, temp);
foreach (var result in results.Results.OrderBy(r => r.Start))
{
    await vb.Build(result, CameraAngle.Front);
    await vb.Build(result, CameraAngle.Back);
    await vb.Build(result, CameraAngle.LeftRepeater);
    await vb.Build(result, CameraAngle.RightRepeater);
}