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
namespace org.camunda.bpm.engine.impl.form.handler
{
	using Expression = org.camunda.bpm.engine.@delegate.Expression;
	using VariableScope = org.camunda.bpm.engine.@delegate.VariableScope;
	using FormProperty = org.camunda.bpm.engine.form.FormProperty;
	using FormType = org.camunda.bpm.engine.form.FormType;
	using StartProcessVariableScope = org.camunda.bpm.engine.impl.el.StartProcessVariableScope;
	using AbstractFormFieldType = org.camunda.bpm.engine.impl.form.type.AbstractFormFieldType;
	using ExecutionEntity = org.camunda.bpm.engine.impl.persistence.entity.ExecutionEntity;
	using VariableMap = org.camunda.bpm.engine.variable.VariableMap;


	/// <summary>
	/// @author Tom Baeyens
	/// </summary>
	public class FormPropertyHandler
	{

	  protected internal string id;
	  protected internal string name;
	  protected internal AbstractFormFieldType type;
	  protected internal bool isReadable;
	  protected internal bool isWritable;
	  protected internal bool isRequired;
	  protected internal string variableName;
	  protected internal Expression variableExpression;
	  protected internal Expression defaultExpression;

	  public virtual FormProperty createFormProperty(ExecutionEntity execution)
	  {
		FormPropertyImpl formProperty = new FormPropertyImpl(this);
		object modelValue = null;

		if (execution != null)
		{
		  if (!string.ReferenceEquals(variableName, null) || variableExpression == null)
		  {
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String varName = variableName != null ? variableName : id;
			string varName = !string.ReferenceEquals(variableName, null) ? variableName : id;
			if (execution.hasVariable(varName))
			{
			  modelValue = execution.getVariable(varName);
			}
			else if (defaultExpression != null)
			{
			  modelValue = defaultExpression.getValue(execution);
			}
		  }
		  else
		  {
			modelValue = variableExpression.getValue(execution);
		  }
		}
		else
		{
		  // Execution is null, the form-property is used in a start-form. Default value
		  // should be available (ACT-1028) even though no execution is available.
		  if (defaultExpression != null)
		  {
			modelValue = defaultExpression.getValue(StartProcessVariableScope.SharedInstance);
		  }
		}

		if (modelValue is string)
		{
		  formProperty.Value = (string) modelValue;
		}
		else if (type != null)
		{
		  string formValue = type.convertModelValueToFormValue(modelValue);
		  formProperty.Value = formValue;
		}
		else if (modelValue != null)
		{
		  formProperty.Value = modelValue.ToString();
		}

		return formProperty;
	  }

	  public virtual void submitFormProperty(VariableScope variableScope, VariableMap variables)
	  {
		if (!isWritable && variables.containsKey(id))
		{
		  throw new ProcessEngineException("form property '" + id + "' is not writable");
		}

		if (isRequired && !variables.containsKey(id) && defaultExpression == null)
		{
		  throw new ProcessEngineException("form property '" + id + "' is required");
		}

		object modelValue = null;
		if (variables.containsKey(id))
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Object propertyValue = variables.remove(id);
		  object propertyValue = variables.remove(id);
		  if (type != null)
		  {
			modelValue = type.convertFormValueToModelValue(propertyValue);
		  }
		  else
		  {
			modelValue = propertyValue;
		  }
		}
		else if (defaultExpression != null)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Object expressionValue = defaultExpression.getValue(variableScope);
		  object expressionValue = defaultExpression.getValue(variableScope);
		  if (type != null && expressionValue != null)
		  {
			modelValue = type.convertFormValueToModelValue(expressionValue.ToString());
		  }
		  else if (expressionValue != null)
		  {
			modelValue = expressionValue.ToString();
		  }
		  else if (isRequired)
		  {
			throw new ProcessEngineException("form property '" + id + "' is required");
		  }
		}

		if (modelValue != null)
		{
		  if (!string.ReferenceEquals(variableName, null))
		  {
			variableScope.setVariable(variableName, modelValue);
		  }
		  else if (variableExpression != null)
		  {
			variableExpression.setValue(modelValue, variableScope);
		  }
		  else
		  {
			variableScope.setVariable(id, modelValue);
		  }
		}
	  }

	  // getters and setters //////////////////////////////////////////////////////

	  public virtual string Id
	  {
		  get
		  {
			return id;
		  }
		  set
		  {
			this.id = value;
		  }
	  }


	  public virtual string Name
	  {
		  get
		  {
			return name;
		  }
		  set
		  {
			this.name = value;
		  }
	  }


	  public virtual FormType getType()
	  {
		return type;
	  }

	  public virtual void setType(AbstractFormFieldType type)
	  {
		this.type = type;
	  }

	  public virtual bool Readable
	  {
		  get
		  {
			return isReadable;
		  }
		  set
		  {
			this.isReadable = value;
		  }
	  }


	  public virtual bool Required
	  {
		  get
		  {
			return isRequired;
		  }
		  set
		  {
			this.isRequired = value;
		  }
	  }


	  public virtual string VariableName
	  {
		  get
		  {
			return variableName;
		  }
		  set
		  {
			this.variableName = value;
		  }
	  }


	  public virtual Expression VariableExpression
	  {
		  get
		  {
			return variableExpression;
		  }
		  set
		  {
			this.variableExpression = value;
		  }
	  }


	  public virtual Expression DefaultExpression
	  {
		  get
		  {
			return defaultExpression;
		  }
		  set
		  {
			this.defaultExpression = value;
		  }
	  }


	  public virtual bool Writable
	  {
		  get
		  {
			return isWritable;
		  }
		  set
		  {
			this.isWritable = value;
		  }
	  }

	}

}