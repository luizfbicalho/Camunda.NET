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


	public class MigrationInstructionDtoBuilder
	{

	  public const string PROP_SOURCE_ACTIVITY_IDS = "sourceActivityIds";
	  public const string PROP_TARGET_ACTIVITY_IDS = "targetActivityIds";
	  public const string PROP_UPDATE_EVENT_TRIGGER = "updateEventTrigger";

	  protected internal readonly IDictionary<string, object> migrationInstruction;

	  public MigrationInstructionDtoBuilder()
	  {
		migrationInstruction = new Dictionary<string, object>();
	  }

	  public virtual MigrationInstructionDtoBuilder migrate(string sourceActivityId, string targetActivityId)
	  {
		return migrate(Collections.singletonList(sourceActivityId), Collections.singletonList(targetActivityId), null);
	  }

	  public virtual MigrationInstructionDtoBuilder migrate(string sourceActivityId, string targetActivityId, bool? updateEventTrigger)
	  {
		return migrate(Collections.singletonList(sourceActivityId), Collections.singletonList(targetActivityId), updateEventTrigger);
	  }

	  public virtual MigrationInstructionDtoBuilder migrate(IList<string> sourceActivityId, IList<string> targetActivityId, bool? updateEventTrigger)
	  {
		migrationInstruction[PROP_SOURCE_ACTIVITY_IDS] = sourceActivityId;
		migrationInstruction[PROP_TARGET_ACTIVITY_IDS] = targetActivityId;
		migrationInstruction[PROP_UPDATE_EVENT_TRIGGER] = updateEventTrigger;

		return this;
	  }

	  public virtual IDictionary<string, object> build()
	  {
		return migrationInstruction;
	  }

	}

}