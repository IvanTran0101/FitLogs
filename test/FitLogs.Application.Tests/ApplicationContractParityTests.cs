using System;
using System.Linq;
using System.Reflection;
using Xunit;

namespace FitLogs;

public class ApplicationContractParityTests
{
    [Fact]
    public void Every_public_application_service_method_is_declared_by_its_contract()
    {
        var applicationAssembly = typeof(FitLogsApplicationModule).Assembly;
        var serviceTypes = applicationAssembly
            .GetTypes()
            .Where(type => type.Name.EndsWith("AppService", StringComparison.Ordinal) && !type.IsAbstract);

        var missingMethods = serviceTypes
            .SelectMany(serviceType =>
            {
                var contracts = serviceType.GetInterfaces()
                    .Where(contract => contract.Name.EndsWith("AppService", StringComparison.Ordinal))
                    .ToArray();
                var contractMethods = contracts
                    .SelectMany(contract => contract.GetMethods())
                    .Select(MethodSignature)
                    .ToHashSet(StringComparer.Ordinal);

                return serviceType
                    .GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly)
                    .Where(method => !method.IsSpecialName)
                    .Where(method => !contractMethods.Contains(MethodSignature(method)))
                    .Select(method => $"{serviceType.Name}.{MethodSignature(method)}");
            })
            .ToArray();

        Assert.True(
            missingMethods.Length == 0,
            "Application methods missing from contracts: " + string.Join(", ", missingMethods));
    }

    private static string MethodSignature(MethodInfo method)
    {
        return $"{method.Name}({string.Join(",", method.GetParameters().Select(parameter => parameter.ParameterType.FullName))})";
    }
}
