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
namespace org.camunda.bpm.engine.impl.batch.update
{
	using JsonObjectConverter = org.camunda.bpm.engine.impl.json.JsonObjectConverter;
	using JsonUtil = org.camunda.bpm.engine.impl.util.JsonUtil;
	using JsonObject = com.google.gson.JsonObject;

	public class UpdateProcessInstancesSuspendStateBatchConfigurationJsonConverter : JsonObjectConverter<UpdateProcessInstancesSuspendStateBatchConfiguration>
	{

	  public static readonly UpdateProcessInstancesSuspendStateBatchConfigurationJsonConverter INSTANCE = new UpdateProcessInstancesSuspendStateBatchConfigurationJsonConverter();

	  public const string PROCESS_INSTANCE_IDS = "processInstanceIds";
	  public const string SUSPENDING = "suspended";

	  public virtual JsonObject toJsonObject(UpdateProcessInstancesSuspendStateBatchConfiguration configuration)
	  {
		JsonObject json = JsonUtil.createObject();

		JsonUtil.addListField(json, PROCESS_INSTANCE_IDS, configuration.Ids);
		JsonUtil.addField(json, SUSPENDING, configuration.Suspended);
		return json;
	  }

	  public override UpdateProcessInstancesSuspendStateBatchConfiguration toObject(JsonObject json)
	  {
		UpdateProcessInstancesSuspendStateBatchConfiguration configuration = new UpdateProcessInstancesSuspendStateBatchConfiguration(readProcessInstanceIds(json), JsonUtil.getBoolean(json, SUSPENDING));

		return configuration;
	  }

	  protected internal virtual IList<string> readProcessInstanceIds(JsonObject jsonObject)
	  {
		return JsonUtil.asStringList(JsonUtil.getArray(jsonObject, PROCESS_INSTANCE_IDS));
	  }
	}

}