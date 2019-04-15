using System;
using System.Linq;
using Kaitai;

namespace JavaScannerMain
{
    class Program
    {
        static void Main(string[] args)
        {
            //var data = JavaClass.FromFile("D:\\Work\\java\\test_compile\\HelloWorld.class");
            var jar = JavaScanner.JarReader.FromFile("D:\\Work\\java\\test_compile\\myJar.jar");
            //Console.WriteLine( data.NameIndex);
            // JsonConvert.DefaultSettings = () => new JsonSerializerSettings
            // {
            //     PreserveReferencesHandling = PreserveReferencesHandling.All,
            //     ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                
            // };

            // foreach (var mInfo in data.Methods)
            // {
            //     ShowSign(data, mInfo);
            // }

        }

        private static void ShowSign(JavaClass javaClass, JavaClass.MethodInfo methodInfo)
        {
            var signAttr = methodInfo.Attributes.FirstOrDefault(a => a.NameAsStr == "Signature");
            Console.WriteLine(methodInfo.NameAsStr);
            var descriptor = ((JavaClass.Utf8CpInfo) (javaClass.ConstantPool[(methodInfo.DescriptorIndex - 1)].CpInfo));
            if (signAttr != null)
            {
                var index = signAttr.NameIndex;
                var signatureIndex = ((byte[]) signAttr.Info)[1];
                var signCpInfo = ((JavaClass.Utf8CpInfo) ((javaClass.ConstantPool[(signatureIndex - 1)].CpInfo)));
                var sign = (signCpInfo).Value;
                Console.WriteLine(sign);
                Console.WriteLine("---");
                return;
            }

            var args1 = descriptor.Value;
            Console.WriteLine(args1);
            Console.WriteLine("---");
        }
    }
}
