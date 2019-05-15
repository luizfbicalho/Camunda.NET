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
namespace org.camunda.bpm.engine.impl.util
{

	using JsonObjectConverter = org.camunda.bpm.engine.impl.json.JsonObjectConverter;
	using Gson = com.google.gson.Gson;
	using GsonBuilder = com.google.gson.GsonBuilder;
	using JsonArray = com.google.gson.JsonArray;
	using JsonDeserializationContext = com.google.gson.JsonDeserializationContext;
	using JsonDeserializer = com.google.gson.JsonDeserializer;
	using JsonElement = com.google.gson.JsonElement;
	using JsonIOException = com.google.gson.JsonIOException;
	using JsonNull = com.google.gson.JsonNull;
	using JsonObject = com.google.gson.JsonObject;
	using JsonParseException = com.google.gson.JsonParseException;
	using JsonPrimitive = com.google.gson.JsonPrimitive;
	using JsonSyntaxException = com.google.gson.JsonSyntaxException;
	using LazilyParsedNumber = com.google.gson.@internal.LazilyParsedNumber;

	/// <summary>
	/// @author Tassilo Weidner
	/// </summary>
	public sealed class JsonUtil
	{

	  private static readonly EngineUtilLogger LOG = ProcessEngineLogger.UTIL_LOGGER;

	  protected internal static Gson gsonMapper = createGsonMapper();

	  public static void addFieldRawValue(JsonObject jsonObject, string memberName, object rawValue)
	  {
		if (rawValue != null)
		{
		  JsonElement jsonNode = null;

		  try
		  {
			jsonNode = GsonMapper.toJsonTree(rawValue);

		  }
		  catch (JsonIOException e)
		  {
			LOG.logJsonException(e);
		  }

		  if (jsonNode != null)
		  {
			jsonObject.add(memberName, jsonNode);
		  }
		}
	  }

	  public static void addField<T>(JsonObject jsonObject, string name, JsonObjectConverter<T> converter, T value)
	  {
		if (jsonObject != null && !string.ReferenceEquals(name, null) && converter != null && value != default(T))
		{
		  jsonObject.add(name, converter.toJsonObject(value));
		}
	  }

	  public static void addListField(JsonObject jsonObject, string name, IList<string> list)
	  {
		if (jsonObject != null && !string.ReferenceEquals(name, null) && list != null)
		{
		  jsonObject.add(name, asArray(list));
		}
	  }

	  public static void addArrayField(JsonObject jsonObject, string name, string[] array)
	  {
		if (jsonObject != null && !string.ReferenceEquals(name, null) && array != null)
		{
		  addListField(jsonObject, name, Arrays.asList(array));
		}
	  }

	  public static void addDateField(JsonObject jsonObject, string name, DateTime date)
	  {
		if (jsonObject != null && !string.ReferenceEquals(name, null) && date != null)
		{
		  jsonObject.addProperty(name, date.Ticks);
		}
	  }

	  public static void addElement<T>(JsonArray jsonObject, JsonObjectConverter<T> converter, T value)
	  {
		if (jsonObject != null && converter != null && value != default(T))
		{
		  JsonObject jsonElement = converter.toJsonObject(value);

		  if (jsonElement != null)
		  {
			jsonObject.add(jsonElement);
		  }
		}
	  }

	  public static void addListField<T>(JsonObject jsonObject, string name, JsonObjectConverter<T> converter, IList<T> list)
	  {
		if (jsonObject != null && !string.ReferenceEquals(name, null) && converter != null && list != null)
		{
		  JsonArray arrayNode = createArray();

		  foreach (T item in list)
		  {
			if (item != default(T))
			{
			  JsonObject jsonElement = converter.toJsonObject(item);
			  if (jsonElement != null)
			  {
				arrayNode.add(jsonElement);
			  }
			}
		  }

		  jsonObject.add(name, arrayNode);
		}
	  }

	  public static T asJavaObject<T>(JsonObject jsonObject, JsonObjectConverter<T> converter)
	  {
		if (jsonObject != null && converter != null)
		{
		  return converter.toObject(jsonObject);

		}
		else
		{
		  return default(T);

		}
	  }

	  public static void addNullField(JsonObject jsonObject, string name)
	  {
		if (jsonObject != null && !string.ReferenceEquals(name, null))
		{
		  jsonObject.add(name, JsonNull.INSTANCE);
		}
	  }

	  public static void addField(JsonObject jsonObject, string name, JsonArray value)
	  {
		if (jsonObject != null && !string.ReferenceEquals(name, null) && value != null)
		{
		  jsonObject.add(name, value);
		}
	  }

	  public static void addField(JsonObject jsonObject, string name, string value)
	  {
		if (jsonObject != null && !string.ReferenceEquals(name, null) && !string.ReferenceEquals(value, null))
		{
		  jsonObject.addProperty(name, value);
		}
	  }

	  public static void addField(JsonObject jsonObject, string name, bool? value)
	  {
		if (jsonObject != null && !string.ReferenceEquals(name, null) && value != null)
		{
		  jsonObject.addProperty(name, value);
		}
	  }

	  public static void addField(JsonObject jsonObject, string name, int? value)
	  {
		if (jsonObject != null && !string.ReferenceEquals(name, null) && value != null)
		{
		  jsonObject.addProperty(name, value);
		}
	  }

	  public static void addField(JsonObject jsonObject, string name, short? value)
	  {
		if (jsonObject != null && !string.ReferenceEquals(name, null) && value != null)
		{
		  jsonObject.addProperty(name, value);
		}
	  }

	  public static void addField(JsonObject jsonObject, string name, long? value)
	  {
		if (jsonObject != null && !string.ReferenceEquals(name, null) && value != null)
		{
		  jsonObject.addProperty(name, value);
		}
	  }

	  public static void addField(JsonObject jsonObject, string name, double? value)
	  {
		if (jsonObject != null && !string.ReferenceEquals(name, null) && value != null)
		{
		  jsonObject.addProperty(name, value);
		}
	  }

	  public static void addDefaultField(JsonObject jsonObject, string name, bool defaultValue, bool? value)
	  {
		if (jsonObject != null && !string.ReferenceEquals(name, null) && value != null && !value.Equals(defaultValue))
		{
		  addField(jsonObject, name, value);
		}
	  }

	  public static sbyte[] asBytes(JsonElement jsonObject)
	  {
		string jsonString = null;

		if (jsonObject != null)
		{
		  try
		  {
			jsonString = GsonMapper.toJson(jsonObject);

		  }
		  catch (JsonIOException e)
		  {
			LOG.logJsonException(e);
		  }
		}

		if (string.ReferenceEquals(jsonString, null))
		{
		  jsonString = "";
		}

		return StringUtil.toByteArray(jsonString);
	  }

	  public static JsonObject asObject(sbyte[] byteArray)
	  {
		string stringValue = null;

		if (byteArray != null)
		{
		  stringValue = StringUtil.fromBytes(byteArray);
		}

		if (string.ReferenceEquals(stringValue, null))
		{
		  return createObject();

		}

		JsonObject jsonObject = null;
		try
		{
		  jsonObject = GsonMapper.fromJson(stringValue, typeof(JsonObject));

		}
		catch (JsonParseException e)
		{
		  LOG.logJsonException(e);
		}

		if (jsonObject != null)
		{
		  return jsonObject;

		}
		else
		{
		  return createObject();

		}
	  }

	  public static JsonObject asObject(string jsonString)
	  {
		JsonObject jsonObject = null;

		if (!string.ReferenceEquals(jsonString, null))
		{
		  try
		  {
			jsonObject = GsonMapper.fromJson(jsonString, typeof(JsonObject));

		  }
		  catch (Exception e) when (e is JsonParseException || e is System.InvalidCastException)
		  {
			LOG.logJsonException(e);
		  }
		}

		if (jsonObject != null)
		{
		  return jsonObject;

		}
		else
		{
		  return createObject();

		}
	  }

	  public static JsonObject asObject(IDictionary<string, object> properties)
	  {
		if (properties != null)
		{
		  JsonObject jsonObject = null;

		  try
		  {
			jsonObject = (JsonObject) GsonMapper.toJsonTree(properties);

		  }
		  catch (Exception e) when (e is JsonIOException || e is System.InvalidCastException)
		  {
			LOG.logJsonException(e);
		  }

		  if (jsonObject != null)
		  {
			return jsonObject;

		  }
		  else
		  {
			return createObject();

		  }
		}
		else
		{
		  return createObject();

		}
	  }

	  public static IList<string> asStringList(JsonElement jsonObject)
	  {
		JsonArray jsonArray = null;

		if (jsonObject != null)
		{
		  try
		  {
			jsonArray = jsonObject.AsJsonArray;

		  }
		  catch (Exception e) when (e is System.InvalidOperationException || e is System.InvalidCastException)
		  {
			LOG.logJsonException(e);
		  }
		}

		if (jsonArray == null)
		{
		  return Collections.emptyList();
		}

		IList<string> list = new List<string>();
		foreach (JsonElement entry in jsonArray)
		{
		  string stringValue = null;

		  try
		  {
			stringValue = entry.AsString;

		  }
		  catch (Exception e) when (e is System.InvalidOperationException || e is System.InvalidCastException)
		  {
			LOG.logJsonException(e);
		  }

		  if (!string.ReferenceEquals(stringValue, null))
		  {
			list.Add(stringValue);
		  }
		}

		return list;
	  }

	  public static IList<T> asList<T>(JsonArray jsonArray, JsonObjectConverter<T> converter)
	  {
		if (jsonArray == null || converter == null)
		{
		  return Collections.emptyList();
		}

		IList<T> list = new List<T>();

		foreach (JsonElement element in jsonArray)
		{
		  JsonObject jsonObject = null;

		  try
		  {
			jsonObject = element.AsJsonObject;

		  }
		  catch (Exception e) when (e is System.InvalidOperationException || e is System.InvalidCastException)
		  {
			LOG.logJsonException(e);
		  }

		  if (jsonObject != null)
		  {

			T rawObject = converter.toObject(jsonObject);
			if (rawObject != default(T))
			{
			  list.Add(rawObject);
			}
		  }
		}

		return list;
	  }

	  public static IList<object> asList(JsonElement jsonElement)
	  {
		if (jsonElement == null)
		{
		  return Collections.emptyList();
		}

		JsonArray jsonArray = null;

		try
		{
		  jsonArray = jsonElement.AsJsonArray;

		}
		catch (Exception e) when (e is System.InvalidOperationException || e is System.InvalidCastException)
		{
		  LOG.logJsonException(e);
		}

		if (jsonArray == null)
		{
		  return Collections.emptyList();
		}

		IList<object> list = new List<object>();
		foreach (JsonElement entry in jsonArray)
		{

		  if (entry.JsonPrimitive)
		  {

			object rawObject = asPrimitiveObject((JsonPrimitive) entry);
			if (rawObject != null)
			{
			  list.Add(rawObject);
			}

		  }
		  else if (entry.JsonNull)
		  {
			list.Add(null);

		  }
		  else if (entry.JsonObject)
		  {
			list.Add(asMap(entry));

		  }
		  else if (entry.JsonArray)
		  {
			list.Add(asList(entry));

		  }
		}

		return list;
	  }

	  public static IDictionary<string, object> asMap(JsonElement jsonElement)
	  {
		if (jsonElement == null)
		{
		  return Collections.emptyMap();
		}

		JsonObject jsonObject = null;

		try
		{
		  jsonObject = jsonElement.AsJsonObject;

		}
		catch (Exception e) when (e is System.InvalidOperationException || e is System.InvalidCastException)
		{
		  LOG.logJsonException(e);
		}

		if (jsonObject == null)
		{
		  return Collections.emptyMap();

		}

		IDictionary<string, object> map = new Dictionary<string, object>();
		foreach (KeyValuePair<string, JsonElement> jsonEntry in jsonObject.entrySet())
		{

		  string key = jsonEntry.Key;
		  JsonElement value = jsonEntry.Value;

		  if (value.JsonPrimitive)
		  {

			object rawObject = asPrimitiveObject((JsonPrimitive) value);
			if (rawObject != null)
			{
			  map[key] = rawObject;
			}

		  }
		  else if (value.JsonNull)
		  {
			map[key] = null;

		  }
		  else if (value.JsonObject)
		  {
			map[key] = asMap(value);

		  }
		  else if (value.JsonArray)
		  {
			map[key] = asList(value);

		  }
		}

		return map;
	  }

	  public static string asString(IDictionary<string, object> properties)
	  {
		if (properties != null)
		{

		  JsonObject jsonObject = asObject(properties);
		  if (jsonObject != null)
		  {

			string stringValue = jsonObject.ToString();
			if (!string.ReferenceEquals(stringValue, null))
			{
			  return stringValue;

			}
			else
			{
			  return "";

			}
		  }
		  else
		  {
			return "";

		  }

		}
		else
		{
		  return "";

		}
	  }

	  public static JsonArray asArray(IList<string> list)
	  {
		if (list != null)
		{
		  JsonElement jsonElement = null;

		  try
		  {
			jsonElement = GsonMapper.toJsonTree(list);

		  }
		  catch (JsonIOException e)
		  {
			LOG.logJsonException(e);
		  }

		  if (jsonElement != null)
		  {
			return getArray(jsonElement);

		  }
		  else
		  {
			return createArray();

		  }
		}
		else
		{
		  return createArray();

		}
	  }

	  public static object getRawObject(JsonObject jsonObject, string memberName)
	  {
		if (jsonObject == null || string.ReferenceEquals(memberName, null))
		{
		  return null;
		}

		object rawValue = null;

		if (jsonObject.has(memberName))
		{
		  JsonPrimitive jsonPrimitive = null;

		  try
		  {
			jsonPrimitive = jsonObject.getAsJsonPrimitive(memberName);

		  }
		  catch (System.InvalidCastException e)
		  {
			LOG.logJsonException(e);
		  }

		  if (jsonPrimitive != null)
		  {
			rawValue = asPrimitiveObject(jsonPrimitive);

		  }
		}

		if (rawValue != null)
		{
		  return rawValue;

		}
		else
		{
		  return null;

		}
	  }

	  public static object asPrimitiveObject(JsonPrimitive jsonValue)
	  {
		if (jsonValue == null)
		{
		  return null;
		}

		object rawObject = null;

		if (jsonValue.Number)
		{
		  object numberValue = null;

		  try
		  {
			numberValue = jsonValue.AsNumber;

		  }
		  catch (System.FormatException e)
		  {
			LOG.logJsonException(e);
		  }

		  if (numberValue != null && numberValue is LazilyParsedNumber)
		  {
			string numberString = numberValue.ToString();
			if (!string.ReferenceEquals(numberString, null))
			{
			  rawObject = parseNumber(numberString);
			}

		  }
		  else
		  {
			rawObject = numberValue;

		  }
		}
		else
		{ // string, boolean
		  try
		  {
			rawObject = GsonMapper.fromJson(jsonValue, typeof(object));

		  }
		  catch (Exception e) when (e is JsonSyntaxException || e is JsonIOException)
		  {
			LOG.logJsonException(e);
		  }

		}

		if (rawObject != null)
		{
		  return rawObject;

		}
		else
		{
		  return null;

		}
	  }

	  protected internal static Number parseNumber(string numberString)
	  {
		if (string.ReferenceEquals(numberString, null))
		{
		  return null;
		}

		try
		{
		  return int.Parse(numberString);

		}
		catch (System.FormatException)
		{
		}

		try
		{
		  return long.Parse(numberString);

		}
		catch (System.FormatException)
		{
		}

		try
		{
		  return double.Parse(numberString);

		}
		catch (System.FormatException)
		{
		}

		return null;
	  }

	  public static bool getBoolean(JsonObject json, string memberName)
	  {
		if (json != null && !string.ReferenceEquals(memberName, null) && json.has(memberName))
		{
		  try
		  {
			return json.get(memberName).AsBoolean;

		  }
		  catch (Exception e) when (e is System.InvalidCastException || e is System.InvalidOperationException)
		  {
			LOG.logJsonException(e);

			return false;

		  }
		}
		else
		{
		  return false;

		}
	  }

	  public static string getString(JsonObject json, string memberName)
	  {
		if (json != null && !string.ReferenceEquals(memberName, null) && json.has(memberName))
		{
		  return getString(json.get(memberName));

		}
		else
		{
		  return "";

		}
	  }

	  public static string getString(JsonElement jsonElement)
	  {
		if (jsonElement == null)
		{
		  return "";
		}

		try
		{
		  return jsonElement.AsString;

		}
		catch (Exception e) when (e is System.InvalidCastException || e is System.InvalidOperationException)
		{
		  LOG.logJsonException(e);

		  return "";

		}
	  }

	  public static int getInt(JsonObject json, string memberName)
	  {
		if (json != null && !string.ReferenceEquals(memberName, null) && json.has(memberName))
		{
		  try
		  {
			return json.get(memberName).AsInt;

		  }
		  catch (Exception e) when (e is System.InvalidCastException || e is System.InvalidOperationException)
		  {
			LOG.logJsonException(e);

			return 0;

		  }
		}
		else
		{
		  return 0;

		}
	  }

	  public static bool isNull(JsonObject jsonObject, string memberName)
	  {
		if (jsonObject != null && !string.ReferenceEquals(memberName, null) && jsonObject.has(memberName))
		{
		  return jsonObject.get(memberName).JsonNull;

		}
		else
		{
		  return false;

		}
	  }

	  public static long getLong(JsonObject json, string memberName)
	  {
		if (json != null && !string.ReferenceEquals(memberName, null) && json.has(memberName))
		{
		  try
		  {
			return json.get(memberName).AsLong;

		  }
		  catch (Exception e) when (e is System.InvalidCastException || e is System.InvalidOperationException)
		  {
			LOG.logJsonException(e);

			return 0L;

		  }
		}
		else
		{
		  return 0L;

		}
	  }

	  public static JsonArray getArray(JsonObject json, string memberName)
	  {
		if (json != null && !string.ReferenceEquals(memberName, null) && json.has(memberName))
		{
		  return getArray(json.get(memberName));

		}
		else
		{
		  return createArray();

		}
	  }

	  public static JsonArray getArray(JsonElement json)
	  {
		if (json != null && json.JsonArray)
		{
		  return json.AsJsonArray;

		}
		else
		{
		  return createArray();

		}
	  }

	  public static JsonObject getObject(JsonObject json, string memberName)
	  {
		if (json != null && !string.ReferenceEquals(memberName, null) && json.has(memberName))
		{
		  return getObject(json.get(memberName));

		}
		else
		{
		  return createObject();

		}
	  }

	  public static JsonObject getObject(JsonElement json)
	  {
		if (json != null && json.JsonObject)
		{
		  return json.AsJsonObject;

		}
		else
		{
		  return createObject();

		}
	  }

	  public static JsonObject createObject()
	  {
		return new JsonObject();

	  }

	  public static JsonArray createArray()
	  {
		return new JsonArray();

	  }

	  public static Gson GsonMapper
	  {
		  get
		  {
			return gsonMapper;
		  }
	  }

	  public static Gson createGsonMapper()
	  {
		return (new GsonBuilder()).serializeNulls().registerTypeAdapter(typeof(System.Collections.IDictionary), new JsonDeserializerAnonymousInnerClass())
		 .create();
	  }

	  private class JsonDeserializerAnonymousInnerClass : JsonDeserializer<IDictionary<string, object>>
	  {
		  public IDictionary<string, object> deserialize(JsonElement json, Type typeOfT, JsonDeserializationContext context)
		  {

			IDictionary<string, object> map = new Dictionary<string, object>();

			foreach (KeyValuePair<string, JsonElement> entry in getObject(json).entrySet())
			{
			  if (entry != null)
			  {
				string key = entry.Key;
				JsonElement jsonElement = entry.Value;

				if (jsonElement != null && jsonElement.JsonNull)
				{
				  map[key] = null;

				}
				else if (jsonElement != null && jsonElement.JsonPrimitive)
				{

				  object rawValue = asPrimitiveObject((JsonPrimitive) jsonElement);
				  if (rawValue != null)
				  {
					map[key] = rawValue;

				  }
				}
			  }
			}

			return map;
		  }
	  }

	}

}