using System.Reflection;
using System.Runtime.InteropServices;
using System.Web.UI;

[assembly: AssemblyTitle("WebFiler")]
[assembly: AssemblyDescription("Yet another web file manager")]
[assembly: AssemblyCompany("MWM")]
[assembly: AssemblyProduct("WebFiler")]
[assembly: AssemblyCopyright("Copyright © MWM 2010. All Rights Reserved.")]
[assembly: ComVisible(false)]
[assembly: Guid("3d5900ae-111a-45be-96b3-d9e4606ca793")]
[assembly: AssemblyVersion("1.2.0.0")]
[assembly: AssemblyFileVersion("1.2.0.0")]

// All of the images: might mean a larger dll but less clutter to output.
[assembly: WebResource("WebFiler.Images.delete.png", "img/png")]
[assembly: WebResource("WebFiler.Images.file.png", "img/png")]
[assembly: WebResource("WebFiler.Images.folder.png", "img/png")]
[assembly: WebResource("WebFiler.Images.folder_up.png", "img/png")]
[assembly: WebResource("WebFiler.Images.rename.png", "img/png")]

// The style sheet.
[assembly: WebResource("WebFiler.CSS.Default.css", "text/css")]