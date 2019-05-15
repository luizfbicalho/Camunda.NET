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
namespace org.camunda.bpm.integrationtest.functional.ejb.request.beans
{

	using DelegateExecution = org.camunda.bpm.engine.@delegate.DelegateExecution;
	using JavaDelegate = org.camunda.bpm.engine.@delegate.JavaDelegate;
	using TestConstants = org.camunda.bpm.integrationtest.util.TestConstants;


	/// 
	/// <summary>
	/// @author Daniel Meyer
	/// 
	/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Named public class InvocationCounterDelegateBean implements org.camunda.bpm.engine.delegate.JavaDelegate
	public class InvocationCounterDelegateBean : JavaDelegate
	{

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void execute(org.camunda.bpm.engine.delegate.DelegateExecution execution) throws Exception
	  public virtual void execute(DelegateExecution execution)
	  {
		InvocationCounterService invocationCounterService = (InvocationCounterService) (new InitialContext()).lookup("java:global/" + TestConstants.AppName + "service/" + "InvocationCounterServiceBean!org.camunda.bpm.integrationtest.functional.ejb.request.beans.InvocationCounterService");

		execution.setVariable("invocationCounter", invocationCounterService.NumOfInvocations);
	  }

	}

}