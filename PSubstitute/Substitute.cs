using System.Reflection;
using System.Reflection.Emit;

namespace PSubstitute;

public class Substitute
{
    public static List<string> MethodCalls = new ();
    internal static readonly Dictionary<int, string> hashCodeToGuid = new();

    public static T For<T>() where T : class
        {
            // Get the type of T
            var type = typeof(T);

            // Get all methods of T
            var methods = type.GetMethods();

            // Create a dynamic assembly, module, and type
            var assemblyName = new AssemblyName("DynamicAssembly");
            var assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run);
            var moduleBuilder = assemblyBuilder.DefineDynamicModule("DynamicModule");

            // Define a dynamic type that implements T
            var typeBuilder = moduleBuilder.DefineType("DynamicType", TypeAttributes.Public, null, new Type[] { type });

            // Get the MethodInfo for Console.WriteLine(string)
            var addMethod = typeof(List<string>).GetMethod("Add");
            
            var guid = Guid.NewGuid().ToString();

            // For each method of T
            foreach (var method in methods)
            {
                // Define a method on the dynamic type with the same signature
                var methodBuilder = typeBuilder.DefineMethod(method.Name, MethodAttributes.Public | MethodAttributes.Virtual, method.ReturnType, Array.ConvertAll(method.GetParameters(), p => p.ParameterType));

                // In the body of the dynamic method, call Console.WriteLine(string)
                var ilGenerator = methodBuilder.GetILGenerator();
                ilGenerator.Emit(OpCodes.Ldsfld, typeof(Substitute).GetField(nameof(MethodCalls)));
                ilGenerator.Emit(OpCodes.Ldstr, $"{method.Name}_{guid}");
                ilGenerator.Emit(OpCodes.Callvirt, addMethod);
                ilGenerator.Emit(OpCodes.Ret);
                ilGenerator.Emit(OpCodes.Ret);
            }

            // Create the type
            var dynamicType = typeBuilder.CreateType();

            // Create an instance of the dynamic type
            var instance = (T)Activator.CreateInstance(dynamicType);
            hashCodeToGuid.Add(instance.GetHashCode(), guid);
            return instance;
        }
}


public static class SubstituteExtensions
{
    public static void Received<T>(this T mock, string methodName, int? expectedCount = null) where T : class
    {
        Substitute.hashCodeToGuid.TryGetValue(mock.GetHashCode(), out var guid);
        var message = $"{methodName} was not called.";
        if (guid == null)
        {
            throw new Exception(message);
        }


        var count = Substitute.MethodCalls.Count(x => x == $"{methodName}_{guid}");
        if (expectedCount != null && count != expectedCount ||  count == 0)
        {
            throw new Exception(message);
        }
    }
}