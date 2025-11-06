using System;

namespace Dimpoc;

class Program
{
    static void Main()
    {

        var demo = new DemoData();

        Console.WriteLine("Original object:");
        Dump(demo);

        Console.WriteLine("\n--- Sanitizing ---\n");
        var san = AnnotationProcessor.Sanitize(demo);
        PrintResults(san);

        Console.WriteLine("Object after sanitize:");
        Dump(demo);

        Console.WriteLine("\n--- Validating ---\n");
        var val = AnnotationProcessor.Validate(demo);
        PrintResults(val);

        Console.WriteLine("Object after validate:");
        Dump(demo);
    }

    static void Dump(DemoData d)
    {
        Console.WriteLine($"SSN: '{d.SSN}'");
        Console.WriteLine($"Card: '{d.Card}'");
        Console.WriteLine($"CardInvalid: '{d.CardInvalid}'");
        Console.WriteLine($"Phone: '{d.Phone}'");
        Console.WriteLine($"Number: '{d.Number}'");
        Console.WriteLine($"TinNull: '{d.TinNull}'");
    }

    static void PrintResults(FieldResults r)
    {
        Console.WriteLine($"AllValid: {r.AllValid}");
        foreach (var it in r.Items)
        {
            Console.WriteLine($"- {it.FieldName}: Empty={it.Empty}, Valid={it.Valid}");
        }
    }
}
