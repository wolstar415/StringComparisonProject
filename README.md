# StringComparisonProject


        // Numeric comparisons
        var cmp1 = Comparison.Parse("<=30");
        int num1 = 25;
        Debug.Log($"25 <= 30 ? {cmp1.Check(num1)}");             // ✅ true
        
        var cmp2 = Comparison.Parse(">100");
        int num2 = 120;
        Debug.Log($"120 > 100 ? {cmp2.Check(num2)}");            // ✅ true
        
        var cmp3 = Comparison.Parse("==10");
        int num3 = 10;
        Debug.Log($"10 == 10 ? {cmp3.Check(num3)}");             // ✅ true
        
        var cmp4 = Comparison.Parse("!=50");
        int num4 = 49;
        Debug.Log($"49 != 50 ? {cmp4.Check(num4)}");             // ✅ true
        
        var cmp5 = Comparison.Parse("<5");
        int num5 = 5;
        Debug.Log($"5 < 5 ? {cmp5.Check(num5)}");                // ❌ false
        
        var cmp6 = Comparison.Parse(">=7");
        int num6 = 3;
        Debug.Log($"3 >= 7 ? {cmp6.Check(num6)}");               // ❌ false
        
        // Floating-point comparisons
        var cmp7 = Comparison.Parse("==0.5");
        float f1 = 0.5f;
        Debug.Log($"0.5 == 0.5 ? {cmp7.Check(f1)}");             // ✅ true
        
        var cmp8 = Comparison.Parse(">=1.5");
        float f2 = 1.6f;
        Debug.Log($"1.6 >= 1.5 ? {cmp8.Check(f2)}");             // ✅ true
        
        var cmp9 = Comparison.Parse("1.0<x<=2.0");
        float f3 = 1.5f;
        float f4 = 2.0f;
        Debug.Log($"1.5 in 1.0<x<=2.0 ? {cmp9.Check(f3)}");      // ✅ true
        Debug.Log($"2.0 in 1.0<x<=2.0 ? {cmp9.Check(f4)}");      // ✅ true
        
        var cmp10 = Comparison.Parse("1.0<x<2.0");
        float f5 = 2.0f;
        Debug.Log($"2.0 in 1.0<x<2.0 ? {cmp10.Check(f5)}");      // ❌ false
        
        // String comparisons
        var cmp11 = Comparison.Parse("==Elite");
        string str1 = "Elite";
        Debug.Log($"Elite == Elite ? {cmp11.Check(str1)}");      // ✅ true
        
        var cmp12 = Comparison.Parse("Boss");
        string str2 = "Boss";
        Debug.Log($"Boss == Boss ? {cmp12.Check(str2)}");        // ✅ true
        
        var cmp13 = Comparison.Parse("!=Boss");
        string str3 = "Boss";
        string str4 = "Normal";
        Debug.Log($"Boss != Boss ? {cmp13.Check(str3)}");        // ❌ false
        Debug.Log($"Normal != Boss ? {cmp13.Check(str4)}");      // ✅ true
        
        var cmp14 = Comparison.Parse("==elite");
        string str5 = "Elite";
        Debug.Log($"Elite == elite ? {cmp14.Check(str5)}");      // ✅ true (case-insensitive)
        
        // Boolean comparisons
        var cmp15 = Comparison.Parse("==true");
        bool b1 = true;
        Debug.Log($"true == true ? {cmp15.Check(b1)}");          // ✅ true
        
        var cmp16 = Comparison.Parse("!=false");
        bool b2 = true;
        Debug.Log($"true != false ? {cmp16.Check(b2)}");         // ✅ true
        
        var cmp17 = Comparison.Parse("==false");
        bool b3 = true;
        Debug.Log($"true == false ? {cmp17.Check(b3)}");         // ❌ false
        
        // Range and special cases
        var cmp18 = Comparison.Parse("3<x<6");
        int r1 = 4;
        int r2 = 6;
        Debug.Log($"4 in 3<x<6 ? {cmp18.Check(r1)}");            // ✅ true
        Debug.Log($"6 in 3<x<6 ? {cmp18.Check(r2)}");            // ❌ false
        
        var cmp19 = Comparison.Parse("3<=x<=6");
        int r3 = 3;
        int r4 = 6;
        Debug.Log($"3 in 3<=x<=6 ? {cmp19.Check(r3)}");          // ✅ true
        Debug.Log($"6 in 3<=x<=6 ? {cmp19.Check(r4)}");          // ✅ true
        
        var cmp20 = Comparison.Parse("");
        int emptyCase = 0;
        Debug.Log($"0 == None ? {cmp20.Check(emptyCase)}");      // ❌ false
        
        var cmp21 = Comparison.Parse("!Boss");
        string notCase1 = "Boss";
        string notCase2 = "Elite";
        Debug.Log($"Boss != Boss ? {cmp21.Check(notCase1)}");    // ❌ false
        Debug.Log($"Elite != Boss ? {cmp21.Check(notCase2)}");   // ✅ true
        
        // FlexibleValue.None or invalid
        var cmp22 = Comparison.Parse(">10");
        FlexibleValue noneValue = FlexibleValue.None;
        Debug.Log($"None > 10 ? {cmp22.Check(noneValue)}");      // ❌ false

