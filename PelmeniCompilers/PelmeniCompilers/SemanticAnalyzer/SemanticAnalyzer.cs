using System.Collections.Immutable;
using System.Reflection;
using System.Runtime.CompilerServices;
using PelmeniCompilers.ExtensionsMethods;
using PelmeniCompilers.Models;
using PelmeniCompilers.SemanticAnalyzer.Checkers;
using PelmeniCompilers.SemanticAnalyzer.ScopeUnit;
using PelmeniCompilers.SemanticAnalyzer.VirtualTable;
using PelmeniCompilers.Values;

namespace PelmeniCompilers.SemanticAnalyzer;

public class SemanticAnalyzer
{
    private readonly Node _mainNode;
    
    public SemanticAnalyzer(Node mainNode)
    {
        mainNode.ThrowIfNodeNotTypeOf(NodeType.Program);
        
        _mainNode = mainNode;
    }

    public void Analyze()
    {
        RegisterStdLib("PelmeniLib");
        
        _mainNode.RemoveAliasing();
        _mainNode.CheckSemantic();
        _mainNode.Optimize();
    }

    private void RegisterStdLib(string name)
    {
        var typeConversion = new Dictionary<string, string>
        {
            { "System.Int32", "integer" },
            { "Int32", "integer" },
            { "System.Int64", "integer" },
            { "Int64", "integer" },
            { "System.Boolean", "boolean" },
            { "Boolean", "boolean" },
            { "System.Double", "real" },
            { "Double", "real" },
            { "System.Float", "real" },
            { "Float", "real" },
            { "System.Char", "char" },
            { "Char", "char" },
            { "System.String", "string" },
            { "String", "string" },
            { "Void", "None" }
        };
        
        var assembly = Assembly.Load(name);
        var userTypes = assembly.GetTypes()
            .Where(t => t.Namespace == "PelmeniLib")
            .Where(t => !t.GetTypeInfo().IsDefined(typeof(CompilerGeneratedAttribute), true));
        foreach (var type in userTypes)
        {
            var virtualTableEntry = new RecordVirtualTableEntry();
            
            var identifier = type.Name;
            var fields = type.GetFields();
            var methods = type.GetMethods();
            if (fields.Length > 0)
            {
                virtualTableEntry.Name = identifier;
                
                foreach (var field in fields)
                {
                    if (!field.IsStatic)
                    {
                        continue;
                    }
                    var fieldName = field.Name;
                    var typeStr = field.FieldType.Name;


                    var fieldList = new List<VariableVirtualTableEntry>();

                    var success = typeConversion.TryGetValue(typeStr, out var typeName);
                    if (success)
                    {
                        fieldList.Add(new VariableVirtualTableEntry()
                        {
                            Name = fieldName, 
                            Node = null!, 
                            Type = typeName!, 
                            Value = null!
                        });
                    }
                    else
                    {
                        throw new InvalidOperationException($"unknown type {typeStr} in assembly {name}");
                    }

                    virtualTableEntry.Members = fieldList;
                    virtualTableEntry.Node = null;
                }

                BaseNodeRuleChecker.RecordVirtualTable.Add(identifier, virtualTableEntry);
            }

            if (methods.Length > 0)
            {
                foreach (var method in methods)
                {
                    if (!method.IsStatic)
                    {
                        continue;
                    }
                    
                    var routineVirtualTableEntry = new RoutineVirtualTableEntry();
                    
                    var methodName = method.Name;
                    routineVirtualTableEntry.Name = methodName;
                    
                    var returnType = method.ReturnType.Name;
                    
                    var parameters = method.GetParameters();
                    var success = typeConversion.TryGetValue(returnType, out var returnTypeName);
                    if (success)
                    {
                        routineVirtualTableEntry.ReturnType = returnTypeName;
                    }
                    else
                    {
                        throw new InvalidOperationException($"unknown type {returnType} in assembly {name}");
                    }

                    var parametersList = new List<VariableVirtualTableEntry>();
                    
                    foreach (var parameter in parameters)
                    {
                        var parameterName = parameter.Name;
                        var parameterType = parameter.ParameterType.Name;
                        
                        success = typeConversion.TryGetValue(parameterType, out var typeName);
                        if (success)
                        {
                            parametersList.Add(new VariableVirtualTableEntry()
                            {
                                Name = parameterName, 
                                Node = null!, 
                                Type = typeName!, 
                                Value = null!
                            });
                        }
                        else
                        {
                            throw new InvalidOperationException($"unknown type {parameterType} in assembly {name}");
                        }
                    }

                    routineVirtualTableEntry.Parameters = parametersList;
                    BaseNodeRuleChecker.RoutineVirtualTable.Add(methodName, routineVirtualTableEntry);
                }
            }
            
        }
    }
}