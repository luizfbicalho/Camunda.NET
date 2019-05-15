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
	using JobExecutorAdd = org.camunda.bpm.container.impl.jboss.extension.handler.JobExecutorAdd;
	using JobExecutorRemove = org.camunda.bpm.container.impl.jboss.extension.handler.JobExecutorRemove;
	using AttributeDefinition = org.jboss.@as.controller.AttributeDefinition;
	using PersistentResourceDefinition = org.jboss.@as.controller.PersistentResourceDefinition;

	public class JobExecutorDefinition : PersistentResourceDefinition
	{

	  public static readonly JobExecutorDefinition INSTANCE = new JobExecutorDefinition();

	  private JobExecutorDefinition() : base(BpmPlatformExtension.JOB_EXECUTOR_PATH, BpmPlatformExtension.getResourceDescriptionResolver(org.camunda.bpm.container.impl.jboss.extension.ModelConstants_Fields.JOB_EXECUTOR), JobExecutorAdd.INSTANCE, JobExecutorRemove.INSTANCE)
	  {
	  }

	  public override ICollection<AttributeDefinition> Attributes
	  {
		  get
		  {
			return Arrays.asList(SubsystemAttributeDefinitons.JOB_EXECUTOR_ATTRIBUTES);
		  }
	  }

//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: @Override protected List<? extends org.jboss.as.controller.PersistentResourceDefinition> getChildren()
	  protected internal override IList<PersistentResourceDefinition> Children
	  {
		  get
		  {
			return Collections.singletonList(JobAcquisitionDefinition.INSTANCE);
		  }
	  }

	}

}