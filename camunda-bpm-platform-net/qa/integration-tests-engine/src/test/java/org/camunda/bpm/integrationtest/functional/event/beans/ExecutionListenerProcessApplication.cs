﻿/*
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
namespace org.camunda.bpm.integrationtest.functional.@event.beans
{
	using ProcessApplication = org.camunda.bpm.application.ProcessApplication;
	using ServletProcessApplication = org.camunda.bpm.application.impl.ServletProcessApplication;
	using DelegateExecution = org.camunda.bpm.engine.@delegate.DelegateExecution;
	using ExecutionListener = org.camunda.bpm.engine.@delegate.ExecutionListener;

	/// <summary>
	/// @author Daniel Meyer
	/// 
	/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @ProcessApplication public class ExecutionListenerProcessApplication extends org.camunda.bpm.application.impl.ServletProcessApplication
	public class ExecutionListenerProcessApplication : ServletProcessApplication
	{

	  public const string LISTENER_INVOCATION_COUNT = "listenerInvocationCount";

	  public override ExecutionListener ExecutionListener
	  {
		  get
		  {
			return new ExecutionListenerAnonymousInnerClass(this);
		  }
	  }

	  private class ExecutionListenerAnonymousInnerClass : ExecutionListener
	  {
		  private readonly ExecutionListenerProcessApplication outerInstance;

		  public ExecutionListenerAnonymousInnerClass(ExecutionListenerProcessApplication outerInstance)
		  {
			  this.outerInstance = outerInstance;
		  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void notify(org.camunda.bpm.engine.delegate.DelegateExecution execution) throws Exception
		  public void notify(DelegateExecution execution)
		  {

			int listenerInvocationCount = 0;

			if (execution.hasVariable(LISTENER_INVOCATION_COUNT))
			{
			  listenerInvocationCount = (int?) execution.getVariable(LISTENER_INVOCATION_COUNT).Value;
			}

			listenerInvocationCount += 1;

			execution.setVariable(LISTENER_INVOCATION_COUNT, listenerInvocationCount);
		  }
	  }

	}

}