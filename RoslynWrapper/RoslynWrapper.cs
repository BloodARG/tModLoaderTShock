﻿using System.CodeDom.Compiler;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Terraria.ModLoader
{
    public class RoslynWrapper
    {
        public static CompilerResults Compile(CompilerParameters args, string[] files) {
            var name = Path.GetFileNameWithoutExtension(args.OutputAssembly);
            var options = new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary)
                .WithOptimizationLevel(args.IncludeDebugInformation ? OptimizationLevel.Debug : OptimizationLevel.Release)
                .WithAssemblyIdentityComparer(DesktopAssemblyIdentityComparer.Default);
            var refs = args.ReferencedAssemblies.Cast<string>().Select(s => MetadataReference.CreateFromFile(s));
            var src = files.Select(f => SyntaxFactory.ParseSyntaxTree(File.ReadAllText(f), null, f, Encoding.UTF8));
            var comp = CSharpCompilation.Create(name, src, refs, options);

            var res = comp.Emit(args.OutputAssembly, args.IncludeDebugInformation ? Path.ChangeExtension(args.OutputAssembly, "pdb") : null);
            var cRes = new CompilerResults(args.TempFiles);
            foreach (var d in res.Diagnostics) {
                if (d.Severity != DiagnosticSeverity.Error)
                    continue;

                var loc = d.Location.GetLineSpan();
                var pos = loc.StartLinePosition;
                cRes.Errors.Add(new CompilerError(loc.Path, pos.Line, pos.Character, d.Id, d.GetMessage()));
            }

            return cRes;
        }
    }
}
