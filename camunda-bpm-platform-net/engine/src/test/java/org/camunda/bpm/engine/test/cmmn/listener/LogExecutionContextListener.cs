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
	using CaseExecutionContext = org.camunda.bpm.engine.impl.context.CaseExecutionContext;
	using Context = org.camunda.bpm.engine.impl.context.Context;

	/// <summary>
	/// @author Thorben Lindhauer
	/// 
	/// </summary>
	public class LogExecutionContextListener : CaseVariableListener
	{

	  protected internal static IList<CaseExecutionContext> executionContexts = new List<CaseExecutionContext>();


//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void notify(org.camunda.bpm.engine.delegate.DelegateCaseVariableInstance variableInstance) throws Exception
	  public virtual void notify(DelegateCaseVariableInstance variableInstance)
	  {
		executionContexts.Add(Context.CaseExecutionContext);
	  }

	  public static IList<CaseExecutionContext> CaseExecutionContexts
	  {
		  get
		  {
			return executionContexts;
		  }
	  }

	  public static void reset()
	  {
		executionContexts = new List<CaseExecutionContext>();
	  }

	}

}