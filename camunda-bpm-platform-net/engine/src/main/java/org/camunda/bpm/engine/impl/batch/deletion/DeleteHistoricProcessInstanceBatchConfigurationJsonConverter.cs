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
namespace org.camunda.bpm.engine.impl.batch.deletion
{
	using JsonObjectConverter = org.camunda.bpm.engine.impl.json.JsonObjectConverter;
	using JsonUtil = org.camunda.bpm.engine.impl.util.JsonUtil;
	using JsonObject = com.google.gson.JsonObject;

	/// <summary>
	/// @author Askar Akhmerov
	/// </summary>
	public class DeleteHistoricProcessInstanceBatchConfigurationJsonConverter : JsonObjectConverter<BatchConfiguration>
	{


	  public static readonly DeleteHistoricProcessInstanceBatchConfigurationJsonConverter INSTANCE = new DeleteHistoricProcessInstanceBatchConfigurationJsonConverter();

	  public const string HISTORIC_PROCESS_INSTANCE_IDS = "historicProcessInstanceIds";
	  public const string FAIL_IF_NOT_EXISTS = "failIfNotExists";

	  public virtual JsonObject toJsonObject(BatchConfiguration configuration)
	  {
		JsonObject json = JsonUtil.createObject();

		JsonUtil.addListField(json, HISTORIC_PROCESS_INSTANCE_IDS, configuration.Ids);
		JsonUtil.addField(json, FAIL_IF_NOT_EXISTS, configuration.FailIfNotExists);
		return json;
	  }

	  public override BatchConfiguration toObject(JsonObject json)
	  {
		BatchConfiguration configuration = new BatchConfiguration(readProcessInstanceIds(json), JsonUtil.getBoolean(json, FAIL_IF_NOT_EXISTS));
		return configuration;
	  }

	  protected internal virtual IList<string> readProcessInstanceIds(JsonObject jsonObject)
	  {
		return JsonUtil.asStringList(JsonUtil.getArray(jsonObject, HISTORIC_PROCESS_INSTANCE_IDS));
	  }
	}

}