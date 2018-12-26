using System;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using Acb.Aop.Attributes;
using AspectCore.Extensions.Reflection;

namespace Acb.Aop.Proxy
{
    public class AopProxy
    {
        public static TInterface Create<TInterface, TImp>(Type interceptorAttributeType = null,
            Type actionAttributeType = null) where TImp : class, new() where TInterface : class
        {
            return Invoke<TInterface, TImp>(false, interceptorAttributeType, actionAttributeType);
        }

        public static TProxyClass Create<TProxyClass>(Type interceptorAttributeType = null,
            Type actionAttributeType = null) where TProxyClass : class, new()
        {
            return Invoke<TProxyClass, TProxyClass>(true, interceptorAttributeType, actionAttributeType);
        }

        private static TInterface Invoke<TInterface, TImp>(bool inheritMode = false, Type interceptorAttributeType = null,
        Type actionAttributeType = null) where TImp : class, new() where TInterface : class
        {
            var impType = typeof(TImp);
            var interfaceType = typeof(TInterface);

            var nameOfAssembly = impType.Name + "ProxyAssembly";
            var nameOfModule = impType.Name + "ProxyModule";
            var nameOfType = impType.Name + "Proxy";

            var assemblyName = new AssemblyName(nameOfAssembly);

            var assembly = AssemblyBuilder.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run);
            var moduleBuilder = assembly.DefineDynamicModule(nameOfModule);

            var typeBuilder = inheritMode
                ? moduleBuilder.DefineType(nameOfType, TypeAttributes.Public, impType)
                : moduleBuilder.DefineType(nameOfType, TypeAttributes.Public, null, new[] { interfaceType });

            //拦截器类型
            interceptorAttributeType = interceptorAttributeType ??
                                       impType.GetReflector().GetCustomAttribute(typeof(AopInterceptorAttribute))?.GetType();

            InjectInterceptor<TImp>(typeBuilder, interceptorAttributeType, actionAttributeType, inheritMode);

            var t = typeBuilder.CreateTypeInfo();

            return Activator.CreateInstance(t) as TInterface;
        }

        private static void InjectInterceptor<TImp>(TypeBuilder typeBuilder, Type interceptorAttributeType = null, Type actionType = null, bool inheritMode = false)
        {
            var impType = typeof(TImp);
            // ---- define fields ----
            FieldBuilder fieldInterceptor = null;
            if (interceptorAttributeType != null)
            {
                fieldInterceptor = typeBuilder.DefineField("_interceptor", interceptorAttributeType, FieldAttributes.Private);
                var constructorBuilder = typeBuilder.DefineConstructor(MethodAttributes.Public, CallingConventions.Standard, null);
                var ilOfCtor = constructorBuilder.GetILGenerator();

                ilOfCtor.Emit(OpCodes.Ldarg_0);
                ilOfCtor.Emit(OpCodes.Newobj, interceptorAttributeType.GetConstructor(new Type[0]));
                ilOfCtor.Emit(OpCodes.Stfld, fieldInterceptor);
                ilOfCtor.Emit(OpCodes.Ret);
            }

            // ---- define methods ----

            var methodsOfType = impType.GetMethods(BindingFlags.Public | BindingFlags.Instance);

            var ignoreMethodName = new[] { "GetType", "ToString", "GetHashCode", "Equals" };

            foreach (var method in methodsOfType)
            {
                //ignore method
                if (ignoreMethodName.Contains(method.Name))
                    return;

                var methodParameterTypes = method.GetParameters().Select(p => p.ParameterType).ToArray();

                MethodAttributes methodAttributes;

                if (inheritMode)
                    methodAttributes = MethodAttributes.Public | MethodAttributes.Virtual;
                else
                    methodAttributes = MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.NewSlot | MethodAttributes.Virtual | MethodAttributes.Final;

                var methodBuilder = typeBuilder.DefineMethod(method.Name, methodAttributes, CallingConventions.Standard, method.ReturnType, methodParameterTypes);

                var ilMethod = methodBuilder.GetILGenerator();

                // set local field
                var impObj = ilMethod.DeclareLocal(impType);                //instance of imp object
                var methodName = ilMethod.DeclareLocal(typeof(string));     //instance of method name
                var parameters = ilMethod.DeclareLocal(typeof(object[]));   //instance of parameters
                var result = ilMethod.DeclareLocal(typeof(object));         //instance of result
                LocalBuilder actionAttributeObj = null;

                //attribute init
                if (actionType == null)
                {
                    actionType = method.GetReflector().GetCustomAttribute(typeof(AopActionAttribute))?.GetType();
                    if (actionType == null)
                        actionType = impType.GetReflector().GetCustomAttribute(typeof(AopActionAttribute))?.GetType();
                }

                if (actionType != null)
                {
                    actionAttributeObj = ilMethod.DeclareLocal(actionType);
                    ilMethod.Emit(OpCodes.Newobj, actionType.GetConstructor(new Type[0]));
                    ilMethod.Emit(OpCodes.Stloc, actionAttributeObj);
                }

                //instance imp
                ilMethod.Emit(OpCodes.Newobj, impType.GetConstructor(new Type[0]));
                ilMethod.Emit(OpCodes.Stloc, impObj);

                //if no attribute
                if (fieldInterceptor != null || actionAttributeObj != null)
                {
                    ilMethod.Emit(OpCodes.Ldstr, method.Name);
                    ilMethod.Emit(OpCodes.Stloc, methodName);

                    ilMethod.Emit(OpCodes.Ldc_I4, methodParameterTypes.Length);
                    ilMethod.Emit(OpCodes.Newarr, typeof(object));
                    ilMethod.Emit(OpCodes.Stloc, parameters);

                    // build the method parameters
                    for (var j = 0; j < methodParameterTypes.Length; j++)
                    {
                        ilMethod.Emit(OpCodes.Ldloc, parameters);
                        ilMethod.Emit(OpCodes.Ldc_I4, j);
                        ilMethod.Emit(OpCodes.Ldarg, j + 1);
                        //box
                        ilMethod.Emit(OpCodes.Box, methodParameterTypes[j]);
                        ilMethod.Emit(OpCodes.Stelem_Ref);
                    }
                }

                //dynamic proxy action before
                if (actionType != null)
                {
                    //load arguments
                    ilMethod.Emit(OpCodes.Ldloc, actionAttributeObj);
                    ilMethod.Emit(OpCodes.Ldloc, methodName);
                    ilMethod.Emit(OpCodes.Ldloc, parameters);
                    ilMethod.Emit(OpCodes.Call, actionType.GetMethod("Before"));
                }

                if (interceptorAttributeType != null)
                {
                    //load arguments
                    ilMethod.Emit(OpCodes.Ldarg_0);//this
                    ilMethod.Emit(OpCodes.Ldfld, fieldInterceptor);
                    ilMethod.Emit(OpCodes.Ldloc, impObj);
                    ilMethod.Emit(OpCodes.Ldloc, methodName);
                    ilMethod.Emit(OpCodes.Ldloc, parameters);
                    // call Invoke() method of Interceptor
                    ilMethod.Emit(OpCodes.Callvirt, interceptorAttributeType.GetMethod("Invoke"));
                }
                else
                {
                    //direct call method
                    if (method.ReturnType == typeof(void) && actionType == null)
                    {
                        ilMethod.Emit(OpCodes.Ldnull);
                    }

                    ilMethod.Emit(OpCodes.Ldloc, impObj);
                    for (var j = 0; j < methodParameterTypes.Length; j++)
                    {
                        ilMethod.Emit(OpCodes.Ldarg, j + 1);
                    }
                    ilMethod.Emit(OpCodes.Callvirt, impType.GetMethod(method.Name));
                    //box
                    if (actionType != null)
                    {
                        if (method.ReturnType != typeof(void))
                            ilMethod.Emit(OpCodes.Box, method.ReturnType);
                        else
                            ilMethod.Emit(OpCodes.Ldnull);
                    }
                }

                //dynamic proxy action after
                if (actionType != null)
                {
                    ilMethod.Emit(OpCodes.Stloc, result);
                    //load arguments
                    ilMethod.Emit(OpCodes.Ldloc, actionAttributeObj);
                    ilMethod.Emit(OpCodes.Ldloc, methodName);
                    ilMethod.Emit(OpCodes.Ldloc, result);
                    ilMethod.Emit(OpCodes.Call, actionType.GetMethod("After"));
                }

                // pop the stack if return void
                if (method.ReturnType == typeof(void))
                {
                    ilMethod.Emit(OpCodes.Pop);
                }
                else
                {
                    //unbox,if direct invoke,no box
                    if (fieldInterceptor != null || actionAttributeObj != null)
                    {
                        if (method.ReturnType.IsValueType)
                            ilMethod.Emit(OpCodes.Unbox_Any, method.ReturnType);
                        else
                            ilMethod.Emit(OpCodes.Castclass, method.ReturnType);
                    }
                }
                // complete
                ilMethod.Emit(OpCodes.Ret);
            }
        }
    }
}
