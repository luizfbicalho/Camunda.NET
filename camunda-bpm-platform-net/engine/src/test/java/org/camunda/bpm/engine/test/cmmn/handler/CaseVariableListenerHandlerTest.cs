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
namespace org.camunda.bpm.engine.test.cmmn.handler
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertEquals;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertTrue;

	using CaseVariableListener = org.camunda.bpm.engine.@delegate.CaseVariableListener;
	using Expression = org.camunda.bpm.engine.@delegate.Expression;
	using VariableListener = org.camunda.bpm.engine.@delegate.VariableListener;
	using CaseTaskItemHandler = org.camunda.bpm.engine.impl.cmmn.handler.CaseTaskItemHandler;
	using CmmnActivity = org.camunda.bpm.engine.impl.cmmn.model.CmmnActivity;
	using ClassDelegateCaseVariableListener = org.camunda.bpm.engine.impl.variable.listener.ClassDelegateCaseVariableListener;
	using DelegateExpressionCaseVariableListener = org.camunda.bpm.engine.impl.variable.listener.DelegateExpressionCaseVariableListener;
	using ExpressionCaseVariableListener = org.camunda.bpm.engine.impl.variable.listener.ExpressionCaseVariableListener;
	using SpecUtil = org.camunda.bpm.engine.test.cmmn.handler.specification.SpecUtil;
	using CaseTask = org.camunda.bpm.model.cmmn.instance.CaseTask;
	using ExtensionElements = org.camunda.bpm.model.cmmn.instance.ExtensionElements;
	using PlanItem = org.camunda.bpm.model.cmmn.instance.PlanItem;
	using CamundaField = org.camunda.bpm.model.cmmn.instance.camunda.CamundaField;
	using CamundaVariableListener = org.camunda.bpm.model.cmmn.instance.camunda.CamundaVariableListener;
	using Assert = org.junit.Assert;
	using Before = org.junit.Before;
	using Test = org.junit.Test;

	/// <summary>
	/// @author Thorben Lindhauer
	/// 
	/// </summary>
	public class CaseVariableListenerHandlerTest : CmmnElementHandlerTest
	{

	  protected internal CaseTask caseTask;
	  protected internal PlanItem planItem;
	  protected internal CaseTaskItemHandler handler = new CaseTaskItemHandler();

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void setUp()
	  public virtual void setUp()
	  {
		caseTask = createElement(casePlanModel, "aCaseTask", typeof(CaseTask));

		planItem = createElement(casePlanModel, "PI_aCaseTask", typeof(PlanItem));
		planItem.Definition = caseTask;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testClassDelegateHandling()
	  public virtual void testClassDelegateHandling()
	  {
		ExtensionElements extensionElements = SpecUtil.createElement(modelInstance, caseTask, null, typeof(ExtensionElements));
		CamundaVariableListener variableListener = SpecUtil.createElement(modelInstance, extensionElements, null, typeof(CamundaVariableListener));
		CamundaField field = SpecUtil.createElement(modelInstance, variableListener, null, typeof(CamundaField));
		field.CamundaName = "fieldName";
		field.CamundaStringValue = "a string value";

		variableListener.CamundaClass = "a.class.Name";

		// when
		CmmnActivity activity = handler.handleElement(planItem, context);

//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: java.util.List<org.camunda.bpm.engine.delegate.VariableListener<?>> listeners = activity.getVariableListenersLocal(org.camunda.bpm.engine.delegate.CaseVariableListener_Fields.CREATE);
		IList<VariableListener<object>> listeners = activity.getVariableListenersLocal(org.camunda.bpm.engine.@delegate.CaseVariableListener_Fields.CREATE);
		Assert.assertEquals(1, listeners.Count);

		ClassDelegateCaseVariableListener listener = (ClassDelegateCaseVariableListener) listeners[0];
		Assert.assertEquals("a.class.Name", listener.ClassName);
		Assert.assertEquals(1, listener.FieldDeclarations.Count);
		Assert.assertEquals("fieldName", listener.FieldDeclarations[0].Name);
		object fieldValue = listener.FieldDeclarations[0].Value;
		assertTrue(fieldValue is Expression);
		Expression expressionValue = (Expression) fieldValue;
		assertEquals("a string value", expressionValue.ExpressionText);

		Assert.assertEquals(listener, activity.getVariableListenersLocal(org.camunda.bpm.engine.@delegate.CaseVariableListener_Fields.UPDATE)[0]);
		Assert.assertEquals(listener, activity.getVariableListenersLocal(org.camunda.bpm.engine.@delegate.CaseVariableListener_Fields.DELETE)[0]);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDelegateExpressionDelegateHandling()
	  public virtual void testDelegateExpressionDelegateHandling()
	  {
		ExtensionElements extensionElements = SpecUtil.createElement(modelInstance, caseTask, null, typeof(ExtensionElements));
		CamundaVariableListener variableListener = SpecUtil.createElement(modelInstance, extensionElements, null, typeof(CamundaVariableListener));
		variableListener.CamundaDelegateExpression = "${expression}";
		variableListener.CamundaEvent = org.camunda.bpm.engine.@delegate.CaseVariableListener_Fields.CREATE;

		// when
		CmmnActivity activity = handler.handleElement(planItem, context);

//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: java.util.List<org.camunda.bpm.engine.delegate.VariableListener<?>> listeners = activity.getVariableListenersLocal(org.camunda.bpm.engine.delegate.CaseVariableListener_Fields.CREATE);
		IList<VariableListener<object>> listeners = activity.getVariableListenersLocal(org.camunda.bpm.engine.@delegate.CaseVariableListener_Fields.CREATE);
		Assert.assertEquals(1, listeners.Count);

		DelegateExpressionCaseVariableListener listener = (DelegateExpressionCaseVariableListener) listeners[0];
		Assert.assertEquals("${expression}", listener.ExpressionText);

		Assert.assertEquals(0, activity.getVariableListenersLocal(org.camunda.bpm.engine.@delegate.CaseVariableListener_Fields.UPDATE).Count);
		Assert.assertEquals(0, activity.getVariableListenersLocal(org.camunda.bpm.engine.@delegate.CaseVariableListener_Fields.DELETE).Count);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testExpressionDelegateHandling()
	  public virtual void testExpressionDelegateHandling()
	  {
		ExtensionElements extensionElements = SpecUtil.createElement(modelInstance, caseTask, null, typeof(ExtensionElements));
		CamundaVariableListener variableListener = SpecUtil.createElement(modelInstance, extensionElements, null, typeof(CamundaVariableListener));
		variableListener.CamundaExpression = "${expression}";
		variableListener.CamundaEvent = org.camunda.bpm.engine.@delegate.CaseVariableListener_Fields.CREATE;

		// when
		CmmnActivity activity = handler.handleElement(planItem, context);

//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: java.util.List<org.camunda.bpm.engine.delegate.VariableListener<?>> listeners = activity.getVariableListenersLocal(org.camunda.bpm.engine.delegate.CaseVariableListener_Fields.CREATE);
		IList<VariableListener<object>> listeners = activity.getVariableListenersLocal(org.camunda.bpm.engine.@delegate.CaseVariableListener_Fields.CREATE);
		Assert.assertEquals(1, listeners.Count);

		ExpressionCaseVariableListener listener = (ExpressionCaseVariableListener) listeners[0];
		Assert.assertEquals("${expression}", listener.ExpressionText);

		Assert.assertEquals(0, activity.getVariableListenersLocal(org.camunda.bpm.engine.@delegate.CaseVariableListener_Fields.UPDATE).Count);
		Assert.assertEquals(0, activity.getVariableListenersLocal(org.camunda.bpm.engine.@delegate.CaseVariableListener_Fields.DELETE).Count);
	  }

	}

}