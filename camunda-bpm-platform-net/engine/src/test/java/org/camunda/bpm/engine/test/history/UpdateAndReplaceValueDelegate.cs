using System;
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
namespace org.camunda.bpm.engine.test.history
{

	using DelegateExecution = org.camunda.bpm.engine.@delegate.DelegateExecution;
	using JavaDelegate = org.camunda.bpm.engine.@delegate.JavaDelegate;

	/// <summary>
	/// @author Thorben Lindhauer
	/// 
	/// </summary>
	[Serializable]
	public class UpdateAndReplaceValueDelegate : JavaDelegate
	{

	  private const long serialVersionUID = 1L;

	  public const string NEW_ELEMENT = "new element";

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void execute(org.camunda.bpm.engine.delegate.DelegateExecution execution) throws Exception
	  public virtual void execute(DelegateExecution execution)
	  {
		IList<string> list = (IList<string>) execution.getVariable("listVar");

		// implicitly update the previous list, should update the variable value
		list.Add(NEW_ELEMENT);

		// replace the list by another object
		execution.setVariable("listVar", new List<string>());

		// note that this is the condensed form of more realistic scenarios like
		// an implicit update in task 1 and an explicit update in the following task 2,
		// both in the same transaction.
	  }

	}

}