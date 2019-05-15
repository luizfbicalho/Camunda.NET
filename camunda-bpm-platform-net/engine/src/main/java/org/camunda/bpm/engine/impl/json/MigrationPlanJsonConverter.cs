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
namespace org.camunda.bpm.engine.impl.json
{
	using MigrationPlanImpl = org.camunda.bpm.engine.impl.migration.MigrationPlanImpl;
	using JsonUtil = org.camunda.bpm.engine.impl.util.JsonUtil;
	using JsonObject = com.google.gson.JsonObject;
	using MigrationPlan = org.camunda.bpm.engine.migration.MigrationPlan;

	public class MigrationPlanJsonConverter : JsonObjectConverter<MigrationPlan>
	{

	  public static readonly MigrationPlanJsonConverter INSTANCE = new MigrationPlanJsonConverter();

	  public const string SOURCE_PROCESS_DEFINITION_ID = "sourceProcessDefinitionId";
	  public const string TARGET_PROCESS_DEFINITION_ID = "targetProcessDefinitionId";
	  public const string INSTRUCTIONS = "instructions";

	  public virtual JsonObject toJsonObject(MigrationPlan migrationPlan)
	  {
		JsonObject json = JsonUtil.createObject();

		JsonUtil.addField(json, SOURCE_PROCESS_DEFINITION_ID, migrationPlan.SourceProcessDefinitionId);
		JsonUtil.addField(json, TARGET_PROCESS_DEFINITION_ID, migrationPlan.TargetProcessDefinitionId);
		JsonUtil.addListField(json, INSTRUCTIONS, MigrationInstructionJsonConverter.INSTANCE, migrationPlan.Instructions);

		return json;
	  }

	  public override MigrationPlan toObject(JsonObject json)
	  {
		MigrationPlanImpl migrationPlan = new MigrationPlanImpl(JsonUtil.getString(json, SOURCE_PROCESS_DEFINITION_ID), JsonUtil.getString(json, TARGET_PROCESS_DEFINITION_ID));

		migrationPlan.Instructions = JsonUtil.asList(JsonUtil.getArray(json, INSTRUCTIONS), MigrationInstructionJsonConverter.INSTANCE);

		return migrationPlan;
	  }

	}

}