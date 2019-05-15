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
namespace org.camunda.bpm.engine.impl.json
{

	using AbstractProcessInstanceModificationCommand = org.camunda.bpm.engine.impl.cmd.AbstractProcessInstanceModificationCommand;
	using JsonUtil = org.camunda.bpm.engine.impl.util.JsonUtil;
	using JsonObject = com.google.gson.JsonObject;

	public class ModificationBatchConfigurationJsonConverter : JsonObjectConverter<ModificationBatchConfiguration>
	{

	  public static readonly ModificationBatchConfigurationJsonConverter INSTANCE = new ModificationBatchConfigurationJsonConverter();
	  public const string INSTRUCTIONS = "instructions";
	  public const string PROCESS_INSTANCE_IDS = "processInstanceIds";
	  public const string SKIP_LISTENERS = "skipListeners";
	  public const string SKIP_IO_MAPPINGS = "skipIoMappings";
	  public const string PROCESS_DEFINITION_ID = "processDefinitionId";

	  public override JsonObject toJsonObject(ModificationBatchConfiguration configuration)
	  {
		JsonObject json = JsonUtil.createObject();

		JsonUtil.addListField(json, INSTRUCTIONS, ModificationCmdJsonConverter.INSTANCE, configuration.Instructions);
		JsonUtil.addListField(json, PROCESS_INSTANCE_IDS, configuration.Ids);
		JsonUtil.addField(json, PROCESS_DEFINITION_ID, configuration.ProcessDefinitionId);
		JsonUtil.addField(json, SKIP_LISTENERS, configuration.SkipCustomListeners);
		JsonUtil.addField(json, SKIP_IO_MAPPINGS, configuration.SkipIoMappings);

		return json;
	  }

	  public override ModificationBatchConfiguration toObject(JsonObject json)
	  {

		IList<string> processInstanceIds = readProcessInstanceIds(json);
		string processDefinitionId = JsonUtil.getString(json, PROCESS_DEFINITION_ID);
		IList<AbstractProcessInstanceModificationCommand> instructions = JsonUtil.asList(JsonUtil.getArray(json, INSTRUCTIONS), ModificationCmdJsonConverter.INSTANCE);
		bool skipCustomListeners = JsonUtil.getBoolean(json, SKIP_LISTENERS);
		bool skipIoMappings = JsonUtil.getBoolean(json, SKIP_IO_MAPPINGS);

		return new ModificationBatchConfiguration(processInstanceIds, processDefinitionId, instructions, skipCustomListeners, skipIoMappings);
	  }

	  protected internal virtual IList<string> readProcessInstanceIds(JsonObject jsonObject)
	  {
		return JsonUtil.asStringList(JsonUtil.getArray(jsonObject, PROCESS_INSTANCE_IDS));
	  }

	}

}