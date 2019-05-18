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
	using CallActivity = org.camunda.bpm.model.bpmn.instance.CallActivity;
	using CamundaIn = org.camunda.bpm.model.bpmn.instance.camunda.CamundaIn;
	using CamundaOut = org.camunda.bpm.model.bpmn.instance.camunda.CamundaOut;

	/// <summary>
	/// @author Sebastian Menski
	/// </summary>
	public class AbstractCallActivityBuilder<B> : AbstractActivityBuilder<B, CallActivity> where B : AbstractCallActivityBuilder<B>
	{

	  protected internal AbstractCallActivityBuilder(BpmnModelInstance modelInstance, CallActivity element, Type selfType) : base(modelInstance, element, selfType)
	  {
	  }

	  /// <summary>
	  /// Sets the called element
	  /// </summary>
	  /// <param name="calledElement">  the process to call </param>
	  /// <returns> the builder object </returns>
	  public virtual B calledElement(string calledElement)
	  {
		element.CalledElement = calledElement;
		return myself;
	  }

	  /// <summary>
	  /// camunda extensions </summary>

	  /// @deprecated use camundaAsyncBefore() instead.
	  /// 
	  /// Sets the camunda async attribute to true.
	  /// 
	  /// <returns> the builder object </returns>
	  [Obsolete("use camundaAsyncBefore() instead.")]
	  public virtual B camundaAsync()
	  {
		element.CamundaAsyncBefore = true;
		return myself;
	  }

	  /// @deprecated use camundaAsyncBefore(isCamundaAsyncBefore) instead
	  /// 
	  /// Sets the camunda async attribute.
	  /// 
	  /// <param name="isCamundaAsync">  the async state of the task </param>
	  /// <returns> the builder object </returns>
	  [Obsolete("use camundaAsyncBefore(isCamundaAsyncBefore) instead")]
	  public virtual B camundaAsync(bool isCamundaAsync)
	  {
		element.CamundaAsyncBefore = isCamundaAsync;
		return myself;
	  }

	  /// <summary>
	  /// Sets the camunda calledElementBinding attribute
	  /// </summary>
	  /// <param name="camundaCalledElementBinding">  the element binding to use </param>
	  /// <returns> the builder object </returns>
	  public virtual B camundaCalledElementBinding(string camundaCalledElementBinding)
	  {
		element.CamundaCalledElementBinding = camundaCalledElementBinding;
		return myself;
	  }

	  /// <summary>
	  /// Sets the camunda calledElementVersion attribute
	  /// </summary>
	  /// <param name="camundaCalledElementVersion">  the element version to use </param>
	  /// <returns> the builder object </returns>
	  public virtual B camundaCalledElementVersion(string camundaCalledElementVersion)
	  {
		element.CamundaCalledElementVersion = camundaCalledElementVersion;
		return myself;
	  }

	  /// <summary>
	  /// Sets the camunda calledElementVersionTag attribute
	  /// </summary>
	  /// <param name="camundaCalledElementVersionTag">  the element version to use </param>
	  /// <returns> the builder object </returns>
	  public virtual B camundaCalledElementVersionTag(string camundaCalledElementVersionTag)
	  {
		element.CamundaCalledElementVersionTag = camundaCalledElementVersionTag;
		return myself;
	  }

	  /// <summary>
	  /// Sets the camunda calledElementTenantId attribute </summary>
	  /// <param name="camundaCalledElementTenantId"> the called element tenant id </param>
	  /// <returns> the builder object </returns>
	  public virtual B camundaCalledElementTenantId(string camundaCalledElementTenantId)
	  {
		element.CamundaCalledElementTenantId = camundaCalledElementTenantId;
		return myself;
	  }

	  /// <summary>
	  /// Sets the camunda caseRef attribute
	  /// </summary>
	  /// <param name="caseRef"> the case to call </param>
	  /// <returns> the builder object </returns>
	  public virtual B camundaCaseRef(string caseRef)
	  {
		element.CamundaCaseRef = caseRef;
		return myself;
	  }

	  /// <summary>
	  /// Sets the camunda caseBinding attribute
	  /// </summary>
	  /// <param name="camundaCaseBinding">  the case binding to use </param>
	  /// <returns> the builder object </returns>
	  public virtual B camundaCaseBinding(string camundaCaseBinding)
	  {
		element.CamundaCaseBinding = camundaCaseBinding;
		return myself;
	  }

	  /// <summary>
	  /// Sets the camunda caseVersion attribute
	  /// </summary>
	  /// <param name="camundaCaseVersion">  the case version to use </param>
	  /// <returns> the builder object </returns>
	  public virtual B camundaCaseVersion(string camundaCaseVersion)
	  {
		element.CamundaCaseVersion = camundaCaseVersion;
		return myself;
	  }

	  /// <summary>
	  /// Sets the caseTenantId </summary>
	  /// <param name="tenantId"> the tenant id to set </param>
	  /// <returns> the builder object </returns>
	  public virtual B camundaCaseTenantId(string tenantId)
	  {
		element.CamundaCaseTenantId = tenantId;
		return myself;
	  }

	  /// <summary>
	  /// Sets a "camunda in" parameter to pass a variable from the super process instance to the sub process instance
	  /// </summary>
	  /// <param name="source"> the name of variable in the super process instance </param>
	  /// <param name="target"> the name of the variable in the sub process instance </param>
	  /// <returns> the builder object </returns>
	  public virtual B camundaIn(string source, string target)
	  {
		CamundaIn param = modelInstance.newInstance(typeof(CamundaIn));
		param.CamundaSource = source;
		param.CamundaTarget = target;
		addExtensionElement(param);
		return myself;
	  }

	  /// <summary>
	  /// Sets a "camunda out" parameter to pass a variable from a sub process instance to the super process instance
	  /// </summary>
	  /// <param name="source"> the name of variable in the sub process instance </param>
	  /// <param name="target"> the name of the variable in the super process instance </param>
	  /// <returns> the builder object </returns>
	  public virtual B camundaOut(string source, string target)
	  {
		CamundaOut param = modelInstance.newInstance(typeof(CamundaOut));
		param.CamundaSource = source;
		param.CamundaTarget = target;
		addExtensionElement(param);
		return myself;
	  }

	  /// <summary>
	  /// Sets the camunda variableMappingClass attribute. It references on a class which implements the
	  /// <seealso cref="DelegateVariableMapping"/> interface.
	  /// Is used to delegate the variable in- and output mapping to the given class.
	  /// </summary>
	  /// <param name="camundaVariableMappingClass">                  the class name to set </param>
	  /// <returns>                              the builder object </returns>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("rawtypes") public B camundaVariableMappingClass(Class camundaVariableMappingClass)
	  public virtual B camundaVariableMappingClass(Type camundaVariableMappingClass)
	  {
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		return camundaVariableMappingClass(camundaVariableMappingClass.FullName);
	  }

	  /// <summary>
	  /// Sets the camunda variableMappingClass attribute. It references on a class which implements the
	  /// <seealso cref="DelegateVariableMapping"/> interface.
	  /// Is used to delegate the variable in- and output mapping to the given class.
	  /// </summary>
	  /// <param name="camundaVariableMappingClass">                  the class name to set </param>
	  /// <returns>                              the builder object </returns>
	  public virtual B camundaVariableMappingClass(string fullQualifiedClassName)
	  {
		element.CamundaVariableMappingClass = fullQualifiedClassName;
		return myself;
	  }

	  /// <summary>
	  /// Sets the camunda variableMappingDelegateExpression attribute. The expression when is resolved
	  /// references to an object of a class, which implements the <seealso cref="DelegateVariableMapping"/> interface.
	  /// Is used to delegate the variable in- and output mapping to the given class.
	  /// </summary>
	  /// <param name="camundaVariableMappingDelegateExpression">     the expression which references a delegate object </param>
	  /// <returns>                              the builder object </returns>
	  public virtual B camundaVariableMappingDelegateExpression(string camundaVariableMappingDelegateExpression)
	  {
		element.CamundaVariableMappingDelegateExpression = camundaVariableMappingDelegateExpression;
		return myself;
	  }
	}

}