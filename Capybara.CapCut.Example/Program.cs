using Capybara.CapCut;

var newVideo = args[0];
//Set current directory to the folder of CapCut project, for example: %userprofile%\AppData\Local\CapCut\User Data\Projects\com.lveditor.draft\Test
var ctx = new Context(Directory.GetCurrentDirectory());
var project = await ctx.GetProjectAsync();
var v = project.Materials.Videos.First();

//Pass single argument as full path to new video file, for example: C:\Videos\newvideo.mp4
v.ChangePath(args[0]);

await ctx.SaveChangesAsync();
