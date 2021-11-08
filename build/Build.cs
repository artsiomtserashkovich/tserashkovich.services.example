using System.IO;
using Nuke.Common;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Utilities.Collections;

using static Nuke.Common.IO.FileSystemTasks;
using static Nuke.Common.IO.CompressionTasks;
using static Nuke.Common.Tools.DotNet.DotNetTasks;

public class Build : NukeBuild
{
    public static int Main() => Execute<Build>(x => x.PublishApp);

    AbsolutePath OutputDirectory => RootDirectory / "buildOutput";
    AbsolutePath SourceDirectory => RootDirectory / "src";
    
    [Solution] private readonly Solution Solution;


    Target CleanDirectories => _ => _
        .Executes(() =>
        {
            EnsureCleanDirectory(OutputDirectory);
            
            SourceDirectory.GlobDirectories("**/bin", "**/obj").ForEach(DeleteDirectory);
        });
    
    Target BuildAndTestApp => _ => _
        .DependsOn(CleanDirectories)
        .Executes(() =>
        {
            DotNetBuild(settings => settings
                .SetProjectFile(Solution)
                .SetConfiguration("Release"));
            
            DotNetTest(settings => settings
                .CombineWith(
                    Solution.GetProjects("*.Tests"), 
                    (_, project) => _.SetProjectFile(project)));
        });

    Target PublishApp => _ => _
        .DependsOn(BuildAndTestApp)
        .Executes(() =>
        {
            var apiPublishPath = OutputDirectory / "api";
            
            DotNetPublish(settings => settings
                .SetProject(Solution.GetProject($"{Solution.Name}.Api"))
                .SetNoBuild(true)
                .SetConfiguration("Release")
                .SetOutput(apiPublishPath));
            
            CompressZip(
                directory: apiPublishPath,
                archiveFile: OutputDirectory / $"{Solution.Name}.zip",
                fileMode: FileMode.Create);
        });
            
}