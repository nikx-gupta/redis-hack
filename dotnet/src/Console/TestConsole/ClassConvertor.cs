using System;
using System.Collections.Generic;
using System.IO;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json.Linq;

namespace TestConsole {
    public class ClassConvertor {
        public void ConvertJsonToClass(string jsonFilePath, string className) {
            JObject objJson = JObject.Parse(File.ReadAllText(jsonFilePath));
            WriteClass(objJson, className);
        }

        public void ConvertBsonToClass(List<BsonDocument> docs, string className) {
            ClassInfo classInfo = new() {
                Name = className,
                IsRoot = true
            };

            foreach (var doc in docs) {
                ReadClassInfo(doc, classInfo);
            }

            WriteClassInfo(classInfo, @"E:\src\temp\MongoClasses");
        }

        public void ReadClassInfo(BsonDocument doc, ClassInfo classInfo) {
            foreach (var bsonElement in doc.Elements) {
                FieldInfo field = null;
                if (bsonElement.Value.BsonType == BsonType.Document && classInfo.Fields.ContainsKey(bsonElement.Name)) {
                    field = classInfo.Fields[bsonElement.Name];
                }
                else if (classInfo.Fields.ContainsKey(bsonElement.Name))
                    continue;
                else {
                    field = new() { FieldType = "string" };
                    if (bsonElement.Name == "id" && !classInfo.IsRoot) {
                        field.Name =
                            $"{char.ToUpper(classInfo.Name[0])}{classInfo.Name.Substring(1)}Id";
                        field.Attributes.Add(new BsonElementAttribute(bsonElement.Name));
                    }
                    else {
                        field.Name = bsonElement.Name;
                    }
                    
                    classInfo.Fields.Add(bsonElement.Name, field);
                }

                switch (bsonElement.Value.BsonType) {
                    case BsonType.ObjectId:
                        field.Attributes.Add(new BsonRepresentationAttribute(bsonElement.Value.BsonType));
                        break;
                    case BsonType.Double:
                        field.FieldType = "double";
                        break;
                    case BsonType.Array:
                        field.IsArray = true;
                        if (bsonElement.Value.BsonType == BsonType.Document) {
                            // field.FieldType = bsonElement.Name;
                            // if (!classInfo.Childs.ContainsKey(bsonElement.Name)) {
                            //     classInfo.Childs.Add(bsonElement.Name, new() { Name = bsonElement.Name });
                            // }
                            //
                            // ReadClassInfo(bsonElement.Value as BsonDocument, classInfo.Childs[bsonElement.Name]);
                        }
                        else {
                            field.FieldType = "string[]";
                        }

                        break;
                    case BsonType.Binary:
                        field.FieldType = "byte[]";
                        break;
                    case BsonType.Boolean:
                        field.FieldType = "bool";
                        break;
                    case BsonType.DateTime:
                    case BsonType.Timestamp:
                        field.FieldType = "DateTime";
                        break;
                    case BsonType.Int32:
                        field.FieldType = "int";
                        break;
                    case BsonType.Int64:
                        field.FieldType = "long";
                        break;
                    case BsonType.Decimal128:
                        field.FieldType = "decimal";
                        break;
                    case BsonType.Document:
                        field.FieldType = bsonElement.Name;
                        if (!classInfo.Childs.ContainsKey(bsonElement.Name)) {
                            classInfo.Childs.Add(bsonElement.Name, new() { Name = bsonElement.Name });
                        }

                        ReadClassInfo(bsonElement.Value as BsonDocument, classInfo.Childs[bsonElement.Name]);
                        break;
                }
            }
        }

        public void WriteClassInfo(ClassInfo classInfo, string dirName) {
            StringWriter writer = new();
            writer.WriteLine("namespace Hack.Service.RedisDb.Models {");
            writer.WriteLine($"public class {classInfo.Name} {{");
            foreach (var field in classInfo.Fields) {
                if (field.Value.Attributes.Count > 0) {
                    foreach (var attr in field.Value.Attributes) {
                        if (attr is BsonRepresentationAttribute bsonRepAttr)
                            writer.WriteLine(
                                $"[BsonRepresentation(BsonType.{bsonRepAttr.Representation.ToString()})]");
                        else if (attr is BsonElementAttribute elAttr)
                            writer.WriteLine(
                                $"[BsonElement(\"{elAttr.ElementName}\")]");
                    }
                }

                writer.WriteLine($"public {field.Value.FieldType} {field.Value.Name} {{get;set;}}");
            }

            writer.WriteLine("} }");

            foreach (var childInfo in classInfo.Childs) {
                WriteClassInfo(childInfo.Value, dirName);
            }

            File.WriteAllText($"{Path.Combine(dirName, classInfo.Name)}.cs", writer.ToString());
        }

        private void WriteClass(JObject jsonClass, string className) {
            StringWriter writer = new StringWriter();
            writer.WriteLine("namespace Hack.Service.RedisDb.Models {");
            writer.WriteLine($"public class {className} {{");

            foreach (var property in jsonClass.Properties()) {
                if (property.Value.Type == JTokenType.Array) {
                    if (property.Value.First.Type == JTokenType.Object) {
                        writer.WriteLine($"public {property.Name}[] {property.Name} {{get; set;}}");
                        WriteClass(property.Value as JObject, property.Name);
                    }
                    else {
                        writer.WriteLine($"public string[] {property.Name} {{get; set;}}");
                    }
                }
                else if (property.Value.Type == JTokenType.Object) {
                    writer.WriteLine($"public {property.Name} {property.Name} {{get; set;}}");
                    WriteClass(property.Value as JObject, property.Name);
                }
                else {
                    writer.WriteLine($"public string {property.Name} {{get; set;}}");
                }
            }

            writer.WriteLine("} }");
            File.WriteAllText($"{className}.cs", writer.ToString());
        }
    }

    public class ClassInfo {
        public ClassInfo() {
            Fields = new(StringComparer.InvariantCultureIgnoreCase);
            Childs = new(StringComparer.InvariantCultureIgnoreCase);
        }

        public string Name { get; set; }
        public Dictionary<string, ClassInfo> Childs { get; set; }
        public Dictionary<string, FieldInfo> Fields { get; set; }
        public bool IsRoot { get; set; }
    }

    public class FieldInfo {
        public FieldInfo() {
            Attributes = new();
        }

        public string Name { get; set; }
        public bool IsArray { get; set; }
        public string FieldType { get; set; }
        public List<Attribute> Attributes { get; set; }
    }
}