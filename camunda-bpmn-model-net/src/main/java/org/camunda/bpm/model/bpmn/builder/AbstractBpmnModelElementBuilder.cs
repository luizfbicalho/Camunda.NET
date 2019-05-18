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
	using BpmnModelElementInstance = org.camunda.bpm.model.bpmn.instance.BpmnModelElementInstance;
	using EndEvent = org.camunda.bpm.model.bpmn.instance.EndEvent;
	using IntermediateThrowEvent = org.camunda.bpm.model.bpmn.instance.IntermediateThrowEvent;
	using SubProcess = org.camunda.bpm.model.bpmn.instance.SubProcess;
	using Transaction = org.camunda.bpm.model.bpmn.instance.Transaction;
	using ModelElementInstance = org.camunda.bpm.model.xml.instance.ModelElementInstance;

	/// <summary>
	/// @author Sebastian Menski
	/// </summary>
	public abstract class AbstractBpmnModelElementBuilder<B, E> where B : AbstractBpmnModelElementBuilder<B, E> where E : org.camunda.bpm.model.bpmn.instance.BpmnModelElementInstance
	{

	  protected internal readonly BpmnModelInstance modelInstance;
	  protected internal readonly E element;
	  protected internal readonly B myself;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") protected AbstractBpmnModelElementBuilder(org.camunda.bpm.model.bpmn.BpmnModelInstance modelInstance, E element, Class selfType)
	  protected internal AbstractBpmnModelElementBuilder(BpmnModelInstance modelInstance, E element, Type selfType)
	  {
		this.modelInstance = modelInstance;
		myself = (B) selfType.cast(this);
		this.element = element;
	  }

	  /// <summary>
	  /// Finishes the process building.
	  /// </summary>
	  /// <returns> the model instance with the build process </returns>
	  public virtual BpmnModelInstance done()
	  {
		return modelInstance;
	  }

	  /// <summary>
	  /// Finishes the building of an embedded sub-process.
	  /// </summary>
	  /// <returns> the parent sub-process builder </returns>
	  /// <exception cref="BpmnModelException"> if no parent sub-process can be found </exception>
	  public virtual SubProcessBuilder subProcessDone()
	  {
		BpmnModelElementInstance lastSubProcess = element.Scope;
		if (lastSubProcess != null && lastSubProcess is SubProcess)
		{
		  return ((SubProcess) lastSubProcess).builder();
		}
		else
		{
		  throw new BpmnModelException("Unable to find a parent subProcess.");
		}
	  }

	  public virtual TransactionBuilder transactionDone()
	  {
		BpmnModelElementInstance lastTransaction = element.Scope;
		if (lastTransaction != null && lastTransaction is Transaction)
		{
		  return new TransactionBuilder(modelInstance, (Transaction) lastTransaction);
		}
		else
		{
		  throw new BpmnModelException("Unable to find a parent transaction.");
		}
	  }

	  public virtual AbstractThrowEventBuilder throwEventDefinitionDone()
	  {
		ModelElementInstance lastEvent = element.DomElement.ParentElement.ModelElementInstance;
		if (lastEvent != null && lastEvent is IntermediateThrowEvent)
		{
		  return new IntermediateThrowEventBuilder(modelInstance, (IntermediateThrowEvent) lastEvent);
		}
		else if (lastEvent != null && lastEvent is EndEvent)
		{
		  return new EndEventBuilder(modelInstance, (EndEvent) lastEvent);
		}
		else
		{
		  throw new BpmnModelException("Unable to find a parent event.");
		}
	  }

	  public virtual E Element
	  {
		  get
		  {
			return element;
		  }
	  }
	}

}