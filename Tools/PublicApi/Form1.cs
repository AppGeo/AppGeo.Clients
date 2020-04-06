using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;

namespace PublicApi
{
  public partial class Form1 : Form
  {
    private static string _basePath = null;

    public Form1()
    {
      InitializeComponent();
    }

    private void Form1_Resize(object sender, EventArgs e)
    {
      cmdLoad.Location = new Point(this.Width - 20 - cmdLoad.Width, cmdLoad.Location.Y);
      tboAssembly.Width = this.Width - tboAssembly.Location.X - 26 - cmdLoad.Width;
      tboApi.Width = this.Width - 32;
      tboApi.Height = this.Height - 78;
    }

    private void cmdLoad_Click(object sender, EventArgs e)
    {
      OpenFileDialog dialog = new OpenFileDialog();
      dialog.Filter = "Assemblies (*.dll)|*.dll";
      dialog.CheckFileExists = true;
      dialog.CheckPathExists = true;
      DialogResult result = dialog.ShowDialog();

      if (result == DialogResult.OK)
      {
        tboAssembly.Text = dialog.FileName;
        LoadAssembly(dialog.FileName);
        cmdSave.Enabled = true;
      }
    }

    private void LoadAssembly(string fileName)
    {
      tboApi.Text = "";
      _basePath = (new FileInfo(fileName)).DirectoryName;

      Assembly assembly = Assembly.ReflectionOnlyLoadFrom(fileName);
      IEnumerable<String> namespaces = assembly.GetTypes().Select(o => o.Namespace).OrderBy(o => o).Distinct();

      List<String> s = new List<String>();

      foreach (string ns in namespaces)
      {
        if (!String.IsNullOrEmpty(ns))
        {
          s.Add(LoadNamespace(assembly, ns));
        }
      }

      tboApi.Text = String.Join("\n\n", s.ToArray()).Replace("\n", "\r\n");
    }

    private string LoadNamespace(Assembly assembly, string ns)
    {
      List<String> s = new List<String>();

      AddIfNotNullOrEmpty(s, LoadEnums(assembly, ns));
      AddIfNotNullOrEmpty(s, LoadStructs(assembly, ns));
      AddIfNotNullOrEmpty(s, LoadClasses(assembly, ns));
      AddIfNotNullOrEmpty(s, LoadInterfaces(assembly, ns));

      return String.Format("namespace {0}\n{{\n{1}\n}}", ns, String.Join("\n\n", s.ToArray()));
    }

    private string LoadInterfaces(Assembly assembly, string ns)
    {
      List<String> s = new List<String>();

      foreach (Type type in assembly.GetTypes().Where(o => o.Namespace == ns && o.IsInterface && !o.IsNotPublic).OrderBy(o => o.Name))
      {
        List<String> s2 = new List<String>();

        AddIfNotNullOrEmpty(s2, LoadProperties(type, true));
        AddIfNotNullOrEmpty(s2, LoadMethods(type, true));
        string c = s2.Count > 0 ? "\n  {\n" + String.Join("\n\n", s2.ToArray()) + "\n  }" : " { }";

        string inherits = LoadInheritance(type);
        s.Add(String.Format("  public interface {0}{1}{2}", type.Name, !String.IsNullOrEmpty(inherits) ? " : " + inherits : "", c));
      }

      return String.Join("\n\n", s.ToArray());
    }

    private string LoadClasses(Assembly assembly, string ns)
    {
      List<String> s = new List<String>();

      foreach (Type type in assembly.GetTypes().Where(o => o.Namespace == ns && o.IsClass && !o.IsNotPublic && !o.Name.StartsWith("<>")).OrderBy(o => o.Name))
      {
        string m = type.IsAbstract ? "abstract " : "";
        string n = type.Name;

        if (type.IsGenericTypeDefinition)
        {
          string g = String.Join(", ", type.GetGenericArguments().Select(o => o.Name).ToArray());
          n = String.Format("{0}<{1}>", n.Substring(0, n.LastIndexOf('`')), g);
        }

        List<String> s2 = new List<String>();

        AddIfNotNullOrEmpty(s2, LoadFields(type));
        AddIfNotNullOrEmpty(s2, LoadProperties(type, false));
        AddIfNotNullOrEmpty(s2, LoadMethods(type, false));
        string c = s2.Count > 0 ? "\n  {\n" + String.Join("\n\n", s2.ToArray()) + "\n  }" : " { }";

        string inherits = LoadInheritance(type);
        s.Add(String.Format("  public {0}class {1}{2}{3}", m, n, !String.IsNullOrEmpty(inherits) ? " : " + inherits : "", c));
      }

      return String.Join("\n\n", s.ToArray());
    }

    private string LoadInheritance(Type type)
    {
      List<Type> types = new List<Type>();
      
      if (type.BaseType != null && type.BaseType.Name != "Object")
      {
        types.Add(type.BaseType);
      }

      foreach (Type iType in type.GetInterfaces().OrderBy(o => o.Name))
      {
        if (!types.Any(o => o.GetInterfaces().Contains(iType)))
        {
          types.Add(iType);
        }
      }

      return String.Join(", ", types.Select(o => ToCSharpType(o)).ToArray());
    }

    private string LoadFields(Type type)
    {
      List<String> s = new List<String>();

      foreach (FieldInfo field in type.GetFields().Where(o => o.Name != "value__").OrderBy(o => o.Name))
      {
        s.Add(String.Format("    public {0} {1};", ToCSharpType(field.FieldType), field.Name));
      }

      return String.Join("\n\n", s.ToArray());
    }

    private string LoadProperties(Type type, bool inInterface)
    {
      List<String> s = new List<String>();

      foreach (PropertyInfo prop in type.GetProperties().OrderBy(o => o.Name))
      {
        string m = !inInterface ? "public " : "";

        MethodInfo method = prop.GetGetMethod();
        string a = method != null && method.IsPublic ? "get; " : "";
        method = prop.GetSetMethod();
        a += method != null && method.IsPublic ? "set; " : "";

        s.Add(String.Format("    {0}{1} {2} {{ {3}}}", m, ToCSharpType(prop.PropertyType), prop.Name, a));
      }

      return String.Join("\n\n", s.ToArray());
    }

    private string LoadMethods(Type type, bool inInterface)
    {
      List<String> s = new List<String>();

      foreach (MethodInfo method in type.GetMethods().OrderBy(o => o.Name))
      {
        if (method.DeclaringType == type && !method.Name.StartsWith("get_") && !method.Name.StartsWith("set_"))
        {
          string m = !inInterface ? "public " : "";
          m += !inInterface && method.IsAbstract ? "abstract " : "";
          m += !inInterface && method.IsStatic ? "static " : "";
          m += !inInterface && !method.IsAbstract && !method.IsFinal && method.IsVirtual ? "virtual " : "";

          List<String> s2 = new List<String>();

          foreach (ParameterInfo param in method.GetParameters())
          {
            s2.Add(String.Format("{0} {1}", ToCSharpType(param.ParameterType), param.Name));
          }

          s.Add(String.Format("    {0}{1} {2}({3});", m, ToCSharpType(method.ReturnType), method.Name, String.Join(", ", s2.ToArray())));
        }
      }

      return String.Join("\n\n", s.ToArray());
    }

    private string LoadEnums(Assembly assembly, string ns)
    {
      List<String> s = new List<String>();

      foreach (Type type in assembly.GetTypes().Where(o => o.Namespace == ns && o.IsEnum && !o.IsNotPublic).OrderBy(o => o.Name))
      {
        s.Add(String.Format("  public enum {0}\n  {{\n    {1}\n  }}", type.Name, (String.Join(",\n    ", Enum.GetNames(type).OrderBy(o => o).ToArray()))));
      }

      return String.Join("\n\n", s.ToArray());
    }

    private string LoadStructs(Assembly assembly, string ns)
    {
      List<String> s = new List<String>();

      foreach (Type type in assembly.GetTypes().Where(o => o.Namespace == ns && o.IsValueType && !o.IsEnum && !o.IsNotPublic).OrderBy(o => o.Name))
      {
        string n = type.Name;

        if (type.IsGenericTypeDefinition)
        {
          string g = String.Join(", ", type.GetGenericArguments().Select(o => o.Name).ToArray());
          n = String.Format("{0}<{1}>", n.Substring(0, n.LastIndexOf('`')), g);
        }

        List<String> s2 = new List<String>();

        AddIfNotNullOrEmpty(s2, LoadFields(type));
        AddIfNotNullOrEmpty(s2, LoadProperties(type, false));
        AddIfNotNullOrEmpty(s2, LoadMethods(type, false));
        string c = s2.Count > 0 ? "\n  {\n" + String.Join("\n\n", s2.ToArray()) + "\n  }" : " { }";

        s.Add(String.Format("  public struct {0}{1}", n, c));
      }

      return String.Join("\n\n", s.ToArray());
    }

    private void AddIfNotNullOrEmpty(List<String> s, string t)
    {
      if (!String.IsNullOrEmpty(t))
      {
        s.Add(t);
      }
    }

    private string ToCSharpType(Type t)
    {
      return ToCSharpType(t, false);
    }

    private string ToCSharpType(Type t, bool inGeneric)
    {
      string n = t.Name;

      if (t.IsGenericType)
      {
        if (t.GetGenericTypeDefinition().Name == "Nullable`1")
        {
          return ToCSharpType(t.GetGenericArguments()[0], false) + "?";
        }
        else
        {
          List<String> s = new List<String>();

          foreach (Type a in t.GetGenericArguments())
          {
            s.Add(ToCSharpType(a, true));
          }

          if (n.Contains('`'))
          {
            n = n.Substring(0, t.Name.LastIndexOf('`'));
          }

          return String.Format("{0}<{1}>", n, String.Join(", ", s.ToArray()));
        }
      }
      else
      {
        if (!inGeneric)
        {
          string arr = "[]";

          if (!n.EndsWith(arr))
          {
            arr = "";
          }
          else
          {
            n = n.Substring(0, n.Length - 2);
          }

          switch (n)
          {
            case "Boolean": n = "bool"; break;
            case "Byte": n = "byte"; break;
            case "Double": n = "double"; break;
            case "Int16": n = "short"; break;
            case "Int32": n = "int"; break;
            case "Int64": n = "long"; break;
            case "Object": n = "object"; break;
            case "Single": n = "float"; break;
            case "String": n = "string"; break;
            case "Void": n = "void"; break;
          }

          n += arr;
        }

        return n;
      }
    }

    private void Form1_Load(object sender, EventArgs e)
    {
      AppDomain.CurrentDomain.ReflectionOnlyAssemblyResolve += new ResolveEventHandler(CurrentDomain_ReflectionOnlyAssemblyResolve);
    }

    private static Assembly CurrentDomain_ReflectionOnlyAssemblyResolve(object sender, ResolveEventArgs args)
    {
      string assemblyName = args.Name.Split(',')[0];

      if (assemblyName == "System" || assemblyName.StartsWith("System."))
      {
        return Assembly.ReflectionOnlyLoad(args.Name);
      }
      else
      {
        return Assembly.ReflectionOnlyLoadFrom(String.Format("{0}\\{1}.dll", _basePath, assemblyName));
      }
    }

    private void cmdSave_Click(object sender, EventArgs e)
    {
      SaveFileDialog dialog = new SaveFileDialog();
      dialog.Filter = "Text file (*.txt)|*.txt";
      dialog.OverwritePrompt = true;
      DialogResult result = dialog.ShowDialog();

      if (result == DialogResult.OK)
      {
        File.WriteAllText(dialog.FileName, tboApi.Text.Replace("\r\n", "\n"));
      }
    }
  }
}
