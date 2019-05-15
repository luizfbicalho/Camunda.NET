using System.Collections.Generic;

/*
 * Copyright Camunda Services GmbH and/or licensed to Camunda Services GmbH
 * under one or more contributor license agreements. See the NOTICE file
 * distributed with this work for additional information regarding copyright
 * ownership. Camunda licenses this file to you under the Apache License,
 * Version 2.0; you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
namespace org.camunda.bpm.engine.test.cmmn.listener
{

	using CaseVariableListener = org.camunda.bpm.engine.@delegate.CaseVariableListener;
	using DelegateCaseVariableInstance = org.camunda.bpm.engine.@delegate.DelegateCaseVariableInstance;
	using Expression = org.camunda.bpm.engine.@delegate.Expression;

	/// <summary>
	/// @author Thorben Lindhauer
	/// 
	/// </summary>
	public class LogInjectedValuesListener : CaseVariableListener
	{

	  protected internal Expression stringValueExpression;
	  protected internal Expression juelExpression;

	  protected internal static IList<object> resolvedStringValueExpressions = new List<object>();
	  protected internal static IList<object> resolvedJuelExpressions = new List<object>();


//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void notify(org.camunda.bpm.engine.delegate.DelegateCaseVariableInstance variableInstance) throws Exception
	  public virtual void notify(DelegateCaseVariableInstance variableInstance)
	  {
		resolvedJuelExpressions.Add(juelExpression.getValue(variableInstance.SourceExecution));
		resolvedStringValueExpressions.Add(stringValueExpression.getValue(variableInstance.SourceExecution));
	  }

	  public static IList<object> ResolvedStringValueExpressions
	  {
		  get
		  {
			return resolvedStringValueExpressions;
		  }
	  }

	  public static IList<object> ResolvedJuelExpressions
	  {
		  get
		  {
			return resolvedJuelExpressions;
		  }
	  }

	  public static void reset()
	  {
		resolvedJuelExpressions = new List<object>();
		resolvedStringValueExpressions = new List<object>();
	  }

	}

}