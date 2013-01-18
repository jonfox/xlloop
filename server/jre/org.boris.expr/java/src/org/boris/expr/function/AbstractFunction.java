/*******************************************************************************
 * This program and the accompanying materials
 * are made available under the terms of the Common Public License v1.0
 * which accompanies this distribution, and is available at 
 * http://www.eclipse.org/legal/cpl-v10.html
 * 
 * Contributors:
 *     Peter Smith
 *******************************************************************************/
package org.boris.expr.function;

import org.boris.expr.Expr;
import org.boris.expr.ExprArray;
import org.boris.expr.ExprBoolean;
import org.boris.expr.ExprDouble;
import org.boris.expr.ExprEvaluatable;
import org.boris.expr.ExprException;
import org.boris.expr.ExprInteger;
import org.boris.expr.ExprMissing;
import org.boris.expr.ExprNumber;
import org.boris.expr.ExprString;
import org.boris.expr.ExprType;
import org.boris.expr.IExprFunction;

public abstract class AbstractFunction implements IExprFunction
{
    public boolean isVolatile() {
        return false;
    }

    protected void assertArgCount(Expr[] args, int count) throws ExprException {
        if (args == null && count != 0) {
            throw new ExprException(getClass().getSimpleName() +
                    " function takes no arguments");
        }

        if (args.length != count)
            throw new ExprException(getClass().getSimpleName() +
                    " function takes " + count + " arguments");
    }

    protected void assertArgTypes(Expr[] args, ExprType... types)
            throws ExprException {
        assertArgCount(args, types.length);
        for (int i = 0; i < args.length; i++) {
            if (!args[i].type.equals(types[i])) {
                throw new ExprException("Invalid argument (" + i + 1 +
                        ") to function: " + getClass().getSimpleName());
            }
        }
    }

    protected double asDouble(Expr arg, boolean strict) throws ExprException {
        if (arg instanceof ExprEvaluatable) {
            arg = ((ExprEvaluatable) arg).evaluate();
        }
        if (arg instanceof ExprNumber) {
            return ((ExprNumber) arg).doubleValue();
        }
        if (!strict)
            return 0;
        throw new ExprException("Invalid argument type for function " +
                getClass().getSimpleName());
    }

    protected int asInteger(Expr arg, boolean strict) throws ExprException {
        if (arg instanceof ExprEvaluatable) {
            arg = ((ExprEvaluatable) arg).evaluate();
        }
        if (arg instanceof ExprNumber) {
            return ((ExprNumber) arg).intValue();
        }
        if (!strict)
            return 0;
        throw new ExprException("Invalid argument type for function " +
                getClass().getSimpleName());
    }

    protected boolean asBoolean(Expr arg, boolean strict) throws ExprException {
        if (arg instanceof ExprEvaluatable) {
            arg = ((ExprEvaluatable) arg).evaluate();
        }
        if (arg instanceof ExprNumber) {
            return ((ExprNumber) arg).booleanValue();
        }
        if (!strict)
            return false;
        throw new ExprException("Invalid argument type for function " +
                getClass().getSimpleName());
    }

    protected String asString(Expr arg, boolean strict) throws ExprException {
        if (arg instanceof ExprEvaluatable) {
            arg = ((ExprEvaluatable) arg).evaluate();
        }
        if (arg instanceof ExprString) {
            return ((ExprString) arg).str;
        }
        if (!strict) {
            if (arg instanceof ExprNumber) {
                return arg.toString();
            }
            return "";
        }
        throw new ExprException("Invalid argument type for function " +
                getClass().getSimpleName());
    }
    
    protected String asString(Expr[] args, int index, String defaultValue) throws ExprException {
        Expr arg = getArg(args, index);
        return arg == null ? defaultValue : arg.toString();
    }
    
    protected int asInteger(Expr[] args, int index, int defaultValue) throws ExprException {
        Expr arg = getArg(args, index);
        if(arg instanceof ExprNumber)
            return ((ExprNumber)arg).intValue();
        return defaultValue;
    }
    
    protected double asDouble(Expr[] args, int index, double defaultValue) throws ExprException {
        Expr arg = getArg(args, index);
        if(arg instanceof ExprNumber)
            return ((ExprNumber)arg).doubleValue();
        return defaultValue;
    }
    
    protected Expr getArg(Expr[] args, int index) throws ExprException {
        if(args == null || args.length <= index)
            return null;
        
        Expr arg = args[index];
        if(arg instanceof ExprArray) {
            arg = ((ExprArray)arg).get(0);
        } 
        if (arg instanceof ExprEvaluatable) {
            arg = ((ExprEvaluatable) arg).evaluate();
        }
        if(arg instanceof ExprArray) {
            arg = ((ExprArray)arg).get(0);
        }
        
        return arg;
    }

    protected void assertArgType(Expr expr, ExprType type) throws ExprException {
        if (expr == null) {
            if (type != null)
                throw new ExprException("Invalid empty argument for function " +
                        getClass().getSimpleName());

        } else {
            if (!expr.type.equals(type)) {
                throw new ExprException("Invalid argument type for function " +
                        getClass().getSimpleName());
            }
        }
    }

    protected void assertArgCount(Expr[] args, int min, int max)
            throws ExprException {
        assertMinArgCount(args, min);
        assertMaxArgCount(args, max);
    }

    protected void assertMinArgCount(Expr[] args, int count)
            throws ExprException {
        if (args.length < count)
            throw new ExprException("Too few arguments to function " +
                    getClass().getSimpleName());
    }

    protected void assertMaxArgCount(Expr[] args, int count)
            throws ExprException {
        if (args.length > count)
            throw new ExprException("Too many arguments to function " +
                    getClass().getSimpleName());
    }

    public static Expr evalArg(Expr arg) throws ExprException {
        if (arg instanceof ExprEvaluatable) {
            return ((ExprEvaluatable) arg).evaluate();
        }
        return arg;
    }

    protected int getLength(Expr range) {
        if (range instanceof ExprArray) {
            return ((ExprArray) range).length();
        } else {
            return 1;
        }
    }

    protected Expr get(Expr range, int index) {
        if (range instanceof ExprArray) {
            ExprArray a = (ExprArray) range;
            if (index >= 0 && index < a.length()) {
                return a.get(index);
            }
        } else if (index == 0) {
            return range;
        }

        return ExprMissing.MISSING;
    }

    protected ExprBoolean bool(boolean bool) {
        return bool ? ExprBoolean.TRUE : ExprBoolean.FALSE;
    }

    protected double asDouble(ExprArray knownY, int index) throws ExprException {
        if (index < 0 || index >= knownY.length())
            return 0;

        Expr e = knownY.get(index);
        return asDouble(e, false);
    }

    protected boolean isOneOf(Expr expr, ExprType... types) {
        for (ExprType t : types) {
            if (expr.type.equals(t))
                return true;
        }
        return false;
    }

    protected ExprArray asArray(Expr expr, boolean strict) throws ExprException {
        if (expr instanceof ExprEvaluatable) {
            expr = ((ExprEvaluatable) expr).evaluate();
        }

        if (expr instanceof ExprArray) {
            return (ExprArray) expr;
        }

        if (strict)
            throw new ExprException("Argument not an array for function: " +
                    getClass().getSimpleName());

        ExprArray ea = new ExprArray(1, 1);
        ea.set(0, expr);
        return ea;
    }

    protected boolean isNumber(Expr x) {
        return x instanceof ExprDouble || x instanceof ExprInteger;
    }
}
