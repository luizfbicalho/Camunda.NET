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
namespace org.camunda.bpm.engine.test.api.repository
{
	using PluggableProcessEngineTestCase = org.camunda.bpm.engine.impl.test.PluggableProcessEngineTestCase;
	using DecisionDefinition = org.camunda.bpm.engine.repository.DecisionDefinition;
	using ProcessDefinition = org.camunda.bpm.engine.repository.ProcessDefinition;

	/// <summary>
	/// @author Stefan Hentschel.
	/// </summary>
	public class VersionTagTest : PluggableProcessEngineTestCase
	{

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testParsingVersionTag()
	  public virtual void testParsingVersionTag()
	  {
		ProcessDefinition process = repositoryService.createProcessDefinitionQuery().orderByProcessDefinitionId().asc().singleResult();

		assertEquals("ver_tag_1", process.VersionTag);
	  }

	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/repository/processOne.bpmn20.xml"})]
	  public virtual void testParsingNullVersionTag()
	  {
		ProcessDefinition process = repositoryService.createProcessDefinitionQuery().orderByProcessDefinitionId().asc().singleResult();

		assertEquals(null, process.VersionTag);
	  }

	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/repository/versionTag.dmn"})]
	  public virtual void testParsingVersionTagDecisionDefinition()
	  {
		DecisionDefinition decision = repositoryService.createDecisionDefinitionQuery().orderByDecisionDefinitionVersion().asc().singleResult();

		assertEquals("1.0.0", decision.VersionTag);
	  }

	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/repository/noVersionTag.dmn"})]
	  public virtual void testParsingNullVersionTagDecisionDefinition()
	  {
		DecisionDefinition decision = repositoryService.createDecisionDefinitionQuery().orderByDecisionDefinitionVersion().asc().singleResult();

		assertEquals(null, decision.VersionTag);
	  }
	}

}