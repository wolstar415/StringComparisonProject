# StringComparisonProject


        var cmp1 = Comparison.Parse("<=30");
        var cmp2 = Comparison.Parse("==Elite");
        var cmp3 = Comparison.Parse("Boss");
        var cmp4 = Comparison.Parse("!=Boss");

        int value1 = 25;
        string value2 = "Boss";

        Debug.Log($"25 <= 30 ? {cmp1.Check(value1)}");      // ✅ true
        Debug.Log($"Boss == Elite ? {cmp2.Check(value2)}"); // ❌ false
        Debug.Log($"Boss == Boss ? {cmp3.Check(value2)}"); // ✅ true
        Debug.Log($"Boss != Boss ? {cmp4.Check(value2)}"); // ❌ false
