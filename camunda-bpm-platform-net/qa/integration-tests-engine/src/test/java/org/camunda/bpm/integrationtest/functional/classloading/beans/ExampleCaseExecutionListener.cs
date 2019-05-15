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
namespace org.camunda.bpm.integrationtest.functional.classloading.beans
{

	using CaseExecutionListener = org.camunda.bpm.engine.@delegate.CaseExecutionListener;
	using DelegateCaseExecution = org.camunda.bpm.engine.@delegate.DelegateCaseExecution;

	/// <summary>
	/// @author Roman Smirnov
	/// 
	/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Named(value = "caseExecutionListener") public class ExampleCaseExecutionListener implements org.camunda.bpm.engine.delegate.CaseExecutionListener
	public class ExampleCaseExecutionListener : CaseExecutionListener
	{

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void notify(org.camunda.bpm.engine.delegate.DelegateCaseExecution caseExecution) throws Exception
	  public virtual void notify(DelegateCaseExecution caseExecution)
	  {
		caseExecution.setVariable("listener", "listener-notified");
	  }

	}

}