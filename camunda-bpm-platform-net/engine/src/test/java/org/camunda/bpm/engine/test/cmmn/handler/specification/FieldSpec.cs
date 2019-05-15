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
namespace org.camunda.bpm.engine.test.cmmn.handler.specification
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertEquals;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertNotNull;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertTrue;

	using Expression = org.camunda.bpm.engine.@delegate.Expression;
	using FieldDeclaration = org.camunda.bpm.engine.impl.bpmn.parser.FieldDeclaration;
	using CmmnModelInstance = org.camunda.bpm.model.cmmn.CmmnModelInstance;
	using CamundaCaseExecutionListener = org.camunda.bpm.model.cmmn.instance.camunda.CamundaCaseExecutionListener;
	using CamundaExpression = org.camunda.bpm.model.cmmn.instance.camunda.CamundaExpression;
	using CamundaField = org.camunda.bpm.model.cmmn.instance.camunda.CamundaField;
	using CamundaString = org.camunda.bpm.model.cmmn.instance.camunda.CamundaString;

	public class FieldSpec
	{

	  protected internal string fieldName;
	  protected internal string expression;
	  protected internal string childExpression;
	  protected internal string stringValue;
	  protected internal string childStringValue;

	  public FieldSpec(string fieldName, string expression, string childExpression, string stringValue, string childStringValue)
	  {
		this.fieldName = fieldName;
		this.expression = expression;
		this.childExpression = childExpression;
		this.stringValue = stringValue;
		this.childStringValue = childStringValue;
	  }

	  public virtual void verify(FieldDeclaration field)
	  {
		assertEquals(fieldName, field.Name);

		object fieldValue = field.Value;
		assertNotNull(fieldValue);

		assertTrue(fieldValue is Expression);
		Expression expressionValue = (Expression) fieldValue;
		assertEquals(ExpectedExpression, expressionValue.ExpressionText);
	  }

	  public virtual void addFieldToListenerElement(CmmnModelInstance modelInstance, CamundaCaseExecutionListener listenerElement)
	  {
		CamundaField field = SpecUtil.createElement(modelInstance, listenerElement, null, typeof(CamundaField));
		field.CamundaName = fieldName;

		if (!string.ReferenceEquals(expression, null))
		{
		  field.CamundaExpression = expression;

		}
		else if (!string.ReferenceEquals(childExpression, null))
		{
		  CamundaExpression fieldExpressionChild = SpecUtil.createElement(modelInstance, field, null, typeof(CamundaExpression));
		  fieldExpressionChild.TextContent = childExpression;

		}
		else if (!string.ReferenceEquals(stringValue, null))
		{
		  field.CamundaStringValue = stringValue;

		}
		else if (!string.ReferenceEquals(childStringValue, null))
		{
		  CamundaString fieldExpressionChild = SpecUtil.createElement(modelInstance, field, null, typeof(CamundaString));
		  fieldExpressionChild.TextContent = childStringValue;
		}
	  }

	  protected internal virtual string ExpectedExpression
	  {
		  get
		  {
			if (!string.ReferenceEquals(expression, null))
			{
			  return expression;
			}
			else if (!string.ReferenceEquals(childExpression, null))
			{
			  return childExpression;
			}
			else if (!string.ReferenceEquals(stringValue, null))
			{
			  return stringValue;
			}
			else
			{
			  return childStringValue;
			}
		  }
	  }

	}

}