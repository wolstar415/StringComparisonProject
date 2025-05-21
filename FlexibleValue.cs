using System;

public readonly struct FlexibleValue
{
    public enum ValueType : byte { None, Integer, Float, Boolean, String }

    public static readonly FlexibleValue None;

    public readonly ValueType Type;
    public readonly int Int;
    public readonly float Float;
    public readonly string Text;

    public bool IsValid => Type != ValueType.None;
    public bool IsNumber => Type is ValueType.Integer or ValueType.Float;
    public bool IsInteger => Type == ValueType.Integer;
    public bool IsFloat => Type == ValueType.Float;
    public bool IsBoolean => Type == ValueType.Boolean;
    public bool IsString => Type == ValueType.String;

    public static implicit operator FlexibleValue(int v) => new(ValueType.Integer, v, 0f, null);
    public static implicit operator FlexibleValue(float v) => new(ValueType.Float, 0, v, null);
    public static implicit operator FlexibleValue(bool v) => new(ValueType.Boolean, v ? 1 : 0, 0f, null);
    public static implicit operator FlexibleValue(string v) => new(ValueType.String, 0, 0f, v);
    public static implicit operator FlexibleValue(Enum e) => new(ValueType.String, 0, 0f, e.ToString());

    private FlexibleValue(ValueType type, int i, float f, string s)
    {
        Type = type;
        Int = i;
        Float = f;
        Text = s;
    }

    public int AsInt()
    {
        return Type switch
        {
            ValueType.Integer or ValueType.Boolean => Int,
            ValueType.Float => (int)Float,
            ValueType.String => int.TryParse(Text, out var i) ? i : 0,
            _ => 0
        };
    }

    public float AsFloat() => Type switch
    {
        ValueType.Float => Float,
        ValueType.Integer => Int,
        ValueType.String => float.TryParse(Text, out var f) ? f : 0f,
        _ => 0f
    };

    public bool AsBool()
    {
        return Type switch
        {
            ValueType.Boolean or ValueType.Integer => Int != 0,
            ValueType.String => !string.IsNullOrWhiteSpace(Text) && Text != "0" && !Text.Equals("false", StringComparison.OrdinalIgnoreCase),
            _ => false
        };
    }

    public override string ToString() => Type switch
    {
        ValueType.Integer => Int.ToString(),
        ValueType.Float => Float.ToString("0.###"),
        ValueType.Boolean => Int != 0 ? "true" : "false",
        ValueType.String => Text,
        _ => "(none)"
    };

    public bool EqualsTo(in FlexibleValue other)
    {
        if (Type != other.Type) return false;

        return Type switch
        {
            ValueType.Boolean or ValueType.Integer => Int == other.Int,
            ValueType.Float => Math.Abs(Float - other.Float) < 0.00001f,
            ValueType.String => string.Equals(Text, other.Text),
            _ => false
        };
    }

    public static FlexibleValue Parse(string input)
    {
        if (string.IsNullOrWhiteSpace(input)) return "";
        if (int.TryParse(input, out var i)) return i;
        if (float.TryParse(input, out var f)) return f;
        if (input.Equals("true", StringComparison.OrdinalIgnoreCase)) return true;
        if (input.Equals("false", StringComparison.OrdinalIgnoreCase)) return false;
        return input;
    }
}
