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
	using ServiceTask = org.camunda.bpm.model.bpmn.instance.ServiceTask;

	/// <summary>
	/// @author Sebastian Menski
	/// </summary>
	public abstract class AbstractServiceTaskBuilder<B> : AbstractTaskBuilder<B, ServiceTask> where B : AbstractServiceTaskBuilder<B>
	{

	  protected internal AbstractServiceTaskBuilder(BpmnModelInstance modelInstance, ServiceTask element, Type selfType) : base(modelInstance, element, selfType)
	  {
	  }

	  /// <summary>
	  /// Sets the implementation of the build service task.
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
	  /// the <seealso cref="#camundaType(String)"/> attribute has the value <code>external</code>.
	  /// </summary>
	  /// <param name="camundaTopic"> the topic to set </param>
	  /// <returns> the build object </returns>
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
	  /// Sets the camunda topic attribute and the camunda type attribute to the
	  /// value <code>external</code. Reduces two calls to <seealso cref="#camundaType(String)"/> and <seealso cref="#camundaTopic(String)"/>.
	  /// </summary>
	  /// <param name="camundaTopic"> the topic to set </param>
	  /// <returns> the build object </returns>
	  public virtual B camundaExternalTask(string camundaTopic)
	  {
		this.camundaType("external");
		this.camundaTopic(camundaTopic);
		return myself;
	  }

	  /// <summary>
	  /// Sets the camunda task priority attribute. This is only meaningful when
	  /// the <seealso cref="#camundaType(String)"/> attribute has the value <code>external</code>.
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