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
namespace org.camunda.bpm.engine.test.cmmn.handler.specification
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertEquals;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertTrue;

	using BaseDelegateExecution = org.camunda.bpm.engine.@delegate.BaseDelegateExecution;
	using DelegateListener = org.camunda.bpm.engine.@delegate.DelegateListener;
	using ExpressionCaseExecutionListener = org.camunda.bpm.engine.impl.cmmn.listener.ExpressionCaseExecutionListener;
	using CmmnModelInstance = org.camunda.bpm.model.cmmn.CmmnModelInstance;
	using CamundaCaseExecutionListener = org.camunda.bpm.model.cmmn.instance.camunda.CamundaCaseExecutionListener;

	public class ExpressionExecutionListenerSpec : AbstractExecutionListenerSpec
	{

	  protected internal const string EXPRESSION = "${myExpression}";

	  public ExpressionExecutionListenerSpec(string eventName) : base(eventName)
	  {
	  }

	  protected internal override void configureCaseExecutionListener(CmmnModelInstance modelInstance, CamundaCaseExecutionListener listener)
	  {
		listener.CamundaExpression = EXPRESSION;

	  }

	  public override void verifyListener<T1>(DelegateListener<T1> listener) where T1 : org.camunda.bpm.engine.@delegate.BaseDelegateExecution
	  {
		assertTrue(listener is ExpressionCaseExecutionListener);

		ExpressionCaseExecutionListener expressionListener = (ExpressionCaseExecutionListener) listener;
		assertEquals(EXPRESSION, expressionListener.ExpressionText);

	  }

	}

}