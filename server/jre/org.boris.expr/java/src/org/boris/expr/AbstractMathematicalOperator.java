/*******************************************************************************
 * This program and the accompanying materials
 * are made available under the terms of the Common Public License v1.0
 * which accompanies this distribution, and is available at 
 * http://www.eclipse.org/legal/cpl-v10.html
 * 
 * Contributors:
 *     Peter Smith
 *******************************************************************************/
package org.boris.expr;

public abstract class AbstractMathematicalOperator extends
        AbstractBinaryOperator
{
    private Expr[] args;
    
    public AbstractMathematicalOperator(ExprType type, Expr lhs, Expr rhs) {
        super(type, lhs, rhs);
        args = new Expr[] { lhs, rhs };
    }
    
    public Expr optimize() throws ExprException {
        lhs = lhs == null ? ExprDouble.ZERO : lhs.optimize();
        rhs = rhs.optimize();
        if(lhs instanceof ExprNumber && rhs instanceof ExprNumber) {
            return evaluate();
        }
        return this;
    }

    protected double evaluateExpr(Expr e) throws ExprException {
        e = eval(e);
        if (e == null)
            return 0;
        if (e instanceof ExprMissing)
            return 0;
        ExprTypes.assertType(e, ExprType.Integer, ExprType.Double);
        return ((ExprNumber) e).doubleValue();
    }

    public Expr evaluate() throws ExprException {
        Expr l = eval(lhs);
        if (l instanceof ExprError) {
            return l;
        }
        Expr r = eval(rhs);
        if (r instanceof ExprError) {
            return r;
        }
        return evaluate(evaluateExpr(l), evaluateExpr(r));
    }
    
    public Expr[] getArgs() {
        return args;
    }

    protected abstract Expr evaluate(double lhs, double rhs)
            throws ExprException;
}
