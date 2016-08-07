using System;
using System.Reflection;
using System.Resources;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using JSLintNet;

[assembly: AssemblyTitle("JSLint.NET")]
[assembly: AssemblyCompany(AssemblyInfo.Company)]
[assembly: AssemblyProduct(AssemblyInfo.Product)]
[assembly: AssemblyCopyright(AssemblyInfo.Copyright)]
[assembly: AssemblyVersion(AssemblyInfo.Version)]
[assembly: AssemblyInformationalVersion(AssemblyInfo.InformationalVersion)]

[assembly: ComVisible(false)]
[assembly: CLSCompliant(false)]
[assembly: Guid("683a3b2e-608a-463b-8046-85e856294a34")]
[assembly: NeutralResourcesLanguage("en-US")]

[assembly: InternalsVisibleTo("JSLintNet.Console, PublicKey=" + AssemblyInfo.PublicKey)]
[assembly: InternalsVisibleTo("JSLintNet.VisualStudio, PublicKey=" + AssemblyInfo.PublicKey)]
[assembly: InternalsVisibleTo("JSLintNet.QualityTools, PublicKey=" + AssemblyInfo.PublicKey)]
[assembly: InternalsVisibleTo("JSLintNet.Specifications, PublicKey=" + AssemblyInfo.PublicKey)]
[assembly: InternalsVisibleTo("JSLintNet.Console.Specifications, PublicKey=" + AssemblyInfo.PublicKey)]
[assembly: InternalsVisibleTo("JSLintNet.VisualStudio.Specifications, PublicKey=" + AssemblyInfo.PublicKey)]
[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2, PublicKey=" + AssemblyInfo.DynamicPublicKey)]

namespace JSLintNet
{
    /// <summary>
    /// Version number and copyright constants for sharing between projects.
    /// </summary>
    internal static class AssemblyInfo
    {
        /// <summary>
        /// The core version number for JSLint.NET.
        /// </summary>
        public const string Version = "2.3.0";

        /// <summary>
        /// The informational version for JSLint.NET.
        /// </summary>
        public const string InformationalVersion = Version;

        /// <summary>
        /// The copyright line for JSLint.NET.
        /// </summary>
        public const string Copyright = "Copyright © JSLint.NET Contributors 2013-2016";

        /// <summary>
        /// The company name for JSLint.NET.
        /// </summary>
        public const string Company = "JSLint.NET Contributors";

        /// <summary>
        /// The product name for JSLint.NET.
        /// </summary>
        public const string Product = "JSLint.NET";

        /// <summary>
        /// The edition of JSLint bundled with this release.
        /// </summary>
        public const string Edition = "2016-07-13";

        /// <summary>
        /// The public key for JSLint.NET.
        /// </summary>
        public const string PublicKey = "00240000048000009400000006020000002400005253413100040000010001000168e96c4e92cd1cfe13015e01d7b5c6db7df73bc4cbb64deb31cf449bbd3510b5f0a7324d9fc12ab1e9890d47a558a6125258b1f49fb4b6f2cd29cad5241337a023e3741961a681139f0e3386a68643e09015953eb97a3a9a0e4e0508983d4f66e1a60491c35ce1e6a88edf48a453a7036d768dde8b596fda2952be86ad73bb";

        /// <summary>
        /// The dynamic public key.
        /// </summary>
        public const string DynamicPublicKey = "0024000004800000940000000602000000240000525341310004000001000100c547cac37abd99c8db225ef2f6c8a3602f3b3606cc9891605d02baa56104f4cfc0734aa39b93bf7852f7d9266654753cc297e7d2edfe0bac1cdcf9f717241550e0a7b191195b7667bb4f64bcb8e2121380fd1d9d46ad2d92d2d15605093924cceaf74c4861eff62abf69b9291ed0a340e113be11e6a7d3113e92484cf7045cc7";
    }
}
