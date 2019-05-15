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
namespace org.camunda.bpm.engine.impl.form.handler
{

	using DelegateExecution = org.camunda.bpm.engine.@delegate.DelegateExecution;
	using VariableScope = org.camunda.bpm.engine.@delegate.VariableScope;
	using FormFieldValidatorContext = org.camunda.bpm.engine.impl.form.validator.FormFieldValidatorContext;
	using TaskEntity = org.camunda.bpm.engine.impl.persistence.entity.TaskEntity;
	using VariableMap = org.camunda.bpm.engine.variable.VariableMap;

	/// <summary>
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public class DefaultFormFieldValidatorContext : FormFieldValidatorContext
	{

	  protected internal VariableScope variableScope;
	  protected internal string configuration;
	  protected internal VariableMap submittedValues;
	  protected internal FormFieldHandler formFieldHandler;

	  public DefaultFormFieldValidatorContext(VariableScope variableScope, string configuration, VariableMap submittedValues, FormFieldHandler formFieldHandler) : base()
	  {
		this.variableScope = variableScope;
		this.configuration = configuration;
		this.submittedValues = submittedValues;
		this.formFieldHandler = formFieldHandler;
	  }

	  public virtual FormFieldHandler FormFieldHandler
	  {
		  get
		  {
			return formFieldHandler;
		  }
	  }

	  public virtual DelegateExecution Execution
	  {
		  get
		  {
			if (variableScope is DelegateExecution)
			{
			  return (DelegateExecution) variableScope;
			}
			else if (variableScope is TaskEntity)
			{
			  return ((TaskEntity) variableScope).getExecution();
			}
			else
			{
			  return null;
			}
		  }
	  }

	  public virtual VariableScope VariableScope
	  {
		  get
		  {
			return variableScope;
		  }
	  }

	  public virtual string Configuration
	  {
		  get
		  {
			return configuration;
		  }
		  set
		  {
			this.configuration = value;
		  }
	  }


	  public virtual IDictionary<string, object> SubmittedValues
	  {
		  get
		  {
			return submittedValues;
		  }
	  }

	}

}