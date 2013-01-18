/*******************************************************************************
 * This program and the accompanying materials
 * are made available under the terms of the Common Public License v1.0
 * which accompanies this distribution, and is available at 
 * http://www.eclipse.org/legal/cpl-v10.html
 * 
 * Contributors:
 *     Peter Smith
 *******************************************************************************/
package org.boris.xlloop;

public class GenDef
{
    public static void main(String[] args) throws Exception {
        int index = 8;
        for (int i = 0; i < 20; i++) {
            System.out.println("\tFSExecute" + i + " @" + (index++));
        }
        for (int i = 0; i < 20; i++) {
            System.out.println("\tFSExecuteVolatile" + i + " @" + (index++));
        }
        for (int i = 0; i < 512; i++) {
            System.out.println("\tFS" + i + " @" + (index++));
        }
        for (int i = 0; i < 512; i++) {
            System.out.println("\tFSC" + i + " @" + (index++));
        }
    }
}
