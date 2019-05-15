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
namespace org.camunda.bpm.engine.impl.cmmn.operation
{

	using CmmnExecution = org.camunda.bpm.engine.impl.cmmn.execution.CmmnExecution;

	/// <summary>
	/// @author Roman Smirnov
	/// 
	/// </summary>
	public class AtomicOperationCaseExecutionDeleteCascade : CmmnAtomicOperation
	{

	  public virtual string CanonicalName
	  {
		  get
		  {
			return "delete-cascade";
		  }
	  }

	 protected internal virtual CmmnExecution findFirstLeaf(CmmnExecution execution)
	 {
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: java.util.List<? extends org.camunda.bpm.engine.impl.cmmn.execution.CmmnExecution> executions = execution.getCaseExecutions();
	   IList<CmmnExecution> executions = execution.CaseExecutions;

	   if (executions.Count > 0)
	   {
		 return findFirstLeaf(executions[0]);
	   }
	   return execution;
	 }

	  public virtual void execute(CmmnExecution execution)
	  {
		CmmnExecution firstLeaf = findFirstLeaf(execution);

		firstLeaf.remove();

		CmmnExecution parent = firstLeaf.Parent;
		if (parent != null)
		{
		  parent.deleteCascade();
		}
	  }

	  public virtual bool isAsync(CmmnExecution execution)
	  {
		return false;
	  }

	}

}