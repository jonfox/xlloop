package org.boris.xlloop;

import org.boris.xlloop.handler.FunctionInformation;

public class FunctionInformationTest
{
    public static void main(String[] args) {
        FunctionInformation fi = new FunctionInformation("Math.pow");
        fi.setFunctionHelp("Raises the first value to the power of the second");
        fi.setCategory("Maths");
        fi.addArgument("value", "The first value");
        fi.addArgument("power", "The power value");
        System.out.println(fi.encode());
    }
}
