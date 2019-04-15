using System.Linq;
using System.IO.Compression;
using System.IO;
using Kaitai;
using System.Collections.Generic;
using JavaScanner.Models;
using System.Collections.ObjectModel;

namespace JavaScanner
{
    public class JarReader
    {
        public JarReader()
        {
        }

        public static Jar FromFile(string jarPath)
        {
            var jar = new Jar();
            var javaClasses = new List<FileItem>();

            using (ZipArchive archive = ZipFile.Open(jarPath, ZipArchiveMode.Read))
            {
                foreach (var entry in archive.Entries)
                {
                    if (!entry.FullName.EndsWith(".class")){
                        continue;
                    }

                    using (var javaClassStream = entry.Open())
                    {
                        var fileItem = GetClass(javaClassStream);
                        javaClasses.Add(fileItem);
                    }
                }
            } 

            jar.FileItems = new ReadOnlyCollection<FileItem>(javaClasses);

            return jar;
        }

        private static FileItem GetClass(Stream javaClassStream)
        {
            var javaClass = new JavaClass(new KaitaiStream(javaClassStream));
            var fileItem =  new FileItem();
            var thisClassCpInfo = GetClassCpInfo(javaClass, javaClass.ThisClass);

            fileItem.Name = thisClassCpInfo.NameAsStr;
            fileItem.Methods = GetJavaMethods(javaClass);

            return fileItem;
        }

        private static IReadOnlyCollection<JavaMethod> GetJavaMethods(JavaClass javaClass)
        {
            var methods = new List<JavaMethod>();

            foreach (var method in javaClass.Methods)
            {
                var javaMethod = new JavaMethod();
                javaMethod.MethodName = method.NameAsStr;
                javaMethod.Signature = GetMethodSignature(javaClass, method);
                methods.Add(javaMethod);
            }

            return new ReadOnlyCollection<JavaMethod>(methods);
        }

        private static Signature GetMethodSignature(JavaClass javaClass, JavaClass.MethodInfo methodInfo)
        {
            var signAttr = methodInfo.Attributes.FirstOrDefault(a => a.NameAsStr == "Signature");
            var descriptor = ((JavaClass.Utf8CpInfo) (javaClass.ConstantPool[(methodInfo.DescriptorIndex - 1)].CpInfo));

            string signature;
            if (signAttr != null)
            {
                var index = signAttr.NameIndex;
                var signatureIndex = ((byte[]) signAttr.Info)[1];
                var signCpInfo = GetUtf8CpInfo(javaClass, signatureIndex);;
                signature = (signCpInfo).Value;
            }
            else
            {
                signature = descriptor.Value;
            }

            var endOfArgs = signature.LastIndexOf(")");
            var argsStr = signature.Substring(1, endOfArgs - 1);
            argsStr = argsStr.TrimEnd(';');
            
            return new Signature{
                Arguments = argsStr.Split(';'),
                ReturnType = signature.Substring(endOfArgs + 1)
            };
        }

        private static JavaClass.Utf8CpInfo GetUtf8CpInfo(JavaClass javaClass, int index)
        {
            return (JavaClass.Utf8CpInfo) ((javaClass.ConstantPool[(index - 1)].CpInfo));
        }

        private static JavaClass.ClassCpInfo GetClassCpInfo(JavaClass javaClass, int index)
        {
            return (JavaClass.ClassCpInfo) ((javaClass.ConstantPool[(index - 1)].CpInfo));
        }
    }
}