using System;
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
namespace org.camunda.bpm.engine.impl.form.validator
{

	using Expression = org.camunda.bpm.engine.@delegate.Expression;
	using BpmnParse = org.camunda.bpm.engine.impl.bpmn.parser.BpmnParse;
	using ExpressionManager = org.camunda.bpm.engine.impl.el.ExpressionManager;
	using StringUtil = org.camunda.bpm.engine.impl.util.StringUtil;
	using Element = org.camunda.bpm.engine.impl.util.xml.Element;

	/// <summary>
	/// <para>Registry for built-in <seealso cref="FormFieldValidator"/> implementations.</para>
	/// 
	/// <para>Factory for <seealso cref="FormFieldValidator"/> instances.</para>
	/// 
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public class FormValidators
	{

	  /// <summary>
	  /// the registry of configured validators. Populated through <seealso cref="ProcessEngineConfiguration"/>. </summary>
	  protected internal IDictionary<string, Type> validators = new Dictionary<string, Type>();

	  /// <summary>
	  /// factory method for creating validator instances
	  /// 
	  /// </summary>
	  public virtual FormFieldValidator createValidator(Element constraint, BpmnParse bpmnParse, ExpressionManager expressionManager)
	  {

		string name = constraint.attribute("name");
		string config = constraint.attribute("config");

		if ("validator".Equals(name))
		{

		  // custom validators

		  if (string.ReferenceEquals(config, null) || config.Length == 0)
		  {
			bpmnParse.addError("validator configuration needs to provide either a fully " + "qualified classname or an expression resolving to a custom FormFieldValidator implementation.", constraint);

		  }
		  else
		  {
			if (StringUtil.isExpression(config))
			{
			  // expression
			  Expression validatorExpression = expressionManager.createExpression(config);
			  return new DelegateFormFieldValidator(validatorExpression);
			}
			else
			{
			  // classname
			  return new DelegateFormFieldValidator(config);
			}
		  }

		}
		else
		{

		  // built-in validators

		  Type validator = validators[name];
		  if (validator != null)
		  {
			FormFieldValidator validatorInstance = createValidatorInstance(validator);
			return validatorInstance;

		  }
		  else
		  {
			bpmnParse.addError("Cannot find validator implementation for name '" + name + "'.", constraint);

		  }

		}

		return null;


	  }

	  protected internal virtual FormFieldValidator createValidatorInstance(Type validator)
	  {
		try
		{
		  return System.Activator.CreateInstance(validator);

		}
		catch (InstantiationException e)
		{
		  throw new ProcessEngineException("Could not instantiate validator", e);

		}
		catch (IllegalAccessException e)
		{
		  throw new ProcessEngineException("Could not instantiate validator", e);

		}
	  }

	  public virtual void addValidator(string name, Type validatorType)
	  {
		validators[name] = validatorType;
	  }

	  public virtual IDictionary<string, Type> Validators
	  {
		  get
		  {
			return validators;
		  }
	  }

	}

}