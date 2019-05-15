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
namespace org.camunda.bpm.engine.impl.cmmn.handler
{

	using ProcessOrCaseTaskActivityBehavior = org.camunda.bpm.engine.impl.cmmn.behavior.ProcessOrCaseTaskActivityBehavior;
	using CmmnActivity = org.camunda.bpm.engine.impl.cmmn.model.CmmnActivity;
	using CallableElement = org.camunda.bpm.engine.impl.core.model.CallableElement;
	using CallableElementParameter = org.camunda.bpm.engine.impl.core.model.CallableElementParameter;
	using ParameterValueProvider = org.camunda.bpm.engine.impl.core.variable.mapping.value.ParameterValueProvider;
	using ExpressionManager = org.camunda.bpm.engine.impl.el.ExpressionManager;
	using CmmnElement = org.camunda.bpm.model.cmmn.instance.CmmnElement;
	using PlanItemDefinition = org.camunda.bpm.model.cmmn.instance.PlanItemDefinition;
	using CamundaIn = org.camunda.bpm.model.cmmn.instance.camunda.CamundaIn;
	using CamundaOut = org.camunda.bpm.model.cmmn.instance.camunda.CamundaOut;

	/// <summary>
	/// @author Roman Smirnov
	/// 
	/// </summary>
	public abstract class ProcessOrCaseTaskItemHandler : CallingTaskItemHandler
	{

	  protected internal override CallableElement createCallableElement()
	  {
		return new CallableElement();
	  }

	  protected internal override void initializeCallableElement(CmmnElement element, CmmnActivity activity, CmmnHandlerContext context)
	  {
		base.initializeCallableElement(element, activity, context);

		ProcessOrCaseTaskActivityBehavior behavior = (ProcessOrCaseTaskActivityBehavior) activity.ActivityBehavior;
		CallableElement callableElement = behavior.CallableElement;

		// inputs
		initializeInputParameter(element, activity, context, callableElement);

		// outputs
		initializeOutputParameter(element, activity, context, callableElement);
	  }

	  protected internal virtual void initializeInputParameter(CmmnElement element, CmmnActivity activity, CmmnHandlerContext context, CallableElement callableElement)
	  {
		ExpressionManager expressionManager = context.ExpressionManager;

		IList<CamundaIn> inputs = getInputs(element);

		foreach (CamundaIn input in inputs)
		{

		  // businessKey
		  string businessKey = input.CamundaBusinessKey;
		  if (!string.ReferenceEquals(businessKey, null) && businessKey.Length > 0)
		  {
			ParameterValueProvider businessKeyValueProvider = createParameterValueProvider(businessKey, expressionManager);
			callableElement.BusinessKeyValueProvider = businessKeyValueProvider;

		  }
		  else
		  {
			// create new parameter
			CallableElementParameter parameter = new CallableElementParameter();
			callableElement.addInput(parameter);

			if (input.CamundaLocal)
			{
			  parameter.ReadLocal = true;
			}

			// all variables
			string variables = input.CamundaVariables;
			if ("all".Equals(variables))
			{
			  parameter.AllVariables = true;
			  continue;
			}

			// source/sourceExpression
			string source = input.CamundaSource;
			if (string.ReferenceEquals(source, null) || source.Length == 0)
			{
			  source = input.CamundaSourceExpression;
			}

			ParameterValueProvider sourceValueProvider = createParameterValueProvider(source, expressionManager);
			parameter.SourceValueProvider = sourceValueProvider;

			// target
			string target = input.CamundaTarget;
			parameter.Target = target;
		  }
		}
	  }

	  protected internal virtual void initializeOutputParameter(CmmnElement element, CmmnActivity activity, CmmnHandlerContext context, CallableElement callableElement)
	  {
		ExpressionManager expressionManager = context.ExpressionManager;

		IList<CamundaOut> outputs = getOutputs(element);

		foreach (CamundaOut output in outputs)
		{

		  // create new parameter
		  CallableElementParameter parameter = new CallableElementParameter();
		  callableElement.addOutput(parameter);

		  // all variables
		  string variables = output.CamundaVariables;
		  if ("all".Equals(variables))
		  {
			parameter.AllVariables = true;
			continue;
		  }

		  // source/sourceExpression
		  string source = output.CamundaSource;
		  if (string.ReferenceEquals(source, null) || source.Length == 0)
		  {
			source = output.CamundaSourceExpression;
		  }

		  ParameterValueProvider sourceValueProvider = createParameterValueProvider(source, expressionManager);
		  parameter.SourceValueProvider = sourceValueProvider;

		  // target
		  string target = output.CamundaTarget;
		  parameter.Target = target;

		}
	  }

	  protected internal virtual IList<CamundaIn> getInputs(CmmnElement element)
	  {
		PlanItemDefinition definition = getDefinition(element);
		return queryExtensionElementsByClass(definition, typeof(CamundaIn));
	  }

	  protected internal virtual IList<CamundaOut> getOutputs(CmmnElement element)
	  {
		PlanItemDefinition definition = getDefinition(element);
		return queryExtensionElementsByClass(definition, typeof(CamundaOut));
	  }
	}

}