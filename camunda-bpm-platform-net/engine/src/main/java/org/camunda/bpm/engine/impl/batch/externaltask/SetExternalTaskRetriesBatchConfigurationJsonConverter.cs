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
namespace org.camunda.bpm.engine.impl.batch.externaltask
{

	using JsonObjectConverter = org.camunda.bpm.engine.impl.json.JsonObjectConverter;
	using JsonUtil = org.camunda.bpm.engine.impl.util.JsonUtil;
	using JsonObject = com.google.gson.JsonObject;

	public class SetExternalTaskRetriesBatchConfigurationJsonConverter : JsonObjectConverter<SetRetriesBatchConfiguration>
	{

	  public static readonly SetExternalTaskRetriesBatchConfigurationJsonConverter INSTANCE = new SetExternalTaskRetriesBatchConfigurationJsonConverter();

	  public const string EXTERNAL_TASK_IDS = "externalTaskIds";
	  public const string RETRIES = "retries";

	  public override JsonObject toJsonObject(SetRetriesBatchConfiguration configuration)
	  {
		JsonObject json = JsonUtil.createObject();

		JsonUtil.addListField(json, EXTERNAL_TASK_IDS, configuration.Ids);
		JsonUtil.addField(json, RETRIES, configuration.Retries);

		return json;
	  }

	  public override SetRetriesBatchConfiguration toObject(JsonObject json)
	  {
		return new SetRetriesBatchConfiguration(readExternalTaskIds(json), JsonUtil.getInt(json, RETRIES));
	  }

	  protected internal virtual IList<string> readExternalTaskIds(JsonObject json)
	  {
		return JsonUtil.asStringList(JsonUtil.getArray(json, EXTERNAL_TASK_IDS));
	  }

	}

}