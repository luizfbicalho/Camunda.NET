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
namespace org.camunda.bpm.model.bpmn
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.assertj.core.api.Assertions.assertThat;

	using HumanPerformer = org.camunda.bpm.model.bpmn.instance.HumanPerformer;
	using Performer = org.camunda.bpm.model.bpmn.instance.Performer;
	using PotentialOwner = org.camunda.bpm.model.bpmn.instance.PotentialOwner;
	using ResourceRole = org.camunda.bpm.model.bpmn.instance.ResourceRole;
	using UserTask = org.camunda.bpm.model.bpmn.instance.UserTask;
	using BeforeClass = org.junit.BeforeClass;
	using Test = org.junit.Test;

	/// <summary>
	/// @author Dario Campagna
	/// </summary>
	public class ResourceRolesTest
	{

	  private static BpmnModelInstance modelInstance;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @BeforeClass public static void parseModel()
	  public static void parseModel()
	  {
		modelInstance = Bpmn.readModelFromStream(typeof(ResourceRolesTest).getResourceAsStream("ResourceRolesTest.bpmn"));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetPerformer()
	  public virtual void testGetPerformer()
	  {
		UserTask userTask = modelInstance.getModelElementById("_3");
		ICollection<ResourceRole> resourceRoles = userTask.ResourceRoles;
		assertThat(resourceRoles.Count).isEqualTo(1);
		ResourceRole resourceRole = resourceRoles.GetEnumerator().next();
		assertThat(resourceRole is Performer).True;
		assertThat(resourceRole.Name).isEqualTo("Task performer");
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetHumanPerformer()
	  public virtual void testGetHumanPerformer()
	  {
		UserTask userTask = modelInstance.getModelElementById("_7");
		ICollection<ResourceRole> resourceRoles = userTask.ResourceRoles;
		assertThat(resourceRoles.Count).isEqualTo(1);
		ResourceRole resourceRole = resourceRoles.GetEnumerator().next();
		assertThat(resourceRole is HumanPerformer).True;
		assertThat(resourceRole.Name).isEqualTo("Task human performer");
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetPotentialOwner()
	  public virtual void testGetPotentialOwner()
	  {
		UserTask userTask = modelInstance.getModelElementById("_9");
		ICollection<ResourceRole> resourceRoles = userTask.ResourceRoles;
		assertThat(resourceRoles.Count).isEqualTo(1);
		ResourceRole resourceRole = resourceRoles.GetEnumerator().next();
		assertThat(resourceRole is PotentialOwner).True;
		assertThat(resourceRole.Name).isEqualTo("Task potential owner");
	  }

	}

}