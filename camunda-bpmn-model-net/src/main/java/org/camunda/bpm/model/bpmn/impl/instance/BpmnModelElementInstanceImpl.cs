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
namespace org.camunda.bpm.model.bpmn.impl.instance
{
	using AbstractBaseElementBuilder = org.camunda.bpm.model.bpmn.builder.AbstractBaseElementBuilder;
	using BpmnModelElementInstance = org.camunda.bpm.model.bpmn.instance.BpmnModelElementInstance;
	using SubProcess = org.camunda.bpm.model.bpmn.instance.SubProcess;
	using ModelElementInstanceImpl = org.camunda.bpm.model.xml.impl.instance.ModelElementInstanceImpl;
	using ModelTypeInstanceContext = org.camunda.bpm.model.xml.impl.instance.ModelTypeInstanceContext;

	/// <summary>
	/// Shared base class for all BPMN Model Elements. Provides implementation
	/// of the <seealso cref="BpmnModelElementInstance"/> interface.
	/// 
	/// @author Daniel Meyer
	/// </summary>
	public abstract class BpmnModelElementInstanceImpl : ModelElementInstanceImpl, BpmnModelElementInstance
	{

	  public BpmnModelElementInstanceImpl(ModelTypeInstanceContext instanceContext) : base(instanceContext)
	  {
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("rawtypes") public org.camunda.bpm.model.bpmn.builder.AbstractBaseElementBuilder builder()
	  public virtual AbstractBaseElementBuilder builder()
	  {
		throw new BpmnModelException("No builder implemented for " + this);
	  }

	  public virtual bool Scope
	  {
		  get
		  {
			return this is org.camunda.bpm.model.bpmn.instance.Process || this is SubProcess;
		  }
	  }

	  public virtual BpmnModelElementInstance Scope
	  {
		  get
		  {
			BpmnModelElementInstance parentElement = (BpmnModelElementInstance) ParentElement;
			if (parentElement != null)
			{
			  if (parentElement.Scope)
			  {
				return parentElement;
			  }
			  else
			  {
				return parentElement.Scope;
			  }
			}
			else
			{
			  return null;
			}
		  }
	  }
	}

}