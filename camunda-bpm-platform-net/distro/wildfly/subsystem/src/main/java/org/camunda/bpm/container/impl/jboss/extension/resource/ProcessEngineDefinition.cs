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
namespace org.camunda.bpm.container.impl.jboss.extension.resource
{
	using ProcessEngineAdd = org.camunda.bpm.container.impl.jboss.extension.handler.ProcessEngineAdd;
	using ProcessEngineRemove = org.camunda.bpm.container.impl.jboss.extension.handler.ProcessEngineRemove;
	using AttributeDefinition = org.jboss.@as.controller.AttributeDefinition;
	using PersistentResourceDefinition = org.jboss.@as.controller.PersistentResourceDefinition;


	public class ProcessEngineDefinition : PersistentResourceDefinition
	{

	  public static readonly ProcessEngineDefinition INSTANCE = new ProcessEngineDefinition();

	  private ProcessEngineDefinition() : base(BpmPlatformExtension.PROCESS_ENGINES_PATH, BpmPlatformExtension.getResourceDescriptionResolver(org.camunda.bpm.container.impl.jboss.extension.ModelConstants_Fields.PROCESS_ENGINE), ProcessEngineAdd.INSTANCE, ProcessEngineRemove.INSTANCE)
	  {
	  }

	  public override ICollection<AttributeDefinition> Attributes
	  {
		  get
		  {
			return Arrays.asList(SubsystemAttributeDefinitons.PROCESS_ENGINE_ATTRIBUTES);
		  }
	  }

	}

}