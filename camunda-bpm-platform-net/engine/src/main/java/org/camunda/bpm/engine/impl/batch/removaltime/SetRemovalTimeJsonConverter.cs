using System;
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
namespace org.camunda.bpm.engine.impl.batch.removaltime
{
	using JsonObject = com.google.gson.JsonObject;
	using JsonObjectConverter = org.camunda.bpm.engine.impl.json.JsonObjectConverter;
	using JsonUtil = org.camunda.bpm.engine.impl.util.JsonUtil;


	/// <summary>
	/// @author Tassilo Weidner
	/// </summary>
	public class SetRemovalTimeJsonConverter : JsonObjectConverter<SetRemovalTimeBatchConfiguration>
	{

	  public static readonly SetRemovalTimeJsonConverter INSTANCE = new SetRemovalTimeJsonConverter();

	  protected internal const string IDS = "ids";
	  protected internal const string REMOVAL_TIME = "removalTime";
	  protected internal const string HAS_REMOVAL_TIME = "hasRemovalTime";
	  protected internal const string IS_HIERARCHICAL = "isHierarchical";

	  public virtual JsonObject toJsonObject(SetRemovalTimeBatchConfiguration configuration)
	  {
		JsonObject json = JsonUtil.createObject();

		JsonUtil.addListField(json, IDS, configuration.Ids);
		JsonUtil.addDateField(json, REMOVAL_TIME, configuration.RemovalTime);
		JsonUtil.addField(json, HAS_REMOVAL_TIME, configuration.hasRemovalTime());
		JsonUtil.addField(json, IS_HIERARCHICAL, configuration.Hierarchical);

		return json;
	  }

	  public override SetRemovalTimeBatchConfiguration toObject(JsonObject jsonObject)
	  {

		long removalTimeMills = JsonUtil.getLong(jsonObject, REMOVAL_TIME);
		DateTime removalTime = removalTimeMills > 0 ? new DateTime(removalTimeMills) : null;

		IList<string> instanceIds = JsonUtil.asStringList(JsonUtil.getArray(jsonObject, IDS));

		bool hasRemovalTime = JsonUtil.getBoolean(jsonObject, HAS_REMOVAL_TIME);

		bool isHierarchical = JsonUtil.getBoolean(jsonObject, IS_HIERARCHICAL);

		return (new SetRemovalTimeBatchConfiguration(instanceIds)).setRemovalTime(removalTime).setHasRemovalTime(hasRemovalTime).setHierarchical(isHierarchical);
	  }

	}

}