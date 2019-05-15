﻿using System.Collections.Generic;

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
namespace org.camunda.bpm.engine.test.bpmn.servicetask
{

	using PluggableProcessEngineTestCase = org.camunda.bpm.engine.impl.test.PluggableProcessEngineTestCase;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using ValueBean = org.camunda.bpm.engine.test.bpmn.servicetask.util.ValueBean;

	/// <summary>
	/// @author Christian Stettler
	/// </summary>
	public class ExpressionServiceTaskTest : PluggableProcessEngineTestCase
	{

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testSetServiceResultToProcessVariables()
	  public virtual void testSetServiceResultToProcessVariables()
	  {
		IDictionary<string, object> variables = new Dictionary<string, object>();
		variables["bean"] = new ValueBean("ok");
		ProcessInstance pi = runtimeService.startProcessInstanceByKey("setServiceResultToProcessVariables", variables);
		assertEquals("ok", runtimeService.getVariable(pi.Id, "result"));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testBackwardsCompatibleExpression()
	  public virtual void testBackwardsCompatibleExpression()
	  {
		IDictionary<string, object> variables = new Dictionary<string, object>();
		variables["var"] = "---";
		ProcessInstance pi = runtimeService.startProcessInstanceByKey("BackwardsCompatibleExpressionProcess", variables);
		assertEquals("...---...", runtimeService.getVariable(pi.Id, "result"));
	  }
	}

}