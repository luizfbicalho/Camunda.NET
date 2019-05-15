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
	using BpmPlatformSubsystemAdd = org.camunda.bpm.container.impl.jboss.extension.handler.BpmPlatformSubsystemAdd;
	using BpmPlatformSubsystemRemove = org.camunda.bpm.container.impl.jboss.extension.handler.BpmPlatformSubsystemRemove;
	using AttributeDefinition = org.jboss.@as.controller.AttributeDefinition;
	using PersistentResourceDefinition = org.jboss.@as.controller.PersistentResourceDefinition;
	using GenericSubsystemDescribeHandler = org.jboss.@as.controller.operations.common.GenericSubsystemDescribeHandler;
	using ManagementResourceRegistration = org.jboss.@as.controller.registry.ManagementResourceRegistration;


	public class BpmPlatformRootDefinition : PersistentResourceDefinition
	{

	  public static readonly BpmPlatformRootDefinition INSTANCE = new BpmPlatformRootDefinition();

	  private BpmPlatformRootDefinition() : base(BpmPlatformExtension.SUBSYSTEM_PATH, BpmPlatformExtension.ResourceDescriptionResolver, BpmPlatformSubsystemAdd.INSTANCE, BpmPlatformSubsystemRemove.INSTANCE)
	  {
	  }

	  public override ICollection<AttributeDefinition> Attributes
	  {
		  get
		  {
			return Collections.emptyList();
		  }
	  }

//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: @Override protected java.util.List<? extends org.jboss.as.controller.PersistentResourceDefinition> getChildren()
	  protected internal override IList<PersistentResourceDefinition> Children
	  {
		  get
		  {
			IList<PersistentResourceDefinition> children = new List<PersistentResourceDefinition>();
    
			children.Add(JobExecutorDefinition.INSTANCE);
			children.Add(ProcessEngineDefinition.INSTANCE);
    
			return children;
		  }
	  }

	  public override void registerOperations(ManagementResourceRegistration resourceRegistration)
	  {
		base.registerOperations(resourceRegistration);

		resourceRegistration.registerOperationHandler(GenericSubsystemDescribeHandler.DEFINITION, GenericSubsystemDescribeHandler.INSTANCE);
	  }
	}

}