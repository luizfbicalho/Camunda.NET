using System;

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
namespace org.camunda.bpm.model.bpmn.builder
{
	using BusinessRuleTask = org.camunda.bpm.model.bpmn.instance.BusinessRuleTask;

	/// <summary>
	/// @author Sebastian Menski
	/// </summary>
	public abstract class AbstractBusinessRuleTaskBuilder<B> : AbstractTaskBuilder<B, BusinessRuleTask> where B : AbstractBusinessRuleTaskBuilder<B>
	{

	  protected internal AbstractBusinessRuleTaskBuilder(BpmnModelInstance modelInstance, BusinessRuleTask element, Type selfType) : base(modelInstance, element, selfType)
	  {
	  }

	  /// <summary>
	  /// Sets the implementation of the business rule task.
	  /// </summary>
	  /// <param name="implementation">  the implementation to set </param>
	  /// <returns> the builder object </returns>
	  public virtual B implementation(string implementation)
	  {
		element.Implementation = implementation;
		return myself;
	  }

	  /// <summary>
	  /// camunda extensions </summary>

	  /// <summary>
	  /// Sets the camunda class attribute.
	  /// </summary>
	  /// <param name="camundaClass">  the class name to set </param>
	  /// <returns> the builder object </returns>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("rawtypes") public B camundaClass(Class camundaClass)
	  public virtual B camundaClass(Type camundaClass)
	  {
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		return camundaClass(camundaClass.FullName);
	  }

	  /// <summary>
	  /// Sets the camunda class attribute.
	  /// </summary>
	  /// <param name="camundaClass">  the class name to set </param>
	  /// <returns> the builder object </returns>
	  public virtual B camundaClass(string fullQualifiedClassName)
	  {
		element.CamundaClass = fullQualifiedClassName;
		return myself;
	  }

	  /// <summary>
	  /// Sets the camunda delegateExpression attribute.
	  /// </summary>
	  /// <param name="camundaExpression">  the delegateExpression to set </param>
	  /// <returns> the builder object </returns>
	  public virtual B camundaDelegateExpression(string camundaExpression)
	  {
		element.CamundaDelegateExpression = camundaExpression;
		return myself;
	  }

	  /// <summary>
	  /// Sets the camunda expression attribute.
	  /// </summary>
	  /// <param name="camundaExpression">  the expression to set </param>
	  /// <returns> the builder object </returns>
	  public virtual B camundaExpression(string camundaExpression)
	  {
		element.CamundaExpression = camundaExpression;
		return myself;
	  }

	  /// <summary>
	  /// Sets the camunda resultVariable attribute.
	  /// </summary>
	  /// <param name="camundaResultVariable">  the name of the process variable </param>
	  /// <returns> the builder object </returns>
	  public virtual B camundaResultVariable(string camundaResultVariable)
	  {
		element.CamundaResultVariable = camundaResultVariable;
		return myself;
	  }

	  /// <summary>
	  /// Sets the camunda topic attribute. This is only meaningful when
	  /// the <seealso cref="camundaType(string)"/> attribute has the value <code>external</code>.
	  /// </summary>
	  /// <param name="camundaTopic"> the topic to set </param>
	  /// <returns> the builder object </returns>
	  public virtual B camundaTopic(string camundaTopic)
	  {
		element.CamundaTopic = camundaTopic;
		return myself;
	  }

	  /// <summary>
	  /// Sets the camunda type attribute.
	  /// </summary>
	  /// <param name="camundaType">  the type of the service task </param>
	  /// <returns> the builder object </returns>
	  public virtual B camundaType(string camundaType)
	  {
		element.CamundaType = camundaType;
		return myself;
	  }

	  /// <summary>
	  /// Sets the camunda decisionRef attribute.
	  /// </summary>
	  /// <param name="camundaDecisionRef"> the decisionRef to set </param>
	  /// <returns> the builder object </returns>
	  public virtual B camundaDecisionRef(string camundaDecisionRef)
	  {
		element.CamundaDecisionRef = camundaDecisionRef;
		return myself;
	  }

	  /// <summary>
	  /// Sets the camunda decisionRefBinding attribute.
	  /// </summary>
	  /// <param name="camundaDecisionRefBinding"> the decisionRefBinding to set </param>
	  /// <returns> the builder object </returns>
	  public virtual B camundaDecisionRefBinding(string camundaDecisionRefBinding)
	  {
		element.CamundaDecisionRefBinding = camundaDecisionRefBinding;
		return myself;
	  }

	  /// <summary>
	  /// Sets the camunda decisionRefVersion attribute.
	  /// </summary>
	  /// <param name="camundaDecisionRefVersion"> the decisionRefVersion to set </param>
	  /// <returns> the builder object </returns>
	  public virtual B camundaDecisionRefVersion(string camundaDecisionRefVersion)
	  {
		element.CamundaDecisionRefVersion = camundaDecisionRefVersion;
		return myself;
	  }

	  /// <summary>
	  /// Sets the camunda decisionRefVersionTag attribute.
	  /// </summary>
	  /// <param name="camundaDecisionRefVersionTag"> the decisionRefVersionTag to set </param>
	  /// <returns> the builder object </returns>
	  public virtual B camundaDecisionRefVersionTag(string camundaDecisionRefVersionTag)
	  {
		element.CamundaDecisionRefVersionTag = camundaDecisionRefVersionTag;
		return myself;
	  }

	  /// <summary>
	  /// Sets the camunda decisionRefTenantId attribute.
	  /// </summary>
	  /// <param name="decisionRefTenantId"> the decisionRefTenantId to set </param>
	  /// <returns> the builder object </returns>
	  public virtual B camundaDecisionRefTenantId(string decisionRefTenantId)
	  {
		element.CamundaDecisionRefTenantId = decisionRefTenantId;
		return myself;
	  }

	  /// <summary>
	  /// Set the camunda mapDecisionResult attribute.
	  /// </summary>
	  /// <param name="camundaMapDecisionResult"> the mapper for the decision result to set </param>
	  /// <returns> the builder object </returns>
	  public virtual B camundaMapDecisionResult(string camundaMapDecisionResult)
	  {
		element.CamundaMapDecisionResult = camundaMapDecisionResult;
		return myself;
	  }

	  /// <summary>
	  /// Sets the camunda task priority attribute. This is only meaningful when
	  /// the <seealso cref="camundaType(string)"/> attribute has the value <code>external</code>.
	  /// 
	  /// </summary>
	  /// <param name="taskPriority"> the priority for the external task </param>
	  /// <returns> the builder object </returns>
	  public virtual B camundaTaskPriority(string taskPriority)
	  {
		element.CamundaTaskPriority = taskPriority;
		return myself;
	  }
	}

}