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
	using JobAcquisitionAdd = org.camunda.bpm.container.impl.jboss.extension.handler.JobAcquisitionAdd;
	using JobAcquisitionRemove = org.camunda.bpm.container.impl.jboss.extension.handler.JobAcquisitionRemove;
	using AttributeDefinition = org.jboss.@as.controller.AttributeDefinition;
	using PersistentResourceDefinition = org.jboss.@as.controller.PersistentResourceDefinition;
	using ManagementResourceRegistration = org.jboss.@as.controller.registry.ManagementResourceRegistration;


	public class JobAcquisitionDefinition : PersistentResourceDefinition
	{

	  public static readonly JobAcquisitionDefinition INSTANCE = new JobAcquisitionDefinition();

	  private JobAcquisitionDefinition() : base(BpmPlatformExtension.JOB_ACQUISTIONS_PATH, BpmPlatformExtension.getResourceDescriptionResolver(org.camunda.bpm.container.impl.jboss.extension.ModelConstants_Fields.JOB_ACQUISITION), JobAcquisitionAdd.INSTANCE, JobAcquisitionRemove.INSTANCE)
	  {
	  }

	  public override ICollection<AttributeDefinition> Attributes
	  {
		  get
		  {
			return Arrays.asList(SubsystemAttributeDefinitons.JOB_ACQUISITION_ATTRIBUTES);
		  }
	  }

	  public override void registerAttributes(ManagementResourceRegistration resourceRegistration)
	  {
		base.registerAttributes(resourceRegistration);
	  }
	}

}