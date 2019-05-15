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
	/// Perform serialization of DeleteProcessInstanceBatchConfiguration into JSON format.
	/// 
	/// @author Askar Akhmerov
	/// </summary>
	public class DeleteProcessInstanceBatchConfigurationJsonConverter : JsonObjectConverter<DeleteProcessInstanceBatchConfiguration>
	{
	  public static readonly DeleteProcessInstanceBatchConfigurationJsonConverter INSTANCE = new DeleteProcessInstanceBatchConfigurationJsonConverter();

	  public const string DELETE_REASON = "deleteReason";
	  public const string PROCESS_INSTANCE_IDS = "processInstanceIds";
	  public const string SKIP_CUSTOM_LISTENERS = "skipCustomListeners";
	  public const string SKIP_SUBPROCESSES = "skipSubprocesses";
	  public const string FAIL_IF_NOT_EXISTS = "failIfNotExists";

	  public virtual JsonObject toJsonObject(DeleteProcessInstanceBatchConfiguration configuration)
	  {
		JsonObject json = JsonUtil.createObject();

		JsonUtil.addField(json, DELETE_REASON, configuration.DeleteReason);
		JsonUtil.addListField(json, PROCESS_INSTANCE_IDS, configuration.Ids);
		JsonUtil.addField(json, SKIP_CUSTOM_LISTENERS, configuration.SkipCustomListeners);
		JsonUtil.addField(json, SKIP_SUBPROCESSES, configuration.SkipSubprocesses);
		JsonUtil.addField(json, FAIL_IF_NOT_EXISTS, configuration.FailIfNotExists);
		return json;
	  }

	  public override DeleteProcessInstanceBatchConfiguration toObject(JsonObject json)
	  {
		DeleteProcessInstanceBatchConfiguration configuration = new DeleteProcessInstanceBatchConfiguration(readProcessInstanceIds(json), null, JsonUtil.getBoolean(json, SKIP_CUSTOM_LISTENERS), JsonUtil.getBoolean(json, SKIP_SUBPROCESSES), JsonUtil.getBoolean(json, FAIL_IF_NOT_EXISTS));

		string deleteReason = JsonUtil.getString(json, DELETE_REASON);
		if (!string.ReferenceEquals(deleteReason, null) && deleteReason.Length > 0)
		{
		  configuration.DeleteReason = deleteReason;
		}

		return configuration;
	  }

	  protected internal virtual IList<string> readProcessInstanceIds(JsonObject jsonObject)
	  {
		return JsonUtil.asStringList(JsonUtil.getArray(jsonObject, PROCESS_INSTANCE_IDS));
	  }
	}

}