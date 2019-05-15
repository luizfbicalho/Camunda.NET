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
namespace org.camunda.bpm.engine.impl
{

	using AbstractProcessInstanceModificationCommand = org.camunda.bpm.engine.impl.cmd.AbstractProcessInstanceModificationCommand;
	using JsonObjectConverter = org.camunda.bpm.engine.impl.json.JsonObjectConverter;
	using ModificationCmdJsonConverter = org.camunda.bpm.engine.impl.json.ModificationCmdJsonConverter;
	using JsonUtil = org.camunda.bpm.engine.impl.util.JsonUtil;
	using JsonObject = com.google.gson.JsonObject;

	public class RestartProcessInstancesBatchConfigurationJsonConverter : JsonObjectConverter<RestartProcessInstancesBatchConfiguration>
	{

	  public static readonly RestartProcessInstancesBatchConfigurationJsonConverter INSTANCE = new RestartProcessInstancesBatchConfigurationJsonConverter();

	  public const string PROCESS_INSTANCE_IDS = "processInstanceIds";
	  public const string INSTRUCTIONS = "instructions";
	  public const string PROCESS_DEFINITION_ID = "processDefinitionId";
	  public const string INITIAL_VARIABLES = "initialVariables";
	  public const string SKIP_CUSTOM_LISTENERS = "skipCustomListeners";
	  public const string SKIP_IO_MAPPINGS = "skipIoMappings";
	  public const string WITHOUT_BUSINESS_KEY = "withoutBusinessKey";

	  public override JsonObject toJsonObject(RestartProcessInstancesBatchConfiguration configuration)
	  {
		JsonObject json = JsonUtil.createObject();

		JsonUtil.addListField(json, PROCESS_INSTANCE_IDS, configuration.Ids);
		JsonUtil.addField(json, PROCESS_DEFINITION_ID, configuration.ProcessDefinitionId);
		JsonUtil.addListField(json, INSTRUCTIONS, ModificationCmdJsonConverter.INSTANCE, configuration.Instructions);
		JsonUtil.addField(json, INITIAL_VARIABLES, configuration.InitialVariables);
		JsonUtil.addField(json, SKIP_CUSTOM_LISTENERS, configuration.SkipCustomListeners);
		JsonUtil.addField(json, SKIP_IO_MAPPINGS, configuration.SkipIoMappings);
		JsonUtil.addField(json, WITHOUT_BUSINESS_KEY, configuration.WithoutBusinessKey);

		return json;
	  }

	  public override RestartProcessInstancesBatchConfiguration toObject(JsonObject json)
	  {
		IList<string> processInstanceIds = readProcessInstanceIds(json);
		IList<AbstractProcessInstanceModificationCommand> instructions = JsonUtil.asList(JsonUtil.getArray(json, INSTRUCTIONS), ModificationCmdJsonConverter.INSTANCE);

		return new RestartProcessInstancesBatchConfiguration(processInstanceIds, instructions, JsonUtil.getString(json, PROCESS_DEFINITION_ID), JsonUtil.getBoolean(json, INITIAL_VARIABLES), JsonUtil.getBoolean(json, SKIP_CUSTOM_LISTENERS), JsonUtil.getBoolean(json, SKIP_IO_MAPPINGS), JsonUtil.getBoolean(json, WITHOUT_BUSINESS_KEY));
	  }

	  protected internal virtual IList<string> readProcessInstanceIds(JsonObject jsonObject)
	  {
		return JsonUtil.asStringList(JsonUtil.getArray(jsonObject, PROCESS_INSTANCE_IDS));
	  }
	}

}