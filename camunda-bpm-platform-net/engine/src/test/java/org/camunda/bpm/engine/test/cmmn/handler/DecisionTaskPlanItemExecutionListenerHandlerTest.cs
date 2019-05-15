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
namespace org.camunda.bpm.engine.test.cmmn.handler
{
	using DecisionTaskItemHandler = org.camunda.bpm.engine.impl.cmmn.handler.DecisionTaskItemHandler;
	using CmmnActivity = org.camunda.bpm.engine.impl.cmmn.model.CmmnActivity;
	using AbstractExecutionListenerSpec = org.camunda.bpm.engine.test.cmmn.handler.specification.AbstractExecutionListenerSpec;
	using DecisionTask = org.camunda.bpm.model.cmmn.instance.DecisionTask;
	using PlanItem = org.camunda.bpm.model.cmmn.instance.PlanItem;
	using Before = org.junit.Before;
	using Test = org.junit.Test;
	using RunWith = org.junit.runner.RunWith;
	using Parameterized = org.junit.runners.Parameterized;
	using Parameters = org.junit.runners.Parameterized.Parameters;

	/// <summary>
	/// @author Roman Smirnov
	/// 
	/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @RunWith(Parameterized.class) public class DecisionTaskPlanItemExecutionListenerHandlerTest extends CmmnElementHandlerTest
	public class DecisionTaskPlanItemExecutionListenerHandlerTest : CmmnElementHandlerTest
	{
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Parameters(name = "testListener: {0}") public static Iterable<Object[]> data()
		public static IEnumerable<object[]> data()
		{
		return ExecutionListenerCases.TASK_OR_STAGE_CASES;
		}

	  protected internal DecisionTask decisionTask;
	  protected internal PlanItem planItem;
	  protected internal DecisionTaskItemHandler handler = new DecisionTaskItemHandler();

	  protected internal AbstractExecutionListenerSpec testSpecification;

	  public DecisionTaskPlanItemExecutionListenerHandlerTest(AbstractExecutionListenerSpec testSpecification)
	  {
		this.testSpecification = testSpecification;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void setUp()
	  public virtual void setUp()
	  {
		decisionTask = createElement(casePlanModel, "aDecisionTask", typeof(DecisionTask));

		planItem = createElement(casePlanModel, "PI_aHumanTask", typeof(PlanItem));
		planItem.Definition = decisionTask;

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCaseExecutionListener()
	  public virtual void testCaseExecutionListener()
	  {
		// given:
		testSpecification.addListenerToElement(modelInstance, decisionTask);

		// when
		CmmnActivity activity = handler.handleElement(planItem, context);

		// then
		testSpecification.verify(activity);
	  }

	}

}