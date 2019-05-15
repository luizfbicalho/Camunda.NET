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
namespace org.camunda.bpm.engine.rest.util.migration
{


	public class MigrationPlanDtoBuilder
	{

	  public const string PROP_SOURCE_PROCESS_DEFINITION_ID = "sourceProcessDefinitionId";
	  public const string PROP_TARGET_PROCESS_DEFINITION_ID = "targetProcessDefinitionId";
	  public const string PROP_INSTRUCTIONS = "instructions";

	  protected internal readonly IDictionary<string, object> migrationPlan;

	  public MigrationPlanDtoBuilder(string sourceProcessDefinitionId, string targetProcessDefinitionId)
	  {
		migrationPlan = new Dictionary<string, object>();
		migrationPlan[PROP_SOURCE_PROCESS_DEFINITION_ID] = sourceProcessDefinitionId;
		migrationPlan[PROP_TARGET_PROCESS_DEFINITION_ID] = targetProcessDefinitionId;
	  }

	  public virtual MigrationPlanDtoBuilder instructions(IList<IDictionary<string, object>> instructions)
	  {
		migrationPlan[PROP_INSTRUCTIONS] = instructions;
		return this;
	  }

	  public virtual MigrationPlanDtoBuilder instruction(string sourceActivityId, string targetActivityId)
	  {
		return instruction(sourceActivityId, targetActivityId, null);
	  }

	  public virtual MigrationPlanDtoBuilder instruction(string sourceActivityId, string targetActivityId, bool? updateEventTrigger)
	  {
		IList<IDictionary<string, object>> instructions = (IList<IDictionary<string, object>>) migrationPlan[PROP_INSTRUCTIONS];
		if (instructions == null)
		{
		  instructions = new List<IDictionary<string, object>>();
		  migrationPlan[PROP_INSTRUCTIONS] = instructions;
		}

		IDictionary<string, object> migrationInstruction = (new MigrationInstructionDtoBuilder()).migrate(sourceActivityId, targetActivityId, updateEventTrigger).build();

		instructions.Add(migrationInstruction);
		return this;
	  }

	  public virtual IDictionary<string, object> build()
	  {
		return migrationPlan;
	  }
	}

}